﻿
namespace MSFSAdapter20_24
{
  ///<remarks>
  ///Contains only SimConnect Enums in the main namespace
  ///
  /// copy from SDK V1.1.2 Microsoft.FlightSimulator.SimConnect.dll
  /// 
  ///</remarks>

  public enum SIMCONNECT_CLIENT_DATA_PERIOD
  {
    NEVER,
    ONCE,
    VISUAL_FRAME,
    ON_SET,
    SECOND
  }

  public enum SIMCONNECT_CLIENT_DATA_REQUEST_FLAG
  {
    DEFAULT,
    CHANGED,
    TAGGED
  }

  public enum SIMCONNECT_CLIENT_DATA_SET_FLAG
  {
    DEFAULT,
    TAGGED
  }

  public enum SIMCONNECT_CREATE_CLIENT_DATA_FLAG
  {
    DEFAULT,
    READ_ONLY
  }
  public enum SIMCONNECT_DATA_REQUEST_FLAG
  {
    DEFAULT,
    CHANGED,
    TAGGED
  }

  public enum SIMCONNECT_DATA_SET_FLAG
  {
    DEFAULT,
    TAGGED
  }

  public enum SIMCONNECT_DATATYPE
  {
    INVALID,
    INT32,
    INT64,
    FLOAT32,
    FLOAT64,
    STRING8,
    STRING32,
    STRING64,
    STRING128,
    STRING256,
    STRING260,
    STRINGV,
    INITPOSITION,
    MARKERSTATE,
    WAYPOINT,
    LATLONALT,
    XYZ,
    MAX
  }

  public enum SIMCONNECT_EVENT_FLAG
  {
    DEFAULT = 0,
    FAST_REPEAT_TIMER = 1,
    SLOW_REPEAT_TIMER = 2,
    GROUPID_IS_PRIORITY = 0x10
  }


  public enum SIMCONNECT_EXCEPTION
  {
    NONE,
    ERROR,
    SIZE_MISMATCH,
    UNRECOGNIZED_ID,
    UNOPENED,
    VERSION_MISMATCH,
    TOO_MANY_GROUPS,
    NAME_UNRECOGNIZED,
    TOO_MANY_EVENT_NAMES,
    EVENT_ID_DUPLICATE,
    TOO_MANY_MAPS,
    TOO_MANY_OBJECTS,
    TOO_MANY_REQUESTS,
    WEATHER_INVALID_PORT,
    WEATHER_INVALID_METAR,
    WEATHER_UNABLE_TO_GET_OBSERVATION,
    WEATHER_UNABLE_TO_CREATE_STATION,
    WEATHER_UNABLE_TO_REMOVE_STATION,
    INVALID_DATA_TYPE,
    INVALID_DATA_SIZE,
    DATA_ERROR,
    INVALID_ARRAY,
    CREATE_OBJECT_FAILED,
    LOAD_FLIGHTPLAN_FAILED,
    OPERATION_INVALID_FOR_OBJECT_TYPE,
    ILLEGAL_OPERATION,
    ALREADY_SUBSCRIBED,
    INVALID_ENUM,
    DEFINITION_ERROR,
    DUPLICATE_ID,
    DATUM_ID,
    OUT_OF_BOUNDS,
    ALREADY_CREATED,
    OBJECT_OUTSIDE_REALITY_BUBBLE,
    OBJECT_CONTAINER,
    OBJECT_AI,
    OBJECT_ATC,
    OBJECT_SCHEDULE,
    JETWAY_DATA,
    ACTION_NOT_FOUND,
    NOT_AN_ACTION,
    INCORRECT_ACTION_PARAMS,
    GET_INPUT_EVENT_FAILED,
    SET_INPUT_EVENT_FAILED,
    INTERNAL  // added in 2024 SDK 1.1.2
  }

  public enum SIMCONNECT_FACILITY_DATA_TYPE
  {
    AIRPORT,
    RUNWAY,
    START,
    FREQUENCY,
    HELIPAD,
    APPROACH,
    APPROACH_TRANSITION,
    APPROACH_LEG,
    FINAL_APPROACH_LEG,
    MISSED_APPROACH_LEG,
    DEPARTURE,
    ARRIVAL,
    RUNWAY_TRANSITION,
    ENROUTE_TRANSITION,
    TAXI_POINT,
    TAXI_PARKING,
    TAXI_PATH,
    TAXI_NAME,
    JETWAY,
    VOR,
    NDB,
    WAYPOINT,
    ROUTE,
    PAVEMENT,
    APPROACH_LIGHTS,
    VASI,
    VDGS,  // added in 2024 SDK 1.1.2
    HOLDING_PATTERN  // added in 2024 SDK 1.1.2
  }

  public enum SIMCONNECT_FACILITY_LIST_TYPE
  {
    AIRPORT,
    WAYPOINT,
    NDB,
    VOR,
    COUNT
  }

  public enum SIMCONNECT_INPUT_EVENT_TYPE : uint
  {
    DOUBLE,
    STRING
  }

  public enum SIMCONNECT_MISSION_END
  {
    FAILED,
    CRASHED,
    SUCCEEDED
  }

  public enum SIMCONNECT_PERIOD
  {
    NEVER,
    ONCE,
    VISUAL_FRAME,
    SIM_FRAME,
    SECOND
  }

  public enum SIMCONNECT_RECV_ID
  {
    NULL,
    EXCEPTION,
    OPEN,
    QUIT,
    EVENT,
    EVENT_OBJECT_ADDREMOVE,
    EVENT_FILENAME,
    EVENT_FRAME,
    SIMOBJECT_DATA,
    SIMOBJECT_DATA_BYTYPE,
    WEATHER_OBSERVATION,
    CLOUD_STATE,
    ASSIGNED_OBJECT_ID,
    RESERVED_KEY,
    CUSTOM_ACTION,
    SYSTEM_STATE,
    CLIENT_DATA,
    EVENT_WEATHER_MODE,
    AIRPORT_LIST,
    VOR_LIST,
    NDB_LIST,
    WAYPOINT_LIST,
    EVENT_MULTIPLAYER_SERVER_STARTED,
    EVENT_MULTIPLAYER_CLIENT_STARTED,
    EVENT_MULTIPLAYER_SESSION_ENDED,
    EVENT_RACE_END,
    EVENT_RACE_LAP,
    EVENT_EX1,
    FACILITY_DATA,
    FACILITY_DATA_END,
    FACILITY_MINIMAL_LIST,
    JETWAY_DATA,
    CONTROLLERS_LIST,
    ACTION_CALLBACK,
    ENUMERATE_INPUT_EVENTS,
    GET_INPUT_EVENT,
    SUBSCRIBE_INPUT_EVENT,
    ENUMERATE_INPUT_EVENT_PARAMS,
    ENUMERATE_SIMOBJECT_AND_LIVERY_LIST // added in 2024 SDK 1.1.2
  }

  public enum SIMCONNECT_SIMOBJECT_TYPE
  {
    USER,
    ALL,
    AIRCRAFT,
    HELICOPTER,
    BOAT,
    GROUND,
    HOT_AIR_BALLOON,  // added in 2024 SDK 1.1.2
    ANIMAL // added in 2024 SDK 1.1.2
  }

  public enum SIMCONNECT_SOUND_SYSTEM_EVENT_DATA
  {
    MASTER = 1
  }

  public enum SIMCONNECT_STATE
  {
    OFF,
    ON
  }
  public enum SIMCONNECT_TEXT_RESULT
  {
    MENU_SELECT_1 = 0,
    MENU_SELECT_2 = 1,
    MENU_SELECT_3 = 2,
    MENU_SELECT_4 = 3,
    MENU_SELECT_5 = 4,
    MENU_SELECT_6 = 5,
    MENU_SELECT_7 = 6,
    MENU_SELECT_8 = 7,
    MENU_SELECT_9 = 8,
    MENU_SELECT_10 = 9,
    DISPLAYED = 65536,
    QUEUED = 65537,
    REMOVED = 65538,
    REPLACED = 65539,
    TIMEOUT = 65540
  }
  public enum SIMCONNECT_TEXT_TYPE
  {
    SCROLL_BLACK = 0,
    SCROLL_WHITE = 1,
    SCROLL_RED = 2,
    SCROLL_GREEN = 3,
    SCROLL_BLUE = 4,
    SCROLL_YELLOW = 5,
    SCROLL_MAGENTA = 6,
    SCROLL_CYAN = 7,
    PRINT_BLACK = 256,
    PRINT_WHITE = 257,
    PRINT_RED = 258,
    PRINT_GREEN = 259,
    PRINT_BLUE = 260,
    PRINT_YELLOW = 261,
    PRINT_MAGENTA = 262,
    PRINT_CYAN = 263,
    MENU = 512
  }
  public enum SIMCONNECT_VIEW_SYSTEM_EVENT_DATA
  {
    COCKPIT_2D = 1,
    COCKPIT_VIRTUAL = 2,
    ORTHOGONAL = 4
  }
  public enum SIMCONNECT_VOR_FLAGS
  {
    SIMCONNECT_RECV_ID_VOR_LIST_HAS_NAV_SIGNAL = 1,
    SIMCONNECT_RECV_ID_VOR_LIST_HAS_LOCALIZER = 2,
    SIMCONNECT_RECV_ID_VOR_LIST_HAS_GLIDE_SLOPE = 4,
    SIMCONNECT_RECV_ID_VOR_LIST_HAS_DME = 8
  }
  public enum SIMCONNECT_WAYPOINT_FLAGS
  {
    NONE = 0,
    SPEED_REQUESTED = 4,
    THROTTLE_REQUESTED = 8,
    COMPUTE_VERTICAL_SPEED = 0x10,
    ALTITUDE_IS_AGL = 0x20,
    ON_GROUND = 0x100000,
    REVERSE = 0x200000,
    WRAP_TO_FIRST = 0x400000,
    ALWAYS_BACKUP = 0x800000,
    KEEP_LAST_HEADING = 0x1000000,
    YIELD_TO_USER = 0x2000000,
    CAN_REVERSE = 0x4000000
  }
  public enum SIMCONNECT_WEATHER_MODE
  {
    THEME,
    RWW,
    CUSTOM,
    GLOBAL
  }

}