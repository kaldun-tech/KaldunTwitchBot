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
			this.labelB = new System.Windows.Forms.Label();
			this.textBoxR = new System.Windows.Forms.TextBox();
			this.textBoxG = new System.Windows.Forms.TextBox();
			this.labelG = new System.Windows.Forms.Label();
			this.labelR = new System.Windows.Forms.Label();
			this.listBoxRaffle = new System.Windows.Forms.ListBox();
			this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
			this.buttonDraw = new System.Windows.Forms.Button();
			this.buttonClear = new System.Windows.Forms.Button();
			this.labelUser = new System.Windows.Forms.Label();
			this.textBoxUser = new System.Windows.Forms.TextBox();
			this.labelPassword = new System.Windows.Forms.Label();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.labelChat = new System.Windows.Forms.Label();
			this.textBoxChat = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonSend = new System.Windows.Forms.Button();
			this.menuStripMain.SuspendLayout();
			this.tableLayoutPanelMain.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonDoWork
			// 
			this.tableLayoutPanelMain.SetColumnSpan(this.buttonDoWork, 2);
			this.buttonDoWork.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonDoWork.Location = new System.Drawing.Point(3, 32);
			this.buttonDoWork.Name = "buttonDoWork";
			this.buttonDoWork.Size = new System.Drawing.Size(284, 23);
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
			this.tableLayoutPanelMain.SetColumnSpan(this.textBoxLog, 2);
			this.textBoxLog.Location = new System.Drawing.Point(3, 61);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ReadOnly = true;
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxLog.Size = new System.Drawing.Size(284, 191);
			this.textBoxLog.TabIndex = 1;
			// 
			// textBoxInput
			// 
			this.textBoxInput.AcceptsTab = true;
			this.textBoxInput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxInput.Location = new System.Drawing.Point(3, 258);
			this.textBoxInput.Name = "textBoxInput";
			this.textBoxInput.Size = new System.Drawing.Size(139, 20);
			this.textBoxInput.TabIndex = 2;
			// 
			// buttonSendRaw
			// 
			this.buttonSendRaw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSendRaw.Location = new System.Drawing.Point(69, 0);
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
			this.logToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.logToolStripMenuItem.Text = "Log";
			this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.newToolStripMenuItem.Text = "New...";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// appendToolStripMenuItem
			// 
			this.appendToolStripMenuItem.Name = "appendToolStripMenuItem";
			this.appendToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.appendToolStripMenuItem.Text = "Append...";
			this.appendToolStripMenuItem.Click += new System.EventHandler(this.appendToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
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
			// 
			// saveFileDialogLog
			// 
			this.saveFileDialogLog.AddExtension = false;
			// 
			// textBoxB
			// 
			this.textBoxB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxB.Location = new System.Drawing.Point(533, 33);
			this.textBoxB.Name = "textBoxB";
			this.textBoxB.Size = new System.Drawing.Size(44, 20);
			this.textBoxB.TabIndex = 5;
			this.textBoxB.Text = "0";
			this.textBoxB.TextChanged += new System.EventHandler(this.textBoxRGB_TextChanged);
			// 
			// labelB
			// 
			this.labelB.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.labelB.AutoSize = true;
			this.labelB.Location = new System.Drawing.Point(513, 37);
			this.labelB.Name = "labelB";
			this.labelB.Size = new System.Drawing.Size(14, 13);
			this.labelB.TabIndex = 6;
			this.labelB.Text = "B";
			// 
			// textBoxR
			// 
			this.textBoxR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxR.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxR.Location = new System.Drawing.Point(341, 33);
			this.textBoxR.Name = "textBoxR";
			this.textBoxR.Size = new System.Drawing.Size(42, 20);
			this.textBoxR.TabIndex = 7;
			this.textBoxR.Text = "0";
			this.textBoxR.TextChanged += new System.EventHandler(this.textBoxRGB_TextChanged);
			// 
			// textBoxG
			// 
			this.textBoxG.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxG.Location = new System.Drawing.Point(437, 33);
			this.textBoxG.Name = "textBoxG";
			this.textBoxG.Size = new System.Drawing.Size(42, 20);
			this.textBoxG.TabIndex = 8;
			this.textBoxG.Text = "255";
			this.textBoxG.TextChanged += new System.EventHandler(this.textBoxRGB_TextChanged);
			// 
			// labelG
			// 
			this.labelG.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.labelG.AutoSize = true;
			this.labelG.Location = new System.Drawing.Point(416, 37);
			this.labelG.Name = "labelG";
			this.labelG.Size = new System.Drawing.Size(15, 13);
			this.labelG.TabIndex = 9;
			this.labelG.Text = "G";
			// 
			// labelR
			// 
			this.labelR.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.labelR.AutoSize = true;
			this.labelR.Location = new System.Drawing.Point(320, 37);
			this.labelR.Name = "labelR";
			this.labelR.Size = new System.Drawing.Size(15, 13);
			this.labelR.TabIndex = 10;
			this.labelR.Text = "R";
			// 
			// listBoxRaffle
			// 
			this.tableLayoutPanelMain.SetColumnSpan(this.listBoxRaffle, 6);
			this.listBoxRaffle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxRaffle.Location = new System.Drawing.Point(293, 61);
			this.listBoxRaffle.Name = "listBoxRaffle";
			this.listBoxRaffle.Size = new System.Drawing.Size(284, 186);
			this.listBoxRaffle.TabIndex = 11;
			// 
			// tableLayoutPanelMain
			// 
			this.tableLayoutPanelMain.ColumnCount = 8;
			this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
			this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
			this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
			this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
			this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
			this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
			this.tableLayoutPanelMain.Controls.Add(this.listBoxRaffle, 2, 2);
			this.tableLayoutPanelMain.Controls.Add(this.textBoxB, 7, 1);
			this.tableLayoutPanelMain.Controls.Add(this.textBoxInput, 0, 3);
			this.tableLayoutPanelMain.Controls.Add(this.textBoxG, 5, 1);
			this.tableLayoutPanelMain.Controls.Add(this.labelG, 4, 1);
			this.tableLayoutPanelMain.Controls.Add(this.labelB, 6, 1);
			this.tableLayoutPanelMain.Controls.Add(this.labelR, 2, 1);
			this.tableLayoutPanelMain.Controls.Add(this.textBoxLog, 0, 2);
			this.tableLayoutPanelMain.Controls.Add(this.textBoxR, 3, 1);
			this.tableLayoutPanelMain.Controls.Add(this.buttonDraw, 2, 3);
			this.tableLayoutPanelMain.Controls.Add(this.buttonClear, 5, 3);
			this.tableLayoutPanelMain.Controls.Add(this.buttonDoWork, 0, 1);
			this.tableLayoutPanelMain.Controls.Add(this.labelUser, 0, 0);
			this.tableLayoutPanelMain.Controls.Add(this.textBoxUser, 1, 0);
			this.tableLayoutPanelMain.Controls.Add(this.labelPassword, 2, 0);
			this.tableLayoutPanelMain.Controls.Add(this.textBoxPassword, 4, 0);
			this.tableLayoutPanelMain.Controls.Add(this.labelChat, 5, 0);
			this.tableLayoutPanelMain.Controls.Add(this.textBoxChat, 6, 0);
			this.tableLayoutPanelMain.Controls.Add(this.panel1, 1, 3);
			this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 24);
			this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
			this.tableLayoutPanelMain.RowCount = 4;
			this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			this.tableLayoutPanelMain.Size = new System.Drawing.Size(580, 284);
			this.tableLayoutPanelMain.TabIndex = 12;
			// 
			// buttonDraw
			// 
			this.tableLayoutPanelMain.SetColumnSpan(this.buttonDraw, 3);
			this.buttonDraw.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonDraw.Location = new System.Drawing.Point(293, 258);
			this.buttonDraw.Name = "buttonDraw";
			this.buttonDraw.Size = new System.Drawing.Size(138, 23);
			this.buttonDraw.TabIndex = 13;
			this.buttonDraw.Text = "Draw";
			this.buttonDraw.UseVisualStyleBackColor = true;
			this.buttonDraw.Click += new System.EventHandler(this.buttonDraw_Click);
			// 
			// buttonClear
			// 
			this.tableLayoutPanelMain.SetColumnSpan(this.buttonClear, 3);
			this.buttonClear.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonClear.Location = new System.Drawing.Point(437, 258);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new System.Drawing.Size(140, 23);
			this.buttonClear.TabIndex = 14;
			this.buttonClear.Text = "Clear";
			this.buttonClear.UseVisualStyleBackColor = true;
			this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
			// 
			// labelUser
			// 
			this.labelUser.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.labelUser.AutoSize = true;
			this.labelUser.Location = new System.Drawing.Point(113, 8);
			this.labelUser.Name = "labelUser";
			this.labelUser.Size = new System.Drawing.Size(29, 13);
			this.labelUser.TabIndex = 15;
			this.labelUser.Text = "User";
			// 
			// textBoxUser
			// 
			this.textBoxUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxUser.Location = new System.Drawing.Point(148, 4);
			this.textBoxUser.Name = "textBoxUser";
			this.textBoxUser.Size = new System.Drawing.Size(139, 20);
			this.textBoxUser.TabIndex = 16;
			// 
			// labelPassword
			// 
			this.labelPassword.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.labelPassword.AutoSize = true;
			this.tableLayoutPanelMain.SetColumnSpan(this.labelPassword, 2);
			this.labelPassword.Location = new System.Drawing.Point(295, 1);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(88, 26);
			this.labelPassword.TabIndex = 17;
			this.labelPassword.Text = "OAuth (including \"oauth:\")";
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxPassword.Location = new System.Drawing.Point(389, 4);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.Size = new System.Drawing.Size(42, 20);
			this.textBoxPassword.TabIndex = 18;
			this.textBoxPassword.UseSystemPasswordChar = true;
			// 
			// labelChat
			// 
			this.labelChat.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.labelChat.AutoSize = true;
			this.labelChat.Location = new System.Drawing.Point(450, 8);
			this.labelChat.Name = "labelChat";
			this.labelChat.Size = new System.Drawing.Size(29, 13);
			this.labelChat.TabIndex = 19;
			this.labelChat.Text = "Chat";
			// 
			// textBoxChat
			// 
			this.tableLayoutPanelMain.SetColumnSpan(this.textBoxChat, 2);
			this.textBoxChat.Location = new System.Drawing.Point(485, 3);
			this.textBoxChat.Name = "textBoxChat";
			this.textBoxChat.Size = new System.Drawing.Size(83, 20);
			this.textBoxChat.TabIndex = 20;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonSend);
			this.panel1.Controls.Add(this.buttonSendRaw);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(148, 258);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(139, 23);
			this.panel1.TabIndex = 12;
			// 
			// buttonSend
			// 
			this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSend.Location = new System.Drawing.Point(3, 0);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(60, 23);
			this.buttonSend.TabIndex = 4;
			this.buttonSend.Text = "Send";
			this.buttonSend.UseVisualStyleBackColor = true;
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// TwitchBot
			// 
			this.AcceptButton = this.buttonSend;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(580, 308);
			this.Controls.Add(this.tableLayoutPanelMain);
			this.Controls.Add(this.menuStripMain);
			this.MainMenuStrip = this.menuStripMain;
			this.Name = "TwitchBot";
			this.Text = "TwitchBot";
			this.TransparencyKey = System.Drawing.Color.PaleTurquoise;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TwitchBot_FormClosed);
			this.menuStripMain.ResumeLayout(false);
			this.menuStripMain.PerformLayout();
			this.tableLayoutPanelMain.ResumeLayout(false);
			this.tableLayoutPanelMain.PerformLayout();
			this.panel1.ResumeLayout(false);
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
		private System.Windows.Forms.Label labelB;
		private System.Windows.Forms.TextBox textBoxR;
		private System.Windows.Forms.TextBox textBoxG;
		private System.Windows.Forms.Label labelG;
		private System.Windows.Forms.Label labelR;
		private System.Windows.Forms.ListBox listBoxRaffle;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
		private System.Windows.Forms.Panel panel1;
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
	}
}

