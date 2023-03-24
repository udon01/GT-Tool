using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GT3用モデルサイズ調整ツール
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string[] path = new string[] { "" };
        public static int j = 0;
        public static int GTM1header = 0;
        public static bool close = false;
        //頂点座標の増減量を指定

        private void textBox1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            //16進数とバックスペース以外の時は入力をキャンセル
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b' && (e.KeyChar < 'a' || 'f' < e.KeyChar) && (e.KeyChar < 'A' || 'F' < e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Inc_or_Dec == 0)
                Inc_Dec.Text = "増やす";
            else
                Inc_Dec.Text = "減らす";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Inc_Dec.Text == "減らす")
                Properties.Settings.Default.Inc_or_Dec = 1;
            else
                Properties.Settings.Default.Inc_or_Dec = 0;
            Properties.Settings.Default.Save();
        }

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
            ProgressBar1.Maximum = GTM1header;
            Label1.Text = j.ToString() + "/" + GTM1header.ToString();
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
            byte[] bs_none1 = new byte[1] { 0x00 };

            for (int i = 0; i < path.Count(); i++)
            {
                string dire = Path.GetDirectoryName(path[i]);
                string[] readfiles = Directory.GetFiles(dire, "*", SearchOption.AllDirectories);
                List<int> readmenufilesextension = new List<int>();
                List<int> readmenufilesnum = new List<int>();

                string verticeedit_str = sizeedit.Text;
                int verticeedit = Convert.ToInt32(verticeedit_str, 16);
                if (Inc_Dec.Text == "減らす")
                    verticeedit = -verticeedit;

                // ドラッグ＆ドロップされたファイル
                FileStream fsr = new FileStream(path[i].ToString(), FileMode.Open, FileAccess.Read);

                byte[] bs = new byte[fsr.Length];
                fsr.Read(bs, 0, bs.Length);

                string filecheck = Getbytestr(bs, 64);
                if (filecheck == "47544349")
                    goto labelstart;

                else
                    goto labelfinish;

                labelstart:;

                byte[] bsGT3car = new byte[bs.Length];
                Array.Copy(bs, 0, bsGT3car, 0, bs.Length);

                //GTM1を分割する
                int GTM1lengthstart = Getbyte(bsGT3car, 4);
                byte[] bsGTM1header = new byte[GTM1lengthstart];
                Array.Copy(bsGT3car, 0, bsGTM1header, 0, GTM1lengthstart);

                GTM1lengthstart = Getbyte(bsGT3car, 4);
                int GTM1lengthend = Getbyte(bsGT3car, 8);
                int GTM1length2 = GTM1lengthend - GTM1lengthstart;
                byte[] bsGTM1GTCI = new byte[GTM1length2];
                Array.Copy(bsGT3car, GTM1lengthstart, bsGTM1GTCI, 0, GTM1length2);

                GTM1lengthstart = Getbyte(bsGT3car, 8);
                GTM1lengthend = Getbyte(bsGT3car, 12);
                GTM1length2 = GTM1lengthend - GTM1lengthstart;
                byte[] bsGTM1 = new byte[GTM1length2];
                Array.Copy(bsGT3car, GTM1lengthstart, bsGTM1, 0, GTM1length2);

                GTM1lengthstart = Getbyte(bsGT3car, 12);
                GTM1lengthend = Getbyte(bsGT3car, 16);
                GTM1length2 = GTM1lengthend - GTM1lengthstart;
                byte[] bsGTM1GTTR = new byte[GTM1length2];
                Array.Copy(bsGT3car, GTM1lengthstart, bsGTM1GTTR, 0, GTM1length2);

                GTM1lengthstart = Getbyte(bsGT3car, 16);
                GTM1lengthend = Getbyte(bsGT3car, 20);
                GTM1length2 = GTM1lengthend - GTM1lengthstart;
                byte[] bsGTM1GTTW = new byte[GTM1length2];
                Array.Copy(bsGT3car, GTM1lengthstart, bsGTM1GTTW, 0, GTM1length2);

                GTM1lengthstart = Getbyte(bsGT3car, 20);
                GTM1lengthend = bsGT3car.Length;
                GTM1length2 = GTM1lengthend - GTM1lengthstart;
                byte[] bsGTM1dummy = new byte[GTM1length2];
                Array.Copy(bsGT3car, GTM1lengthstart, bsGTM1dummy, 0, GTM1length2);

                //GTM1のヘッダーまでの長さ
                int GTM1headerstart = Getbyte(bsGTM1, 36);
                //GTM1のヘッダー終わりまでの長さ
                int GTM1headerend = Getbyte(bsGTM1, 48);

                byte[] GTM1headertop = new byte[GTM1headerend];
                Array.Copy(bsGTM1, 0, GTM1headertop, 0, GTM1headerend);

                GTM1header = GTM1headerend - GTM1headerstart;
                GTM1header /= 4;

                int modelheaderlength_1 = Getbyte(bsGTM1, GTM1headerstart);
                byte[] modelunknown = new byte[modelheaderlength_1 - GTM1headerend];
                Array.Copy(bsGTM1, GTM1headerend, modelunknown, 0, modelheaderlength_1 - GTM1headerend);

                byte[] newGTM1 = new byte[0];

                for (j = 0; j < GTM1header; j++)
                {
                    byte[] newmodelall = new byte[0];

                    int seek_j = j * 4;
                    //モデルデータのヘッダーまでの長さ
                    int modelheaderstart = Getbyte(bsGTM1, GTM1headerstart + seek_j);

                    if (modelheaderstart == 0)
                        break;

                    int newGTM1headerlength2 = GTM1headerstart + modelunknown.Length + newGTM1.Length;

                    int modelheadercount = Getbyte2(bsGTM1, modelheaderstart + 10);
                    byte[] modelheader = new byte[(modelheadercount * 8) + 16];
                    Array.Copy(bsGTM1, modelheaderstart, modelheader, 0, modelheader.Length);

                    if (modelheader.Length % 16 != 0)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            Array.Resize(ref modelheader, modelheader.Length + 4);
                            Array.Copy(bs_none, 0, modelheader, modelheader.Length - 4, 4);
                        }
                    }

                    for (int k = 0; k < modelheadercount; k++)
                    {
                        int modelheaderlength = Getbyte2(bsGTM1, modelheaderstart + (8 * k) + 16 + 4);
                        modelheaderlength *= 16;
                        int seek_k = k * 8;
                        int modelstartget = Getbyte(bsGTM1, modelheaderstart + 16 + seek_k);
                        int verticestart = modelheaderstart + modelstartget + 20;
                        int verticecount = Getbyte1(bsGTM1, modelheaderstart + modelstartget + 18);

                        int modelstart = modelheaderstart + modelstartget;
                        byte[] modelunknown1 = new byte[20];
                        Array.Copy(bsGTM1, modelstart, modelunknown1, 0, 20);
                        int verticelength = verticecount * 3 * 4;
                        byte[] modelvertice = new byte[0];

                        for (int l = 0; l < verticecount * 3; l++)
                        {
                            int seek_l = l * 4;
                            int verticeint = Getbyte(bsGTM1, verticestart + seek_l);
                            if (verticeint != 0)
                                verticeint += verticeedit;
                            byte[] vertice_new = new byte[4];
                            vertice_new = Gethex(verticeint);
                            Array.Resize(ref modelvertice, modelvertice.Length + 4);
                            Array.Copy(vertice_new, 0, modelvertice, modelvertice.Length - 4, 4);
                        }

                        modelheaderlength -= modelvertice.Length + 20;
                        byte[] modelother = new byte[modelheaderlength];
                        Array.Copy(bsGTM1, verticestart + modelvertice.Length, modelother, 0, modelheaderlength);

                        //変換先のbyte配列を用意する
                        byte[] newmodel = new byte[0];
                        int newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelunknown1.Length);
                        Array.Copy(modelunknown1, 0, newmodel, newmodellength, modelunknown1.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelvertice.Length);
                        Array.Copy(modelvertice, 0, newmodel, newmodellength, modelvertice.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelother.Length);
                        Array.Copy(modelother, 0, newmodel, newmodellength, modelother.Length);
                        //MessageBox.Show(newmodel.Length.ToString());

                        int newmodelalllength = newmodelall.Length;
                        Array.Resize(ref newmodelall, newmodelall.Length + newmodel.Length);
                        Array.Copy(newmodel, 0, newmodelall, newmodelalllength, newmodel.Length);
                    }

                    int newGTM1length = newGTM1.Length;
                    Array.Resize(ref newGTM1, newGTM1.Length + modelheader.Length);
                    Array.Copy(modelheader, 0, newGTM1, newGTM1length, modelheader.Length);
                    newGTM1length = newGTM1.Length;
                    Array.Resize(ref newGTM1, newGTM1.Length + newmodelall.Length);
                    Array.Copy(newmodelall, 0, newGTM1, newGTM1length, newmodelall.Length);
                }

                byte[] newfile_GTM1 = new byte[0];
                Array.Resize(ref newfile_GTM1, GTM1headertop.Length);
                Array.Copy(GTM1headertop, 0, newfile_GTM1, 0, GTM1headertop.Length);
                int newfile_GTM1length = newfile_GTM1.Length;
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + modelunknown.Length);
                Array.Copy(modelunknown, 0, newfile_GTM1, newfile_GTM1length, modelunknown.Length);
                newfile_GTM1length = newfile_GTM1.Length;
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + newGTM1.Length);
                Array.Copy(newGTM1, 0, newfile_GTM1, newfile_GTM1length, newGTM1.Length);

                int Tex1headerstart = Getbyte(bsGTM1, 44);

                if (Tex1headerstart - newfile_GTM1.Length != 0)
                {
                    byte[] modelfooter = new byte[Tex1headerstart - newfile_GTM1.Length];
                    Array.Copy(bsGTM1, newfile_GTM1.Length, modelfooter, 0, modelfooter.Length);
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + modelfooter.Length);
                    Array.Copy(modelfooter, 0, newfile_GTM1, newfile_GTM1.Length - modelfooter.Length, modelfooter.Length);
                }

                //Tex1
                int Tex1start = Getbyte(bsGTM1, Tex1headerstart);
                byte[] Tex1header = new byte[Tex1start - Tex1headerstart];
                Array.Copy(bsGTM1, Tex1headerstart, Tex1header, 0, Tex1header.Length);
                byte[] newTex1all = new byte[0];
                int Tex1ryou = 5;
                Tex1start = Getbyte(bsGTM1, Tex1headerstart + 4);
                if (Tex1start == 0)
                    Tex1ryou = 1;

                for (int k = 0; k < Tex1ryou; k++)
                {
                    int seek_k = k * 4;
                    Tex1start = Getbyte(bsGTM1, Tex1headerstart + seek_k);
                    int Tex1end = Getbyte(bsGTM1, Tex1start + 12);

                    byte[] newTex1 = new byte[Tex1end];
                    Array.Copy(bsGTM1, Tex1start, newTex1, 0, newTex1.Length);
                    Array.Resize(ref newTex1all, newTex1all.Length + newTex1.Length);
                    Array.Copy(newTex1, 0, newTex1all, newTex1all.Length - newTex1.Length, newTex1.Length);

                    if (k < Tex1ryou)
                    {
                        seek_k = (k + 1) * 4;
                        Tex1start = Getbyte(bsGTM1, Tex1headerstart + seek_k);
                        if (Tex1start - newfile_GTM1.Length - newTex1all.Length - Tex1header.Length != 0 && Tex1start != 0)
                        {
                            int kurikaeshi = (Tex1start - newfile_GTM1.Length - newTex1all.Length - Tex1header.Length) / 4;
                            for (int l = 0; l < kurikaeshi; l++)
                            {
                                Array.Resize(ref newTex1all, newTex1all.Length + 4);
                                Array.Copy(bs_none, 0, newTex1all, newTex1all.Length - 4, 4);
                            }
                        }
                    }
                }

                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + Tex1header.Length);
                Array.Copy(Tex1header, 0, newfile_GTM1, newfile_GTM1.Length - Tex1header.Length, Tex1header.Length);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + newTex1all.Length);
                Array.Copy(newTex1all, 0, newfile_GTM1, newfile_GTM1.Length - newTex1all.Length, newTex1all.Length);

                int GTM1dif = bsGTM1.Length - newfile_GTM1.Length;
                if (GTM1dif > 0)
                {
                    int kurikaeshi = GTM1dif / 4;
                    for (int k = 0; k < kurikaeshi; k++)
                    {
                        Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                        Array.Copy(bs_none, 0, newfile_GTM1, newfile_GTM1.Length - 4, 4);
                    }
                }
                
                FileStream fsw = new FileStream(Path.GetDirectoryName(path[i]) + @"\" + Path.GetFileNameWithoutExtension(path[i]) + "_new.bin", FileMode.Create, FileAccess.Write);
                fsw.Write(bsGTM1header, 0, bsGTM1header.Length);
                fsw.Write(bsGTM1GTCI, 0, bsGTM1GTCI.Length);
                fsw.Write(newfile_GTM1, 0, newfile_GTM1.Length);
                fsw.Write(bsGTM1GTTR, 0, bsGTM1GTTR.Length);
                fsw.Write(bsGTM1GTTW, 0, bsGTM1GTTW.Length);
                fsw.Write(bsGTM1dummy, 0, bsGTM1dummy.Length);
                fsw.Close();

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
