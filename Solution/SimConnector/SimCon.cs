using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Threading;

using FS = MSFSAdapter20_24;

namespace SimConnector
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
  public class SimCon : ISimCon
  {

    // Pacer to maintain the connection state
    private readonly DispatcherTimer _timer = new DispatcherTimer( );

    private readonly int m_myPID = 0;
    private readonly string m_myPName = "";

    #region API

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


    /// <summary>
    /// cTor: may provide a monitor pace interval 2..20 sec (default=5)
    /// </summary>
    /// <param name="paceInterval_sec">Monitor Pace interval in seconds (default=5)</param>
    public SimCon( uint paceInterval_sec = 5 )
    {
      // sanity limit 2..20 sec
      paceInterval_sec = (paceInterval_sec < 2) ? 2 : (paceInterval_sec > 20) ? 20 : paceInterval_sec;

      // track the PID in Console Writes to debug parallel running Libraries
      Process currentProcess = Process.GetCurrentProcess( );
      m_myPID = currentProcess.Id;
      m_myPName = currentProcess.ProcessName;

      _timer.Stop( );
      _timer.Tick += timer1_Tick;
      _timer.Interval = new TimeSpan( 0, 0, (int)paceInterval_sec ); // default 5sec

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
      Debug.WriteLine( $"TRACE: {m_myPID:####0} - Connect" );

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
      Debug.WriteLine( $"TRACE: {m_myPID:####0} - Disconnect" );

      _timer.Stop( );

      return Disconnect_low( );
      // state should be Disconnected now
    }

    #endregion

    #region SimConnectClient chores

    // SimConnectClient instance
    private FS.SimConnect _simConnect = null;
    private SimConState _state = SimConState.Idle;

    // Form to receive Windows Messages for SimConnect
    private MsgForm _winMsgForm = null;

    // number of periods to wait for data arrival after connect
    private const int c_scGracePeriodSet = 10; // N * 5 sec

    // Monitor the Sim Event Handler after Connection
    private int _scGracePeriod = -1;    // grace period count down

    private string instanceGUID = "";

    private FS.FSVersion _version;

    // True when connecting 2024
    private bool _is2024 = false;
    // The connected FSim Application name from SIMCONNECT_RECV_OPEN
    private string _fSAppName = "";
    // The connected FSim Application version string from SIMCONNECT_RECV_OPEN
    private string _fSAppVersion = "";
    // The connected FSim SimConnect version string from SIMCONNECT_RECV_OPEN
    private string _fSSimConnectVersion = "";


    // Connect to SimConnect
    private bool Connect_low( )
    {
      Debug.WriteLine( "TRACE: Connect_low..." );

      if (_state == SimConState.Connected) {
        Debug.WriteLine( "TRACE: .. already connected" );
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
          Debug.WriteLine( $"ERROR: Creating WinMsgForm  failed:\n{ex.Message}\n" );

          return false; // cannot without Form
        }
      }

      // may fail?!
      try {
        instanceGUID = Guid.NewGuid( ).ToString( "D" );
        Debug.WriteLine( "TRACE: .. establish connection with SimConnect.." );
        // The constructor is similar to SimConnect_Open in the native API
        _state = SimConState.Connecting;
        _simConnect = new FS.SimConnect( $"FacilityStreamLib<{instanceGUID}>", _winMsgForm.Handle, FS.SimConnect.WM_USER_SIMCONNECT, null, 0 );
        AttachHandlers( );

        // Init SimConnect with defaults
        _version = _simConnect.Init( );
        Debug.WriteLine( $"TRACE: Detected FSim Version: {_version}" );

        if (_version == FS.FSVersion.Unknown) {
          Debug.WriteLine( "INFO: No MSFS App seems running - shutting connection down" );
        }
        else {

          // about to receive the Open msg from SimConnect now
          OnEstablishing( );

          Debug.WriteLine( $"INFO: FS Title: {_simConnect.FSimWindowTitle} .. waiting for response" );
          return true;
        }

      }
      catch (COMException ex) {
        Debug.WriteLine( $"ERROR: Connect failed with exception:\n{ex.Message}\n" );
      }

      _simConnect?.Dispose( );
      _simConnect = null;
      _state = SimConState.ConnectionClosed; // causes to reconnect via pacer

      return false; // wait until the FSim is available
    }

    // wire event handlers we care of
    private void AttachHandlers( )
    {
      Debug.WriteLine( "TRACE: .. AttachHandlers .." );
      // sanity
      if (_simConnect == null) throw new InvalidOperationException( "_SC is null" );

      try {
        // Listen to connect and quit msgs
        _simConnect.OnRecvOpen += new FS.SimConnect.RecvOpenEventHandler( SimConnect_OnRecvOpen );
        _simConnect.OnRecvQuit += new FS.SimConnect.RecvQuitEventHandler( SimConnect_OnRecvQuit );
        // Listen to exceptions
        //_simConnect.OnRecvException += new FS.SimConnect.RecvExceptionEventHandler( SimConnect_OnRecvException );

      }
      catch (Exception ex) {
        Debug.WriteLine( $"ERROR: AttachHandlers: Failed with exception\n{ex.Message}" );
        throw ex;
      }
    }

    // Disconnect from SimConnect - will really disconnect once the last user disconnects
    // Set true if SimConnect called it Quit (default is false)</param>
    private bool Disconnect_low( bool forceQuit = false )
    {
      Debug.WriteLine( $"TRACE: Disconnect(force:{forceQuit})" );

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
      Debug.WriteLine( "TRACE: Disconnecting now" );
      _state = SimConState.Disconnected;
      OnDisconnected( ); // signal closed before the handle is disposed
      if (_simConnect != null) {
        // this should also remove all wired Events
        _simConnect.Dispose( );
        _simConnect = null;
      }

      Debug.WriteLine( "INFO: Disconnected!" );

      return true;
    }


    // internal reset when the connection is closed, so we may start again
    private void ResetSimConHandler( )
    {
      Debug.WriteLine( "TRACE: ResetSimConHandler()" );

      // whatever would be needed
    }


    // to confirm the connection
    private void ConfirmConnection( )
    {
      // only if not yet there...
      if (_state != SimConState.ConfirmedConnection) {
        Debug.WriteLine( $"TRACE: SimClient {m_myPID:####0} - Connection confirmed" );
        _state = SimConState.ConfirmedConnection;
        OnConnected( );
      }
    }


    /// <summary>
    /// Timer Event
    /// </summary>
    private void timer1_Tick( object sender, EventArgs e )
    {
      // Call SimConnectPacer
      SimConnectPacer( );
    }

    /// <summary>
    /// SimConnect chores on a timer, mostly reconnecting and monitoring the connection status
    /// Intended to be called about every 5 seconds
    /// </summary>
    private void SimConnectPacer( )
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
          Debug.WriteLine( $"TRACE: SimCon {m_myPID:####0} - Connecting now" );
          // try to connect
          if (Connect_low( )) {
            _scGracePeriod = c_scGracePeriodSet;
            // waiting to be connected
            Debug.WriteLine( $"TRACE: SimCon {m_myPID:####0} - Success - waiting for confirmation" );
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
            Debug.WriteLine( $"SimClient {m_myPID:####0} - Waiting for confirmation - grace period expired" );
            // grace period is expired !
            Debug.WriteLine( $"TRACE: SimCon {m_myPID:####0} - Disconnecting now" );
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
    private void SimConnect_OnRecvOpen( FS.SimConnect sender, FS.SIMCONNECT_RECV_OPEN data )
    {
      _is2024 = data.dwApplicationVersionMajor >= 12;
      _fSAppName = data.szApplicationName;
      _fSAppVersion = $"V{data.dwApplicationVersionMajor}.{data.dwApplicationVersionMinor}.{data.dwApplicationBuildMajor}.{data.dwApplicationBuildMinor}";
      _fSSimConnectVersion = $"SimConV{data.dwSimConnectVersionMajor}.{data.dwSimConnectVersionMinor}.{data.dwSimConnectBuildMajor}.{data.dwSimConnectBuildMinor}";

      Debug.WriteLine( $"TRACE: SimConnect Open: ({_fSAppName} V{_fSAppVersion} ({_fSSimConnectVersion}))\n" );

      _state = SimConState.Connected; // only now we are connected, wait for confirmation
    }

    /// <summary>
    /// The case where the user closes game
    /// </summary>
    private void SimConnect_OnRecvQuit( FS.SimConnect sender, FS.SIMCONNECT_RECV data )
    {
      Debug.WriteLine( $"TRACE: SimClient {m_myPID:####0} - SimConnect_OnRecvQuit" );
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
      if (m.Msg != FS.SimConnect.WM_USER_SIMCONNECT) return false; // not for us anyway
      if (_simConnect == null) return false; // cannot handle it

      return _simConnect.WinMessageHandled( m.Msg ); // utility provided by the Adapter
    }

    #endregion

  }
}
