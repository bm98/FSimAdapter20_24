using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSFSAdapter20_24
{
  /// <summary>
  /// Provide info about running MSFS instances
  /// </summary>
  public static class MSFS
  {
    private const string c_2020ExeName = "FlightSimulator.exe";
    private const string c_2020WindowName = "Microsoft Flight Simulator"; // starts with
    private const string c_2024ExeName = "FlightSimulator2024.exe";
    private const string c_2024WindowName = "Microsoft Flight Simulator 2024"; // starts with

    /// <summary>
    /// MSFS2020 WindowTitle if is detected (running)
    /// </summary>
    /// <returns>Window Title or empty</returns>
    public static string MSFS2020running( )
    {
      // get running processes and find the one of either MSFS Sim Exe
      return CheckExeRunning( c_2020ExeName, c_2020WindowName );
    }

    /// <summary>
    /// MSFS2024 WindowTitle if is detected (running)
    /// </summary>
    /// <returns>Window Title or empty</returns>
    public static string MSFS2024running( )
    {
      return CheckExeRunning( c_2024ExeName, c_2024WindowName );
    }

    // true when an exe having a window with title is running
    private static string CheckExeRunning( string exeName, string windowName )
    {
      bool exeRunning = false;
      string fsWindowTitle = "";

      // check if it is running
      IEnumerable<Process> processes = Process.GetProcesses( ).Where( p => p.MainWindowHandle != IntPtr.Zero ); // only processes with WindowHandles
      foreach (Process p in processes) {
        try {
          // may trigger access violation on certain modules
          if (!string.IsNullOrEmpty( p.MainModule.ModuleName )) {
            if ((p.MainModule.ModuleName == exeName) && p.MainWindowTitle.StartsWith( windowName )) {
              fsWindowTitle = p.MainWindowTitle;
              exeRunning = true;
              break;
            }
          }
        }
        catch { continue; }
      }
      return exeRunning ? fsWindowTitle : "";
    }

  }
}
