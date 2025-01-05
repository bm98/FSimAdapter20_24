using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using static MSFSAdapter20_24.SimConnect;

namespace MSFSAdapter20_24
{
  /// <summary>
  /// An Interface which an adapter needs to implement
  /// (mostly derived from the managed SimConnect DLL)
  /// 
  /// Based on managed SimConnect interface for 2024
  /// 
  /// Uses ENUMs in EnumsA.cs
  /// Uses STRUCTs in StructsA.cs
  /// 
  /// Delta for 2020 needs to be implemented in the Plug module
  /// 
  /// </summary>
  public interface ISimConnectA
  {
    #region Events

    event RecvEnumerateInputEventParamsEventHandler OnRecvEnumerateInputEventParams;
    event RecvSubscribeInputEventEventHandler OnRecvSubscribeInputEvent;
    event RecvGetInputEventEventHandler OnRecvGetInputEvent;
    event RecvEnumerateInputEventsEventHandler OnRecvEnumerateInputEvents;
    event RecvActionCallbackEventHandler OnRecvActionCallback;
    event RecvControllersListEventHandler OnRecvControllersList;
    event RecvJetwayDataEventHandler OnRecvJetwayData;
    event RecvFacilityMinimalListEventHandler OnRecvFacilityMinimalList;
    event RecvFacilityDataEndEventHandler OnRecvFacilityDataEnd;
    event RecvFacilityDataEventHandler OnRecvFacilityData;
    event RecvEventEx1EventHandler OnRecvEventEx1;
    event RecvEventRaceLapEventHandler OnRecvEventRaceLap;
    event RecvEventRaceEndEventHandler OnRecvEventRaceEnd;
    event RecvEventMultiplayerSessionEndedEventHandler OnRecvEventMultiplayerSessionEnded;
    event RecvEventMultiplayerClientStartedEventHandler OnRecvEventMultiplayerClientStarted;
    event RecvEventMultiplayerServerStartedEventHandler OnRecvEventMultiplayerServerStarted;
    event RecvWaypointListEventHandler OnRecvWaypointList;
    event RecvNdbListEventHandler OnRecvNdbList;
    event RecvVorListEventHandler OnRecvVorList;
    event RecvAirportListEventHandler OnRecvAirportList;
    event RecvEventWeatherModeEventHandler OnRecvEventWeatherMode;
    event RecvClientDataEventHandler OnRecvClientData;
    event RecvSystemStateEventHandler OnRecvSystemState;
    event RecvCustomActionEventHandler OnRecvCustomAction;
    event RecvReservedKeyEventHandler OnRecvReservedKey;
    event RecvAssignedObjectIdEventHandler OnRecvAssignedObjectId;
    event RecvCloudStateEventHandler OnRecvCloudState;
    event RecvWeatherObservationEventHandler OnRecvWeatherObservation;
    event RecvSimobjectDataBytypeEventHandler OnRecvSimobjectDataBytype;
    event RecvSimobjectDataEventHandler OnRecvSimobjectData;
    event RecvEventFrameEventHandler OnRecvEventFrame;
    event RecvEventFilenameEventHandler OnRecvEventFilename;
    event RecvEventObjectAddremoveEventHandler OnRecvEventObjectAddremove;
    event RecvEventEventHandler OnRecvEvent;
    event RecvQuitEventHandler OnRecvQuit;
    event RecvOpenEventHandler OnRecvOpen;
    event RecvExceptionEventHandler OnRecvException;
    event RecvNullEventHandler OnRecvNull;
    // added in 2024 SDK 1.1.2
    event RecvEnumerateSimobjectAndLiveryListEventHandler OnRecvEnumerateSimobjectAndLiveryList;

    #endregion

    #region Methods

    // replaces the cTor for the Plug assembly
    bool Open( string szName, IntPtr hWnd, uint UserEventWin32, WaitHandle hEventHandle, uint ConfigIndex );

    int TestMarshaling( );
    void RegisterDataDefineStruct<T>( Enum dwID );
    void RegisterFacilityDataDefineStruct<T>( SIMCONNECT_FACILITY_DATA_TYPE dwType );
    /*
    // Might not be possible to handle derived structs in a consistent manner...
    void RegisterStruct<RECV, T>( Enum dwID ) where RECV : SIMCONNECT_RECV;

    void ReceiveDispatch( SignalProcDelegate pfcnSignal );
    */
    void ReceiveMessage( );
    uint GetLastSentPacketID( );
    void MapClientEventToSimEvent( Enum EventID, string EventName );
    void TransmitClientEvent( uint ObjectID, Enum EventID, uint dwData, Enum GroupID, SIMCONNECT_EVENT_FLAG Flags );
    void SetSystemEventState( Enum EventID, SIMCONNECT_STATE dwState );
    void AddClientEventToNotificationGroup( Enum GroupID, Enum EventID, [MarshalAs( UnmanagedType.U1 )] bool bMaskable );
    void RemoveClientEvent( Enum GroupID, Enum EventID );
    void SetNotificationGroupPriority( Enum GroupID, uint uPriority );
    void ClearNotificationGroup( Enum GroupID );
    void RequestNotificationGroup( Enum GroupID, uint dwReserved, uint Flags );
    void AddToDataDefinition( Enum DefineID, string DatumName, string UnitsName, SIMCONNECT_DATATYPE DatumType, float fEpsilon, uint DatumID );
    void ClearDataDefinition( Enum DefineID );
    void RequestDataOnSimObject( Enum RequestID, Enum DefineID, uint ObjectID, SIMCONNECT_PERIOD Period, SIMCONNECT_DATA_REQUEST_FLAG Flags, uint origin, uint interval, uint limit );
    void RequestDataOnSimObjectType( Enum RequestID, Enum DefineID, uint dwRadiusMeters, SIMCONNECT_SIMOBJECT_TYPE type );
    void SetDataOnSimObject( Enum DefineID, uint ObjectID, SIMCONNECT_DATA_SET_FLAG Flags, object pDataSet );
    void MapInputEventToClientEvent( Enum GroupID, string szInputDefinition, Enum DownEventID, uint DownValue, Enum UpEventID, uint UpValue, [MarshalAs( UnmanagedType.U1 )] bool bMaskable );
    void SetInputGroupPriority( Enum GroupID, uint uPriority );
    void RemoveInputEvent( Enum GroupID, string szInputDefinition );
    void ClearInputGroup( Enum GroupID );
    void SetInputGroupState( Enum GroupID, uint dwState );
    void RequestReservedKey( Enum EventID, string szKeyChoice1, string szKeyChoice2, string szKeyChoice3 );
    void SubscribeToSystemEvent( Enum EventID, string SystemEventName );
    void UnsubscribeFromSystemEvent( Enum EventID );
    void WeatherRequestInterpolatedObservation( Enum RequestID, float lat, float lon, float alt );
    void WeatherRequestObservationAtStation( Enum RequestID, string szICAO );
    void WeatherRequestObservationAtNearestStation( Enum RequestID, float lat, float lon );
    void WeatherCreateStation( Enum RequestID, string szICAO, string szName, float lat, float lon, float alt );
    void WeatherRemoveStation( Enum RequestID, string szICAO );
    void WeatherSetObservation( uint Seconds, string szMETAR );
    void WeatherSetModeServer( uint dwPort, uint dwSeconds );
    void WeatherSetModeTheme( string szThemeName );
    void WeatherSetModeGlobal( );
    void WeatherSetModeCustom( );
    void WeatherSetDynamicUpdateRate( uint dwRate );
    void WeatherRequestCloudState( Enum RequestID, float minLat, float minLon, float minAlt, float maxLat, float maxLon, float maxAlt, uint dwFlags );
    void WeatherCreateThermal( Enum RequestID, float lat, float lon, float alt, float radius, float height, float coreRate, float coreTurbulence, float sinkRate, float sinkTurbulence, float coreSize, float coreTransitionSize, float sinkLayerSize, float sinkTransitionSize );
    void WeatherRemoveThermal( uint ObjectID );
    void AICreateParkedATCAircraft( string szContainerTitle, string szTailNumber, string szAirportID, Enum RequestID );
    void AICreateEnrouteATCAircraft( string szContainerTitle, string szTailNumber, int iFlightNumber, string szFlightPlanPath, double dFlightPlanPosition, [MarshalAs( UnmanagedType.U1 )] bool bTouchAndGo, Enum RequestID );
    void AICreateNonATCAircraft( string szContainerTitle, string szTailNumber, SIMCONNECT_DATA_INITPOSITION InitPos, Enum RequestID );
    void AICreateSimulatedObject( string szContainerTitle, SIMCONNECT_DATA_INITPOSITION InitPos, Enum RequestID );
    void AIReleaseControl( uint ObjectID, Enum RequestID );
    void AIRemoveObject( uint ObjectID, Enum RequestID );
    void AISetAircraftFlightPlan( uint ObjectID, string szFlightPlanPath, Enum RequestID );
    void ExecuteMissionAction( ValueType guidInstanceId );
    void CompleteCustomMissionAction( ValueType guidInstanceId );
    void CameraSetRelative6DOF( float fDeltaX, float fDeltaY, float fDeltaZ, float fPitchDeg, float fBankDeg, float fHeadingDeg );
    void MenuAddItem( string szMenuItem, Enum MenuEventID, uint dwData );
    void MenuDeleteItem( Enum MenuEventID );
    void MenuAddSubItem( Enum MenuEventID, string szMenuItem, Enum SubMenuEventID, uint dwData );
    void MenuDeleteSubItem( Enum MenuEventID, Enum SubMenuEventID );
    void RequestSystemState( Enum RequestID, string szState );
    void SetSystemState( string szState, uint dwInteger, float fFloat, string szString );
    void MapClientDataNameToID( string szClientDataName, Enum ClientDataID );
    void CreateClientData( Enum ClientDataID, uint dwSize, SIMCONNECT_CREATE_CLIENT_DATA_FLAG Flags );
    void AddToClientDataDefinition( Enum DefineID, uint dwOffset, uint dwSizeOrType, float fEpsilon, uint DatumID );
    void ClearClientDataDefinition( Enum DefineID );
    void RequestClientData( Enum ClientDataID, Enum RequestID, Enum DefineID, SIMCONNECT_CLIENT_DATA_PERIOD Period, SIMCONNECT_CLIENT_DATA_REQUEST_FLAG Flags, uint origin, uint interval, uint limit );
    void SetClientData( Enum ClientDataID, Enum DefineID, SIMCONNECT_CLIENT_DATA_SET_FLAG Flags, uint dwReserved, object pDataSet );
    void FlightLoad( string szFileName );
    void FlightSave( string szFileName, string szTitle, string szDescription, uint Flags );
    void FlightPlanLoad( string szFileName );
    void Text( SIMCONNECT_TEXT_TYPE type, float fTimeSeconds, Enum EventID, object pDataSet );
    void SubscribeToFacilities( SIMCONNECT_FACILITY_LIST_TYPE type, Enum RequestID );
    void UnsubscribeToFacilities( SIMCONNECT_FACILITY_LIST_TYPE type );
    void RequestFacilitiesList( SIMCONNECT_FACILITY_LIST_TYPE type, Enum RequestID );
    void TransmitClientEvent_EX1( uint ObjectID, Enum EventID, Enum GroupID, SIMCONNECT_EVENT_FLAG Flags, uint dwData0, uint dwData1, uint dwData2, uint dwData3, uint dwData4 );
    void AddToFacilityDefinition( Enum DefineID, string FieldName );
    void RequestFacilityData( Enum DefineID, Enum RequestID, string ICAO, string Region );
    void SubscribeToFacilities_EX1( SIMCONNECT_FACILITY_LIST_TYPE type, Enum newElemInRangeRequestID, Enum oldElemOutRangeRequestID );
    void UnsubscribeToFacilities_EX1( SIMCONNECT_FACILITY_LIST_TYPE type, [MarshalAs( UnmanagedType.U1 )] bool bUnsubscribeNewInRange, [MarshalAs( UnmanagedType.U1 )] bool bUnsubscribeOldOutRange );
    void RequestFacilitiesList_EX1( SIMCONNECT_FACILITY_LIST_TYPE type, Enum RequestID );
    void RequestFacilityData_EX1( Enum DefineID, Enum RequestID, string ICAO, string Region, sbyte Type );
    void EnumerateControllers( );
    void MapInputEventToClientEvent_EX1( Enum GroupID, string szInputDefinition, Enum DownEventID, uint DownValue, Enum UpEventID, uint UpValue, [MarshalAs( UnmanagedType.U1 )] bool bMaskable );
    void EnumerateInputEvents( Enum RequestID );
    void GetInputEvent( Enum RequestID, ulong Hash );
    void SetInputEvent( ulong Hash, object Value );
    void SubscribeInputEvent( ulong Hash );
    void UnsubscribeInputEvent( ulong Hash );
    void EnumerateInputEventParams( ulong Hash );
    void AddFacilityDataDefinitionFilter( Enum DefineID, string szFilterPath, object pFilterData );
    void ClearAllFacilityDataDefinitionFilters( Enum DefineID );
    void RequestResponseTimes( float[] fElapsedSeconds );
    void RequestJetwayData( string AirportIcao, ICollection<int> Indexes );

    // added in 2024 SDK 1.1.2
    void EnumerateSimObjectsAndLiveries( Enum RequestID, SIMCONNECT_SIMOBJECT_TYPE Type );


    void Dispose( );

    #endregion

  }
}
