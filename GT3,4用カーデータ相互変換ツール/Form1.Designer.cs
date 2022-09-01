namespace GT3_4用カーデータ相互変換ツール
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 61);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.ProgressBar1);
            this.Name = "Form1";
            this.Text = "GT3,4用カーデータ相互変換ツール";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker BackgroundWorker1;
        private System.Windows.Forms.ProgressBar ProgressBar1;
        private System.Windows.Forms.Label Label1;
    }
}

