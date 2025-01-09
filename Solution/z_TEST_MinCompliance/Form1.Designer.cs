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
      this.btSubSome = new System.Windows.Forms.Button();
      this.btSub1Hz = new System.Windows.Forms.Button();
      this.btReqInpEvents = new System.Windows.Forms.Button();
      this.btReqAptList = new System.Windows.Forms.Button();
      this.btRequFacWYP = new System.Windows.Forms.Button();
      this.btRequFacVOR = new System.Windows.Forms.Button();
      this.btRequFacNDB = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.txLastReqID = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // btConnect
      // 
      this.btConnect.Location = new System.Drawing.Point(12, 61);
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
      this.RTB.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RTB.Location = new System.Drawing.Point(12, 157);
      this.RTB.Name = "RTB";
      this.RTB.Size = new System.Drawing.Size(909, 350);
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
      this.btReqSome.Location = new System.Drawing.Point(144, 12);
      this.btReqSome.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btReqSome.Name = "btReqSome";
      this.btReqSome.Size = new System.Drawing.Size(124, 43);
      this.btReqSome.TabIndex = 3;
      this.btReqSome.Text = "Request Some";
      this.btReqSome.UseVisualStyleBackColor = true;
      this.btReqSome.Click += new System.EventHandler(this.btReqSome_Click);
      // 
      // btSubSome
      // 
      this.btSubSome.Location = new System.Drawing.Point(276, 12);
      this.btSubSome.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btSubSome.Name = "btSubSome";
      this.btSubSome.Size = new System.Drawing.Size(124, 43);
      this.btSubSome.TabIndex = 4;
      this.btSubSome.Text = "Subscribe Some";
      this.btSubSome.UseVisualStyleBackColor = true;
      this.btSubSome.Click += new System.EventHandler(this.btSubSome_Click);
      // 
      // btSub1Hz
      // 
      this.btSub1Hz.Location = new System.Drawing.Point(276, 61);
      this.btSub1Hz.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btSub1Hz.Name = "btSub1Hz";
      this.btSub1Hz.Size = new System.Drawing.Size(124, 43);
      this.btSub1Hz.TabIndex = 5;
      this.btSub1Hz.Text = "Subscribe 1&&4Sec";
      this.btSub1Hz.UseVisualStyleBackColor = true;
      this.btSub1Hz.Click += new System.EventHandler(this.btSub1Hz_Click);
      // 
      // btReqInpEvents
      // 
      this.btReqInpEvents.Location = new System.Drawing.Point(408, 12);
      this.btReqInpEvents.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btReqInpEvents.Name = "btReqInpEvents";
      this.btReqInpEvents.Size = new System.Drawing.Size(124, 43);
      this.btReqInpEvents.TabIndex = 6;
      this.btReqInpEvents.Text = "Request InpEvents";
      this.btReqInpEvents.UseVisualStyleBackColor = true;
      this.btReqInpEvents.Click += new System.EventHandler(this.btReqInpEvents_Click);
      // 
      // btReqAptList
      // 
      this.btReqAptList.Location = new System.Drawing.Point(540, 12);
      this.btReqAptList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btReqAptList.Name = "btReqAptList";
      this.btReqAptList.Size = new System.Drawing.Size(195, 27);
      this.btReqAptList.TabIndex = 7;
      this.btReqAptList.Text = "Request Facilities APT (bubble)";
      this.btReqAptList.UseVisualStyleBackColor = true;
      this.btReqAptList.Click += new System.EventHandler(this.btReqAptList_Click);
      // 
      // btRequFacWYP
      // 
      this.btRequFacWYP.Location = new System.Drawing.Point(540, 45);
      this.btRequFacWYP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btRequFacWYP.Name = "btRequFacWYP";
      this.btRequFacWYP.Size = new System.Drawing.Size(195, 27);
      this.btRequFacWYP.TabIndex = 8;
      this.btRequFacWYP.Text = "Request Facilities WYP";
      this.btRequFacWYP.UseVisualStyleBackColor = true;
      this.btRequFacWYP.Click += new System.EventHandler(this.btRequFacWYP_Click);
      // 
      // btRequFacVOR
      // 
      this.btRequFacVOR.Location = new System.Drawing.Point(540, 78);
      this.btRequFacVOR.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btRequFacVOR.Name = "btRequFacVOR";
      this.btRequFacVOR.Size = new System.Drawing.Size(195, 27);
      this.btRequFacVOR.TabIndex = 9;
      this.btRequFacVOR.Text = "Request Facilities VOR";
      this.btRequFacVOR.UseVisualStyleBackColor = true;
      this.btRequFacVOR.Click += new System.EventHandler(this.btRequFacVOR_Click);
      // 
      // btRequFacNDB
      // 
      this.btRequFacNDB.Location = new System.Drawing.Point(540, 111);
      this.btRequFacNDB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btRequFacNDB.Name = "btRequFacNDB";
      this.btRequFacNDB.Size = new System.Drawing.Size(195, 27);
      this.btRequFacNDB.TabIndex = 10;
      this.btRequFacNDB.Text = "Request Facilities NDB";
      this.btRequFacNDB.UseVisualStyleBackColor = true;
      this.btRequFacNDB.Click += new System.EventHandler(this.btRequFacNDB_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 131);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(90, 15);
      this.label1.TabIndex = 11;
      this.label1.Text = "Last Req ID sent";
      // 
      // txLastReqID
      // 
      this.txLastReqID.Location = new System.Drawing.Point(108, 128);
      this.txLastReqID.Name = "txLastReqID";
      this.txLastReqID.Size = new System.Drawing.Size(100, 23);
      this.txLastReqID.TabIndex = 12;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(933, 519);
      this.Controls.Add(this.txLastReqID);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btRequFacNDB);
      this.Controls.Add(this.btRequFacVOR);
      this.Controls.Add(this.btRequFacWYP);
      this.Controls.Add(this.btReqAptList);
      this.Controls.Add(this.btReqInpEvents);
      this.Controls.Add(this.btSub1Hz);
      this.Controls.Add(this.btSubSome);
      this.Controls.Add(this.btReqSome);
      this.Controls.Add(this.btConDiscon);
      this.Controls.Add(this.RTB);
      this.Controls.Add(this.btConnect);
      this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btConnect;
    private System.Windows.Forms.RichTextBox RTB;
    private System.Windows.Forms.Button btConDiscon;
    private System.Windows.Forms.Button btReqSome;
    private System.Windows.Forms.Button btSubSome;
    private System.Windows.Forms.Button btSub1Hz;
    private System.Windows.Forms.Button btReqInpEvents;
    private System.Windows.Forms.Button btReqAptList;
    private System.Windows.Forms.Button btRequFacWYP;
    private System.Windows.Forms.Button btRequFacVOR;
    private System.Windows.Forms.Button btRequFacNDB;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txLastReqID;
  }
}

