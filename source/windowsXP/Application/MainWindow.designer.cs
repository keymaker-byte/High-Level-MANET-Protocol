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
    partial class MainWindow
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método conectar el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.chatControl = new CommControl.ChatControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.netUserControl = new CommControl.NetUserControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.statusControl = new CommControl.StatusControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.groupBoxMyFiles = new System.Windows.Forms.GroupBox();
            this.buttonAddFile = new System.Windows.Forms.Button();
            this.treeViewMyFiles = new System.Windows.Forms.TreeView();
            this.iconList = new System.Windows.Forms.ImageList(this.components);
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.fileListControl = new CommControl.FileListControl();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.fileControl = new CommControl.FileControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.networkGraphControl = new CommControl.NetworkGraphControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.textBoxPing = new System.Windows.Forms.TextBox();
            this.buttonPing = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.groupBoxMyFiles.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.chatControl);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(662, 484);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Chat";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // chatControl
            // 
            this.chatControl.ChatProtocol = null;
            this.chatControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatControl.Location = new System.Drawing.Point(3, 3);
            this.chatControl.Name = "chatControl";
            this.chatControl.NetUser = null;
            this.chatControl.Size = new System.Drawing.Size(656, 478);
            this.chatControl.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.netUserControl);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(662, 484);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "User List";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // netUserControl
            // 
            this.netUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.netUserControl.ImageSize = new System.Drawing.Size(32, 32);
            this.netUserControl.Location = new System.Drawing.Point(3, 3);
            this.netUserControl.Name = "netUserControl";
            this.netUserControl.Size = new System.Drawing.Size(656, 478);
            this.netUserControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.statusControl);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(662, 484);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Connection";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // statusControl
            // 
            this.statusControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusControl.Location = new System.Drawing.Point(3, 3);
            this.statusControl.Name = "statusControl";
            this.statusControl.Size = new System.Drawing.Size(656, 478);
            this.statusControl.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage8);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(670, 510);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.groupBoxMyFiles);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(662, 484);
            this.tabPage8.TabIndex = 10;
            this.tabPage8.Text = "My Shared Files";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // groupBoxMyFiles
            // 
            this.groupBoxMyFiles.Controls.Add(this.buttonAddFile);
            this.groupBoxMyFiles.Controls.Add(this.treeViewMyFiles);
            this.groupBoxMyFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxMyFiles.Location = new System.Drawing.Point(3, 3);
            this.groupBoxMyFiles.Name = "groupBoxMyFiles";
            this.groupBoxMyFiles.Size = new System.Drawing.Size(656, 478);
            this.groupBoxMyFiles.TabIndex = 0;
            this.groupBoxMyFiles.TabStop = false;
            this.groupBoxMyFiles.Text = "My Files";
            // 
            // buttonAddFile
            // 
            this.buttonAddFile.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonAddFile.Location = new System.Drawing.Point(291, 448);
            this.buttonAddFile.Name = "buttonAddFile";
            this.buttonAddFile.Size = new System.Drawing.Size(75, 23);
            this.buttonAddFile.TabIndex = 1;
            this.buttonAddFile.Text = "Add File";
            this.buttonAddFile.UseVisualStyleBackColor = true;
            this.buttonAddFile.Click += new System.EventHandler(this.buttonAddFile_Click);
            // 
            // treeViewMyFiles
            // 
            this.treeViewMyFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewMyFiles.ImageIndex = 0;
            this.treeViewMyFiles.ImageList = this.iconList;
            this.treeViewMyFiles.Location = new System.Drawing.Point(6, 19);
            this.treeViewMyFiles.Name = "treeViewMyFiles";
            this.treeViewMyFiles.SelectedImageIndex = 0;
            this.treeViewMyFiles.Size = new System.Drawing.Size(644, 421);
            this.treeViewMyFiles.TabIndex = 0;
            // 
            // iconList
            // 
            this.iconList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("iconList.ImageStream")));
            this.iconList.TransparentColor = System.Drawing.Color.Transparent;
            this.iconList.Images.SetKeyName(0, "file_icon.gif");
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.fileListControl);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(662, 484);
            this.tabPage6.TabIndex = 6;
            this.tabPage6.Text = "Community Files";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // fileListControl
            // 
            this.fileListControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileListControl.FileTransferProtocol = null;
            this.fileListControl.Location = new System.Drawing.Point(3, 3);
            this.fileListControl.Name = "fileListControl";
            this.fileListControl.Size = new System.Drawing.Size(656, 478);
            this.fileListControl.TabIndex = 0;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.fileControl);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(662, 484);
            this.tabPage7.TabIndex = 7;
            this.tabPage7.Text = "File Transfer List";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // fileControl
            // 
            this.fileControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileControl.Location = new System.Drawing.Point(3, 3);
            this.fileControl.Name = "fileControl";
            this.fileControl.Size = new System.Drawing.Size(656, 478);
            this.fileControl.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.networkGraphControl);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(662, 484);
            this.tabPage2.TabIndex = 8;
            this.tabPage2.Text = "MANET Graph";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // networkGraphControl
            // 
            this.networkGraphControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.networkGraphControl.Location = new System.Drawing.Point(3, 3);
            this.networkGraphControl.Name = "networkGraphControl";
            this.networkGraphControl.Size = new System.Drawing.Size(656, 478);
            this.networkGraphControl.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.textBoxPing);
            this.tabPage5.Controls.Add(this.buttonPing);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(662, 484);
            this.tabPage5.TabIndex = 9;
            this.tabPage5.Text = "Ping";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // textBoxPing
            // 
            this.textBoxPing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPing.Location = new System.Drawing.Point(8, 35);
            this.textBoxPing.Multiline = true;
            this.textBoxPing.Name = "textBoxPing";
            this.textBoxPing.Size = new System.Drawing.Size(699, 388);
            this.textBoxPing.TabIndex = 1;
            // 
            // buttonPing
            // 
            this.buttonPing.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonPing.Location = new System.Drawing.Point(284, 6);
            this.buttonPing.Name = "buttonPing";
            this.buttonPing.Size = new System.Drawing.Size(147, 23);
            this.buttonPing.TabIndex = 0;
            this.buttonPing.Text = "Ping a Todos los Usuarios ";
            this.buttonPing.UseVisualStyleBackColor = true;
            this.buttonPing.Click += new System.EventHandler(this.buttonPing_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 510);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HLMP";
            this.tabPage4.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage8.ResumeLayout(false);
            this.groupBoxMyFiles.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage4;
        private CommControl.ChatControl chatControl;
        private System.Windows.Forms.TabPage tabPage3;
        private CommControl.NetUserControl netUserControl;
        private System.Windows.Forms.TabPage tabPage1;
        private CommControl.StatusControl statusControl;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage6;
        private CommControl.FileListControl fileListControl;
        private System.Windows.Forms.TabPage tabPage7;
        private CommControl.FileControl fileControl;
        private System.Windows.Forms.TabPage tabPage2;
        private CommControl.NetworkGraphControl networkGraphControl;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TextBox textBoxPing;
        private System.Windows.Forms.Button buttonPing;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.GroupBox groupBoxMyFiles;
        private System.Windows.Forms.Button buttonAddFile;
        private System.Windows.Forms.TreeView treeViewMyFiles;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ImageList iconList;

    }
}

