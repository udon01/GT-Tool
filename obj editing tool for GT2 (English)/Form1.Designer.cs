namespace objeditingtoolforGT2
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.File_open = new System.Windows.Forms.Button();
            this.Convert = new System.Windows.Forms.Button();
            this.OBJpath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // File_open
            // 
            this.File_open.Location = new System.Drawing.Point(12, 12);
            this.File_open.Name = "File_open";
            this.File_open.Size = new System.Drawing.Size(408, 24);
            this.File_open.TabIndex = 0;
            this.File_open.Text = "Open File";
            this.File_open.UseVisualStyleBackColor = true;
            this.File_open.Click += new System.EventHandler(this.File_open_Click);
            // 
            // Convert
            // 
            this.Convert.Location = new System.Drawing.Point(165, 80);
            this.Convert.Name = "Convert";
            this.Convert.Size = new System.Drawing.Size(100, 24);
            this.Convert.TabIndex = 4;
            this.Convert.Text = "Convert";
            this.Convert.UseVisualStyleBackColor = true;
            this.Convert.Click += new System.EventHandler(this.Convert_Click);
            // 
            // OBJpath
            // 
            this.OBJpath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::objeditingtoolforGT2.Properties.Settings.Default, "OBJpathsave", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.OBJpath.Location = new System.Drawing.Point(12, 47);
            this.OBJpath.Name = "OBJpath";
            this.OBJpath.ReadOnly = true;
            this.OBJpath.Size = new System.Drawing.Size(408, 22);
            this.OBJpath.TabIndex = 5;
            this.OBJpath.Text = global::objeditingtoolforGT2.Properties.Settings.Default.OBJpathsave;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 118);
            this.Controls.Add(this.OBJpath);
            this.Controls.Add(this.Convert);
            this.Controls.Add(this.File_open);
            this.Name = "Form1";
            this.Text = "obj editing tool for GT2 (English)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button File_open;
        private System.Windows.Forms.Button Convert;
        private System.Windows.Forms.TextBox OBJpath;
    }
}

