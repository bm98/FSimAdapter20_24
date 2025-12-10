using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using FS = Microsoft.FlightSimulator.SimConnect;

using SimConShared;
using SimConShared.SystemState;
using SimConTool2024;

namespace z_TEST_Target2024
{
  public partial class Form1 : Form
  {
    // static only because it's easier...
    private static FS.SimConnect _SC = null;

    // SimConnector
    private SimCon _simCon = null;

    private FSVersion _version = FSVersion.Unknown;
    private bool _connected = false;

    // define and Init RequestIDs for planned Requests
    private REQUEST RQ_InputEvents = SimConnectIDs.GetREQUEST( );
    private REQUEST RQ_AptList = SimConnectIDs.GetREQUEST( );
    private REQUEST RQ_WypList = SimConnectIDs.GetREQUEST( );
    private REQUEST RQ_VorList = SimConnectIDs.GetREQUEST( );
    private REQUEST RQ_NdbList = SimConnectIDs.GetREQUEST( );

    // Catalog of supported Event Subscriptions and Requests
    private SysStateHandlerCat _eventCat;

    // SimVar Get/Set Data Test Module
    private CamVarGetModule _camVarGetData = null;
    private SimVarGetModule _sVarGetData = null;
    private SimVarSetModule _sVarSetData = null;
    private SimEventSetModule _sSetKeyEvent = null;
    private SimEventGetModule _sGetKeyEvent = null;


    private WinFormInvoker _invoker;

    #region Via SimCon

    private bool ConnectSimCon( )
    {
      // sanity
      if (_simCon != null) {
        RTB.Text += $"SimCon:  Is already in use - will disconnect\n";
        _simCon.Disconnect( );
        return false;
      }

      bool ret = true; // set false on error !!

      RTB.Text += $"SimCon:  Create and use SimCon MSFS2024 utility\n";

      _simCon?.Dispose( );
      _simCon = new SimCon( );
      _simCon.Establishing += _SimCon_Establishing;
      _simCon.Connected += _SimCon_Connected;
      _simCon.Disconnected += _SimCon_Disconnected;

      _simCon.Connect( );
      // wait for the Events
      return ret;
    }

    private void _SimCon_Establishing( object sender, EventArgs e )
    {
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += $"SimCon:  Establishing - wait until Connected ...\n";
      } );
    }

    private void _SimCon_Connected( object sender, EventArgs e )
    {
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += $"SimCon:  Connected\n";
        // test, only now SC is valid
        if (_simCon.IsConnected) {
          RTB.Text += $"SimCon:  IsConnected: {_simCon.IsConnected}\n";
          _SC = _simCon.SimConnectRef; // get the SC ref for use
          _connected = true;
          _version = _simCon.SimVersion;
          RTB.Text += $"SimCon:  Detected: {_version}\n";
          RTB.Text += "SimCon:  Attach Eventhandlers\n";
          AttachHandlers( );
          MarkButton( btConnectSimCon, true );

          if (_version == FSVersion.Unknown) {
            RTB.Text += $"SimCon:  No MSFS App seems running\n";
            RTB.Text += $"SimCon:  FS Title: {_simCon.FSimWindowTitle}";
            RTB.Text += $"SimCon:  Shutdown\n";
            _simCon.Disconnect( );
            _SC = null;
            RTB.Text += $"SimCon:  Done...\n";
          }
          else {
            // OK..
            RTB.Text += $"SimCon:  FS Title: {_simCon.FSimWindowTitle} is alive\n";
          }
        }
        else {
          RTB.Text += $"SimCon:  ???? Replies IsConnected=false ???\n";
        }
      } );

    }

    private void _SimCon_Disconnected( object sender, EventArgs e )
    {
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += $"SimCon:  Disconnected\n";
        _SC = null;
        _simCon.Establishing -= _SimCon_Establishing;
        _simCon.Connected -= _SimCon_Connected;
        _simCon.Disconnected -= _SimCon_Disconnected;
        _simCon?.Dispose( );
        _simCon = null;
        MarkButton( btConnectSimCon, false );
      } );
    }


    #endregion

    // may throw on unexpected errors
    private void AttachHandlers( )
    {
      // sanity
      if (_SC == null) throw new ApplicationException( "_SC is null" );

      // Init the Subscription and Request Catalog for this SC instance
      _eventCat = SysStateHandler.DefaultSysStateCatalog( _SC );

      try {

        _SC.OnRecvException += new FS.SimConnect.RecvExceptionEventHandler( _simConnect_OnRecvException );

        // events and request replies
        _SC.OnRecvSystemState += new FS.SimConnect.RecvSystemStateEventHandler( _simConnect_OnRecvSystemState );
        _SC.OnRecvEvent += new FS.SimConnect.RecvEventEventHandler( _simConnect_OnRecvEvent );
        _SC.OnRecvEventFilename += new FS.SimConnect.RecvEventFilenameEventHandler( _simConnectRef_OnRecvEventFilename );

        _SC.OnRecvEventFrame += new FS.SimConnect.RecvEventFrameEventHandler( _simConnect_OnRecvEventFrame );
        _SC.OnRecvSimobjectDataBytype += new FS.SimConnect.RecvSimobjectDataBytypeEventHandler( _simConnect_OnRecvSimobjectDataBytype );
        _SC.OnRecvSimobjectData += new FS.SimConnect.RecvSimobjectDataEventHandler( _simConnect_OnRecvSimobjectData );
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

    }

    // change button forecol based on flag
    private void MarkButton( Button button, bool on )
    {
      // sanity
      if (button == null) return;

      button.ForeColor = on ? Color.Green : Color.Black;
    }



    #region FORM

    public Form1( )
    {
      InitializeComponent( );

      _invoker = new WinFormInvoker( this );

    }

    private void Form1_Load( object sender, EventArgs e )
    {

    }

    private void Form1_FormClosing( object sender, FormClosingEventArgs e )
    {
      _simCon?.Dispose( );

    }
    private void btClearRTB_Click( object sender, EventArgs e )
    {
      RTB.Text = "";
    }

    private void btConnectSimCon_Click( object sender, EventArgs e )
    {
      ConnectSimCon( );
    }

    private void btReqSome_Click( object sender, EventArgs e )
    {
      // sanity
      if (!_connected || (_SC == null)) return;

      _eventCat[_eventCat.REQUEST_SIM_STATE].Request( );
      _eventCat[_eventCat.REQUEST_SIM_DIALOG].Request( );
      _eventCat[_eventCat.REQUEST_ACFT_LOADED].Request( );
      _eventCat[_eventCat.REQUEST_FLIGHT_LOADED].Request( );
      _eventCat[_eventCat.REQUEST_FLIGHT_PLAN].Request( );
    }

    private bool mustSubSome = true;
    private bool mustSub1_4Sec = true;
    private bool mustSubFrame = true;
    private bool mustSubCam = true;
    private bool mustSubEvents = true;


    private void btSubSome_Click( object sender, EventArgs e )
    {
      // sanity
      if (!_connected || (_SC == null)) return;

      if (mustSubSome) {
        _eventCat[_eventCat.EVENT_SIM_STATE].Sub( );
        _eventCat[_eventCat.EVENT_SIM_PAUSE].Sub( );
        _eventCat[_eventCat.EVENT_SIM_PAUSE_EX1].Sub( );
        _eventCat[_eventCat.EVENT_ACFT_LOAD].Sub( );
        _eventCat[_eventCat.EVENT_FLIGHT_LOAD].Sub( );
        _eventCat[_eventCat.EVENT_FPLAN_ACTIVATE].Sub( );
        _eventCat[_eventCat.EVENT_FPLAN_DEACTIVATE].Sub( );
        MarkButton( sender as Button, true );
      }
      else {
        _eventCat[_eventCat.EVENT_SIM_STATE].UnSub( );
        _eventCat[_eventCat.EVENT_SIM_PAUSE].UnSub( );
        _eventCat[_eventCat.EVENT_SIM_PAUSE_EX1].UnSub( );
        _eventCat[_eventCat.EVENT_ACFT_LOAD].UnSub( );
        _eventCat[_eventCat.EVENT_FLIGHT_LOAD].UnSub( );
        _eventCat[_eventCat.EVENT_FPLAN_ACTIVATE].UnSub( );
        _eventCat[_eventCat.EVENT_FPLAN_DEACTIVATE].UnSub( );
        MarkButton( sender as Button, false );
      }
      mustSubSome = !mustSubSome; // toggle
    }

    private void btSub1Hz_Click( object sender, EventArgs e )
    {
      // sanity
      if (!_connected || (_SC == null)) return;

      if (mustSub1_4Sec) {
        _eventCat[_eventCat.EVENT_SIM_1SEC].Sub( );
        _eventCat[_eventCat.EVENT_SIM_4SEC].Sub( );
        MarkButton( sender as Button, true );
      }
      else {
        _eventCat[_eventCat.EVENT_SIM_1SEC].UnSub( );
        _eventCat[_eventCat.EVENT_SIM_4SEC].UnSub( );
        MarkButton( sender as Button, false );
      }
      mustSub1_4Sec = !mustSub1_4Sec; // toggle
    }

    private void btSubFrame_Click( object sender, EventArgs e )
    {
      // sanity
      if (!_connected || (_SC == null)) return;

      if (mustSubFrame) {
        _eventCat[_eventCat.EVENT_SIM_FRAME].Sub( );
        MarkButton( sender as Button, true );
      }
      else {
        _eventCat[_eventCat.EVENT_SIM_FRAME].UnSub( );
        MarkButton( sender as Button, false );
      }
      mustSubFrame = !mustSubFrame;
    }

    private void btReqInpEvents_Click( object sender, EventArgs e )
    {
      // sanity
      if (!_connected || (_SC == null)) return;

      RQ_InputEvents = SimConnectIDs.GetREQUEST( );
      txLastReqID.Text = $"{RQ_InputEvents}";
      _sb.AppendLine( "Sent request for InputEvents ID:" + txLastReqID.Text );
      _SC.EnumerateInputEvents( RQ_InputEvents );
    }

    // Facilities
    private void btReqAptList_Click( object sender, EventArgs e )
    {
      // sanity
      if (!_connected || (_SC == null)) return;

      RQ_AptList = SimConnectIDs.GetREQUEST( );
      txLastReqID.Text = $"{RQ_AptList}";
      _sb.AppendLine( "Sent request for APT ID:" + txLastReqID.Text );
      _SC.RequestFacilitiesList_EX1( FS.SIMCONNECT_FACILITY_LIST_TYPE.AIRPORT, RQ_AptList );
    }

    private void btRequFacWYP_Click( object sender, EventArgs e )
    {
      // sanity
      if (!_connected || (_SC == null)) return;

      RQ_WypList = SimConnectIDs.GetREQUEST( );
      txLastReqID.Text = $"{RQ_WypList}";
      _sb.AppendLine( "Sent request for WYP ID:" + txLastReqID.Text );
      _SC.RequestFacilitiesList( FS.SIMCONNECT_FACILITY_LIST_TYPE.WAYPOINT, RQ_WypList );
    }

    private void btRequFacVOR_Click( object sender, EventArgs e )
    {
      // sanity
      if (!_connected || (_SC == null)) return;

      RQ_VorList = SimConnectIDs.GetREQUEST( );
      txLastReqID.Text = $"{RQ_VorList}";
      _sb.AppendLine( "Sent request for VOR ID:" + txLastReqID.Text );
      _SC.RequestFacilitiesList( FS.SIMCONNECT_FACILITY_LIST_TYPE.VOR, RQ_VorList );
    }

    private void btRequFacNDB_Click( object sender, EventArgs e )
    {
      // sanity
      if (!_connected || (_SC == null)) return;

      RQ_NdbList = SimConnectIDs.GetREQUEST( );
      txLastReqID.Text = $"{RQ_NdbList}";
      _sb.AppendLine( "Sent request for NDB ID:" + txLastReqID.Text );
      _SC.RequestFacilitiesList( FS.SIMCONNECT_FACILITY_LIST_TYPE.NDB, RQ_NdbList );
    }

    private void btReqCamData_Click( object sender, EventArgs e )
    {
      // sanity
      if (!_connected || (_SC == null)) return;

      if (_camVarGetData == null) {
        _camVarGetData = new CamVarGetModule( );
        _camVarGetData.RegisterWithSimConnect( _SC );
        return; // first call registers only
      }

      _camVarGetData.SubscribeFromSimConnect( _SC, mustSubCam );
      MarkButton( sender as Button, mustSubCam );
      mustSubCam = !mustSubCam;
    }

    private void btReqData_Click( object sender, EventArgs e )
    {
      // sanity
      if (!_connected || (_SC == null)) return;

      if (_sVarGetData == null) {
        _sVarGetData = new SimVarGetModule( );
        _sVarGetData.RegisterWithSimConnect( _SC );
        MarkButton( sender as Button, true );
        return; // first call registers only
      }
      if (_sVarSetData == null) {
        _sVarSetData = new SimVarSetModule( );
        _sVarSetData.RegisterWithSimConnect( _SC );
        MarkButton( sender as Button, true );
        return; // first call registers only
      }
      // further calls try to retrieve data
      _sVarGetData.RequestFromSimConnect( _SC );
      _sVarSetData.RequestFromSimConnect( _SC );
    }

    private void btReqSendData_Click( object sender, EventArgs e )
    {
      // sanity
      if (!_connected || (_SC == null)) return;

      if (_sVarSetData == null) {
        _sVarSetData = new SimVarSetModule( );
        _sVarSetData.RegisterWithSimConnect( _SC );
        MarkButton( sender as Button, true );
        return; // first call registers only
      }
      _sVarSetData.ChangeVarsABit( ); // change vars
      // further calls try to send data
      _sVarSetData.RequestToSimConnect( _SC );
    }

    private void btSubSimEvents_Click( object sender, EventArgs e )
    {
      // sanity
      if (!_connected || (_SC == null)) return;
      if (_sGetKeyEvent == null) return;

      _sGetKeyEvent.RequestFromSimConnect( _SC, mustSubEvents );
      MarkButton( sender as Button, mustSubEvents );
      mustSubEvents = !mustSubEvents;
    }

    private void btSendKey_Click( object sender, EventArgs e )
    {
      // sanity
      if (!_connected || (_SC == null)) return;

      if (_sSetKeyEvent == null) {
        _sSetKeyEvent = new SimEventSetModule( "PARKING_BRAKES" );
        _sSetKeyEvent.RegisterWithSimConnect( _SC );
        MarkButton( sender as Button, true );
        return; // first call registers only
      }
      // further calls try to send data
      _sSetKeyEvent.RequestToSimConnect( _SC, 0 ); // no arg, toggles always 
    }

    #endregion

    #region SimConnect Event Handlers

    // usually just forward the callback to internal subscribers

    private StringBuilder _sb = new StringBuilder( );

    // Handle SimConnect Exception
    private void _simConnect_OnRecvException( FS.SimConnect sender, FS.SIMCONNECT_RECV_EXCEPTION data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( "_simConnect_OnRecvException" );
      FS.SIMCONNECT_EXCEPTION eException = (FS.SIMCONNECT_EXCEPTION)data.dwException;
      _sb.AppendLine( $"{eException.ToString( )}" );
      string text = _sb.ToString( );
      _sb.Clear( );

      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
    }

    // Handle SimConnect Reply for System State Requests
    private void _simConnect_OnRecvSystemState( FS.SimConnect sender, FS.SIMCONNECT_RECV_SYSTEM_STATE data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( "_simConnect_OnRecvSystemState" );

      // this is the Sim reply to a SystemState Request
      if (data.dwRequestID == (uint)_eventCat.REQUEST_SIM_STATE) {
        _sb.AppendLine( $"REQUEST_SIM_STATE: {data.dwInteger}" );
      }
      else if (data.dwRequestID == (uint)_eventCat.REQUEST_SIM_DIALOG) {
        _sb.AppendLine( $"REQUEST_SIM_DIALOG: {data.dwInteger}" );
      }
      else if (data.dwRequestID == (uint)_eventCat.REQUEST_ACFT_LOADED) {
        _sb.AppendLine( $"REQUEST_ACFT_LOADED: {data.szString}" );
      }
      // FLT file
      else if (data.dwRequestID == (uint)_eventCat.REQUEST_FLIGHT_LOADED) {
        _sb.AppendLine( $"REQUEST_FLIGHT_LOADED: {data.szString}" );
      }
      // PLN file (not seen this working...)
      else if (data.dwRequestID == (uint)_eventCat.REQUEST_FLIGHT_PLAN) {
        _sb.AppendLine( $"REQUEST_FLIGHT_PLAN: {data.szString}" );
      }
      else {
        _sb.AppendLine( $"Other Request with ID:{data.dwRequestID} " );
      }
      string text = _sb.ToString( );
      _sb.Clear( );
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
    }

    // SimConnect Reply for System State Event Subscription
    private void _simConnect_OnRecvEvent( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( "_simConnect_OnRecvEvent" );

      if (data.uEventID == (uint)_eventCat.EVENT_SIM_6HZ) {
        _sb.AppendLine( $"EVENT_SIM_6HZ" );
      }
      else if (data.uEventID == (uint)_eventCat.EVENT_SIM_1SEC) {
        _sb.AppendLine( $"EVENT_SIM_1SEC" );
      }
      else if (data.uEventID == (uint)_eventCat.EVENT_SIM_4SEC) {
        _sb.AppendLine( $"EVENT_SIM_4SEC" );
      }

      else if (data.uEventID == (uint)_eventCat.EVENT_SIM_START) {
        _sb.AppendLine( $"EVENT_SIM_START" );
      }
      else if (data.uEventID == (uint)_eventCat.EVENT_SIM_STOP) {
        _sb.AppendLine( $"EVENT_SIM_STOP" );
      }
      else if (data.uEventID == (uint)_eventCat.EVENT_SIM_PAUSE_EX1) {
        bool isOff = data.dwData == 0;
        bool isFull = (data.dwData & 1) > 0;
        bool isFSX = (data.dwData & 2) > 0;
        bool isActive = (data.dwData & 4) > 0;
        bool isSimPause = (data.dwData & 8) > 0;
        _sb.AppendLine( $"EVENT_SIM_PAUSE_EX1 data<{data.dwData}> off:{isOff}, full:{isFull}, active:{isActive}, sim:{isSimPause}" );
      }
      else if (data.uEventID == (uint)_eventCat.EVENT_SIM_STATE) {
        bool isOn = data.dwData == (uint)FS.SIMCONNECT_STATE.ON;
        _sb.AppendLine( $"EVENT_SIM_STATE data<{data.dwData}> on:{isOn}" );
      }

      else if (data.uEventID == (uint)_eventCat.EVENT_SIM_PAUSED) {
        _sb.AppendLine( $"EVENT_SIM_PAUSED" );
      }
      else if (data.uEventID == (uint)_eventCat.EVENT_SIM_UNPAUSED) {
        _sb.AppendLine( $"EVENT_SIM_UNPAUSED" );
      }
      else if (data.uEventID == (uint)_eventCat.EVENT_SIM_PAUSE) {
        // DOES NOT WORK - never called  -- TODO CHEK IN CURRENT SIM
        bool isOn = data.dwData == (uint)FS.SIMCONNECT_STATE.ON;
        _sb.AppendLine( $"EVENT_SIM_PAUSE data<{data.dwData}> paused:{isOn}" );
      }

      else if (data.uEventID == (uint)_eventCat.EVENT_AI_ADDED) {
        _sb.AppendLine( $"EVENT_AI_ADDED data<{data.dwData}>" );
      }
      else if (data.uEventID == (uint)_eventCat.EVENT_AI_REMOVED) {
        _sb.AppendLine( $"EVENT_AI_ADDED data<{data.dwData}>" );
      }
      else if (data.uEventID == (uint)_eventCat.EVENT_POS_CHANGED) {
        _sb.AppendLine( $"EVENT_POS_CHANGED data<{data.dwData}>" );
      }
      else if (data.uEventID == (uint)_eventCat.EVENT_ACFT_CRASH) {
        _sb.AppendLine( $"EVENT_ACFT_CRASH data<{data.dwData}>" );
      }
      else if (data.uEventID == (uint)_eventCat.EVENT_FPLAN_DEACTIVATE) {
        _sb.AppendLine( $"EVENT_FPLAN_DEACTIVATE data<{data.dwData}>" );
      }

      else {
        var handled = _sGetKeyEvent?.HandleSimEvent( data );
        if (handled.HasValue && handled.Value) {
          var se = SimEventCat.GetEvent( (EventID)data.uEventID );
          _sb.AppendLine( $"\tEVENT: {se} received" );
        }
        else {
          // everything else not handled here
          _sb.AppendLine( $"Other Event with ID: evtId<{data.uEventID}> data<{data.dwData}>" );
        }
      }

      string text = _sb.ToString( );
      _sb.Clear( );
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
    }

    // SimConnect Reply for FileName Subscriptions or Requests
    private void _simConnectRef_OnRecvEventFilename( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_FILENAME data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( "_simConnectRef_OnRecvEventFilename" );

      if (data.uEventID == (uint)_eventCat.EVENT_ACFT_LOAD) {
        _sb.AppendLine( $"EVENT_ACFT_LOAD {data.szFileName}" );
      }
      // FLT file was loaded
      else if (data.uEventID == (uint)_eventCat.EVENT_FLIGHT_LOAD) {
        _sb.AppendLine( $"EVENT_FLIGHT_LOAD {data.szFileName}" );
      }
      // FLT file was saved 
      else if (data.uEventID == (uint)_eventCat.EVENT_FLIGHT_SAVE) {
        _sb.AppendLine( $"EVENT_FLIGHT_SAVE {data.szFileName}" );
      }
      // PLN file (not seen this working)
      else if (data.uEventID == (uint)_eventCat.EVENT_FPLAN_ACTIVATE) {
        _sb.AppendLine( $"EVENT_FPLAN_ACTIVATE {data.szFileName}" );
      }
      // PLN discarded (not seen this working with a filename - reports only the fact in_simConnect_OnRecvEvent???)
      else if (data.uEventID == (uint)_eventCat.EVENT_FPLAN_DEACTIVATE) {
        _sb.AppendLine( $"EVENT_FPLAN_DEACTIVATE {data.szFileName}" );
      }
      else {
        // not handled here
        _sb.AppendLine( $"Other Event with ID: evtId<{data.uEventID}>" );
      }

      string text = _sb.ToString( );
      _sb.Clear( );
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
    }

    // SimConnect Frame event arrived
    private void _simConnect_OnRecvEventFrame( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_FRAME data )
    {
      // cannot monitor in RTB - too fast...
      _ = data.fFrameRate;
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        txFPS.Text = $"{data.fFrameRate:##0}";
      } );
      _ = data.fSimSpeed;
    }


    // SimConnect reply for request obj by type
    private void _simConnect_OnRecvSimobjectData( FS.SimConnect sender, FS.SIMCONNECT_RECV_SIMOBJECT_DATA data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( "_simConnect_OnRecvSimobjectData" );
      string text;
      var handled = _sVarGetData?.HandleSimObjectType( data );
      if (handled.HasValue && handled.Value) {
        _sb.AppendLine( " Got a SimVar Get Data Update:" );
        _sb.Append( _sVarGetData.GetStruct.ToString( ) );
        text = _sb.ToString( );
        _sb.Clear( );
        // invoke this on the Form not in the callback..
        _invoker.HandleEvent( ( ) => {
          RTB.Text += text;
        } );
        return;
      }
      // next try
      handled = _sVarSetData?.HandleSimObjectType( data );
      if (handled.HasValue && handled.Value) {
        _sb.AppendLine( " Got a SimVar Set Data Update:" );
        _sb.Append( _sVarSetData.SetStruct.ToString( ) );
        text = _sb.ToString( );
        _sb.Clear( );
        // invoke this on the Form not in the callback..
        _invoker.HandleEvent( ( ) => {
          RTB.Text += text;
        } );
        return;
      }
      // next try
      handled = _camVarGetData?.HandleSimObjectType( data );
      if (handled.HasValue && handled.Value) {
        _sb.AppendLine( " Got a CamVar Get Data Update:" );
        _sb.Append( _camVarGetData.GetStruct.ToString( ) );
        text = _sb.ToString( );
        _sb.Clear( );
        // invoke this on the Form not in the callback..
        _invoker.HandleEvent( ( ) => {
          RTB.Text += text;
        } );
        return;
      }


      text = _sb.ToString( );
      _sb.Clear( );
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
    }

    // SimConnect reply for request obj by type
    private void _simConnect_OnRecvSimobjectDataBytype( FS.SimConnect sender, FS.SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( "_simConnect_OnRecvSimobjectDataBytype" );

      string text;
      var handled = _sVarGetData?.HandleSimObjectType( data );
      if (handled.HasValue && handled.Value) {
        _sb.AppendLine( " Got a SimVar Get Data Update:" );
        _sb.Append( _sVarGetData.GetStruct.ToString( ) );
        text = _sb.ToString( );
        _sb.Clear( );
        // invoke this on the Form not in the callback..
        _invoker.HandleEvent( ( ) => {
          RTB.Text += text;
        } );
        return;
      }
      // next try
      handled = _sVarSetData?.HandleSimObjectType( data );
      if (handled.HasValue && handled.Value) {
        _sb.AppendLine( " Got a SimVar Set Data Update:" );
        _sb.Append( _sVarSetData.SetStruct.ToString( ) );
        text = _sb.ToString( );
        _sb.Clear( );
        // invoke this on the Form not in the callback..
        _invoker.HandleEvent( ( ) => {
          RTB.Text += text;
        } );
        return;
      }
      // next try
      handled = _camVarGetData?.HandleSimObjectType( data );
      if (handled.HasValue && handled.Value) {
        _sb.AppendLine( " Got a CamVar Get Data Update:" );
        _sb.Append( _camVarGetData.GetStruct.ToString( ) );
        text = _sb.ToString( );
        _sb.Clear( );
        // invoke this on the Form not in the callback..
        _invoker.HandleEvent( ( ) => {
          RTB.Text += text;
        } );
        return;
      }


      text = _sb.ToString( );
      _sb.Clear( );
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
    }

    // SimConnect Client data arrived
    private void _simConnect_OnRecvClientData( FS.SimConnect sender, FS.SIMCONNECT_RECV_CLIENT_DATA data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( $"_simConnect_OnRecvClientData: RqID<{data.dwRequestID}> DID<{data.dwDefineID}> msgIndex:{data.dwentrynumber} #msgs:{data.dwoutof} #DWORDs:{data.dwDefineCount}" );

      string text = _sb.ToString( );
      _sb.Clear( );
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
    }

    // SimConnect Enum InputEvents Data arrived
    private void _simConnect_OnRecvEnumerateInputEvents( FS.SimConnect sender, FS.SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( $"_simConnect_OnRecvEnumerateInputEvents: RqID<{data.dwRequestID}> msgIndex:{data.dwEntryNumber} #msgs:{data.dwOutOf} #records:{data.dwArraySize}" );

      for (int i = 0; i < data.dwArraySize; i++) {
        var item = (FS.SIMCONNECT_INPUT_EVENT_DESCRIPTOR)data.rgData[i];
        _sb.AppendLine( $"<{i:##0}>:  (0x{item.Hash,-20:X}) \ttype({item.eType}) \t{item.Name} " );
      }
      string text = _sb.ToString( );
      _sb.Clear( );
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
    }

    // SimConnect returns data 
    private void _simConnect_OnRecvEnumerateInputEventParams( FS.SimConnect sender, FS.SIMCONNECT_RECV_ENUMERATE_INPUT_EVENT_PARAMS data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( "_simConnect_OnRecvEnumerateInputEventParams" );

      string text = _sb.ToString( );
      _sb.Clear( );
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
    }

    // SimConnect returns data for a single InputEvent 
    private void _simConnect_OnRecvGetInputEvent( FS.SimConnect sender, FS.SIMCONNECT_RECV_GET_INPUT_EVENT data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( "_simConnect_OnRecvGetInputEvent" );

      string text = _sb.ToString( );
      _sb.Clear( );
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
    }

    // SimConnect returns data for an InputEvent subscription
    private void _simConnect_OnRecvSubscribeInputEvent( FS.SimConnect sender, FS.SIMCONNECT_RECV_SUBSCRIBE_INPUT_EVENT data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( "_simConnect_OnRecvSubscribeInputEvent" );

      switch (data.eType) {
        case FS.SIMCONNECT_INPUT_EVENT_TYPE.DOUBLE:
          _sb.AppendLine( $"Got Double {(double)data.Value[0]} for HashItem {data.Hash}" );
          break;
        case FS.SIMCONNECT_INPUT_EVENT_TYPE.STRING:
          _sb.AppendLine( $"Got String {(FS.SimConnect.InputEventString)data.Value[0]} for HashItem {data.Hash}" );
          break;
      }

      string text = _sb.ToString( );
      _sb.Clear( );
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
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
    /// Asynch call by FS having received  Airport Lists
    /// </summary>
    private void _simConnect_OnRecvAirportList( FS.SimConnect sender, FS.SIMCONNECT_RECV_AIRPORT_LIST data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( $"_OnRecvAirportList: RqID<{data.dwRequestID}> msgIndex:{data.dwEntryNumber} #msgs:{data.dwOutOf} #records:{data.dwArraySize}" );

      for (int i = 0; i < data.dwArraySize; i++) {
        var item = (FS.SIMCONNECT_DATA_FACILITY_AIRPORT)data.rgData[i];
        _sb.AppendLine( $"<{i:##0}>:  {item.Ident}({item.Region}) LLA({item.Latitude:00.0000},{item.Longitude:000.0000},{item.Altitude:##,##0})" );
      }
      string text = _sb.ToString( );
      _sb.Clear( );
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
    }

    /// <summary>
    /// Asynch call by FS having received VOR/LOC Lists
    /// </summary>
    private void _simConnect_OnRecvVorList( FS.SimConnect sender, FS.SIMCONNECT_RECV_VOR_LIST data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( $"_OnRecvVorList: RqID<{data.dwRequestID}> msgIndex:{data.dwEntryNumber} #msgs:{data.dwOutOf} #records:{data.dwArraySize}" );

      for (int i = 0; i < data.dwArraySize; i++) {
        var item = (FS.SIMCONNECT_DATA_FACILITY_VOR)data.rgData[i];
        _sb.AppendLine( $"<{i:##0}>:  {item.Ident}({item.Region}) LLA({item.Latitude:00.0000},{item.Longitude:000.0000},{item.Altitude:##,##0}) mv({item.fMagVar:0#.00})" );
        _sb.AppendLine( $"            f({item.fFrequency})" );
        _sb.AppendLine( $"            flags({item.Flags}), LOC({item.fLocalizer}), GsLLA({item.GlideLat:00.0000},{item.GlideLon:000.0000}, {item.GlideAlt:##,##0}), GsAng({item.fGlideSlopeAngle})" );
      }
      string text = _sb.ToString( );
      _sb.Clear( );
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
    }

    /// <summary>
    /// Asynch call by FS having received NDB Lists
    /// </summary>
    private void _simConnect_OnRecvNdbList( FS.SimConnect sender, FS.SIMCONNECT_RECV_NDB_LIST data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( $"_OnRecvNdbList: RqID<{data.dwRequestID}> msgIndex:{data.dwEntryNumber} #msgs:{data.dwOutOf} #records:{data.dwArraySize}" );

      for (int i = 0; i < data.dwArraySize; i++) {
        var item = (FS.SIMCONNECT_DATA_FACILITY_NDB)data.rgData[i];
        _sb.AppendLine( $"<{i:##0}>:  {item.Ident}({item.Region}) LLA({item.Latitude:00.0000},{item.Longitude:000.0000},{item.Altitude:##,##0}) mv({item.fMagVar:0#.00})" );
        _sb.AppendLine( $"            f({item.fFrequency})" );
      }
      string text = _sb.ToString( );
      _sb.Clear( );
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
    }

    /// <summary>
    /// Asynch call by FS having received Waypoint Lists
    /// </summary>
    private void _simConnect_OnRecvWaypointList( FS.SimConnect sender, FS.SIMCONNECT_RECV_WAYPOINT_LIST data )
    {
      _sb.AppendLine( "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" );
      _sb.AppendLine( $"_OnRecvWaypointList: RqID<{data.dwRequestID}> msgIndex:{data.dwEntryNumber} #msgs:{data.dwOutOf} #records:{data.dwArraySize}" );

      for (int i = 0; i < data.dwArraySize; i++) {
        var item = (FS.SIMCONNECT_DATA_FACILITY_WAYPOINT)data.rgData[i];
        _sb.AppendLine( $"<{i:##0}>:  {item.Ident}({item.Region}) LLA({item.Latitude:00.0000},{item.Longitude:000.0000},{item.Altitude:##,##0}) mv({item.fMagVar:0#.00})" );
      }
      string text = _sb.ToString( );
      _sb.Clear( );
      // invoke this on the Form not in the callback..
      _invoker.HandleEvent( ( ) => {
        RTB.Text += text;
      } );
    }


    #endregion

  }
}
