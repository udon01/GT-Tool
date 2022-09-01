namespace GT7_B_specツール
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
            this.Start_notdaily = new System.Windows.Forms.Button();
            this.Modosu = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Stop = new System.Windows.Forms.Button();
            this.Workmessage = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.Waitbacktomenu = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.Waitentry = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.Waitentry2 = new System.Windows.Forms.NumericUpDown();
            this.Waitracemenu = new System.Windows.Forms.NumericUpDown();
            this.Waitreplay = new System.Windows.Forms.NumericUpDown();
            this.Waitstart = new System.Windows.Forms.NumericUpDown();
            this.Start = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.Rightkeydown = new System.Windows.Forms.NumericUpDown();
            this.Racefinishsec = new System.Windows.Forms.NumericUpDown();
            this.Racefinishmin = new System.Windows.Forms.NumericUpDown();
            this.Rightkeywait = new System.Windows.Forms.NumericUpDown();
            this.Settingexport = new System.Windows.Forms.Button();
            this.Settingimport = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Waitbacktomenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Waitentry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Waitentry2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Waitracemenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Waitreplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Waitstart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Rightkeydown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Racefinishsec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Racefinishmin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Rightkeywait)).BeginInit();
            this.SuspendLayout();
            // 
            // Start_notdaily
            // 
            this.Start_notdaily.Location = new System.Drawing.Point(9, 214);
            this.Start_notdaily.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Start_notdaily.Name = "Start_notdaily";
            this.Start_notdaily.Size = new System.Drawing.Size(378, 24);
            this.Start_notdaily.TabIndex = 15;
            this.Start_notdaily.Text = "開始(デイリーミッション未消化) ※0kmのみ";
            this.Start_notdaily.UseVisualStyleBackColor = true;
            this.Start_notdaily.Click += new System.EventHandler(this.Start_notdaily_Click);
            // 
            // Modosu
            // 
            this.Modosu.Location = new System.Drawing.Point(206, 99);
            this.Modosu.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Modosu.Name = "Modosu";
            this.Modosu.Size = new System.Drawing.Size(64, 63);
            this.Modosu.TabIndex = 17;
            this.Modosu.Text = "設定を初期化";
            this.Modosu.UseVisualStyleBackColor = true;
            this.Modosu.Click += new System.EventHandler(this.Modosu_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(164, 106);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 16;
            this.label8.Text = "秒";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 106);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "2戦目ロード";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(164, 85);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = "秒";
            // 
            // Stop
            // 
            this.Stop.Location = new System.Drawing.Point(233, 178);
            this.Stop.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(154, 24);
            this.Stop.TabIndex = 10;
            this.Stop.Text = "停止";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // Workmessage
            // 
            this.Workmessage.AutoSize = true;
            this.Workmessage.Location = new System.Drawing.Point(9, 250);
            this.Workmessage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Workmessage.Name = "Workmessage";
            this.Workmessage.Size = new System.Drawing.Size(120, 12);
            this.Workmessage.TabIndex = 9;
            this.Workmessage.Text = "現在実行されていません";
            this.Workmessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 85);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "リプレイ～レースメニュー";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(164, 62);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "秒";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 62);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "賞金獲得～リプレイ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 41);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "秒";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 41);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "スタートボタンを押した後";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.Waitbacktomenu);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.Waitentry);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.Waitentry2);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.Waitracemenu);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.Waitreplay);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.Waitstart);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(9, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(187, 156);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PS4ユーザー向け(ロード時間調整)";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(164, 127);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 12);
            this.label11.TabIndex = 23;
            this.label11.Text = "秒";
            // 
            // Waitbacktomenu
            // 
            this.Waitbacktomenu.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GT7_B_specツール.Properties.Settings.Default, "wait_backtomenu", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Waitbacktomenu.DecimalPlaces = 1;
            this.Waitbacktomenu.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Waitbacktomenu.Location = new System.Drawing.Point(120, 125);
            this.Waitbacktomenu.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Waitbacktomenu.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.Waitbacktomenu.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Waitbacktomenu.Name = "Waitbacktomenu";
            this.Waitbacktomenu.Size = new System.Drawing.Size(39, 19);
            this.Waitbacktomenu.TabIndex = 22;
            this.Waitbacktomenu.Value = global::GT7_B_specツール.Properties.Settings.Default.wait_backtomenu;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(4, 127);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(106, 12);
            this.label12.TabIndex = 21;
            this.label12.Text = "2戦目棄権～メニュー";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(164, 19);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 12);
            this.label9.TabIndex = 20;
            this.label9.Text = "秒";
            // 
            // Waitentry
            // 
            this.Waitentry.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GT7_B_specツール.Properties.Settings.Default, "wait_entry", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Waitentry.DecimalPlaces = 1;
            this.Waitentry.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Waitentry.Location = new System.Drawing.Point(120, 17);
            this.Waitentry.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Waitentry.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.Waitentry.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Waitentry.Name = "Waitentry";
            this.Waitentry.Size = new System.Drawing.Size(39, 19);
            this.Waitentry.TabIndex = 19;
            this.Waitentry.Value = global::GT7_B_specツール.Properties.Settings.Default.wait_entry;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 19);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 12);
            this.label10.TabIndex = 18;
            this.label10.Text = "エントリー後";
            // 
            // Waitentry2
            // 
            this.Waitentry2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GT7_B_specツール.Properties.Settings.Default, "wait_race2load", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Waitentry2.DecimalPlaces = 1;
            this.Waitentry2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Waitentry2.Location = new System.Drawing.Point(120, 103);
            this.Waitentry2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Waitentry2.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.Waitentry2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Waitentry2.Name = "Waitentry2";
            this.Waitentry2.Size = new System.Drawing.Size(39, 19);
            this.Waitentry2.TabIndex = 15;
            this.Waitentry2.Value = global::GT7_B_specツール.Properties.Settings.Default.wait_race2load;
            // 
            // Waitracemenu
            // 
            this.Waitracemenu.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GT7_B_specツール.Properties.Settings.Default, "wait_racemenu", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Waitracemenu.DecimalPlaces = 1;
            this.Waitracemenu.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Waitracemenu.Location = new System.Drawing.Point(120, 82);
            this.Waitracemenu.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Waitracemenu.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.Waitracemenu.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Waitracemenu.Name = "Waitracemenu";
            this.Waitracemenu.Size = new System.Drawing.Size(39, 19);
            this.Waitracemenu.TabIndex = 12;
            this.Waitracemenu.Value = global::GT7_B_specツール.Properties.Settings.Default.wait_racemenu;
            // 
            // Waitreplay
            // 
            this.Waitreplay.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GT7_B_specツール.Properties.Settings.Default, "wait_replay", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Waitreplay.DecimalPlaces = 1;
            this.Waitreplay.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Waitreplay.Location = new System.Drawing.Point(120, 60);
            this.Waitreplay.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Waitreplay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.Waitreplay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Waitreplay.Name = "Waitreplay";
            this.Waitreplay.Size = new System.Drawing.Size(39, 19);
            this.Waitreplay.TabIndex = 9;
            this.Waitreplay.Value = global::GT7_B_specツール.Properties.Settings.Default.wait_replay;
            // 
            // Waitstart
            // 
            this.Waitstart.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GT7_B_specツール.Properties.Settings.Default, "wait_start", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Waitstart.DecimalPlaces = 1;
            this.Waitstart.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Waitstart.Location = new System.Drawing.Point(120, 38);
            this.Waitstart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Waitstart.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.Waitstart.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Waitstart.Name = "Waitstart";
            this.Waitstart.Size = new System.Drawing.Size(39, 19);
            this.Waitstart.TabIndex = 6;
            this.Waitstart.Value = global::GT7_B_specツール.Properties.Settings.Default.wait_start;
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(9, 178);
            this.Start.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(154, 24);
            this.Start.TabIndex = 8;
            this.Start.Text = "開始";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(200, 29);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(76, 12);
            this.label13.TabIndex = 19;
            this.label13.Text = "レース終了まで";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(200, 72);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(71, 12);
            this.label14.TabIndex = 20;
            this.label14.Text = "右キーの間隔";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(317, 71);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(17, 12);
            this.label15.TabIndex = 23;
            this.label15.Text = "秒";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(311, 29);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(17, 12);
            this.label16.TabIndex = 24;
            this.label16.Text = "分";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(372, 29);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(17, 12);
            this.label17.TabIndex = 26;
            this.label17.Text = "秒";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(200, 10);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(41, 12);
            this.label18.TabIndex = 27;
            this.label18.Text = "調整用";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(317, 50);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(17, 12);
            this.label19.TabIndex = 30;
            this.label19.Text = "秒";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(200, 50);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(73, 12);
            this.label20.TabIndex = 28;
            this.label20.Text = "右キー入力秒";
            // 
            // Rightkeydown
            // 
            this.Rightkeydown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GT7_B_specツール.Properties.Settings.Default, "rightkey_down", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Rightkeydown.DecimalPlaces = 1;
            this.Rightkeydown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Rightkeydown.Location = new System.Drawing.Point(275, 47);
            this.Rightkeydown.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Rightkeydown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.Rightkeydown.Name = "Rightkeydown";
            this.Rightkeydown.Size = new System.Drawing.Size(38, 19);
            this.Rightkeydown.TabIndex = 29;
            this.Rightkeydown.Value = global::GT7_B_specツール.Properties.Settings.Default.rightkey_down;
            // 
            // Racefinishsec
            // 
            this.Racefinishsec.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GT7_B_specツール.Properties.Settings.Default, "racefinish_sec", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Racefinishsec.Location = new System.Drawing.Point(332, 26);
            this.Racefinishsec.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Racefinishsec.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.Racefinishsec.Name = "Racefinishsec";
            this.Racefinishsec.Size = new System.Drawing.Size(35, 19);
            this.Racefinishsec.TabIndex = 25;
            this.Racefinishsec.Value = global::GT7_B_specツール.Properties.Settings.Default.racefinish_sec;
            // 
            // Racefinishmin
            // 
            this.Racefinishmin.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GT7_B_specツール.Properties.Settings.Default, "racefinish_min", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Racefinishmin.Location = new System.Drawing.Point(275, 26);
            this.Racefinishmin.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Racefinishmin.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.Racefinishmin.Name = "Racefinishmin";
            this.Racefinishmin.Size = new System.Drawing.Size(32, 19);
            this.Racefinishmin.TabIndex = 22;
            this.Racefinishmin.Value = global::GT7_B_specツール.Properties.Settings.Default.racefinish_min;
            // 
            // Rightkeywait
            // 
            this.Rightkeywait.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GT7_B_specツール.Properties.Settings.Default, "rightkey_wait", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Rightkeywait.DecimalPlaces = 1;
            this.Rightkeywait.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Rightkeywait.Location = new System.Drawing.Point(275, 69);
            this.Rightkeywait.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Rightkeywait.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.Rightkeywait.Name = "Rightkeywait";
            this.Rightkeywait.Size = new System.Drawing.Size(38, 19);
            this.Rightkeywait.TabIndex = 21;
            this.Rightkeywait.Value = global::GT7_B_specツール.Properties.Settings.Default.rightkey_wait;
            // 
            // Settingexport
            // 
            this.Settingexport.Location = new System.Drawing.Point(290, 99);
            this.Settingexport.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Settingexport.Name = "Settingexport";
            this.Settingexport.Size = new System.Drawing.Size(98, 28);
            this.Settingexport.TabIndex = 31;
            this.Settingexport.Text = "設定を書き出す";
            this.Settingexport.UseVisualStyleBackColor = true;
            this.Settingexport.Click += new System.EventHandler(this.Settingexport_Click);
            // 
            // Settingimport
            // 
            this.Settingimport.Location = new System.Drawing.Point(290, 134);
            this.Settingimport.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Settingimport.Name = "Settingimport";
            this.Settingimport.Size = new System.Drawing.Size(98, 28);
            this.Settingimport.TabIndex = 32;
            this.Settingimport.Text = "設定を読み込む";
            this.Settingimport.UseVisualStyleBackColor = true;
            this.Settingimport.Click += new System.EventHandler(this.Settingimport_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 274);
            this.Controls.Add(this.Settingimport);
            this.Controls.Add(this.Settingexport);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.Rightkeydown);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.Racefinishsec);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.Racefinishmin);
            this.Controls.Add(this.Rightkeywait);
            this.Controls.Add(this.Modosu);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.Start_notdaily);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.Workmessage);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Start);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "GT7 B-specツール v1.22";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Waitbacktomenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Waitentry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Waitentry2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Waitracemenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Waitreplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Waitstart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Rightkeydown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Racefinishsec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Racefinishmin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Rightkeywait)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Start_notdaily;
        private System.Windows.Forms.Button Modosu;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown Waitentry2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button Stop;
        private System.Windows.Forms.Label Workmessage;
        private System.Windows.Forms.NumericUpDown Waitracemenu;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown Waitreplay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown Waitstart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown Waitbacktomenu;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown Waitentry;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown Rightkeywait;
        private System.Windows.Forms.NumericUpDown Racefinishmin;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown Racefinishsec;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.NumericUpDown Rightkeydown;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button Settingexport;
        private System.Windows.Forms.Button Settingimport;
    }
}

