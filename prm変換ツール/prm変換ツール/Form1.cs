using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace prm変換ツール
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string[] path = new string[] { "" };
        public static int j = 0;
        public static int GTM1_MDLSheader = 0;
        public static bool close = false;

        private void Form1_Shown(object sender, EventArgs e)
        {
            ProgressBar1.Minimum = 0;
            ProgressBar1.Value = 0;
            BackgroundWorker1.WorkerReportsProgress = true;
            BackgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar1.Value = j;
            ProgressBar1.Maximum = GTM1_MDLSheader;
            Label1.Text = j.ToString() + "/" + GTM1_MDLSheader.ToString();
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (close == true)
                Close();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = (BackgroundWorker)sender;
            string[] path = Environment.GetCommandLineArgs();
            if (path.Count() != 1)
            {
                Label1.Text = "実行中...";
                close = true;
            }

            byte[] bs_none = new byte[4] { 0x00, 0x00, 0x00, 0x00 };

            for (int i = 0; i < path.Count(); i++)
            {
                string dire = Path.GetDirectoryName(path[i]);
                string[] readfiles = Directory.GetFiles(dire, "*", SearchOption.AllDirectories);
                List<int> readmenufilesextension = new List<int>();
                List<int> readmenufilesnum = new List<int>();


                // ドラッグ＆ドロップされたファイル
                FileStream fsr = new FileStream(path[i].ToString(), FileMode.Open, FileAccess.Read);

                byte[] bs = new byte[fsr.Length];
                fsr.Read(bs, 0, bs.Length);
                fsr.Close();

                string filecheck = Path.GetExtension(path[i]);

                if (filecheck == ".prm" || filecheck == ".PRM")
                    goto GTM1;

                else
                    goto labelfinish;

                GTM1:;

                string binfolder = Path.GetDirectoryName(path[i]) + @"\bin";
                List<int> readbinfilesextension = new List<int>();
                List<int> readbinfilesnum = new List<int>();
                if (Directory.Exists(binfolder) == false)
                    goto labelfinish;

                string[] readbinfiles = Directory.GetFiles(binfolder, "*", SearchOption.AllDirectories);
                for (int k = 0; k < readbinfiles.Count(); k++)
                {
                    if (Path.GetExtension(readbinfiles[k]) == ".bin" || Path.GetExtension(readbinfiles[k]) == ".BIN")
                        readbinfilesextension.Add(k);
                }

                for (int k = 0; k < readbinfilesextension.Count(); k++)
                {
                    FileStream fsrbin2 = new FileStream(readbinfiles[readbinfilesextension[k]].ToString(), FileMode.Open, FileAccess.Read);
                    byte[] bsbin2 = new byte[fsrbin2.Length];
                    fsrbin2.Read(bsbin2, 0, bsbin2.Length);
                    if (Getbytestr(bsbin2, 64) == "47544349")
                        readbinfilesnum.Add(readbinfilesextension[k]);
                    fsrbin2.Close();
                }

                if (readbinfilesnum.Count != 1)
                    goto labelfinish;

                FileStream fsrbin = new FileStream(readbinfiles[readbinfilesnum[0]].ToString(), FileMode.Open, FileAccess.Read);
                byte[] bsbinall = new byte[fsrbin.Length];
                fsrbin.Read(bsbinall, 0, bsbinall.Length);
                fsrbin.Close();

                //ファイルを分割する
                int bintopend = Getbyte(bsbinall, 8);
                int binheaderlength = Getbyte(bsbinall, 4);
                int bintoplength = bintopend - binheaderlength;
                byte[] bsbintop = new byte[bintoplength];
                Array.Copy(bsbinall, binheaderlength, bsbintop, 0, bintoplength);

                int binunderstart = Getbyte(bsbinall, 12);
                int binunderlength = bsbinall.Length - binunderstart;
                byte[] bsbinunder = new byte[binunderlength];
                Array.Copy(bsbinall, binunderstart, bsbinunder, 0, binunderlength);

                int binGTTR = Getbyte(bsbinall, 12);
                int binGTTW = Getbyte(bsbinall, 16);
                int bindummy = Getbyte(bsbinall, 20);

                int prmGTM1start = Getbyte(bs, 44);
                int prmGTM1length = bs.Length - prmGTM1start;
                byte[] newGTM1 = new byte[prmGTM1length];
                Array.Copy(bs, prmGTM1start, newGTM1, 0, prmGTM1length);

                byte[] newfileheader = new byte[12];
                Array.Copy(bsbinall, 0, newfileheader, 0, 12);

                int binGTM1length = binunderstart - bintopend;
                int GTM1dif = binGTM1length - prmGTM1length;
                int newGTTR = binGTTR - GTM1dif;
                byte[] newGTTR_byte = new byte[4];
                newGTTR_byte = Gethex(newGTTR);
                int newGTTW = binGTTW - GTM1dif;
                byte[] newGTTW_byte = new byte[4];
                newGTTW_byte = Gethex(newGTTW);
                int newdummy = bindummy - GTM1dif;
                byte[] newdummy_byte = new byte[4];
                newdummy_byte = Gethex(newdummy);

                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(newGTTR_byte, 0, newfileheader, newfileheader.Length - 4, 4);
                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(newGTTW_byte, 0, newfileheader, newfileheader.Length - 4, 4);
                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(newdummy_byte, 0, newfileheader, newfileheader.Length - 4, 4);

                for (int k = 0; k < 10; k++)
                {
                    Array.Resize(ref newfileheader, newfileheader.Length + 4);
                    Array.Copy(bs_none, 0, newfileheader, newfileheader.Length - 4, 4);
                }

                FileStream fsw = new FileStream(Path.GetDirectoryName(path[i]) + @"\" + Path.GetFileNameWithoutExtension(readbinfiles[readbinfilesnum[0]]) + ".bin", FileMode.Create, FileAccess.Write);
                fsw.Write(newfileheader, 0, newfileheader.Length);
                fsw.Write(bsbintop, 0, bsbintop.Length);
                fsw.Write(newGTM1, 0, newGTM1.Length);
                fsw.Write(bsbinunder, 0, bsbinunder.Length);
                fsw.Close();

                goto labelfinish;

            labelfinish:;
            }
        }

        //byte配列1バイトをintに変換して戻す
        public static int Getbyte1(byte[] bytes, int seek)
        {
            byte[] byte1 = new byte[1];
            Array.Copy(bytes, seek, byte1, 0, 1);

            string str1 = BitConverter.ToString(byte1);
            int returnint = Convert.ToInt32(str1, 16);

            return returnint;
        }

        //byte配列2バイトをintに変換して戻す
        public static int Getbyte2(byte[] bytes, int seek)
        {
            byte[] byte1 = new byte[1];
            Array.Copy(bytes, seek, byte1, 0, 1);
            byte[] byte2 = new byte[1];
            Array.Copy(bytes, seek + 1, byte2, 0, 1);

            string str1 = BitConverter.ToString(byte1);
            string str2 = BitConverter.ToString(byte2);
            int bytelength = 0;

            if (str2 != "00")
            {
                bytelength = 2;
                goto label_byteget;
            }
            else if (str1 != "00")
            {
                bytelength = 1;
                goto label_byteget;
            }

            else
                return 0;

            label_byteget:;

            string str16 = "";
            if (bytelength == 1)
                str16 = str1;
            else if (bytelength == 2)
                str16 = str2 + str1;

            int returnint = Convert.ToInt32(str16, 16);

            return returnint;
        }

        //byte配列4バイトをintに変換して戻す
        public static int Getbyte(byte[] bytes, int seek)
        {
            byte[] byte1 = new byte[1];
            Array.Copy(bytes, seek, byte1, 0, 1);
            byte[] byte2 = new byte[1];
            Array.Copy(bytes, seek + 1, byte2, 0, 1);
            byte[] byte3 = new byte[1];
            Array.Copy(bytes, seek + 2, byte3, 0, 1);
            byte[] byte4 = new byte[1];
            Array.Copy(bytes, seek + 3, byte4, 0, 1);

            string str1 = BitConverter.ToString(byte1);
            string str2 = BitConverter.ToString(byte2);
            string str3 = BitConverter.ToString(byte3);
            string str4 = BitConverter.ToString(byte4);
            int bytelength = 0;

            if (str4 != "00")
            {
                bytelength = 4;
                goto label_byteget;
            }
            else if (str3 != "00")
            {
                bytelength = 3;
                goto label_byteget;
            }
            else if (str2 != "00")
            {
                bytelength = 2;
                goto label_byteget;
            }
            else if (str1 != "00")
            {
                bytelength = 1;
                goto label_byteget;
            }

            else
                return 0;

        label_byteget:;

            string str16 = "";
            if (bytelength == 1)
                str16 = str1;
            else if (bytelength == 2)
                str16 = str2 + str1;
            else if (bytelength == 3)
                str16 = str3 + str2 + str1;
            else if (bytelength == 4)
                str16 = str4 + str3 + str2 + str1;

            int returnint = Convert.ToInt32(str16, 16);

            return returnint;
        }

        //byte配列4バイトをstringに変換して戻す
        public static string Getbytestr(byte[] bytes, int seek)
        {
            byte[] byte1 = new byte[1];
            Array.Copy(bytes, seek, byte1, 0, 1);
            byte[] byte2 = new byte[1];
            Array.Copy(bytes, seek + 1, byte2, 0, 1);
            byte[] byte3 = new byte[1];
            Array.Copy(bytes, seek + 2, byte3, 0, 1);
            byte[] byte4 = new byte[1];
            Array.Copy(bytes, seek + 3, byte4, 0, 1);

            string str1 = BitConverter.ToString(byte1);
            string str2 = BitConverter.ToString(byte2);
            string str3 = BitConverter.ToString(byte3);
            string str4 = BitConverter.ToString(byte4);

            string returnstr = str1 + str2 + str3 + str4;

            return returnstr;
        }

        //intをbyte配列4バイト(リトルエンディアン)に変換して戻す
        public static byte[] Gethex(int hex)
        {
            string hexstr = hex.ToString("X");
            if (hexstr.Length == 1 || hexstr.Length == 3 || hexstr.Length == 5 || hexstr.Length == 7)
                hexstr = "0" + hexstr;

            if (hexstr.Length == 2)
                hexstr = hexstr + "000000";

            else if (hexstr.Length == 4)
            {
                string hexstr1 = hexstr.Substring(0, 2);
                string hexstr2 = hexstr.Substring(2, 2);
                hexstr = hexstr2 + hexstr1 + "0000";
            }

            else if (hexstr.Length == 6)
            {
                string hexstr1 = hexstr.Substring(0, 2);
                string hexstr2 = hexstr.Substring(2, 2);
                string hexstr3 = hexstr.Substring(4, 2);
                hexstr = hexstr3 + hexstr2 + hexstr1 + "00";
            }

            else if (hexstr.Length == 8)
            {
                string hexstr1 = hexstr.Substring(0, 2);
                string hexstr2 = hexstr.Substring(2, 2);
                string hexstr3 = hexstr.Substring(4, 2);
                string hexstr4 = hexstr.Substring(6, 2);
                hexstr = hexstr4 + hexstr3 + hexstr2 + hexstr1;
            }
            byte[] hexbyte = new byte[4];
            hexbyte = StringToBytes(hexstr);
            return hexbyte;
        }

        //16進数文字列 => Byte配列
        public static byte[] StringToBytes(string str)
        {
            var bs = new List<byte>();
            for (int i = 0; i < str.Length / 2; i++)
            {
                bs.Add(Convert.ToByte(str.Substring(i * 2, 2), 16));
            }
            // "01-AB-EF" こういう"-"区切りを想定する場合は以下のようにする
            // var bs = str.Split('-').Select(hex => Convert.ToByte(hex, 16));
            return bs.ToArray();
        }
    }
}
