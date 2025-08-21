using System;
using System.Runtime.InteropServices;

namespace MSFSAdapter20_24
{
  ///<remarks>
  /// Contains only SimConnect Structs from MSFS2024 in the main namespace
  /// 
  /// copy from SDK V1.1.2 Microsoft.FlightSimulator.SimConnect.dll
  /// 
  ///</remarks>


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_CONTROLLER_ITEM
  {
    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 256 )]
    public string DeviceName;

    public uint DeviceId;

    public uint ProductId;

    public uint CompositeID;

    public readonly SIMCONNECT_VERSION_BASE_TYPE HardwareVersion = new SIMCONNECT_VERSION_BASE_TYPE( );
  }

  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_DATA_FACILITY_AIRPORT
  {
    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 9 )]
    public string Ident;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 3 )]
    public string Region;

    public double Latitude;

    public double Longitude;

    public double Altitude;
  }

  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_DATA_FACILITY_NDB : SIMCONNECT_DATA_FACILITY_WAYPOINT
  {
    public uint fFrequency;
  }

  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_DATA_FACILITY_VOR : SIMCONNECT_DATA_FACILITY_NDB
  {
    public uint Flags;

    public float fLocalizer;

    public double GlideLat;

    public double GlideLon;

    public double GlideAlt;

    public float fGlideSlopeAngle;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_DATA_FACILITY_WAYPOINT : SIMCONNECT_DATA_FACILITY_AIRPORT
  {
    public float fMagVar;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public struct SIMCONNECT_DATA_INITPOSITION
  {
    public double Latitude;

    public double Longitude;

    public double Altitude;

    public double Pitch;

    public double Bank;

    public double Heading;

    public uint OnGround;

    public uint Airspeed;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public struct SIMCONNECT_DATA_LATLONALT
  {
    public double Latitude;

    public double Longitude;

    public double Altitude;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public struct SIMCONNECT_DATA_MARKERSTATE
  {
    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 64 )]
    public string szMarkerName;

    public uint dwMarkerState;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public struct SIMCONNECT_DATA_PBH
  {
    public float Pitch;

    public float Bank;

    public float Heading;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public struct SIMCONNECT_DATA_RACE_RESULT
  {
    public uint dwNumberOfRacers;

    public Guid MissionGUID;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
    public string szPlayerName;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
    public string szSessionType;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
    public string szAircraft;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
    public string szPlayerRole;

    public double fTotalTime;

    public double fPenaltyTime;

    public uint dwIsDisqualified;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public struct SIMCONNECT_DATA_WAYPOINT
  {
    public double Latitude;

    public double Longitude;

    public double Altitude;

    public uint Flags;

    public double ktsSpeed;

    public double percentThrottle;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public struct SIMCONNECT_DATA_XYZ
  {
    public double x;

    public double y;

    public double z;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_FACILITY_MINIMAL
  {
    public SIMCONNECT_ICAO icao;

    public SIMCONNECT_DATA_LATLONALT lla;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public struct SIMCONNECT_ICAO
  {
    public sbyte Type;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 9 )]
    public string Ident;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 3 )]
    public string Region;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 5 )]
    public string Airport;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_INPUT_EVENT_DESCRIPTOR
  {
    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 64 )]
    public string Name;

    public ulong Hash;

    public SIMCONNECT_INPUT_EVENT_TYPE eType;
  }

  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_JETWAY_DATA
  {
    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 8 )]
    public string AirportIcao;

    public int ParkingIndex;

    public SIMCONNECT_DATA_LATLONALT Lla;

    public SIMCONNECT_DATA_PBH Pbh;

    public int Status;

    public int Door;

    public SIMCONNECT_DATA_XYZ ExitDoorRelativePos;

    public SIMCONNECT_DATA_XYZ MainHandlePos;

    public SIMCONNECT_DATA_XYZ SecondaryHandle;

    public SIMCONNECT_DATA_XYZ WheelGroundLock;

    public uint JetwayObjectId;

    public uint AttachedObjectId;
  }

  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV
  {
    public uint dwSize;

    public uint dwVersion;

    public uint dwID;
  }

  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_ACTION_CALLBACK : SIMCONNECT_RECV
  {
    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
    public string szActionID;

    public uint cbRequestId;
  }

  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_AIRPORT_LIST : SIMCONNECT_RECV_FACILITIES_LIST
  {
    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct )]
    [VariableSize( FixedType = typeof( SIMCONNECT_DATA_FACILITY_AIRPORT ), CountMember = "dwArraySize" )]
    public object[] rgData;
  }

  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_ASSIGNED_OBJECT_ID : SIMCONNECT_RECV
  {
    public uint dwRequestID;

    public uint dwObjectID;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1, Size = 1 )]
  public class SIMCONNECT_RECV_CLIENT_DATA : SIMCONNECT_RECV_SIMOBJECT_DATA
  {
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_CLOUD_STATE : SIMCONNECT_RECV
  {
    public uint dwRequestID;

    public uint dwArraySize;

    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct )]
    [VariableSize( FixedType = typeof( byte ), CountMember = "dwArraySize" )]
    public object[] rgbData;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_CONTROLLERS_LIST : SIMCONNECT_RECV_LIST_TEMPLATE
  {
    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct )]
    [VariableSize( FixedType = typeof( SIMCONNECT_CONTROLLER_ITEM ), CountMember = "dwArraySize" )]
    public object[] rgData;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_CUSTOM_ACTION : SIMCONNECT_RECV_EVENT
  {
    public Guid guidInstanceId;

    public uint dwWaitForCompletion;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 1 )]
    [VariableSize]
    public string szPayLoad;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_ENUMERATE_INPUT_EVENT_PARAMS : SIMCONNECT_RECV
  {
    public ulong Hash;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
    public string Value;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS : SIMCONNECT_RECV_LIST_TEMPLATE
  {
    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct )]
    [VariableSize( FixedType = typeof( SIMCONNECT_INPUT_EVENT_DESCRIPTOR ), CountMember = "dwArraySize" )]
    public object[] rgData;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_EVENT : SIMCONNECT_RECV
  {
    public static uint UNKNOWN_GROUP = uint.MaxValue;

    public uint uGroupID;

    public uint uEventID;

    public uint dwData;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_EVENT_EX1 : SIMCONNECT_RECV
  {
    public static uint UNKNOWN_GROUP = uint.MaxValue;

    public uint uGroupID;

    public uint uEventID;

    public uint dwData0;

    public uint dwData1;

    public uint dwData2;

    public uint dwData3;

    public uint dwData4;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_EVENT_FILENAME : SIMCONNECT_RECV_EVENT
  {
    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
    public string szFileName;

    public uint dwFlags;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_EVENT_FRAME : SIMCONNECT_RECV_EVENT
  {
    public float fFrameRate;

    public float fSimSpeed;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1, Size = 1 )]
  public class SIMCONNECT_RECV_EVENT_MULTIPLAYER_CLIENT_STARTED : SIMCONNECT_RECV_EVENT
  {
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1, Size = 1 )]
  public class SIMCONNECT_RECV_EVENT_MULTIPLAYER_SERVER_STARTED : SIMCONNECT_RECV_EVENT
  {
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1, Size = 1 )]
  public class SIMCONNECT_RECV_EVENT_MULTIPLAYER_SESSION_ENDED : SIMCONNECT_RECV_EVENT
  {
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_EVENT_OBJECT_ADDREMOVE : SIMCONNECT_RECV_EVENT
  {
    public SIMCONNECT_SIMOBJECT_TYPE eObjType;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_EVENT_RACE_END : SIMCONNECT_RECV_EVENT
  {
    public uint dwRacerNumber;

    public SIMCONNECT_DATA_RACE_RESULT RacerData;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_EVENT_RACE_LAP : SIMCONNECT_RECV_EVENT
  {
    public uint dwLapIndex;

    public SIMCONNECT_DATA_RACE_RESULT RacerData;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1, Size = 1 )]
  public class SIMCONNECT_RECV_EVENT_WEATHER_MODE : SIMCONNECT_RECV_EVENT
  {
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_EXCEPTION : SIMCONNECT_RECV
  {
    public uint dwException;

    public static uint UNKNOWN_SENDID = 0u;

    public uint dwSendID;

    public static uint UNKNOWN_INDEX = uint.MaxValue;

    public uint dwIndex;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_FACILITIES_LIST : SIMCONNECT_RECV
  {
    public uint dwRequestID;

    public uint dwArraySize;

    public uint dwEntryNumber;

    public uint dwOutOf;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_FACILITY_DATA : SIMCONNECT_RECV
  {
    public uint UserRequestId;

    public uint UniqueRequestId;

    public uint ParentUniqueRequestId;

    public uint Type;

    public uint IsListItem;

    public uint ItemIndex;

    public uint ListSize;

    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct )]
    [VariableSize( TypeMember = "Type", CountMember = "" )]
    public object[] Data;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_FACILITY_DATA_END : SIMCONNECT_RECV
  {
    public uint RequestId;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_FACILITY_MINIMAL_LIST : SIMCONNECT_RECV_LIST_TEMPLATE
  {
    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct )]
    [VariableSize( FixedType = typeof( SIMCONNECT_FACILITY_MINIMAL ), CountMember = "dwArraySize" )]
    public object[] rgData;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_GET_INPUT_EVENT : SIMCONNECT_RECV
  {
    public uint dwRequestID;

    public SIMCONNECT_INPUT_EVENT_TYPE eType;

    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct )]
    [VariableSize( TypeMember = "eType", CountMember = "" )]
    public object[] Value;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_JETWAY_DATA : SIMCONNECT_RECV_LIST_TEMPLATE
  {
    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct )]
    [VariableSize( FixedType = typeof( SIMCONNECT_JETWAY_DATA ), CountMember = "dwArraySize" )]
    public object[] rgData;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_LIST_TEMPLATE : SIMCONNECT_RECV
  {
    public uint dwRequestID;

    public uint dwArraySize;

    public uint dwEntryNumber;

    public uint dwOutOf;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_NDB_LIST : SIMCONNECT_RECV_FACILITIES_LIST
  {
    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct )]
    [VariableSize( FixedType = typeof( SIMCONNECT_DATA_FACILITY_NDB ), CountMember = "dwArraySize" )]
    public object[] rgData;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_OPEN : SIMCONNECT_RECV
  {
    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 256 )]
    public string szApplicationName;

    public uint dwApplicationVersionMajor;

    public uint dwApplicationVersionMinor;

    public uint dwApplicationBuildMajor;

    public uint dwApplicationBuildMinor;

    public uint dwSimConnectVersionMajor;

    public uint dwSimConnectVersionMinor;

    public uint dwSimConnectBuildMajor;

    public uint dwSimConnectBuildMinor;

    public uint dwReserved1;

    public uint dwReserved2;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1, Size = 1 )]
  public class SIMCONNECT_RECV_QUIT : SIMCONNECT_RECV
  {
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_RESERVED_KEY : SIMCONNECT_RECV
  {
    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 30 )]
    public string szChoiceReserved;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 50 )]
    public string szReservedKey;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_SIMOBJECT_DATA : SIMCONNECT_RECV
  {
    public uint dwRequestID;

    public uint dwObjectID;

    public uint dwDefineID;

    public uint dwFlags;

    public uint dwentrynumber;

    public uint dwoutof;

    public uint dwDefineCount;

    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct )]
    [VariableSize( TypeMember = "dwDefineID", CountMember = "" )]
    public object[] dwData;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1, Size = 1 )]
  public class SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE : SIMCONNECT_RECV_SIMOBJECT_DATA
  {
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_SUBSCRIBE_INPUT_EVENT : SIMCONNECT_RECV
  {
    public ulong Hash;

    public SIMCONNECT_INPUT_EVENT_TYPE eType;

    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct )]
    [VariableSize( TypeMember = "eType", CountMember = "" )]
    public object[] Value;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_SYSTEM_STATE : SIMCONNECT_RECV
  {
    public uint dwRequestID;

    public uint dwInteger;

    public float fFloat;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
    public string szString;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_VOR_LIST : SIMCONNECT_RECV_FACILITIES_LIST
  {
    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct )]
    [VariableSize( FixedType = typeof( SIMCONNECT_DATA_FACILITY_VOR ), CountMember = "dwArraySize" )]
    public object[] rgData;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_WAYPOINT_LIST : SIMCONNECT_RECV_FACILITIES_LIST
  {
    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct )]
    [VariableSize( FixedType = typeof( SIMCONNECT_DATA_FACILITY_WAYPOINT ), CountMember = "dwArraySize" )]
    public object[] rgData;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_WEATHER_OBSERVATION : SIMCONNECT_RECV
  {
    public uint dwRequestID;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 1 )]
    [VariableSize]
    public string szMetar;
  }


  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_VERSION_BASE_TYPE
  {
    public ushort Major;

    public ushort Minor;

    public ushort Revision;

    public ushort Build;
  }


  public class VariableSizeAttribute : Attribute
  {
    private string m_TypeMember = string.Empty;

    private string m_CountMember = string.Empty;

    private Type m_FixedType = null;

    public Type FixedType {
      get {
        return m_FixedType;
      }
      set {
        m_FixedType = value;
      }
    }

    public string CountMember {
      get {
        return m_CountMember;
      }
      set {
        m_CountMember = value;
      }
    }

    public string TypeMember {
      get {
        return m_TypeMember;
      }
      set {
        m_TypeMember = value;
      }
    }
  }


  // added in 2024 SDK 1.1.2
  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_ENUMERATE_SIMOBJECT_LIVERY
  {
    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 256 )]
    public string AircraftTitle;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 256 )]
    public string LiveryName;
  }
  // added in 2024 SDK 1.1.2
  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_ENUMERATE_SIMOBJECT_AND_LIVERY_LIST : SIMCONNECT_RECV_LIST_TEMPLATE
  {
    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct )]
    [VariableSize( FixedType = typeof( SIMCONNECT_ENUMERATE_SIMOBJECT_LIVERY ), CountMember = "dwArraySize" )]
    public object[] rgData;
  }

  // added in 2024 SDK 1.4.4
  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public class SIMCONNECT_RECV_FLOW_EVENT : SIMCONNECT_RECV
  {
    public SIMCONNECT_FLOW_EVENT FlowEvent;

    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 256 )]
    public string FltPath;
  }


}
