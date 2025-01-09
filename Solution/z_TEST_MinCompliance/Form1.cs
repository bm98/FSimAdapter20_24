using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using FS = MSFSAdapter20_24;

using SimConnectToolkit;

using static SimConnectToolkit.SysStateHandler; // event IDs for the SystemStateHandler

namespace z_TEST_MinCompliance
{
  public partial class Form1 : Form
  {
    // static only because it's easier...
    private static FS.SimConnect _SC = null;

    private FS.FSVersion _version = FS.FSVersion.Unknown;

    // define and Init RequestIDs for planned Requests
    private REQUEST RQ_InputEvents = SimConnectIDs.GetREQUEST( );
    private REQUEST RQ_AptList = SimConnectIDs.GetREQUEST( );
    private REQUEST RQ_WypList = SimConnectIDs.GetREQUEST( );
    private REQUEST RQ_VorList = SimConnectIDs.GetREQUEST( );
    private REQUEST RQ_NdbList = SimConnectIDs.GetREQUEST( );

    // Catalog of supported Event Subscriptions and Requests
    private SysStateHandlerCat _eventCat;


    // primitive connect and dispose
    private bool ConnectAndDispose( )
    {
      bool ret = true; // set false on error !!

      _SC?.Dispose( );
      // create an instance without using callbacks
      RTB.Text = "Create SimConnectX without Callbacks\n";

      _SC = new FS.SimConnect( "ADAPTER_TEST_APP", IntPtr.Zero, FS.SimConnect.WM_USER_NONE, null, 0 );

      RTB.Text += "Init Adapter\n";
      _version = _SC.Init( ); // Init default
      RTB.Text += $"Detected: {_version}\n";

      if (_version == FS.FSVersion.Unknown) {
        RTB.Text += $"No MSFS App seems running\n";
        ret = false; // could not connect...
      }

      // Dispose
      RTB.Text += $"Adapter Shutdown\n";
      _SC?.Dispose( );
      _SC = null;
      RTB.Text += $"Done...\n";

      return ret;
    }


    // Connect for use with Callbacks
    // Note: true only means that a running FSim was found and a new SimConnect(2020 or 2024) instance was created
    //       monitor the Open Callback and further events to see if it is really up and running correctly
    private bool Connect( )
    {
      bool ret = true; // set false on error !!

      try {
        _SC?.Dispose( );
        // create an instance without using callbacks
        RTB.Text = "Create SimConnectX with Callbacks\n";
        // using this Form as WIN MSG receiver and the provided Message ID
        _SC = new FS.SimConnect( "ADAPTER_TEST_APP", this.Handle, FS.SimConnect.WM_USER_SIMCONNECT, null, 0 );

        RTB.Text += "Attach Eventhandlers\n";
        AttachHandlers( ); // may throw Exceptions

        RTB.Text += "Init Adapter\n";
        _version = _SC.Init( ); // Init default
        RTB.Text += $"Detected: {_version}\n";

        if (_version == FS.FSVersion.Unknown) {
          RTB.Text += $"No MSFS App seems running\n";
          RTB.Text += $"Adapter Shutdown\n";
          _SC?.Dispose( );
          _SC = null;
          RTB.Text += $"Done...\n";
          ret = false;
        }
        else {
          RTB.Text += $"Adapter is alive\n";
        }
      }
      catch (Exception ex) {
        RTB.Text += $"Connect: Failed with Exception\n{ex.Message}";
        ret = false;
      }

      return ret;
    }

    // may throw on unexpected errors
    private void AttachHandlers( )
    {
      // sanity
      if (_SC == null) throw new InvalidOperationException( "_SC is null" );

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
        _SC.OnRecvEnumerateInputEventParams += new FS.SimConnect.RecvEnumerateInputEventParamsEventHandler( _simConnect_OnRecvEnumerateInputEventParams );
        _SC.OnRecvGetInputEvent += new FS.SimConnect.RecvGetInputEventEventHandler( _simConnect_OnRecvGetInputEvent );
        _SC.OnRecvSubscribeInputEvent += new FS.SimConnect.RecvSubscribeInputEventEventHandler( _simConnect_OnRecvSubscribeInputEvent );// monitor B Events

        _SC.OnRecvAirportList += new FS.SimConnect.RecvAirportListEventHandler( _simConnect_OnRecvAirportList );
        _SC.OnRecvVorList += new FS.SimConnect.RecvVorListEventHandler( _simConnect_OnRecvVorList );
        _SC.OnRecvNdbList += new FS.SimConnect.RecvNdbListEventHandler( _simConnect_OnRecvNdbList );
        _SC.OnRecvWaypointList += new FS.SimConnect.RecvWaypointListEventHandler( _simConnect_OnRecvWaypointList );
      }
      catch (Exception ex) {
        RTB.Text += "AttachSimEvents_Base: Failed\n" + ex.Message;
        throw ex;
      }

      // Init the Subscription and Request Catalog
      _eventCat = SysStateHandler.DefaultSysStateCatalog( _SC );
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

    private bool mustSubSome = true;
    private bool mustSub1_4Sec = true;
    private void btSubSome_Click( object sender, EventArgs e )
    {
      if (mustSubSome) {
        _eventCat[EVENT_SIM_STATE].Sub( );
        _eventCat[EVENT_ACFT_LOAD].Sub( );
        _eventCat[EVENT_FLIGHT_LOAD].Sub( );
        _eventCat[EVENT_FPLAN_ACTIVATE].Sub( );
        _eventCat[EVENT_FPLAN_DEACTIVATE].Sub( );
        _eventCat[EVENT_SIM_PAUSE].Sub( );
        _eventCat[EVENT_SIM_PAUSE_EX1].Sub( );
      }
      else {
        _eventCat[EVENT_SIM_STATE].UnSub( );
        _eventCat[EVENT_ACFT_LOAD].UnSub( );
        _eventCat[EVENT_FLIGHT_LOAD].UnSub( );
        _eventCat[EVENT_FPLAN_ACTIVATE].UnSub( );
        _eventCat[EVENT_FPLAN_DEACTIVATE].UnSub( );
        _eventCat[EVENT_SIM_PAUSE].UnSub( );
        _eventCat[EVENT_SIM_PAUSE_EX1].UnSub( );
      }
      mustSubSome = !mustSubSome; // toggle
    }

    private void btSub1Hz_Click( object sender, EventArgs e )
    {
      if (mustSub1_4Sec) {
        _eventCat[EVENT_SIM_1SEC].Sub( );
        _eventCat[EVENT_SIM_4SEC].Sub( );
      }
      else {
        _eventCat[EVENT_SIM_1SEC].UnSub( );
        _eventCat[EVENT_SIM_4SEC].UnSub( );
      }
      mustSub1_4Sec = !mustSub1_4Sec; // toggle
    }

    private void btReqInpEvents_Click( object sender, EventArgs e )
    {
      RQ_InputEvents = SimConnectIDs.GetREQUEST( );
      txLastReqID.Text = $"{RQ_InputEvents}";
      _sb.AppendLine( "Sent request for InputEvents ID:" + txLastReqID.Text );
      _SC.EnumerateInputEvents( RQ_InputEvents );
    }

    // Facilities
    private void btReqAptList_Click( object sender, EventArgs e )
    {
      RQ_AptList = SimConnectIDs.GetREQUEST( );
      txLastReqID.Text = $"{RQ_AptList}";
      _sb.AppendLine( "Sent request for APT ID:" + txLastReqID.Text );
      _SC.RequestFacilitiesList_EX1( FS.SIMCONNECT_FACILITY_LIST_TYPE.AIRPORT, RQ_AptList );
    }

    private void btRequFacWYP_Click( object sender, EventArgs e )
    {
      RQ_WypList = SimConnectIDs.GetREQUEST( );
      txLastReqID.Text = $"{RQ_WypList}";
      _sb.AppendLine( "Sent request for WYP ID:" + txLastReqID.Text );
      _SC.RequestFacilitiesList( FS.SIMCONNECT_FACILITY_LIST_TYPE.WAYPOINT, RQ_WypList );
    }

    private void btRequFacVOR_Click( object sender, EventArgs e )
    {
      RQ_VorList = SimConnectIDs.GetREQUEST( );
      txLastReqID.Text = $"{RQ_VorList}";
      _sb.AppendLine( "Sent request for VOR ID:" + txLastReqID.Text );
      _SC.RequestFacilitiesList( FS.SIMCONNECT_FACILITY_LIST_TYPE.VOR, RQ_VorList );
    }

    private void btRequFacNDB_Click( object sender, EventArgs e )
    {
      RQ_NdbList = SimConnectIDs.GetREQUEST( );
      txLastReqID.Text = $"{RQ_NdbList}";
      _sb.AppendLine( "Sent request for NDB ID:" + txLastReqID.Text );
      _SC.RequestFacilitiesList( FS.SIMCONNECT_FACILITY_LIST_TYPE.NDB, RQ_NdbList );
    }

    #region SimConnect Event Handlers

    // usually just forward the callback to internal subscribers

    private bool _scOpen = false;
    private StringBuilder _sb = new StringBuilder( );

    // Handle SimConnect Open Reply
    private void _simConnect_OnRecvOpen( FS.SimConnect sender, FS.SIMCONNECT_RECV_OPEN data )
    {
      _sb.AppendLine( "_simConnect_OnRecvOpen" );
      _sb.AppendLine( data.szApplicationName );
      _sb.AppendLine( $"V{data.dwApplicationVersionMajor}.{data.dwApplicationVersionMinor}.{data.dwApplicationBuildMajor}.{data.dwApplicationBuildMinor}" );
      _sb.AppendLine( $"SimConV{data.dwSimConnectVersionMajor}.{data.dwSimConnectVersionMinor}.{data.dwSimConnectBuildMajor}.{data.dwSimConnectBuildMinor}" );

      _scOpen = true;
      RTB.Text += _sb.ToString( );
      _sb.Clear( );
    }

    // Handle SimConnect Quit Event
    private void _simConnect_OnRecvQuit( FS.SimConnect sender, FS.SIMCONNECT_RECV data )
    {
      _sb.AppendLine( "_simConnect_OnRecvQuit" );
      _scOpen = true;
      RTB.Text += _sb.ToString( );
      _sb.Clear( );
    }

    // Handle SimConnect Exception
    private void _simConnect_OnRecvException( FS.SimConnect sender, FS.SIMCONNECT_RECV_EXCEPTION data )
    {
      _sb.AppendLine( "_simConnect_OnRecvException" );
      FS.SIMCONNECT_EXCEPTION eException = (FS.SIMCONNECT_EXCEPTION)data.dwException;
      _sb.AppendLine( $"{eException.ToString( )}" );
      RTB.Text += _sb.ToString( );
      _sb.Clear( );
    }

    // Handle SimConnect Reply for System State Requests
    private void _simConnect_OnRecvSystemState( FS.SimConnect sender, FS.SIMCONNECT_RECV_SYSTEM_STATE data )
    {
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
      _sb.Clear( );
    }

    // SimConnect Reply for System State Event Subscription
    private void _simConnect_OnRecvEvent( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT data )
    {
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
        _sb.AppendLine( $"EVENT_SIM_PAUSE_EX1 data<{data.dwData}>" );
      }
      else if (data.uEventID == (uint)EVENT_SIM_STATE) {
        _sb.AppendLine( $"EVENT_SIM_STATE data<{data.dwData}>" );
      }

      else if (data.uEventID == (uint)EVENT_SIM_PAUSED) {
        _sb.AppendLine( $"EVENT_SIM_PAUSED" );
      }
      else if (data.uEventID == (uint)EVENT_SIM_UNPAUSED) {
        _sb.AppendLine( $"EVENT_SIM_UNPAUSED" );
      }
      else if (data.uEventID == (uint)EVENT_SIM_PAUSE) {
        // DOES NOT WORK - never called  -- TODO CHEK IN CURRENT SIM
        _sb.AppendLine( $"EVENT_SIM_PAUSE data<{data.dwData}>" );
      }

      else if (data.uEventID == (uint)EVENT_AI_ADDED) {
        _sb.AppendLine( $"EVENT_AI_ADDED data<{data.dwData}>" );
      }
      else if (data.uEventID == (uint)EVENT_AI_REMOVED) {
        _sb.AppendLine( $"EVENT_AI_ADDED data<{data.dwData}>" );
      }
      else if (data.uEventID == (uint)EVENT_POS_CHANGED) {
        _sb.AppendLine( $"EVENT_POS_CHANGED data<{data.dwData}>" );
      }
      else if (data.uEventID == (uint)EVENT_ACFT_CRASH) {
        _sb.AppendLine( $"EVENT_ACFT_CRASH data<{data.dwData}>" );
      }
      else if (data.uEventID == (uint)EVENT_FPLAN_DEACTIVATE) {
        _sb.AppendLine( $"EVENT_FPLAN_DEACTIVATE data<{data.dwData}>" );
      }

      else {
        // everything else not handled here
        _sb.AppendLine( $"Other Event with ID: evtId<{data.uEventID}> data<{data.dwData}>" );
      }

      RTB.Text += _sb.ToString( );
      _sb.Clear( );
    }

    // SimConnect Reply for FileName Subscriptions or Requests
    private void _simConnectRef_OnRecvEventFilename( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_FILENAME data )
    {
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
      // PLN discarded (not seen this working with a filename - reports only the fact in_simConnect_OnRecvEvent???)
      else if (data.uEventID == (uint)EVENT_FPLAN_DEACTIVATE) {
        _sb.AppendLine( $"EVENT_FPLAN_DEACTIVATE {data.szFileName}" );
      }
      else {
        // not handled here
        _sb.AppendLine( $"Other Event with ID: evtId<{data.uEventID}>" );
      }

      RTB.Text += _sb.ToString( );
      _sb.Clear( );
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
      _sb.AppendLine( "_simConnect_OnRecvSimobjectDataBytype" );

      RTB.Text += _sb.ToString( );
      _sb.Clear( );
    }

    // SimConnect Client data arrived
    private void _simConnect_OnRecvClientData( FS.SimConnect sender, FS.SIMCONNECT_RECV_CLIENT_DATA data )
    {
      _sb.AppendLine( "_simConnect_OnRecvClientData" );

      RTB.Text += _sb.ToString( );
      _sb.Clear( );
    }

    // SimConnect Enum InputEvents Data arrived
    private void _simConnect_OnRecvEnumerateInputEvents( FS.SimConnect sender, FS.SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS data )
    {
      _sb.AppendLine( $"_simConnect_OnRecvEnumerateInputEvents: RqID<{data.dwRequestID}> msgIndex:{data.dwEntryNumber} #msgs:{data.dwOutOf} #records:{data.dwArraySize}" );

      for (int i = 0; i < data.dwArraySize; i++) {
        var item = (FS.SIMCONNECT_INPUT_EVENT_DESCRIPTOR)data.rgData[i];
        _sb.AppendLine( $"<{i:##0}>:  (0x{item.Hash,-20:X}) \ttype({item.eType}) \t{item.Name} " );
      }
      RTB.Text += _sb.ToString( );
      _sb.Clear( );
    }

    // SimConnect returns data 
    private void _simConnect_OnRecvEnumerateInputEventParams( FS.SimConnect sender, FS.SIMCONNECT_RECV_ENUMERATE_INPUT_EVENT_PARAMS data )
    {
      _sb.AppendLine( "_simConnect_OnRecvEnumerateInputEventParams" );

      RTB.Text += _sb.ToString( );
      _sb.Clear( );
    }

    // SimConnect returns data for a single InputEvent 
    private void _simConnect_OnRecvGetInputEvent( FS.SimConnect sender, FS.SIMCONNECT_RECV_GET_INPUT_EVENT data )
    {
      _sb.AppendLine( "_simConnect_OnRecvGetInputEvent" );

      RTB.Text += _sb.ToString( );
      _sb.Clear( );
    }

    // SimConnect returns data for an InputEvent subscription
    private void _simConnect_OnRecvSubscribeInputEvent( FS.SimConnect sender, FS.SIMCONNECT_RECV_SUBSCRIBE_INPUT_EVENT data )
    {
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
      _sb.Clear( );
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
*/
    /// <summary>
    /// NOT SUPPORTED BY NOW
    /// Asynch call by FS having received  Airport Lists
    /// </summary>
    private void _simConnect_OnRecvAirportList( FS.SimConnect sender, FS.SIMCONNECT_RECV_AIRPORT_LIST data )
    {
      _sb.AppendLine( $"_OnRecvAirportList: RqID<{data.dwRequestID}> msgIndex:{data.dwEntryNumber} #msgs:{data.dwOutOf} #records:{data.dwArraySize}" );

      for (int i = 0; i < data.dwArraySize; i++) {
        var item = (FS.SIMCONNECT_DATA_FACILITY_AIRPORT)data.rgData[i];
        _sb.AppendLine( $"<{i:##0}>:  {item.Ident}({item.Region}) LLA({item.Latitude:00.0000},{item.Longitude:000.0000},{item.Altitude:##,##0})" );
      }
      RTB.Text += _sb.ToString( );
      _sb.Clear( );
    }

    /// <summary>
    /// Asynch call by FS having received VOR/LOC Lists
    /// </summary>
    private void _simConnect_OnRecvVorList( FS.SimConnect sender, FS.SIMCONNECT_RECV_VOR_LIST data )
    {
      _sb.AppendLine( $"_OnRecvVorList: RqID<{data.dwRequestID}> msgIndex:{data.dwEntryNumber} #msgs:{data.dwOutOf} #records:{data.dwArraySize}" );

      for (int i = 0; i < data.dwArraySize; i++) {
        var item = (FS.SIMCONNECT_DATA_FACILITY_VOR)data.rgData[i];
        _sb.AppendLine( $"<{i:##0}>:  {item.Ident}({item.Region}) LLA({item.Latitude:00.0000},{item.Longitude:000.0000},{item.Altitude:##,##0}) mv({item.fMagVar:0#.00})" );
        _sb.AppendLine( $"            f({item.fFrequency})" );
        _sb.AppendLine( $"            flags({item.Flags}), LOC({item.fLocalizer}), GsLLA({item.GlideLat:00.0000},{item.GlideLon:000.0000}, {item.GlideAlt:##,##0}), GsAng({item.fGlideSlopeAngle})" );
      }
      RTB.Text += _sb.ToString( );
      _sb.Clear( );
    }

    /// <summary>
    /// Asynch call by FS having received NDB Lists
    /// </summary>
    private void _simConnect_OnRecvNdbList( FS.SimConnect sender, FS.SIMCONNECT_RECV_NDB_LIST data )
    {
      _sb.AppendLine( $"_OnRecvNdbList: RqID<{data.dwRequestID}> msgIndex:{data.dwEntryNumber} #msgs:{data.dwOutOf} #records:{data.dwArraySize}" );

      for (int i = 0; i < data.dwArraySize; i++) {
        var item = (FS.SIMCONNECT_DATA_FACILITY_NDB)data.rgData[i];
        _sb.AppendLine( $"<{i:##0}>:  {item.Ident}({item.Region}) LLA({item.Latitude:00.0000},{item.Longitude:000.0000},{item.Altitude:##,##0}) mv({item.fMagVar:0#.00})" );
        _sb.AppendLine( $"            f({item.fFrequency})" );
      }
      RTB.Text += _sb.ToString( );
      _sb.Clear( );
    }

    /// <summary>
    /// NOT SUPPORTED BY NOW
    /// Asynch call by FS having received Waypoint Lists
    /// </summary>
    private void _simConnect_OnRecvWaypointList( FS.SimConnect sender, FS.SIMCONNECT_RECV_WAYPOINT_LIST data )
    {
      _sb.AppendLine( $"_OnRecvWaypointList: RqID<{data.dwRequestID}> msgIndex:{data.dwEntryNumber} #msgs:{data.dwOutOf} #records:{data.dwArraySize}" );

      for (int i = 0; i < data.dwArraySize; i++) {
        var item = (FS.SIMCONNECT_DATA_FACILITY_WAYPOINT)data.rgData[i];
        _sb.AppendLine( $"<{i:##0}>:  {item.Ident}({item.Region}) LLA({item.Latitude:00.0000},{item.Longitude:000.0000},{item.Altitude:##,##0}) mv({item.fMagVar:0#.00})" );
      }
      RTB.Text += _sb.ToString( );
      _sb.Clear( );
    }

    #endregion


    #region WinProc 

    /// <summary>
    /// Windows Message Handler Override 
    /// - must handle messages from SimConnect otherwise SimConnect does not provide events and callbacks
    /// Using the Adapter provided MessageHandler
    /// </summary>
    protected override void DefWndProc( ref Message m )
    {
      if ((_SC != null) && (_version != FS.FSVersion.Unknown)) {
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
