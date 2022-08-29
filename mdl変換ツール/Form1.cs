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

namespace mdl変換ツール
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

                if (filecheck == ".mdl" || filecheck == ".MDL")
                    goto MDLS;

                else
                    goto labelfinish;

                MDLS:;

                string lodfolder = Path.GetDirectoryName(path[i]) + @"\lod";
                List<int> readlodfilesextension = new List<int>();
                List<int> readlodfilesnum = new List<int>();
                if (Directory.Exists(lodfolder) == false)
                    goto labelfinish;

                string[] readlodfiles = Directory.GetFiles(lodfolder, "*", SearchOption.AllDirectories);
                for (int k = 0; k < readlodfiles.Count(); k++)
                {
                    if (Path.GetExtension(readlodfiles[k]) == "")
                        readlodfilesextension.Add(k);
                }

                for (int k = 0; k < readlodfilesextension.Count(); k++)
                {
                    FileStream fsrlod2 = new FileStream(readlodfiles[readlodfilesextension[k]].ToString(), FileMode.Open, FileAccess.Read);
                    byte[] bslod2 = new byte[fsrlod2.Length];
                    fsrlod2.Read(bslod2, 0, bslod2.Length);
                    if (Getbytestr(bslod2, 0) == "43415234")
                        readlodfilesnum.Add(readlodfilesextension[k]);
                    fsrlod2.Close();
                }

                if (readlodfilesnum.Count != 1)
                    goto labelfinish;

                FileStream fsrlod = new FileStream(readlodfiles[readlodfilesnum[0]].ToString(), FileMode.Open, FileAccess.Read);
                byte[] bslodall = new byte[fsrlod.Length];
                fsrlod.Read(bslodall, 0, bslodall.Length);
                fsrlod.Close();

                //ファイルを分割する
                int lodMDLStopend = Getbyte(bslodall, 24);
                int lodMDLSheaderlength = Getbyte(bslodall, 16);
                int lodMDLStoplength = lodMDLStopend - lodMDLSheaderlength;
                byte[] bslodMDLStop = new byte[lodMDLStoplength];
                Array.Copy(bslodall, lodMDLSheaderlength, bslodMDLStop, 0, lodMDLStoplength);

                int lodMDLSunderstart = Getbyte(bslodall, 32);
                int lodMDLSunderlength = bslodall.Length - lodMDLSunderstart;
                byte[] bslodMDLSunder = new byte[lodMDLSunderlength];
                Array.Copy(bslodall, lodMDLSunderstart, bslodMDLSunder, 0, lodMDLSunderlength);
                int lodMDLS = Getbyte(bslodall, 8);

                int bslodlength = Getbyte(bslodall, 8);
                int lodheaderunknown1 = Getbyte(bslodall, 32);
                int lodheaderunknown2 = Getbyte(bslodall, 36);
                int lodheaderunknown3 = Getbyte(bslodall, 44);
                int lodheaderunknown4 = Getbyte(bslodall, 48);
                int lodheaderunknown5 = Getbyte(bslodall, 52);

                byte[] bs_none1 = new byte[1] { 0x00 };
                string MDLSlengthhex = bs.Length.ToString("X");
                string MDLSlengthhex0 = MDLSlengthhex.Substring(MDLSlengthhex.Length - 1, 1);

                if (MDLSlengthhex0 != "0")
                {
                    while (true)
                    {
                        Array.Resize(ref bs, bs.Length + 1);
                        Array.Copy(bs_none1, 0, bs, bs.Length - 1, 1);
                        MDLSlengthhex = bs.Length.ToString("X");
                        MDLSlengthhex0 = MDLSlengthhex.Substring(MDLSlengthhex.Length - 1, 1);
                        if (MDLSlengthhex0 == "0")
                            break;
                    }
                }

                for (int k = 0; k < 4; k++)
                {
                    Array.Resize(ref bs, bs.Length + 4);
                    Array.Copy(bs_none, 0, bs, bs.Length - 4, 4);
                }

                byte[] newfileheader = new byte[4] { 0x43, 0x41, 0x52, 0x34 };

                int MDLSlength = bslodall.Length - lodMDLSheaderlength - lodMDLStoplength - lodMDLSunderlength;
                int MDLSdif = MDLSlength - bs.Length;
                int newlodlength = bslodlength - MDLSdif;
                byte[] newlodlength_byte = new byte[4];
                newlodlength_byte = Gethex(newlodlength);
                int newlodheaderunknown1 = lodheaderunknown1 - MDLSdif;
                byte[] newlodheaderunknown1_byte = new byte[4];
                newlodheaderunknown1_byte = Gethex(newlodheaderunknown1);
                int newlodheaderunknown2 = lodheaderunknown2 - MDLSdif;
                byte[] newlodheaderunknown2_byte = new byte[4];
                newlodheaderunknown2_byte = Gethex(newlodheaderunknown2);
                int newlodheaderunknown3 = lodheaderunknown3 - MDLSdif;
                byte[] newlodheaderunknown3_byte = new byte[4];
                newlodheaderunknown3_byte = Gethex(newlodheaderunknown3);
                int newlodheaderunknown4 = lodheaderunknown4 - MDLSdif;
                byte[] newlodheaderunknown4_byte = new byte[4];
                newlodheaderunknown4_byte = Gethex(newlodheaderunknown4);
                int newlodheaderunknown5 = lodheaderunknown5 - MDLSdif;
                byte[] newlodheaderunknown5_byte = new byte[4];
                newlodheaderunknown5_byte = Gethex(newlodheaderunknown5);

                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(bs_none, 0, newfileheader, newfileheader.Length - 4, 4);
                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(newlodlength_byte, 0, newfileheader, newfileheader.Length - 4, 4);
                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(bs_none, 0, newfileheader, newfileheader.Length - 4, 4);
                Array.Resize(ref newfileheader, newfileheader.Length + 16);
                Array.Copy(bslodall, 16, newfileheader, newfileheader.Length - 16, 16);
                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(newlodheaderunknown1_byte, 0, newfileheader, newfileheader.Length - 4, 4);
                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(newlodheaderunknown2_byte, 0, newfileheader, newfileheader.Length - 4, 4);
                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(bs_none, 0, newfileheader, newfileheader.Length - 4, 4);
                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(newlodheaderunknown3_byte, 0, newfileheader, newfileheader.Length - 4, 4);
                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(newlodheaderunknown4_byte, 0, newfileheader, newfileheader.Length - 4, 4);
                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(newlodheaderunknown5_byte, 0, newfileheader, newfileheader.Length - 4, 4);
                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(bs_none, 0, newfileheader, newfileheader.Length - 4, 4);
                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(bs_none, 0, newfileheader, newfileheader.Length - 4, 4);

                FileStream fsw = new FileStream(Path.GetDirectoryName(path[i]) + @"\" + Path.GetFileNameWithoutExtension(readlodfiles[readlodfilesnum[0]]), FileMode.Create, FileAccess.Write);
                fsw.Write(newfileheader, 0, newfileheader.Length);
                fsw.Write(bslodMDLStop, 0, bslodMDLStop.Length);
                fsw.Write(bs, 0, bs.Length);
                fsw.Write(bslodMDLSunder, 0, bslodMDLSunder.Length);
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
