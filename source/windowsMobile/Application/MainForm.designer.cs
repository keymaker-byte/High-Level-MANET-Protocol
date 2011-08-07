/***************************************************************************
----------------------------------------------------------------------------
  This file is part of the HLMP API - File Sharing Sample Application.
  http://hlmprotocol.bicubic.cl
 
  Copyright (C) Bicubic TMG.  All rights reserved.
 
  This source code is intended only as a supplement to HLMP API 
  and/or on-line documentation.  
 
  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      
  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        
  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       
  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  
  AND PERMISSION FROM BICUBIC TMG.   

  THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
----------------------------------------------------------------------------
****************************************************************************/


namespace HLMP
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.statusControl = new CommControl.StatusControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.netUserControl = new CommControl.NetUserControl();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.fileListControl = new CommControl.FileListControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.fileControl = new CommControl.FileControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.chatControl = new CommControl.ChatControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panelPing = new System.Windows.Forms.Panel();
            this.textBoxPing = new System.Windows.Forms.TextBox();
            this.buttonPing = new System.Windows.Forms.Button();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panelPing.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Exit";
            this.menuItem1.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(240, 268);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.statusControl);
            this.tabPage1.Location = new System.Drawing.Point(0, 0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(240, 245);
            this.tabPage1.Text = "Connection";
            // 
            // statusControl
            // 
            this.statusControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusControl.ImageSize = new System.Drawing.Size(64, 64);
            this.statusControl.Location = new System.Drawing.Point(0, 0);
            this.statusControl.Name = "statusControl";
            this.statusControl.Size = new System.Drawing.Size(240, 245);
            this.statusControl.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.netUserControl);
            this.tabPage2.Location = new System.Drawing.Point(0, 0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(232, 242);
            this.tabPage2.Text = "User List";
            // 
            // netUserControl
            // 
            this.netUserControl.BackColor = System.Drawing.Color.White;
            this.netUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.netUserControl.ImageSize = new System.Drawing.Size(48, 48);
            this.netUserControl.Indent = 5;
            this.netUserControl.Location = new System.Drawing.Point(0, 0);
            this.netUserControl.Name = "netUserControl";
            this.netUserControl.Size = new System.Drawing.Size(232, 242);
            this.netUserControl.TabIndex = 0;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.fileListControl);
            this.tabPage6.Location = new System.Drawing.Point(0, 0);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(240, 245);
            this.tabPage6.Text = "Shared Files";
            // 
            // fileListControl
            // 
            this.fileListControl.BackColor = System.Drawing.Color.White;
            this.fileListControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileListControl.ImageSize = new System.Drawing.Size(48, 48);
            this.fileListControl.Indent = 5;
            this.fileListControl.Location = new System.Drawing.Point(0, 0);
            this.fileListControl.Name = "fileListControl";
            this.fileListControl.Size = new System.Drawing.Size(240, 245);
            this.fileListControl.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.fileControl);
            this.tabPage5.Location = new System.Drawing.Point(0, 0);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(232, 242);
            this.tabPage5.Text = "File Transfer";
            // 
            // fileControl
            // 
            this.fileControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileControl.ImageSize = new System.Drawing.Size(24, 24);
            this.fileControl.Indent = 20;
            this.fileControl.Location = new System.Drawing.Point(0, 0);
            this.fileControl.Name = "fileControl";
            this.fileControl.Size = new System.Drawing.Size(232, 242);
            this.fileControl.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chatControl);
            this.tabPage3.Location = new System.Drawing.Point(0, 0);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(232, 242);
            this.tabPage3.Text = "Chat";
            // 
            // chatControl
            // 
            this.chatControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatControl.Location = new System.Drawing.Point(0, 0);
            this.chatControl.Name = "chatControl";
            this.chatControl.Size = new System.Drawing.Size(232, 242);
            this.chatControl.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.panelPing);
            this.tabPage4.Location = new System.Drawing.Point(0, 0);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(232, 242);
            this.tabPage4.Text = "Ping";
            // 
            // panelPing
            // 
            this.panelPing.Controls.Add(this.textBoxPing);
            this.panelPing.Controls.Add(this.buttonPing);
            this.panelPing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPing.Location = new System.Drawing.Point(0, 0);
            this.panelPing.Name = "panelPing";
            this.panelPing.Size = new System.Drawing.Size(232, 242);
            // 
            // textBoxPing
            // 
            this.textBoxPing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPing.BackColor = System.Drawing.Color.White;
            this.textBoxPing.Location = new System.Drawing.Point(7, 40);
            this.textBoxPing.Multiline = true;
            this.textBoxPing.Name = "textBoxPing";
            this.textBoxPing.ReadOnly = true;
            this.textBoxPing.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxPing.Size = new System.Drawing.Size(218, 191);
            this.textBoxPing.TabIndex = 2;
            // 
            // buttonPing
            // 
            this.buttonPing.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonPing.Location = new System.Drawing.Point(55, 11);
            this.buttonPing.Name = "buttonPing";
            this.buttonPing.Size = new System.Drawing.Size(122, 23);
            this.buttonPing.TabIndex = 0;
            this.buttonPing.Text = "Ping";
            this.buttonPing.Click += new System.EventHandler(this.buttonPing_Click);
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.textBoxLog);
            this.tabPage7.Location = new System.Drawing.Point(0, 0);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(232, 242);
            this.tabPage7.Text = "Log";
            // 
            // textBoxLog
            // 
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLog.Location = new System.Drawing.Point(0, 0);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(232, 242);
            this.textBoxLog.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.tabControl1);
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "HLMP";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.panelPing.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private CommControl.StatusControl statusControl;
        private System.Windows.Forms.TabPage tabPage2;
        private CommControl.NetUserControl netUserControl;
        private System.Windows.Forms.TabPage tabPage6;
        private CommControl.FileListControl fileListControl;
        private System.Windows.Forms.TabPage tabPage5;
        private CommControl.FileControl fileControl;
        private System.Windows.Forms.TabPage tabPage3;
        private CommControl.ChatControl chatControl;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Panel panelPing;
        private System.Windows.Forms.TextBox textBoxPing;
        private System.Windows.Forms.Button buttonPing;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.TextBox textBoxLog;
    }
}

