﻿namespace SimConnector
{
  partial class MsgForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing )
    {
      if (disposing && (components != null)) {
        components.Dispose( );
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent( )
    {
      this.SuspendLayout();
      // 
      // MsgForm
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.CausesValidation = false;
      this.ClientSize = new System.Drawing.Size(259, 172);
      this.ControlBox = false;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MsgForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "MsgForm";
      this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
      this.Load += new System.EventHandler(this.MsgForm_Load);
      this.ResumeLayout(false);

    }

    #endregion
  }
}