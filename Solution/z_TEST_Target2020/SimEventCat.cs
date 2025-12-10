using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SimConShared;

namespace z_TEST_Target2020
{
  /// <summary>
  /// Catalog of SimEvents
  ///  as they are the same for Notify and Get we maintain a Catalog
  /// </summary>
  internal static class SimEventCat
  {
    // registered Events
    private static Dictionary<string, SimEvent> _events = new Dictionary<string, SimEvent>( );

    /// <summary>
    /// True if the EventID is known
    /// </summary>
    /// <param name="eventID">An event ID</param>
    /// <returns>True if known</returns>
    public static bool ContainsEId( EventID eventID ) => _events.Values.Any( e => e.EvtId == eventID );

    /// <summary>
    /// Return a SimEvent for use  (can be new or already in use)
    /// </summary>
    /// <param name="eventName">name of the event</param>
    /// <returns>A SimEvent</returns>
    public static SimEvent AddOrGetEvent( string eventName )
    {
      if (string.IsNullOrEmpty( eventName )) return null;

      string regName = eventName.ToUpperInvariant( );
      if (_events.ContainsKey( regName )) {
        return _events[regName];
      }
      var evt = new SimEvent( regName );
      _events.Add( regName, evt );
      return evt;
    }
    /// <summary>
    /// Update the Event Data
    /// </summary>
    /// <param name="eID"></param>
    /// <param name="data"></param>
    public static void Update( EventID eID, uint data )
    {
      var se = _events.Values.FirstOrDefault( e => e.EvtId == eID );
      if (se != default) {
        se.Update( data );
      }
    }

    /// <summary>
    /// Returns a SimEvent with ID or null (default)
    /// </summary>
    /// <param name="eID">A SimEvent ID</param>
    /// <returns>A SimEvent or null</returns>
    public static SimEvent GetEvent( EventID eID ) => _events.Values.FirstOrDefault( e => e.EvtId == eID );


  }


  /// <summary>
  /// Sim Event wrapper
  /// </summary>
  internal class SimEvent
  {
    public string EventName { get; private set; } = "";
    /// <summary>
    /// True is Mapped with SimConnect
    /// </summary>
    public bool Mapped { get; private set; } = false;
    public void SetMapped( ) => Mapped = true;

    public EventID EvtId { get; private set; } = 0;
    public GroupID NotificationGrpID { get; private set; } = 0; // set Group when adding to notification Grp
    public void SetGroup( GroupID group ) => NotificationGrpID = group;

    public uint Value { get; private set; } = 0;
    public void Update( uint data ) => Value = data;

    public SimEvent( string evtName, GroupID grpId = 0 )
    {
      EventName = evtName;
      EvtId = SimConnectIDs.GetEventID( ); // create
      NotificationGrpID = grpId;
    }

    public override string ToString( )
    {
      return $"{EventName} data: <{Value}>";
    }
  }



}
