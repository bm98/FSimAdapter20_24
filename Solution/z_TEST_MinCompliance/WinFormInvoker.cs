using System;
using System.Windows.Forms;

namespace z_TEST_MinCompliance
{
  /// <summary>
  /// Handles Events in WinForms and invokes a delegate if required
  /// </summary>
  internal class WinFormInvoker
  {
    private readonly Control _cctrl;

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="cctrl">The Control to handle</param>
    public WinFormInvoker( Control cctrl )
    {
      _cctrl = cctrl;
    }

    /// <summary>
    /// Handle Events on behalf of the Form
    /// </summary>
    /// <param name="method">An parameterless method to execute</param>
    public void HandleEvent( Action method )
    {
      // sanity 
      if (_cctrl == null) return;

      if (_cctrl.InvokeRequired) {
        _cctrl.Invoke( (MethodInvoker)delegate { method( ); } );
      }
      else {
        method( );
      }
    }

  }
}
