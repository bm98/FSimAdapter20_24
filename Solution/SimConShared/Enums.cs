using System;
using System.Collections.Generic;
using System.Text;

namespace SimConShared
{
  /// <summary>
  /// The FS Version detected
  /// </summary>
  public enum FSVersion
  {
    Unknown = 0,
    V2020,
    V2024,
  }

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

}
