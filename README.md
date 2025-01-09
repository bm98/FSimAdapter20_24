# MSFS SimConnect Adapter for 2020 and 2024 (c# .Net)

# ALPHA VERSION - WORK IN PROGRESS

for the time beeing it must be considered as 'Proof of Concept'   

---

### c# .Net Adapter to seamlessly use MSFS 2020 and/or 2024 SimConnect

* Hides the two versions of SimConnect behind an Adapter  
* Exposes the MSFS2024 interface of SimConnect  
* Attaches the respective versions DLLs dynamically when either version of the Sim is in use  
* Adds a Helper class for SimConnectID management
* Adds a Helper class for dissecting the managed SimConnect DLL (finding differences)
* Adds a Helper method for message handling

MSFS2020 breaking changes are taken care of and it should work seamlessly  
Methods not available in MSFS2020 will raise an Exception callback but should otherwise not cause harm  

### Performance

The adapter maps objects between namespaces when calling or receiving data. 
As .Net does not support type casting for foreign objects. The adapter copies the sent or received item into it's needed form.  
Serializing is not an option as the FS records are not tagged for it. Also copy via Marshalling is about 10x slower than this trivial approach.  

When measuring the mapping time I found the following results  

Copy of SIMCONNECT_RECV_EVENT_FRAME  

```
Perf_AssignItemFrame Summary: 
  Slowest time:  op # 20'230 in 10'000'000 =  4'932 ticks
  Fastest time:  op #  1 in 10'000'000 =    ticks
  Average time:  0.3 ticks =  33.4 nanoseconds  <<<<<<------
  Total time looping through 10'000'000 operations:   866 milliseconds
Environment: CPU Intel(R) Core(TM) i7-14700KF @3.400GHz
```

Copy of SIMCONNECT_RECV_AIRPORT_LIST with 200 entries  
Due to namespaces there is no cast for list items either - so we need to copy the whole packet each time  

```
Perf_AssignItemList2020 Summary:
  Slowest time:  op # 9'741 in 1'000'000 =  5 239 ticks
  Fastest time:  op # 17'976 in 1'000'000 =   25 ticks
  Average time:  29.3 ticks = 2'929.6 nanoseconds (~3ms) <<<<<<------
  Total time looping through 1'000'000 operations:  4'068 milliseconds
Environment: CPU Intel(R) Core(TM) i7-14700KF @3.400GHz
```



### Limitations

Right now the following two calls are not mapped (don't know how to map the types between the namespaces...)

```
public void RegisterStruct<RECV, T>( Enum dwID ) where RECV : SIMCONNECT_RECV

public void ReceiveDispatch( SignalProcDelegate pfcnSignal )
```

If needed let me know or let me know how to map them.  


## Usage

Reference MSFSAdapter_20_24 instead of Microsoft.FlightSimulator.SimConnect  
Put the two Deployment folders V2020 and V2024 into the target folder   

Create an instance of the adapters SimConnect class  
 (The Adapter will not open the connection FS when creating it like the original does)  
Wire events at will  
Call the adapters Init() method which attempts to start the connection.  

!! The Adapter can only progress and connect with SimConnect once a running MSFS2020 or 2024 App is detected  

Init() will return FSVersion == Unknown when no running Sim is detected  
Wait some time to retry or give up  
Note: Init may raise Exceptions if something went wrong and the Adapter cannot work e.g. DLLs cannot be found  

Once Init is successfull use the Adapter exactly like the original SimConnect obj.  
The MSFS2024 SimConnect Const, Enums, Structs, Events, and Methods are exposed and used by the Adapter,   
-> don't reference the managed SimConnect DLL (else it will not work...)  

If you code with:  

```
using FS = Microsoft.FlightSimulator.SimConnect;
..
FS.RefSomething
```
you may later only change to
```
using FS = MSFSAdapter_20_24;
```

in order to use the adapter  


## Examples

### A simple connect check  

```
using FS = MSFSAdapter_20_24;

...
    private FS.SimConnect _SC = null;
    private FS.FSVersion _version = FS.FSVersion.Unknown;

    // primitive connect check and dispose
    private void ConnectAndDispose( )
    {
      _SC?.Dispose( );
      // create an instance without using callbacks
      _SC = new FS.SimConnect( "ADAPTER_TEST_APP", IntPtr.Zero, FS.SimConnect.WM_USER_NONE, null, 0 );

      // *** Add this when using the Adapter
      _version = _SC.Init( ); // Init default

      if (_version == FS.FSVersion.Unknown) {
        Console.WriteLine("No MSFS App seems running");
      }
      else {
        Console.WriteLine( $"MSFS App {_version} is detected");
      }

      _SC?.Dispose( );
      _SC = null;
    }
```

### Connecting using the WinMsg queue and a WinForms Form (WinForms App)

```
using FS = MSFSAdapter_20_24;

...
    private FS.SimConnect _SC = null;
    private FS.FSVersion _version = FS.FSVersion.Unknown;

    // Connect for use with Callbacks
    private void Connect( )
    {
      _SC?.Dispose( );
      // create an instance using callbacks
      // using this Form as WIN MSG receiver and the provided Message ID
      _SC = new FS.SimConnect( "ADAPTER_TEST_APP", this.Handle, FS.SimConnect.WM_USER_SIMCONNECT, null, 0 );

      AttachHandlers( ); // wire events to be received

      // *** Add this when using the Adapter
      _version = _SC.Init( ); // Init default

      if (_version == FS.FSVersion.Unknown) {
        Console.WriteLine( $"No MSFS App seems running" );
        Console.WriteLine( $"Adapter Shutdown" );
        _SC?.Dispose( );
        _SC = null;
        return;
      }
      Console.WriteLine( $"Adapter is alive" );
    }

    private void AttachHandlers( )
    {
      // sanity
      if (_SC == null) return;

      try {
        _SC.OnRecvOpen += new FS.SimConnect.RecvOpenEventHandler( _simConnect_OnRecvOpen );
        _SC.OnRecvQuit += new FS.SimConnect.RecvQuitEventHandler( _simConnect_OnRecvQuit );
        _SC.OnRecvException += new FS.SimConnect.RecvExceptionEventHandler( _simConnect_OnRecvException );

        // events and request replies
        _SC.OnRecvSystemState += new FS.SimConnect.RecvSystemStateEventHandler( _simConnect_OnRecvSystemState );
        _SC.OnRecvEvent += new FS.SimConnect.RecvEventEventHandler( _simConnect_OnRecvEvent );
        _SC.OnRecvEventFilename += new FS.SimConnect.RecvEventFilenameEventHandler( _simConnectRef_OnRecvEventFilename );

        _SC.OnRecvEventFrame += new FS.SimConnect.RecvEventFrameEventHandler( _simConnect_OnRecvEventFrame );
        _SC.OnRecvSimobjectDataBytype += new FS.SimConnect.RecvSimobjectDataBytypeEventHandler( _simConnect_OnRecvSimobjectDataBytype );
        _SC.OnRecvClientData += new FS.SimConnect.RecvClientDataEventHandler( _simConnect_OnRecvClientData );

        _SC.OnRecvEnumerateInputEvents += new FS.SimConnect.RecvEnumerateInputEventsEventHandler( _simConnect_OnRecvEnumerateInputEvents ); // enum B events
        _SC.OnRecvSubscribeInputEvent += new FS.SimConnect.RecvSubscribeInputEventEventHandler( _simConnect_OnRecvSubscribeInputEvent );// monitor B Events
      }
      catch (Exception ex) {
        Console.WriteLine( "AttachSimEvents_Base: Failed" );
      }
    }
    
    /// <summary>
    /// Windows Message Handler Override 
    /// - must handle messages from SimConnect otherwise SimConnect does not provide events and callbacks
    /// Using the Adapter provided MessageHandler
    /// </summary>
    protected override void DefWndProc( ref Message m )
    {
      if ((_SC != null) && (_version != FS.FSVersion.Unknown)) {
        try {
          // catch MSFS exits in the worst moment
          if (_SC.WinMessageHandled( m.Msg )) return; // Event was handled
        }
        catch { }
      }
      // everything else goes here
      base.DefWndProc( ref m ); // default handling for this Window
    }

```


---

.NET Framework 4.8 Library DLL

Built with VisualStudio 2022 Community free version



EOD
