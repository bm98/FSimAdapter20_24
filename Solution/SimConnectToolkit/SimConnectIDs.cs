
namespace SimConnectToolkit
{
  /// <summary>
  /// Handles SimConnect IDs at one place 
  /// 
  /// Those mostly need to be unique - so make all unique for tracking and debugging
  /// 
  /// ID Groups are:
  /// 
  ///   DEFINITION     1..   independent num space
  ///   
  ///   REQUEST    20'001..25'999
  ///   EventID    16'001..19'999
  ///   InputID    13'001..15'999
  ///   GroupID    10'001..12'999
  ///   
  ///   PrivateID  26'001..29'999 (internal ID range)
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
  /// REQUEST IDs for SimConnect interaction (unique ID as Enum required)
  /// 9_999 InputIDs supported (no check for overflow...)
  /// </summary>
  public enum REQUEST : uint
  {
    /// <summary>
    /// SimConnect REQUEST prototype
    /// </summary>
    Dummy = 0
  };

  /// <summary>
  /// EVENT ID for SimConnect interaction (unique ID as Enum required)
  /// 9_899 EventIDs supported (no check for overflow...)
  /// </summary>
  public enum EventID : uint // have to set this to uint else the managed interface throws an overflow error when converting SIMCONNECT_UNSUSED (-1) to Uint 
  {
    /// <summary>
    /// SimConnect EventID prototype
    ///  Reserved range 10_000..10_099 for predefined keys
    /// </summary>
    KeyUp = 10_001, // preset ID for a KeyUp event
    // other preset EventIDs here up to 10_099 only
    // distributed Event IDs now
    Dummy = 10_100,
  }

  /// <summary>
  /// EVENT ID for SimConnect interaction (unique ID as Enum required)
  /// 9_999 InputIDs supported (no check for overflow...)
  /// </summary>
  public enum InputID : uint
  {
    /// <summary>
    /// SimConnect InputID prototype
    /// </summary>
    Dummy = 20_000,
  }

  /// <summary>
  /// GROUP ID for SimConnect interaction (unique ID as Enum required) 
  /// 999 GroupIDs supported (no check for overflow...)
  /// </summary>
  public enum GroupID : uint
  {
    /// <summary>
    /// SimConnect GroupID prototype
    /// </summary>
    Dummy = 30_000,
  }

  // reserve 31_000+ for internal and special purposes

  /// <summary>
  /// Generic Private ID for SimConnect interaction (unique ID as Enum required)
  /// 10_000+ PrivateID supported (no check for overflow...)
  /// </summary>
  internal enum PrivateID : uint // have to set this to uint else the managed interface throws an overflow error when converting SIMCONNECT_UNSUSED (-1) to Uint 
  {
    /// <summary>
    /// SimConnect PrivateID prototype
    /// </summary>
    Dummy = 31_000,
  }

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

    private static PrivateID s_privateID = PrivateID.Dummy;

    #region Public IDs

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

    #endregion

    #region Private (internal) IDs

    /// <summary>
    /// Returns a new internal EventID 
    /// </summary>
    /// <returns>A new EventID</returns>
    internal static EventID GetPrivateEventID( )
    {
      return (EventID)(++s_privateID);
    }
    /// <summary>
    /// Returns a new internal GroupID 
    /// </summary>
    /// <returns>A new EventID</returns>
    internal static GroupID GetPrivateGroupID( )
    {
      return (GroupID)(++s_privateID);
    }
    /// <summary>
    /// Returns a new internal InputID 
    /// </summary>
    /// <returns>A new EventID</returns>
    internal static InputID GetPrivateInputID( )
    {
      return (InputID)(++s_privateID);
    }
    /// <summary>
    /// Returns a new internal DEFINITION 
    /// </summary>
    /// <returns>A new EventID</returns>
    internal static DEFINITION GetPrivateDEFINITION( )
    {
      return (DEFINITION)(++s_privateID);
    }
    /// <summary>
    /// Returns a new internal REQUEST 
    /// </summary>
    /// <returns>A new EventID</returns>
    internal static REQUEST GetPrivateREQUEST( )
    {
      return (REQUEST)(++s_privateID);
    }

    #endregion

  }
}
