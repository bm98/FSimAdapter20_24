using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FS = MSFSAdapter20_24;

using MSFSAdapter20_24;

namespace z_TEST_MinCompliance
{
  public partial class Form1 : Form
  {
    // static only because it's easier...
    private static SimConnect _SC = null;

    private FSVersion _version = FSVersion.Unknown;

    // Catalog of supported Event Subscriptions and Requests
    private Dictionary<EventID, EventHelper> _eventCat;

    // primitive connect and dispose
    private void ConnectAndDispose( )
    {
      _SC?.Dispose( );
      // create an instance without using callbacks
      RTB.Text = "Create SimConnectX without Callbacks\n";

      _SC = new SimConnect( "ADAPTER_TEST_APP", IntPtr.Zero, SimConnect.WM_USER_NONE, null, 0 );

      RTB.Text += "Init Adapter\n";
      _version = _SC.Init( ); // Init default
      RTB.Text += $"Detected: {_version}\n";

      if (_version == FSVersion.Unknown) {
        RTB.Text += $"No MSFS App seems running\n";
      }

      RTB.Text += $"Adapter Shutdown\n";
      _SC?.Dispose( );
      _SC = null;
      RTB.Text += $"Done...\n";
    }

    // Connect for use with Callbacks
    private void Connect( )
    {
      _SC?.Dispose( );
      // create an instance without using callbacks
      RTB.Text = "Create SimConnectX with Callbacks\n";
      // using this Form as WIN MSG receiver and the provided Message ID
      _SC = new SimConnect( "ADAPTER_TEST_APP", this.Handle, SimConnect.WM_USER_SIMCONNECT, null, 0 );
      AttachHandlers( );

      RTB.Text += "Init Adapter\n";
      _version = _SC.Init( ); // Init default
      RTB.Text += $"Detected: {_version}\n";

      if (_version == FSVersion.Unknown) {
        RTB.Text += $"No MSFS App seems running\n";
        RTB.Text += $"Adapter Shutdown\n";
        _SC?.Dispose( );
        _SC = null;
        RTB.Text += $"Done...\n";
        return;
      }

      RTB.Text += $"Adapter is alive\n";
    }

    private void AttachHandlers( )
    {
      // sanity
      if (_SC == null) return;

      try {
        // must be safe to connect even before the SC replied with Open
        _SC.OnRecvOpen += new FS.SimConnect.RecvOpenEventHandler( _simConnect_OnRecvOpen );
        _SC.OnRecvQuit += new FS.SimConnect.RecvQuitEventHandler( _simConnect_OnRecvQuit );
        _SC.OnRecvException += new FS.SimConnect.RecvExceptionEventHandler( _simConnect_OnRecvException );

        // events and request replies
        _SC.OnRecvSystemState += new FS.SimConnect.RecvSystemStateEventHandler( _simConnect_OnRecvSystemState );
        _SC.OnRecvEvent += new FS.SimConnect.RecvEventEventHandler( _simConnect_OnRecvEvent );
        _SC.OnRecvEventFilename += new FS.SimConnect.RecvEventFilenameEventHandler( _simConnectRef_OnRecvEventFilename );

        _SC.OnRecvEventFrame += new FS.SimConnect.RecvEventFrameEventHandler( _simConnect_OnRecvEventFrame );
        _SC.OnRecvSimobjectDataBytype += new FS.SimConnect.RecvSimobjectDataBytypeEventHandler( _simConnect_OnRecvSimobjectDataBytype );
        _SC.OnRecvClientData += new FS.SimConnect.RecvClientDataEventHandler( _simConnect_OnRecvClientData );

        _SC.OnRecvEnumerateInputEvents += new FS.SimConnect.RecvEnumerateInputEventsEventHandler( _simConnect_OnRecvEnumerateInputEvents ); // enum B events
        _SC.OnRecvSubscribeInputEvent += new FS.SimConnect.RecvSubscribeInputEventEventHandler( _simConnect_OnRecvSubscribeInputEvent );// monitor B Events
      }
      catch (Exception ex) {
        RTB.Text += "AttachSimEvents_Base: Failed\n" + ex.Message;
      }

      // Init the Subscription and Request Catalog
      _eventCat = new Dictionary<EventID, EventHelper>( ) {
        // SystemState Subscriptions
        {EVENT_SIM_FRAME, new EventHelper( EVENT_SIM_FRAME, "Frame", true, false ) },
        {EVENT_SIM_6HZ, new EventHelper( EVENT_SIM_6HZ, "6Hz", true, false )},
        { EVENT_SIM_1SEC, new EventHelper( EVENT_SIM_1SEC, "1sec", true, false )},
        { EVENT_SIM_4SEC, new EventHelper( EVENT_SIM_4SEC, "4sec", true, false )},
        // immediately returns current state (1= running, 0= not running), else in dwData the same
        { EVENT_SIM_STATE, new EventHelper( EVENT_SIM_STATE, "Sim", true, false ) },
        { EVENT_SIM_START, new EventHelper( EVENT_SIM_START, "SimStart", true, false ) }, // multiple SimStart, SimStop sequ are usual before entering a flight
        { EVENT_SIM_STOP, new EventHelper( EVENT_SIM_STOP, "SimStop", true, false ) },

        { EVENT_SIM_PAUSE, new EventHelper( EVENT_SIM_PAUSE, "Pause", true, false )}, // state in dwData (1=paused, 0=not paused)
        { EVENT_SIM_PAUSE_EX1, new EventHelper( EVENT_SIM_PAUSE_EX1, "Pause_EX1", true, false ) }, // detailed state in dwData
        { EVENT_SIM_PAUSED, new EventHelper( EVENT_SIM_PAUSED, "Paused", true, false ) },
        { EVENT_SIM_UNPAUSED, new EventHelper( EVENT_SIM_UNPAUSED, "Unpaused", true, false ) },

        { EVENT_ACFT_LOAD, new EventHelper( EVENT_ACFT_LOAD, "AircraftLoaded", true, false ) }, // reply FILENAME

        { EVENT_FLIGHT_LOAD, new EventHelper( EVENT_FLIGHT_LOAD, "FlightLoaded", true, false ) }, // reply FILENAME
        { EVENT_FLIGHT_SAVE, new EventHelper( EVENT_FLIGHT_SAVE, "FlightSaved", true, false )},  // reply FILENAME

        { EVENT_FPLAN_ACTIVATE, new EventHelper( EVENT_FPLAN_ACTIVATE, "FlightPlanActivated", true, false ) },     // reply FILENAME
        { EVENT_FPLAN_DEACTIVATE, new EventHelper( EVENT_FPLAN_DEACTIVATE, "FlightPlanDeactivated", true, false ) }, // reply FILENAME

        // SystemState Requests
        { REQUEST_ACFT_LOADED, new EventHelper( REQUEST_ACFT_LOADED, "AircraftLoaded", false, true ) }, // reply FILENAME
        { REQUEST_SIM_DIALOG, new EventHelper( REQUEST_SIM_DIALOG, "DialogMode", false, true ) },
        { REQUEST_FLIGHT_LOADED, new EventHelper( REQUEST_FLIGHT_LOADED, "FlightLoaded", false, true ) }, // reply FILENAME
        { REQUEST_FLIGHT_PLAN, new EventHelper( REQUEST_FLIGHT_PLAN, "FlightPlan", false, true ) }, // reply FILENAME
        { REQUEST_SIM_STATE, new EventHelper( REQUEST_SIM_STATE, "Sim", false, true ) },
      };
    }

    public Form1( )
    {
      InitializeComponent( );
    }

    private void btConnect_Click( object sender, EventArgs e )
    {
      Connect( );
    }

    private void btConDiscon_Click( object sender, EventArgs e )
    {
      ConnectAndDispose( );
    }

    private void btReqSome_Click( object sender, EventArgs e )
    {
      _eventCat[REQUEST_SIM_STATE].Request( );
      _eventCat[REQUEST_SIM_DIALOG].Request( );
      _eventCat[REQUEST_ACFT_LOADED].Request( );
      _eventCat[REQUEST_FLIGHT_LOADED].Request( );
      _eventCat[REQUEST_FLIGHT_PLAN].Request( );
    }

    #region SimConnect IDs

    // Create System Event IDs
    // pacer Events
    private static readonly EventID EVENT_SIM_FRAME = SimConnectIDs.GetEventID( );           // subscription text: Frame
    private static readonly EventID EVENT_SIM_1SEC = SimConnectIDs.GetEventID( );           // subscription text: 1sec
    private static readonly EventID EVENT_SIM_4SEC = SimConnectIDs.GetEventID( );           // subscription text: 4sec
    private static readonly EventID EVENT_SIM_6HZ = SimConnectIDs.GetEventID( );           // subscription text: 6Hz


    /*
     * Request a notification when a flight is loaded. 
     * Note that when a flight is ended, a default flight is typically loaded, so these events will occur when flights 
     * and missions are started and finished. 
     * The filename of the flight loaded is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
     */
    private static readonly EventID EVENT_FLIGHT_LOAD = SimConnectIDs.GetEventID( );        // subscription text: FlightFileLoaded
    /*
     * Request a notification when a flight is saved correctly. 
     * The filename of the flight saved is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
     * 
     * NOTE: Having this subscribed while the Sim Loads a new flight causes Issued (the sim does not get the new Plane and Plan it seems)
     */
    private static readonly EventID EVENT_FLIGHT_SAVE = SimConnectIDs.GetEventID( );        // subscription text: FlightFileSaved
    /*
     * Request a notification when the aircraft flight dynamics file is changed. 
     * These files have a .AIR extension. 
     * The filename is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
     */
    private static readonly EventID EVENT_ACFT_LOAD = SimConnectIDs.GetEventID( );          // subscription text: AircraftFileLoaded

    /*
     * Request a notification when a new flight plan is activated. 
     * The filename of the activated flight plan is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
     */
    private static readonly EventID EVENT_FPLAN_ACTIVATE = SimConnectIDs.GetEventID( );     // subscription text: FlightPlanActivated
    /*
     * Request a notification when the active flight plan is de-activated.
     */
    private static readonly EventID EVENT_FPLAN_DEACTIVATE = SimConnectIDs.GetEventID( );   // subscription text: FlightPlanDeactivated
    /*
     * Request notifications when the flight is running or not, 
     * and also immediately returns the current state (1 = running or 0 = not running). 
     * The state is returned in the dwData parameter.
     */
    private static readonly EventID EVENT_SIM_STATE = SimConnectIDs.GetEventID( );          // subscription text: Sim
    /*
     * The simulator is running. Typically the user is actively controlling the aircraft on the ground or in the air. 
     * However, in some cases additional pairs of SimStart/SimStop events are sent. For example, when a flight is reset 
     * the events that are sent are SimStop, SimStart, SimStop, SimStart. 
     * Also when a flight is started with the SHOW_OPENING_SCREEN value set to zero, 
     * then an additional SimStart/SimStop pair are sent before a second SimStart event is sent when the scenery is fully loaded. 
     * The opening screen provides the options to change aircraft, departure airport, and so on.
     */
    private static readonly EventID EVENT_SIM_START = SimConnectIDs.GetEventID( );          // subscription text: SimStart
    /*
     * The simulator is not running. Typically the user is loading a flight, navigating the shell or in a dialog.
     */
    private static readonly EventID EVENT_SIM_STOP = SimConnectIDs.GetEventID( );           // subscription text: SimStop

    /*
     * Request a notification when paused
     */
    // Request notifications when the flight is paused or unpaused,
    // and also immediately returns the current pause state (1 = paused or 0 = unpaused).
    // The state is returned in the dwData parameter.
    private static readonly EventID EVENT_SIM_PAUSE = SimConnectIDs.GetEventID( );           // subscription text: Pause

    // Request notifications when the flight is paused or unpaused,
    // and also immediately returns the current pause state with more detail than the regular Pause system event.
    // The state is returned in the dwData parameter, and will be one of the following defines:
    /*
        #define PAUSE_STATE_FLAG_OFF 0 // No Pause 
        #define PAUSE_STATE_FLAG_PAUSE 1 // "full" Pause (sim + traffic + etc...) 
        #define PAUSE_STATE_FLAG_PAUSE_WITH_SOUND 2 // FSX Legacy Pause (not used anymore) 
        #define PAUSE_STATE_FLAG_ACTIVE_PAUSE 4 // Pause was activated using the "Active Pause" Button 
        #define PAUSE_STATE_FLAG_SIM_PAUSE 8 // Pause the player sim but traffic, multi, etc... will still run     
    */
    private static readonly EventID EVENT_SIM_PAUSE_EX1 = SimConnectIDs.GetEventID( );

    // Request a notification when the flight is paused.
    private static readonly EventID EVENT_SIM_PAUSED = SimConnectIDs.GetEventID( );
    // Request a notification when the flight is unpaused.
    private static readonly EventID EVENT_SIM_UNPAUSED = SimConnectIDs.GetEventID( );


    // Request IDs
    private static readonly EventID REQUEST_ACFT_LOADED = SimConnectIDs.GetEventID( );
    private static readonly EventID REQUEST_FLIGHT_LOADED = SimConnectIDs.GetEventID( );
    private static readonly EventID REQUEST_FLIGHT_PLAN = SimConnectIDs.GetEventID( );
    private static readonly EventID REQUEST_SIM_STATE = SimConnectIDs.GetEventID( );
    private static readonly EventID REQUEST_SIM_DIALOG = SimConnectIDs.GetEventID( );

    #endregion

    #region EventHelper Class

    // Helper Class supporting SimConnect Events and Requests
    private class EventHelper
    {
      // Workaround Asobo bug when having a Flight /Plan Subscription and not in a Flight
      //  has crashed or not loaded the correct aircraft in the past (still ???)

      // set true to use SimConnect_SetSystemEventState
      // set false to use Sub, Unsub
      private readonly bool c_useDisable = false; // Use Sub and Unsub (worked so far...)

      // The Event ID
      public EventID EId { get; private set; } = EventID.Dummy;

      // name to be used with SimConnect Subscription / Request
      public string EventName { get; private set; } = "";

      // Subscription Supported Flag 
      public bool CanSubscribe { get; private set; } = false;
      // Request Supported Flag 
      public bool CanRequest { get; private set; } = false;

      // Subscribed Flag 
      public bool Subscribed { get; private set; } = false;

      // cTor:
      public EventHelper( EventID eId, string name, bool canSub, bool canReq )
      {
        this.EId = eId;
        this.EventName = name;
        this.CanSubscribe = canSub;
        this.CanRequest = canReq;
      }

      // Subscribe
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
      // UnSubscribe
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
      // Request 
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

    }// EventHelper

    #endregion

    #region SimConnect Event Handlers

    // usually just forward the callback to internal subscribers

    private bool _scOpen = false;
    private StringBuilder _sb = new StringBuilder( );

    // Handle SimConnect Open Reply
    private void _simConnect_OnRecvOpen( FS.SimConnect sender, FS.SIMCONNECT_RECV_OPEN data )
    {
      _sb.Clear( );
      _sb.AppendLine( "_simConnect_OnRecvOpen" );
      _sb.AppendLine( data.szApplicationName );
      _sb.AppendLine( $"V{data.dwApplicationVersionMajor}.{data.dwApplicationVersionMinor}.{data.dwApplicationBuildMajor}.{data.dwApplicationBuildMinor}" );
      _sb.AppendLine( $"SimConV{data.dwSimConnectVersionMajor}.{data.dwSimConnectVersionMinor}.{data.dwSimConnectBuildMajor}.{data.dwSimConnectBuildMinor}" );

      _scOpen = true;
      RTB.Text += _sb.ToString( );
    }

    // Handle SimConnect Quit Event
    private void _simConnect_OnRecvQuit( FS.SimConnect sender, FS.SIMCONNECT_RECV data )
    {
      _sb.Clear( );
      _sb.AppendLine( "_simConnect_OnRecvQuit" );
      _scOpen = true;
      RTB.Text += _sb.ToString( );
    }

    // Handle SimConnect Exception
    private void _simConnect_OnRecvException( FS.SimConnect sender, FS.SIMCONNECT_RECV_EXCEPTION data )
    {
      _sb.Clear( );
      _sb.AppendLine( "_simConnect_OnRecvException" );
      FS.SIMCONNECT_EXCEPTION eException = (FS.SIMCONNECT_EXCEPTION)data.dwException;
      _sb.AppendLine( $"{eException.ToString( )}" );
      RTB.Text += _sb.ToString( );
    }

    // Handle SimConnect Reply for System State Requests
    private void _simConnect_OnRecvSystemState( FS.SimConnect sender, FS.SIMCONNECT_RECV_SYSTEM_STATE data )
    {
      _sb.Clear( );
      _sb.AppendLine( "_simConnect_OnRecvSystemState" );

      // this is the Sim reply to a SystemState Request
      if (data.dwRequestID == (uint)REQUEST_SIM_STATE) {
        _sb.AppendLine( $"REQUEST_SIM_STATE: {data.dwInteger}" );
      }
      else if (data.dwRequestID == (uint)REQUEST_SIM_DIALOG) {
        _sb.AppendLine( $"REQUEST_SIM_DIALOG: {data.dwInteger}" );
      }
      else if (data.dwRequestID == (uint)REQUEST_ACFT_LOADED) {
        _sb.AppendLine( $"REQUEST_ACFT_LOADED: {data.szString}" );
      }
      // FLT file
      else if (data.dwRequestID == (uint)REQUEST_FLIGHT_LOADED) {
        _sb.AppendLine( $"REQUEST_FLIGHT_LOADED: {data.szString}" );
      }
      // PLN file (not seen this working...)
      else if (data.dwRequestID == (uint)REQUEST_FLIGHT_PLAN) {
        _sb.AppendLine( $"REQUEST_FLIGHT_PLAN: {data.szString}" );
      }
      else {
        _sb.AppendLine( $"Other Request with ID:{data.dwRequestID} " );
      }
      RTB.Text += _sb.ToString( );
    }

    // SimConnect Reply for System State Event Subscription
    private void _simConnect_OnRecvEvent( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT data )
    {
      _sb.Clear( );
      _sb.AppendLine( "_simConnect_OnRecvEvent" );

      if (data.uEventID == (uint)EVENT_SIM_6HZ) {
        _sb.AppendLine( $"EVENT_SIM_6HZ" );
      }
      else if (data.uEventID == (uint)EVENT_SIM_1SEC) {
        _sb.AppendLine( $"EVENT_SIM_1SEC" );
      }
      else if (data.uEventID == (uint)EVENT_SIM_4SEC) {
        _sb.AppendLine( $"EVENT_SIM_4SEC" );
      }

      else if (data.uEventID == (uint)EVENT_SIM_START) {
        _sb.AppendLine( $"EVENT_SIM_START" );
      }
      else if (data.uEventID == (uint)EVENT_SIM_STOP) {
        _sb.AppendLine( $"EVENT_SIM_STOP" );
      }
      else if (data.uEventID == (uint)EVENT_SIM_PAUSE_EX1) {
        _sb.AppendLine( $"EVENT_SIM_PAUSE_EX1 {data.dwData}" );
      }
      else if (data.uEventID == (uint)EVENT_SIM_STATE) {
        _sb.AppendLine( $"EVENT_SIM_STATE {data.dwData}" );
      }

      else if (data.uEventID == (uint)EVENT_SIM_PAUSED) {
        _sb.AppendLine( $"EVENT_SIM_PAUSED " );
      }
      else if (data.uEventID == (uint)EVENT_SIM_UNPAUSED) {
        _sb.AppendLine( $"EVENT_SIM_UNPAUSED " );
      }
      else if (data.uEventID == (uint)EVENT_SIM_PAUSE) {
        // DOES NOT WORK - never called  -- TODO CHEK IN CURRENT SIM
        _sb.AppendLine( $"EVENT_SIM_PAUSE {data.dwData}" );
      }

      else {
        // everything else not handled here
        _sb.AppendLine( $"Other Event with ID: {data.uEventID}" );
      }

      RTB.Text += _sb.ToString( );
    }

    // SimConnect Reply for FileName Subscriptions or Requests
    private void _simConnectRef_OnRecvEventFilename( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_FILENAME data )
    {
      _sb.Clear( );
      _sb.AppendLine( "_simConnectRef_OnRecvEventFilename" );

      if (data.uEventID == (uint)EVENT_ACFT_LOAD) {
        _sb.AppendLine( $"EVENT_ACFT_LOAD {data.szFileName}" );
      }
      // FLT file was loaded
      else if (data.uEventID == (uint)EVENT_FLIGHT_LOAD) {
        _sb.AppendLine( $"EVENT_FLIGHT_LOAD {data.szFileName}" );
      }
      // FLT file was saved 
      else if (data.uEventID == (uint)EVENT_FLIGHT_SAVE) {
        _sb.AppendLine( $"EVENT_FLIGHT_SAVE {data.szFileName}" );
      }
      // PLN file (not seen this working)
      else if (data.uEventID == (uint)EVENT_FPLAN_ACTIVATE) {
        _sb.AppendLine( $"EVENT_FPLAN_ACTIVATE {data.szFileName}" );
      }
      // PLN discarded (not seen this working)
      else if (data.uEventID == (uint)EVENT_FPLAN_DEACTIVATE) {
        _sb.AppendLine( $"EVENT_FPLAN_DEACTIVATE {data.szFileName}" );
      }
      else {
        // not handled here
        _sb.AppendLine( $"Other Event with ID: {data.uEventID}" );
      }

      RTB.Text += _sb.ToString( );
    }

    // SimConnect Frame event arrived
    private void _simConnect_OnRecvEventFrame( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_FRAME data )
    {
      // cannot monitor in RTB - too fast...
      _ = data.fFrameRate;
      _ = data.fSimSpeed;
    }


    // SimConnect reply for request obj by type
    private void _simConnect_OnRecvSimobjectDataBytype( FS.SimConnect sender, FS.SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data )
    {
      _sb.Clear( );
      _sb.AppendLine( "_simConnect_OnRecvSimobjectDataBytype" );

      RTB.Text += _sb.ToString( );
    }

    // SimConnect Client data arrived
    private void _simConnect_OnRecvClientData( FS.SimConnect sender, FS.SIMCONNECT_RECV_CLIENT_DATA data )
    {
      _sb.Clear( );
      _sb.AppendLine( "_simConnect_OnRecvClientData" );

      RTB.Text += _sb.ToString( );
    }

    // SimConnect Enum InputEvents Data arrived
    private void _simConnect_OnRecvEnumerateInputEvents( FS.SimConnect sender, FS.SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS data )
    {
      _sb.Clear( );
      _sb.AppendLine( "_simConnect_OnRecvEnumerateInputEvents" );
      _sb.AppendLine( $"RequestID: {data.dwRequestID} Entry: {data.dwEntryNumber} OutOf: {data.dwOutOf} Got {data.dwArraySize} items" );

      RTB.Text += _sb.ToString( );
    }

    // SimConnect returns data 
    private void _simConnect_OnRecvEnumerateInputEventParams( FS.SimConnect sender, FS.SIMCONNECT_RECV_ENUMERATE_INPUT_EVENT_PARAMS data )
    {
      _sb.Clear( );
      _sb.AppendLine( "_simConnect_OnRecvEnumerateInputEventParams" );

      RTB.Text += _sb.ToString( );
    }

    // SimConnect returns data for a single InputEvent 
    private void _simConnect_OnRecvGetInputEvent( FS.SimConnect sender, FS.SIMCONNECT_RECV_GET_INPUT_EVENT data )
    {
      _sb.Clear( );
      _sb.AppendLine( "_simConnect_OnRecvGetInputEvent" );

      RTB.Text += _sb.ToString( );
    }

    // SimConnect returns data for an InputEvent subscription
    private void _simConnect_OnRecvSubscribeInputEvent( FS.SimConnect sender, FS.SIMCONNECT_RECV_SUBSCRIBE_INPUT_EVENT data )
    {
      _sb.Clear( );
      _sb.AppendLine( "_simConnect_OnRecvSubscribeInputEvent" );

      switch (data.eType) {
        case FS.SIMCONNECT_INPUT_EVENT_TYPE.DOUBLE:
          _sb.AppendLine( $"Got Double {(double)data.Value[0]} for HashItem {data.Hash}" );
          break;
        case FS.SIMCONNECT_INPUT_EVENT_TYPE.STRING:
          _sb.AppendLine( $"Got String {(FS.SimConnect.InputEventString)data.Value[0]} for HashItem {data.Hash}" );
          break;
      }

      RTB.Text += _sb.ToString( );
    }


    /*
    /// <summary>
    /// NOT SUPPORTED BY NOW
    /// Asynch call by FS to submit key events
    /// </summary>
    private   void SimConnect_OnRecvReservedKey( SimConnect sender, SIMCONNECT_RECV_RESERVED_KEY data )
    {
      Log( $"_OnRecvReservedKey: ID {data.dwID} - {data.szReservedKey} - {data.szChoiceReserved}" );
      foreach (var kreq in _keyRequests) {
        if (data.dwID == (uint)kreq.EventId) {
          // inform the caller about the key
          OnKeyArrived( kreq.KeyString, kreq.EventName );
        }
      }
    }

    /// <summary>
    /// NOT SUPPORTED BY NOW
    /// Asynch call by FS having received  Airport Lists
    /// </summary>
    private   void _simConnect_OnRecvAirportList( SimConnect sender, SIMCONNECT_RECV_AIRPORT_LIST data )
    {
      Log( $"_OnRecvAirportList: ID {data.dwID} - {data.dwRequestID} - {data.dwArraySize}" );
    }

    /// <summary>
    /// NOT SUPPORTED BY NOW
    /// Asynch call by FS having received VOR/LOC Lists
    /// </summary>
    private   void _simConnect_OnRecvVorList( SimConnect sender, SIMCONNECT_RECV_VOR_LIST data )
    {
      Log( $"_OnRecvVorList: ID {data.dwID} - {data.dwRequestID} - {data.dwArraySize}" );
    }

    /// <summary>
    /// NOT SUPPORTED BY NOW
    /// Asynch call by FS having received NDB Lists
    /// </summary>
    private   void _simConnect_OnRecvNdbList( SimConnect sender, SIMCONNECT_RECV_NDB_LIST data )
    {
      Log( $"_OnRecvNdbList(: ID {data.dwID} - {data.dwRequestID} - {data.dwArraySize}" );
    }

    /// <summary>
    /// NOT SUPPORTED BY NOW
    /// Asynch call by FS having received Waypoint Lists
    /// </summary>
    private   void _simConnect_OnRecvWaypointList( SimConnect sender, SIMCONNECT_RECV_WAYPOINT_LIST data )
    {
      Log( $"_OnRecvWaypointList: ID {data.dwID} - {data.dwRequestID} - {data.dwArraySize}" );
    }


    /// <summary>
    /// NOT SUPPORTED BY NOW
    /// Asynch call by FS having received WeatherObservation Events
    /// </summary>
    private   void SimConnect_OnRecvEventWeatherObs( SimConnect sender, SIMCONNECT_RECV_WEATHER_OBSERVATION data )
    {
      Log( $"_OnRecvEventWeatherObs: ID {data.dwID} - {data.szMetar}" );

      //if (WeatherHandler.HandleWeatherObservationEvent( data, sender )) return; // handled 
    }
    */

    #endregion


    #region WinProc 

    /// <summary>
    /// Windows Message Handler Override 
    /// - must handle messages from SimConnect otherwise SimConnect does not provide events and callbacks
    /// Using the Adapter provided MessageHandler
    /// </summary>
    protected override void DefWndProc( ref Message m )
    {
      if ((_SC != null) && (_version != FSVersion.Unknown)) {
        try {
          // catch MSFS exits in the worst moment
          if (_SC.WinMessageHandled( m.Msg )) return; // Event was handled
        }
        catch { }
      }
      // everything else goes here
      base.DefWndProc( ref m ); // default handling for this Window
    }

    #endregion

  }
}
