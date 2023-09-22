namespace GT3用モデルサイズ調整ツール
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
            this.BackgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.ProgressBar1 = new System.Windows.Forms.ProgressBar();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Inc_Dec = new System.Windows.Forms.ComboBox();
            this.sizeedit = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BackgroundWorker1
            // 
            this.BackgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.BackgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorker1_ProgressChanged);
            this.BackgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.Location = new System.Drawing.Point(10, 9);
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(314, 18);
            this.ProgressBar1.TabIndex = 0;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(10, 36);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(191, 12);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "exeに直接ドラッグアンドドロップしてね！";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(10, 79);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(184, 12);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "または...サイズの調整 (16進数で入力)";
            // 
            // Inc_Dec
            // 
            this.Inc_Dec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Inc_Dec.FormattingEnabled = true;
            this.Inc_Dec.Items.AddRange(new object[] {
            "増やす",
            "減らす"});
            this.Inc_Dec.Location = new System.Drawing.Point(78, 93);
            this.Inc_Dec.Name = "Inc_Dec";
            this.Inc_Dec.Size = new System.Drawing.Size(82, 20);
            this.Inc_Dec.TabIndex = 4;
            // 
            // sizeedit
            // 
            this.sizeedit.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GT3用モデルサイズ調整ツール.Properties.Settings.Default, "Size_edit", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sizeedit.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.sizeedit.Location = new System.Drawing.Point(12, 94);
            this.sizeedit.MaxLength = 8;
            this.sizeedit.Name = "sizeedit";
            this.sizeedit.ShortcutsEnabled = false;
            this.sizeedit.Size = new System.Drawing.Size(60, 19);
            this.sizeedit.TabIndex = 3;
            this.sizeedit.Text = global::GT3用モデルサイズ調整ツール.Properties.Settings.Default.Size_edit;
            this.sizeedit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 122);
            this.Controls.Add(this.Inc_Dec);
            this.Controls.Add(this.sizeedit);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.ProgressBar1);
            this.Name = "Form1";
            this.Text = "GT3用モデルサイズ調整ツール";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker BackgroundWorker1;
        private System.Windows.Forms.ProgressBar ProgressBar1;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.TextBox sizeedit;
        private System.Windows.Forms.ComboBox Inc_Dec;
    }
}

