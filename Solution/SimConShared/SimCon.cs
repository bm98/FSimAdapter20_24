using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Threading;

using NLog;

using FS = Microsoft.FlightSimulator.SimConnect;

using SimConShared;

namespace SimConPrivate
{
  /// <summary>
  /// SimConnect Adapter Utility
  /// 
  ///   wraps the connection and disconnection with SimConnect via the Adapter
  ///   handles the Windows message queue via a hidden WinForms form
  ///   
  ///   After an initial Connect call it attempts to connect with a 5sec pacer 
  ///     until the connection is confirmed 
  ///   Attempts to reconnect in case the Sim drops the connection until the Disconnect call is made
  ///    (the SimConnectRef will no longer be valid or null until the connection is confirmed again)
  ///   Fires events while doing so (see Interface)
  ///   Provides a reference to the connected SimConnect (Adapter)
  ///   
  /// </summary>
  public abstract class SimCon : IDisposable
  {
    private static readonly Logger LOG = LogManager.GetCurrentClassLogger( );

    // used to ID and ref this class
    protected readonly string c_myName = "SimCon2020";

    // Pacer to maintain the connection state
    protected readonly DispatcherTimer _timer = new DispatcherTimer( );

    protected readonly int _myPID = 0;

    // last Window Title of the detected Sim
    protected string _fsWindowTitle = "";

    #region SimConnect Xtended API

    /// <summary>
    /// User-defined Win32 event for SimConnect
    ///   can be used as UserEventWin32 when creating the instance
    /// </summary>
    public const int WM_USER_SIMCONNECT = 0x0402;

    /// <summary>
    /// User-defined Win32 event for SimConnect NONE
    ///   can be used as UserEventWin32 when creating the instance and the Win MsgQueue is no used
    /// </summary>
    public const int WM_USER_NONE = 0x0;

    /// <summary>
    /// The SimVersion set from SimConnect Open call
    /// </summary>
    public FSVersion SimVersion { get; protected set; } = FSVersion.Unknown;

    /// <summary>
    /// The last detected WindowTitle (which incl version info as of today)
    /// </summary>
    public string FSimWindowTitle => _fsWindowTitle;

    /// <summary>
    /// Init just tries to detect a running Sim and returns it,
    /// </summary>
    /// <returns>The detected Sim running or Unknown if no sim found running</returns>
    public FSVersion DetectRunningSim( )
    {
      // get running processes and find the one of either MSFS Sim Exe
      _fsWindowTitle = MSFS.MSFS2024running( );
      if (!string.IsNullOrEmpty( _fsWindowTitle )) return FSVersion.V2024;

      _fsWindowTitle = MSFS.MSFS2020running( );
      if (!string.IsNullOrEmpty( _fsWindowTitle )) return FSVersion.V2020;

      // none
      _fsWindowTitle = "<FS App not found>";
      return FSVersion.Unknown;
    }


    /// <summary>
    /// Fired when the connection is about to be connected
    /// </summary>
    public event EventHandler<EventArgs> Establishing;
    private void OnEstablishing( )
    {
      Establishing?.Invoke( this, new EventArgs( ) );
    }

    /// <summary>
    /// Fired when the connection was established
    /// </summary>
    public event EventHandler<EventArgs> Connected;
    private void OnConnected( )
    {
      Connected?.Invoke( this, new EventArgs( ) );
    }

    /// <summary>
    /// Fired when the connection was closed
    /// </summary>
    public event EventHandler<EventArgs> Disconnected;
    private void OnDisconnected( )
    {
      Disconnected?.Invoke( this, new EventArgs( ) );
    }


    /// <summary>
    ///  SimConnect reference for a valid connection
    ///    can be null when not fully Connected!!!
    ///    will change if a reconnection was successfull
    /// </summary>
    public FS.SimConnect SimConnectRef => _simConnect;

    /// <summary>
    /// Returns true if we have a confirmed connection
    ///   and the SimConnectRef is valid
    /// </summary>
    public bool IsConnected => (_simConnect != null) && (_state == SimConState.ConfirmedConnection);

    /// <summary>
    /// The current state of the connector
    /// </summary>
    public SimConState SimConState => _state;


    // cTor: empty is private
    private SimCon( ) { }

    /// <summary>
    /// cTor: may provide a monitor pace interval 2..20 sec (default=5)
    /// </summary>
    /// <param name="paceInterval_sec">Monitor Pace interval in seconds</param>
    public SimCon( string idName, uint paceInterval_sec )
    {
      c_myName = idName;

      // sanity limit 2..20 sec
      paceInterval_sec = (paceInterval_sec < 2) ? 2 : (paceInterval_sec > 20) ? 20 : paceInterval_sec;

      // track the PID in Console Writes to debug parallel running Libraries
      _myPID = Process.GetCurrentProcess( ).Id;

      _timer.Tick += timer1_Tick;
      _timer.Interval = new TimeSpan( 0, 0, (int)paceInterval_sec ); // default 5sec
      _timer.Stop( );

      // create if needed
      if (_winMsgForm == null) {
        try {
          _winMsgForm = new MsgForm( this );
          _winMsgForm.Show( ); // show hidden and out of displays - most at least
        }
        catch (Exception ex) {
          using (ScopeContext.PushNestedState( "SimCon.cTor" )) LOG.Error( ex, "Creating WinMsgForm failed with exception" );
        }
        // cannot without Form - will fail later...
      }


      // start with Disconnected - will stay there
      _state = SimConState.Disconnected;
    }

    /// <summary>
    /// Connect the Adapter
    ///  will raise Establishing Event when starting to connect
    ///  will raise Connected Event when ready
    /// </summary>
    public bool Connect( )
    {
      LOG.Trace( "PID<{0}>.Connect()", _myPID );

      // must be Disconnected to start a connection
      if (_state != SimConState.Disconnected) return false;

      _state = SimConState.Idle; // the pacer may init the connection
      _timer.Start( );
      return true;
    }

    /// <summary>
    /// Disconnect and Reset the Adapter
    ///  will raise Disconnected Event when done
    /// </summary>
    public bool Disconnect( )
    {
      LOG.Trace( "PID<{0}>.Disconnect()", _myPID );

      _timer.Stop( );

      return Disconnect_low( );
      // state should be Disconnected now
    }

    #endregion

    #region SimConnectClient chores

    // SimConnectClient instance
    protected FS.SimConnect _simConnect = null;
    protected SimConState _state = SimConState.Idle;

    // Form to receive Windows Messages for SimConnect
    protected MsgForm _winMsgForm = null;

    // number of periods to wait for data arrival after connect
    protected const int c_scGracePeriodSet = 10; // N * 5 sec

    // Monitor the Sim Event Handler after Connection
    protected int _scGracePeriod = -1;    // grace period count down

    protected string instanceGUID = "";

    protected FSVersion _version;

    // True when connecting 2024
    protected bool _is2024 = false;
    // The connected FSim Application name from SIMCONNECT_RECV_OPEN
    protected string _fSAppName = "";
    // The connected FSim Application version string from SIMCONNECT_RECV_OPEN
    protected string _fSAppVersion = "";
    // The connected FSim SimConnect version string from SIMCONNECT_RECV_OPEN
    protected string _fSSimConnectVersion = "";


    // Connect to SimConnect
    protected bool Connect_low( )
    {
      LOG.Trace( "PID<{0}>.Connect_low()", _myPID );

      if (_state == SimConState.Connected) {
        using (ScopeContext.PushNestedState( "Connect_low" )) LOG.Trace( ".. already connected" );
        return true;
      }

      // should not be needed but in case the _connected flag was screwed up..
      _simConnect?.Dispose( );
      _state = SimConState.Disconnected;

      // create if needed
      if (_winMsgForm == null) {
        try {
          _winMsgForm = new MsgForm( this );
          _winMsgForm.Show( );
        }
        catch (Exception ex) {
          using (ScopeContext.PushNestedState( "Connect_low" )) LOG.Error( ex, "Creating WinMsgForm failed with exception" );

          return false; // cannot without Form
        }
      }

      // may fail?!
      try {
        instanceGUID = Guid.NewGuid( ).ToString( "D" );
        using (ScopeContext.PushNestedState( "Connect_low" )) LOG.Trace( ".. establish connection with SimConnect.." );
        // The constructor is similar to SimConnect_Open in the native API
        _state = SimConState.Connecting;
        _simConnect = new FS.SimConnect( $"{c_myName}_<{instanceGUID}>", _winMsgForm.Handle, WM_USER_SIMCONNECT, null, 0 );
        if (_simConnect == null) {
          // failed to create the simConnect obj - can it happen?? - should never happen anyway...
          throw new ApplicationException( "Could not create SimConnect object" );
        }
        AttachHandlers( );

        // Init SimConnect with defaults
        _version = DetectRunningSim( );

        if (_version == FSVersion.Unknown) {
          using (ScopeContext.PushNestedState( "Connect_low" )) LOG.Info( "No MSFS App seems running - shutting connection down" );
        }
        else {
          using (ScopeContext.PushNestedState( "Connect_low" )) LOG.Trace( "Detected FSim Version: {0}", _version );

          // about to receive the Open msg from SimConnect now
          OnEstablishing( );

          using (ScopeContext.PushNestedState( "Connect_low" )) LOG.Info( "FS Title: {0} .. waiting for response", FSimWindowTitle );
          return true;
        }

      }
      catch (COMException ex) {
        using (ScopeContext.PushNestedState( "Connect_low" )) LOG.Trace( ex, "Connect failed with COM exception" );
      }
      catch (Exception ex) {
        using (ScopeContext.PushNestedState( "Connect_low" )) LOG.Error( ex, "Connect failed with other exception" );
      }

      _simConnect?.Dispose( );
      _simConnect = null;
      _state = SimConState.ConnectionClosed; // causes to reconnect via pacer

      return false; // wait until the FSim is available
    }

    // wire event handlers we care of
    protected void AttachHandlers( )
    {
      LOG.Trace( "PID<{0}>.AttachHandlers()", _myPID );

      // sanity
      if (_simConnect == null) throw new ApplicationException( "_SC is null" );

      try {
        // Listen to connect and quit msgs
        _simConnect.OnRecvOpen += new FS.SimConnect.RecvOpenEventHandler( SimConnect_OnRecvOpen );
        _simConnect.OnRecvQuit += new FS.SimConnect.RecvQuitEventHandler( SimConnect_OnRecvQuit );
        // Listen to exceptions
        //_simConnect.OnRecvException += new FS.SimConnect.RecvExceptionEventHandler( SimConnect_OnRecvException );

      }
      catch (Exception ex) {
        using (ScopeContext.PushNestedState( "AttachHandlers" )) LOG.Error( ex, "Failed with exception" );
        throw ex;
      }
    }

    // Disconnect from SimConnect - will really disconnect once the last user disconnects
    // Set true if SimConnect called it Quit (default is false)</param>
    protected bool Disconnect_low( bool forceQuit = false )
    {
      LOG.Trace( "PID<{0}>.Disconnect_low(force:{1})", _myPID, forceQuit.ToString( ) );

      // add condition when normal disconnect should be prevented
      if (!forceQuit && (false)) {
        return false; // aborted... state remains 
      }

      ResetSimConHandler( );

      if (_state != SimConState.Connected) {
        _state = SimConState.Disconnected;
        // we were not connected during this call
        OnDisconnected( ); // signal closed before the handle is disposed
        // for lingering stuff
        _simConnect?.Dispose( );
        _simConnect = null;

        return true; // not even connected
      }
      // proceed with disconnect
      using (ScopeContext.PushNestedState( "Disconnect_low" )) LOG.Trace( "Disconnecting now" );
      _state = SimConState.Disconnected;
      OnDisconnected( ); // signal closed before the handle is disposed
      if (_simConnect != null) {
        // this should also remove all wired Events
        _simConnect.Dispose( );
        _simConnect = null;
      }

      using (ScopeContext.PushNestedState( "Disconnect_low" )) LOG.Info( "Disconnected!" );

      return true;
    }


    // internal reset when the connection is closed, so we may start again
    protected void ResetSimConHandler( )
    {
      LOG.Trace( "PID<{0}>.ResetSimConHandler()", _myPID );

      SimVersion = FSVersion.Unknown;

      // whatever would be needed
    }


    // to confirm the connection
    protected void ConfirmConnection( )
    {
      // only if not yet there...
      if (_state != SimConState.ConfirmedConnection) {
        using (ScopeContext.PushNestedState( "ConfirmConnection" )) LOG.Trace( "Connection confirmed" );
        _state = SimConState.ConfirmedConnection;
        OnConnected( );
      }
    }


    /// <summary>
    /// Timer Event
    /// </summary>
    protected void timer1_Tick( object sender, EventArgs e )
    {
      // Call SimConnectPacer
      SimConnectPacer( );
    }

    /// <summary>
    /// SimConnect chores on a timer, mostly reconnecting and monitoring the connection status
    /// Intended to be called about every 5 seconds
    /// </summary>
    protected void SimConnectPacer( )
    {
      /*
       * Idle:          Try to Reconnect
       * Connecting:    Wait for Connected
       * Connected:     Disconnect if not confirmed during grace period
       * Confirmed:     Stay there
       * Closed:        Set Idle
       * Disconnected:  Stay there and wait for the caller to Connect again
       */
      switch (_state) {
        case SimConState.Idle:
          using (ScopeContext.PushNestedState( "SimConnectPacer" )) LOG.Trace( "Connecting now" );
          // try to connect
          if (Connect_low( )) {
            _scGracePeriod = c_scGracePeriodSet;
            // waiting to be connected
            using (ScopeContext.PushNestedState( "SimConnectPacer" )) LOG.Trace( "Success - waiting for confirmation" );
          }
          else {
            // connect failed - will be retried through the pacer
            _state = SimConState.Idle;
          }
          break;

        case SimConState.Connecting:
          // wait to be connected
          break;

        case SimConState.Connected:
          // wait for the connection to be confirmed i.e. able to interact with SimConnect

          // handle the situation where Sim is connected but could not hookup to events
          // Sometimes the Connection is made but was not hooking up to the event handling

          // Disconnect and try to reconnect 
          if (_scGracePeriod <= 0) {
            using (ScopeContext.PushNestedState( "SimConnectPacer" )) LOG.Info( "Waiting for confirmation - grace period expired" );
            // grace period is expired !
            using (ScopeContext.PushNestedState( "SimConnectPacer" )) LOG.Trace( "Disconnecting now" );
            Disconnect_low( );
            _state = SimConState.Idle; // restart trying during the next pace
          }
          _scGracePeriod--;

          //*** FOR NOW WE JUST Confirm without having interacted with SimConnect
          //***  TODO establish Confirmed
          ConfirmConnection( );

          break;

        case SimConState.ConfirmedConnection:
          // all good.. connection is up
          //OnConnected( ); // signal Connected
          break;

        case SimConState.ConnectionClosed:
          // MSFS dropped SimConnect or signaled we closed the connection
          _state = SimConState.Idle; // restart trying during the next pace
          break;

        case SimConState.Disconnected:
          // we disconnected from SimClient, stay there until someone calls Connect          
          break;
      }

    }


    // FS confirms connection
    protected void SimConnect_OnRecvOpen( FS.SimConnect sender, FS.SIMCONNECT_RECV_OPEN data )
    {
      LOG.Trace( "PID<{0}>.SimConnect_OnRecvOpen()", _myPID );

      _is2024 = data.dwApplicationVersionMajor >= 12;
      _fSAppName = data.szApplicationName;
      _fSAppVersion = $"V{data.dwApplicationVersionMajor}.{data.dwApplicationVersionMinor}.{data.dwApplicationBuildMajor}.{data.dwApplicationBuildMinor}";
      _fSSimConnectVersion = $"SimConV{data.dwSimConnectVersionMajor}.{data.dwSimConnectVersionMinor}.{data.dwSimConnectBuildMajor}.{data.dwSimConnectBuildMinor}";

      SimVersion = _is2024 ? FSVersion.V2024 : FSVersion.V2020;

      using (ScopeContext.PushNestedState( "SimConnect_OnRecvOpen" ))
        LOG.Trace( $"Opened with: ({_fSAppName} V{_fSAppVersion} ({_fSSimConnectVersion}))" );

      _state = SimConState.Connected; // only now we are connected, wait for confirmation
    }

    /// <summary>
    /// The case where the user closes game
    /// </summary>
    protected void SimConnect_OnRecvQuit( FS.SimConnect sender, FS.SIMCONNECT_RECV data )
    {
      LOG.Trace( "PID<{0}>.SimConnect_OnRecvQuit()", _myPID );

      Disconnect_low( true ); // Force it, even if users are still connected

      _state = SimConState.ConnectionClosed; // causes an attempt to reconnect
    }



    /// <summary>
    /// Utility: called by the MsgForms WndProc
    /// 
    /// Handle the complete Windows Message queue callback
    /// 
    /// </summary>
    /// <param name="m">Message</param>
    /// <returns>True when handled</returns>
    internal bool SimConnectWndProc( ref Message m )
    {
      // sanity
      if (disposedValue) return false; // already gone..
      if (m.Msg != WM_USER_SIMCONNECT) return false; // not for us anyway
      if (_simConnect == null) return false; // cannot handle it

      return WinMessageHandled( m.Msg ); // utility provided by the Adapter
    }

    #endregion

    #region WinMSG Handler

    /// <summary>
    /// Windows Message Handling Model when using the provided WM_USER_SIMCONNECT message ID
    /// 
    /// Provide the Message from the Main Message Loop to check and handle SimConnect events
    /// Propagate if not handled here (returned false)
    /// </summary>
    /// <param name="msg">The message ID</param>
    /// <returns>True if handled</returns>
    internal bool WinMessageHandled( int msg )
    {
      // sanity

      // seems legit to handle it
      try {
        if (msg == WM_USER_SIMCONNECT) {
          _simConnect.ReceiveMessage( ); // handle if it seems connected
          return true;
        }
      }
      catch (Exception ex) {
        _ = ex; // DEBUG
      }

      return false;
    }

    #endregion

    #region DISPOSE

    private bool disposedValue;

    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          Disconnect( );
          _winMsgForm?.Dispose( );
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~SimConWH()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose( )
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose( disposing: true );
      GC.SuppressFinalize( this );
    }

    #endregion

  }
}



