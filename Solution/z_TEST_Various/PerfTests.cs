using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;

using MSFSAdapter20_24;

namespace z_TEST_Various
{
  internal class PerfTests
  {
    Stopwatch _stopWatch = new Stopwatch( );

    private class PerfProps
    {
      private readonly long nanosecPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;

      public string operationName { get; set; } = "";
      public long numIterations { get; set; } = 0;

      // Define variables for operation statistics.
      public long numTicks { get; set; } = 0;
      public long numRollovers { get; set; } = 0;
      public long maxTicks { get; set; } = 0;
      public long minTicks { get; set; } = Int64.MaxValue;
      public int indexFastest { get; set; } = -1;
      public int indexSlowest { get; set; } = -1;
      public long milliSec { get; set; } = 0;


      public string Results( )
      {
        uint clockSpeed = 0;
        string cpuName = "";
        StringBuilder sb = new StringBuilder( );
        var searcher = new ManagementObjectSearcher( "select MaxClockSpeed from Win32_Processor" );
        foreach (var item in searcher.Get( )) { clockSpeed = (uint)item["MaxClockSpeed"]; }

        searcher = new ManagementObjectSearcher( "select Name from Win32_Processor" );
        foreach (var item in searcher.Get( )) { cpuName = (string)item["Name"]; }

        sb.AppendLine( );
        sb.AppendLine( $"{operationName} Summary:" );
        sb.AppendLine( $"  Slowest time:  op #{indexSlowest:## ### ###} in {numIterations:## ### ###} = {maxTicks:## ### ###} ticks" );
        sb.AppendLine( $"  Fastest time:  op #{indexFastest:## ### ###} in {numIterations:## ### ###} = {minTicks:## ### ###} ticks" );
        sb.AppendLine( $"  Average time:  {(double)numTicks / numIterations:##0.0} ticks = {(double)(numTicks * nanosecPerTick) / numIterations:# ##0.0} nanoseconds" );
        sb.AppendLine( $"  Total time looping through {numIterations:## ### ###} operations: {milliSec:## ### ###} milliseconds" );
        sb.AppendLine( $"Environment: CPU {cpuName} @ {clockSpeed: ## ###}GHz" );

        Console.WriteLine( sb.ToString( ) );

        return sb.ToString( );
      }
    }


    public static void DisplayTimerProperties( )
    {
      // Display the timer frequency and resolution.
      if (Stopwatch.IsHighResolution) {
        Console.WriteLine( "Operations timed using the system's high-resolution performance counter." );
      }
      else {
        Console.WriteLine( "Operations timed using the DateTime class." );
      }

      long frequency = Stopwatch.Frequency;
      Console.WriteLine( "  Timer frequency in ticks per second = {0}", frequency );
      long nanosecPerTick = (1000L * 1000L * 1000L) / frequency;
      Console.WriteLine( "  Timer is accurate within {0} nanoseconds", nanosecPerTick );
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public PerfTests( )
    {
      DisplayTimerProperties( );
    }


    /// <summary>
    /// A DEMO OP (Int.Parse("0") enclosed in try catch
    /// </summary>
    /// <param name="iterations"></param>
    /// <returns>The result string</returns>
    private string DoPerfomanceTest( string testName, int iterations, Func<PerfProps, long> opUnterTest )
    {
      var perf = new PerfProps( ) { operationName = testName, numIterations = iterations };
      Stopwatch time10kOperations = Stopwatch.StartNew( );

      for (int i = 0; i <= perf.numIterations; i++) {
        // OP Operation under Test
        long ticksThisTime = opUnterTest( perf );

        // Skip over the time for the first operation, just in case it caused a one-time performance hit.
        if (i == 0) {
          time10kOperations.Reset( );
          time10kOperations.Start( );
        }
        else {
          // Update operation statistics for iterations 1-N.
          if (perf.maxTicks < ticksThisTime) {
            perf.indexSlowest = i;
            perf.maxTicks = ticksThisTime;
          }
          if (perf.minTicks > ticksThisTime) {
            perf.indexFastest = i;
            perf.minTicks = ticksThisTime;
          }
          perf.numTicks += ticksThisTime;
          if (perf.numTicks < ticksThisTime) {
            // Keep track of rollovers.
            perf.numRollovers++;
          }
        }
      }
      // Display the statistics for N iterations.

      time10kOperations.Stop( );
      perf.milliSec = time10kOperations.ElapsedMilliseconds;

      return perf.Results( );
    }

    #region DEMO Performance Test Operation

    // A DEMO OP (Int.Parse("0") enclosed in try catch
    // returns the elapsed time for the OP
    private long Perf_Demo_OP( PerfProps props )
    {
      // Parse a valid integer using a try-catch statement.
      // Start a new stopwatch timer.
      Stopwatch timePerOP;
      int inputNum;
      timePerOP = Stopwatch.StartNew( );
      {//  OPERATION BLOCK
        try {
          inputNum = Int32.Parse( "0" );
        }
        catch (FormatException) {
          inputNum = 0;
        }
      }
      // Stop the timer, and save the elapsed ticks for the operation.
      timePerOP.Stop( );
      return timePerOP.ElapsedTicks;
    }

    /// <summary>
    /// A DEMO OP (Int.Parse("0") enclosed in try catch
    /// </summary>
    /// <param name="iterations"></param>
    /// <returns>The result string</returns>
    public string Perf_Demo( int iterations )
    {
      return DoPerfomanceTest( "Perf_Demo", iterations, Perf_Demo_OP );
    }

    #endregion

    #region Performance Test CastCopy SIMCONNECT_RECV_EVENT_FRAME

    /// <summary>
    /// Brute force casting...
    /// </summary>
    private static dynamic CastTo<T>( dynamic obj )
    {
      int size = Marshal.SizeOf( obj );
      byte[] bytes = new byte[size];
      // Initialize unmanged memory to hold the struct.
      IntPtr pnt = Marshal.AllocHGlobal( 10000 );
      //      GCHandle handle = GCHandle.Alloc( bytes, GCHandleType.Pinned );
      //Marshal.StructureToPtr( obj, handle.AddrOfPinnedObject( ), true );
      Marshal.StructureToPtr( obj, pnt, true );
      //T theStructure = (T)Marshal.PtrToStructure( handle.AddrOfPinnedObject( ), typeof( T ) );
      T theStructure = (T)Marshal.PtrToStructure( pnt, typeof( T ) );
      //handle.Free( );
      Marshal.FreeHGlobal( pnt );
      return theStructure;
    }

    // returns the elapsed time for the OP
    private long Perf_CopyItemFrame_OP( PerfProps props )
    {
      // Start a new stopwatch timer.
      Stopwatch timePerOP;
      // setup
      var data = new MSFSAdapter20_24.SIMCONNECT_RECV_EVENT_FRAME( ) {
        dwSize = 1,
        dwVersion = 2,
        dwID = 3,
        dwData = 4,
        uEventID = 5,
        uGroupID = 6,
        fFrameRate = 7,
        fSimSpeed = 8,
      };

      timePerOP = Stopwatch.StartNew( );
      {//  OPERATION BLOCK
       // Brute force casting...
        var dataX = CastTo<MSFSAdapter20_24.SIMCONNECT_RECV_EVENT_FRAME>( data );
      }
      // Stop the timer, and save the elapsed ticks for the operation.
      timePerOP.Stop( );
      return timePerOP.ElapsedTicks;
    }

    /// <summary>
    /// Perform a Copy op for item A
    /// </summary>
    /// <param name="iterations">Number of iterations to do</param>
    /// <returns>The result string</returns>
    public string Perf_CopyItemFrame( int iterations )
    {
      return DoPerfomanceTest( "Perf_CopyItemFrame", iterations, Perf_CopyItemFrame_OP );
    }

    #endregion

    #region Performance Test Assign SIMCONNECT_RECV_EVENT_FRAME

    // returns the elapsed time for the OP
    private long Perf_AssignItemFrame_OP( PerfProps props )
    {
      // Start a new stopwatch timer.
      Stopwatch timePerOP;
      // setup
      var data = new MSFSAdapter20_24.SIMCONNECT_RECV_EVENT_FRAME( ) {
        dwSize = 1,
        dwVersion = 2,
        dwID = 3,
        dwData = 4,
        uEventID = 5,
        uGroupID = 6,
        fFrameRate = 7,
        fSimSpeed = 8,
      };

      timePerOP = Stopwatch.StartNew( );
      {//  OPERATION BLOCK
        var dataX = new MSFSAdapter20_24.SIMCONNECT_RECV_EVENT_FRAME( ) {
          dwSize = data.dwSize,
          dwVersion = data.dwVersion,
          dwID = data.dwID,
          dwData = data.dwData,
          uEventID = data.uEventID,
          uGroupID = data.uGroupID,
          fFrameRate = data.fFrameRate,
          fSimSpeed = data.fSimSpeed,
        };
      }
      // Stop the timer, and save the elapsed ticks for the operation.
      timePerOP.Stop( );
      return timePerOP.ElapsedTicks;
    }

    /// <summary>
    /// Perform a Copy op for item A
    /// </summary>
    /// <param name="iterations">Number of iterations to do</param>
    /// <returns>The result string</returns>
    public string Perf_AssignItemFrame( int iterations )
    {
      return DoPerfomanceTest( "Perf_AssignItemFrame", iterations, Perf_AssignItemFrame_OP );
    }

    #endregion

    #region Performance Test CastCopy SIMCONNECT_RECV_AIRPORT_LIST

    // returns the elapsed time for the OP
    private long Perf_CopyItemList_OP( PerfProps props )
    {
      // Start a new stopwatch timer.
      Stopwatch timePerOP;
      // setup
      var data = new MSFSAdapter20_24.SIMCONNECT_RECV_AIRPORT_LIST( ) {
        dwSize = 1,
        dwVersion = 2,
        dwID = 3,
        dwRequestID = 4,
        dwEntryNumber = 1,
        dwOutOf = 1,
        dwArraySize = 200, // num entries
      };
      SIMCONNECT_DATA_FACILITY_AIRPORT[] arr = new SIMCONNECT_DATA_FACILITY_AIRPORT[data.dwArraySize];
      for (int i = 0; i < arr.Length; i++) {
        arr[i] = new SIMCONNECT_DATA_FACILITY_AIRPORT( ) { Altitude = i, Ident = "TEST", Latitude = 0, Longitude = 0, Region = "TE" };
      }
      data.rgData = arr;

      timePerOP = Stopwatch.StartNew( );
      {//  OPERATION BLOCK
       // Brute force casting...
        var dataX = CastTo<MSFSAdapter20_24.SIMCONNECT_RECV_AIRPORT_LIST>( data );
      }
      // Stop the timer, and save the elapsed ticks for the operation.
      timePerOP.Stop( );
      return timePerOP.ElapsedTicks;
    }

    /// <summary>
    /// Perform a Copy op for item A
    /// </summary>
    /// <param name="iterations">Number of iterations to do</param>
    /// <returns>The result string</returns>
    public string Perf_CopyItemList( int iterations )
    {
      return DoPerfomanceTest( "Perf_CopyItemList", iterations, Perf_CopyItemList_OP );
    }

    #endregion

    #region Performance Test Assign SIMCONNECT_RECV_AIRPORT_LIST

    // returns the elapsed time for the OP
    private long Perf_AssignItemList_OP( PerfProps props )
    {
      // Start a new stopwatch timer.
      Stopwatch timePerOP;
      // setup
      var data = new MSFSAdapter20_24.SIMCONNECT_RECV_AIRPORT_LIST( ) {
        dwSize = 1,
        dwVersion = 2,
        dwID = 3,
        dwRequestID = 4,
        dwEntryNumber = 1,
        dwOutOf = 1,
        dwArraySize = 200, // num entries
      };
      SIMCONNECT_DATA_FACILITY_AIRPORT[] arr = new SIMCONNECT_DATA_FACILITY_AIRPORT[data.dwArraySize];
      for (int i = 0; i < arr.Length; i++) {
        arr[i] = new SIMCONNECT_DATA_FACILITY_AIRPORT( ) { Altitude = i, Ident = "TEST", Latitude = 0, Longitude = 0, Region = "TE" };
      }
      data.rgData = arr;


      timePerOP = Stopwatch.StartNew( );
      {//  OPERATION BLOCK
        var dataX = new SIMCONNECT_RECV_AIRPORT_LIST( ) {
          dwSize = data.dwSize,
          dwID = data.dwID,
          dwVersion = data.dwVersion,
          dwArraySize = data.dwArraySize,
          dwEntryNumber = data.dwEntryNumber,
          dwOutOf = data.dwOutOf,
          dwRequestID = data.dwRequestID,
          rgData = data.rgData,
        };
      }
      // Stop the timer, and save the elapsed ticks for the operation.
      timePerOP.Stop( );
      return timePerOP.ElapsedTicks;
    }

    /// <summary>
    /// Perform a Copy op for item A
    /// </summary>
    /// <param name="iterations">Number of iterations to do</param>
    /// <returns>The result string</returns>
    public string Perf_AssignItemList( int iterations )
    {
      return DoPerfomanceTest( "Perf_AssignItemList", iterations, Perf_AssignItemList_OP );
    }

    #endregion

    #region Performance Test Assign SIMCONNECT_RECV_AIRPORT_LIST

    // needs Ident adjustment
    // used by SIMCONNECT_RECV_AIRPORT_LIST
    public static SIMCONNECT_DATA_FACILITY_AIRPORT GetFrom( SIMCONNECT_DATA_FACILITY_AIRPORT orig )
    {
      return new SIMCONNECT_DATA_FACILITY_AIRPORT( ) {
        Ident = orig.Ident,
        Region = orig.Region,
        Altitude = orig.Altitude,
        Latitude = orig.Latitude,
        Longitude = orig.Longitude,
      };
    }
    // needs Ident adjustment
    public static SIMCONNECT_RECV_AIRPORT_LIST GetFrom( SIMCONNECT_RECV_AIRPORT_LIST orig )
    {
      var ret = new SIMCONNECT_RECV_AIRPORT_LIST( ) {
        // SIMCONNECT_RECV
        dwID = orig.dwID,
        dwVersion = orig.dwVersion,
        dwSize = orig.dwSize,
        // SIMCONNECT_RECV_LIST_TEMPLATE 
        dwRequestID = orig.dwRequestID,
        dwArraySize = orig.dwArraySize,
        dwEntryNumber = orig.dwEntryNumber,
        dwOutOf = orig.dwOutOf,
      };
      Array.Resize( ref ret.rgData, (int)orig.dwArraySize );
      for (int i = 0; i < (int)orig.dwArraySize; i++) {
        ret.rgData[i] = GetFrom( (SIMCONNECT_DATA_FACILITY_AIRPORT)orig.rgData[i] );
      }
      ret.dwSize = orig.dwSize + (orig.dwArraySize * 3 * 1); // 1 Ident converted per entry

      return ret;
    }

    // returns the elapsed time for the OP
    private long Perf_AssignItemList2020_OP( PerfProps props )
    {
      // Start a new stopwatch timer.
      Stopwatch timePerOP;
      // setup
      var data = new MSFSAdapter20_24.SIMCONNECT_RECV_AIRPORT_LIST( ) {
        dwSize = 1,
        dwVersion = 2,
        dwID = 3,
        dwRequestID = 4,
        dwEntryNumber = 1,
        dwOutOf = 1,
        dwArraySize = 200, // num entries
      };
      SIMCONNECT_DATA_FACILITY_AIRPORT[] arr = new SIMCONNECT_DATA_FACILITY_AIRPORT[data.dwArraySize];
      for (int i = 0; i < arr.Length; i++) {
        arr[i] = new SIMCONNECT_DATA_FACILITY_AIRPORT( ) { Altitude = i, Ident = "TEST", Latitude = 0, Longitude = 0, Region = "TE" };
      }
      data.rgData = arr;

      timePerOP = Stopwatch.StartNew( );
      {//  OPERATION BLOCK
        var dataX = GetFrom( data );
      }
      // Stop the timer, and save the elapsed ticks for the operation.
      timePerOP.Stop( );
      return timePerOP.ElapsedTicks;
    }

    /// <summary>
    /// Perform a Copy op for item A
    /// </summary>
    /// <param name="iterations">Number of iterations to do</param>
    /// <returns>The result string</returns>
    public string Perf_AssignItemList2020( int iterations )
    {
      return DoPerfomanceTest( "Perf_AssignItemList2020", iterations, Perf_AssignItemList2020_OP );
    }

    #endregion

  }
}
