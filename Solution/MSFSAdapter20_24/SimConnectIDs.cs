
namespace MSFSAdapter20_24
{
  /// <summary>
  /// Handles SimConnect IDs at one place 
  /// 
  /// Those mostly need to be unique - so make all unique for tracking and debugging
  /// 
  /// ID Groups are:
  /// 
  ///   DEFINITION     1..  9'999
  ///   GroupID    10'001..12'999
  ///   InputID    13'001..15'999
  ///   EventID    16'001..19'999
  ///   REQUEST    20'001..
  ///   
  ///   User assigned and special purpose: use range 30_000..
  ///   
  /// </summary>


  /// <summary>
  /// DEFINITION ID for SimConnect interaction (unique ID as Enum required)
  /// </summary>
  public enum DEFINITION : uint // have to set this to uint else the managed interface throws an overflow error when converting SIMCONNECT_UNSUSED (-1) to Uint 
  {
    /// <summary>
    /// SimConnect DEFINITION prototype
    /// 9_999 Definitions supported (no check for overflow...)
    /// </summary>
    Dummy = 0
  };

  /// <summary>
  /// GROUP ID for SimConnect interaction (unique ID as Enum required) 
  /// 2_999 GroupIDs supported (no check for overflow...)
  /// </summary>
  public enum GroupID : uint // have to set this to uint else the managed interface throws an overflow error when converting SIMCONNECT_UNSUSED (-1) to Uint 
  {
    /// <summary>
    /// SimConnect GroupID prototype
    /// </summary>
    Dummy = 10_000,
  }

  /// <summary>
  /// EVENT ID for SimConnect interaction (unique ID as Enum required)
  /// 2_999 InputIDs supported (no check for overflow...)
  /// </summary>
  public enum InputID : uint // have to set this to uint else the managed interface throws an overflow error when converting SIMCONNECT_UNSUSED (-1) to Uint 
  {
    /// <summary>
    /// SimConnect InputID prototype
    /// </summary>
    Dummy = 13_000,
  }

  /// <summary>
  /// EVENT ID for SimConnect interaction (unique ID as Enum required)
  /// 3_999 EventIDs supported (no check for overflow...)
  /// </summary>
  public enum EventID : uint // have to set this to uint else the managed interface throws an overflow error when converting SIMCONNECT_UNSUSED (-1) to Uint 
  {
    /// <summary>
    /// SimConnect EventID prototype
    /// </summary>
    Dummy = 16_000,
  }

  /// <summary>
  /// REQUEST ID for SimConnect interaction (unique ID as Enum required)
  /// 2_000+ RequestIDs supported (no check for overflow...)
  /// </summary>
  public enum REQUEST : uint // have to set this to uint else the managed interface throws an overflow error when converting SIMCONNECT_UNSUSED (-1) to Uint 
  {
    /// <summary>
    /// SimConnect REQUEST prototype
    /// </summary>
    Dummy = 20_000
  };

  /// <summary>
  /// Create new SimConnect IDs
  /// </summary>
  public static class SimConnectIDs
  {
    private static DEFINITION s_definitionID = DEFINITION.Dummy;
    private static GroupID s_groupID = GroupID.Dummy;
    private static EventID s_eventID = EventID.Dummy;
    private static InputID s_inputID = InputID.Dummy;
    private static REQUEST s_requestID = REQUEST.Dummy;

    /// <summary>
    /// Returns a new DEFINITION ID
    /// </summary>
    /// <returns>A new DEFINITION ID</returns>
    public static DEFINITION GetDEFINITION( )
    {
      return ++s_definitionID;
    }
    /// <summary>
    /// Returns a new data REQUEST ID
    /// </summary>
    /// <returns>A new data REQUEST ID</returns>
    public static REQUEST GetREQUEST( )
    {
      return ++s_requestID;
    }
    /// <summary>
    /// Returns a new GroupID 
    /// </summary>
    /// <returns>A new GroupID</returns>
    public static GroupID GetGroupID( )
    {
      return ++s_groupID;
    }
    /// <summary>
    /// Returns a new InputID 
    /// </summary>
    /// <returns>A new InputID</returns>
    public static InputID GetInputID( )
    {
      return ++s_inputID;
    }
    /// <summary>
    /// Returns a new EventID 
    /// </summary>
    /// <returns>A new EventID</returns>
    public static EventID GetEventID( )
    {
      return ++s_eventID;
    }
  }
}
