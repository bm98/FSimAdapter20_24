using System;

using FS = MSFSAdapter20_24;

using SimConnectToolkit;

namespace z_TEST_MinCompliance
{
  /// <summary>
  /// Send a KeyEvent to the Sim
  /// </summary>
  internal class SimEventSetModule
  {
    public readonly EventID _keyEvtID = SimConnectIDs.GetEventID( );

    public SimEvent SimEvent;

    public string KeyName { get => (SimEvent != null) ? SimEvent.EventName : ""; }

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="keyName"></param>
    public SimEventSetModule( string keyName )
    {
      SimEvent = SimEventCat.AddOrGetEvent( keyName );
    }

    /// <summary>
    /// Register the Events with SimConnect
    /// </summary>
    /// <param name="simConnect">simConnect obj</param>
    public void RegisterWithSimConnect( FS.SimConnect simConnect )
    {
      // sanity
      if (SimEvent == null) return; // ??? should not happen...

      if (!SimEvent.Mapped) {
        simConnect?.MapClientEventToSimEvent( SimEvent.EvtId, SimEvent.EventName );
        SimEvent.SetMapped( );
      }
    }

    /// <summary>
    /// Send Key with optional Data to the Sim
    /// </summary>
    /// <param name="simConnect">simConnect obj</param>
    public void RequestToSimConnect( FS.SimConnect simConnect, uint data = 0 )
    {
      // sanity
      if (SimEvent == null) return; // ??? should not happen...

      RegisterWithSimConnect( simConnect ); // just in case..

      simConnect?.TransmitClientEvent(
        (uint)FS.SIMCONNECT_SIMOBJECT_TYPE.USER, SimEvent.EvtId, data,
        (GroupID)FS.SimConnect.SIMCONNECT_GROUP_PRIORITY_HIGHEST, FS.SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY
        );

    }


  }
}
