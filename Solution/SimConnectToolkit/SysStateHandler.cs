using System;

using FS = MSFSAdapter20_24;

namespace SimConnectToolkit
{
  /// <summary>
  /// Event and Request Helper
  ///  can Subscribe, Unsubscribe and Request from SimConnect SystemState
  ///  
  /// </summary>
  public class SysStateHandler
  {
    // using from the PrivateID range here

    // pacer Events
    /// <summary>
    /// Per Frame Event
    /// </summary>
    public static readonly EventID EVENT_SIM_FRAME = SimConnectIDs.GetPrivateEventID( );         // subscription text: Frame
    /// <summary>
    /// 1 Sec Event
    /// </summary>
    public static readonly EventID EVENT_SIM_1SEC = SimConnectIDs.GetPrivateEventID( );          // subscription text: 1sec
    /// <summary>
    /// 4 Sec Event
    /// </summary>
    public static readonly EventID EVENT_SIM_4SEC = SimConnectIDs.GetPrivateEventID( );          // subscription text: 4sec
    /// <summary>
    /// 6Hz Event
    /// </summary>
    public static readonly EventID EVENT_SIM_6HZ = SimConnectIDs.GetPrivateEventID( );           // subscription text: 6Hz


    /// <summary>
    /// Request a notification when the aircraft flight dynamics file is changed. 
    /// These files have a.AIR extension.
    /// The filename is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
    /// </summary>
    public static readonly EventID EVENT_ACFT_LOAD = SimConnectIDs.GetPrivateEventID( );          // subscription text: AircraftFileLoaded
    /// <summary>
    /// Request a notification if the user aircraft crashes.
    /// </summary>
    public static readonly EventID EVENT_ACFT_CRASH = SimConnectIDs.GetPrivateEventID( );          // subscription text: Crashed

    /// <summary>
    /// Request a notification when a flight is loaded. 
    /// Note that when a flight is ended, a default flight is typically loaded, so these events will occur when flights 
    /// and missions are started and finished.
    /// The filename of the flight loaded is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
    /// </summary>
    public static readonly EventID EVENT_FLIGHT_LOAD = SimConnectIDs.GetPrivateEventID( );        // subscription text: FlightFileLoaded
    /// <summary>
    /// Request a notification when a flight is saved correctly. 
    /// The filename of the flight saved is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
    /// NOTE: Having this subscribed while the Sim Loads a new flight causes Issued( the sim does not get the new Plane and Plan it seems)
    /// </summary>
    public static readonly EventID EVENT_FLIGHT_SAVE = SimConnectIDs.GetPrivateEventID( );        // subscription text: FlightFileSaved
    /// <summary>
    /// Request a notification when a new flight plan is activated. 
    /// The filename of the activated flight plan is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
    /// </summary>
    public static readonly EventID EVENT_FPLAN_ACTIVATE = SimConnectIDs.GetPrivateEventID( );     // subscription text: FlightPlanActivated
    /// <summary>
    /// Request a notification when the active flight plan is de-activated.
    /// </summary>
    public static readonly EventID EVENT_FPLAN_DEACTIVATE = SimConnectIDs.GetPrivateEventID( );   // subscription text: FlightPlanDeactivated

    /// <summary>
    /// Request a notification when an AI object is added to the simulation. 
    ///   Refer also to the SIMCONNECT_RECV_EVENT_OBJECT_ADDREMOVE structure.
    /// </summary>
    public static readonly EventID EVENT_AI_ADDED = SimConnectIDs.GetPrivateEventID( );          // subscription text: ObjectAdded
    /// <summary>
    /// Request a notification when an AI object is removed from the simulation. 
    ///   Refer also to the SIMCONNECT_RECV_EVENT_OBJECT_ADDREMOVE structure.
    /// </summary>
    public static readonly EventID EVENT_AI_REMOVED = SimConnectIDs.GetPrivateEventID( );          // subscription text: ObjectRemoved

    /// <summary>
    /// Request a notification when the user changes the position of their aircraft through a dialog.
    /// </summary>
    public static readonly EventID EVENT_POS_CHANGED = SimConnectIDs.GetPrivateEventID( );          // subscription text: PositionChanged

    /// <summary>
    /// Request notifications when the flight is running or not, 
    /// </summary>
    public static readonly EventID EVENT_SIM_STATE = SimConnectIDs.GetPrivateEventID( );          // subscription text: Sim

    /// <summary>
    /// The simulator is running. Typically the user is actively controlling the aircraft on the ground or in the air. 
    /// However, in some cases additional pairs of SimStart/SimStop events are sent.For example, when a flight is reset
    /// the events that are sent are SimStop, SimStart, SimStop, SimStart.
    /// Also when a flight is started with the SHOW_OPENING_SCREEN value set to zero,
    /// then an additional SimStart/SimStop pair are sent before a second SimStart event is sent when the scenery is fully loaded. 
    /// The opening screen provides the options to change aircraft, departure airport, and so on.
    /// </summary>
    public static readonly EventID EVENT_SIM_START = SimConnectIDs.GetPrivateEventID( );          // subscription text: SimStart
    /// <summary>
    /// The simulator is not running. Typically the user is loading a flight, navigating the shell or in a dialog.
    /// </summary>
    public static readonly EventID EVENT_SIM_STOP = SimConnectIDs.GetPrivateEventID( );           // subscription text: SimStop

    /// <summary>
    /// Request notifications when the flight is paused or unpaused,
    /// </summary>
    public static readonly EventID EVENT_SIM_PAUSE = SimConnectIDs.GetPrivateEventID( );           // subscription text: Pause

    /// <summary>
    /// Request notifications when the flight is paused or unpaused,
    /// 
    /// #define PAUSE_STATE_FLAG_OFF              0 // No Pause 
    /// #define PAUSE_STATE_FLAG_PAUSE            1 // "full" Pause (sim + traffic + etc...) 
    /// #define PAUSE_STATE_FLAG_PAUSE_WITH_SOUND 2 // FSX Legacy Pause (not used anymore) 
    /// #define PAUSE_STATE_FLAG_ACTIVE_PAUSE     4 // Pause was activated using the "Active Pause" Button 
    /// #define PAUSE_STATE_FLAG_SIM_PAUSE        8 // Pause the player sim but traffic, multi, etc... will still run     
    /// </summary>
    public static readonly EventID EVENT_SIM_PAUSE_EX1 = SimConnectIDs.GetPrivateEventID( );

    /// <summary>
    /// Request a notification when the flight is paused.
    /// </summary>
    public static readonly EventID EVENT_SIM_PAUSED = SimConnectIDs.GetPrivateEventID( );
    /// <summary>
    /// Request a notification when the flight is unpaused.
    /// </summary>
    public static readonly EventID EVENT_SIM_UNPAUSED = SimConnectIDs.GetPrivateEventID( );


    // Request IDs
    /// <summary>
    /// Request the current Aircraft loaded
    /// The filename is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
    /// </summary>
    public static readonly EventID REQUEST_ACFT_LOADED = SimConnectIDs.GetPrivateEventID( );
    /// <summary>
    /// Request the current Flight File (FLT) loaded
    /// The filename is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
    /// </summary>
    public static readonly EventID REQUEST_FLIGHT_LOADED = SimConnectIDs.GetPrivateEventID( );
    /// <summary>
    /// Request the current Flight Plan (PLN) loaded
    /// The filename is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
    /// </summary>
    public static readonly EventID REQUEST_FLIGHT_PLAN = SimConnectIDs.GetPrivateEventID( );
    /// <summary>
    /// Request the current SimState (On or Off)
    /// </summary>
    public static readonly EventID REQUEST_SIM_STATE = SimConnectIDs.GetPrivateEventID( );
    /// <summary>
    /// Request the current state of the Dialog (In Dialog or not)
    /// </summary>
    public static readonly EventID REQUEST_SIM_DIALOG = SimConnectIDs.GetPrivateEventID( );


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
      scat = new SysStateHandlerCat( ) {
        // SystemState Subscriptions
        { EVENT_SIM_1SEC, new SysStateHandler( simConnect,EVENT_SIM_1SEC, "1sec", true, false )},
        { EVENT_SIM_4SEC, new SysStateHandler( simConnect,EVENT_SIM_4SEC, "4sec", true, false )},
        { EVENT_SIM_6HZ, new SysStateHandler(simConnect, EVENT_SIM_6HZ, "6Hz", true, false )},
        { EVENT_SIM_FRAME, new SysStateHandler(simConnect, EVENT_SIM_FRAME, "Frame", true, false ) },

        { EVENT_ACFT_LOAD, new SysStateHandler(simConnect,  EVENT_ACFT_LOAD, "AircraftLoaded", true, false ) }, // reply FILENAME
        { EVENT_ACFT_CRASH, new SysStateHandler(simConnect,  EVENT_ACFT_CRASH, "Crashed", true, false ) }, // reply FILENAME

        { EVENT_FLIGHT_LOAD, new SysStateHandler(simConnect,  EVENT_FLIGHT_LOAD, "FlightLoaded", true, false ) }, // reply FILENAME
        { EVENT_FLIGHT_SAVE, new SysStateHandler(simConnect,  EVENT_FLIGHT_SAVE, "FlightSaved", true, false )},  // reply FILENAME

        { EVENT_FPLAN_ACTIVATE, new SysStateHandler(simConnect,  EVENT_FPLAN_ACTIVATE, "FlightPlanActivated", true, false ) },     // reply FILENAME
        { EVENT_FPLAN_DEACTIVATE, new SysStateHandler(simConnect,    EVENT_FPLAN_DEACTIVATE, "FlightPlanDeactivated", true, false ) }, // reply FILENAME

        { EVENT_AI_ADDED, new SysStateHandler(simConnect,  EVENT_AI_ADDED, "ObjectAdded", true, false ) },
        { EVENT_AI_REMOVED, new SysStateHandler(simConnect,  EVENT_AI_REMOVED, "ObjectRemoved", true, false ) },

        { EVENT_POS_CHANGED, new SysStateHandler(simConnect,  EVENT_POS_CHANGED, "PositionChanged", true, false ) },

        { EVENT_SIM_STATE, new SysStateHandler(simConnect,  EVENT_SIM_STATE, "Sim", true, false ) },
        { EVENT_SIM_START, new SysStateHandler(simConnect,  EVENT_SIM_START, "SimStart", true, false ) }, // multiple SimStart, SimStop sequ are usual before entering a flight
        { EVENT_SIM_STOP, new SysStateHandler(simConnect,  EVENT_SIM_STOP, "SimStop", true, false ) },

        { EVENT_SIM_PAUSE, new SysStateHandler(simConnect,  EVENT_SIM_PAUSE, "Pause", true, false )}, // state in dwData (1=paused, 0=not paused)
        { EVENT_SIM_PAUSE_EX1, new SysStateHandler(simConnect,  EVENT_SIM_PAUSE_EX1, "Pause_EX1", true, false ) }, // detailed state in dwData
        { EVENT_SIM_PAUSED, new SysStateHandler(simConnect,  EVENT_SIM_PAUSED, "Paused", true, false ) },
        { EVENT_SIM_UNPAUSED, new SysStateHandler(simConnect,  EVENT_SIM_UNPAUSED, "Unpaused", true, false ) },

        // SystemState Requests
        { REQUEST_ACFT_LOADED, new SysStateHandler(simConnect,   REQUEST_ACFT_LOADED, "AircraftLoaded", false, true ) }, // reply FILENAME
        { REQUEST_FLIGHT_LOADED, new SysStateHandler(simConnect,  REQUEST_FLIGHT_LOADED, "FlightLoaded", false, true ) }, // reply FILENAME
        { REQUEST_FLIGHT_PLAN, new SysStateHandler(simConnect,  REQUEST_FLIGHT_PLAN, "FlightPlan", false, true ) }, // reply FILENAME
        { REQUEST_SIM_STATE, new SysStateHandler(simConnect,  REQUEST_SIM_STATE, "Sim", false, true ) },
        { REQUEST_SIM_DIALOG, new SysStateHandler(simConnect,  REQUEST_SIM_DIALOG, "DialogMode", false, true ) },
      };

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
