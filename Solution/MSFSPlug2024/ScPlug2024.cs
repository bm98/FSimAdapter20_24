using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using FS = Microsoft.FlightSimulator.SimConnect;
using CX = MSFSAdapter20_24;

using MSFSAdapter20_24;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MSFSPlug2024
{

  /// <summary>
  /// Adapter Plugin for SimConnect 2024
  /// </summary>
  public class ScPlug2024 : ISimConnectA
  {
    // SimConnect handle
    private FS.SimConnect _simConnect;

    #region Events

    public event SimConnect.RecvEnumerateInputEventParamsEventHandler OnRecvEnumerateInputEventParams;
    public event SimConnect.RecvSubscribeInputEventEventHandler OnRecvSubscribeInputEvent;
    public event SimConnect.RecvGetInputEventEventHandler OnRecvGetInputEvent;
    public event SimConnect.RecvEnumerateInputEventsEventHandler OnRecvEnumerateInputEvents;
    public event SimConnect.RecvActionCallbackEventHandler OnRecvActionCallback;
    public event SimConnect.RecvControllersListEventHandler OnRecvControllersList;
    public event SimConnect.RecvJetwayDataEventHandler OnRecvJetwayData;
    public event SimConnect.RecvFacilityMinimalListEventHandler OnRecvFacilityMinimalList;
    public event SimConnect.RecvFacilityDataEndEventHandler OnRecvFacilityDataEnd;
    public event SimConnect.RecvFacilityDataEventHandler OnRecvFacilityData;
    public event SimConnect.RecvEventEx1EventHandler OnRecvEventEx1;
    public event SimConnect.RecvEventRaceLapEventHandler OnRecvEventRaceLap;
    public event SimConnect.RecvEventRaceEndEventHandler OnRecvEventRaceEnd;
    public event SimConnect.RecvEventMultiplayerSessionEndedEventHandler OnRecvEventMultiplayerSessionEnded;
    public event SimConnect.RecvEventMultiplayerClientStartedEventHandler OnRecvEventMultiplayerClientStarted;
    public event SimConnect.RecvEventMultiplayerServerStartedEventHandler OnRecvEventMultiplayerServerStarted;
    public event SimConnect.RecvWaypointListEventHandler OnRecvWaypointList;
    public event SimConnect.RecvNdbListEventHandler OnRecvNdbList;
    public event SimConnect.RecvVorListEventHandler OnRecvVorList;
    public event SimConnect.RecvAirportListEventHandler OnRecvAirportList;
    public event SimConnect.RecvEventWeatherModeEventHandler OnRecvEventWeatherMode;
    public event SimConnect.RecvClientDataEventHandler OnRecvClientData;
    public event SimConnect.RecvSystemStateEventHandler OnRecvSystemState;
    public event SimConnect.RecvCustomActionEventHandler OnRecvCustomAction;
    public event SimConnect.RecvReservedKeyEventHandler OnRecvReservedKey;
    public event SimConnect.RecvAssignedObjectIdEventHandler OnRecvAssignedObjectId;
    public event SimConnect.RecvCloudStateEventHandler OnRecvCloudState;
    public event SimConnect.RecvWeatherObservationEventHandler OnRecvWeatherObservation;
    public event SimConnect.RecvSimobjectDataBytypeEventHandler OnRecvSimobjectDataBytype;
    public event SimConnect.RecvSimobjectDataEventHandler OnRecvSimobjectData;
    public event SimConnect.RecvEventFrameEventHandler OnRecvEventFrame;
    public event SimConnect.RecvEventFilenameEventHandler OnRecvEventFilename;
    public event SimConnect.RecvEventObjectAddremoveEventHandler OnRecvEventObjectAddremove;
    public event SimConnect.RecvEventEventHandler OnRecvEvent;
    public event SimConnect.RecvQuitEventHandler OnRecvQuit;
    public event SimConnect.RecvOpenEventHandler OnRecvOpen;
    public event SimConnect.RecvExceptionEventHandler OnRecvException;
    public event SimConnect.RecvNullEventHandler OnRecvNull;
    // added in 2024 SDK 1.1.2
    public event SimConnect.RecvEnumerateSimobjectAndLiveryListEventHandler OnRecvEnumerateSimobjectAndLiveryList;

    #endregion

    public bool Open( string szName, IntPtr hWnd, uint UserEventWin32, WaitHandle hEventHandle, uint ConfigIndex )
    {
      try {
        // if no sim is running this bails out with a COM exception
        _simConnect = new FS.SimConnect( szName, hWnd, UserEventWin32, hEventHandle, ConfigIndex );
        if (_simConnect != null) AttachHandlers( );
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch (COMException _) {
#pragma warning restore CS0168 // Variable is declared but never used
        return false;
      }

      if (_simConnect != null) {
        Debug.WriteLine( $"Plug2024 Created SimConnect " );
        return true;
      }

      Debug.WriteLine( $"Plug2024 FAILED to create SimConnect " );
      return false;
    }

    public void Dispose( )
    {
      _simConnect?.Dispose( );
      _simConnect = null;
    }


    // attach all Plug handlers when possible
    private void AttachHandlers( )
    {
      // sanity
      if (_simConnect == null) throw new InvalidOperationException( "_simConnect is null, cannot continue" ); // PROGRAM ERROR

      // the structs and classes are exactly the same in the FS2024 case, however Type conversion for those items does not work in c# -
      // as the Assing version is about 10x faster than generic CastCopy we copy all items in this section - lot of writing...
      // despite CastCopy does not work with FS LIST items, as they don't expose the Size properly (well...)
      _simConnect.OnRecvActionCallback += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_ACTION_CALLBACK data )
        => {
          var dataX = new SIMCONNECT_RECV_ACTION_CALLBACK( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            cbRequestId = data.cbRequestId,
            szActionID = data.szActionID
          };
          OnRecvActionCallback?.Invoke( null, dataX );
        };

      _simConnect.OnRecvAirportList += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_AIRPORT_LIST data )
        => {
          var dataX = new SIMCONNECT_RECV_AIRPORT_LIST( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwArraySize = data.dwArraySize,
            dwEntryNumber = data.dwEntryNumber,
            dwOutOf = data.dwOutOf,
            dwRequestID = data.dwRequestID,
            rgData = data.rgData,
          };
          OnRecvAirportList?.Invoke( null, dataX );
        };

      _simConnect.OnRecvAssignedObjectId += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_ASSIGNED_OBJECT_ID data )
        => {
          var dataX = new SIMCONNECT_RECV_ASSIGNED_OBJECT_ID( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            dwObjectID = data.dwObjectID,
          };
          OnRecvAssignedObjectId?.Invoke( null, dataX );
        };

      _simConnect.OnRecvClientData += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_CLIENT_DATA data )
        => {
          var dataX = new SIMCONNECT_RECV_CLIENT_DATA( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            dwObjectID = data.dwObjectID,
            dwDefineID = data.dwDefineID,
            dwFlags = data.dwFlags,
            dwentrynumber = data.dwentrynumber,
            dwoutof = data.dwoutof,
            dwDefineCount = data.dwDefineCount, // The number of 8-byte elements in the dwData array.
            dwData = data.dwData, //A data array containing information on a specified object in 8-byte (double word) elements. The length of the array is dwDefineCount.
          };
          OnRecvClientData?.Invoke( null, dataX );
        };

      _simConnect.OnRecvCloudState += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_CLOUD_STATE data )
        => {
          var dataX = new SIMCONNECT_RECV_CLOUD_STATE( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            dwArraySize = data.dwArraySize,
            rgbData = data.rgbData,
          };
          OnRecvCloudState?.Invoke( null, dataX );
        };

      _simConnect.OnRecvControllersList += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_CONTROLLERS_LIST data )
        => {
          var dataX = new SIMCONNECT_RECV_CONTROLLERS_LIST( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            dwArraySize = data.dwArraySize,
            dwEntryNumber = data.dwEntryNumber,
            dwOutOf = data.dwOutOf,
            rgData = data.rgData,
          };
          OnRecvControllersList?.Invoke( null, dataX );
        };

      _simConnect.OnRecvCustomAction += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_CUSTOM_ACTION data )
        => {
          var dataX = new SIMCONNECT_RECV_CUSTOM_ACTION( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            uEventID = data.uEventID,
            uGroupID = data.uGroupID,
            guidInstanceId = data.guidInstanceId,
            dwWaitForCompletion = data.dwWaitForCompletion,
            szPayLoad = data.szPayLoad,
            dwData = data.dwData,
          };
          OnRecvCustomAction?.Invoke( null, dataX );
        };

      _simConnect.OnRecvEnumerateInputEventParams += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_ENUMERATE_INPUT_EVENT_PARAMS data )
        => {
          var dataX = new SIMCONNECT_RECV_ENUMERATE_INPUT_EVENT_PARAMS( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            Hash = data.Hash,
            Value = data.Value,
          };
          OnRecvEnumerateInputEventParams?.Invoke( null, dataX );
        };

      _simConnect.OnRecvEnumerateInputEvents += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS data )
        => {
          var dataX = new SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            dwEntryNumber = data.dwEntryNumber,
            dwOutOf = data.dwOutOf,
            dwArraySize = data.dwArraySize,
            rgData = data.rgData,
          };
          OnRecvEnumerateInputEvents?.Invoke( null, dataX );
        };

      _simConnect.OnRecvEvent += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT data )
        => {
          var dataX = new SIMCONNECT_RECV_EVENT( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            uEventID = data.uEventID,
            uGroupID = data.uGroupID,
            dwData = data.dwData,
          };
          OnRecvEvent?.Invoke( null, dataX );
        };

      _simConnect.OnRecvEventEx1 += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_EX1 data )
        => {
          var dataX = new SIMCONNECT_RECV_EVENT_EX1( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            uEventID = data.uEventID,
            uGroupID = data.uGroupID,
            dwData0 = data.dwData0,
            dwData1 = data.dwData1,
            dwData2 = data.dwData2,
            dwData3 = data.dwData3,
            dwData4 = data.dwData4,
          };
          OnRecvEventEx1?.Invoke( null, dataX );
        };

      _simConnect.OnRecvEventFilename += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_FILENAME data )
        => {
          var dataX = new SIMCONNECT_RECV_EVENT_FILENAME( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            uEventID = data.uEventID,
            uGroupID = data.uGroupID,
            dwFlags = data.dwFlags,
            dwData = data.dwData,
            szFileName = data.szFileName,
          };
          OnRecvEventFilename?.Invoke( null, dataX );
        };

      _simConnect.OnRecvEventFrame += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_FRAME data )
        => {
          var dataX = new SIMCONNECT_RECV_EVENT_FRAME( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            uEventID = data.uEventID,
            uGroupID = data.uGroupID,
            fFrameRate = data.fFrameRate,
            fSimSpeed = data.fSimSpeed,
          };
          OnRecvEventFrame?.Invoke( null, dataX );
        };

      _simConnect.OnRecvEventMultiplayerClientStarted += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_MULTIPLAYER_CLIENT_STARTED data )
        => {
          var dataX = new SIMCONNECT_RECV_EVENT_MULTIPLAYER_CLIENT_STARTED( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            uEventID = data.uEventID,
            uGroupID = data.uGroupID,
            dwData = data.dwData,
          };
          OnRecvEventMultiplayerClientStarted?.Invoke( null, dataX );
        };

      _simConnect.OnRecvEventMultiplayerServerStarted += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_MULTIPLAYER_SERVER_STARTED data )
        => {
          var dataX = new SIMCONNECT_RECV_EVENT_MULTIPLAYER_SERVER_STARTED( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            uEventID = data.uEventID,
            uGroupID = data.uGroupID,
            dwData = data.dwData,
          };
          OnRecvEventMultiplayerServerStarted?.Invoke( null, dataX );
        };

      _simConnect.OnRecvEventMultiplayerSessionEnded += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_MULTIPLAYER_SESSION_ENDED data )
        => {
          var dataX = new SIMCONNECT_RECV_EVENT_MULTIPLAYER_SESSION_ENDED( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            uEventID = data.uEventID,
            uGroupID = data.uGroupID,
            dwData = data.dwData,
          };
          OnRecvEventMultiplayerSessionEnded?.Invoke( null, dataX );
        };

      _simConnect.OnRecvEventObjectAddremove += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_OBJECT_ADDREMOVE data )
        => {
          var dataX = new SIMCONNECT_RECV_EVENT_OBJECT_ADDREMOVE( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            uEventID = data.uEventID,
            uGroupID = data.uGroupID,
            dwData = data.dwData,
            eObjType = (SIMCONNECT_SIMOBJECT_TYPE)data.eObjType,
          };
          OnRecvEventObjectAddremove?.Invoke( null, dataX );
        };

      _simConnect.OnRecvEventRaceEnd += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_RACE_END data )
        => {
          var dataX = new SIMCONNECT_RECV_EVENT_RACE_END( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            uEventID = data.uEventID,
            uGroupID = data.uGroupID,
            dwData = data.dwData,
            dwRacerNumber = data.dwRacerNumber,
            RacerData = new SIMCONNECT_DATA_RACE_RESULT( ) {
              dwIsDisqualified = data.RacerData.dwIsDisqualified,
              dwNumberOfRacers = data.RacerData.dwNumberOfRacers,
              fPenaltyTime = data.RacerData.fPenaltyTime,
              fTotalTime = data.RacerData.fTotalTime,
              MissionGUID = data.RacerData.MissionGUID,
              szAircraft = data.RacerData.szAircraft,
              szPlayerName = data.RacerData.szPlayerName,
              szPlayerRole = data.RacerData.szPlayerRole,
              szSessionType = data.RacerData.szSessionType,
            }
          };
          OnRecvEventRaceEnd?.Invoke( null, dataX );
        };

      _simConnect.OnRecvEventRaceLap += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_RACE_LAP data )
        => {
          var dataX = new SIMCONNECT_RECV_EVENT_RACE_LAP( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            uEventID = data.uEventID,
            uGroupID = data.uGroupID,
            dwData = data.dwData,
            dwLapIndex = data.dwLapIndex,
            RacerData = new SIMCONNECT_DATA_RACE_RESULT( ) {
              dwIsDisqualified = data.RacerData.dwIsDisqualified,
              dwNumberOfRacers = data.RacerData.dwNumberOfRacers,
              fPenaltyTime = data.RacerData.fPenaltyTime,
              fTotalTime = data.RacerData.fTotalTime,
              MissionGUID = data.RacerData.MissionGUID,
              szAircraft = data.RacerData.szAircraft,
              szPlayerName = data.RacerData.szPlayerName,
              szPlayerRole = data.RacerData.szPlayerRole,
              szSessionType = data.RacerData.szSessionType,
            }
          };
          OnRecvEventRaceLap?.Invoke( null, dataX );
        };

      _simConnect.OnRecvEventWeatherMode += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_EVENT_WEATHER_MODE data )
        => {
          var dataX = new SIMCONNECT_RECV_EVENT_WEATHER_MODE( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            uEventID = data.uEventID,
            uGroupID = data.uGroupID,
            dwData = data.dwData,
          };
          OnRecvEventWeatherMode?.Invoke( null, dataX );
        };

      _simConnect.OnRecvException += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_EXCEPTION data )
        => {
          var dataX = new SIMCONNECT_RECV_EXCEPTION( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwException = data.dwException,
            dwIndex = data.dwIndex,
            dwSendID = data.dwSendID,
          };
          OnRecvException?.Invoke( null, dataX );
        };

      _simConnect.OnRecvFacilityData += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_FACILITY_DATA data )
        => {
          var dataX = new SIMCONNECT_RECV_FACILITY_DATA( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            ParentUniqueRequestId = data.ParentUniqueRequestId,
            UniqueRequestId = data.UniqueRequestId,
            UserRequestId = data.UserRequestId,
            IsListItem = data.IsListItem,
            ItemIndex = data.ItemIndex,
            Type = data.Type,
            ListSize = data.ListSize,
            Data = data.Data,
          };
          OnRecvFacilityData?.Invoke( null, dataX );
        };

      _simConnect.OnRecvFacilityDataEnd += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_FACILITY_DATA_END data )
        => {
          var dataX = new SIMCONNECT_RECV_FACILITY_DATA_END( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            RequestId = data.RequestId,
          };
          OnRecvFacilityDataEnd?.Invoke( null, dataX );
        };

      _simConnect.OnRecvFacilityMinimalList += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_FACILITY_MINIMAL_LIST data )
        => {
          var dataX = new SIMCONNECT_RECV_FACILITY_MINIMAL_LIST( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            dwEntryNumber = data.dwEntryNumber,
            dwOutOf = data.dwOutOf,
            dwArraySize = data.dwArraySize,
            rgData = data.rgData,
          };
          OnRecvFacilityMinimalList?.Invoke( null, dataX );
        };

      _simConnect.OnRecvGetInputEvent += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_GET_INPUT_EVENT data )
        => {
          var dataX = new SIMCONNECT_RECV_GET_INPUT_EVENT( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            eType = (SIMCONNECT_INPUT_EVENT_TYPE)data.eType,
            Value = data.Value,
          };
          OnRecvGetInputEvent?.Invoke( null, dataX );
        };

      _simConnect.OnRecvJetwayData += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_JETWAY_DATA data )
        => {
          var dataX = new SIMCONNECT_RECV_JETWAY_DATA( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            dwEntryNumber = data.dwEntryNumber,
            dwOutOf = data.dwOutOf,
            dwArraySize = data.dwArraySize,
            rgData = data.rgData,
          };
          OnRecvJetwayData?.Invoke( null, dataX );
        };

      _simConnect.OnRecvNdbList += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_NDB_LIST data )
        => {
          var dataX = new SIMCONNECT_RECV_NDB_LIST( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            dwEntryNumber = data.dwEntryNumber,
            dwOutOf = data.dwOutOf,
            dwArraySize = data.dwArraySize,
            rgData = data.rgData,
          };
          OnRecvNdbList?.Invoke( null, dataX );
        };

      _simConnect.OnRecvNull += ( FS.SimConnect sender, FS.SIMCONNECT_RECV data )
        => {
          var dataX = new SIMCONNECT_RECV( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
          };
          OnRecvNull?.Invoke( null, dataX );
        };

      _simConnect.OnRecvOpen += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_OPEN data )
        => {
          var dataX = new SIMCONNECT_RECV_OPEN( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwApplicationVersionMajor = data.dwApplicationVersionMajor,
            dwApplicationVersionMinor = data.dwApplicationVersionMinor,
            dwApplicationBuildMajor = data.dwApplicationBuildMajor,
            dwApplicationBuildMinor = data.dwApplicationBuildMinor,
            dwSimConnectVersionMajor = data.dwSimConnectVersionMajor,
            dwSimConnectVersionMinor = data.dwSimConnectVersionMinor,
            dwSimConnectBuildMajor = data.dwSimConnectBuildMajor,
            dwSimConnectBuildMinor = data.dwSimConnectBuildMinor,
            szApplicationName = data.szApplicationName,
            dwReserved1 = data.dwReserved1,
            dwReserved2 = data.dwReserved2,
          };
          OnRecvOpen?.Invoke( null, dataX );
        };

      _simConnect.OnRecvQuit += ( FS.SimConnect sender, FS.SIMCONNECT_RECV data )
        => {
          var dataX = new SIMCONNECT_RECV( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
          };
          OnRecvQuit?.Invoke( null, dataX );
        };

      _simConnect.OnRecvReservedKey += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_RESERVED_KEY data )
        => {
          var dataX = new SIMCONNECT_RECV_RESERVED_KEY( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            szChoiceReserved = data.szChoiceReserved,
            szReservedKey = data.szReservedKey,
          };
          OnRecvReservedKey?.Invoke( null, dataX );
        };

      _simConnect.OnRecvSimobjectData += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_SIMOBJECT_DATA data )
        => {
          var dataX = new SIMCONNECT_RECV_SIMOBJECT_DATA( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            dwObjectID = data.dwObjectID,
            dwDefineID = data.dwDefineID,
            dwFlags = data.dwFlags,
            dwentrynumber = data.dwentrynumber,
            dwoutof = data.dwoutof,
            dwDefineCount = data.dwDefineCount, // The number of 8-byte elements in the dwData array.
            dwData = data.dwData, //A data array containing information on a specified object in 8-byte (double word) elements. The length of the array is dwDefineCount.
          };
          OnRecvSimobjectData?.Invoke( null, dataX );
        };

      _simConnect.OnRecvSimobjectDataBytype += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data )
        => {
          var dataX = new SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            dwObjectID = data.dwObjectID,
            dwDefineID = data.dwDefineID,
            dwFlags = data.dwFlags,
            dwentrynumber = data.dwentrynumber,
            dwoutof = data.dwoutof,
            dwDefineCount = data.dwDefineCount, // The number of 8-byte elements in the dwData array.
            dwData = data.dwData, //A data array containing information on a specified object in 8-byte (double word) elements. The length of the array is dwDefineCount.
          };
          OnRecvSimobjectDataBytype?.Invoke( null, dataX );
        };

      _simConnect.OnRecvSubscribeInputEvent += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_SUBSCRIBE_INPUT_EVENT data )
        => {
          var dataX = new SIMCONNECT_RECV_SUBSCRIBE_INPUT_EVENT( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            eType = (SIMCONNECT_INPUT_EVENT_TYPE)data.eType,
            Hash = data.Hash,
            Value = data.Value,
          };
          OnRecvSubscribeInputEvent?.Invoke( null, dataX );
        };

      _simConnect.OnRecvSystemState += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_SYSTEM_STATE data )
        => {
          var dataX = new SIMCONNECT_RECV_SYSTEM_STATE( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            dwInteger = data.dwInteger,
            fFloat = data.fFloat,
            szString = data.szString,
          };
          OnRecvSystemState?.Invoke( null, dataX );
        };

      _simConnect.OnRecvVorList += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_VOR_LIST data )
        => {
          var dataX = new SIMCONNECT_RECV_VOR_LIST( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            dwEntryNumber = data.dwEntryNumber,
            dwOutOf = data.dwOutOf,
            dwArraySize = data.dwArraySize,
            rgData = data.rgData,
          };
          OnRecvVorList?.Invoke( null, dataX );
        };

      _simConnect.OnRecvWaypointList += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_WAYPOINT_LIST data )
        => {
          var dataX = new SIMCONNECT_RECV_WAYPOINT_LIST( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            dwEntryNumber = data.dwEntryNumber,
            dwOutOf = data.dwOutOf,
            dwArraySize = data.dwArraySize,
            rgData = data.rgData,
          };
          OnRecvWaypointList?.Invoke( null, dataX );
        };

      _simConnect.OnRecvWeatherObservation += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_WEATHER_OBSERVATION data )
        => {
          var dataX = new SIMCONNECT_RECV_WEATHER_OBSERVATION( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            szMetar = data.szMetar,
          };
          OnRecvWeatherObservation?.Invoke( null, dataX );
        };

      // added in 2024 SDK 1.1.2
      _simConnect.OnRecvEnumerateSimobjectAndLiveryList += ( FS.SimConnect sender, FS.SIMCONNECT_RECV_ENUMERATE_SIMOBJECT_AND_LIVERY_LIST data )
        => {
          var dataX = new SIMCONNECT_RECV_ENUMERATE_SIMOBJECT_AND_LIVERY_LIST( ) {
            dwSize = data.dwSize,
            dwID = data.dwID,
            dwVersion = data.dwVersion,
            dwRequestID = data.dwRequestID,
            dwEntryNumber = data.dwEntryNumber,
            dwOutOf = data.dwOutOf,
            dwArraySize = data.dwArraySize,
            rgData = data.rgData,
          };
          OnRecvEnumerateSimobjectAndLiveryList?.Invoke( null, dataX );
        };
    }



    #region Call forwarder

    public int TestMarshaling( )
    {
      int? result = _simConnect?.TestMarshaling( );
      if (result.HasValue) return result.Value;
      throw new Exception( "TestMarshaling failed" ); // subst if it does not work as expected
    }
    public uint GetLastSentPacketID( )
    {
      uint? result = _simConnect?.GetLastSentPacketID( );
      if (result.HasValue) return result.Value;
      throw new Exception( "GetLastSentPacketID failed" );
    }

    public void RegisterDataDefineStruct<T>( Enum dwID )
      => _simConnect?.RegisterDataDefineStruct<T>( dwID );
    public void RegisterFacilityDataDefineStruct<T>( CX.SIMCONNECT_FACILITY_DATA_TYPE dwType )
      => _simConnect?.RegisterFacilityDataDefineStruct<T>( (FS.SIMCONNECT_FACILITY_DATA_TYPE)dwType );

    /*
    // Might not be possible to handle derived structs in a consistent manner...
    public void RegisterStruct<RECV, T>( Enum dwID ) where RECV : MSFSAdapter20_24.SIMCONNECT_RECV
      => _simConnect?.RegisterStruct<RECV, T>( dwID ); // would need to cast to be derived from FS.SIMCONNECT_RECV which not likely work

    public void ReceiveDispatch( SimConnectX.SignalProcDelegate pfcnSignal )
      => _simConnect?.ReceiveDispatch( CastTo<FS.SignalProcDelegate>( pfcnSignal ) );

     */

    public void ReceiveMessage( )
      => _simConnect?.ReceiveMessage( );
    public void MapClientEventToSimEvent( Enum EventID, string EventName )
      => _simConnect?.MapClientEventToSimEvent( EventID, EventName );
    public void TransmitClientEvent( uint ObjectID, Enum EventID, uint dwData, Enum GroupID, CX.SIMCONNECT_EVENT_FLAG Flags )
      => _simConnect?.TransmitClientEvent( ObjectID, EventID, dwData, GroupID, (FS.SIMCONNECT_EVENT_FLAG)Flags );
    public void SetSystemEventState( Enum EventID, CX.SIMCONNECT_STATE dwState )
      => _simConnect?.SetSystemEventState( EventID, (FS.SIMCONNECT_STATE)dwState );
    public void AddClientEventToNotificationGroup( Enum GroupID, Enum EventID, [MarshalAs( UnmanagedType.U1 )] bool bMaskable )
      => _simConnect?.AddClientEventToNotificationGroup( GroupID, EventID, bMaskable );
    public void RemoveClientEvent( Enum GroupID, Enum EventID )
      => _simConnect?.RemoveClientEvent( GroupID, EventID );
    public void SetNotificationGroupPriority( Enum GroupID, uint uPriority )
      => _simConnect?.SetNotificationGroupPriority( GroupID, uPriority );
    public void ClearNotificationGroup( Enum GroupID )
      => _simConnect?.ClearNotificationGroup( GroupID );
    public void RequestNotificationGroup( Enum GroupID, uint dwReserved, uint Flags )
      => _simConnect?.RequestNotificationGroup( GroupID, dwReserved, Flags );
    public void AddToDataDefinition( Enum DefineID, string DatumName, string UnitsName, CX.SIMCONNECT_DATATYPE DatumType, float fEpsilon, uint DatumID )
      => _simConnect?.AddToDataDefinition( DefineID, DatumName, UnitsName, (FS.SIMCONNECT_DATATYPE)DatumType, fEpsilon, DatumID );
    public void ClearDataDefinition( Enum DefineID ) => _simConnect?.ClearDataDefinition( DefineID );
    public void RequestDataOnSimObject( Enum RequestID, Enum DefineID, uint ObjectID, CX.SIMCONNECT_PERIOD Period, CX.SIMCONNECT_DATA_REQUEST_FLAG Flags, uint origin, uint interval, uint limit )
      => _simConnect?.RequestDataOnSimObject( RequestID, DefineID, ObjectID, (FS.SIMCONNECT_PERIOD)Period, (FS.SIMCONNECT_DATA_REQUEST_FLAG)Flags, origin, interval, limit );
    public void RequestDataOnSimObjectType( Enum RequestID, Enum DefineID, uint dwRadiusMeters, CX.SIMCONNECT_SIMOBJECT_TYPE type )
      => _simConnect?.RequestDataOnSimObjectType( RequestID, DefineID, dwRadiusMeters, (FS.SIMCONNECT_SIMOBJECT_TYPE)type );
    public void SetDataOnSimObject( Enum DefineID, uint ObjectID, CX.SIMCONNECT_DATA_SET_FLAG Flags, object pDataSet )
      => _simConnect?.SetDataOnSimObject( DefineID, ObjectID, (FS.SIMCONNECT_DATA_SET_FLAG)Flags, pDataSet );
    public void MapInputEventToClientEvent( Enum GroupID, string szInputDefinition, Enum DownEventID, uint DownValue, Enum UpEventID, uint UpValue, [MarshalAs( UnmanagedType.U1 )] bool bMaskable )
      => _simConnect?.MapInputEventToClientEvent( GroupID, szInputDefinition, DownEventID, DownValue, UpEventID, UpValue, bMaskable );
    public void SetInputGroupPriority( Enum GroupID, uint uPriority )
      => _simConnect?.SetInputGroupPriority( GroupID, uPriority );
    public void RemoveInputEvent( Enum GroupID, string szInputDefinition )
      => _simConnect?.RemoveInputEvent( GroupID, szInputDefinition );
    public void ClearInputGroup( Enum GroupID )
      => _simConnect?.ClearInputGroup( GroupID );
    public void SetInputGroupState( Enum GroupID, uint dwState )
      => _simConnect?.SetInputGroupState( GroupID, dwState );
    public void RequestReservedKey( Enum EventID, string szKeyChoice1, string szKeyChoice2, string szKeyChoice3 )
      => _simConnect?.RequestReservedKey( EventID, szKeyChoice1, szKeyChoice2, szKeyChoice3 );
    public void SubscribeToSystemEvent( Enum EventID, string SystemEventName )
      => _simConnect?.SubscribeToSystemEvent( EventID, SystemEventName );
    public void UnsubscribeFromSystemEvent( Enum EventID )
      => _simConnect?.UnsubscribeFromSystemEvent( EventID );
    public void WeatherRequestInterpolatedObservation( Enum RequestID, float lat, float lon, float alt )
      => _simConnect?.WeatherRequestInterpolatedObservation( RequestID, lat, lon, alt );
    public void WeatherRequestObservationAtStation( Enum RequestID, string szICAO )
      => _simConnect?.WeatherRequestObservationAtStation( RequestID, szICAO );
    public void WeatherRequestObservationAtNearestStation( Enum RequestID, float lat, float lon )
      => _simConnect?.WeatherRequestObservationAtNearestStation( RequestID, lat, lon );
    public void WeatherCreateStation( Enum RequestID, string szICAO, string szName, float lat, float lon, float alt )
      => _simConnect?.WeatherCreateStation( RequestID, szICAO, szName, lat, lon, alt );
    public void WeatherRemoveStation( Enum RequestID, string szICAO )
      => _simConnect?.WeatherRemoveStation( RequestID, szICAO );
    public void WeatherSetObservation( uint Seconds, string szMETAR )
      => _simConnect?.WeatherSetObservation( Seconds, szMETAR );
    public void WeatherSetModeServer( uint dwPort, uint dwSeconds )
      => _simConnect?.WeatherSetModeServer( dwPort, dwSeconds );
    public void WeatherSetModeTheme( string szThemeName )
      => _simConnect?.WeatherSetModeTheme( szThemeName );
    public void WeatherSetModeGlobal( )
      => _simConnect?.WeatherSetModeGlobal( );
    public void WeatherSetModeCustom( )
      => _simConnect?.WeatherSetModeCustom( );
    public void WeatherSetDynamicUpdateRate( uint dwRate )
      => _simConnect?.WeatherSetDynamicUpdateRate( dwRate );
    public void WeatherRequestCloudState( Enum RequestID, float minLat, float minLon, float minAlt, float maxLat, float maxLon, float maxAlt, uint dwFlags )
      => _simConnect?.WeatherRequestCloudState( RequestID, minLat, minLon, minAlt, maxLat, maxLon, maxAlt, dwFlags );
    public void WeatherCreateThermal( Enum RequestID, float lat, float lon, float alt, float radius, float height, float coreRate, float coreTurbulence, float sinkRate, float sinkTurbulence, float coreSize, float coreTransitionSize, float sinkLayerSize, float sinkTransitionSize )
      => _simConnect?.WeatherCreateThermal( RequestID, lat, lon, alt, radius, height, coreRate, coreTurbulence, sinkRate, sinkTurbulence, coreSize, coreTransitionSize, sinkLayerSize, sinkTransitionSize );
    public void WeatherRemoveThermal( uint ObjectID )
      => _simConnect?.WeatherRemoveThermal( ObjectID );
    public void AICreateParkedATCAircraft( string szContainerTitle, string szTailNumber, string szAirportID, Enum RequestID )
      => _simConnect?.AICreateParkedATCAircraft( szContainerTitle, szTailNumber, szAirportID, RequestID );
    public void AICreateEnrouteATCAircraft( string szContainerTitle, string szTailNumber, int iFlightNumber, string szFlightPlanPath, double dFlightPlanPosition, [MarshalAs( UnmanagedType.U1 )] bool bTouchAndGo, Enum RequestID )
      => _simConnect?.AICreateEnrouteATCAircraft( szContainerTitle, szTailNumber, iFlightNumber, szFlightPlanPath, dFlightPlanPosition, bTouchAndGo, RequestID );
    public void AICreateNonATCAircraft( string szContainerTitle, string szTailNumber, CX.SIMCONNECT_DATA_INITPOSITION InitPos, Enum RequestID )
    {
      var InitPosFS = new FS.SIMCONNECT_DATA_INITPOSITION( ) {
        Altitude = InitPos.Altitude,
        Airspeed = InitPos.Airspeed,
        Bank = InitPos.Bank,
        Heading = InitPos.Heading,
        Latitude = InitPos.Latitude,
        Longitude = InitPos.Longitude,
        OnGround = InitPos.OnGround,
        Pitch = InitPos.Pitch
      };
      _simConnect?.AICreateNonATCAircraft( szContainerTitle, szTailNumber, InitPosFS, RequestID );
    }

    public void AICreateSimulatedObject( string szContainerTitle, CX.SIMCONNECT_DATA_INITPOSITION InitPos, Enum RequestID )
    {
      var InitPosFS = new FS.SIMCONNECT_DATA_INITPOSITION( ) {
        Altitude = InitPos.Altitude,
        Airspeed = InitPos.Airspeed,
        Bank = InitPos.Bank,
        Heading = InitPos.Heading,
        Latitude = InitPos.Latitude,
        Longitude = InitPos.Longitude,
        OnGround = InitPos.OnGround,
        Pitch = InitPos.Pitch
      };
      _simConnect?.AICreateSimulatedObject( szContainerTitle, InitPosFS, RequestID );
    }

    public void AIReleaseControl( uint ObjectID, Enum RequestID )
      => _simConnect?.AIReleaseControl( ObjectID, RequestID );
    public void AIRemoveObject( uint ObjectID, Enum RequestID )
      => _simConnect?.AIRemoveObject( ObjectID, RequestID );
    public void AISetAircraftFlightPlan( uint ObjectID, string szFlightPlanPath, Enum RequestID )
      => _simConnect?.AISetAircraftFlightPlan( ObjectID, szFlightPlanPath, RequestID );
    public void ExecuteMissionAction( ValueType guidInstanceId )
      => _simConnect?.ExecuteMissionAction( guidInstanceId );
    public void CompleteCustomMissionAction( ValueType guidInstanceId )
      => _simConnect?.CompleteCustomMissionAction( guidInstanceId );
    public void CameraSetRelative6DOF( float fDeltaX, float fDeltaY, float fDeltaZ, float fPitchDeg, float fBankDeg, float fHeadingDeg )
      => _simConnect?.CameraSetRelative6DOF( fDeltaX, fDeltaY, fDeltaZ, fPitchDeg, fBankDeg, fHeadingDeg );
    public void MenuAddItem( string szMenuItem, Enum MenuEventID, uint dwData )
      => _simConnect?.MenuAddItem( szMenuItem, MenuEventID, dwData );
    public void MenuDeleteItem( Enum MenuEventID )
      => _simConnect?.MenuDeleteItem( MenuEventID );
    public void MenuAddSubItem( Enum MenuEventID, string szMenuItem, Enum SubMenuEventID, uint dwData )
      => _simConnect?.MenuAddSubItem( MenuEventID, szMenuItem, SubMenuEventID, dwData );
    public void MenuDeleteSubItem( Enum MenuEventID, Enum SubMenuEventID )
      => _simConnect?.MenuDeleteSubItem( MenuEventID, SubMenuEventID );
    public void RequestSystemState( Enum RequestID, string szState )
      => _simConnect?.RequestSystemState( RequestID, szState );
    public void SetSystemState( string szState, uint dwInteger, float fFloat, string szString )
      => _simConnect?.SetSystemState( szState, dwInteger, fFloat, szString );
    public void MapClientDataNameToID( string szClientDataName, Enum ClientDataID )
      => _simConnect?.MapClientDataNameToID( szClientDataName, ClientDataID );
    public void CreateClientData( Enum ClientDataID, uint dwSize, CX.SIMCONNECT_CREATE_CLIENT_DATA_FLAG Flags )
      => _simConnect?.CreateClientData( ClientDataID, dwSize, (FS.SIMCONNECT_CREATE_CLIENT_DATA_FLAG)Flags );
    public void AddToClientDataDefinition( Enum DefineID, uint dwOffset, uint dwSizeOrType, float fEpsilon, uint DatumID )
      => _simConnect?.AddToClientDataDefinition( DefineID, dwOffset, dwSizeOrType, fEpsilon, DatumID );
    public void ClearClientDataDefinition( Enum DefineID )
      => _simConnect?.ClearClientDataDefinition( DefineID );
    public void RequestClientData( Enum ClientDataID, Enum RequestID, Enum DefineID, CX.SIMCONNECT_CLIENT_DATA_PERIOD Period, MSFSAdapter20_24.SIMCONNECT_CLIENT_DATA_REQUEST_FLAG Flags, uint origin, uint interval, uint limit )
      => _simConnect?.RequestClientData( ClientDataID, RequestID, DefineID, (FS.SIMCONNECT_CLIENT_DATA_PERIOD)Period, (FS.SIMCONNECT_CLIENT_DATA_REQUEST_FLAG)Flags, origin, interval, limit );
    public void SetClientData( Enum ClientDataID, Enum DefineID, CX.SIMCONNECT_CLIENT_DATA_SET_FLAG Flags, uint dwReserved, object pDataSet )
      => _simConnect?.SetClientData( ClientDataID, DefineID, (FS.SIMCONNECT_CLIENT_DATA_SET_FLAG)Flags, dwReserved, pDataSet );
    public void FlightLoad( string szFileName )
      => _simConnect?.FlightLoad( szFileName );
    public void FlightSave( string szFileName, string szTitle, string szDescription, uint Flags )
      => _simConnect?.FlightSave( szFileName, szTitle, szDescription, Flags );
    public void FlightPlanLoad( string szFileName )
      => _simConnect?.FlightPlanLoad( szFileName );
    public void Text( CX.SIMCONNECT_TEXT_TYPE type, float fTimeSeconds, Enum EventID, object pDataSet )
      => _simConnect?.Text( (FS.SIMCONNECT_TEXT_TYPE)type, fTimeSeconds, EventID, pDataSet );
    public void SubscribeToFacilities( CX.SIMCONNECT_FACILITY_LIST_TYPE type, Enum RequestID )
      => _simConnect?.SubscribeToFacilities( (FS.SIMCONNECT_FACILITY_LIST_TYPE)type, RequestID );
    public void UnsubscribeToFacilities( CX.SIMCONNECT_FACILITY_LIST_TYPE type )
      => _simConnect?.UnsubscribeToFacilities( (FS.SIMCONNECT_FACILITY_LIST_TYPE)type );
    public void RequestFacilitiesList( CX.SIMCONNECT_FACILITY_LIST_TYPE type, Enum RequestID )
      => _simConnect?.RequestFacilitiesList( (FS.SIMCONNECT_FACILITY_LIST_TYPE)type, RequestID );
    public void TransmitClientEvent_EX1( uint ObjectID, Enum EventID, Enum GroupID, MSFSAdapter20_24.SIMCONNECT_EVENT_FLAG Flags, uint dwData0, uint dwData1, uint dwData2, uint dwData3, uint dwData4 )
      => _simConnect?.TransmitClientEvent_EX1( ObjectID, EventID, GroupID, (FS.SIMCONNECT_EVENT_FLAG)Flags, dwData0, dwData1, dwData2, dwData3, dwData4 );
    public void AddToFacilityDefinition( Enum DefineID, string FieldName )
      => _simConnect?.AddToFacilityDefinition( DefineID, FieldName );
    public void RequestFacilityData( Enum DefineID, Enum RequestID, string ICAO, string Region )
      => _simConnect?.RequestFacilityData( DefineID, RequestID, ICAO, Region );
    public void SubscribeToFacilities_EX1( CX.SIMCONNECT_FACILITY_LIST_TYPE type, Enum newElemInRangeRequestID, Enum oldElemOutRangeRequestID )
      => _simConnect?.SubscribeToFacilities_EX1( (FS.SIMCONNECT_FACILITY_LIST_TYPE)type, newElemInRangeRequestID, oldElemOutRangeRequestID );
    public void UnsubscribeToFacilities_EX1( CX.SIMCONNECT_FACILITY_LIST_TYPE type, [MarshalAs( UnmanagedType.U1 )] bool bUnsubscribeNewInRange, [MarshalAs( UnmanagedType.U1 )] bool bUnsubscribeOldOutRange )
      => _simConnect?.UnsubscribeToFacilities_EX1( (FS.SIMCONNECT_FACILITY_LIST_TYPE)type, bUnsubscribeNewInRange, bUnsubscribeOldOutRange );
    public void RequestFacilitiesList_EX1( CX.SIMCONNECT_FACILITY_LIST_TYPE type, Enum RequestID )
      => _simConnect?.RequestFacilitiesList_EX1( (FS.SIMCONNECT_FACILITY_LIST_TYPE)type, RequestID );
    public void RequestFacilityData_EX1( Enum DefineID, Enum RequestID, string ICAO, string Region, sbyte Type )
      => _simConnect?.RequestFacilityData_EX1( DefineID, RequestID, ICAO, Region, Type );
    public void EnumerateControllers( )
      => _simConnect?.EnumerateControllers( );
    public void MapInputEventToClientEvent_EX1( Enum GroupID, string szInputDefinition, Enum DownEventID, uint DownValue, Enum UpEventID, uint UpValue, [MarshalAs( UnmanagedType.U1 )] bool bMaskable )
      => _simConnect?.MapInputEventToClientEvent_EX1( GroupID, szInputDefinition, DownEventID, DownValue, UpEventID, UpValue, bMaskable );
    public void EnumerateInputEvents( Enum RequestID )
      => _simConnect?.EnumerateInputEvents( RequestID );
    public void GetInputEvent( Enum RequestID, ulong Hash )
      => _simConnect?.GetInputEvent( RequestID, Hash );
    public void SetInputEvent( ulong Hash, object Value )
      => _simConnect?.SetInputEvent( Hash, Value );
    public void SubscribeInputEvent( ulong Hash )
      => _simConnect?.SubscribeInputEvent( Hash );
    public void UnsubscribeInputEvent( ulong Hash )
      => _simConnect?.UnsubscribeInputEvent( Hash );
    public void EnumerateInputEventParams( ulong Hash )
      => _simConnect?.EnumerateInputEventParams( Hash );
    public void AddFacilityDataDefinitionFilter( Enum DefineID, string szFilterPath, object pFilterData )
      => _simConnect?.AddFacilityDataDefinitionFilter( DefineID, szFilterPath, pFilterData );
    public void ClearAllFacilityDataDefinitionFilters( Enum DefineID )
      => _simConnect?.ClearAllFacilityDataDefinitionFilters( DefineID );
    public void RequestResponseTimes( float[] fElapsedSeconds )
      => _simConnect?.RequestResponseTimes( fElapsedSeconds );
    public void RequestJetwayData( string AirportIcao, ICollection<int> Indexes )
      => _simConnect?.RequestJetwayData( AirportIcao, Indexes );

    // added in 2024 SDK 1.1.2
    public void EnumerateSimObjectsAndLiveries( Enum RequestID, CX.SIMCONNECT_SIMOBJECT_TYPE Type )
      => _simConnect?.EnumerateSimObjectsAndLiveries( RequestID, (FS.SIMCONNECT_SIMOBJECT_TYPE)Type );

    #endregion // Call Forwarder



  }
}
