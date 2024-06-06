using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GT7_B_specツール
{
    public partial class Form1 : Form
    {
        public static bool stop = false;

        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private async void Start_Click(object sender, EventArgs e)
        {
            stop = false;
            DateTime nowtime2 = DateTime.Now;
            int count = 0;
            Workmessage.Text = "実行中...";

            try
            {
                var client = new ViGEmClient();
                var controller = client.CreateDualShock4Controller();
                controller.Connect();
                await Task.Delay(500);

                string accel = ComboBoxaccel.Text;
                string handle = ComboBoxhandle.Text;
                string boost = ComboBoxboost.Text;

                if (accel == "" || handle == "")
                    goto label2;

                label1:

                decimal wait_entry_dec = Waitentry.Value * 1000;
                decimal wait_start_dec = Waitstart.Value * 1000;
                decimal wait_replay_dec = Waitreplay.Value * 1000;
                decimal wait_racemenu_dec = Waitracemenu.Value * 1000;
                decimal wait_entry2_dec = Waitentry2.Value * 1000;
                decimal wait_backtomenu_dec = Waitbacktomenu.Value * 1000;
                int racefinish_min = decimal.ToInt32(Racefinishmin.Value) * 60;
                int racefinish_sec = decimal.ToInt32(Racefinishsec.Value);
                decimal rightkey_down_dec = Rightkeydown.Value * 1000;
                decimal rightkey_wait_dec = Rightkeywait.Value * 1000;
                int racefinishover = 0;
                int racefinish = (racefinish_min + racefinish_sec) * 1000;
                racefinish += 6000;
                int wait_entry = decimal.ToInt32(wait_entry_dec);
                int wait_start = decimal.ToInt32(wait_start_dec);
                int wait_replay = decimal.ToInt32(wait_replay_dec);
                int wait_racemenu = decimal.ToInt32(wait_racemenu_dec);
                int wait_entry2 = decimal.ToInt32(wait_entry2_dec);
                int wait_backtomenu = decimal.ToInt32(wait_backtomenu_dec);
                int rightkey_down = decimal.ToInt32(rightkey_down_dec);
                int rightkey_wait = decimal.ToInt32(rightkey_wait_dec);


                controller.SetDPadDirection(DualShock4DPadDirection.South);
                await Task.Delay(220);
                controller.SetDPadDirection(DualShock4DPadDirection.None);

                for (int i = 0; i < 6; i++)
                {
                    await Task.Delay(300);
                    controller.SetDPadDirection(DualShock4DPadDirection.East);
                    await Task.Delay(220);
                    controller.SetDPadDirection(DualShock4DPadDirection.None);
                }

                await Task.Delay(300);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);
                await Task.Delay(500);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);

                await Task.Delay(wait_entry);

                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);
                await Task.Delay(780);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);

                await Task.Delay(wait_start);

                await Task.Delay(14000);

                //アクセル
                if (ComboBoxaccel.Text == "×")
                    controller.SetButtonState(DualShock4Button.Cross, true);
                if (ComboBoxaccel.Text == "□")
                    controller.SetButtonState(DualShock4Button.Square, true);
                if (ComboBoxaccel.Text == "△")
                    controller.SetButtonState(DualShock4Button.Triangle, true);
                if (ComboBoxaccel.Text == "〇")
                    controller.SetButtonState(DualShock4Button.Circle, true);
                if (ComboBoxaccel.Text == "↑")
                    controller.SetDPadDirection(DualShock4DPadDirection.North);
                if (ComboBoxaccel.Text == "→")
                    controller.SetDPadDirection(DualShock4DPadDirection.East);
                if (ComboBoxaccel.Text == "←")
                    controller.SetDPadDirection(DualShock4DPadDirection.West);
                if (ComboBoxaccel.Text == "↓")
                    controller.SetDPadDirection(DualShock4DPadDirection.South);
                if (ComboBoxaccel.Text == "L1")
                    controller.SetButtonState(DualShock4Button.ShoulderLeft, true);
                if (ComboBoxaccel.Text == "R1")
                    controller.SetButtonState(DualShock4Button.ShoulderRight, true);
                if (ComboBoxaccel.Text == "R2")
                {
                    controller.SetButtonState(DualShock4Button.TriggerRight, true);
                    controller.SetSliderValue(DualShock4Slider.RightTrigger, 255);
                }
                if (ComboBoxaccel.Text == "L3")
                    controller.SetButtonState(DualShock4Button.ThumbLeft, true);
                if (ComboBoxaccel.Text == "R3")
                    controller.SetButtonState(DualShock4Button.ThumbRight, true);
                if (ComboBoxaccel.Text == "右スティック")
                    controller.SetAxisValue(DualShock4Axis.RightThumbY, byte.MinValue);


                if (boost != "")
                {
                    if (ComboBoxboost.Text == "×")
                        controller.SetButtonState(DualShock4Button.Cross, true);
                    if (ComboBoxboost.Text == "□")
                        controller.SetButtonState(DualShock4Button.Square, true);
                    if (ComboBoxboost.Text == "△")
                        controller.SetButtonState(DualShock4Button.Triangle, true);
                    if (ComboBoxboost.Text == "〇")
                        controller.SetButtonState(DualShock4Button.Circle, true);
                    if (ComboBoxboost.Text == "↑")
                        controller.SetDPadDirection(DualShock4DPadDirection.North);
                    if (ComboBoxboost.Text == "→")
                        controller.SetDPadDirection(DualShock4DPadDirection.East);
                    if (ComboBoxboost.Text == "←")
                        controller.SetDPadDirection(DualShock4DPadDirection.West);
                    if (ComboBoxboost.Text == "↓")
                        controller.SetDPadDirection(DualShock4DPadDirection.South);
                    if (ComboBoxboost.Text == "L1")
                        controller.SetButtonState(DualShock4Button.ShoulderLeft, true);
                    if (ComboBoxboost.Text == "R1")
                        controller.SetButtonState(DualShock4Button.ShoulderRight, true);
                    if (ComboBoxboost.Text == "L3")
                        controller.SetButtonState(DualShock4Button.ThumbLeft, true);
                    if (ComboBoxboost.Text == "R3")
                        controller.SetButtonState(DualShock4Button.ThumbRight, true);
                }

                if (Alwaysstickcheck.Checked == false)
                {
                    while (true)
                    {
                        if (ComboBoxhandle.Text == "方向キー")
                            controller.SetDPadDirection(DualShock4DPadDirection.East);
                        if (ComboBoxhandle.Text == "左スティック")
                            controller.SetAxisValue(DualShock4Axis.LeftThumbX, byte.MaxValue);
                        await Task.Delay(rightkey_down);
                        if (ComboBoxhandle.Text == "方向キー")
                            controller.SetDPadDirection(DualShock4DPadDirection.None);
                        if (ComboBoxhandle.Text == "左スティック")
                            controller.SetAxisValue(DualShock4Axis.LeftThumbX, 128);
                        await Task.Delay(rightkey_wait);
                        racefinishover = racefinishover + rightkey_down + rightkey_wait;
                        if (racefinishover > racefinish)
                            break;
                    }
                }

                else
                {
                    int angle128_int = (int)Stickangle.Value + 128;
                    byte angle128 = (byte)angle128_int;
                    controller.SetAxisValue(DualShock4Axis.LeftThumbX, angle128);
                    while (true)
                    {
                        await Task.Delay(1000);
                        racefinishover += 1000;
                        if (racefinishover > racefinish)
                        {
                            controller.SetAxisValue(DualShock4Axis.LeftThumbX, 128);
                            break;
                        }
                    }
                }

                //ブーストオフ
                if (ComboBoxboost.Text == "×")
                    controller.SetButtonState(DualShock4Button.Cross, false);
                if (ComboBoxboost.Text == "□")
                    controller.SetButtonState(DualShock4Button.Square, false);
                if (ComboBoxboost.Text == "△")
                    controller.SetButtonState(DualShock4Button.Triangle, false);
                if (ComboBoxboost.Text == "〇")
                    controller.SetButtonState(DualShock4Button.Circle, false);
                if (ComboBoxboost.Text == "↑" || ComboBoxboost.Text == "→" || ComboBoxboost.Text == "←" || ComboBoxboost.Text == "↓")
                    controller.SetDPadDirection(DualShock4DPadDirection.None);
                if (ComboBoxboost.Text == "L1")
                    controller.SetButtonState(DualShock4Button.ShoulderLeft, false);
                if (ComboBoxboost.Text == "R1")
                    controller.SetButtonState(DualShock4Button.ShoulderRight, false);
                if (ComboBoxboost.Text == "L3")
                    controller.SetButtonState(DualShock4Button.ThumbLeft, false);
                if (ComboBoxboost.Text == "R3")
                    controller.SetButtonState(DualShock4Button.ThumbRight, false);

                //アクセルオフ
                if (ComboBoxaccel.Text == "×")
                    controller.SetButtonState(DualShock4Button.Cross, false);
                if (ComboBoxaccel.Text == "□")
                    controller.SetButtonState(DualShock4Button.Square, false);
                if (ComboBoxaccel.Text == "△")
                    controller.SetButtonState(DualShock4Button.Triangle, false);
                if (ComboBoxaccel.Text == "〇")
                    controller.SetButtonState(DualShock4Button.Circle, false);
                if (ComboBoxaccel.Text == "↑" || ComboBoxaccel.Text == "→" || ComboBoxaccel.Text == "←" || ComboBoxaccel.Text == "↓")
                    controller.SetDPadDirection(DualShock4DPadDirection.None);
                if (ComboBoxaccel.Text == "L1")
                    controller.SetButtonState(DualShock4Button.ShoulderLeft, false);
                if (ComboBoxaccel.Text == "R1")
                    controller.SetButtonState(DualShock4Button.ShoulderRight, false);
                if (ComboBoxaccel.Text == "R2")
                {
                    controller.SetSliderValue(DualShock4Slider.RightTrigger, 0);
                    controller.SetButtonState(DualShock4Button.TriggerRight, false);
                }
                if (ComboBoxaccel.Text == "L3")
                    controller.SetButtonState(DualShock4Button.ThumbLeft, false);
                if (ComboBoxaccel.Text == "R3")
                    controller.SetButtonState(DualShock4Button.ThumbRight, false);
                if (ComboBoxaccel.Text == "右スティック")
                    controller.SetAxisValue(DualShock4Axis.RightThumbY, 128);

                if (Gettimecheck.Checked == true)
                {
                    var nowtime = DateTime.Now;
                    bool daycheck = (nowtime.Date == nowtime2.Date);
                    if (daycheck == false)
                        count += 1;
                }

                if (count < 3)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        await Task.Delay(780);
                        if (PS5check.Checked == true)
                            controller.SetButtonState(DualShock4Button.Cross, true);
                        else
                            controller.SetButtonState(DualShock4Button.Circle, true);
                        await Task.Delay(220);
                        if (PS5check.Checked == true)
                            controller.SetButtonState(DualShock4Button.Cross, false);
                        else
                            controller.SetButtonState(DualShock4Button.Circle, false);
                    }
                }

                if (count >= 3)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        await Task.Delay(780);
                        if (PS5check.Checked == true)
                            controller.SetButtonState(DualShock4Button.Cross, true);
                        else
                            controller.SetButtonState(DualShock4Button.Circle, true);
                        await Task.Delay(220);
                        if (PS5check.Checked == true)
                            controller.SetButtonState(DualShock4Button.Cross, false);
                        else
                            controller.SetButtonState(DualShock4Button.Circle, false);
                    }
                    await Task.Delay(4500);
                    if (PS5check.Checked == true)
                        controller.SetButtonState(DualShock4Button.Cross, true);
                    else
                        controller.SetButtonState(DualShock4Button.Circle, true);
                    await Task.Delay(220);
                    if (PS5check.Checked == true)
                        controller.SetButtonState(DualShock4Button.Cross, false);
                    else
                        controller.SetButtonState(DualShock4Button.Circle, false);
                    await Task.Delay(1000);
                    if (PS5check.Checked == true)
                        controller.SetButtonState(DualShock4Button.Cross, true);
                    else
                        controller.SetButtonState(DualShock4Button.Circle, true);
                    await Task.Delay(220);
                    if (PS5check.Checked == true)
                        controller.SetButtonState(DualShock4Button.Cross, false);
                    else
                        controller.SetButtonState(DualShock4Button.Circle, false);
                    nowtime2 = DateTime.Now;
                    count = 0;
                }

                await Task.Delay(wait_replay);

                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);
                await Task.Delay(500);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);

                await Task.Delay(wait_racemenu);

                controller.SetDPadDirection(DualShock4DPadDirection.East);
                await Task.Delay(220);
                controller.SetDPadDirection(DualShock4DPadDirection.None);
                await Task.Delay(500);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);

                await Task.Delay(wait_entry2);

                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);
                await Task.Delay(1000);

                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Circle, true);
                else
                    controller.SetButtonState(DualShock4Button.Cross, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Circle, false);
                else
                    controller.SetButtonState(DualShock4Button.Cross, false);
                await Task.Delay(500);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);
                await Task.Delay(500);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);

                await Task.Delay(wait_backtomenu);

                if (stop == false)
                    goto label1;

                else if (stop == true)
                    goto label2;

            label2:
                if (accel == "" && handle == "")
                    MessageBox.Show("アクセルとハンドルにボタンがセットされていません！");
                if (accel == "" && handle != "")
                    MessageBox.Show("アクセルにボタンがセットされていません！");
                if (accel != "" && handle == "")
                    MessageBox.Show("ハンドルにボタンがセットされていません！");
                stop = false;
                controller.Disconnect();
                Stop.Text = "停止";
                Workmessage.Text = "現在実行されていません";
            }

            catch (Nefarius.ViGEm.Client.Exceptions.VigemBusNotFoundException)
            {
                MessageBox.Show("ViGEmBusをインストールしてね！\nreadmeにURL載せてあります");
            }
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            if (stop == false && Workmessage.Text == "実行中...")
            {
                Workmessage.Text = "停止中。レース終了までお待ちください";
                stop = true;
                Stop.Text = "停止をキャンセル";
            }
            else if (stop == true && Workmessage.Text == "停止中。レース終了までお待ちください")
            {
                Workmessage.Text = "実行中...";
                stop = false;
                Stop.Text = "停止";
            }
        }

        private async void Start_notdaily_Click(object sender, EventArgs e)
        {

            stop = false;
            DateTime nowtime2 = DateTime.Now;
            int count = 0;
            Workmessage.Text = "実行中...";

            try
            {
                var client = new ViGEmClient();
                var controller = client.CreateDualShock4Controller();
                controller.Connect();
                await Task.Delay(500);

                string accel = ComboBoxaccel.Text;
                string handle = ComboBoxhandle.Text;
                string boost = ComboBoxboost.Text;

                if (accel == "" || handle == "")
                    goto label2;

                int count_day = 1;

            label1:

                decimal wait_entry_dec = Waitentry.Value * 1000;
                decimal wait_start_dec = Waitstart.Value * 1000;
                decimal wait_replay_dec = Waitreplay.Value * 1000;
                decimal wait_racemenu_dec = Waitracemenu.Value * 1000;
                decimal wait_entry2_dec = Waitentry2.Value * 1000;
                decimal wait_backtomenu_dec = Waitbacktomenu.Value * 1000;
                int racefinish_min = decimal.ToInt32(Racefinishmin.Value) * 60;
                int racefinish_sec = decimal.ToInt32(Racefinishsec.Value);
                decimal rightkey_down_dec = Rightkeydown.Value * 1000;
                decimal rightkey_wait_dec = Rightkeywait.Value * 1000;
                int racefinishover = 0;
                int racefinish = (racefinish_min + racefinish_sec) * 1000;
                racefinish += 6000;
                int wait_entry = decimal.ToInt32(wait_entry_dec);
                int wait_start = decimal.ToInt32(wait_start_dec);
                int wait_replay = decimal.ToInt32(wait_replay_dec);
                int wait_racemenu = decimal.ToInt32(wait_racemenu_dec);
                int wait_entry2 = decimal.ToInt32(wait_entry2_dec);
                int wait_backtomenu = decimal.ToInt32(wait_backtomenu_dec);
                int rightkey_down = decimal.ToInt32(rightkey_down_dec);
                int rightkey_wait = decimal.ToInt32(rightkey_wait_dec);

                if (count_day >= 1)
                    count_day = count_day + 1;


                controller.SetDPadDirection(DualShock4DPadDirection.South);
                await Task.Delay(220);
                controller.SetDPadDirection(DualShock4DPadDirection.None);

                for (int i = 0; i < 6; i++)
                {
                    await Task.Delay(300);
                    controller.SetDPadDirection(DualShock4DPadDirection.East);
                    await Task.Delay(220);
                    controller.SetDPadDirection(DualShock4DPadDirection.None);
                }

                await Task.Delay(300);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);
                await Task.Delay(500);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);

                await Task.Delay(wait_entry);

                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);
                await Task.Delay(1000);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);

                await Task.Delay(wait_start);

                await Task.Delay(14000);

                //アクセル
                if (ComboBoxaccel.Text == "×")
                    controller.SetButtonState(DualShock4Button.Cross, true);
                if (ComboBoxaccel.Text == "□")
                    controller.SetButtonState(DualShock4Button.Square, true);
                if (ComboBoxaccel.Text == "△")
                    controller.SetButtonState(DualShock4Button.Triangle, true);
                if (ComboBoxaccel.Text == "〇")
                    controller.SetButtonState(DualShock4Button.Circle, true);
                if (ComboBoxaccel.Text == "↑")
                    controller.SetDPadDirection(DualShock4DPadDirection.North);
                if (ComboBoxaccel.Text == "→")
                    controller.SetDPadDirection(DualShock4DPadDirection.East);
                if (ComboBoxaccel.Text == "←")
                    controller.SetDPadDirection(DualShock4DPadDirection.West);
                if (ComboBoxaccel.Text == "↓")
                    controller.SetDPadDirection(DualShock4DPadDirection.South);
                if (ComboBoxaccel.Text == "L1")
                    controller.SetButtonState(DualShock4Button.ShoulderLeft, true);
                if (ComboBoxaccel.Text == "R1")
                    controller.SetButtonState(DualShock4Button.ShoulderRight, true);
                if (ComboBoxaccel.Text == "R2")
                {
                    controller.SetButtonState(DualShock4Button.TriggerRight, true);
                    controller.SetSliderValue(DualShock4Slider.RightTrigger, 255);
                }
                if (ComboBoxaccel.Text == "L3")
                    controller.SetButtonState(DualShock4Button.ThumbLeft, true);
                if (ComboBoxaccel.Text == "R3")
                    controller.SetButtonState(DualShock4Button.ThumbRight, true);
                if (ComboBoxaccel.Text == "右スティック")
                    controller.SetAxisValue(DualShock4Axis.RightThumbY, byte.MinValue);


                if (boost != "")
                {
                    if (ComboBoxboost.Text == "×")
                        controller.SetButtonState(DualShock4Button.Cross, true);
                    if (ComboBoxboost.Text == "□")
                        controller.SetButtonState(DualShock4Button.Square, true);
                    if (ComboBoxboost.Text == "△")
                        controller.SetButtonState(DualShock4Button.Triangle, true);
                    if (ComboBoxboost.Text == "〇")
                        controller.SetButtonState(DualShock4Button.Circle, true);
                    if (ComboBoxboost.Text == "↑")
                        controller.SetDPadDirection(DualShock4DPadDirection.North);
                    if (ComboBoxboost.Text == "→")
                        controller.SetDPadDirection(DualShock4DPadDirection.East);
                    if (ComboBoxboost.Text == "←")
                        controller.SetDPadDirection(DualShock4DPadDirection.West);
                    if (ComboBoxboost.Text == "↓")
                        controller.SetDPadDirection(DualShock4DPadDirection.South);
                    if (ComboBoxboost.Text == "L1")
                        controller.SetButtonState(DualShock4Button.ShoulderLeft, true);
                    if (ComboBoxboost.Text == "R1")
                        controller.SetButtonState(DualShock4Button.ShoulderRight, true);
                    if (ComboBoxboost.Text == "L3")
                        controller.SetButtonState(DualShock4Button.ThumbLeft, true);
                    if (ComboBoxboost.Text == "R3")
                        controller.SetButtonState(DualShock4Button.ThumbRight, true);
                }

                if (Alwaysstickcheck.Checked == false)
                {
                    while (true)
                    {
                        if (ComboBoxhandle.Text == "方向キー")
                            controller.SetDPadDirection(DualShock4DPadDirection.East);
                        if (ComboBoxhandle.Text == "左スティック")
                            controller.SetAxisValue(DualShock4Axis.LeftThumbX, byte.MaxValue);
                        await Task.Delay(rightkey_down);
                        if (ComboBoxhandle.Text == "方向キー")
                            controller.SetDPadDirection(DualShock4DPadDirection.None);
                        if (ComboBoxhandle.Text == "左スティック")
                            controller.SetAxisValue(DualShock4Axis.LeftThumbX, 128);
                        await Task.Delay(rightkey_wait);
                        racefinishover = racefinishover + rightkey_down + rightkey_wait;
                        if (racefinishover > racefinish)
                            break;
                    }
                }

                else
                {
                    int angle128_int = (int)Stickangle.Value + 128;
                    byte angle128 = (byte)angle128_int;
                    controller.SetAxisValue(DualShock4Axis.LeftThumbX, angle128);
                    while (true)
                    {
                        await Task.Delay(1000);
                        racefinishover += 1000;
                        if (racefinishover > racefinish)
                        {
                            controller.SetAxisValue(DualShock4Axis.LeftThumbX, 128);
                            break;
                        }
                    }
                }

                //ブーストオフ
                if (ComboBoxboost.Text == "×")
                    controller.SetButtonState(DualShock4Button.Cross, false);
                if (ComboBoxboost.Text == "□")
                    controller.SetButtonState(DualShock4Button.Square, false);
                if (ComboBoxboost.Text == "△")
                    controller.SetButtonState(DualShock4Button.Triangle, false);
                if (ComboBoxboost.Text == "〇")
                    controller.SetButtonState(DualShock4Button.Circle, false);
                if (ComboBoxboost.Text == "↑" || ComboBoxboost.Text == "→" || ComboBoxboost.Text == "←" || ComboBoxboost.Text == "↓")
                    controller.SetDPadDirection(DualShock4DPadDirection.None);
                if (ComboBoxboost.Text == "L1")
                    controller.SetButtonState(DualShock4Button.ShoulderLeft, false);
                if (ComboBoxboost.Text == "R1")
                    controller.SetButtonState(DualShock4Button.ShoulderRight, false);
                if (ComboBoxboost.Text == "L3")
                    controller.SetButtonState(DualShock4Button.ThumbLeft, false);
                if (ComboBoxboost.Text == "R3")
                    controller.SetButtonState(DualShock4Button.ThumbRight, false);

                //アクセルオフ
                if (ComboBoxaccel.Text == "×")
                    controller.SetButtonState(DualShock4Button.Cross, false);
                if (ComboBoxaccel.Text == "□")
                    controller.SetButtonState(DualShock4Button.Square, false);
                if (ComboBoxaccel.Text == "△")
                    controller.SetButtonState(DualShock4Button.Triangle, false);
                if (ComboBoxaccel.Text == "〇")
                    controller.SetButtonState(DualShock4Button.Circle, false);
                if (ComboBoxaccel.Text == "↑" || ComboBoxaccel.Text == "→" || ComboBoxaccel.Text == "←" || ComboBoxaccel.Text == "↓")
                    controller.SetDPadDirection(DualShock4DPadDirection.None);
                if (ComboBoxaccel.Text == "L1")
                    controller.SetButtonState(DualShock4Button.ShoulderLeft, false);
                if (ComboBoxaccel.Text == "R1")
                    controller.SetButtonState(DualShock4Button.ShoulderRight, false);
                if (ComboBoxaccel.Text == "R2")
                {
                    controller.SetSliderValue(DualShock4Slider.RightTrigger, 0);
                    controller.SetButtonState(DualShock4Button.TriggerRight, false);
                }
                if (ComboBoxaccel.Text == "L3")
                    controller.SetButtonState(DualShock4Button.ThumbLeft, false);
                if (ComboBoxaccel.Text == "R3")
                    controller.SetButtonState(DualShock4Button.ThumbRight, false);
                if (ComboBoxaccel.Text == "右スティック")
                    controller.SetAxisValue(DualShock4Axis.RightThumbY, 128);

                if (Gettimecheck.Checked == true)
                {
                    var nowtime = DateTime.Now;
                    bool daycheck = (nowtime.Date == nowtime2.Date);
                    if (daycheck == false)
                    {
                        count_day = 0;
                        count += 1;
                    }
                }

                if (count_day > 3 && count < 3)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        await Task.Delay(780);
                        if (PS5check.Checked == true)
                            controller.SetButtonState(DualShock4Button.Cross, true);
                        else
                            controller.SetButtonState(DualShock4Button.Circle, true);
                        await Task.Delay(220);
                        if (PS5check.Checked == true)
                            controller.SetButtonState(DualShock4Button.Cross, false);
                        else
                            controller.SetButtonState(DualShock4Button.Circle, false);
                    }
                    count_day = 0;
                }

                if (count_day <= 3 || count >= 3)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        await Task.Delay(780);
                        if (PS5check.Checked == true)
                            controller.SetButtonState(DualShock4Button.Cross, true);
                        else
                            controller.SetButtonState(DualShock4Button.Circle, true);
                        await Task.Delay(220);
                        if (PS5check.Checked == true)
                            controller.SetButtonState(DualShock4Button.Cross, false);
                        else
                            controller.SetButtonState(DualShock4Button.Circle, false);
                    }
                    await Task.Delay(4500);
                    if (PS5check.Checked == true)
                        controller.SetButtonState(DualShock4Button.Cross, true);
                    else
                        controller.SetButtonState(DualShock4Button.Circle, true);
                    await Task.Delay(220);
                    if (PS5check.Checked == true)
                        controller.SetButtonState(DualShock4Button.Cross, false);
                    else
                        controller.SetButtonState(DualShock4Button.Circle, false);
                    await Task.Delay(1000);
                    if (PS5check.Checked == true)
                        controller.SetButtonState(DualShock4Button.Cross, true);
                    else
                        controller.SetButtonState(DualShock4Button.Circle, true);
                    await Task.Delay(220);
                    if (PS5check.Checked == true)
                        controller.SetButtonState(DualShock4Button.Cross, false);
                    else
                        controller.SetButtonState(DualShock4Button.Circle, false);
                    nowtime2 = DateTime.Now;
                    count = 0;
                }

                await Task.Delay(wait_replay);

                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);
                await Task.Delay(500);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);

                await Task.Delay(wait_racemenu);

                controller.SetDPadDirection(DualShock4DPadDirection.East);
                await Task.Delay(220);
                controller.SetDPadDirection(DualShock4DPadDirection.None);
                await Task.Delay(500);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);

                await Task.Delay(wait_entry2);

                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);
                await Task.Delay(1000);

                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Circle, true);
                else
                    controller.SetButtonState(DualShock4Button.Cross, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Circle, false);
                else
                    controller.SetButtonState(DualShock4Button.Cross, false);
                await Task.Delay(500);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);
                await Task.Delay(500);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, true);
                else
                    controller.SetButtonState(DualShock4Button.Circle, true);
                await Task.Delay(220);
                if (PS5check.Checked == true)
                    controller.SetButtonState(DualShock4Button.Cross, false);
                else
                    controller.SetButtonState(DualShock4Button.Circle, false);

                await Task.Delay(wait_backtomenu);

                if (stop == false)
                    goto label1;

                else if (stop == true)
                    goto label2;

                label2:
                if (accel == "" && handle == "")
                    MessageBox.Show("アクセルとハンドルにボタンがセットされていません！");
                if (accel == "" && handle != "")
                    MessageBox.Show("アクセルにボタンがセットされていません！");
                if (accel != "" && handle == "")
                    MessageBox.Show("ハンドルにボタンがセットされていません！");
                stop = false;
                controller.Disconnect();
                Stop.Text = "停止";
                Workmessage.Text = "現在実行されていません";
            }

            catch (Nefarius.ViGEm.Client.Exceptions.VigemBusNotFoundException)
            {
                MessageBox.Show("ViGEmBusをインストールしてね！\nreadmeにURL載せてあります");
            }
        }

        private void Modosu_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("全ての設定を初期化しますか？", "", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                Waitentry.Value = 12;
                Waitstart.Value = 1;
                Waitreplay.Value = Convert.ToDecimal(1.5);
                Waitracemenu.Value = Convert.ToDecimal(1.5);
                Waitentry2.Value = 14;
                Waitbacktomenu.Value = 8;
                Racefinishmin.Value = 4;
                Racefinishsec.Value = 2;
                Rightkeydown.Value = Convert.ToDecimal(0.3);
                Rightkeywait.Value = Convert.ToDecimal(0.6);
                PS5check.Checked = true;
                ComboBoxaccel.Text = "R2";
                ComboBoxhandle.Text = "左スティック";
                ComboBoxboost.Text = "R1";
                Alwaysstickcheck.Checked = true;
                Stickangle.Value = 30;
                Gettimecheck.Checked = true;

                MessageBox.Show("初期化しました", "", MessageBoxButtons.OK);
            }
            else if (result == DialogResult.Cancel)
            {

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void Settingexport_Click(object sender, EventArgs e)
        {
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            appPath = Path.GetDirectoryName(appPath);
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.FileName = "setting.txt";
            sfd.InitialDirectory = appPath + @"\";
            sfd.Filter = "txtファイル(*.txt)|*.txt|すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            sfd.FilterIndex = 1;
            //タイトルを設定する
            sfd.Title = "設定をエクスポート";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Stream stream;
                stream = sfd.OpenFile();
                if (stream != null)
                {
                    StreamWriter sw = new StreamWriter(stream);
                    sw.WriteLine("エントリー後：" + Waitentry.Value + "秒");
                    sw.WriteLine("スタートボタンを押した後：" + Waitstart.Value + "秒");
                    sw.WriteLine("賞金獲得～リプレイ：" + Waitreplay.Value + "秒");
                    sw.WriteLine("リプレイ～レースメニュー：" + Waitracemenu.Value + "秒");
                    sw.WriteLine("2戦目ロード：" + Waitentry2.Value + "秒");
                    sw.WriteLine("2戦目棄権～メニュー：" + Waitbacktomenu.Value + "秒");
                    sw.WriteLine("完走タイム：" + Racefinishmin.Value + "分" + Racefinishsec.Value + "秒");
                    sw.Write("×ボタンで決定：");
                    if (PS5check.Checked == true)
                        sw.WriteLine("ON");
                    else
                        sw.WriteLine("OFF");
                    if (ComboBoxaccel.Text != "")
                    {
                        sw.Write("アクセル：");
                        sw.WriteLine(ComboBoxaccel.Text);
                    }
                    if (ComboBoxhandle.Text != "")
                    {
                        sw.Write("ハンドル：");
                        sw.WriteLine(ComboBoxhandle.Text);
                    }
                    if (ComboBoxboost.Text != "")
                    {
                        sw.Write("ブースト：");
                        sw.WriteLine(ComboBoxboost.Text);
                    }
                    sw.Write("スティックを常に倒す：");
                    if (Alwaysstickcheck.Checked == true)
                        sw.WriteLine("ON");
                    else
                        sw.WriteLine("OFF");
                    sw.WriteLine("スティックの角度：" + Stickangle.Value);
                    sw.WriteLine("右キー入力秒：" + Rightkeydown.Value + "秒");
                    sw.WriteLine("右キーの間隔：" + Rightkeywait.Value + "秒");
                    sw.Write("時刻取得：");
                    if (Gettimecheck.Checked == true)
                        sw.Write("ON");
                    else
                        sw.Write("OFF");
                    sw.Close();
                    stream.Close();
                }
            }
        }

        private void Settingimport_Click(object sender, EventArgs e)
        {
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            appPath = Path.GetDirectoryName(appPath);
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.FileName = "";
            ofd.InitialDirectory = appPath + @"\";
            ofd.Filter = "txtファイル(*.txt)|*.txt|gbtファイル(*.gbt)|*.gbt|すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            ofd.FilterIndex = 1;
            //タイトルを設定する
            ofd.Title = "設定をインポート";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;

            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Stream stream;
                stream = ofd.OpenFile();

                if (stream != null)
                {
                    StreamReader sr = new StreamReader(stream);
                    string line = sr.ReadLine();
                    decimal linedec = 0;

                    while (line != null)
                    {
                        if (0 <= line.IndexOf("エントリー後："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            line = line.Replace("秒", "");
                            linedec = decimal.Parse(line);
                            if (linedec > 10000)
                                linedec = 10000;
                            if (linedec < 1)
                                linedec = 1;
                            Waitentry.Value = linedec;
                        }
                        
                        else if (0 <= line.IndexOf("スタートボタンを押した後："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            line = line.Replace("秒", "");
                            linedec = decimal.Parse(line);
                            if (linedec > 10000)
                                linedec = 10000;
                            if (linedec < 1)
                                linedec = 1;
                            Waitstart.Value = linedec;
                        }

                        else if (0 <= line.IndexOf("賞金獲得～リプレイ："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            line = line.Replace("秒", "");
                            linedec = decimal.Parse(line);
                            if (linedec > 10000)
                                linedec = 10000;
                            if (linedec < 1)
                                linedec = 1;
                            Waitreplay.Value = linedec;
                        }

                        else if (0 <= line.IndexOf("リプレイ～レースメニュー："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            line = line.Replace("秒", "");
                            linedec = decimal.Parse(line);
                            if (linedec > 10000)
                                linedec = 10000;
                            if (linedec < 1)
                                linedec = 1;
                            Waitracemenu.Value = linedec;
                        }

                        else if (0 <= line.IndexOf("2戦目ロード："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            line = line.Replace("秒", "");
                            linedec = decimal.Parse(line);
                            if (linedec > 10000)
                                linedec = 10000;
                            if (linedec < 1)
                                linedec = 1;
                            Waitentry2.Value = linedec;
                        }

                        else if (0 <= line.IndexOf("2戦目棄権～メニュー："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            line = line.Replace("秒", "");
                            linedec = decimal.Parse(line);
                            if (linedec > 10000)
                                linedec = 10000;
                            if (linedec < 1)
                                linedec = 1;
                            Waitbacktomenu.Value = linedec;
                        }

                        else if (0 <= line.IndexOf("完走タイム："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            line = line.Replace("秒", "");
                            string line2 = line;
                            line = line.Remove(line.IndexOf("分"));
                            line2 = line2.Remove(0, line2.IndexOf("分") + 1);
                            linedec = decimal.Parse(line);
                            if (linedec > 10000)
                                linedec = 10000;
                            decimal linedec2 = decimal.Parse(line2);
                            if (linedec2 > 59)
                                linedec2 = 59;
                            Racefinishmin.Value = linedec;
                            Racefinishsec.Value = linedec2;
                        }

                        else if (0 <= line.IndexOf("PS5：") || 0 <= line.IndexOf("×ボタンで決定："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            if (line == "ON" || line == "on" || line == "オン")
                                PS5check.Checked = true;
                        }

                        else if (0 <= line.IndexOf("アクセル："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            if (line != "")
                                ComboBoxaccel.Text = line;
                        }

                        else if (0 <= line.IndexOf("ハンドル："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            if (line != "")
                                ComboBoxhandle.Text = line;
                        }

                        else if (0 <= line.IndexOf("ブースト："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            if (line != "")
                                ComboBoxboost.Text = line;
                        }

                        else if (0 <= line.IndexOf("スティックを常に倒す："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            if (line == "ON" || line == "on" || line == "オン")
                                Alwaysstickcheck.Checked = true;
                        }

                        else if (0 <= line.IndexOf("スティックの角度："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            linedec = decimal.Parse(line);
                            if (linedec > 127)
                                linedec = 127;
                            Stickangle.Value = linedec;
                        }

                        else if (0 <= line.IndexOf("右キー入力秒："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            line = line.Replace("秒", "");
                            linedec = decimal.Parse(line);
                            if (linedec > 10000)
                                linedec = 10000;
                            Rightkeydown.Value = linedec;
                        }

                        else if (0 <= line.IndexOf("右キーの間隔："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            line = line.Replace("秒", "");
                            linedec = decimal.Parse(line);
                            if (linedec > 10000)
                                linedec = 10000;
                            Rightkeywait.Value = linedec;
                        }

                        else if (0 <= line.IndexOf("時刻取得："))
                        {
                            line = line.Remove(0, line.IndexOf("：") + 1);
                            if (line == "ON" || line == "on" || line == "オン")
                                Gettimecheck.Checked = true;
                        }

                        line = sr.ReadLine();
                    }

                    //閉じる
                    sr.Close();
                    stream.Close();
                }
            }
        }
    }
}
