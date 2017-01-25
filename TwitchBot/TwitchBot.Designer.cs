namespace TwitchBot
{
	partial class TwitchBot
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonDoWork = new System.Windows.Forms.Button();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.textBoxInput = new System.Windows.Forms.TextBox();
			this.buttonSendRaw = new System.Windows.Forms.Button();
			this.menuStripMain = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.appendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.imageWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.scrollToNewMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialogLog = new System.Windows.Forms.SaveFileDialog();
			this.textBoxB = new System.Windows.Forms.TextBox();
			this.labelBlue = new System.Windows.Forms.Label();
			this.textBoxR = new System.Windows.Forms.TextBox();
			this.textBoxG = new System.Windows.Forms.TextBox();
			this.labelGreen = new System.Windows.Forms.Label();
			this.labelRed = new System.Windows.Forms.Label();
			this.listBoxRaffle = new System.Windows.Forms.ListBox();
			this.buttonDraw = new System.Windows.Forms.Button();
			this.buttonClear = new System.Windows.Forms.Button();
			this.labelUser = new System.Windows.Forms.Label();
			this.textBoxUser = new System.Windows.Forms.TextBox();
			this.labelPassword = new System.Windows.Forms.Label();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.labelChat = new System.Windows.Forms.Label();
			this.textBoxChat = new System.Windows.Forms.TextBox();
			this.buttonSend = new System.Windows.Forms.Button();
			this.tabControlConnected = new System.Windows.Forms.TabControl();
			this.tabPageRaffle = new System.Windows.Forms.TabPage();
			this.tabPageTraffic = new System.Windows.Forms.TabPage();
			this.tabPageEffects = new System.Windows.Forms.TabPage();
			this.tabPageConnect = new System.Windows.Forms.TabPage();
			this.menuStripMain.SuspendLayout();
			this.tabControlConnected.SuspendLayout();
			this.tabPageRaffle.SuspendLayout();
			this.tabPageTraffic.SuspendLayout();
			this.tabPageEffects.SuspendLayout();
			this.tabPageConnect.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonDoWork
			// 
			this.buttonDoWork.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDoWork.Location = new System.Drawing.Point(8, 95);
			this.buttonDoWork.Name = "buttonDoWork";
			this.buttonDoWork.Size = new System.Drawing.Size(556, 23);
			this.buttonDoWork.TabIndex = 0;
			this.buttonDoWork.Text = "Connect";
			this.buttonDoWork.UseVisualStyleBackColor = true;
			this.buttonDoWork.Click += new System.EventHandler(this.buttonDoWork_Click);
			// 
			// textBoxLog
			// 
			this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLog.Location = new System.Drawing.Point(6, 6);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ReadOnly = true;
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxLog.Size = new System.Drawing.Size(560, 59);
			this.textBoxLog.TabIndex = 1;
			// 
			// textBoxInput
			// 
			this.textBoxInput.AcceptsTab = true;
			this.textBoxInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxInput.Location = new System.Drawing.Point(6, 71);
			this.textBoxInput.Name = "textBoxInput";
			this.textBoxInput.Size = new System.Drawing.Size(560, 20);
			this.textBoxInput.TabIndex = 2;
			// 
			// buttonSendRaw
			// 
			this.buttonSendRaw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSendRaw.Location = new System.Drawing.Point(499, 97);
			this.buttonSendRaw.Name = "buttonSendRaw";
			this.buttonSendRaw.Size = new System.Drawing.Size(67, 23);
			this.buttonSendRaw.TabIndex = 3;
			this.buttonSendRaw.Text = "Send Raw";
			this.buttonSendRaw.UseVisualStyleBackColor = true;
			this.buttonSendRaw.Click += new System.EventHandler(this.buttonSendRaw_Click);
			// 
			// menuStripMain
			// 
			this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
			this.menuStripMain.Location = new System.Drawing.Point(0, 0);
			this.menuStripMain.Name = "menuStripMain";
			this.menuStripMain.Size = new System.Drawing.Size(580, 24);
			this.menuStripMain.TabIndex = 4;
			this.menuStripMain.Text = "menuStripMain";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// logToolStripMenuItem
			// 
			this.logToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.appendToolStripMenuItem});
			this.logToolStripMenuItem.Name = "logToolStripMenuItem";
			this.logToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
			this.logToolStripMenuItem.Text = "Log";
			this.logToolStripMenuItem.Visible = false;
			this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.newToolStripMenuItem.Text = "New...";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// appendToolStripMenuItem
			// 
			this.appendToolStripMenuItem.Name = "appendToolStripMenuItem";
			this.appendToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.appendToolStripMenuItem.Text = "Append...";
			this.appendToolStripMenuItem.Click += new System.EventHandler(this.appendToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "Edit";
			this.editToolStripMenuItem.Visible = false;
			// 
			// clearToolStripMenuItem
			// 
			this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
			this.clearToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
			this.clearToolStripMenuItem.Text = "Clear";
			this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageWindowToolStripMenuItem,
            this.scrollToNewMessageToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// imageWindowToolStripMenuItem
			// 
			this.imageWindowToolStripMenuItem.Name = "imageWindowToolStripMenuItem";
			this.imageWindowToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.imageWindowToolStripMenuItem.Text = "Image Window";
			this.imageWindowToolStripMenuItem.Click += new System.EventHandler(this.imageWindowToolStripMenuItem_Click);
			// 
			// scrollToNewMessageToolStripMenuItem
			// 
			this.scrollToNewMessageToolStripMenuItem.Checked = true;
			this.scrollToNewMessageToolStripMenuItem.CheckOnClick = true;
			this.scrollToNewMessageToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.scrollToNewMessageToolStripMenuItem.Name = "scrollToNewMessageToolStripMenuItem";
			this.scrollToNewMessageToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.scrollToNewMessageToolStripMenuItem.Text = "Scroll to New Message";
			this.scrollToNewMessageToolStripMenuItem.Visible = false;
			// 
			// saveFileDialogLog
			// 
			this.saveFileDialogLog.AddExtension = false;
			// 
			// textBoxB
			// 
			this.textBoxB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxB.Location = new System.Drawing.Point(46, 63);
			this.textBoxB.Name = "textBoxB";
			this.textBoxB.Size = new System.Drawing.Size(499, 20);
			this.textBoxB.TabIndex = 5;
			this.textBoxB.Text = "0";
			this.textBoxB.TextChanged += new System.EventHandler(this.textBoxRGB_TextChanged);
			// 
			// labelBlue
			// 
			this.labelBlue.AutoSize = true;
			this.labelBlue.Location = new System.Drawing.Point(4, 40);
			this.labelBlue.Name = "labelBlue";
			this.labelBlue.Size = new System.Drawing.Size(28, 13);
			this.labelBlue.TabIndex = 6;
			this.labelBlue.Text = "Blue";
			// 
			// textBoxR
			// 
			this.textBoxR.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxR.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxR.Location = new System.Drawing.Point(46, 11);
			this.textBoxR.Name = "textBoxR";
			this.textBoxR.Size = new System.Drawing.Size(499, 20);
			this.textBoxR.TabIndex = 7;
			this.textBoxR.Text = "0";
			this.textBoxR.TextChanged += new System.EventHandler(this.textBoxRGB_TextChanged);
			// 
			// textBoxG
			// 
			this.textBoxG.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxG.Location = new System.Drawing.Point(46, 37);
			this.textBoxG.Name = "textBoxG";
			this.textBoxG.Size = new System.Drawing.Size(499, 20);
			this.textBoxG.TabIndex = 8;
			this.textBoxG.Text = "255";
			this.textBoxG.TextChanged += new System.EventHandler(this.textBoxRGB_TextChanged);
			// 
			// labelGreen
			// 
			this.labelGreen.AutoSize = true;
			this.labelGreen.Location = new System.Drawing.Point(4, 66);
			this.labelGreen.Name = "labelGreen";
			this.labelGreen.Size = new System.Drawing.Size(36, 13);
			this.labelGreen.TabIndex = 9;
			this.labelGreen.Text = "Green";
			// 
			// labelRed
			// 
			this.labelRed.AutoSize = true;
			this.labelRed.Location = new System.Drawing.Point(3, 14);
			this.labelRed.Name = "labelRed";
			this.labelRed.Size = new System.Drawing.Size(27, 13);
			this.labelRed.TabIndex = 10;
			this.labelRed.Text = "Red";
			// 
			// listBoxRaffle
			// 
			this.listBoxRaffle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxRaffle.Location = new System.Drawing.Point(6, 6);
			this.listBoxRaffle.Name = "listBoxRaffle";
			this.listBoxRaffle.Size = new System.Drawing.Size(560, 56);
			this.listBoxRaffle.TabIndex = 11;
			// 
			// buttonDraw
			// 
			this.buttonDraw.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDraw.Location = new System.Drawing.Point(6, 97);
			this.buttonDraw.Name = "buttonDraw";
			this.buttonDraw.Size = new System.Drawing.Size(498, 23);
			this.buttonDraw.TabIndex = 13;
			this.buttonDraw.Text = "Draw";
			this.buttonDraw.UseVisualStyleBackColor = true;
			this.buttonDraw.Click += new System.EventHandler(this.buttonDraw_Click);
			// 
			// buttonClear
			// 
			this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClear.Location = new System.Drawing.Point(510, 97);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new System.Drawing.Size(56, 23);
			this.buttonClear.TabIndex = 14;
			this.buttonClear.Text = "Clear";
			this.buttonClear.UseVisualStyleBackColor = true;
			this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
			// 
			// labelUser
			// 
			this.labelUser.AutoSize = true;
			this.labelUser.Location = new System.Drawing.Point(3, 6);
			this.labelUser.Name = "labelUser";
			this.labelUser.Size = new System.Drawing.Size(29, 13);
			this.labelUser.TabIndex = 15;
			this.labelUser.Text = "User";
			// 
			// textBoxUser
			// 
			this.textBoxUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxUser.Location = new System.Drawing.Point(140, 3);
			this.textBoxUser.Name = "textBoxUser";
			this.textBoxUser.Size = new System.Drawing.Size(424, 20);
			this.textBoxUser.TabIndex = 16;
			// 
			// labelPassword
			// 
			this.labelPassword.AutoSize = true;
			this.labelPassword.Location = new System.Drawing.Point(3, 32);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(131, 13);
			this.labelPassword.TabIndex = 17;
			this.labelPassword.Text = "OAuth (including \"oauth:\")";
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxPassword.Location = new System.Drawing.Point(140, 29);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.Size = new System.Drawing.Size(424, 20);
			this.textBoxPassword.TabIndex = 18;
			this.textBoxPassword.UseSystemPasswordChar = true;
			// 
			// labelChat
			// 
			this.labelChat.AutoSize = true;
			this.labelChat.Location = new System.Drawing.Point(8, 58);
			this.labelChat.Name = "labelChat";
			this.labelChat.Size = new System.Drawing.Size(29, 13);
			this.labelChat.TabIndex = 19;
			this.labelChat.Text = "Chat";
			// 
			// textBoxChat
			// 
			this.textBoxChat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxChat.Location = new System.Drawing.Point(140, 55);
			this.textBoxChat.Name = "textBoxChat";
			this.textBoxChat.Size = new System.Drawing.Size(424, 20);
			this.textBoxChat.TabIndex = 20;
			// 
			// buttonSend
			// 
			this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSend.Location = new System.Drawing.Point(6, 97);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(487, 23);
			this.buttonSend.TabIndex = 4;
			this.buttonSend.Text = "Send";
			this.buttonSend.UseVisualStyleBackColor = true;
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// tabControlConnected
			// 
			this.tabControlConnected.Controls.Add(this.tabPageConnect);
			this.tabControlConnected.Controls.Add(this.tabPageRaffle);
			this.tabControlConnected.Controls.Add(this.tabPageTraffic);
			this.tabControlConnected.Controls.Add(this.tabPageEffects);
			this.tabControlConnected.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlConnected.Location = new System.Drawing.Point(0, 24);
			this.tabControlConnected.Name = "tabControlConnected";
			this.tabControlConnected.SelectedIndex = 0;
			this.tabControlConnected.Size = new System.Drawing.Size(580, 284);
			this.tabControlConnected.TabIndex = 13;
			this.tabControlConnected.SelectedIndexChanged += new System.EventHandler(this.tabControlConnected_SelectedIndexChanged);
			// 
			// tabPageRaffle
			// 
			this.tabPageRaffle.Controls.Add(this.listBoxRaffle);
			this.tabPageRaffle.Controls.Add(this.buttonDraw);
			this.tabPageRaffle.Controls.Add(this.buttonClear);
			this.tabPageRaffle.Location = new System.Drawing.Point(4, 22);
			this.tabPageRaffle.Name = "tabPageRaffle";
			this.tabPageRaffle.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageRaffle.Size = new System.Drawing.Size(572, 126);
			this.tabPageRaffle.TabIndex = 0;
			this.tabPageRaffle.Text = "Raffle";
			this.tabPageRaffle.UseVisualStyleBackColor = true;
			// 
			// tabPageTraffic
			// 
			this.tabPageTraffic.Controls.Add(this.buttonSend);
			this.tabPageTraffic.Controls.Add(this.textBoxInput);
			this.tabPageTraffic.Controls.Add(this.textBoxLog);
			this.tabPageTraffic.Controls.Add(this.buttonSendRaw);
			this.tabPageTraffic.Location = new System.Drawing.Point(4, 22);
			this.tabPageTraffic.Name = "tabPageTraffic";
			this.tabPageTraffic.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageTraffic.Size = new System.Drawing.Size(572, 126);
			this.tabPageTraffic.TabIndex = 1;
			this.tabPageTraffic.Text = "Traffic";
			this.tabPageTraffic.UseVisualStyleBackColor = true;
			// 
			// tabPageEffects
			// 
			this.tabPageEffects.Controls.Add(this.labelBlue);
			this.tabPageEffects.Controls.Add(this.textBoxB);
			this.tabPageEffects.Controls.Add(this.labelRed);
			this.tabPageEffects.Controls.Add(this.textBoxG);
			this.tabPageEffects.Controls.Add(this.textBoxR);
			this.tabPageEffects.Controls.Add(this.labelGreen);
			this.tabPageEffects.Location = new System.Drawing.Point(4, 22);
			this.tabPageEffects.Name = "tabPageEffects";
			this.tabPageEffects.Size = new System.Drawing.Size(572, 126);
			this.tabPageEffects.TabIndex = 2;
			this.tabPageEffects.Text = "Effects";
			this.tabPageEffects.UseVisualStyleBackColor = true;
			// 
			// tabPageConnect
			// 
			this.tabPageConnect.Controls.Add(this.labelUser);
			this.tabPageConnect.Controls.Add(this.textBoxUser);
			this.tabPageConnect.Controls.Add(this.labelChat);
			this.tabPageConnect.Controls.Add(this.labelPassword);
			this.tabPageConnect.Controls.Add(this.buttonDoWork);
			this.tabPageConnect.Controls.Add(this.textBoxChat);
			this.tabPageConnect.Controls.Add(this.textBoxPassword);
			this.tabPageConnect.Location = new System.Drawing.Point(4, 22);
			this.tabPageConnect.Name = "tabPageConnect";
			this.tabPageConnect.Size = new System.Drawing.Size(572, 258);
			this.tabPageConnect.TabIndex = 3;
			this.tabPageConnect.Text = "Connect";
			this.tabPageConnect.UseVisualStyleBackColor = true;
			// 
			// TwitchBot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(580, 308);
			this.Controls.Add(this.tabControlConnected);
			this.Controls.Add(this.menuStripMain);
			this.MainMenuStrip = this.menuStripMain;
			this.Name = "TwitchBot";
			this.Text = "TwitchBot";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TwitchBot_FormClosed);
			this.menuStripMain.ResumeLayout(false);
			this.menuStripMain.PerformLayout();
			this.tabControlConnected.ResumeLayout(false);
			this.tabPageRaffle.ResumeLayout(false);
			this.tabPageTraffic.ResumeLayout(false);
			this.tabPageTraffic.PerformLayout();
			this.tabPageEffects.ResumeLayout(false);
			this.tabPageEffects.PerformLayout();
			this.tabPageConnect.ResumeLayout(false);
			this.tabPageConnect.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonDoWork;
		private System.Windows.Forms.TextBox textBoxLog;
		private System.Windows.Forms.TextBox textBoxInput;
		private System.Windows.Forms.Button buttonSendRaw;
		private System.Windows.Forms.MenuStrip menuStripMain;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog saveFileDialogLog;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem imageWindowToolStripMenuItem;
		private System.Windows.Forms.TextBox textBoxB;
		private System.Windows.Forms.Label labelBlue;
		private System.Windows.Forms.TextBox textBoxR;
		private System.Windows.Forms.TextBox textBoxG;
		private System.Windows.Forms.Label labelGreen;
		private System.Windows.Forms.Label labelRed;
		private System.Windows.Forms.ListBox listBoxRaffle;
		private System.Windows.Forms.Button buttonDraw;
		private System.Windows.Forms.Button buttonClear;
		private System.Windows.Forms.ToolStripMenuItem scrollToNewMessageToolStripMenuItem;
		private System.Windows.Forms.Button buttonSend;
		private System.Windows.Forms.Label labelUser;
		private System.Windows.Forms.TextBox textBoxUser;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.Label labelChat;
		private System.Windows.Forms.TextBox textBoxChat;
		private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem appendToolStripMenuItem;
		private System.Windows.Forms.TabControl tabControlConnected;
		private System.Windows.Forms.TabPage tabPageRaffle;
		private System.Windows.Forms.TabPage tabPageTraffic;
		private System.Windows.Forms.TabPage tabPageEffects;
		private System.Windows.Forms.TabPage tabPageConnect;
	}
}

