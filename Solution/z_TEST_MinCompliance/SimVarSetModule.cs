using System;
using System.Runtime.InteropServices;
using System.Text;

using FS = MSFSAdapter20_24;

using SimConnectToolkit;

namespace z_TEST_MinCompliance
{
  internal class SimVarSetModule
  {
    public readonly DEFINITION _structDefID = SimConnectIDs.GetDEFINITION( );
    public REQUEST _requestID = SimConnectIDs.GetREQUEST( );

    /// <summary>
    /// Live Data
    /// </summary>
    public SimVarSetStruct SetStruct = new SimVarSetStruct( );

    /// <summary>
    /// Register the Vars in 'SimVarGetStruct' with SimConnect
    /// </summary>
    /// <param name="simConnect">simConnect obj</param>
    public void RegisterWithSimConnect( FS.SimConnect simConnect )
    {
      // add in the struct sequence
      simConnect?.AddToDataDefinition( _structDefID, "AUTOPILOT FLIGHT LEVEL CHANGE", "bool", FS.SIMCONNECT_DATATYPE.INT32, 0.0f, FS.SimConnect.SIMCONNECT_UNUSED );
      simConnect?.AddToDataDefinition( _structDefID, "AUTOPILOT HEADING LOCK DIR", "degrees", FS.SIMCONNECT_DATATYPE.FLOAT32, 0.0f, FS.SimConnect.SIMCONNECT_UNUSED );
      simConnect?.AddToDataDefinition( _structDefID, "AUTOPILOT VERTICAL HOLD VAR", "feet/minute", FS.SIMCONNECT_DATATYPE.FLOAT64, 0.0f, FS.SimConnect.SIMCONNECT_UNUSED );

      simConnect?.RegisterDataDefineStruct<SimVarGetStruct>( _structDefID );
    }

    /// <summary>
    /// Will change the settable vars to detect a change...
    /// </summary>
    public void ChangeVarsABit( )
    {
      SetStruct.SetFLC( !SetStruct.bFLC_active ); // toggle
      SetStruct.SetHDG( SetStruct.fHDG_setting_deg + (SetStruct.bFLC_active ? 10f : -10f) ); // change with FLC state (sends +-10)
      SetStruct.SetVS( SetStruct.fVS_setting_fpm + (SetStruct.bFLC_active ? 1000f : -1000f) ); // change with FLC state (sends +-1000)
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
    /// Send Data to the Sim
    /// </summary>
    /// <param name="simConnect">simConnect obj</param>
    public void RequestToSimConnect( FS.SimConnect simConnect )
    {
      simConnect?.SetDataOnSimObject(
              _structDefID,
              (uint)FS.SIMCONNECT_SIMOBJECT_TYPE.USER,
              FS.SIMCONNECT_DATA_SET_FLAG.DEFAULT,
              SetStruct
              );
    }

    /// <summary>
    /// Handle the reception of data from Sim
    /// </summary>
    /// <param name="data">data from Sim</param>
    /// <returns>True when handled</returns>
    public bool HandleSimObjectType( FS.SIMCONNECT_RECV_SIMOBJECT_DATA data )
    {
      if ((data.dwRequestID == (uint)_requestID) && (data.dwDefineID == (uint)_structDefID)) {
        SetStruct = (SimVarSetStruct)data.dwData[0];
        return true;
      }
      return false; // not for us
    }


  }


  [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1 )] // MUST be packed and for strings use the ANSI charset
  internal struct SimVarSetStruct
  {
    public bool bFLC_active => (AUTOPILOT_FLIGHT_LEVEL_CHANGE > 0);
    private int AUTOPILOT_FLIGHT_LEVEL_CHANGE;
    public void SetFLC( bool on ) => AUTOPILOT_FLIGHT_LEVEL_CHANGE = on ? 1 : 0;

    public float fHDG_setting_deg;
    public void SetHDG( float setting ) => fHDG_setting_deg = setting;

    public float fVS_setting_fpm;
    public void SetVS( float setting ) => fVS_setting_fpm = setting;

    // just return the data as string
    public override string ToString( )
    {
      StringBuilder sb = new StringBuilder( );

      sb.AppendLine( $"\tFLC on : {bFLC_active}" );
      sb.AppendLine( $"\tHDG deg: {fHDG_setting_deg}000" );
      sb.AppendLine( $"\tVS fpm : {fVS_setting_fpm:#'##0}" );
      return sb.ToString( );

    }
  }


}
