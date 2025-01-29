using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SimConnector
{
  /// <summary>
  /// Utility enum for client users to monitor the connect state
  /// </summary>
  public enum SimConState
  {
    /// <summary>
    /// Init state, linger
    ///  will cause to reconnect after a while
    /// </summary>
    Idle = 0,

    /// <summary>
    /// State after calling Connect on SimClient
    /// </summary>
    Connecting,

    /// <summary>
    /// State after SimClient raised the Connect Event
    ///  'Established' is signaled when changing to this state
    ///  waiting to confirm the Connection
    /// </summary>
    Connected,

    /// <summary>
    /// Final state after the owner was confirming the connection
    ///  'Connected' is signaled when changing to this state
    /// </summary>
    ConfirmedConnection,

    /// <summary>
    /// State when the SimClient raised the ConnectionClosed Event
    /// or a Connect attempt failed (e.g. Sim was not detected)
    /// </summary>
    ConnectionClosed,

    /// <summary>
    /// State when the owner was disconnecting from the SimClient
    /// or a fatal error was detected
    ///  'Disconnected' is signaled when changing to this state
    /// - will stay until a Connect attempt is made
    /// </summary>
    Disconnected,
  }

  /// <summary>
  /// SimCon interface for clients
  /// </summary>
  public interface ISimCon
  {
    /// <summary>
    /// Fired when the connection is about to be connected
    /// </summary>
    event EventHandler<EventArgs> Establishing;
    /// <summary>
    /// Fired when the connection was established
    /// </summary>
    event EventHandler<EventArgs> Connected;
    /// <summary>
    /// Fired when the connection was closed
    /// </summary>
    event EventHandler<EventArgs> Disconnected;

    /// <summary>
    /// The SimConnect obj ref
    /// </summary>
    MSFSAdapter20_24.SimConnect SimConnectRef { get; }

    /// <summary>
    /// Returns true if we have a confirmed connection
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// The current state of the connector
    /// </summary>
    SimConState SimConState { get; }

    /// <summary>
    /// Connect the Adapter
    ///  will raise Establishing Event when starting to connect
    ///  will raise Connected Event when ready
    /// </summary>
    /// <returns>True if started</returns>
    bool Connect( );

    /// <summary>
    /// Disconnect and Reset the Adapter
    ///  will raise Disconnected Event when done
    /// </summary>
    /// <returns>True if started</returns>
    bool Disconnect( );


  }
}
