using System;
using System.Collections.Generic;

namespace SimConnectToolkit.SystemState
{
  /// <summary>
  /// A Catalog of SysStateHandler items
  /// </summary>
  public class SysStateHandlerCat : Dictionary<EventID, SysStateHandler>
  {
    // using from the PrivateID range here
    // the IDs need to be created for each session i.e. will be created with the Catalog
    // and should be used as long as the SimConnect session lasts.

    // pacer Events
    /// <summary>
    /// Per Frame Event
    /// </summary>
    public EventID EVENT_SIM_FRAME { get; } = SimConnectIDs.GetPrivateEventID( );         // subscription text: Frame
    /// <summary>
    /// 1 Sec Event
    /// </summary>
    public EventID EVENT_SIM_1SEC { get; } = SimConnectIDs.GetPrivateEventID( );          // subscription text: 1sec
    /// <summary>
    /// 4 Sec Event
    /// </summary>
    public EventID EVENT_SIM_4SEC { get; } = SimConnectIDs.GetPrivateEventID( );          // subscription text: 4sec
    /// <summary>
    /// 6Hz Event
    /// </summary>
    public EventID EVENT_SIM_6HZ { get; } = SimConnectIDs.GetPrivateEventID( );           // subscription text: 6Hz


    /// <summary>
    /// Request a notification when the aircraft flight dynamics file is changed. 
    /// These files have a.AIR extension.
    /// The filename is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
    /// </summary>
    public EventID EVENT_ACFT_LOAD { get; } = SimConnectIDs.GetPrivateEventID( );          // subscription text: AircraftFileLoaded
    /// <summary>
    /// Request a notification if the user aircraft crashes.
    /// </summary>
    public EventID EVENT_ACFT_CRASH { get; } = SimConnectIDs.GetPrivateEventID( );          // subscription text: Crashed

    /// <summary>
    /// Request a notification when a flight is loaded. 
    /// Note that when a flight is ended, a default flight is typically loaded, so these events will occur when flights 
    /// and missions are started and finished.
    /// The filename of the flight loaded is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
    /// </summary>
    public EventID EVENT_FLIGHT_LOAD { get; } = SimConnectIDs.GetPrivateEventID( );        // subscription text: FlightFileLoaded
    /// <summary>
    /// Request a notification when a flight is saved correctly. 
    /// The filename of the flight saved is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
    /// NOTE: Having this subscribed while the Sim Loads a new flight causes Issued( the sim does not get the new Plane and Plan it seems)
    /// </summary>
    public EventID EVENT_FLIGHT_SAVE { get; } = SimConnectIDs.GetPrivateEventID( );        // subscription text: FlightFileSaved
    /// <summary>
    /// Request a notification when a new flight plan is activated. 
    /// The filename of the activated flight plan is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
    /// </summary>
    public EventID EVENT_FPLAN_ACTIVATE { get; } = SimConnectIDs.GetPrivateEventID( );     // subscription text: FlightPlanActivated
    /// <summary>
    /// Request a notification when the active flight plan is de-activated.
    /// </summary>
    public EventID EVENT_FPLAN_DEACTIVATE { get; } = SimConnectIDs.GetPrivateEventID( );   // subscription text: FlightPlanDeactivated

    /// <summary>
    /// Request a notification when an AI object is added to the simulation. 
    ///   Refer also to the SIMCONNECT_RECV_EVENT_OBJECT_ADDREMOVE structure.
    /// </summary>
    public EventID EVENT_AI_ADDED { get; } = SimConnectIDs.GetPrivateEventID( );          // subscription text: ObjectAdded
    /// <summary>
    /// Request a notification when an AI object is removed from the simulation. 
    ///   Refer also to the SIMCONNECT_RECV_EVENT_OBJECT_ADDREMOVE structure.
    /// </summary>
    public EventID EVENT_AI_REMOVED { get; } = SimConnectIDs.GetPrivateEventID( );          // subscription text: ObjectRemoved

    /// <summary>
    /// Request a notification when the user changes the position of their aircraft through a dialog.
    /// </summary>
    public EventID EVENT_POS_CHANGED { get; } = SimConnectIDs.GetPrivateEventID( );          // subscription text: PositionChanged

    /// <summary>
    /// Request notifications when the flight is running or not, 
    /// </summary>
    public EventID EVENT_SIM_STATE { get; } = SimConnectIDs.GetPrivateEventID( );          // subscription text: Sim

    /// <summary>
    /// The simulator is running. Typically the user is actively controlling the aircraft on the ground or in the air. 
    /// However, in some cases additional pairs of SimStart/SimStop events are sent.For example, when a flight is reset
    /// the events that are sent are SimStop, SimStart, SimStop, SimStart.
    /// Also when a flight is started with the SHOW_OPENING_SCREEN value set to zero,
    /// then an additional SimStart/SimStop pair are sent before a second SimStart event is sent when the scenery is fully loaded. 
    /// The opening screen provides the options to change aircraft, departure airport, and so on.
    /// </summary>
    public EventID EVENT_SIM_START { get; } = SimConnectIDs.GetPrivateEventID( );          // subscription text: SimStart
    /// <summary>
    /// The simulator is not running. Typically the user is loading a flight, navigating the shell or in a dialog.
    /// </summary>
    public EventID EVENT_SIM_STOP { get; } = SimConnectIDs.GetPrivateEventID( );           // subscription text: SimStop

    /// <summary>
    /// Request notifications when the flight is paused or unpaused,
    /// </summary>
    public EventID EVENT_SIM_PAUSE { get; } = SimConnectIDs.GetPrivateEventID( );           // subscription text: Pause

    /// <summary>
    /// Request notifications when the flight is paused or unpaused,
    /// 
    /// #define PAUSE_STATE_FLAG_OFF              0 // No Pause 
    /// #define PAUSE_STATE_FLAG_PAUSE            1 // "full" Pause (sim + traffic + etc...) 
    /// #define PAUSE_STATE_FLAG_PAUSE_WITH_SOUND 2 // FSX Legacy Pause (not used anymore) 
    /// #define PAUSE_STATE_FLAG_ACTIVE_PAUSE     4 // Pause was activated using the "Active Pause" Button 
    /// #define PAUSE_STATE_FLAG_SIM_PAUSE        8 // Pause the player sim but traffic, multi, etc... will still run     
    /// </summary>
    public EventID EVENT_SIM_PAUSE_EX1 { get; } = SimConnectIDs.GetPrivateEventID( );

    /// <summary>
    /// Request a notification when the flight is paused.
    /// </summary>
    public EventID EVENT_SIM_PAUSED { get; } = SimConnectIDs.GetPrivateEventID( );
    /// <summary>
    /// Request a notification when the flight is unpaused.
    /// </summary>
    public EventID EVENT_SIM_UNPAUSED { get; } = SimConnectIDs.GetPrivateEventID( );


    // Request IDs
    /// <summary>
    /// Request the current Aircraft loaded
    /// The filename is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
    /// </summary>
    public EventID REQUEST_ACFT_LOADED { get; } = SimConnectIDs.GetPrivateEventID( );
    /// <summary>
    /// Request the current Flight File (FLT) loaded
    /// The filename is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
    /// </summary>
    public EventID REQUEST_FLIGHT_LOADED { get; } = SimConnectIDs.GetPrivateEventID( );
    /// <summary>
    /// Request the current Flight Plan (PLN) loaded
    /// The filename is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
    /// </summary>
    public EventID REQUEST_FLIGHT_PLAN { get; } = SimConnectIDs.GetPrivateEventID( );
    /// <summary>
    /// Request the current SimState (On or Off)
    /// </summary>
    public EventID REQUEST_SIM_STATE { get; } = SimConnectIDs.GetPrivateEventID( );
    /// <summary>
    /// Request the current state of the Dialog (In Dialog or not)
    /// </summary>
    public EventID REQUEST_SIM_DIALOG { get; } = SimConnectIDs.GetPrivateEventID( );

  }
}
