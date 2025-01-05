namespace z_TEST_Various
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
      this.btPerf = new System.Windows.Forms.Button();
      this.RTB = new System.Windows.Forms.RichTextBox();
      this.btPerfFrameAssign = new System.Windows.Forms.Button();
      this.btPerfListAssign = new System.Windows.Forms.Button();
      this.btListAssign2020 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btPerf
      // 
      this.btPerf.Location = new System.Drawing.Point(439, 12);
      this.btPerf.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btPerf.Name = "btPerf";
      this.btPerf.Size = new System.Drawing.Size(126, 47);
      this.btPerf.TabIndex = 0;
      this.btPerf.Text = "Perf DEMO call";
      this.btPerf.UseVisualStyleBackColor = true;
      this.btPerf.Click += new System.EventHandler(this.btPerf_Click);
      // 
      // RTB
      // 
      this.RTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.RTB.Location = new System.Drawing.Point(14, 118);
      this.RTB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.RTB.Name = "RTB";
      this.RTB.Size = new System.Drawing.Size(579, 387);
      this.RTB.TabIndex = 1;
      this.RTB.Text = "";
      // 
      // btPerfFrameAssign
      // 
      this.btPerfFrameAssign.Location = new System.Drawing.Point(13, 12);
      this.btPerfFrameAssign.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btPerfFrameAssign.Name = "btPerfFrameAssign";
      this.btPerfFrameAssign.Size = new System.Drawing.Size(126, 47);
      this.btPerfFrameAssign.TabIndex = 3;
      this.btPerfFrameAssign.Text = "Perf FrameAssign";
      this.btPerfFrameAssign.UseVisualStyleBackColor = true;
      this.btPerfFrameAssign.Click += new System.EventHandler(this.btPerfFrameAssign_Click);
      // 
      // btPerfListAssign
      // 
      this.btPerfListAssign.Location = new System.Drawing.Point(148, 65);
      this.btPerfListAssign.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btPerfListAssign.Name = "btPerfListAssign";
      this.btPerfListAssign.Size = new System.Drawing.Size(126, 47);
      this.btPerfListAssign.TabIndex = 5;
      this.btPerfListAssign.Text = "Perf ListAssign";
      this.btPerfListAssign.UseVisualStyleBackColor = true;
      this.btPerfListAssign.Click += new System.EventHandler(this.btPerfListAssign_Click);
      // 
      // btListAssign2020
      // 
      this.btListAssign2020.Location = new System.Drawing.Point(148, 12);
      this.btListAssign2020.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btListAssign2020.Name = "btListAssign2020";
      this.btListAssign2020.Size = new System.Drawing.Size(126, 47);
      this.btListAssign2020.TabIndex = 4;
      this.btListAssign2020.Text = "Perf ListAssign2020";
      this.btListAssign2020.UseVisualStyleBackColor = true;
      this.btListAssign2020.Click += new System.EventHandler(this.btListAssign2020_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(607, 519);
      this.Controls.Add(this.btPerfListAssign);
      this.Controls.Add(this.btListAssign2020);
      this.Controls.Add(this.btPerfFrameAssign);
      this.Controls.Add(this.RTB);
      this.Controls.Add(this.btPerf);
      this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btPerf;
    private System.Windows.Forms.RichTextBox RTB;
    private System.Windows.Forms.Button btPerfFrameAssign;
    private System.Windows.Forms.Button btPerfListAssign;
    private System.Windows.Forms.Button btListAssign2020;
  }
}

