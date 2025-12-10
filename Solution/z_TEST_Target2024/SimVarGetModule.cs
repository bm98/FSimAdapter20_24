using System;
using System.Runtime.InteropServices;
using System.Text;

using FS = Microsoft.FlightSimulator.SimConnect;

using SimConShared;

namespace z_TEST_Target2024
{
  /// <summary>
  /// Test env to get some SimVars
  ///   access vars with GetStruct.ITEM
  /// </summary>
  internal class SimVarGetModule
  {
    public readonly DEFINITION _structDefID = SimConnectIDs.GetDEFINITION( );
    public  readonly REQUEST _requestID = SimConnectIDs.GetREQUEST( );

    /// <summary>
    /// Live Data
    /// </summary>
    public SimVarGetStruct GetStruct = new SimVarGetStruct( );

    /// <summary>
    /// Register the Vars in 'SimVarGetStruct' with SimConnect
    /// </summary>
    /// <param name="simConnect">simConnect obj</param>
    public void RegisterWithSimConnect( FS.SimConnect simConnect )
    {
      // add in the struct sequence
      simConnect?.AddToDataDefinition( _structDefID, "SIM ON GROUND", "bool", FS.SIMCONNECT_DATATYPE.INT32, 0.0f, FS.SimConnect.SIMCONNECT_UNUSED );
      simConnect?.AddToDataDefinition( _structDefID, "BRAKE PARKING POSITION", "bool", FS.SIMCONNECT_DATATYPE.INT32, 0.0f, FS.SimConnect.SIMCONNECT_UNUSED );
      simConnect?.AddToDataDefinition( _structDefID, "SIMULATION RATE", "number", FS.SIMCONNECT_DATATYPE.FLOAT32, 0.0f, FS.SimConnect.SIMCONNECT_UNUSED );
      simConnect?.AddToDataDefinition( _structDefID, "TITLE", "", FS.SIMCONNECT_DATATYPE.STRING256, 0.0f, FS.SimConnect.SIMCONNECT_UNUSED );
      simConnect?.AddToDataDefinition( _structDefID, "ABSOLUTE TIME", "seconds", FS.SIMCONNECT_DATATYPE.FLOAT64, 0.0f, FS.SimConnect.SIMCONNECT_UNUSED );
      simConnect?.AddToDataDefinition( _structDefID, "SIMULATION TIME", "seconds", FS.SIMCONNECT_DATATYPE.FLOAT64, 0.0f, FS.SimConnect.SIMCONNECT_UNUSED );

      simConnect?.RegisterDataDefineStruct<SimVarGetStruct>( _structDefID );
    }

    /// <summary>
    /// Request and update of Vars for SimVars
    ///  should get a callback for DataOnSimObjectType
    /// </summary>
    /// <param name="simConnect">simConnect obj</param>
    public void RequestFromSimConnect( FS.SimConnect simConnect )
    {
      simConnect?.RequestDataOnSimObjectType( _requestID, _structDefID, 0, FS.SIMCONNECT_SIMOBJECT_TYPE.USER );
    }

    /// <summary>
    /// Handle the reception of data from Sim
    /// </summary>
    /// <param name="data">data from Sim</param>
    /// <returns>True when handled</returns>
    public bool HandleSimObjectType( FS.SIMCONNECT_RECV_SIMOBJECT_DATA data )
    {
      if ((data.dwRequestID == (uint)_requestID) && (data.dwDefineID == (uint)_structDefID)) {
        GetStruct = (SimVarGetStruct)data.dwData[0];
        return true;
      }
      return false; // not for us
    }


  }


  [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1 )] // MUST be packed and for strings use the ANSI charset
  internal struct SimVarGetStruct
  {
    /// <summary>
    /// On ground flag
    /// </summary>
    public bool Sim_OnGround => (SIM_ON_GROUND > 0);
    private int SIM_ON_GROUND;

    public bool bParkBrake_active => (BRAKE_PARKING_POSITION > 0);
    private int BRAKE_PARKING_POSITION;

    /// <summary>
    /// Simulation Rate
    /// </summary>
    public float f_Sim_Rate_rate;
    /// <summary>
    /// Name of the Aircraft Config File
    /// </summary>
    [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 256 )]
    public string AcftTitle;

    /// <summary>
    /// Current Time in seconds
    /// </summary>
    public double d_Env_Time_absolute_sec;

    /// <summary>
    /// Seconds since Sim started
    /// </summary>
    public double d_Env_Time_sec;


    // just return the data as string
    public override string ToString( )
    {
      StringBuilder sb = new StringBuilder( );

      sb.AppendLine( $"\tSim on ground  : {Sim_OnGround}" );
      sb.AppendLine( $"\tPark Brake set : {bParkBrake_active}" );
      sb.AppendLine( $"\tSim rate       : {f_Sim_Rate_rate}" );
      sb.AppendLine( $"\tAircraft Config: {AcftTitle}" );
      sb.AppendLine( $"\tSim absolute seconds: {d_Env_Time_absolute_sec}" );
      sb.AppendLine( $"\tSim elapsed seconds : {d_Env_Time_sec}" );
      return sb.ToString( );

    }
  }


}
