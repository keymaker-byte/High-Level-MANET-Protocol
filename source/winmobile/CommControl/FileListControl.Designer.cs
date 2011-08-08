namespace CommControl
{
    partial class FileListControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileListControl));
            this.treeView = new System.Windows.Forms.TreeView();
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.menuItem = new System.Windows.Forms.MenuItem();
            this.iconList = new System.Windows.Forms.ImageList();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.ContextMenu = this.contextMenu;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.iconList;
            this.treeView.Indent = 67;
            this.treeView.Location = new System.Drawing.Point(3, 3);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(234, 239);
            this.treeView.TabIndex = 1;
            // 
            // contextMenu
            // 
            this.contextMenu.MenuItems.Add(this.menuItem);
            // 
            // menuItem
            // 
            this.menuItem.Text = "Download";
            this.menuItem.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // iconList
            // 
            this.iconList.ImageSize = new System.Drawing.Size(48, 48);
            this.iconList.Images.Clear();
            this.iconList.Images.Add(((System.Drawing.Image)(resources.GetObject("resource"))));
            // 
            // FileListControlCompact
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.treeView);
            this.Name = "FileListControlCompact";
            this.Size = new System.Drawing.Size(240, 245);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList iconList;
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem menuItem;

    }
}
