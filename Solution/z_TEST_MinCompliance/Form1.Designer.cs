namespace z_TEST_MinCompliance
{
  partial class Form1
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
      this.btConnect = new System.Windows.Forms.Button();
      this.RTB = new System.Windows.Forms.RichTextBox();
      this.btConDiscon = new System.Windows.Forms.Button();
      this.btReqSome = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btConnect
      // 
      this.btConnect.Location = new System.Drawing.Point(235, 12);
      this.btConnect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btConnect.Name = "btConnect";
      this.btConnect.Size = new System.Drawing.Size(124, 43);
      this.btConnect.TabIndex = 0;
      this.btConnect.Text = "Connect";
      this.btConnect.UseVisualStyleBackColor = true;
      this.btConnect.Click += new System.EventHandler(this.btConnect_Click);
      // 
      // RTB
      // 
      this.RTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.RTB.Location = new System.Drawing.Point(12, 70);
      this.RTB.Name = "RTB";
      this.RTB.Size = new System.Drawing.Size(909, 437);
      this.RTB.TabIndex = 1;
      this.RTB.Text = "";
      // 
      // btConDiscon
      // 
      this.btConDiscon.Location = new System.Drawing.Point(12, 12);
      this.btConDiscon.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btConDiscon.Name = "btConDiscon";
      this.btConDiscon.Size = new System.Drawing.Size(124, 43);
      this.btConDiscon.TabIndex = 2;
      this.btConDiscon.Text = "Minimum Connect";
      this.btConDiscon.UseVisualStyleBackColor = true;
      this.btConDiscon.Click += new System.EventHandler(this.btConDiscon_Click);
      // 
      // btReqSome
      // 
      this.btReqSome.Location = new System.Drawing.Point(397, 12);
      this.btReqSome.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btReqSome.Name = "btReqSome";
      this.btReqSome.Size = new System.Drawing.Size(124, 43);
      this.btReqSome.TabIndex = 3;
      this.btReqSome.Text = "Request Some";
      this.btReqSome.UseVisualStyleBackColor = true;
      this.btReqSome.Click += new System.EventHandler(this.btReqSome_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(933, 519);
      this.Controls.Add(this.btReqSome);
      this.Controls.Add(this.btConDiscon);
      this.Controls.Add(this.RTB);
      this.Controls.Add(this.btConnect);
      this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btConnect;
    private System.Windows.Forms.RichTextBox RTB;
    private System.Windows.Forms.Button btConDiscon;
    private System.Windows.Forms.Button btReqSome;
  }
}

