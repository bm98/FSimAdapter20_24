﻿using System;

using FS = MSFSAdapter20_24;

namespace SimConnectToolkit.SystemState
{
  /// <summary>
  /// Event and Request Helper
  ///  can Subscribe, Unsubscribe and Request from SimConnect SystemState
  ///  
  /// </summary>
  public class SysStateHandler
  {
    /// <summary>
    /// Factory method:
    ///  Returns the default SystemState catalog
    /// </summary>
    /// <param name="simConnect">The simConnect handle to execute items with</param>
    /// <returns>A populated SysStateCat</returns>
    public static SysStateHandlerCat DefaultSysStateCatalog( FS.SimConnect simConnect )
    {
      var scat = new SysStateHandlerCat( );

      // Init the Subscription and Request Catalog
      scat.Add( scat.EVENT_SIM_1SEC, new SysStateHandler( simConnect, scat.EVENT_SIM_1SEC, "1sec", true, false ) );
      scat.Add( scat.EVENT_SIM_4SEC, new SysStateHandler( simConnect, scat.EVENT_SIM_4SEC, "4sec", true, false ) );
      scat.Add( scat.EVENT_SIM_6HZ, new SysStateHandler( simConnect, scat.EVENT_SIM_6HZ, "6Hz", true, false ) );
      scat.Add( scat.EVENT_SIM_FRAME, new SysStateHandler( simConnect, scat.EVENT_SIM_FRAME, "Frame", true, false ) );

      scat.Add( scat.EVENT_ACFT_LOAD, new SysStateHandler( simConnect, scat.EVENT_ACFT_LOAD, "AircraftLoaded", true, false ) ); // reply FILENAME
      scat.Add( scat.EVENT_ACFT_CRASH, new SysStateHandler( simConnect, scat.EVENT_ACFT_CRASH, "Crashed", true, false ) ); // reply FILENAME

      scat.Add( scat.EVENT_FLIGHT_LOAD, new SysStateHandler( simConnect, scat.EVENT_FLIGHT_LOAD, "FlightLoaded", true, false ) ); // reply FILENAME
      scat.Add( scat.EVENT_FLIGHT_SAVE, new SysStateHandler( simConnect, scat.EVENT_FLIGHT_SAVE, "FlightSaved", true, false ) );  // reply FILENAME

      scat.Add( scat.EVENT_FPLAN_ACTIVATE, new SysStateHandler( simConnect, scat.EVENT_FPLAN_ACTIVATE, "FlightPlanActivated", true, false ) );     // reply FILENAME
      scat.Add( scat.EVENT_FPLAN_DEACTIVATE, new SysStateHandler( simConnect, scat.EVENT_FPLAN_DEACTIVATE, "FlightPlanDeactivated", true, false ) ); // reply FILENAME

      scat.Add( scat.EVENT_AI_ADDED, new SysStateHandler( simConnect, scat.EVENT_AI_ADDED, "ObjectAdded", true, false ) );
      scat.Add( scat.EVENT_AI_REMOVED, new SysStateHandler( simConnect, scat.EVENT_AI_REMOVED, "ObjectRemoved", true, false ) );

      scat.Add( scat.EVENT_POS_CHANGED, new SysStateHandler( simConnect, scat.EVENT_POS_CHANGED, "PositionChanged", true, false ) );

      scat.Add( scat.EVENT_SIM_STATE, new SysStateHandler( simConnect, scat.EVENT_SIM_STATE, "Sim", true, false ) );
      scat.Add( scat.EVENT_SIM_START, new SysStateHandler( simConnect, scat.EVENT_SIM_START, "SimStart", true, false ) ); // multiple SimStart, SimStop sequ are usual before entering a flight
      scat.Add( scat.EVENT_SIM_STOP, new SysStateHandler( simConnect, scat.EVENT_SIM_STOP, "SimStop", true, false ) );

      scat.Add( scat.EVENT_SIM_PAUSE, new SysStateHandler( simConnect, scat.EVENT_SIM_PAUSE, "Pause", true, false ) ); // state in dwData (1=paused, 0=not paused)
      scat.Add( scat.EVENT_SIM_PAUSE_EX1, new SysStateHandler( simConnect, scat.EVENT_SIM_PAUSE_EX1, "Pause_EX1", true, false ) ); // detailed state in dwData
      scat.Add( scat.EVENT_SIM_PAUSED, new SysStateHandler( simConnect, scat.EVENT_SIM_PAUSED, "Paused", true, false ) );
      scat.Add( scat.EVENT_SIM_UNPAUSED, new SysStateHandler( simConnect, scat.EVENT_SIM_UNPAUSED, "Unpaused", true, false ) );

      // SystemState Requests
      scat.Add( scat.REQUEST_ACFT_LOADED, new SysStateHandler( simConnect, scat.REQUEST_ACFT_LOADED, "AircraftLoaded", false, true ) ); // reply FILENAME
      scat.Add( scat.REQUEST_FLIGHT_LOADED, new SysStateHandler( simConnect, scat.REQUEST_FLIGHT_LOADED, "FlightLoaded", false, true ) ); // reply FILENAME
      scat.Add( scat.REQUEST_FLIGHT_PLAN, new SysStateHandler( simConnect, scat.REQUEST_FLIGHT_PLAN, "FlightPlan", false, true ) ); // reply FILENAME
      scat.Add( scat.REQUEST_SIM_STATE, new SysStateHandler( simConnect, scat.REQUEST_SIM_STATE, "Sim", false, true ) );
      scat.Add( scat.REQUEST_SIM_DIALOG, new SysStateHandler( simConnect, scat.REQUEST_SIM_DIALOG, "DialogMode", false, true ) );

      return scat;
    }

    // Workaround Asobo bug when having a Flight /Plan Subscription and not in a Flight
    //  has crashed or not loaded the correct aircraft in the past (still ???)

    // usage Flag:
    // set true to use SimConnect_SetSystemEventState(event, ON or OFF)
    // set false to use Sub, Unsub
    private readonly bool c_useDisable = false; // false=Use Sub and Unsub (worked so far...)

    // simConnect obj to exec with
    private readonly FS.SimConnect _SC = null;

    /// <summary>
    /// The Event ID
    /// </summary>
    public EventID EId { get; private set; } = EventID.Dummy;

    /// <summary>
    /// name to be used with SimConnect Subscription / Request
    /// </summary>
    public string EventName { get; private set; } = "";

    /// <summary>
    /// Subscription Supported Flag 
    /// </summary>
    public bool CanSubscribe { get; private set; } = false;
    /// <summary>
    /// Request Supported Flag 
    /// </summary>
    public bool CanRequest { get; private set; } = false;

    /// <summary>
    /// Subscribed Flag 
    /// </summary>
    public bool Subscribed { get; private set; } = false;

    /// <summary>
    /// cTor: Internal, SimConnect is the factory
    /// </summary>
    /// <param name="simConnect">SimConnect handle</param>
    /// <param name="eId">EVENT ID</param>
    /// <param name="name">The item name string</param>
    /// <param name="canSub">Set true for Sub/Unsub item</param>
    /// <param name="canReq">Set true for Request item</param>
    public SysStateHandler( FS.SimConnect simConnect, EventID eId, string name, bool canSub, bool canReq )
    {
      _SC = simConnect;
      EId = eId;
      EventName = name;
      CanSubscribe = canSub;
      CanRequest = canReq;
    }

    /// <summary>
    /// Subscribe a SystemState item
    /// </summary>
    public void Sub( )
    {
      //  
      if (!this.CanSubscribe) return;

      try {
        if (c_useDisable) {
          if (!Subscribed) { _SC?.SetSystemEventState( EId, FS.SIMCONNECT_STATE.ON ); }
        }
        else {
          if (!Subscribed) { _SC?.SubscribeToSystemEvent( EId, EventName ); }
        }
        Subscribed = true;
      }
      catch (Exception ex) {
        _ = ex; // to read it when debugging
        Subscribed = false;
      }
    }
    /// <summary>
    /// UnSubscribe a SystemState item
    /// </summary>
    public void UnSub( )
    {
      //  
      if (!this.CanSubscribe) return;

      try {
        if (c_useDisable) {
          if (Subscribed) { _SC?.SetSystemEventState( EId, FS.SIMCONNECT_STATE.OFF ); }
        }
        else {
          if (Subscribed) { _SC?.UnsubscribeFromSystemEvent( EId ); }
        }
        Subscribed = false;
      }
      catch (Exception ex) {
        _ = ex; // to read it when debugging
        Subscribed = false;
      }
    }
    /// <summary>
    /// Request REPLY on a SystemState item
    /// </summary>
    public void Request( )
    {
      //  
      if (!this.CanRequest) return;

      try {
        _SC?.RequestSystemState( EId, EventName );
      }
      catch (Exception ex) {
        _ = ex; // to read it when debugging
      }
    }
  }
}
