namespace z_TEST_Decomposer
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
      this.btDc2020 = new System.Windows.Forms.Button();
      this.btDc2024 = new System.Windows.Forms.Button();
      this.RTB = new System.Windows.Forms.RichTextBox();
      this.SuspendLayout();
      // 
      // btDc2020
      // 
      this.btDc2020.Location = new System.Drawing.Point(14, 14);
      this.btDc2020.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btDc2020.Name = "btDc2020";
      this.btDc2020.Size = new System.Drawing.Size(138, 60);
      this.btDc2020.TabIndex = 0;
      this.btDc2020.Text = "Decomp V2020";
      this.btDc2020.UseVisualStyleBackColor = true;
      this.btDc2020.Click += new System.EventHandler(this.btDc2020_Click);
      // 
      // btDc2024
      // 
      this.btDc2024.Location = new System.Drawing.Point(247, 14);
      this.btDc2024.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btDc2024.Name = "btDc2024";
      this.btDc2024.Size = new System.Drawing.Size(138, 60);
      this.btDc2024.TabIndex = 1;
      this.btDc2024.Text = "Decomp V2024";
      this.btDc2024.UseVisualStyleBackColor = true;
      this.btDc2024.Click += new System.EventHandler(this.btDc2024_Click);
      // 
      // RTB
      // 
      this.RTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.RTB.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RTB.Location = new System.Drawing.Point(14, 95);
      this.RTB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.RTB.Name = "RTB";
      this.RTB.Size = new System.Drawing.Size(937, 343);
      this.RTB.TabIndex = 2;
      this.RTB.Text = "";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(966, 452);
      this.Controls.Add(this.RTB);
      this.Controls.Add(this.btDc2024);
      this.Controls.Add(this.btDc2020);
      this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btDc2020;
    private System.Windows.Forms.Button btDc2024;
    private System.Windows.Forms.RichTextBox RTB;
  }
}

