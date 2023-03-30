using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GT7_B_specツール_プラグイン無し
{
    public partial class Form1 : Form
    {
        public static bool stop = false;

        public Form1()
        {
            InitializeComponent();
        }

        // マウスイベント(mouse_eventの引数と同様のデータ)
        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public int dwExtraInfo;
        };

        // キーボードイベント(keybd_eventの引数と同様のデータ)
        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public int dwExtraInfo;
        };

        // ハードウェアイベント
        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        };

        // 各種イベント(SendInputの引数データ)
        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT
        {
            [FieldOffset(0)] public int type;
            [FieldOffset(4)] public MOUSEINPUT mi;
            [FieldOffset(4)] public KEYBDINPUT ki;
            [FieldOffset(4)] public HARDWAREINPUT hi;
        };

        // キー操作、マウス操作をシミュレート(擬似的に操作する)
        [DllImport("user32.dll")]
        private extern static void SendInput(
            int nInputs, ref INPUT pInputs, int cbsize);

        // 仮想キーコードをスキャンコードに変換
        [DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
        private extern static int MapVirtualKey(
            int wCode, int wMapType);

        private const int INPUT_MOUSE = 0;                  // マウスイベント
        private const int INPUT_KEYBOARD = 1;               // キーボードイベント
        private const int INPUT_HARDWARE = 2;               // ハードウェアイベント

        public const int MOUSEEVENTF_MOVE = 0x1;
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        public const int MOUSEEVENTF_LEFTDOWN = 0x2;
        public const int MOUSEEVENTF_LEFTUP = 0x4;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x8;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        public const int MOUSEEVENTF_MIDDLEUP = 0x40;
        public const int MOUSEEVENTF_WHEEL = 0x800;
        public const int WHEEL_DELTA = 120;

        private const int KEYEVENTF_KEYDOWN = 0x0;          // キーを押す
        private const int KEYEVENTF_KEYUP = 0x2;            // キーを離す
        private const int KEYEVENTF_EXTENDEDKEY = 0x1;      // 拡張コード
        private const int VK_ENTER = 0x0D;                  // ENTERキー
        private const int VK_RIGHT = 0x27;                  // RIGHTキー
        private const int VK_UP = 0x26;                     // UPキー
        private const int VK_LEFT = 0x25;                   // LEFTキー
        private const int VK_DOWN = 0x28;                   // DOWNキー
        private const int VK_ESCAPE = 0xF3;                 // ESCキー

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private async void Start_Click(object sender, EventArgs e)
        {
            stop = false;
            Workmessage.Text = "実行中...";
            decimal wait_start_dec = Waitstart.Value;
            decimal wait_replay_dec = Waitreplay.Value;
            decimal wait_racemenu_dec = Waitracemenu.Value;
            decimal wait_entry2_dec = Waitentry2.Value;
            int racefinish_min = 0;
            int racefinish_sec = 0;
            int racefinish = 0;
            int racefinishover = 0;
            wait_start_dec = wait_start_dec * 1000;
            wait_replay_dec = wait_replay_dec * 1000;
            wait_racemenu_dec = wait_racemenu_dec * 1000;
            wait_entry2_dec = wait_entry2_dec * 1000;
            int wait_start = decimal.ToInt32(wait_start_dec);
            int wait_replay = decimal.ToInt32(wait_replay_dec);
            int wait_racemenu = decimal.ToInt32(wait_racemenu_dec);
            int wait_entry2 = decimal.ToInt32(wait_entry2_dec);

            const int numenter = 1;
            INPUT[] inp = new INPUT[numenter];

            inp[0].type = INPUT_KEYBOARD;
            inp[0].ki.wVk = VK_ENTER;
            inp[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp[0].ki.dwExtraInfo = 0;
            inp[0].ki.time = 0;

            const int numenterup = 1;
            INPUT[] inp2 = new INPUT[numenterup];

            inp2[0].type = INPUT_KEYBOARD;
            inp2[0].ki.wVk = VK_ENTER;
            inp2[0].ki.wScan = (short)MapVirtualKey(inp2[0].ki.wVk, 0);
            inp2[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp2[0].ki.dwExtraInfo = 0;
            inp2[0].ki.time = 0;

            const int numleft = 1;
            INPUT[] inp3 = new INPUT[numleft];

            inp3[0].type = INPUT_KEYBOARD;
            inp3[0].ki.wVk = VK_LEFT;
            inp3[0].ki.wScan = (short)MapVirtualKey(inp3[0].ki.wVk, 0);
            inp3[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp3[0].ki.dwExtraInfo = 0;
            inp3[0].ki.time = 0;

            const int numleftup = 1;
            INPUT[] inp4 = new INPUT[numleftup];

            inp4[0].type = INPUT_KEYBOARD;
            inp4[0].ki.wVk = VK_LEFT;
            inp4[0].ki.wScan = (short)MapVirtualKey(inp4[0].ki.wVk, 0);
            inp4[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp4[0].ki.dwExtraInfo = 0;
            inp4[0].ki.time = 0;

            const int numright = 1;
            INPUT[] inp5 = new INPUT[numright];

            inp5[0].type = INPUT_KEYBOARD;
            inp5[0].ki.wVk = VK_RIGHT;
            inp5[0].ki.wScan = (short)MapVirtualKey(inp5[0].ki.wVk, 0);
            inp5[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp5[0].ki.dwExtraInfo = 0;
            inp5[0].ki.time = 0;


            const int numrightup = 1;
            INPUT[] inp6 = new INPUT[numrightup];

            inp6[0].type = INPUT_KEYBOARD;
            inp6[0].ki.wVk = VK_RIGHT;
            inp6[0].ki.wScan = (short)MapVirtualKey(inp6[0].ki.wVk, 0);
            inp6[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp6[0].ki.dwExtraInfo = 0;
            inp6[0].ki.time = 0;

            await Task.Delay(500);

        label1:
            foreach (System.Diagnostics.Process p
            in System.Diagnostics.Process.GetProcesses())
            {
                if (0 <= p.MainWindowTitle.IndexOf("PS Remote Play"))
                {
                    SetForegroundWindow(p.MainWindowHandle);
                    break;
                }
            }

            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_start);

            await Task.Delay(7000);

            racefinishover = 0;
            racefinish_min = (int)Racefinishmin.Value;
            racefinish_sec = (int)Racefinishsec.Value;
            racefinish_min = racefinish_min * 60;
            racefinish = racefinish_min + racefinish_sec;
            racefinish = racefinish * 1000;

            while (true)
            {
                await Task.Delay(1000);
                racefinishover = racefinishover + 1000;
                if (racefinishover >= racefinish)
                    break;
            }

            int entercount = 4;
            if (enter1plus.Checked == true)
                entercount += 1;
            for (int i = 0; i < entercount; i++)
            {
                await Task.Delay(780);
                SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
                await Task.Delay(220);
                SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));
            }
            await Task.Delay(780);
            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_replay);

            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(780);
            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_racemenu);

            SendInput(numright, ref inp5[0], Marshal.SizeOf(inp5[0]));
            await Task.Delay(220);
            SendInput(numrightup, ref inp6[0], Marshal.SizeOf(inp6[0]));
            await Task.Delay(500);
            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_entry2);

            if (stop == false)
                goto label1;
            else if (stop == true)
                goto label2;

        label2:
            stop = false;
            Workmessage.Text = "現在実行されていません";
        }

        private async void Stop_Click(object sender, EventArgs e)
        {
            Workmessage.Text = "停止中。レース終了までお待ちください";
            stop = true;
            await Task.Delay(500);
            foreach (System.Diagnostics.Process p
            in System.Diagnostics.Process.GetProcesses())
            {
                if (0 <= p.MainWindowTitle.IndexOf("PS Remote Play"))
                {
                    SetForegroundWindow(p.MainWindowHandle);
                    break;
                }
            }
        }

        private async void Start_notdaily_Click(object sender, EventArgs e)
        {
            stop = false;
            Workmessage.Text = "実行中...";
            decimal wait_start_dec = Waitstart.Value;
            decimal wait_replay_dec = Waitreplay.Value;
            decimal wait_racemenu_dec = Waitracemenu.Value;
            decimal wait_entry2_dec = Waitentry2.Value;
            int racefinish_min = 0;
            int racefinish_sec = 0;
            int racefinish = 0;
            int racefinishover = 0;
            wait_start_dec = wait_start_dec * 1000;
            wait_replay_dec = wait_replay_dec * 1000;
            wait_racemenu_dec = wait_racemenu_dec * 1000;
            wait_entry2_dec = wait_entry2_dec * 1000;
            int wait_start = decimal.ToInt32(wait_start_dec);
            int wait_replay = decimal.ToInt32(wait_replay_dec);
            int wait_racemenu = decimal.ToInt32(wait_racemenu_dec);
            int wait_entry2 = decimal.ToInt32(wait_entry2_dec);
            int count = 0;

            const int numenter = 1;
            INPUT[] inp = new INPUT[numenter];

            inp[0].type = INPUT_KEYBOARD;
            inp[0].ki.wVk = VK_ENTER;
            inp[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp[0].ki.dwExtraInfo = 0;
            inp[0].ki.time = 0;

            const int numenterup = 1;
            INPUT[] inp2 = new INPUT[numenterup];

            inp2[0].type = INPUT_KEYBOARD;
            inp2[0].ki.wVk = VK_ENTER;
            inp2[0].ki.wScan = (short)MapVirtualKey(inp2[0].ki.wVk, 0);
            inp2[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp2[0].ki.dwExtraInfo = 0;
            inp2[0].ki.time = 0;

            const int numleft = 1;
            INPUT[] inp3 = new INPUT[numleft];

            inp3[0].type = INPUT_KEYBOARD;
            inp3[0].ki.wVk = VK_LEFT;
            inp3[0].ki.wScan = (short)MapVirtualKey(inp3[0].ki.wVk, 0);
            inp3[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp3[0].ki.dwExtraInfo = 0;
            inp3[0].ki.time = 0;

            const int numleftup = 1;
            INPUT[] inp4 = new INPUT[numleftup];

            inp4[0].type = INPUT_KEYBOARD;
            inp4[0].ki.wVk = VK_LEFT;
            inp4[0].ki.wScan = (short)MapVirtualKey(inp4[0].ki.wVk, 0);
            inp4[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp4[0].ki.dwExtraInfo = 0;
            inp4[0].ki.time = 0;

            const int numright = 1;
            INPUT[] inp5 = new INPUT[numright];

            inp5[0].type = INPUT_KEYBOARD;
            inp5[0].ki.wVk = VK_RIGHT;
            inp5[0].ki.wScan = (short)MapVirtualKey(inp5[0].ki.wVk, 0);
            inp5[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp5[0].ki.dwExtraInfo = 0;
            inp5[0].ki.time = 0;


            const int numrightup = 1;
            INPUT[] inp6 = new INPUT[numrightup];

            inp6[0].type = INPUT_KEYBOARD;
            inp6[0].ki.wVk = VK_RIGHT;
            inp6[0].ki.wScan = (short)MapVirtualKey(inp6[0].ki.wVk, 0);
            inp6[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp6[0].ki.dwExtraInfo = 0;
            inp6[0].ki.time = 0;

            await Task.Delay(500);

        label1:
            foreach (System.Diagnostics.Process p
            in System.Diagnostics.Process.GetProcesses())
            {
                if (0 <= p.MainWindowTitle.IndexOf("PS Remote Play"))
                {
                    SetForegroundWindow(p.MainWindowHandle);
                    break;
                }
            }

            count = count + 1;

            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_start);

            await Task.Delay(7000);

            racefinishover = 0;
            racefinish_min = (int)Racefinishmin.Value;
            racefinish_sec = (int)Racefinishsec.Value;
            racefinish_min = racefinish_min * 60;
            racefinish = racefinish_min + racefinish_sec;
            racefinish = racefinish * 1000;

            while (true)
            {
                await Task.Delay(1000);
                racefinishover = racefinishover + 1000;
                if (racefinishover >= racefinish)
                    break;
            }

            int entercount = 3;
            if (enter1plus.Checked == true)
                entercount += 1;
            for (int i = 0; i < entercount; i++)
            {
                await Task.Delay(780);
                SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
                await Task.Delay(220);
                SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));
            }

            if (count <= 2)
                await Task.Delay(5000);
            else
                await Task.Delay(780);

            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(780);
            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_replay);

            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(780);
            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_racemenu);

            SendInput(numright, ref inp5[0], Marshal.SizeOf(inp5[0]));
            await Task.Delay(220);
            SendInput(numrightup, ref inp6[0], Marshal.SizeOf(inp6[0]));
            await Task.Delay(500);
            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_entry2);

            if (stop == false)
            goto label1;
            else if (stop == true)
            goto label2;

        label2:
            stop = false;
            Workmessage.Text = "現在実行されていません";
        }

        private async void Start_fulldaily_Click(object sender, EventArgs e)
        {
            stop = false;
            Workmessage.Text = "実行中...";
            decimal wait_start_dec = Waitstart.Value;
            decimal wait_replay_dec = Waitreplay.Value;
            decimal wait_racemenu_dec = Waitracemenu.Value;
            decimal wait_entry2_dec = Waitentry2.Value;
            int racefinish_min = 0;
            int racefinish_sec = 0;
            int racefinish = 0;
            int racefinishover = 0;
            wait_start_dec = wait_start_dec * 1000;
            wait_replay_dec = wait_replay_dec * 1000;
            wait_racemenu_dec = wait_racemenu_dec * 1000;
            wait_entry2_dec = wait_entry2_dec * 1000;
            int wait_start = decimal.ToInt32(wait_start_dec);
            int wait_replay = decimal.ToInt32(wait_replay_dec);
            int wait_racemenu = decimal.ToInt32(wait_racemenu_dec);
            int wait_entry2 = decimal.ToInt32(wait_entry2_dec);

            await Task.Delay(500);

            const int numenter = 1;
            INPUT[] inp = new INPUT[numenter];

            inp[0].type = INPUT_KEYBOARD;
            inp[0].ki.wVk = VK_ENTER;
            inp[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp[0].ki.dwExtraInfo = 0;
            inp[0].ki.time = 0;

            const int numenterup = 1;
            INPUT[] inp2 = new INPUT[numenterup];

            inp2[0].type = INPUT_KEYBOARD;
            inp2[0].ki.wVk = VK_ENTER;
            inp2[0].ki.wScan = (short)MapVirtualKey(inp2[0].ki.wVk, 0);
            inp2[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp2[0].ki.dwExtraInfo = 0;
            inp2[0].ki.time = 0;

            const int numleft = 1;
            INPUT[] inp3 = new INPUT[numleft];

            inp3[0].type = INPUT_KEYBOARD;
            inp3[0].ki.wVk = VK_LEFT;
            inp3[0].ki.wScan = (short)MapVirtualKey(inp3[0].ki.wVk, 0);
            inp3[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp3[0].ki.dwExtraInfo = 0;
            inp3[0].ki.time = 0;

            const int numleftup = 1;
            INPUT[] inp4 = new INPUT[numleftup];

            inp4[0].type = INPUT_KEYBOARD;
            inp4[0].ki.wVk = VK_LEFT;
            inp4[0].ki.wScan = (short)MapVirtualKey(inp4[0].ki.wVk, 0);
            inp4[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp4[0].ki.dwExtraInfo = 0;
            inp4[0].ki.time = 0;

            const int numright = 1;
            INPUT[] inp5 = new INPUT[numright];

            inp5[0].type = INPUT_KEYBOARD;
            inp5[0].ki.wVk = VK_RIGHT;
            inp5[0].ki.wScan = (short)MapVirtualKey(inp5[0].ki.wVk, 0);
            inp5[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp5[0].ki.dwExtraInfo = 0;
            inp5[0].ki.time = 0;


            const int numrightup = 1;
            INPUT[] inp6 = new INPUT[numrightup];

            inp6[0].type = INPUT_KEYBOARD;
            inp6[0].ki.wVk = VK_RIGHT;
            inp6[0].ki.wScan = (short)MapVirtualKey(inp6[0].ki.wVk, 0);
            inp6[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp6[0].ki.dwExtraInfo = 0;
            inp6[0].ki.time = 0;

        label1:
            foreach (System.Diagnostics.Process p
            in System.Diagnostics.Process.GetProcesses())
            {
                if (0 <= p.MainWindowTitle.IndexOf("PS Remote Play"))
                {
                    SetForegroundWindow(p.MainWindowHandle);
                    break;
                }
            }

            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_start);

            await Task.Delay(7000);

            racefinishover = 0;
            racefinish_min = (int)Racefinishmin.Value;
            racefinish_sec = (int)Racefinishsec.Value;
            racefinish_min = racefinish_min * 60;
            racefinish = racefinish_min + racefinish_sec;
            racefinish = racefinish * 1000;

            while (true)
            {
                await Task.Delay(1000);
                racefinishover = racefinishover + 1000;
                if (racefinishover >= racefinish)
                    break;
            }

            int entercount = 3;
            if (enter1plus.Checked == true)
                entercount += 1;
            for (int i = 0; i < entercount; i++)
            {
                await Task.Delay(780);
                SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
                await Task.Delay(220);
                SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));
            }

            await Task.Delay(5000);

            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(780);
            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_replay);

            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(780);
            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_racemenu);

            SendInput(numright, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numrightup, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(500);
            SendInput(numenter, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(numenterup, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_entry2);

            if (stop == false)
            goto label1;
            else if (stop == true)
            goto label2;

        label2:
            stop = false;
            Workmessage.Text = "現在実行されていません";
        }

        private void Modosu_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("全ての設定を初期化しますか？", "", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                Waitstart.Value = 1;
                Waitreplay.Value = 2;
                Waitracemenu.Value = 2;
                Waitentry2.Value = 2;
                Racefinishmin.Value = 5;
                Racefinishsec.Value = 10;
                enter1plus.Checked = false;

                MessageBox.Show("初期化しました", "", MessageBoxButtons.OK);
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
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(stream);
                    sw.WriteLine("スタートボタンを押した後：" + Waitstart.Value + "秒");
                    sw.WriteLine("賞金獲得～リプレイ：" + Waitreplay.Value + "秒");
                    sw.WriteLine("リプレイ～レースメニュー：" + Waitracemenu.Value + "秒");
                    sw.WriteLine("2戦目ロード：" + Waitentry2.Value + "秒");
                    sw.WriteLine("リザルトまで：" + Racefinishmin.Value + "分" + Racefinishsec.Value + "秒");
                    if (enter1plus.Checked == true)
                        sw.WriteLine("レース終了後の決定ボタン入力を1回増やす：" + "ON");
                    else
                        sw.WriteLine("レース終了後の決定ボタン入力を1回増やす：" + "OFF");
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
            ofd.Filter = "txtファイル(*.txt)|*.txt|すべてのファイル(*.*)|*.*";
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
                    string line = "";
                    decimal linedec = 0;

                    line = sr.ReadLine();
                    line = line.Remove(0, line.IndexOf("：") + 1);
                    line = line.Replace("秒", ""); 
                    linedec = decimal.Parse(line);
                    if (linedec > 10000)
                        linedec = 10000;
                    Waitstart.Value = linedec;

                    line = sr.ReadLine();
                    line = line.Remove(0, line.IndexOf("：") + 1);
                    line = line.Replace("秒", "");
                    linedec = decimal.Parse(line);
                    if (linedec > 10000)
                        linedec = 10000;
                    Waitreplay.Value = linedec;

                    line = sr.ReadLine();
                    line = line.Remove(0, line.IndexOf("：") + 1);
                    line = line.Replace("秒", "");
                    linedec = decimal.Parse(line);
                    if (linedec > 10000)
                        linedec = 10000;
                    Waitracemenu.Value = linedec;

                    line = sr.ReadLine();
                    line = line.Remove(0, line.IndexOf("：") + 1);
                    line = line.Replace("秒", "");
                    linedec = decimal.Parse(line);
                    if (linedec > 10000)
                        linedec = 10000;
                    Waitentry2.Value = linedec;

                    line = sr.ReadLine();
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

                    line = sr.ReadLine();
                    line = line.Remove(0, line.IndexOf("：") + 1);
                    if (line == "ON" || line == "on" || line == "オン")
                        enter1plus.Checked = true;

                    //閉じる
                    sr.Close();
                    stream.Close();
                }
            }
        }
    }
}
