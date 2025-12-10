
using System.Threading;

namespace SimConShared
{
  /// <summary>
  /// Handles SimConnect IDs at one place 
  ///  Get Methods are taking all IDs from the same static pool
  /// 
  /// Those mostly need to be unique - so make all unique for tracking and debugging
  /// 
  /// ID Groups are:
  /// 
  ///   DEFINITION      1..30_999   independent num space
  ///   
  /// The following IDs are expected to be unique by SimConnect
  /// 
  ///   GroupID    00'001..01'999
  ///   InputID    02'001..05'999
  ///   EventID    06'001..15'999
  ///   REQUEST    17'001..26'999
  ///   
  ///   User assigned : use range 30_000..30_999
  ///   
  ///   PrivateDEF 31'001..... special purposes (e.g. enum facility data)
  ///   PrivateID  31'001..... special purposes (e.g. enum facility data)
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
  /// 1..1_999 GroupIDs supported (no check for overflow...)
  /// Range 1..1_999
  /// </summary>
  public enum GroupID : uint
  {
    /// <summary>
    /// SimConnect GroupID prototype
    /// </summary>
    Dummy = 0,
  }

  /// <summary>
  /// EVENT ID for SimConnect interaction (unique ID as Enum required)
  /// 3_998 InputIDs supported (no check for overflow...)
  /// Range 2_001..5_999
  /// </summary>
  public enum InputID : uint
  {
    /// <summary>
    /// SimConnect InputID prototype
    /// </summary>
    Dummy = 2_000,
  }

  /// <summary>
  /// EVENT ID for SimConnect interaction (unique ID as Enum required)
  /// 9_899 EventIDs supported (no check for overflow...)
  /// Range 6_001..6_099 fixed key events
  /// Range 6_101..15_999
  /// </summary>
  public enum EventID : uint // have to set this to uint else the managed interface throws an overflow error when converting SIMCONNECT_UNSUSED (-1) to Uint 
  {
    /// <summary>
    /// SimConnect EventID prototype
    ///  Reserved range 6_000..6_099 for predefined keys
    /// </summary>
    KeyUp = 6_001, // preset ID for a KeyUp event
    // other preset EventIDs here up to 10_099 only
    // distributed Event IDs now
    Dummy = 6_100,
  }

  /// <summary>
  /// REQUEST IDs for SimConnect interaction (unique ID as Enum required)
  /// 9_999 RequestIDs supported (no check for overflow...)
  /// Range 17_001..26_999
  /// </summary>
  public enum REQUEST : uint
  {
    /// <summary>
    /// SimConnect REQUEST prototype
    /// </summary>
    Dummy = 17_000
  };

  /// <summary>
  /// USER IDs for SimConnect interaction (unique ID as Enum required)
  /// 9_999 RequestIDs supported (no check for overflow...)
  /// Range 30_001..30_999
  /// </summary>
  public enum UserID : uint
  {
    /// <summary>
    /// SimConnect UserID prototype
    /// </summary>
    Dummy = 30_000
  };


  // reserve 31_000+ for internal and special purposes

  /// <summary>
  /// Generic Private ID for SimConnect interaction (unique ID as Enum required)
  /// 100_000+ PrivateID supported (no check for overflow...)
  /// </summary>
  internal enum PrivateDEF : uint
  {
    /// <summary>
    /// SimConnect PrivateDEF prototype
    /// </summary>
    Dummy = 31_000,
  }
  /// <summary>
  /// Generic Private ID for SimConnect interaction (unique ID as Enum required)
  /// 100_000+ PrivateID supported (no check for overflow...)
  /// </summary>
  internal enum PrivateID : uint
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
    // need a long to use Interlocked.Increment
    private static long s_definitionID = (long)DEFINITION.Dummy;
    private static long s_groupID = (long)GroupID.Dummy;
    private static long s_eventID = (long)EventID.Dummy;
    private static long s_inputID = (long)InputID.Dummy;
    private static long s_requestID = (long)REQUEST.Dummy;
    private static long s_userID = (long)UserID.Dummy;

    private static long s_privateDEF = (long)PrivateDEF.Dummy;
    private static long s_privateID = (long)PrivateID.Dummy;

    #region Public IDs

    /// <summary>
    /// Returns a new DEFINITION ID
    /// </summary>
    /// <returns>A new DEFINITION ID</returns>
    public static DEFINITION GetDEFINITION( )
    {
      return (DEFINITION)Interlocked.Increment( ref s_definitionID );
    }
    /// <summary>
    /// Returns a new data REQUEST ID
    /// </summary>
    /// <returns>A new data REQUEST ID</returns>
    public static REQUEST GetREQUEST( )
    {
      return (REQUEST)Interlocked.Increment( ref s_requestID );
    }
    /// <summary>
    /// Returns a new GroupID 
    /// </summary>
    /// <returns>A new GroupID</returns>
    public static GroupID GetGroupID( )
    {
      return (GroupID)Interlocked.Increment( ref s_groupID );
    }
    /// <summary>
    /// Returns a new InputID 
    /// </summary>
    /// <returns>A new InputID</returns>
    public static InputID GetInputID( )
    {
      return (InputID)Interlocked.Increment( ref s_inputID );
    }
    /// <summary>
    /// Returns a new EventID 
    /// </summary>
    /// <returns>A new EventID</returns>
    public static EventID GetEventID( )
    {
      return (EventID)Interlocked.Increment( ref s_eventID );
    }

    /// <summary>
    /// Returns a new UserID 
    /// </summary>
    /// <returns>A new UserID</returns>
    public static UserID GetUserID( )
    {
      return (UserID)Interlocked.Increment( ref s_userID );
    }

    #endregion

    #region Special IDs (31_000 ...)

    // take all from the Private Pool and cast to common type

    /// <summary>
    /// Returns a new internal DEFINITION 
    /// </summary>
    /// <returns>A new EventID</returns>
    public static DEFINITION GetPrivateDEFINITION( )
    {
      return (DEFINITION)Interlocked.Increment( ref s_privateDEF );
    }

    /// <summary>
    /// Returns a new internal EventID 
    /// </summary>
    /// <returns>A new EventID</returns>
    public static EventID GetPrivateEventID( )
    {
      return (EventID)Interlocked.Increment( ref s_privateID );
    }
    /// <summary>
    /// Returns a new internal GroupID 
    /// </summary>
    /// <returns>A new EventID</returns>
    public static GroupID GetPrivateGroupID( )
    {
      return (GroupID)Interlocked.Increment( ref s_privateID );
    }
    /// <summary>
    /// Returns a new internal InputID 
    /// </summary>
    /// <returns>A new EventID</returns>
    public static InputID GetPrivateInputID( )
    {
      return (InputID)Interlocked.Increment( ref s_privateID );
    }
    /// <summary>
    /// Returns a new internal REQUEST 
    /// </summary>
    /// <returns>A new EventID</returns>
    public static REQUEST GetPrivateREQUEST( )
    {
      return (REQUEST)Interlocked.Increment( ref s_privateID );
    }

    #endregion

  }
}
