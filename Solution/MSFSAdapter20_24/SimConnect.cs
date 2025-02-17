using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace MSFSAdapter20_24
{

  /// <summary>
  /// The FS Version detected
  /// </summary>
  public enum FSVersion
  {
    Unknown = 0,
    V2020,
    V2024,
  }

  /// <summary>
  /// Adapter to seamlessly connect MSFS 2020 or 2024 SimConnect
  /// 
  /// It exposes the MSFS2024 specifications for interfacing 
  ///  MSFS2020 breaking changes are taken care of and it should work seamlessly
  ///  Methods not available will raise an Exception callback but should otherwise not cause harm
  ///  
  /// !! The Adapter can only progress and connect with SimConnect once a running MSFS2020 or 2024 App is detected
  /// 
  /// To use the Adapter, create an instance with the same parameters as the original SimConnect object
  ///  THEN call Init() to detect a running MSFS instance
  ///  if not successfull Init() returns Unknown and you may call later again until success or give up
  ///  Note: Init may raise Exceptions if something went wrong and the Adapter cannot work.
  ///  
  /// Once Init is successfull use the Adapter exactly like the original SimConnect obj.
  /// The original Const, Enums, Structs, Events, and Methods are exposed and used by the Adapter, 
  /// -> there is no need to reference the managed SimConnect DLL (else it will not work...)
  ///  
  /// </summary>
  public class SimConnect : IDisposable
  {
    /*
     * Base is FS2024 latest as of Dec 2024
     * 
      Updates:

      FS2024 SDK 1.1.2:
        SIMCONNECT_ENUMERATE_SIMOBJECT_LIVERY does not exist in 2020
        OnRecvEnumerateSimobjectAndLiveryList 	RecvEnumerateSimobjectAndLiveryListEventHandler  does not exist in 2020
        Microsoft.FlightSimulator.SimConnect.SimConnect+RecvEnumerateSimobjectAndLiveryListEventHandler   does not exist in 2020
        SIMCONNECT_RECV_ID new ENUM: ENUMERATE_SIMOBJECT_AND_LIVERY_LIST = 38
        SIMCONNECT_EXCEPTION new ENUM: INTERNAL = 44
        SIMCONNECT_SIMOBJECT_TYPE new ENUM: HOT_AIR_BALLOON = 6, ANIMAL = 7
        SIMCONNECT_FACILITY_DATA_TYPE new ENUM: VDGS = 26, HOLDING_PATTERN = 27
     
     */

    private const string c_PlugAssemblyName = "MSFSplug";
    private const string c_2020PlugName = "MSFSPlug2020.ScPlug2020";
    private const string c_2024PlugName = "MSFSPlug2024.ScPlug2024";

    private AppDomain currentDomain = null;
    private FSVersion _fsVersion = FSVersion.Unknown;
    private string _fsWindowTitle = "";

    private ISimConnectA _plug;

    // debug helper
    private StringBuilder _sb = new StringBuilder( );

    // call params from cTor, used to connect a Plug
    private string _szName;
    private IntPtr _hWnd;
    private uint _UserEventWin32;
    private WaitHandle _hEventHandle;
    private uint _ConfigIndex;

    #region Original SimConnect API

    // copied 1:1 from SimConnect MSFS2024

    // this delegate is not converted for now - don't know if it is used in managed code anyway
    // public delegate void SignalProcDelegate( SIMCONNECT_RECV pData, uint cbData );

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public class InputEventString
    {
      [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 256 )]
      public string value;
    }

    public delegate void RecvNullEventHandler( SimConnect sender, SIMCONNECT_RECV data );
    public delegate void RecvExceptionEventHandler( SimConnect sender, SIMCONNECT_RECV_EXCEPTION data );
    public delegate void RecvOpenEventHandler( SimConnect sender, SIMCONNECT_RECV_OPEN data );
    public delegate void RecvQuitEventHandler( SimConnect sender, SIMCONNECT_RECV data );
    public delegate void RecvEventEventHandler( SimConnect sender, SIMCONNECT_RECV_EVENT data );
    public delegate void RecvEventObjectAddremoveEventHandler( SimConnect sender, SIMCONNECT_RECV_EVENT_OBJECT_ADDREMOVE data );
    public delegate void RecvEventFilenameEventHandler( SimConnect sender, SIMCONNECT_RECV_EVENT_FILENAME data );
    public delegate void RecvEventFrameEventHandler( SimConnect sender, SIMCONNECT_RECV_EVENT_FRAME data );
    public delegate void RecvSimobjectDataEventHandler( SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data );
    public delegate void RecvSimobjectDataBytypeEventHandler( SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data );
    public delegate void RecvWeatherObservationEventHandler( SimConnect sender, SIMCONNECT_RECV_WEATHER_OBSERVATION data );
    public delegate void RecvCloudStateEventHandler( SimConnect sender, SIMCONNECT_RECV_CLOUD_STATE data );
    public delegate void RecvAssignedObjectIdEventHandler( SimConnect sender, SIMCONNECT_RECV_ASSIGNED_OBJECT_ID data );
    public delegate void RecvReservedKeyEventHandler( SimConnect sender, SIMCONNECT_RECV_RESERVED_KEY data );
    public delegate void RecvCustomActionEventHandler( SimConnect sender, SIMCONNECT_RECV_CUSTOM_ACTION data );
    public delegate void RecvSystemStateEventHandler( SimConnect sender, SIMCONNECT_RECV_SYSTEM_STATE data );
    public delegate void RecvClientDataEventHandler( SimConnect sender, SIMCONNECT_RECV_CLIENT_DATA data );
    public delegate void RecvEventWeatherModeEventHandler( SimConnect sender, SIMCONNECT_RECV_EVENT_WEATHER_MODE data );
    public delegate void RecvAirportListEventHandler( SimConnect sender, SIMCONNECT_RECV_AIRPORT_LIST data );
    public delegate void RecvVorListEventHandler( SimConnect sender, SIMCONNECT_RECV_VOR_LIST data );
    public delegate void RecvNdbListEventHandler( SimConnect sender, SIMCONNECT_RECV_NDB_LIST data );
    public delegate void RecvWaypointListEventHandler( SimConnect sender, SIMCONNECT_RECV_WAYPOINT_LIST data );
    public delegate void RecvEventMultiplayerServerStartedEventHandler( SimConnect sender, SIMCONNECT_RECV_EVENT_MULTIPLAYER_SERVER_STARTED data );
    public delegate void RecvEventMultiplayerClientStartedEventHandler( SimConnect sender, SIMCONNECT_RECV_EVENT_MULTIPLAYER_CLIENT_STARTED data );
    public delegate void RecvEventMultiplayerSessionEndedEventHandler( SimConnect sender, SIMCONNECT_RECV_EVENT_MULTIPLAYER_SESSION_ENDED data );
    public delegate void RecvEventRaceEndEventHandler( SimConnect sender, SIMCONNECT_RECV_EVENT_RACE_END data );
    public delegate void RecvEventRaceLapEventHandler( SimConnect sender, SIMCONNECT_RECV_EVENT_RACE_LAP data );
    public delegate void RecvEventEx1EventHandler( SimConnect sender, SIMCONNECT_RECV_EVENT_EX1 data );
    public delegate void RecvFacilityDataEventHandler( SimConnect sender, SIMCONNECT_RECV_FACILITY_DATA data );
    public delegate void RecvFacilityDataEndEventHandler( SimConnect sender, SIMCONNECT_RECV_FACILITY_DATA_END data );
    public delegate void RecvFacilityMinimalListEventHandler( SimConnect sender, SIMCONNECT_RECV_FACILITY_MINIMAL_LIST data );
    public delegate void RecvJetwayDataEventHandler( SimConnect sender, SIMCONNECT_RECV_JETWAY_DATA data );
    public delegate void RecvControllersListEventHandler( SimConnect sender, SIMCONNECT_RECV_CONTROLLERS_LIST data );
    public delegate void RecvActionCallbackEventHandler( SimConnect sender, SIMCONNECT_RECV_ACTION_CALLBACK data );
    public delegate void RecvEnumerateInputEventsEventHandler( SimConnect sender, SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS data );
    public delegate void RecvGetInputEventEventHandler( SimConnect sender, SIMCONNECT_RECV_GET_INPUT_EVENT data );
    public delegate void RecvSubscribeInputEventEventHandler( SimConnect sender, SIMCONNECT_RECV_SUBSCRIBE_INPUT_EVENT data );
    public delegate void RecvEnumerateInputEventParamsEventHandler( SimConnect sender, SIMCONNECT_RECV_ENUMERATE_INPUT_EVENT_PARAMS data );
    // added in 2024 SDK 1.1.2
    public delegate void RecvEnumerateSimobjectAndLiveryListEventHandler( SimConnect sender, SIMCONNECT_RECV_ENUMERATE_SIMOBJECT_AND_LIVERY_LIST data );

    public event RecvEnumerateInputEventParamsEventHandler OnRecvEnumerateInputEventParams;
    public event RecvSubscribeInputEventEventHandler OnRecvSubscribeInputEvent;
    public event RecvGetInputEventEventHandler OnRecvGetInputEvent;
    public event RecvEnumerateInputEventsEventHandler OnRecvEnumerateInputEvents;
    public event RecvActionCallbackEventHandler OnRecvActionCallback;
    public event RecvControllersListEventHandler OnRecvControllersList;
    public event RecvJetwayDataEventHandler OnRecvJetwayData;
    public event RecvFacilityMinimalListEventHandler OnRecvFacilityMinimalList;
    public event RecvFacilityDataEndEventHandler OnRecvFacilityDataEnd;
    public event RecvFacilityDataEventHandler OnRecvFacilityData;
    public event RecvEventEx1EventHandler OnRecvEventEx1;
    public event RecvEventRaceLapEventHandler OnRecvEventRaceLap;
    public event RecvEventRaceEndEventHandler OnRecvEventRaceEnd;
    public event RecvEventMultiplayerSessionEndedEventHandler OnRecvEventMultiplayerSessionEnded;
    public event RecvEventMultiplayerClientStartedEventHandler OnRecvEventMultiplayerClientStarted;
    public event RecvEventMultiplayerServerStartedEventHandler OnRecvEventMultiplayerServerStarted;
    public event RecvWaypointListEventHandler OnRecvWaypointList;
    public event RecvNdbListEventHandler OnRecvNdbList;
    public event RecvVorListEventHandler OnRecvVorList;
    public event RecvAirportListEventHandler OnRecvAirportList;
    public event RecvEventWeatherModeEventHandler OnRecvEventWeatherMode;
    public event RecvClientDataEventHandler OnRecvClientData;
    public event RecvSystemStateEventHandler OnRecvSystemState;
    public event RecvCustomActionEventHandler OnRecvCustomAction;
    public event RecvReservedKeyEventHandler OnRecvReservedKey;
    public event RecvAssignedObjectIdEventHandler OnRecvAssignedObjectId;
    public event RecvCloudStateEventHandler OnRecvCloudState;
    public event RecvWeatherObservationEventHandler OnRecvWeatherObservation;
    public event RecvSimobjectDataBytypeEventHandler OnRecvSimobjectDataBytype;
    public event RecvSimobjectDataEventHandler OnRecvSimobjectData;
    public event RecvEventFrameEventHandler OnRecvEventFrame;
    public event RecvEventFilenameEventHandler OnRecvEventFilename;
    public event RecvEventObjectAddremoveEventHandler OnRecvEventObjectAddremove;
    public event RecvEventEventHandler OnRecvEvent;
    public event RecvQuitEventHandler OnRecvQuit;
    public event RecvOpenEventHandler OnRecvOpen;
    public event RecvExceptionEventHandler OnRecvException;
    public event RecvNullEventHandler OnRecvNull;
    // added in 2024 SDK 1.1.2
    public event RecvEnumerateSimobjectAndLiveryListEventHandler OnRecvEnumerateSimobjectAndLiveryList;

    public static uint SIMCONNECT_UNUSED = uint.MaxValue;
    public static uint SIMCONNECT_OBJECT_ID_USER = 0u;
    public static float SIMCONNECT_CAMERA_IGNORE_FIELD = float.MaxValue;
    public static uint SIMCONNECT_CLIENTDATA_MAX_SIZE = 8192u;
    public static uint SIMCONNECT_GROUP_PRIORITY_HIGHEST = 1u;
    public static uint SIMCONNECT_GROUP_PRIORITY_HIGHEST_MASKABLE = 10000000u;
    public static uint SIMCONNECT_GROUP_PRIORITY_STANDARD = 1900000000u;
    public static uint SIMCONNECT_GROUP_PRIORITY_DEFAULT = 2000000000u;
    public static uint SIMCONNECT_GROUP_PRIORITY_LOWEST = 4000000000u;
    public static uint MAX_METAR_LENGTH = 2000u;
    public static float MAX_THERMAL_SIZE = 100000f;
    public static float MAX_THERMAL_RATE = 1000f;
    public static uint INITPOSITION_AIRSPEED_CRUISE = uint.MaxValue;
    public static uint INITPOSITION_AIRSPEED_KEEP = 4294967294u;
    public static uint SIMCONNECT_CLIENTDATATYPE_INT8 = uint.MaxValue;
    public static uint SIMCONNECT_CLIENTDATATYPE_INT16 = 4294967294u;
    public static uint SIMCONNECT_CLIENTDATATYPE_INT32 = 4294967293u;
    public static uint SIMCONNECT_CLIENTDATATYPE_INT64 = 4294967292u;
    public static uint SIMCONNECT_CLIENTDATATYPE_FLOAT32 = 4294967291u;
    public static uint SIMCONNECT_CLIENTDATATYPE_FLOAT64 = 4294967290u;
    public static uint SIMCONNECT_CLIENTDATAOFFSET_AUTO = uint.MaxValue;
    public static uint SIMCONNECT_OPEN_CONFIGINDEX_LOCAL = uint.MaxValue;
    public static int SIMCONNECT_CLOUD_STATE_ARRAY_WIDTH = 64;
    public static int SIMCONNECT_CLOUD_STATE_ARRAY_SIZE = 4096;

    #region Call forwarder

    public int TestMarshaling( )
    {
      int? result = _plug?.TestMarshaling( );
      if (result.HasValue) return result.Value;
      throw new Exception( "TestMarshaling failed" ); // subst if it does not work as expected
    }
    public uint GetLastSentPacketID( )
    {
      uint? result = _plug?.GetLastSentPacketID( );
      if (result.HasValue) return result.Value;
      throw new Exception( "GetLastSentPacketID failed" );
    }

    public void RegisterDataDefineStruct<T>( Enum dwID )
      => _plug?.RegisterDataDefineStruct<T>( dwID );
    public void RegisterFacilityDataDefineStruct<T>( SIMCONNECT_FACILITY_DATA_TYPE dwType )
      => _plug?.RegisterFacilityDataDefineStruct<T>( dwType );

    /* Don't know how to convert this one...  provided is a derived SIMCONNECT_RECV struct
      public void RegisterStruct<RECV, T>( Enum dwID ) where RECV : SIMCONNECT_RECV
        => _plug?.RegisterStruct<RECV, T>( dwID );

      public void ReceiveDispatch( SignalProcDelegate pfcnSignal )      => _plug?.ReceiveDispatch( pfcnSignal );
    */

    public void ReceiveMessage( )
      => _plug?.ReceiveMessage( );
    public void MapClientEventToSimEvent( Enum EventID, string EventName )
      => _plug?.MapClientEventToSimEvent( EventID, EventName );
    public void TransmitClientEvent( uint ObjectID, Enum EventID, uint dwData, Enum GroupID, SIMCONNECT_EVENT_FLAG Flags )
      => _plug?.TransmitClientEvent( ObjectID, EventID, dwData, GroupID, Flags );
    public void SetSystemEventState( Enum EventID, SIMCONNECT_STATE dwState )
      => _plug?.SetSystemEventState( EventID, dwState );
    public void AddClientEventToNotificationGroup( Enum GroupID, Enum EventID, [MarshalAs( UnmanagedType.U1 )] bool bMaskable )
      => _plug?.AddClientEventToNotificationGroup( GroupID, EventID, bMaskable );
    public void RemoveClientEvent( Enum GroupID, Enum EventID )
      => _plug?.RemoveClientEvent( GroupID, EventID );
    public void SetNotificationGroupPriority( Enum GroupID, uint uPriority )
      => _plug?.SetNotificationGroupPriority( GroupID, uPriority );
    public void ClearNotificationGroup( Enum GroupID )
      => _plug?.ClearNotificationGroup( GroupID );
    public void RequestNotificationGroup( Enum GroupID, uint dwReserved, uint Flags )
      => _plug?.RequestNotificationGroup( GroupID, dwReserved, Flags );
    public void AddToDataDefinition( Enum DefineID, string DatumName, string UnitsName, SIMCONNECT_DATATYPE DatumType, float fEpsilon, uint DatumID )
      => _plug?.AddToDataDefinition( DefineID, DatumName, UnitsName, DatumType, fEpsilon, DatumID );
    public void ClearDataDefinition( Enum DefineID ) => _plug?.ClearDataDefinition( DefineID );
    public void RequestDataOnSimObject( Enum RequestID, Enum DefineID, uint ObjectID, SIMCONNECT_PERIOD Period, SIMCONNECT_DATA_REQUEST_FLAG Flags, uint origin, uint interval, uint limit )
      => _plug?.RequestDataOnSimObject( RequestID, DefineID, ObjectID, Period, Flags, origin, interval, limit );
    public void RequestDataOnSimObjectType( Enum RequestID, Enum DefineID, uint dwRadiusMeters, SIMCONNECT_SIMOBJECT_TYPE type )
      => _plug?.RequestDataOnSimObjectType( RequestID, DefineID, dwRadiusMeters, type );
    public void SetDataOnSimObject( Enum DefineID, uint ObjectID, SIMCONNECT_DATA_SET_FLAG Flags, object pDataSet )
      => _plug?.SetDataOnSimObject( DefineID, ObjectID, Flags, pDataSet );
    public void MapInputEventToClientEvent( Enum GroupID, string szInputDefinition, Enum DownEventID, uint DownValue, Enum UpEventID, uint UpValue, [MarshalAs( UnmanagedType.U1 )] bool bMaskable )
      => _plug?.MapInputEventToClientEvent( GroupID, szInputDefinition, DownEventID, DownValue, UpEventID, UpValue, bMaskable );
    public void SetInputGroupPriority( Enum GroupID, uint uPriority )
      => _plug?.SetInputGroupPriority( GroupID, uPriority );
    public void RemoveInputEvent( Enum GroupID, string szInputDefinition )
      => _plug?.RemoveInputEvent( GroupID, szInputDefinition );
    public void ClearInputGroup( Enum GroupID )
      => _plug?.ClearInputGroup( GroupID );
    public void SetInputGroupState( Enum GroupID, uint dwState )
      => _plug?.SetInputGroupState( GroupID, dwState );
    public void RequestReservedKey( Enum EventID, string szKeyChoice1, string szKeyChoice2, string szKeyChoice3 )
      => _plug?.RequestReservedKey( EventID, szKeyChoice1, szKeyChoice2, szKeyChoice3 );
    public void SubscribeToSystemEvent( Enum EventID, string SystemEventName )
      => _plug?.SubscribeToSystemEvent( EventID, SystemEventName );
    public void UnsubscribeFromSystemEvent( Enum EventID )
      => _plug?.UnsubscribeFromSystemEvent( EventID );
    public void WeatherRequestInterpolatedObservation( Enum RequestID, float lat, float lon, float alt )
      => _plug?.WeatherRequestInterpolatedObservation( RequestID, lat, lon, alt );
    public void WeatherRequestObservationAtStation( Enum RequestID, string szICAO )
      => _plug?.WeatherRequestObservationAtStation( RequestID, szICAO );
    public void WeatherRequestObservationAtNearestStation( Enum RequestID, float lat, float lon )
      => _plug?.WeatherRequestObservationAtNearestStation( RequestID, lat, lon );
    public void WeatherCreateStation( Enum RequestID, string szICAO, string szName, float lat, float lon, float alt )
      => _plug?.WeatherCreateStation( RequestID, szICAO, szName, lat, lon, alt );
    public void WeatherRemoveStation( Enum RequestID, string szICAO )
      => _plug?.WeatherRemoveStation( RequestID, szICAO );
    public void WeatherSetObservation( uint Seconds, string szMETAR )
      => _plug?.WeatherSetObservation( Seconds, szMETAR );
    public void WeatherSetModeServer( uint dwPort, uint dwSeconds )
      => _plug?.WeatherSetModeServer( dwPort, dwSeconds );
    public void WeatherSetModeTheme( string szThemeName )
      => _plug?.WeatherSetModeTheme( szThemeName );
    public void WeatherSetModeGlobal( )
      => _plug?.WeatherSetModeGlobal( );
    public void WeatherSetModeCustom( )
      => _plug?.WeatherSetModeCustom( );
    public void WeatherSetDynamicUpdateRate( uint dwRate )
      => _plug?.WeatherSetDynamicUpdateRate( dwRate );
    public void WeatherRequestCloudState( Enum RequestID, float minLat, float minLon, float minAlt, float maxLat, float maxLon, float maxAlt, uint dwFlags )
      => _plug?.WeatherRequestCloudState( RequestID, minLat, minLon, minAlt, maxLat, maxLon, maxAlt, dwFlags );
    public void WeatherCreateThermal( Enum RequestID, float lat, float lon, float alt, float radius, float height, float coreRate, float coreTurbulence, float sinkRate, float sinkTurbulence, float coreSize, float coreTransitionSize, float sinkLayerSize, float sinkTransitionSize )
      => _plug?.WeatherCreateThermal( RequestID, lat, lon, alt, radius, height, coreRate, coreTurbulence, sinkRate, sinkTurbulence, coreSize, coreTransitionSize, sinkLayerSize, sinkTransitionSize );
    public void WeatherRemoveThermal( uint ObjectID )
      => _plug?.WeatherRemoveThermal( ObjectID );
    public void AICreateParkedATCAircraft( string szContainerTitle, string szTailNumber, string szAirportID, Enum RequestID )
      => _plug?.AICreateParkedATCAircraft( szContainerTitle, szTailNumber, szAirportID, RequestID );
    public void AICreateEnrouteATCAircraft( string szContainerTitle, string szTailNumber, int iFlightNumber, string szFlightPlanPath, double dFlightPlanPosition, [MarshalAs( UnmanagedType.U1 )] bool bTouchAndGo, Enum RequestID )
      => _plug?.AICreateEnrouteATCAircraft( szContainerTitle, szTailNumber, iFlightNumber, szFlightPlanPath, dFlightPlanPosition, bTouchAndGo, RequestID );
    public void AICreateNonATCAircraft( string szContainerTitle, string szTailNumber, SIMCONNECT_DATA_INITPOSITION InitPos, Enum RequestID )
      => _plug?.AICreateNonATCAircraft( szContainerTitle, szTailNumber, InitPos, RequestID );
    public void AICreateSimulatedObject( string szContainerTitle, SIMCONNECT_DATA_INITPOSITION InitPos, Enum RequestID )
      => _plug?.AICreateSimulatedObject( szContainerTitle, InitPos, RequestID );
    public void AIReleaseControl( uint ObjectID, Enum RequestID )
      => _plug?.AIReleaseControl( ObjectID, RequestID );
    public void AIRemoveObject( uint ObjectID, Enum RequestID )
      => _plug?.AIRemoveObject( ObjectID, RequestID );
    public void AISetAircraftFlightPlan( uint ObjectID, string szFlightPlanPath, Enum RequestID )
      => _plug?.AISetAircraftFlightPlan( ObjectID, szFlightPlanPath, RequestID );
    public void ExecuteMissionAction( ValueType guidInstanceId )
      => _plug?.ExecuteMissionAction( guidInstanceId );
    public void CompleteCustomMissionAction( ValueType guidInstanceId )
      => _plug?.CompleteCustomMissionAction( guidInstanceId );
    public void CameraSetRelative6DOF( float fDeltaX, float fDeltaY, float fDeltaZ, float fPitchDeg, float fBankDeg, float fHeadingDeg )
      => _plug?.CameraSetRelative6DOF( fDeltaX, fDeltaY, fDeltaZ, fPitchDeg, fBankDeg, fHeadingDeg );
    public void MenuAddItem( string szMenuItem, Enum MenuEventID, uint dwData )
      => _plug?.MenuAddItem( szMenuItem, MenuEventID, dwData );
    public void MenuDeleteItem( Enum MenuEventID )
      => _plug?.MenuDeleteItem( MenuEventID );
    public void MenuAddSubItem( Enum MenuEventID, string szMenuItem, Enum SubMenuEventID, uint dwData )
      => _plug?.MenuAddSubItem( MenuEventID, szMenuItem, SubMenuEventID, dwData );
    public void MenuDeleteSubItem( Enum MenuEventID, Enum SubMenuEventID )
      => _plug?.MenuDeleteSubItem( MenuEventID, SubMenuEventID );
    public void RequestSystemState( Enum RequestID, string szState )
      => _plug?.RequestSystemState( RequestID, szState );
    public void SetSystemState( string szState, uint dwInteger, float fFloat, string szString )
      => _plug?.SetSystemState( szState, dwInteger, fFloat, szString );
    public void MapClientDataNameToID( string szClientDataName, Enum ClientDataID )
      => _plug?.MapClientDataNameToID( szClientDataName, ClientDataID );
    public void CreateClientData( Enum ClientDataID, uint dwSize, SIMCONNECT_CREATE_CLIENT_DATA_FLAG Flags )
      => _plug?.CreateClientData( ClientDataID, dwSize, Flags );
    public void AddToClientDataDefinition( Enum DefineID, uint dwOffset, uint dwSizeOrType, float fEpsilon, uint DatumID )
      => _plug?.AddToClientDataDefinition( DefineID, dwOffset, dwSizeOrType, fEpsilon, DatumID );
    public void ClearClientDataDefinition( Enum DefineID )
      => _plug?.ClearClientDataDefinition( DefineID );
    public void RequestClientData( Enum ClientDataID, Enum RequestID, Enum DefineID, SIMCONNECT_CLIENT_DATA_PERIOD Period, SIMCONNECT_CLIENT_DATA_REQUEST_FLAG Flags, uint origin, uint interval, uint limit )
      => _plug?.RequestClientData( ClientDataID, RequestID, DefineID, Period, Flags, origin, interval, limit );
    public void SetClientData( Enum ClientDataID, Enum DefineID, SIMCONNECT_CLIENT_DATA_SET_FLAG Flags, uint dwReserved, object pDataSet )
      => _plug?.SetClientData( ClientDataID, DefineID, Flags, dwReserved, pDataSet );
    public void FlightLoad( string szFileName )
      => _plug?.FlightLoad( szFileName );
    public void FlightSave( string szFileName, string szTitle, string szDescription, uint Flags )
      => _plug?.FlightSave( szFileName, szTitle, szDescription, Flags );
    public void FlightPlanLoad( string szFileName )
      => _plug?.FlightPlanLoad( szFileName );
    public void Text( SIMCONNECT_TEXT_TYPE type, float fTimeSeconds, Enum EventID, object pDataSet )
      => _plug?.Text( type, fTimeSeconds, EventID, pDataSet );
    public void SubscribeToFacilities( SIMCONNECT_FACILITY_LIST_TYPE type, Enum RequestID )
      => _plug?.SubscribeToFacilities( type, RequestID );
    public void UnsubscribeToFacilities( SIMCONNECT_FACILITY_LIST_TYPE type )
      => _plug?.UnsubscribeToFacilities( type );
    public void RequestFacilitiesList( SIMCONNECT_FACILITY_LIST_TYPE type, Enum RequestID )
      => _plug?.RequestFacilitiesList( type, RequestID );
    public void TransmitClientEvent_EX1( uint ObjectID, Enum EventID, Enum GroupID, SIMCONNECT_EVENT_FLAG Flags, uint dwData0, uint dwData1, uint dwData2, uint dwData3, uint dwData4 )
      => _plug?.TransmitClientEvent_EX1( ObjectID, EventID, GroupID, Flags, dwData0, dwData1, dwData2, dwData3, dwData4 );
    public void AddToFacilityDefinition( Enum DefineID, string FieldName )
      => _plug?.AddToFacilityDefinition( DefineID, FieldName );
    public void RequestFacilityData( Enum DefineID, Enum RequestID, string ICAO, string Region )
      => _plug?.RequestFacilityData( DefineID, RequestID, ICAO, Region );
    public void SubscribeToFacilities_EX1( SIMCONNECT_FACILITY_LIST_TYPE type, Enum newElemInRangeRequestID, Enum oldElemOutRangeRequestID )
      => _plug?.SubscribeToFacilities_EX1( type, newElemInRangeRequestID, oldElemOutRangeRequestID );
    public void UnsubscribeToFacilities_EX1( SIMCONNECT_FACILITY_LIST_TYPE type, [MarshalAs( UnmanagedType.U1 )] bool bUnsubscribeNewInRange, [MarshalAs( UnmanagedType.U1 )] bool bUnsubscribeOldOutRange )
      => _plug?.UnsubscribeToFacilities_EX1( type, bUnsubscribeNewInRange, bUnsubscribeOldOutRange );
    public void RequestFacilitiesList_EX1( SIMCONNECT_FACILITY_LIST_TYPE type, Enum RequestID )
      => _plug?.RequestFacilitiesList_EX1( type, RequestID );
    public void RequestFacilityData_EX1( Enum DefineID, Enum RequestID, string ICAO, string Region, sbyte Type )
      => _plug?.RequestFacilityData_EX1( DefineID, RequestID, ICAO, Region, Type );
    public void EnumerateControllers( )
      => _plug?.EnumerateControllers( );
    public void MapInputEventToClientEvent_EX1( Enum GroupID, string szInputDefinition, Enum DownEventID, uint DownValue, Enum UpEventID, uint UpValue, [MarshalAs( UnmanagedType.U1 )] bool bMaskable )
      => _plug?.MapInputEventToClientEvent_EX1( GroupID, szInputDefinition, DownEventID, DownValue, UpEventID, UpValue, bMaskable );
    public void EnumerateInputEvents( Enum RequestID )
      => _plug?.EnumerateInputEvents( RequestID );
    public void GetInputEvent( Enum RequestID, ulong Hash )
      => _plug?.GetInputEvent( RequestID, Hash );
    public void SetInputEvent( ulong Hash, object Value )
      => _plug?.SetInputEvent( Hash, Value );
    public void SubscribeInputEvent( ulong Hash )
      => _plug?.SubscribeInputEvent( Hash );
    public void UnsubscribeInputEvent( ulong Hash )
      => _plug?.UnsubscribeInputEvent( Hash );
    public void EnumerateInputEventParams( ulong Hash )
      => _plug?.EnumerateInputEventParams( Hash );
    public void AddFacilityDataDefinitionFilter( Enum DefineID, string szFilterPath, object pFilterData )
      => _plug?.AddFacilityDataDefinitionFilter( DefineID, szFilterPath, pFilterData );
    public void ClearAllFacilityDataDefinitionFilters( Enum DefineID )
      => _plug?.ClearAllFacilityDataDefinitionFilters( DefineID );
    public void RequestResponseTimes( float[] fElapsedSeconds )
      => _plug?.RequestResponseTimes( fElapsedSeconds );
    public void RequestJetwayData( string AirportIcao, ICollection<int> Indexes )
      => _plug?.RequestJetwayData( AirportIcao, Indexes );

    // added in 2024 SDK 1.1.2
    public void EnumerateSimObjectsAndLiveries( Enum RequestID, SIMCONNECT_SIMOBJECT_TYPE Type )
      => _plug?.EnumerateSimObjectsAndLiveries( RequestID, Type );

    #endregion // Call Forwarder


    #endregion // SimConnect API

    #region SimConnect Xtended API

    /// <summary>
    /// User-defined Win32 event for SimConnect
    ///   can be used as UserEventWin32 when creating the instance
    /// </summary>
    public const int WM_USER_SIMCONNECT = 0x0402;

    /// <summary>
    /// User-defined Win32 event for SimConnect NONE
    ///   can be used as UserEventWin32 when creating the instance and the Win MsgQueue is no used
    /// </summary>
    public const int WM_USER_NONE = 0x0;

    /// <summary>
    /// The detected MSFS Version
    /// </summary>
    public FSVersion FSimVersion => _fsVersion;

    /// <summary>
    /// The detected WindowTitle (incl version info)
    /// </summary>
    public string FSimWindowTitle => _fsWindowTitle;

    /// <summary>
    /// cTor: Same as SimConnect
    ///   Use Init() to start
    ///   
    /// </summary>
    /// <param name="szName">string containing an appropriate name for the client program</param>
    /// <param name="hWnd">Handle to a Windows object. Set this to NULL if the handle is not being used. UserEventWin32</param>
    /// <param name="UserEventWin32">Code number that the client can specify. Set this to 0 if it is not being used.</param>
    /// <param name="hEventHandle">A Windows Event handle. A client can be written to respond to Windows Events, rather than use a polling and callback system, 
    ///                             which can be a more efficient process if the client does not have to 
    ///                             respond very frequently to changes in data in Microsoft Flight Simulator 2024.</param>
    /// <param name="ConfigIndex">The configuration index. (SimConnect.cfg)</param>
    public SimConnect( string szName, IntPtr hWnd, uint UserEventWin32, WaitHandle hEventHandle, uint ConfigIndex )
    {
      _szName = szName;
      _hWnd = hWnd;
      _UserEventWin32 = UserEventWin32;
      _ConfigIndex = ConfigIndex;
      _hEventHandle = hEventHandle;
      _ConfigIndex = ConfigIndex;
    }

    /// <summary>
    /// Initialize the Adapter
    ///  needs a running MSFS instance to start successfully
    ///  It will connect the SimConnect server and wire callbacks
    ///  
    /// When successfull the Adapter should be ready and working
    /// 
    /// </summary>
    /// <param name="forcedVersion">Use a specific Version to omit the Version detection</param>
    /// <returns>The detected MSFS Version when successfull, else Unknown</returns>
    /// <exception cref="Exception">Raised when the Adapter cannot load or connect but should</exception>
    public FSVersion Init( FSVersion forcedVersion = FSVersion.Unknown )
    {
      _sb.Clear( );

      // this should raise events when an assembly cannot be resolved while loading
      currentDomain = AppDomain.CurrentDomain;
      currentDomain.AssemblyResolve += new ResolveEventHandler( MyResolveEventHandler );

      // find version
      if (forcedVersion == FSVersion.Unknown) {
        _fsVersion = DetectVersion( );
        Debug.WriteLine( $"ADAPTER - MSFS Version detected: {_fsVersion}" );
      }
      else {
        _fsVersion = forcedVersion;
      }

      if (_fsVersion == FSVersion.Unknown) {
        return _fsVersion; // cannot detect a running MSFS instance
      }

      try {
        // loads from: .\V2020\MSFSplug.dll or .\V2024\MSFSplug.dll
        var plugAssembly = Assembly.LoadFrom( Path.Combine( ".", $"{_fsVersion}", c_PlugAssemblyName + ".dll" ) );
        if (_fsVersion == FSVersion.V2020) {
          _plug = (ISimConnectA)plugAssembly.CreateInstance( c_2020PlugName );
        }
        else if (_fsVersion == FSVersion.V2024) {
          _plug = (ISimConnectA)plugAssembly.CreateInstance( c_2024PlugName );
        }

        // the loader seems to pull the corresponding Microsoft DLLs from this folder as well without to have it resolved by the callback??!!
      }
      catch {
        ;
      }
      if (_plug == null) {
        throw new Exception( "Loading of the SimConnect Adapter Assembly failed" ); // Cannot continue
      }

      // pull SimConnect with values from the cTor
      bool result = _plug.Open( _szName, _hWnd, _UserEventWin32, _hEventHandle, _ConfigIndex );
      if (result == false) {
        return FSVersion.Unknown; // cannot open, usually the SIM is not running now
      }

      // wire events
      try {
        AttachHandlers( );
      }
      catch {
        throw new Exception( "Wiring Events from the Adapter Assembly failed" ); // Cannot continue
      }

      // from what we collected
      Debug.Write( _sb.ToString( ) );
      // and finish
      Debug.WriteLine( $"ADAPTER - Plug connected and active: {_fsVersion}" );
      return _fsVersion;
    }

    #endregion

    #region Event Wiring

    // attach all Plug handlers when possible
    private void AttachHandlers( )
    {
      // sanity
      if (_plug == null) throw new InvalidOperationException( "_plug is null, cannot continue" ); // PROGRAM ERROR

      // just forward any received event with it's data
      _plug.OnRecvActionCallback += ( SimConnect sender, SIMCONNECT_RECV_ACTION_CALLBACK data ) => { OnRecvActionCallback?.Invoke( this, data ); };
      _plug.OnRecvAirportList += ( SimConnect sender, SIMCONNECT_RECV_AIRPORT_LIST data ) => { OnRecvAirportList?.Invoke( this, data ); };
      _plug.OnRecvAssignedObjectId += ( SimConnect sender, SIMCONNECT_RECV_ASSIGNED_OBJECT_ID data ) => { OnRecvAssignedObjectId?.Invoke( this, data ); };
      _plug.OnRecvClientData += ( SimConnect sender, SIMCONNECT_RECV_CLIENT_DATA data ) => { OnRecvClientData?.Invoke( this, data ); };
      _plug.OnRecvCloudState += ( SimConnect sender, SIMCONNECT_RECV_CLOUD_STATE data ) => { OnRecvCloudState?.Invoke( this, data ); };
      _plug.OnRecvControllersList += ( SimConnect sender, SIMCONNECT_RECV_CONTROLLERS_LIST data ) => { OnRecvControllersList?.Invoke( this, data ); };
      _plug.OnRecvCustomAction += ( SimConnect sender, SIMCONNECT_RECV_CUSTOM_ACTION data ) => { OnRecvCustomAction?.Invoke( this, data ); };
      _plug.OnRecvEnumerateInputEventParams += ( SimConnect sender, SIMCONNECT_RECV_ENUMERATE_INPUT_EVENT_PARAMS data ) => { OnRecvEnumerateInputEventParams?.Invoke( this, data ); };
      _plug.OnRecvEnumerateInputEvents += ( SimConnect sender, SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS data ) => { OnRecvEnumerateInputEvents?.Invoke( this, data ); };
      _plug.OnRecvEvent += ( SimConnect sender, SIMCONNECT_RECV_EVENT data ) => { OnRecvEvent?.Invoke( this, data ); };
      _plug.OnRecvEventEx1 += ( SimConnect sender, SIMCONNECT_RECV_EVENT_EX1 data ) => { OnRecvEventEx1?.Invoke( this, data ); };
      _plug.OnRecvEventFilename += ( SimConnect sender, SIMCONNECT_RECV_EVENT_FILENAME data ) => { OnRecvEventFilename?.Invoke( this, data ); };
      _plug.OnRecvEventFrame += ( SimConnect sender, SIMCONNECT_RECV_EVENT_FRAME data ) => { OnRecvEventFrame?.Invoke( this, data ); };
      _plug.OnRecvEventMultiplayerClientStarted += ( SimConnect sender, SIMCONNECT_RECV_EVENT_MULTIPLAYER_CLIENT_STARTED data ) => { OnRecvEventMultiplayerClientStarted?.Invoke( this, data ); };
      _plug.OnRecvEventMultiplayerServerStarted += ( SimConnect sender, SIMCONNECT_RECV_EVENT_MULTIPLAYER_SERVER_STARTED data ) => { OnRecvEventMultiplayerServerStarted?.Invoke( this, data ); };
      _plug.OnRecvEventMultiplayerSessionEnded += ( SimConnect sender, SIMCONNECT_RECV_EVENT_MULTIPLAYER_SESSION_ENDED data ) => { OnRecvEventMultiplayerSessionEnded?.Invoke( this, data ); };
      _plug.OnRecvEventObjectAddremove += ( SimConnect sender, SIMCONNECT_RECV_EVENT_OBJECT_ADDREMOVE data ) => { OnRecvEventObjectAddremove?.Invoke( this, data ); };
      _plug.OnRecvEventRaceEnd += ( SimConnect sender, SIMCONNECT_RECV_EVENT_RACE_END data ) => { OnRecvEventRaceEnd?.Invoke( this, data ); };
      _plug.OnRecvEventRaceLap += ( SimConnect sender, SIMCONNECT_RECV_EVENT_RACE_LAP data ) => { OnRecvEventRaceLap?.Invoke( this, data ); };
      _plug.OnRecvEventWeatherMode += ( SimConnect sender, SIMCONNECT_RECV_EVENT_WEATHER_MODE data ) => { OnRecvEventWeatherMode?.Invoke( this, data ); };
      _plug.OnRecvException += ( SimConnect sender, SIMCONNECT_RECV_EXCEPTION data ) => { OnRecvException?.Invoke( this, data ); };
      _plug.OnRecvFacilityData += ( SimConnect sender, SIMCONNECT_RECV_FACILITY_DATA data ) => { OnRecvFacilityData?.Invoke( this, data ); };
      _plug.OnRecvFacilityDataEnd += ( SimConnect sender, SIMCONNECT_RECV_FACILITY_DATA_END data ) => { OnRecvFacilityDataEnd?.Invoke( this, data ); };
      _plug.OnRecvFacilityMinimalList += ( SimConnect sender, SIMCONNECT_RECV_FACILITY_MINIMAL_LIST data ) => { OnRecvFacilityMinimalList?.Invoke( this, data ); };
      _plug.OnRecvGetInputEvent += ( SimConnect sender, SIMCONNECT_RECV_GET_INPUT_EVENT data ) => { OnRecvGetInputEvent?.Invoke( this, data ); };
      _plug.OnRecvJetwayData += ( SimConnect sender, SIMCONNECT_RECV_JETWAY_DATA data ) => { OnRecvJetwayData?.Invoke( this, data ); };
      _plug.OnRecvNdbList += ( SimConnect sender, SIMCONNECT_RECV_NDB_LIST data ) => { OnRecvNdbList?.Invoke( this, data ); };
      _plug.OnRecvNull += ( SimConnect sender, SIMCONNECT_RECV data ) => { OnRecvNull?.Invoke( this, data ); };
      _plug.OnRecvOpen += ( SimConnect sender, SIMCONNECT_RECV_OPEN data ) => { OnRecvOpen?.Invoke( this, data ); };
      _plug.OnRecvQuit += ( SimConnect sender, SIMCONNECT_RECV data ) => { OnRecvQuit?.Invoke( this, data ); };
      _plug.OnRecvReservedKey += ( SimConnect sender, SIMCONNECT_RECV_RESERVED_KEY data ) => { OnRecvReservedKey?.Invoke( this, data ); };
      _plug.OnRecvSimobjectData += ( SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data ) => { OnRecvSimobjectData?.Invoke( this, data ); };
      _plug.OnRecvSimobjectDataBytype += ( SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data ) => { OnRecvSimobjectDataBytype?.Invoke( this, data ); };
      _plug.OnRecvSubscribeInputEvent += ( SimConnect sender, SIMCONNECT_RECV_SUBSCRIBE_INPUT_EVENT data ) => { OnRecvSubscribeInputEvent?.Invoke( this, data ); };
      _plug.OnRecvSystemState += ( SimConnect sender, SIMCONNECT_RECV_SYSTEM_STATE data ) => { OnRecvSystemState?.Invoke( this, data ); };
      _plug.OnRecvVorList += ( SimConnect sender, SIMCONNECT_RECV_VOR_LIST data ) => { OnRecvVorList?.Invoke( this, data ); };
      _plug.OnRecvWaypointList += ( SimConnect sender, SIMCONNECT_RECV_WAYPOINT_LIST data ) => { OnRecvWaypointList?.Invoke( this, data ); };
      _plug.OnRecvWeatherObservation += ( SimConnect sender, SIMCONNECT_RECV_WEATHER_OBSERVATION data ) => { OnRecvWeatherObservation?.Invoke( this, data ); };
      // added in 2024 SDK 1.1.2
      _plug.OnRecvEnumerateSimobjectAndLiveryList += ( SimConnect sender, SIMCONNECT_RECV_ENUMERATE_SIMOBJECT_AND_LIVERY_LIST data ) => { OnRecvEnumerateSimobjectAndLiveryList?.Invoke( this, data ); };
    }

    #endregion


    // https://stackoverflow.com/questions/1373100/how-to-add-folder-to-assembly-search-path-at-runtime-in-net
    // helps to resolve assemblies
    private Assembly MyResolveEventHandler( object sender, ResolveEventArgs args )
    {
      _sb.AppendLine( $"ADAPTER - Assembly loaded: {args.Name}, {args.RequestingAssembly}" );

      //This handler is called only when the common language runtime tries to bind to the assembly and fails.

      //Retrieve the list of referenced assemblies in an array of AssemblyName.
      Assembly MyAssembly, objExecutingAssembly;
      string strTempAssmbPath = "";

      objExecutingAssembly = Assembly.GetExecutingAssembly( );
      AssemblyName[] arrReferencedAssmbNames = objExecutingAssembly.GetReferencedAssemblies( );

      //Loop through the array of referenced assembly names.
      foreach (AssemblyName strAssmbName in arrReferencedAssmbNames) {
        //Check for the assembly names that have raised the "AssemblyResolve" event.
        if (strAssmbName.FullName.Substring( 0, strAssmbName.FullName.IndexOf( "," ) ) == args.Name.Substring( 0, args.Name.IndexOf( "," ) )) {
          //Build the path of the assembly from where it has to be loaded.                
          strTempAssmbPath = Path.Combine( ".", $"{_fsVersion}", args.Name.Substring( 0, args.Name.IndexOf( "," ) ) + ".dll" );
          break;
        }

      }
      if (File.Exists( strTempAssmbPath )) {
        //Load the assembly from the specified path.                    
        MyAssembly = Assembly.LoadFrom( strTempAssmbPath );
        //Return the loaded assembly.
        _sb.AppendLine( $"ADAPTER - Assembly loaded: {MyAssembly.FullName}" );
        return MyAssembly;
      }
      else {
        return null; // not found here
      }

    }

    // find out which one to use
    private FSVersion DetectVersion( )
    {
      // get running processes and find the one of either MSFS Sim Exe
      _fsWindowTitle = MSFS.MSFS2024running( );
      if (!string.IsNullOrEmpty( _fsWindowTitle )) return FSVersion.V2024;

      _fsWindowTitle = MSFS.MSFS2020running( );
      if (!string.IsNullOrEmpty( _fsWindowTitle )) return FSVersion.V2020;

      // none
      _fsWindowTitle = "<FS App not found>";
      return FSVersion.Unknown;
    }

    #region DISPOSE

    private bool disposedValue;

    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          if (_plug != null) { _plug.Dispose( ); }
          _plug = null;
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~SimConnectX()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose( )
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose( disposing: true );
      GC.SuppressFinalize( this );
    }

    #endregion

    #region WinMSG Handler

    /// <summary>
    /// Windows Message Handling Model when using the provided WM_USER_SIMCONNECT message ID
    /// 
    /// Provide the Message from the Main Message Loop to check and handle SimConnect events
    /// Propagate if not handled here (returned false)
    /// </summary>
    /// <param name="msg">The message ID</param>
    /// <returns>True if handled</returns>
    public bool WinMessageHandled( int msg )
    {
      // sanity
      if (_plug == null) return false;
      if (_fsVersion == FSVersion.Unknown) return false;

      // seems legit to handle it
      try {
        if (msg == WM_USER_SIMCONNECT) {
          ReceiveMessage( ); // handle if it seems connected
          return true;
        }
      }
      catch (Exception ex) {
        _ = ex; // DEBUG
      }

      return false;
    }

    #endregion


  }
}
