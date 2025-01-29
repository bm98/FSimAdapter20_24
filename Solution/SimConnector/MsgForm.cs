using MSFSAdapter20_24;

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimConnector
{
  public partial class MsgForm : Form
  {
    private SimCon _parent;

    public MsgForm( SimCon parent )
    {
      _parent = parent;

      InitializeComponent( );
    }

    private void MsgForm_Load( object sender, EventArgs e )
    {
      // out of the way...
      this.Location = new Point( -2000, -2000 );
      this.Visible = false;
    }

    /// <summary>
    /// Windows Message Handler Override 
    /// - must handle messages from SimConnect otherwise SimConnect does not provide events and callbacks
    /// Using the provided MessageHandler
    /// </summary>
    protected override void DefWndProc( ref Message m )
    {
      if (_parent != null) {
        if (_parent.SimConnectWndProc( ref m )) return; // handled 
      }
      // everything else goes here
      base.DefWndProc( ref m ); // default handling for this Window
    }

  }
}
