namespace z_TEST_Target2024
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
      this.btSubSimEvents = new System.Windows.Forms.Button();
      this.btReqCamData = new System.Windows.Forms.Button();
      this.btSendKey = new System.Windows.Forms.Button();
      this.btReqSendData = new System.Windows.Forms.Button();
      this.btSubFrame = new System.Windows.Forms.Button();
      this.btReqData = new System.Windows.Forms.Button();
      this.btRequFacNDB = new System.Windows.Forms.Button();
      this.btRequFacVOR = new System.Windows.Forms.Button();
      this.btRequFacWYP = new System.Windows.Forms.Button();
      this.btReqAptList = new System.Windows.Forms.Button();
      this.btReqInpEvents = new System.Windows.Forms.Button();
      this.btSub1Hz = new System.Windows.Forms.Button();
      this.btSubSome = new System.Windows.Forms.Button();
      this.txFPS = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.txLastReqID = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btClearRTB = new System.Windows.Forms.Button();
      this.btReqSome = new System.Windows.Forms.Button();
      this.RTB = new System.Windows.Forms.RichTextBox();
      this.btConnectSimCon = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btSubSimEvents
      // 
      this.btSubSimEvents.Location = new System.Drawing.Point(436, 12);
      this.btSubSimEvents.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btSubSimEvents.Name = "btSubSimEvents";
      this.btSubSimEvents.Size = new System.Drawing.Size(124, 27);
      this.btSubSimEvents.TabIndex = 64;
      this.btSubSimEvents.Text = "Sub SimEvents";
      this.btSubSimEvents.UseVisualStyleBackColor = true;
      this.btSubSimEvents.Click += new System.EventHandler(this.btSubSimEvents_Click);
      // 
      // btReqCamData
      // 
      this.btReqCamData.Location = new System.Drawing.Point(785, 12);
      this.btReqCamData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btReqCamData.Name = "btReqCamData";
      this.btReqCamData.Size = new System.Drawing.Size(124, 27);
      this.btReqCamData.TabIndex = 63;
      this.btReqCamData.Text = "Subscribe Cam Data";
      this.btReqCamData.UseVisualStyleBackColor = true;
      this.btReqCamData.Click += new System.EventHandler(this.btReqCamData_Click);
      // 
      // btSendKey
      // 
      this.btSendKey.Location = new System.Drawing.Point(785, 110);
      this.btSendKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btSendKey.Name = "btSendKey";
      this.btSendKey.Size = new System.Drawing.Size(124, 27);
      this.btSendKey.TabIndex = 62;
      this.btSendKey.Text = "Send Key (PBrake)";
      this.btSendKey.UseVisualStyleBackColor = true;
      this.btSendKey.Click += new System.EventHandler(this.btSendKey_Click);
      // 
      // btReqSendData
      // 
      this.btReqSendData.Location = new System.Drawing.Point(785, 78);
      this.btReqSendData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btReqSendData.Name = "btReqSendData";
      this.btReqSendData.Size = new System.Drawing.Size(124, 27);
      this.btReqSendData.TabIndex = 61;
      this.btReqSendData.Text = "Send Data";
      this.btReqSendData.UseVisualStyleBackColor = true;
      this.btReqSendData.Click += new System.EventHandler(this.btReqSendData_Click);
      // 
      // btSubFrame
      // 
      this.btSubFrame.Location = new System.Drawing.Point(304, 78);
      this.btSubFrame.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btSubFrame.Name = "btSubFrame";
      this.btSubFrame.Size = new System.Drawing.Size(124, 27);
      this.btSubFrame.TabIndex = 60;
      this.btSubFrame.Text = "Subscribe Frame";
      this.btSubFrame.UseVisualStyleBackColor = true;
      this.btSubFrame.Click += new System.EventHandler(this.btSubFrame_Click);
      // 
      // btReqData
      // 
      this.btReqData.Location = new System.Drawing.Point(785, 45);
      this.btReqData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btReqData.Name = "btReqData";
      this.btReqData.Size = new System.Drawing.Size(124, 27);
      this.btReqData.TabIndex = 59;
      this.btReqData.Text = "Request Data";
      this.btReqData.UseVisualStyleBackColor = true;
      this.btReqData.Click += new System.EventHandler(this.btReqData_Click);
      // 
      // btRequFacNDB
      // 
      this.btRequFacNDB.Location = new System.Drawing.Point(568, 111);
      this.btRequFacNDB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btRequFacNDB.Name = "btRequFacNDB";
      this.btRequFacNDB.Size = new System.Drawing.Size(195, 27);
      this.btRequFacNDB.TabIndex = 58;
      this.btRequFacNDB.Text = "Request Facilities NDB";
      this.btRequFacNDB.UseVisualStyleBackColor = true;
      this.btRequFacNDB.Click += new System.EventHandler(this.btRequFacNDB_Click);
      // 
      // btRequFacVOR
      // 
      this.btRequFacVOR.Location = new System.Drawing.Point(568, 78);
      this.btRequFacVOR.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btRequFacVOR.Name = "btRequFacVOR";
      this.btRequFacVOR.Size = new System.Drawing.Size(195, 27);
      this.btRequFacVOR.TabIndex = 57;
      this.btRequFacVOR.Text = "Request Facilities VOR";
      this.btRequFacVOR.UseVisualStyleBackColor = true;
      this.btRequFacVOR.Click += new System.EventHandler(this.btRequFacVOR_Click);
      // 
      // btRequFacWYP
      // 
      this.btRequFacWYP.Location = new System.Drawing.Point(568, 45);
      this.btRequFacWYP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btRequFacWYP.Name = "btRequFacWYP";
      this.btRequFacWYP.Size = new System.Drawing.Size(195, 27);
      this.btRequFacWYP.TabIndex = 56;
      this.btRequFacWYP.Text = "Request Facilities WYP";
      this.btRequFacWYP.UseVisualStyleBackColor = true;
      this.btRequFacWYP.Click += new System.EventHandler(this.btRequFacWYP_Click);
      // 
      // btReqAptList
      // 
      this.btReqAptList.Location = new System.Drawing.Point(568, 12);
      this.btReqAptList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btReqAptList.Name = "btReqAptList";
      this.btReqAptList.Size = new System.Drawing.Size(195, 27);
      this.btReqAptList.TabIndex = 55;
      this.btReqAptList.Text = "Request Facilities APT (bubble)";
      this.btReqAptList.UseVisualStyleBackColor = true;
      this.btReqAptList.Click += new System.EventHandler(this.btReqAptList_Click);
      // 
      // btReqInpEvents
      // 
      this.btReqInpEvents.Location = new System.Drawing.Point(436, 45);
      this.btReqInpEvents.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btReqInpEvents.Name = "btReqInpEvents";
      this.btReqInpEvents.Size = new System.Drawing.Size(124, 27);
      this.btReqInpEvents.TabIndex = 54;
      this.btReqInpEvents.Text = "Request InpEvents";
      this.btReqInpEvents.UseVisualStyleBackColor = true;
      this.btReqInpEvents.Click += new System.EventHandler(this.btReqInpEvents_Click);
      // 
      // btSub1Hz
      // 
      this.btSub1Hz.Location = new System.Drawing.Point(304, 45);
      this.btSub1Hz.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btSub1Hz.Name = "btSub1Hz";
      this.btSub1Hz.Size = new System.Drawing.Size(124, 27);
      this.btSub1Hz.TabIndex = 53;
      this.btSub1Hz.Text = "Subscribe 1&&4Sec";
      this.btSub1Hz.UseVisualStyleBackColor = true;
      this.btSub1Hz.Click += new System.EventHandler(this.btSub1Hz_Click);
      // 
      // btSubSome
      // 
      this.btSubSome.Location = new System.Drawing.Point(304, 12);
      this.btSubSome.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btSubSome.Name = "btSubSome";
      this.btSubSome.Size = new System.Drawing.Size(124, 27);
      this.btSubSome.TabIndex = 52;
      this.btSubSome.Text = "Subscribe Some";
      this.btSubSome.UseVisualStyleBackColor = true;
      this.btSubSome.Click += new System.EventHandler(this.btSubSome_Click);
      // 
      // txFPS
      // 
      this.txFPS.Location = new System.Drawing.Point(246, 115);
      this.txFPS.Name = "txFPS";
      this.txFPS.Size = new System.Drawing.Size(68, 20);
      this.txFPS.TabIndex = 51;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(214, 118);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(27, 13);
      this.label2.TabIndex = 50;
      this.label2.Text = "FPS";
      // 
      // txLastReqID
      // 
      this.txLastReqID.Location = new System.Drawing.Point(108, 115);
      this.txLastReqID.Name = "txLastReqID";
      this.txLastReqID.Size = new System.Drawing.Size(100, 20);
      this.txLastReqID.TabIndex = 49;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 118);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(87, 13);
      this.label1.TabIndex = 48;
      this.label1.Text = "Last Req ID sent";
      // 
      // btClearRTB
      // 
      this.btClearRTB.BackColor = System.Drawing.Color.Gray;
      this.btClearRTB.ForeColor = System.Drawing.Color.WhiteSmoke;
      this.btClearRTB.Location = new System.Drawing.Point(321, 110);
      this.btClearRTB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btClearRTB.Name = "btClearRTB";
      this.btClearRTB.Size = new System.Drawing.Size(124, 29);
      this.btClearRTB.TabIndex = 47;
      this.btClearRTB.Text = "Clear ScratchPad";
      this.btClearRTB.UseVisualStyleBackColor = false;
      this.btClearRTB.Click += new System.EventHandler(this.btClearRTB_Click);
      // 
      // btReqSome
      // 
      this.btReqSome.Location = new System.Drawing.Point(172, 12);
      this.btReqSome.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btReqSome.Name = "btReqSome";
      this.btReqSome.Size = new System.Drawing.Size(124, 27);
      this.btReqSome.TabIndex = 46;
      this.btReqSome.Text = "Request Some";
      this.btReqSome.UseVisualStyleBackColor = true;
      this.btReqSome.Click += new System.EventHandler(this.btReqSome_Click);
      // 
      // RTB
      // 
      this.RTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.RTB.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RTB.Location = new System.Drawing.Point(13, 145);
      this.RTB.Name = "RTB";
      this.RTB.Size = new System.Drawing.Size(896, 349);
      this.RTB.TabIndex = 45;
      this.RTB.Text = "";
      // 
      // btConnectSimCon
      // 
      this.btConnectSimCon.Location = new System.Drawing.Point(13, 12);
      this.btConnectSimCon.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.btConnectSimCon.Name = "btConnectSimCon";
      this.btConnectSimCon.Size = new System.Drawing.Size(141, 27);
      this.btConnectSimCon.TabIndex = 44;
      this.btConnectSimCon.Text = "Connect SimCon";
      this.btConnectSimCon.UseVisualStyleBackColor = true;
      this.btConnectSimCon.Click += new System.EventHandler(this.btConnectSimCon_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(922, 506);
      this.Controls.Add(this.btSubSimEvents);
      this.Controls.Add(this.btReqCamData);
      this.Controls.Add(this.btSendKey);
      this.Controls.Add(this.btReqSendData);
      this.Controls.Add(this.btSubFrame);
      this.Controls.Add(this.btReqData);
      this.Controls.Add(this.btRequFacNDB);
      this.Controls.Add(this.btRequFacVOR);
      this.Controls.Add(this.btRequFacWYP);
      this.Controls.Add(this.btReqAptList);
      this.Controls.Add(this.btReqInpEvents);
      this.Controls.Add(this.btSub1Hz);
      this.Controls.Add(this.btSubSome);
      this.Controls.Add(this.txFPS);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.txLastReqID);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btClearRTB);
      this.Controls.Add(this.btReqSome);
      this.Controls.Add(this.RTB);
      this.Controls.Add(this.btConnectSimCon);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btSubSimEvents;
    private System.Windows.Forms.Button btReqCamData;
    private System.Windows.Forms.Button btSendKey;
    private System.Windows.Forms.Button btReqSendData;
    private System.Windows.Forms.Button btSubFrame;
    private System.Windows.Forms.Button btReqData;
    private System.Windows.Forms.Button btRequFacNDB;
    private System.Windows.Forms.Button btRequFacVOR;
    private System.Windows.Forms.Button btRequFacWYP;
    private System.Windows.Forms.Button btReqAptList;
    private System.Windows.Forms.Button btReqInpEvents;
    private System.Windows.Forms.Button btSub1Hz;
    private System.Windows.Forms.Button btSubSome;
    private System.Windows.Forms.TextBox txFPS;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txLastReqID;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btClearRTB;
    private System.Windows.Forms.Button btReqSome;
    private System.Windows.Forms.RichTextBox RTB;
    private System.Windows.Forms.Button btConnectSimCon;
  }
}

