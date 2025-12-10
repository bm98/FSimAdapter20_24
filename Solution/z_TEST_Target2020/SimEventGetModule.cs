using System;
using System.Runtime.InteropServices;
using System.Text;

using FS = Microsoft.FlightSimulator.SimConnect;

using SimConShared;

namespace z_TEST_Target2020
{
  /// <summary>
  /// Receiving SimEvents
  /// </summary>
  internal class SimEventGetModule
  {
    //  *** CLASS 
    public GroupID _notificationGID = 0; // set when created


    /// <summary>
    /// Register the Events with SimConnect
    /// </summary>
    /// <param name="simConnect">simConnect obj</param>
    public void RegisterWithSimConnect( FS.SimConnect simConnect, SimEvent simEvent )
    {
      // sanity
      if (simEvent == null) return; // ??? should not happen...
      if (string.IsNullOrEmpty(simEvent.EventName) ) return;

      if (!simEvent.Mapped) {
        // must map
        simConnect?.MapClientEventToSimEvent( simEvent.EvtId, simEvent.EventName );
        simEvent.SetMapped( );
      }

      if (_notificationGID == 0) {
        // set group
        _notificationGID = SimConnectIDs.GetGroupID( );
        simConnect?.SetNotificationGroupPriority( _notificationGID, FS.SimConnect.SIMCONNECT_GROUP_PRIORITY_LOWEST );
      }

      if (simEvent.NotificationGrpID == 0) {
        // must add to group
        simConnect?.AddClientEventToNotificationGroup( _notificationGID, simEvent.NotificationGrpID, false );
      }
    }

    /// <summary>
    /// Request to receive Events
    /// </summary>
    /// <param name="simConnect">simConnect obj</param>
    public void RequestFromSimConnect( FS.SimConnect simConnect, bool on )
    {
      if (on) {
        // in 2020 (at least) strangely we don't receive ClientEvents when the Prio is set to STANDARD or below (STANDARD - 100) delivers
        // so we set it below HIGHEST but still above STANDARD ??!! nothing found online re this behavior...
        // may be due to Sim Modules capture and masking these events ??
        simConnect?.SetNotificationGroupPriority( _notificationGID, FS.SimConnect.SIMCONNECT_GROUP_PRIORITY_HIGHEST + 1000 );
      }
      else {
        // in 2020 (at least) strangely we don't receive ClientEvents when the Prio is set to STANDARD or below (STANDARD - 100) delivers
        // so we set it below HIGHEST but still above STANDARD ??!! nothing found online re this behavior...
        // may be due to Sim Modules capture and masking these events ??
        simConnect?.SetNotificationGroupPriority( _notificationGID, FS.SimConnect.SIMCONNECT_GROUP_PRIORITY_LOWEST );
      }
    }


    /// <summary>
    /// Handle the reception of data from Sim
    /// </summary>
    /// <param name="data">data from Sim</param>
    /// <returns>True when handled</returns>
    public bool HandleSimEvent( FS.SIMCONNECT_RECV_EVENT data )
    {
      if ((data.uGroupID == (uint)_notificationGID) && SimEventCat.ContainsEId( (EventID)data.uEventID )) {
        SimEventCat.Update( (EventID)data.uEventID, data.dwData );
        return true;
      }
      return false; // not for us
    }


  }
}