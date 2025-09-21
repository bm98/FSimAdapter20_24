using System;
using System.Diagnostics;
using System.Threading;

using NLog;

using ThreadState = System.Threading.ThreadState;

namespace SimConSupport
{
  /// <summary>
  /// A more generic SignalMonitor for a WaitHandle
  ///  it will perform an Action when signaled
  ///  
  ///  Runs the monitor in a Thread from ThreadPool
  ///  Provides the monitored WaitHandle (ManualResetEvent)
  ///  Provides a mean to start monitoring
  ///  Provides a mean to Cancel the thread
  /// </summary>
  public class SignalMonitor : IDisposable
  {
    /*
     Usage:

      // alloc a monitor
      private readonly SignalMonitor _signalMonitor;

      // create a new monitor
      _signalMonitor = new SignalMonitor( );

      // instead of the WIN message handling use a WaitHandle to create SimConnect
      _simConnect = new FS.SimConnect( "MY_NAME", IntPtr.Zero, 0, _signalMonitor.WaitForHandle, 0 );
       
      // Start waiting for responses
      if (!_signalMonitor.StartSignalMonitor( SignaledAction )) {
        // monitoring does not work - we will never get replies
        throw new ApplicationException( "Starting the monitoring thread failed" );
      }

      // cancel the SignalMonitoring Thread when needed
      _signalMonitor.CancelMonitoring( );

      // then Start again (see above)

      // dispose when finished
      _signalMonitor?.Dispose();
      

      // provides the Action to be done when signaled
      private void SignaledAction()
      {
        // don't let exceptions back into the monitor 
        try {
          // do what is needed
        }
        catch {} 
      }

     */

    private static readonly Logger LOG = LogManager.GetCurrentClassLogger( );

    // Monitor loop timeout to check for '_killThread' trigger - better use Cancel...
    private const double c_waitTimeout_sec = 5.0;

    // Event which is monitored
    private readonly ManualResetEvent _monitoredEvent = null;
    // cancellation support
    private CancellationTokenSource _cancelSrc;
    private CancellationToken _cancelToken;

    // brute force trigger to stop the thread
    private bool _killThread = false;

    // the thread needs to set this when created and set to null when stopped
    private Thread _monitorThreadRef = null;

    /// <summary>
    /// The EventHandle to wait for
    /// </summary>
    public WaitHandle WaitForHandle => _monitoredEvent;

    /// <summary>
    /// True if monitoring thread is active
    /// </summary>
    public bool IsMonitoring {
      get {
        var monitor = _monitorThreadRef;
        return (monitor != null) && monitor.IsAlive;
      }
    }

    /// <summary>
    /// Cancel the Monitoring Thread
    /// </summary>
    public void CancelMonitoring( )
    {
      if (!IsMonitoring) return; // not running

      if (_cancelSrc.Token.CanBeCanceled) {
        _cancelSrc.Cancel( );
      }
      else {
        _killThread = true; // try brute force if cancel does not work..
      }
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public SignalMonitor( )
    {
      _killThread = false;
      // create the WaitHandle
      _monitoredEvent = new ManualResetEvent( false );
      _cancelSrc = default;
      _cancelToken = default;
    }

    /// <summary>
    /// Start a thread which execs the SignalMonitor
    ///   Pls. handle Exceptions in the Action submitted
    ///   the monitor will only report and then ignore any exception raised in the action
    /// </summary>
    /// <param name="handleAction">Action to perform when signaled (cannot be null)</param>
    /// <returns>True if monitoring has started else false</returns>
    public bool StartSignalMonitor( Action handleAction )
    {
      // sanity
      if (handleAction == null) throw new ArgumentNullException( "handleAction cannot be null" );

      // see if our thread is alive
      if (IsMonitoring) {
        // a thread is still running
        return false; // cannot start right now
      }

      // prepare for a new run
      _cancelSrc?.Dispose( );

      _killThread = false;
      _cancelSrc = new CancellationTokenSource( );
      _cancelToken = _cancelSrc.Token;
      _monitoredEvent?.Reset( ); // handle the next signal from SimConnect

      try {
        if (!ThreadPool.QueueUserWorkItem( SignalMonitorTask, handleAction )) {
          using (ScopeContext.PushNestedState( "StartSignalMonitor" )) LOG.Error( "could not start the SignalMonitorTask" );
          return false;
        }
        // Thread ends when cancelled or killed
        return true;
      }
      catch (Exception ex) {
        using (ScopeContext.PushNestedState( "StartSignalMonitor" )) LOG.Error( ex, "QueueUserWorkItem - Exception" );
      }
      return false;
    }

    // Derived from:
    // https://devsupport.flightsimulator.com/t/efficient-message-loop-with-managed-simconnect-in-msfs-2024/16711
    // THANK YOU

    // The SignalMonitor runs in a separate Thread from the ThreadPool (see StartSignalMonitor())
    // Submit the WaitHandle when creating the SimConnect obj (and set the WIN message handling off)

    // This Monitor will wait for the handle be signaled
    //  also waits for a timeout to check if not killed
    //  also waits for a cancellation via _cancelSource (to cleanup)
    //
    // Uses the WaitHandle Extension to WaitOneAsync(handle,...) 
    //  gets the Action from stateInfo submitted when Queued
    private async void SignalMonitorTask( object stateInfo )
    {
      // set our context
      Interlocked.Exchange( ref _monitorThreadRef, Thread.CurrentThread );

      using (ScopeContext.PushNestedState( "SignalMonitorTask" )) LOG.Trace( "SignalMonitorTask has started" );

      if (stateInfo is Action handleAction) {

        bool run = true; // initially run..
        while (run) {

          // Wait for signal or timeout or cancelled
          if (await _monitoredEvent?.WaitOneAsync( TimeSpan.FromSeconds( c_waitTimeout_sec ), _cancelToken )) {
            _monitoredEvent?.Reset( ); // Wait for next signal

            try {
              handleAction?.Invoke( ); // Executes the submitted Action when signaled
            }
            catch (Exception ex) {
              // e.g. cross thread attempt when writing to WinForms controls within the callback without using an Invoker on the Form...
              using (ScopeContext.PushNestedState( "SignalMonitorTask" )) LOG.Error( ex, "WaitOneAsync exception" );
#if DEBUG
              ; // Debug stop
#endif
            }
          }
          else {
            // Signal timeout or cancelled
            // timeout is legit if the handle is not signaled within the timeout period
          }

          // termination criterias - extendable when needed
          run = true;
          run &= !(_killThread == true); // stop if 'killed'
          run &= !(_cancelToken.CanBeCanceled && _cancelToken.IsCancellationRequested); // stop if cancelled

        }// until run becomes false

      }
      else {
        using (ScopeContext.PushNestedState( "SignalMonitorTask" )) LOG.Error( "stateInfo Argument was not an Action" );
#if DEBUG
        // in DEBUG we throw for this program error
        throw new ApplicationException( "stateInfo Argument was not an Action" );
#endif
      }

      Interlocked.Exchange( ref _monitorThreadRef, null );// clear
      using (ScopeContext.PushNestedState( "SignalMonitorTask" )) LOG.Trace( "SignalMonitorTask has terminated" );
    }

    #region DISPOSE

    private bool disposedValue;
    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)

          // while monitoring..
          if (IsMonitoring) {
            // cancel the thread
            _cancelSrc?.Cancel( );
            _killThread = true;
            _monitorThreadRef?.Join( 10_000 ); // wait max 10 sec for termination
          }

          _cancelSrc?.Dispose( );
          _monitoredEvent?.Dispose( );
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~SignalMonitor()
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
  }
  #endregion

}
