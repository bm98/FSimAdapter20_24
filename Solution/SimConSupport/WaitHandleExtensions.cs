using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimConSupport
{
  /// <summary>
  /// WaitHandle Extension to Wait for a Handle to be signaled, timedout or cancelled
  /// </summary>
  public static class WaitHandleExtensions
  {

    /*
        var manualEvent = new ManualResetEvent(false);
        // ... some asynchronous operation that will signal manualEvent ...
        await manualEvent.WaitOneAsync();
        // The code will pause here until manualEvent is signaled   
     */
    // Derived from:
    // https://thomaslevesque.com/2015/06/04/async-and-cancellation-support-for-wait-handles/
    // THANK YOU

    /// <summary>
    /// Asynch Wait for a WaitHandle signal or timeout or cancellation
    /// </summary>
    /// <param name="wHandle">A WaitHandle to wait for</param>
    /// <param name="millisecondsTimeout">Timeout in ms</param>
    /// <param name="cancellationToken">A Cancel Token</param>
    /// <returns>True if the WaitHandle was signaled</returns>
    /// <exception cref="ArgumentException">If the WaitHandle is null</exception>
    public static async Task<bool> WaitOneAsync( this WaitHandle wHandle, int millisecondsTimeout, CancellationToken cancellationToken )
    {
      // sanity
      if (wHandle == null) throw new ArgumentException( "WaitHandle 'wHandle' cannot be null" );

      RegisteredWaitHandle registeredHandle = null;
      CancellationTokenRegistration tokenRegistration = default;
      try {
        var tcs = new TaskCompletionSource<bool>( );
        registeredHandle = ThreadPool.RegisterWaitForSingleObject(
            wHandle,
            ( state, timedOut ) => {
              ((TaskCompletionSource<bool>)state).TrySetResult( !timedOut ); // returns true when signaled
            },
            tcs,
            millisecondsTimeout,
            true );

        // register cancellation if the token supports it
        if (cancellationToken.CanBeCanceled) {
          tokenRegistration = cancellationToken.Register(
            state => ((TaskCompletionSource<bool>)state).TrySetCanceled( ),
            tcs );
        }
        // we wait for the tcs task result or cancelled
        return await tcs.Task;
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch (OperationCanceledException ocEx) {
        ; // called when cancelled
      }
      catch (Exception ex) {
        ; // other ?? Exceptions 
      }
#pragma warning restore CS0168 // Variable is declared but never used
      finally {
        // clean up to prevent leaks
        registeredHandle?.Unregister( null );
        tokenRegistration.Dispose( );
      }
      return false;
    }

    // variants to use

    /// <summary>
    /// Asynch Wait for a WaitHandle signal or timeout or cancellation
    /// </summary>
    /// <param name="handle">A WaitHandle to wait for</param>
    /// <param name="timeout">Timeout TimeSpan</param>
    /// <param name="cancellationToken">A Cancel Token</param>
    /// <returns>True if the WaitHandle was signaled</returns>
    /// <exception cref="ArgumentException">If the WaitHandle is null</exception>
    public static Task<bool> WaitOneAsync( this WaitHandle handle, TimeSpan timeout, CancellationToken cancellationToken )
    {
      return handle.WaitOneAsync( (int)timeout.TotalMilliseconds, cancellationToken );
    }

    /// <summary>
    /// Asynch Wait for a WaitHandle signal or cancellation
    /// </summary>
    /// <param name="handle">A WaitHandle to wait for</param>
    /// <param name="cancellationToken">A Cancel Token</param>
    /// <returns>True if the WaitHandle was signaled</returns>
    /// <exception cref="ArgumentException">If the WaitHandle is null</exception>
    public static Task<bool> WaitOneAsync( this WaitHandle handle, CancellationToken cancellationToken )
    {
      return handle.WaitOneAsync( Timeout.Infinite, cancellationToken );
    }

    /// <summary>
    /// Asynch Wait for a WaitHandle signal or timeout
    /// </summary>
    /// <param name="handle">A WaitHandle to wait for</param>
    /// <param name="timeout">Timeout TimeSpan</param>
    /// <returns>True if the WaitHandle was signaled</returns>
    /// <exception cref="ArgumentException">If the WaitHandle is null</exception>
    public static Task<bool> WaitOneAsync( this WaitHandle handle, TimeSpan timeout )
    {
      return handle.WaitOneAsync( (int)timeout.TotalMilliseconds, default );
    }

    /*  Waits indefinitely - not provided at this time
    /// <summary>
    /// Asynch Wait for a WaitHandle signal (
    /// </summary>
    /// <param name="handle">A WaitHandle to wait for</param>
    /// <returns>True if the WaitHandle was signaled</returns>
    /// <exception cref="ArgumentException">If the WaitHandle is null</exception>
    public static Task<bool> WaitOneAsync( this WaitHandle handle )
    {
      return handle.WaitOneAsync( Timeout.Infinite, default );
    }
    */

  }
}
