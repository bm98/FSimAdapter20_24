using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using NLog;

using FS = MSFSAdapter20_24;

using SimConSupport;
using static SimConSupport.WaitHandleExtensions;

namespace SimConnector
{
  /// <summary>
  /// SimConnect Adapter Utility
  /// 
  ///   wraps the connection and disconnection with SimConnect via the Adapter
  ///   handles SimConnect Receive using a WaitHandle (instead of the WIN Message loop)
  ///   
  ///   After an initial Connect call it attempts to connect with a 5sec pacer 
  ///     until the connection is confirmed 
  ///   Attempts to reconnect in case the Sim drops the connection until the Disconnect call is made
  ///    (the SimConnectRef will no longer be valid or null until the connection is confirmed again)
  ///   Fires events while doing so (see Interface)
  ///   Provides a reference to the connected SimConnect (Adapter)
  ///   
  /// </summary>
  public class SimConWH : ISimCon
  {
    private static readonly Logger LOG = LogManager.GetCurrentClassLogger( );

    // used to ID Debug Out and ref to this class
    private const string c_myName = "SimConWH";

    // Pacer to maintain the connection state
    private readonly DispatcherTimer _timer = new DispatcherTimer( );

    private readonly SignalMonitor _signalMonitor;
    private readonly ManualResetEvent _simConnectAccess = null;

    private readonly int _myPID = 0;

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

    // cTor: empty is private
    private SimConWH( ) { }

    /// <summary>
    /// cTor: may provide a monitor pace interval 2..20 sec (default=5)
    /// </summary>
    /// <param name="paceInterval_sec">Monitor Pace interval in seconds (default=5)</param>
    public SimConWH( uint paceInterval_sec = 5 )
    {
      // sanity limit 2..20 sec
      paceInterval_sec = (paceInterval_sec < 2) ? 2 : (paceInterval_sec > 20) ? 20 : paceInterval_sec;

      // track the PID in Console Writes to debug parallel running Libraries
      _myPID = Process.GetCurrentProcess( ).Id;

      _timer.Tick += timer1_Tick;
      _timer.Interval = new TimeSpan( 0, 0, (int)paceInterval_sec ); // default 5sec
      _timer.Stop( );

      // stuff for the Receiver Handling
      _signalMonitor = new SignalMonitor( );

      // Mutual exclusion for SimConnect - if needed 
      _simConnectAccess = new ManualResetEvent( false ); // start waiting until established

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
    private FS.SimConnect _simConnect = null;
    private SimConState _state = SimConState.Idle;

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
      LOG.Trace( "PID<{0}>.Connect_low()", _myPID );

      if (_state == SimConState.Connected) {
        using (ScopeContext.PushNestedState( "Connect_low" )) LOG.Trace( ".. already connected" );
        return true;
      }

      // should not be needed but in case the _connected flag was screwed up..
      _simConnect?.Dispose( );
      _state = SimConState.Disconnected;

      // may fail when SimConnect is not yet or no longer running
      try {
        instanceGUID = Guid.NewGuid( ).ToString( "D" );
        using (ScopeContext.PushNestedState( "Connect_low" )) LOG.Trace( ".. establish connection with SimConnect.." );
        // The constructor is similar to SimConnect_Open in the native API
        _state = SimConState.Connecting;
        // instead of the WIN message handling use a WaitHandle 
        _simConnect = new FS.SimConnect( $"{c_myName}_<{instanceGUID}>", IntPtr.Zero, 0, _signalMonitor.WaitForHandle, 0 );
        if (_simConnect == null) {
          // failed to create the simConnect obj - can it happen?? - should never happen anyway...
          throw new ApplicationException( "Could not create SimConnect object" );
        }
        // if _simConnect is created, start waiting for responses
        if (!_signalMonitor.StartSignalMonitor( SC_ReceiveAction )) {
          // monitoring does not work - we will never get replies
          throw new ApplicationException( "Starting the monitoring thread failed" );
        }

        // mutual exclusion handling - if needed and supported
        _simConnectAccess.Set( ); // simConnect seems to be available

        AttachHandlers( );

        // Init SimConnect with defaults
        _version = _simConnect.Init( );

        if (_version == FS.FSVersion.Unknown) {
          using (ScopeContext.PushNestedState( "Connect_low" )) LOG.Info( "No MSFS App seems running - shutting connection down" );
        }
        else {
          using (ScopeContext.PushNestedState( "Connect_low" )) LOG.Trace( "Detected FSim Version: {0}", _version );

          // about to receive the Open msg from SimConnect now
          OnEstablishing( );

          using (ScopeContext.PushNestedState( "Connect_low" )) LOG.Info( "FS Title: {0} .. waiting for response", _simConnect.FSimWindowTitle );
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

      // cancel the SignalMonitoring Thread
      _signalMonitor?.CancelMonitoring( );

      return false; // wait until the FSim is available
    }

    // wire event handlers we care of
    private void AttachHandlers( )
    {
      LOG.Trace( "PID<{0}>.AttachHandlers()", _myPID );

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
        using (ScopeContext.PushNestedState( "AttachHandlers" )) LOG.Error( ex, "Failed with exception" );
        throw ex;
      }
    }

    // Disconnect from SimConnect - will really disconnect once the last user disconnects
    // Set true if SimConnect called it Quit (default is false)</param>
    private bool Disconnect_low( bool forceQuit = false )
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
    private void ResetSimConHandler( )
    {
      LOG.Trace( "PID<{0}>.ResetSimConHandler()", _myPID );

      // cancel the SignalMonitoring Thread
      _signalMonitor.CancelMonitoring( );

      // whatever would be needed
    }


    // to confirm the connection
    private void ConfirmConnection( )
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

    // wrapper around the Receive Message Action called from the SignalMonitor
    private async void SC_ReceiveAction( )
    {
      // This is called from the SignalMonitor Thread

      if (disposedValue) return;

      // handle exceptions from SimConnect here as the Monitor will report and ignore them

      bool access = true;
      // wait for access if the handle is allocated
      if (_simConnectAccess != null) {
        // a looong timeout to not wait forever if something went wrong
        access = await _simConnectAccess.WaitOneAsync( TimeSpan.FromSeconds( 60 ) );
      }
      // SimConnect Access is signaled or omitted
      if (access) {
        try {
          _simConnect?.ReceiveMessage( ); // Triggers and executes registered callbacks
        }
        catch (COMException ex) {
          // we may expect those if SimConnect is no longer available
          using (ScopeContext.PushNestedState( "SC_ReceiveAction" )) LOG.Trace( ex, "COM exception" );
        }
        catch (Exception ex) {
          // e.g. cross thread attempt when writing to WinForms controls within the callback without using an Invoker on the Form...
          using (ScopeContext.PushNestedState( "SC_ReceiveAction" )) LOG.Error( ex, "Other exception" );
        }
        finally {
          _simConnectAccess?.Set( ); // allow using _simConnect (if defined)
        }
      }
      else {
        // Access timeout - potiential deadlock situation
        using (ScopeContext.PushNestedState( "SC_ReceiveAction" )) LOG.Error( "SimConnect Access timeout - potential deadlock situation" );
        // we could Set the WaitHandle here if not cancelled but then the coding bug will persist...
      }
    }


    // FS confirms connection
    private void SimConnect_OnRecvOpen( FS.SimConnect sender, FS.SIMCONNECT_RECV_OPEN data )
    {
      LOG.Trace( "PID<{0}>.SimConnect_OnRecvOpen()", _myPID );

      _is2024 = data.dwApplicationVersionMajor >= 12;
      _fSAppName = data.szApplicationName;
      _fSAppVersion = $"V{data.dwApplicationVersionMajor}.{data.dwApplicationVersionMinor}.{data.dwApplicationBuildMajor}.{data.dwApplicationBuildMinor}";
      _fSSimConnectVersion = $"SimConV{data.dwSimConnectVersionMajor}.{data.dwSimConnectVersionMinor}.{data.dwSimConnectBuildMajor}.{data.dwSimConnectBuildMinor}";

      using (ScopeContext.PushNestedState( "SimConnect_OnRecvOpen" ))
        LOG.Trace( $"Opened with: ({_fSAppName} V{_fSAppVersion} ({_fSSimConnectVersion}))" );

      _state = SimConState.Connected; // only now we are connected, wait for confirmation
    }


    /// <summary>
    /// The case where the user closes game
    /// </summary>
    private void SimConnect_OnRecvQuit( FS.SimConnect sender, FS.SIMCONNECT_RECV data )
    {
      LOG.Trace( "PID<{0}>.SimConnect_OnRecvQuit()", _myPID );

      Disconnect_low( true ); // Force it, even if users are still connected

      _state = SimConState.ConnectionClosed; // causes an attempt to reconnect
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
          _signalMonitor?.Dispose( );
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
