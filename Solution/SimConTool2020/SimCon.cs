using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Threading;

using NLog;

namespace SimConTool2020
{
  /// <summary>
  /// SimConnect Adapter Utility  FS2020 build
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
  public class SimCon : SimConPrivate.SimCon
  {

    /// <summary>
    /// cTor: may provide a monitor pace interval 2..20 sec (default=5)
    /// </summary>
    /// <param name="paceInterval_sec">Monitor Pace interval in seconds (default=5)</param>
    public SimCon( uint paceInterval_sec = 5 )
      : base( "SimCon2020", paceInterval_sec )
    {

    }


  }
}
