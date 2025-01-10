using System;

using FS = MSFSAdapter20_24;

using SimConnectToolkit;

namespace z_TEST_MinCompliance
{
  /// <summary>
  /// Send a KeyEvent to the Sim
  /// </summary>
  internal class SetKeyEventModule
  {
    // have to register an DEFINITION before we can send it

    public readonly DEFINITION _keyDefID = SimConnectIDs.GetDEFINITION( );
    private string _keyName = "";
    public string KeyName { get => _keyName; }
    public SetKeyEventModule( string keyName )
    {
      _keyName = keyName;
    }

    /// <summary>
    /// Register the Vars in 'SimVarGetStruct' with SimConnect
    /// </summary>
    /// <param name="simConnect">simConnect obj</param>
    public void RegisterWithSimConnect( FS.SimConnect simConnect )
    {
      simConnect?.MapClientEventToSimEvent( _keyDefID, KeyName );
    }

    /// <summary>
    /// Send Key with optional Data to the Sim
    /// </summary>
    /// <param name="simConnect">simConnect obj</param>
    public void RequestToSimConnect( FS.SimConnect simConnect, uint data = 0 )
    {
      simConnect?.TransmitClientEvent( 0, _keyDefID, data, (GroupID)FS.SimConnect.SIMCONNECT_GROUP_PRIORITY_HIGHEST, FS.SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY );
    }


  }
}
