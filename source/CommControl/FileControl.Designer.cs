namespace CommControl
{
    partial class FileControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.columnUser = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnProgress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridView);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(479, 432);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Transfer List";
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnUser,
            this.columnFileName,
            this.columnProgress,
            this.columnState,
            this.columnType,
            this.columnFile});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 16);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.Size = new System.Drawing.Size(473, 413);
            this.dataGridView.TabIndex = 0;
            // 
            // columnUser
            // 
            this.columnUser.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.columnUser.HeaderText = "User";
            this.columnUser.Name = "columnUser";
            this.columnUser.ReadOnly = true;
            this.columnUser.Width = 54;
            // 
            // columnFileName
            // 
            this.columnFileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.columnFileName.HeaderText = "File";
            this.columnFileName.Name = "columnFileName";
            this.columnFileName.ReadOnly = true;
            this.columnFileName.Width = 48;
            // 
            // columnProgress
            // 
            this.columnProgress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.columnProgress.HeaderText = "Progress";
            this.columnProgress.Name = "columnProgress";
            this.columnProgress.ReadOnly = true;
            this.columnProgress.Width = 73;
            // 
            // columnState
            // 
            this.columnState.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.columnState.HeaderText = "State";
            this.columnState.Name = "columnState";
            this.columnState.ReadOnly = true;
            this.columnState.Width = 57;
            // 
            // columnType
            // 
            this.columnType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.columnType.HeaderText = "Type";
            this.columnType.Name = "columnType";
            this.columnType.ReadOnly = true;
            this.columnType.Width = 56;
            // 
            // columnFile
            // 
            this.columnFile.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.columnFile.HeaderText = "Id";
            this.columnFile.Name = "columnFile";
            this.columnFile.ReadOnly = true;
            this.columnFile.Width = 41;
            // 
            // FileControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "FileControl";
            this.Size = new System.Drawing.Size(479, 432);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnUser;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnProgress;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnState;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnFile;
    }
}
