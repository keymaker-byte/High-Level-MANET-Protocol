namespace CommControlCompact
{
    partial class FileControlCompact
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

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar 
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileControlCompact));
            this.labelDownloadUser = new System.Windows.Forms.Label();
            this.labelDownloadFile = new System.Windows.Forms.Label();
            this.labelDownloadState = new System.Windows.Forms.Label();
            this.labelUploadState = new System.Windows.Forms.Label();
            this.labelUploadFile = new System.Windows.Forms.Label();
            this.labelUploadUser = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.treeView = new System.Windows.Forms.TreeView();
            this.iconList = new System.Windows.Forms.ImageList();
            this.SuspendLayout();
            // 
            // labelDownloadUser
            // 
            this.labelDownloadUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDownloadUser.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.labelDownloadUser.Location = new System.Drawing.Point(57, 17);
            this.labelDownloadUser.Name = "labelDownloadUser";
            this.labelDownloadUser.Size = new System.Drawing.Size(249, 13);
            // 
            // labelDownloadFile
            // 
            this.labelDownloadFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDownloadFile.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.labelDownloadFile.Location = new System.Drawing.Point(57, 30);
            this.labelDownloadFile.Name = "labelDownloadFile";
            this.labelDownloadFile.Size = new System.Drawing.Size(249, 13);
            // 
            // labelDownloadState
            // 
            this.labelDownloadState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDownloadState.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.labelDownloadState.Location = new System.Drawing.Point(57, 43);
            this.labelDownloadState.Name = "labelDownloadState";
            this.labelDownloadState.Size = new System.Drawing.Size(249, 13);
            // 
            // labelUploadState
            // 
            this.labelUploadState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUploadState.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.labelUploadState.Location = new System.Drawing.Point(57, 111);
            this.labelUploadState.Name = "labelUploadState";
            this.labelUploadState.Size = new System.Drawing.Size(249, 13);
            // 
            // labelUploadFile
            // 
            this.labelUploadFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUploadFile.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.labelUploadFile.Location = new System.Drawing.Point(57, 98);
            this.labelUploadFile.Name = "labelUploadFile";
            this.labelUploadFile.Size = new System.Drawing.Size(249, 13);
            // 
            // labelUploadUser
            // 
            this.labelUploadUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUploadUser.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.labelUploadUser.Location = new System.Drawing.Point(57, 85);
            this.labelUploadUser.Name = "labelUploadUser";
            this.labelUploadUser.Size = new System.Drawing.Size(249, 13);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.Font = new System.Drawing.Font("Tahoma", 8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.label6.Location = new System.Drawing.Point(2, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(304, 13);
            this.label6.Text = "File Transfer List";
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.iconList;
            this.treeView.Indent = 51;
            this.treeView.Location = new System.Drawing.Point(3, 20);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(303, 296);
            this.treeView.TabIndex = 1;
            // 
            // iconList
            // 
            this.iconList.ImageSize = new System.Drawing.Size(48, 48);
            this.iconList.Images.Clear();
            this.iconList.Images.Add(((System.Drawing.Image)(resources.GetObject("resource"))));
            // 
            // FileControlCompact
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelUploadState);
            this.Controls.Add(this.labelUploadFile);
            this.Controls.Add(this.labelUploadUser);
            this.Controls.Add(this.labelDownloadState);
            this.Controls.Add(this.labelDownloadFile);
            this.Controls.Add(this.labelDownloadUser);
            this.Name = "FileControlCompact";
            this.Size = new System.Drawing.Size(309, 319);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelDownloadUser;
        private System.Windows.Forms.Label labelDownloadFile;
        private System.Windows.Forms.Label labelDownloadState;
        private System.Windows.Forms.Label labelUploadState;
        private System.Windows.Forms.Label labelUploadFile;
        private System.Windows.Forms.Label labelUploadUser;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList iconList;
    }
}
