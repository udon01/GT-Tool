namespace BMPConverter
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Openfolderbutton = new System.Windows.Forms.Button();
            this.Folderpathtext = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Extractbutton = new System.Windows.Forms.Button();
            this.Extractpathtext = new System.Windows.Forms.TextBox();
            this.Convertbutton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Openfolderbutton);
            this.groupBox1.Controls.Add(this.Folderpathtext);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(494, 89);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "1. Specify the folder to convert";
            // 
            // Openfolderbutton
            // 
            this.Openfolderbutton.Location = new System.Drawing.Point(10, 21);
            this.Openfolderbutton.Name = "Openfolderbutton";
            this.Openfolderbutton.Size = new System.Drawing.Size(150, 30);
            this.Openfolderbutton.TabIndex = 0;
            this.Openfolderbutton.Text = "Open directory";
            this.Openfolderbutton.UseVisualStyleBackColor = true;
            this.Openfolderbutton.Click += new System.EventHandler(this.Openfolderbutton_Click);
            // 
            // Folderpathtext
            // 
            this.Folderpathtext.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::BMPConverter.Properties.Settings.Default, "FPT", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Folderpathtext.Location = new System.Drawing.Point(10, 57);
            this.Folderpathtext.Name = "Folderpathtext";
            this.Folderpathtext.ReadOnly = true;
            this.Folderpathtext.Size = new System.Drawing.Size(478, 22);
            this.Folderpathtext.TabIndex = 1;
            this.Folderpathtext.Text = global::BMPConverter.Properties.Settings.Default.FPT;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Extractbutton);
            this.groupBox2.Controls.Add(this.Extractpathtext);
            this.groupBox2.Location = new System.Drawing.Point(12, 107);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(494, 89);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "2. Specify output folder";
            // 
            // Extractbutton
            // 
            this.Extractbutton.Location = new System.Drawing.Point(10, 21);
            this.Extractbutton.Name = "Extractbutton";
            this.Extractbutton.Size = new System.Drawing.Size(150, 30);
            this.Extractbutton.TabIndex = 0;
            this.Extractbutton.Text = "Open directory";
            this.Extractbutton.UseVisualStyleBackColor = true;
            this.Extractbutton.Click += new System.EventHandler(this.Extractbutton_Click);
            // 
            // Extractpathtext
            // 
            this.Extractpathtext.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::BMPConverter.Properties.Settings.Default, "EPT", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Extractpathtext.Location = new System.Drawing.Point(10, 57);
            this.Extractpathtext.Name = "Extractpathtext";
            this.Extractpathtext.ReadOnly = true;
            this.Extractpathtext.Size = new System.Drawing.Size(478, 22);
            this.Extractpathtext.TabIndex = 1;
            this.Extractpathtext.Text = global::BMPConverter.Properties.Settings.Default.EPT;
            // 
            // Convertbutton
            // 
            this.Convertbutton.Location = new System.Drawing.Point(161, 230);
            this.Convertbutton.Name = "Convertbutton";
            this.Convertbutton.Size = new System.Drawing.Size(194, 35);
            this.Convertbutton.TabIndex = 5;
            this.Convertbutton.Text = "Convert";
            this.Convertbutton.UseVisualStyleBackColor = true;
            this.Convertbutton.Click += new System.EventHandler(this.Convertbutton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 202);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(318, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "If not entered, output to \"BMP\" on the desktop.";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 283);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Convertbutton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "GT2 BMP Converter v1.1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Openfolderbutton;
        private System.Windows.Forms.TextBox Folderpathtext;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Extractbutton;
        private System.Windows.Forms.TextBox Extractpathtext;
        private System.Windows.Forms.Button Convertbutton;
        private System.Windows.Forms.Label label1;
    }
}

