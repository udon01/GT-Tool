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

        private const int KEYEVENTF_KEYDOWN = 0x0;          // キーを押す
        private const int KEYEVENTF_KEYUP = 0x2;            // キーを離す
        private const int KEYEVENTF_EXTENDEDKEY = 0x1;      // 拡張コード
        private const int VK_ENTER = 0x0D;                  // ENTERキー
        private const int VK_ESC = 0x1B;                    // ESCキー
        private const int VK_RIGHT = 0x27;                  // RIGHTキー
        private const int VK_LEFT = 0x25;                   // LEFTキー
        private const int VK_DOWN = 0x28;                   // DOWNキー

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private async void Start_Click(object sender, EventArgs e)
        {
            stop = false;
            Workmessage.Text = "実行中...";
            decimal wait_entry_dec = Waitentry.Value;
            decimal wait_start_dec = Waitstart.Value;
            decimal wait_replay_dec = Waitreplay.Value;
            decimal wait_racemenu_dec = Waitracemenu.Value;
            decimal wait_entry2_dec = Waitentry2.Value;
            decimal wait_backtomenu_dec = Waitbacktomenu.Value;
            int racefinish_min = decimal.ToInt32(Racefinishmin.Value);
            int racefinish_sec = decimal.ToInt32(Racefinishsec.Value);
            decimal rightkey_down_dec = Rightkeydown.Value;
            decimal rightkey_wait_dec = Rightkeywait.Value;
            int racefinishover = 0;
            wait_entry_dec = wait_entry_dec * 1000;
            wait_start_dec = wait_start_dec * 1000;
            wait_replay_dec = wait_replay_dec * 1000;
            wait_racemenu_dec = wait_racemenu_dec * 1000;
            wait_entry2_dec = wait_entry2_dec * 1000;
            wait_backtomenu_dec = wait_backtomenu_dec * 1000;
            rightkey_down_dec = rightkey_down_dec * 1000;
            rightkey_wait_dec = rightkey_wait_dec * 1000;
            racefinish_min = racefinish_min * 60;
            int racefinish = racefinish_min + racefinish_sec;
            racefinish = racefinish * 1000;
            int wait_entry = decimal.ToInt32(wait_entry_dec);
            int wait_start = decimal.ToInt32(wait_start_dec);
            int wait_replay = decimal.ToInt32(wait_replay_dec);
            int wait_racemenu = decimal.ToInt32(wait_racemenu_dec);
            int wait_entry2 = decimal.ToInt32(wait_entry2_dec);
            int wait_backtomenu = decimal.ToInt32(wait_backtomenu_dec);
            int rightkey_down = decimal.ToInt32(rightkey_down_dec);
            int rightkey_wait = decimal.ToInt32(rightkey_wait_dec);
            await Task.Delay(500);

            const int num = 1;
            INPUT[] inp = new INPUT[num];

            inp[0].type = INPUT_KEYBOARD;
            inp[0].ki.wVk = VK_ENTER;
            inp[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp[0].ki.dwExtraInfo = 0;
            inp[0].ki.time = 0;

            const int num2 = 1;
            INPUT[] inp2 = new INPUT[num];

            inp2[0].type = INPUT_KEYBOARD;
            inp2[0].ki.wVk = VK_ENTER;
            inp2[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp2[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp2[0].ki.dwExtraInfo = 0;
            inp2[0].ki.time = 0;

            const int num3 = 1;
            INPUT[] inp3 = new INPUT[num];

            inp3[0].type = INPUT_KEYBOARD;
            inp3[0].ki.wVk = VK_LEFT;
            inp3[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp3[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp3[0].ki.dwExtraInfo = 0;
            inp3[0].ki.time = 0;

            const int num4 = 1;
            INPUT[] inp4 = new INPUT[num];

            inp4[0].type = INPUT_KEYBOARD;
            inp4[0].ki.wVk = VK_LEFT;
            inp4[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp4[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp4[0].ki.dwExtraInfo = 0;
            inp4[0].ki.time = 0;

            const int num5 = 1;
            INPUT[] inp5 = new INPUT[num];

            inp5[0].type = INPUT_KEYBOARD;
            inp5[0].ki.wVk = VK_RIGHT;
            inp5[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp5[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp5[0].ki.dwExtraInfo = 0;
            inp5[0].ki.time = 0;


            const int num6 = 1;
            INPUT[] inp6 = new INPUT[num];

            inp6[0].type = INPUT_KEYBOARD;
            inp6[0].ki.wVk = VK_RIGHT;
            inp6[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp6[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp6[0].ki.dwExtraInfo = 0;
            inp6[0].ki.time = 0;

            const int num7 = 1;
            INPUT[] inp7 = new INPUT[num];

            inp7[0].type = INPUT_KEYBOARD;
            inp7[0].ki.wVk = VK_DOWN;
            inp7[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp7[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp7[0].ki.dwExtraInfo = 0;
            inp7[0].ki.time = 0;

            const int num8 = 1;
            INPUT[] inp8 = new INPUT[num];

            inp8[0].type = INPUT_KEYBOARD;
            inp8[0].ki.wVk = VK_DOWN;
            inp8[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp8[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp8[0].ki.dwExtraInfo = 0;
            inp8[0].ki.time = 0;

            const int num9 = 1;
            INPUT[] inp9 = new INPUT[num];

            inp9[0].type = INPUT_KEYBOARD;
            inp9[0].ki.wVk = VK_ESC;
            inp9[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp9[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp9[0].ki.dwExtraInfo = 0;
            inp9[0].ki.time = 0;

            const int num10 = 1;
            INPUT[] inp10 = new INPUT[num];

            inp10[0].type = INPUT_KEYBOARD;
            inp10[0].ki.wVk = VK_ESC;
            inp10[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp10[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp10[0].ki.dwExtraInfo = 0;
            inp10[0].ki.time = 0;

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

            SendInput(num7, ref inp7[0], Marshal.SizeOf(inp7[0]));
            await Task.Delay(220);
            SendInput(num8, ref inp8[0], Marshal.SizeOf(inp8[0]));

            for (int i = 0; i < 6; i++)
            {
                await Task.Delay(220);
                SendInput(num5, ref inp5[0], Marshal.SizeOf(inp5[0]));
                await Task.Delay(220);
                SendInput(num6, ref inp6[0], Marshal.SizeOf(inp6[0]));
            }

            await Task.Delay(300);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(800);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_entry);

            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(1500);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_start);

            await Task.Delay(14000);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            SendInput(num5, ref inp5[0], Marshal.SizeOf(inp5[0]));

            await Task.Delay(2000);

            SendInput(num6, ref inp6[0], Marshal.SizeOf(inp6[0]));

            await Task.Delay(500);

            racefinishover = 0;
            while (true)
            {
                SendInput(num5, ref inp5[0], Marshal.SizeOf(inp5[0]));
                await Task.Delay(rightkey_down);
                SendInput(num6, ref inp6[0], Marshal.SizeOf(inp6[0]));
                await Task.Delay(rightkey_wait);
                racefinishover = racefinishover + rightkey_down + rightkey_wait;
                if (racefinishover >= racefinish)
                    break;
            }

            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));

            for (int i = 0; i < 8; i++)
            {
                await Task.Delay(1000);
                SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
                await Task.Delay(220);
                SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
            }
            await Task.Delay(wait_replay);

            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(500);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_racemenu);

            SendInput(num5, ref inp5[0], Marshal.SizeOf(inp5[0]));
            await Task.Delay(220);
            SendInput(num6, ref inp6[0], Marshal.SizeOf(inp6[0]));
            await Task.Delay(500);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_entry2);

            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(1300);

            SendInput(num9, ref inp9[0], Marshal.SizeOf(inp9[0]));
            await Task.Delay(220);
            SendInput(num10, ref inp10[0], Marshal.SizeOf(inp10[0]));
            await Task.Delay(500);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(500);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_backtomenu);

            if (stop == false)
            {
                goto label1;
            }

            else if (stop == true)
            {
                goto label2;
            }

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
            decimal wait_entry_dec = Waitentry.Value;
            decimal wait_start_dec = Waitstart.Value;
            decimal wait_replay_dec = Waitreplay.Value;
            decimal wait_racemenu_dec = Waitracemenu.Value;
            decimal wait_entry2_dec = Waitentry2.Value;
            decimal wait_backtomenu_dec = Waitbacktomenu.Value;
            int racefinish_min = decimal.ToInt32(Racefinishmin.Value);
            int racefinish_sec = decimal.ToInt32(Racefinishsec.Value);
            decimal rightkey_down_dec = Rightkeydown.Value;
            decimal rightkey_wait_dec = Rightkeywait.Value;
            int racefinishover = 0;
            wait_entry_dec = wait_entry_dec * 1000;
            wait_start_dec = wait_start_dec * 1000;
            wait_replay_dec = wait_replay_dec * 1000;
            wait_racemenu_dec = wait_racemenu_dec * 1000;
            wait_entry2_dec = wait_entry2_dec * 1000;
            wait_backtomenu_dec = wait_backtomenu_dec * 1000;
            rightkey_down_dec = rightkey_down_dec * 1000;
            rightkey_wait_dec = rightkey_wait_dec * 1000;
            racefinish_min = racefinish_min * 60;
            int racefinish = racefinish_min + racefinish_sec;
            racefinish = racefinish * 1000;
            int wait_entry = decimal.ToInt32(wait_entry_dec);
            int wait_start = decimal.ToInt32(wait_start_dec);
            int wait_replay = decimal.ToInt32(wait_replay_dec);
            int wait_racemenu = decimal.ToInt32(wait_racemenu_dec);
            int wait_entry2 = decimal.ToInt32(wait_entry2_dec);
            int wait_backtomenu = decimal.ToInt32(wait_backtomenu_dec);
            int rightkey_down = decimal.ToInt32(rightkey_down_dec);
            int rightkey_wait = decimal.ToInt32(rightkey_wait_dec);
            int count = 0;
            await Task.Delay(500);

            const int num = 1;
            INPUT[] inp = new INPUT[num];

            inp[0].type = INPUT_KEYBOARD;
            inp[0].ki.wVk = VK_ENTER;
            inp[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp[0].ki.dwExtraInfo = 0;
            inp[0].ki.time = 0;

            const int num2 = 1;
            INPUT[] inp2 = new INPUT[num];

            inp2[0].type = INPUT_KEYBOARD;
            inp2[0].ki.wVk = VK_ENTER;
            inp2[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp2[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp2[0].ki.dwExtraInfo = 0;
            inp2[0].ki.time = 0;

            const int num3 = 1;
            INPUT[] inp3 = new INPUT[num];

            inp3[0].type = INPUT_KEYBOARD;
            inp3[0].ki.wVk = VK_LEFT;
            inp3[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp3[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp3[0].ki.dwExtraInfo = 0;
            inp3[0].ki.time = 0;

            const int num4 = 1;
            INPUT[] inp4 = new INPUT[num];

            inp4[0].type = INPUT_KEYBOARD;
            inp4[0].ki.wVk = VK_LEFT;
            inp4[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp4[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp4[0].ki.dwExtraInfo = 0;
            inp4[0].ki.time = 0;

            const int num5 = 1;
            INPUT[] inp5 = new INPUT[num];

            inp5[0].type = INPUT_KEYBOARD;
            inp5[0].ki.wVk = VK_RIGHT;
            inp5[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp5[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp5[0].ki.dwExtraInfo = 0;
            inp5[0].ki.time = 0;


            const int num6 = 1;
            INPUT[] inp6 = new INPUT[num];

            inp6[0].type = INPUT_KEYBOARD;
            inp6[0].ki.wVk = VK_RIGHT;
            inp6[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp6[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp6[0].ki.dwExtraInfo = 0;
            inp6[0].ki.time = 0;

            const int num7 = 1;
            INPUT[] inp7 = new INPUT[num];

            inp7[0].type = INPUT_KEYBOARD;
            inp7[0].ki.wVk = VK_DOWN;
            inp7[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp7[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp7[0].ki.dwExtraInfo = 0;
            inp7[0].ki.time = 0;


            const int num8 = 1;
            INPUT[] inp8 = new INPUT[num];

            inp8[0].type = INPUT_KEYBOARD;
            inp8[0].ki.wVk = VK_DOWN;
            inp8[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp8[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp8[0].ki.dwExtraInfo = 0;
            inp8[0].ki.time = 0;

            const int num9 = 1;
            INPUT[] inp9 = new INPUT[num];

            inp9[0].type = INPUT_KEYBOARD;
            inp9[0].ki.wVk = VK_ESC;
            inp9[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp9[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp9[0].ki.dwExtraInfo = 0;
            inp9[0].ki.time = 0;

            const int num10 = 1;
            INPUT[] inp10 = new INPUT[num];

            inp10[0].type = INPUT_KEYBOARD;
            inp10[0].ki.wVk = VK_ESC;
            inp10[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp10[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp10[0].ki.dwExtraInfo = 0;
            inp10[0].ki.time = 0;

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
            
            SendInput(num7, ref inp7[0], Marshal.SizeOf(inp7[0]));
            await Task.Delay(220);
            SendInput(num8, ref inp8[0], Marshal.SizeOf(inp8[0]));

            for (int i = 0; i < 6; i++)
            {
                await Task.Delay(220);
                SendInput(num5, ref inp5[0], Marshal.SizeOf(inp5[0]));
                await Task.Delay(220);
                SendInput(num6, ref inp6[0], Marshal.SizeOf(inp6[0]));
            }

            await Task.Delay(300);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(800);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_entry);

            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(1500);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_start);

            await Task.Delay(14000);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            SendInput(num5, ref inp5[0], Marshal.SizeOf(inp5[0]));

            await Task.Delay(2000);

            SendInput(num6, ref inp6[0], Marshal.SizeOf(inp6[0]));

            await Task.Delay(500);

            racefinishover = 0;
            while (true)
            {
                SendInput(num5, ref inp5[0], Marshal.SizeOf(inp5[0]));
                await Task.Delay(rightkey_down);
                SendInput(num6, ref inp6[0], Marshal.SizeOf(inp6[0]));
                await Task.Delay(rightkey_wait);
                racefinishover = racefinishover + rightkey_down + rightkey_wait;
                if (racefinishover >= racefinish)
                    break;
            }

            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));


            for (int i = 0; i < 6; i++)
            {
                await Task.Delay(1000);

                SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
                await Task.Delay(220);
                SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
            }

            if (count == 3)
            {
                await Task.Delay(5000);
                SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
                await Task.Delay(220);
                SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
                await Task.Delay(1000);
                SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
                await Task.Delay(220);
                SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
            }

            else
            {
                await Task.Delay(1000);
                SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
                await Task.Delay(220);
                SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
                await Task.Delay(1000);
                SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
                await Task.Delay(220);
                SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
            }

            await Task.Delay(wait_replay);

            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(500);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_racemenu);

            SendInput(num5, ref inp5[0], Marshal.SizeOf(inp5[0]));
            await Task.Delay(220);
            SendInput(num6, ref inp6[0], Marshal.SizeOf(inp6[0]));
            await Task.Delay(500);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_entry2);

            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(1300);

            SendInput(num9, ref inp9[0], Marshal.SizeOf(inp9[0]));
            await Task.Delay(220);
            SendInput(num10, ref inp10[0], Marshal.SizeOf(inp10[0]));
            await Task.Delay(500);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));
            await Task.Delay(500);
            SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            await Task.Delay(220);
            SendInput(num2, ref inp2[0], Marshal.SizeOf(inp2[0]));

            await Task.Delay(wait_backtomenu);

            if (stop == false)
            {
                goto label1;
            }

            else if (stop == true)
            {
                goto label2;
            }

        label2:
            stop = false;
            Workmessage.Text = "現在実行されていません";
        }

        private void Modosu_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("全ての設定を初期化しますか？", "", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                Waitentry.Value = 12;
                Waitstart.Value = 1;
                Waitreplay.Value = 1;
                Waitracemenu.Value = 2;
                Waitentry2.Value = 14;
                Waitbacktomenu.Value = 8;
                Racefinishmin.Value = 4;
                Racefinishsec.Value = 20;
                Rightkeydown.Value = Convert.ToDecimal(0.2);
                Rightkeywait.Value = Convert.ToDecimal(0.6);

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

            sfd.FileName = "setting.gbt";
            sfd.InitialDirectory = appPath + @"\";
            sfd.Filter = "gbtファイル(*.gbt)|*.gbt|すべてのファイル(*.*)|*.*";
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
                    sw.WriteLine("エントリー後：" + Waitentry.Value + "秒");
                    sw.WriteLine("スタートボタンを押した後：" + Waitstart.Value + "秒");
                    sw.WriteLine("賞金獲得～リプレイ：" + Waitreplay.Value + "秒");
                    sw.WriteLine("リプレイ～レースメニュー：" + Waitracemenu.Value + "秒");
                    sw.WriteLine("2戦目ロード：" + Waitentry2.Value + "秒");
                    sw.WriteLine("2戦目棄権～メニュー：" + Waitbacktomenu.Value + "秒");
                    sw.WriteLine("レース終了まで：" + Racefinishmin.Value + "分" + Racefinishsec.Value + "秒");
                    sw.WriteLine("右キー入力秒：" + Rightkeydown.Value + "秒");
                    sw.Write("右キーの間隔：" + Rightkeywait.Value + "秒");
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

            ofd.FileName = "setting.gbt";
            ofd.InitialDirectory = appPath + @"\";
            ofd.Filter = "gbtファイル(*.gbt)|*.gbt|すべてのファイル(*.*)|*.*";
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
                    line = line.Remove(0, line.IndexOf("：") + 1);
                    line = line.Replace("秒", "");
                    decimal linedec = decimal.Parse(line);
                    if (linedec > 10000)
                        linedec = 10000;
                    Waitentry.Value = linedec;

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
                    linedec = decimal.Parse(line);
                    if (linedec > 10000)
                        linedec = 10000;
                    Waitbacktomenu.Value = linedec;

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
                    line = line.Replace("秒", "");
                    linedec = decimal.Parse(line);
                    if (linedec > 10000)
                        linedec = 10000;
                    Rightkeydown.Value = linedec;

                    line = sr.ReadLine();
                    line = line.Remove(0, line.IndexOf("：") + 1);
                    line = line.Replace("秒", "");
                    linedec = decimal.Parse(line);
                    if (linedec > 10000)
                        linedec = 10000;
                    Rightkeywait.Value = linedec;


                    //閉じる
                    sr.Close();
                    stream.Close();
                }
            }
        }
    }
}
