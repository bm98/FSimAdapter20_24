using System;
using System.Linq;
using System.Windows.Forms;

using MSFSAdapter20_24;

namespace z_TEST_Decomposer
{
  public partial class Form1 : Form
  {
    private SimConnect _SC = null;


    public Form1( )
    {
      InitializeComponent( );
    }

    private void btDc2020_Click( object sender, EventArgs e )
    {
      _SC?.Dispose( );
      // create an instance without using callbacks
      RTB.Text += "Create V2020\n";
      _SC = new SimConnect( "DECOMPOSER2020", IntPtr.Zero, SimConnect.WM_USER_NONE, null, 0 );

      RTB.Text += "Init V2020\n";
      _SC.Init( FSVersion.V2020 ); // Init MSFS2020 dlls

      DecomposeAssembly( );
    }


    private void btDc2024_Click( object sender, EventArgs e )
    {
      _SC?.Dispose( );
      // create an instance without using callbacks
      RTB.Text += "Create V2024\n";
      _SC = new SimConnect( "DECOMPOSER2024", IntPtr.Zero, SimConnect.WM_USER_NONE, null, 0 );

      RTB.Text += "Init V2024\n";
      _SC.Init( FSVersion.V2024 ); // Init MSFS2024 dlls

      DecomposeAssembly( );
    }

    // Dissects the Assembly to find out differences between
    private void DecomposeAssembly( )
    {
      var currentDomain = AppDomain.CurrentDomain;
      // find Microsoft.FlightSimulator.SimConnect assembly currently loaded
      var mssc = currentDomain.GetAssemblies( ).FirstOrDefault( a => a.FullName.Contains( "Microsoft.FlightSimulator.SimConnect" ) );
      if (mssc != null) {
        RTB.Text = NetDecomposer.DecompAssembly( mssc );
      }
      else {
        RTB.Text += "****  Could not find the Assembly 'Microsoft.FlightSimulator.SimConnect'";
      }
    }


  }
}
