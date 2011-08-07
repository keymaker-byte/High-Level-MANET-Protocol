namespace CommControl
{
    partial class NetUserControlCompact
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetUserControlCompact));
            this.treeView = new System.Windows.Forms.TreeView();
            this.iconList = new System.Windows.Forms.ImageList();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.iconList;
            this.treeView.Indent = 67;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(256, 337);
            this.treeView.TabIndex = 1;
            // 
            // iconList
            // 
            this.iconList.ImageSize = new System.Drawing.Size(48, 48);
            this.iconList.Images.Clear();
            this.iconList.Images.Add(((System.Drawing.Image)(resources.GetObject("resource"))));
            this.iconList.Images.Add(((System.Drawing.Image)(resources.GetObject("resource1"))));
            this.iconList.Images.Add(((System.Drawing.Image)(resources.GetObject("resource2"))));
            this.iconList.Images.Add(((System.Drawing.Image)(resources.GetObject("resource3"))));
            this.iconList.Images.Add(((System.Drawing.Image)(resources.GetObject("resource4"))));
            // 
            // NetUserControlCompact
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.treeView);
            this.Name = "NetUserControlCompact";
            this.Size = new System.Drawing.Size(256, 337);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList iconList;

    }
}
