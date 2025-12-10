using System;
using System.Runtime.InteropServices;
using System.Text;

using FS = Microsoft.FlightSimulator.SimConnect;

using SimConShared;
using static z_TEST_Target2024.CamVarGetModule;

namespace z_TEST_Target2024
{
  /// <summary>
  /// Retrieves Camera Data (need to see what changed in 2024..)
  /// </summary>
  internal class CamVarGetModule
  {
    public readonly DEFINITION _structDefID = SimConnectIDs.GetDEFINITION( );
    public REQUEST _requestID = SimConnectIDs.GetREQUEST( );
    public REQUEST _subscriptionID = SimConnectIDs.GetREQUEST( );

    private bool _receiving = false;

    /// <summary>
    /// Live Data
    /// </summary>
    public CamVarGetStruct GetStruct = new CamVarGetStruct( );

    /// <summary>
    /// Register the Vars in 'SimVarGetStruct' with SimConnect
    /// </summary>
    /// <param name="simConnect">simConnect obj</param>
    public void RegisterWithSimConnect( FS.SimConnect simConnect )
    {
      // add in the struct sequence, epsilon is set to 0 to receive any change if subsribed
      simConnect?.AddToDataDefinition( _structDefID, "CAMERA STATE", "enum", FS.SIMCONNECT_DATATYPE.INT32, 0.0f, FS.SimConnect.SIMCONNECT_UNUSED );
      simConnect?.AddToDataDefinition( _structDefID, "CAMERA SUBSTATE", "enum", FS.SIMCONNECT_DATATYPE.INT32, 0.0f, FS.SimConnect.SIMCONNECT_UNUSED );
      simConnect?.AddToDataDefinition( _structDefID, "CAMERA VIEW TYPE AND INDEX:0", "enum", FS.SIMCONNECT_DATATYPE.INT32, 0.0f, FS.SimConnect.SIMCONNECT_UNUSED );
      simConnect?.AddToDataDefinition( _structDefID, "CAMERA VIEW TYPE AND INDEX:1", "number", FS.SIMCONNECT_DATATYPE.INT32, 0.0f, FS.SimConnect.SIMCONNECT_UNUSED );

      simConnect?.RegisterDataDefineStruct<CamVarGetStruct>( _structDefID );
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
    /// Toggle subscription
    /// </summary>
    /// <param name="simConnect">simConnect obj</param>
    public void ToggleFromSimConnect( FS.SimConnect simConnect )
    {
      SubscribeFromSimConnect( simConnect, !_receiving );
    }

    /// <summary>
    /// Subscribe to receive updates if a value changes
    ///  Sub is done to report every Visual Frame (cam values don't change that fast)
    /// </summary>
    /// <param name="simConnect">simConnect obj</param>
    /// <param name="on">True to subscribe, false to cancel subscription</param>
    public void SubscribeFromSimConnect( FS.SimConnect simConnect, bool on )
    {
      if (on && !_receiving) {
        // on when Off
        simConnect?.RequestDataOnSimObject( _subscriptionID, _structDefID, (uint)FS.SIMCONNECT_SIMOBJECT_TYPE.USER,
          FS.SIMCONNECT_PERIOD.VISUAL_FRAME, FS.SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0 ); // unlimited reporting
        _receiving = true; // should be on now..
      }
      else if (!on && _receiving) {
        // off when On
        simConnect?.RequestDataOnSimObject( _subscriptionID, _structDefID, (uint)FS.SIMCONNECT_SIMOBJECT_TYPE.USER,
          FS.SIMCONNECT_PERIOD.VISUAL_FRAME, FS.SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0, 0, uint.MaxValue ); // see SDK Doc
        _receiving = false; // should be off now..
      }
    }

    /// <summary>
    /// Handle the reception of data from Sim
    /// </summary>
    /// <param name="data">data from Sim</param>
    /// <returns>True when handled</returns>
    public bool HandleSimObjectType( FS.SIMCONNECT_RECV_SIMOBJECT_DATA data )
    {
      if ((data.dwRequestID == (uint)_requestID) && (data.dwDefineID == (uint)_structDefID)) {
        GetStruct = (CamVarGetStruct)data.dwData[0];
        return true;
      }
      else if ((data.dwRequestID == (uint)_subscriptionID) && (data.dwDefineID == (uint)_structDefID)) {
        GetStruct = (CamVarGetStruct)data.dwData[0];
        return true;
      }
      return false; // not for us
    }


    #region *** Cam Enums so far ... 20250109

    public enum CameraState
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
      CState00 = 0,  // SU10 - n.a.
      CState01 = 1,  // SU10 - n.a.
      Cockpit = 2,
      External_Chase = 3,
      Drone = 4,
      FixedOnPlane = 5,
      Environment = 6,
      SixDoF = 7, // SU10 - SixDoF - set when issuing the 6DOF command on SimConnect
      Gameplay = 8, //SU10 - Gameplay
      Showcase = 9,  //SU10 - Showcase
      DroneAcft = 10,  // SU10 - Drone Aircraft
      Waiting = 11, // SU10 - Waiting
      WorldMap = 12, // SU10 - Worldmap
      HangarRTC = 13, // SU10 - HangarRTC
      HangarCustom = 14, // SU10 - HangarCustom
      MenuRTC = 15, // SU10 - MenuRTC
      InGameRTC = 16, // SU10 - InGameRTC
      Replay = 17, // SU10 - Replay
      CState18 = 18, // SU10 - n.a.
      DroneTopDown = 19, // SU10 - DroneTopDown
      CState20 = 20,  // SU10 - n.a.
      Hangar = 21, // SU10 - Hangar
      CState22 = 22, // SU10 - n.a.
      CState23 = 23, // SU10 - n.a.
      Ground = 24, // SU10
      FollowTrafficAcft = 25, // SU10 - Follow Traffic Aircraft

      // added for FS2024 (don't know yet which are used what they are doing)
      CState26 = 26,
      CState27 = 27,
      CState28 = 28,
      Transition24 = 29,
      StartFlApt24 = 30,
      CState31 = 31,
      MainMenu24 = 32,
      FreeCam24 = 33,
      CState34 = 34,
      Briefing24 = 35,
      StartFlWorld24 = 36,
      CState37 = 37,
      CState38 = 38,
      CState39 = 39,

      /*
      FS2024 as of V1.2.8
       29, 30, 32, 35 states have been reported .. but are not documented at all (1st Pers Views?)

      29 ??
      30 Starting a flight 2nd (runway view - still waiting)
      32 Main Menu 
      33 Free Cam (sel FreeCam after hitting ESC in aircraft)
      35 Briefing?? (Resume, Restart etc. after hitting ESC in aircraft)
      36 Starting a flight 1st (aerial view - still waiting)

       */

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    ///  The options here are generally only required when working with the in-sim panel UI. 
    ///  Note that the "locked" and "unlocked" state will be changed automatically if the following 
    ///  SimVars have their values changed: COCKPIT_CAMERA_HEADLOOK, CHASE_CAMERA_HEADLOOK.
    /// </summary>
    public enum CameraSubState
    {
      /// <summary>
      /// No specific SubState
      /// </summary>
      Default = 0,
      /// <summary>
      /// Fixed look or chase lock - The camera is locked in position
      /// </summary>
      Locked = 1,
      /// <summary>
      /// Head look or Chase normal - The camera is "attached" to the mouse
      /// </summary>
      Unlocked = 2,
      /// <summary>
      /// The camera is currently using a Quickview
      /// </summary>
      Quickview = 3,
      /// <summary>
      /// The camera has Smart camera active
      /// </summary>
      Smart = 4,
      /// <summary>
      /// The camera is focused on an instruments panel
      /// </summary>
      Instrument = 5,
    }

    /// <summary>
    /// Type of view for the current camera
    /// </summary>
    public enum CameraViewType
    {
      /// <summary>
      /// //External / Chase and Showcase / Drone / Fixed / Environment states
      /// </summary>
      Unknown_default = 0,
      /// <summary>
      /// Cockpit State
      /// </summary>
      PilotView = 1,
      /// <summary>
      /// Cockpit State
      /// </summary>
      InstrumentView = 2,
      /// <summary>
      /// // Cockpit State
      /// </summary>
      Quickview = 3,
      /// <summary>
      /// External State
      /// </summary>
      Quickview_External = 4,
      /// <summary>
      /// Showcase / Drone / Fixed / Environment states
      /// </summary>
      OtherView = 5,
    }

    #endregion

  }



  [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1 )] // MUST be packed and for strings use the ANSI charset
  internal struct CamVarGetStruct
  {
    public int iCAMERA_STATE;
    public CameraState CamState => (CameraState)iCAMERA_STATE;
    public int iCAMERA_SUBSTATE;
    public CameraSubState CamSubState => (CameraSubState)iCAMERA_SUBSTATE;
    public int iCAMERA_VIEW_TYPE;
    public CameraViewType CamViewType => (CameraViewType)iCAMERA_VIEW_TYPE;

    public int iCAMERA_VIEW_INDEX;    // is an index...



    // just return the data as string
    public override string ToString( )
    {
      StringBuilder sb = new StringBuilder( );

      try {
        // catch enum conv errors for unknown values
        sb.AppendLine( $"\tCamera state: {CamState}<{iCAMERA_STATE:#0}>, sub: {CamSubState}<{iCAMERA_SUBSTATE:#0}>" );
        sb.AppendLine( $"\t       vType: {CamViewType}<{iCAMERA_VIEW_TYPE:#0}> idx: {iCAMERA_VIEW_INDEX}#0" );
        return sb.ToString( );
      }
      catch {
        sb.AppendLine( $"\tCamera UNK VALUE: state: <{iCAMERA_STATE:#0}>, sub: <{iCAMERA_SUBSTATE:#0}> vType: <{iCAMERA_VIEW_TYPE:#0}> idx: {iCAMERA_VIEW_INDEX}#0" );
        return sb.ToString( );
      }
    }


  }


}
