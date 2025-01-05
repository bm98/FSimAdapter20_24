using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace z_TEST_Various
{
  public partial class Form1 : Form
  {
    public Form1( )
    {
      InitializeComponent( );
    }

    private void btPerf_Click( object sender, EventArgs e )
    {
      var px = new PerfTests( );
      RTB.Text += px.Perf_Demo( 1_000_000 );

    }

    private void btPerfFrameAssign_Click( object sender, EventArgs e )
    {
      var px = new PerfTests( );
      RTB.Text += px.Perf_AssignItemFrame( 10_000_000 );
    }

    private void btListAssign2020_Click( object sender, EventArgs e )
    {
      var px = new PerfTests( );
      RTB.Text += px.Perf_AssignItemList2020( 1_000_000 );
    }

    private void btPerfListAssign_Click( object sender, EventArgs e )
    {
      var px = new PerfTests( );
      RTB.Text += px.Perf_AssignItemList( 1_000_000 );
    }
  }
}
