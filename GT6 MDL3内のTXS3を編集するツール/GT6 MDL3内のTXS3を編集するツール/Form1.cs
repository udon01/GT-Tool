using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace GT6_body_s内のTXS3を編集するツール
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static int a = 0;
        public static int filecount = 0;
        public static bool close = false;
        public static byte[] zero4 = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
        public static string dfs_path = "";

        private void Form1_Shown(object sender, EventArgs e)
        {
            ProgressBar1.Minimum = 0;
            ProgressBar1.Value = 0;
            BackgroundWorker1.WorkerReportsProgress = true;
            BackgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar1.Value = a;
            ProgressBar1.Maximum = filecount;
            Label1.Text = a.ToString() + "/" + filecount.ToString();
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

            for (int b = 0; b < path.Count(); b++)
            {
                if (Path.GetExtension(path[b]).ToLower() == ".exe")
                    goto labelfinish;

                string MDL3path = path[b];

                string path_hq = path[b] + @"\hq\body";
                string path_race = path[b] + @"\race\body";

                int loopcount = 1;
                bool isfolder = Directory.Exists(path[b]);
                bool path_hq_exist = System.IO.File.Exists(path_hq);
                bool path_race_exist = System.IO.File.Exists(path_race);
                if (isfolder == true)
                {
                    if (path_hq_exist == false && path_race_exist == false)
                        goto labelfolder_comp;
                    if (path_hq_exist == true && path_race_exist == true)
                        loopcount += 1;
                }

                for (int c = 0; c < loopcount; c++)
                {
                    string folderpath = MDL3path + "_hq_texture";
                    string path_body = path_hq;
                    if (path_hq_exist == false || c == 1)
                    {
                        folderpath = MDL3path + "_race_texture";
                        path_body = path_race;
                    }
                    if (isfolder == false)
                    {
                        folderpath = MDL3path + "_texture";
                        path_body = MDL3path;
                    }

                    if (!Directory.Exists(folderpath))
                        Directory.CreateDirectory(folderpath);

                    FileStream fs_s = new FileStream(path_body, FileMode.Open, FileAccess.Read);
                    FileInfo fi_s = new FileInfo(path_body);
                    extractMDL3(ref fs_s, ref fi_s, isfolder);

                    // body
                    FileStream fsr_body = new FileStream(path_body, FileMode.Open, FileAccess.Read);
                    byte[] bs_body = new byte[fsr_body.Length];
                    fsr_body.Read(bs_body, 0, bs_body.Length);
                    fsr_body.Close();

                    // body_s.bin
                    FileStream fsr_body_s = new FileStream(dfs_path, FileMode.Open, FileAccess.Read);
                    byte[] bs_body_s = new byte[fsr_body_s.Length];
                    fsr_body_s.Read(bs_body_s, 0, bs_body_s.Length);
                    fsr_body_s.Close();

                    int TXS3_header_pointer = Getbyteint4(bs_body, 72);
                    int TXS3_count = Getbyteint4(bs_body, TXS3_header_pointer + 20);
                    filecount = TXS3_count;
                    int TXS3_pointer = TXS3_header_pointer + 64;
                    int TXS3_tex_seek = Getbyteint4(bs_body_s, 4);

                    byte[] MDL3 = new byte[4];
                    Array.Copy(bs_body, 0, MDL3, 0, 4);
                    string MDL3_str = System.Text.Encoding.ASCII.GetString(MDL3);
                    if (MDL3_str != "MDL3")
                        goto labelfinish;

                    int writefilecount = 0;

                    for (a = 0; a < TXS3_count; a++)
                    {
                        string DXT = Getbytestr1(bs_body, TXS3_pointer + 9);
                        if (DXT == "A5")
                            goto labelTXS3finish;
                        else
                            writefilecount += 1;

                        byte[] TXS3_yoko = new byte[2];
                        Array.Copy(bs_body, TXS3_pointer + 12, TXS3_yoko, 0, 2);
                        int TXS3_yoko_int = Getbyteint2(TXS3_yoko, 0);
                        byte[] TXS3_tate = new byte[2];
                        Array.Copy(bs_body, TXS3_pointer + 14, TXS3_tate, 0, 2);
                        int TXS3_tate_int = Getbyteint2(TXS3_tate, 0);

                        int TXS3_tex_length = Getbyteint4(bs_body, TXS3_pointer + 4);
                        //if (TXS3_tex_length == 349568)
                            //goto loopfinish;
                        byte[] TXS3_tex = new byte[TXS3_tex_length];

                        /*
                        int TXS3_tex_length_TXS3 = TXS3_yoko_int * TXS3_tate_int;
                        if (DXT == "86")
                            TXS3_tex_length_TXS3 /= 2;
                        //DXTの種類でサイズが異なる
                        if (c == 0)
                            MessageBox.Show((a + 1).ToString() + " " + TXS3_tex_length.ToString("X") + " " + TXS3_tex_length_TXS3.ToString("X") + " " + TXS3_tex_seek.ToString("X"));
                        */

                        Array.Copy(bs_body_s, TXS3_tex_seek, TXS3_tex, 0, TXS3_tex_length);

                        byte[] TXS3_header = new byte[4];
                        byte[] TXS3 = new byte[4] { 0x54, 0x58, 0x53, 0x33 };
                        Array.Copy(TXS3, 0, TXS3_header, 0, 4);

                        byte[] TXS3_length = Gethex4(256 + TXS3_tex_length);
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 4);
                        Array.Copy(TXS3_length, 0, TXS3_header, TXS3_header.Length - 4, 4);

                        Array.Resize(ref TXS3_header, TXS3_header.Length + 4);
                        Array.Copy(zero4, 0, TXS3_header, TXS3_header.Length - 4, 4);
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 4);
                        Array.Copy(TXS3_length, 0, TXS3_header, TXS3_header.Length - 4, 4);

                        byte[] TXS3_unk1 = new byte[16] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x84 };
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 16);
                        Array.Copy(TXS3_unk1, 0, TXS3_header, TXS3_header.Length - 16, 16);

                        /*
                        //GT6用?
                        if (DXT == "")
                        {
                            byte[] TXS3_DXT = new byte[4] { 0x00, 0x00, 0x01, 0x00 };
                            Array.Resize(ref TXS3_header, TXS3_header.Length + 4);
                            Array.Copy(TXS3_DXT, 0, TXS3_header, TXS3_header.Length - 4, 4);
                        }
                        */

                        for (int i = 0; i < 8; i++)
                        {
                            Array.Resize(ref TXS3_header, TXS3_header.Length + 4);
                            Array.Copy(zero4, 0, TXS3_header, TXS3_header.Length - 4, 4);
                        }

                        byte[] TXS3_unk2 = new byte[4] { 0x00, 0x00, 0x1A, 0x00 };
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 4);
                        Array.Copy(TXS3_unk2, 0, TXS3_header, TXS3_header.Length - 4, 4);
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 4);
                        Array.Copy(zero4, 0, TXS3_header, TXS3_header.Length - 4, 4);

                        byte[] TXS3_unk3 = new byte[2] { 0x00, 0x01 };
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 2);
                        Array.Copy(TXS3_unk3, 0, TXS3_header, TXS3_header.Length - 2, 2);

                        byte[] TXS3_DXT1 = new byte[1] { 0xA6 };
                        byte[] TXS3_DXT3 = new byte[1] { 0xA7 };
                        byte[] TXS3_DXT5 = new byte[1] { 0xA8 };
                        byte[] TXS3_DXT10 = new byte[1] { 0xA5 };
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 1);

                        if (DXT == "86")
                            Array.Copy(TXS3_DXT1, 0, TXS3_header, TXS3_header.Length - 1, 1);
                        else if (DXT == "87")
                            Array.Copy(TXS3_DXT3, 0, TXS3_header, TXS3_header.Length - 1, 1);
                        else if (DXT == "88")
                            Array.Copy(TXS3_DXT5, 0, TXS3_header, TXS3_header.Length - 1, 1);
                        else if (DXT == "85")
                            Array.Copy(TXS3_DXT10, 0, TXS3_header, TXS3_header.Length - 1, 1);

                        byte[] TXS3_unk4 = new byte[1] { 0x2A };
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 1);
                        Array.Copy(TXS3_unk4, 0, TXS3_header, TXS3_header.Length - 1, 1);

                        byte[] TXS3_unk5 = new byte[4] { 0x00, 0x03, 0x03, 0x03 };
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 4);
                        Array.Copy(TXS3_unk5, 0, TXS3_header, TXS3_header.Length - 4, 4);

                        byte[] TXS3_unk6 = new byte[12] { 0x80, 0x07, 0x80, 0x00, 0x00, 0x00, 0xAA, 0xE4, 0x02, 0x02, 0x20, 0x00 };
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 12);
                        Array.Copy(TXS3_unk6, 0, TXS3_header, TXS3_header.Length - 12, 12);

                        /*TXS3converter1.1.3
                        if (DXT == "86")
                        {
                            TXS3_yoko_int /= 2;
                            TXS3_yoko = Gethex2(TXS3_yoko_int);
                        }
                        */

                        Array.Resize(ref TXS3_header, TXS3_header.Length + 2);
                        Array.Copy(TXS3_yoko, 0, TXS3_header, TXS3_header.Length - 2, 2);
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 2);
                        Array.Copy(TXS3_tate, 0, TXS3_header, TXS3_header.Length - 2, 2);

                        byte[] TXS3_unk7 = new byte[10] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x18, 0x40, 0x00, 0x10 };
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 10);
                        Array.Copy(TXS3_unk7, 0, TXS3_header, TXS3_header.Length - 10, 10);

                        int TXS3_yoko_4 = Getbyteint2(TXS3_yoko, 0) / 4;
                        string TXS3_yoko_4_str = TXS3_yoko_4.ToString("X");
                        TXS3_yoko_4_str += "0";
                        if (TXS3_yoko_4_str.Length == 1 || TXS3_yoko_4_str.Length == 3)
                            TXS3_yoko_4_str = "0" + TXS3_yoko_4_str;
                        if (TXS3_yoko_4_str.Length == 2)
                            TXS3_yoko_4_str = "00" + TXS3_yoko_4_str;

                        byte[] TXS3_yoko_4_byte = StringToBytes(TXS3_yoko_4_str);
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 2);
                        Array.Copy(TXS3_yoko_4_byte, 0, TXS3_header, TXS3_header.Length - 2, 2);

                        for (int i = 0; i < 6; i++)
                        {
                            Array.Resize(ref TXS3_header, TXS3_header.Length + 4);
                            Array.Copy(zero4, 0, TXS3_header, TXS3_header.Length - 4, 4);
                        }

                        byte[] TXS3_unk8 = new byte[4] { 0x00, 0x00, 0x01, 0x00 };
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 4);
                        Array.Copy(TXS3_unk8, 0, TXS3_header, TXS3_header.Length - 4, 4);

                        string TXS3_unk9_str = BitConverter.ToString(TXS3_length).Replace("-", string.Empty);
                        TXS3_unk9_str = TXS3_unk9_str.Substring(0, TXS3_unk9_str.Length - 2);
                        TXS3_unk9_str = TXS3_unk9_str + "00";
                        int TXS3_unk9_int = Convert.ToInt32(TXS3_unk9_str, 16);
                        TXS3_unk9_int -= 256;
                        TXS3_unk9_str = TXS3_unk9_int.ToString("X");

                        int add0 = 8 - TXS3_unk9_str.Length;
                        if (add0 > 0)
                        {
                            for (int i = 0; i < add0; i++)
                            {
                                TXS3_unk9_str = "0" + TXS3_unk9_str;
                            }
                        }

                        byte[] TXS3_unk9 = StringToBytes(TXS3_unk9_str);
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 4);
                        Array.Copy(TXS3_unk9, 0, TXS3_header, TXS3_header.Length - 4, 4);

                        byte[] TXS3_unk10 = new byte[1] { 0x02 };
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 1);
                        Array.Copy(TXS3_unk10, 0, TXS3_header, TXS3_header.Length - 1, 1);

                        Array.Resize(ref TXS3_header, TXS3_header.Length + 1);
                        if (DXT == "86")
                            Array.Copy(TXS3_DXT1, 0, TXS3_header, TXS3_header.Length - 1, 1);
                        else if (DXT == "88")
                            Array.Copy(TXS3_DXT5, 0, TXS3_header, TXS3_header.Length - 1, 1);
                        else if (DXT == "85")
                            Array.Copy(TXS3_DXT10, 0, TXS3_header, TXS3_header.Length - 1, 1);

                        byte[] TXS3_unk11 = new byte[1] { 0x01 };
                        for (int i = 0; i < 2; i++)
                        {
                            Array.Resize(ref TXS3_header, TXS3_header.Length + 1);
                            Array.Copy(TXS3_unk11, 0, TXS3_header, TXS3_header.Length - 1, 1);
                        }

                        Array.Resize(ref TXS3_header, TXS3_header.Length + 2);
                        Array.Copy(TXS3_yoko, 0, TXS3_header, TXS3_header.Length - 2, 2);
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 2);
                        Array.Copy(TXS3_tate, 0, TXS3_header, TXS3_header.Length - 2, 2);

                        byte[] TXS3_unk12 = new byte[4] { 0x00, 0x01, 0x00, 0x00 };
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 4);
                        Array.Copy(TXS3_unk12, 0, TXS3_header, TXS3_header.Length - 4, 4);

                        for (int i = 0; i < 26; i++)
                        {
                            Array.Resize(ref TXS3_header, TXS3_header.Length + 4);
                            Array.Copy(zero4, 0, TXS3_header, TXS3_header.Length - 4, 4);
                        }

                        //上下反転(むり)
                        /*
                        int TXS3_tate_int = Getbyteint2(TXS3_tate, 0);
                        int TXS3_narabikae_seek = TXS3_tex_length - 8;
                        byte[] TXS3_tex_new = new byte[0];
                        //MessageBox.Show(TXS3_yoko_int + "," + TXS3_tate_int);
                        for (int i = 0; i < TXS3_tex_length / 8; i++)
                        {
                            Array.Resize(ref TXS3_tex_new, TXS3_tex_new.Length + 8);
                            Array.Copy(TXS3_tex, TXS3_narabikae_seek, TXS3_tex_new, TXS3_tex_new.Length - 8, 8);
                            TXS3_narabikae_seek -= 8;
                        }
                        */

                        string newpath = folderpath + @"\" + writefilecount + "_";
                        if (DXT == "86")
                            newpath = newpath + "DXT1";
                        else if (DXT == "87")
                            newpath = newpath + "DXT3";
                        else if (DXT == "88")
                            newpath = newpath + "DXT5";
                        else if (DXT == "85")
                            newpath = newpath + "DXT10";
                        newpath = newpath + ".img";

                        FileStream fsw = new FileStream(newpath, FileMode.Create, FileAccess.Write);
                        fsw.Write(TXS3_header, 0, TXS3_header.Length);
                        fsw.Write(TXS3_tex, 0, TXS3_tex.Length);
                        fsw.Close();

                        TXS3_tex_seek += TXS3_tex_length;

                    labelTXS3finish:;

                        TXS3_pointer += 32;
                        bgWorker.ReportProgress(a);
                    }
                loopfinish:;
                }
                goto labelfinish;

            labelfolder_comp:;

                string[] img_files = null;
                var isDirectory = System.IO.File.GetAttributes(path[b]).HasFlag(FileAttributes.Directory);
                if (isDirectory == true)
                {
                    img_files = Directory.GetFiles(path[b], "*.img");
                    Array.Sort(img_files, new LogicalStringComparer());

                    MDL3path = MDL3path + @"\body_s.bin";
                    /*
                    MDL3path = MDL3path.Remove(MDL3path.LastIndexOf("_texture"), 8);
                    if (MDL3path.LastIndexOf("_hq") - 3 == 0)
                    {
                        MDL3path = MDL3path.Remove(MDL3path.LastIndexOf("_hq"), 3);
                        MDL3path = MDL3path + @"\hq\body_s.bin";
                    }
                    else if (MDL3path.LastIndexOf("_race") - 5 == 0)
                    {
                        MDL3path = MDL3path.Remove(MDL3path.LastIndexOf("_race"), 5);
                        MDL3path = MDL3path + @"\race\body_s.bin";
                    }
                    */
                    if (!System.IO.File.Exists(MDL3path))
                        goto labelfinish;
                }
            /*
            for (int c = 0; c < loopcount; c++)
            {
                string folderpath_file = path[b];

                folderpath_file = folderpath_file.Remove(folderpath_file.LastIndexOf("_texture"), 8);

                if (!System.IO.File.Exists(folderpath_file))
                    goto labelfinish;

                string folderpath_newfile = Path.GetDirectoryName(folderpath_file) + @"\new\" + Path.GetFileName(folderpath_file);
                if (!Directory.Exists(Path.GetDirectoryName(folderpath_file) + @"\new\"))
                    Directory.CreateDirectory(Path.GetDirectoryName(folderpath_file) + @"\new\");

                FileStream fsw_fo = new FileStream(folderpath_newfile, FileMode.Create, FileAccess.Write);
                fsw_fo.Write(bs, 0, bs.Length);
                int newfilecount = -1;

                for (a = 0; a < img_files.Count(); a++)
                {
                labelnewfilestart:;

                    string DXT = Getbytestr1(bs, TXS3_pointer + 9);
                    if (DXT == "A5")
                    {
                        TXS3_pointer += 32;
                        goto labelnewfilestart;
                    }
                    else
                        newfilecount += 1;

                    TXS3_tex_seek = Getbyteint4(bs, TXS3_pointer);
                    int TXS3_tex_length = Getbyteint4(bs, TXS3_pointer + 4);

                    byte[] TXS3_tex_new = new byte[0];
                    //img読み込み
                    FileStream fsr_img = new FileStream(img_files[newfilecount], FileMode.Open, FileAccess.Read);
                    byte[] bs_img = new byte[fsr_img.Length];
                    fsr_img.Read(bs_img, 0, bs_img.Length);
                    fsr_img.Close();
                    Array.Resize(ref TXS3_tex_new, bs_img.Length - 256);
                    Array.Copy(bs_img, 256, TXS3_tex_new, 0, bs_img.Length - 256);
                    fsw_fo.Seek(TXS3_tex_seek, SeekOrigin.Begin);
                    fsw_fo.Write(TXS3_tex_new, 0, TXS3_tex_new.Length);

                    TXS3_pointer += 32;
                    bgWorker.ReportProgress(a);
                }
                fsw_fo.Close();
            }
            */

            labelfinish:;
            }
        }

        //byte配列1バイトをintに変換して戻す
        public static int Getbyteint1(byte[] bytes, int seek)
        {
            byte[] byte1 = new byte[1];
            Array.Copy(bytes, seek, byte1, 0, 1);
            string str1 = BitConverter.ToString(byte1);
            int returnint = Convert.ToInt32(str1, 16);

            return returnint;
        }

        //byte配列2バイト(ビッグエンディアン)をintに変換して戻す
        public static int Getbyteint2(byte[] bytes, int seek)
        {
            byte[] byte1 = new byte[1];
            Array.Copy(bytes, seek, byte1, 0, 1);
            byte[] byte2 = new byte[1];
            Array.Copy(bytes, seek + 1, byte2, 0, 1);

            string str1 = BitConverter.ToString(byte1);
            string str2 = BitConverter.ToString(byte2);
            int bytelength = 0;

            if (str1 != "00")
            {
                bytelength = 2;
                goto label_byteget;
            }
            else if (str2 != "00")
            {
                bytelength = 1;
                goto label_byteget;
            }

            else
                return 0;

            label_byteget:;

            string str16 = "";
            if (bytelength == 1)
                str16 = str2;
            else if (bytelength == 2)
                str16 = str1 + str2;

            int returnint = Convert.ToInt32(str16, 16);

            return returnint;
        }

        //byte配列4バイト(ビッグエンディアン)をintに変換して戻す
        public static int Getbyteint4(byte[] bytes, int seek)
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

            if (str1 != "00")
            {
                bytelength = 4;
                goto label_byteget;
            }
            else if (str2 != "00")
            {
                bytelength = 3;
                goto label_byteget;
            }
            else if (str3 != "00")
            {
                bytelength = 2;
                goto label_byteget;
            }
            else if (str4 != "00")
            {
                bytelength = 1;
                goto label_byteget;
            }

            else
                return 0;

            label_byteget:;

            string str16 = "";
            if (bytelength == 1)
                str16 = str4;
            else if (bytelength == 2)
                str16 = str3 + str4;
            else if (bytelength == 3)
                str16 = str2 + str3 + str4;
            else if (bytelength == 4)
                str16 = str1 + str2 + str3 + str4;

            int returnint = Convert.ToInt32(str16, 16);

            return returnint;
        }

        //byte配列1バイトをstringに変換して戻す
        public static string Getbytestr1(byte[] bytes, int seek)
        {
            byte[] byte1 = new byte[1];
            Array.Copy(bytes, seek, byte1, 0, 1);

            string str1 = BitConverter.ToString(byte1);
            return str1;
        }

        //byte配列4バイトをstringに変換して戻す
        public static string Getbytestr4(byte[] bytes, int seek)
        {
            byte[] byte1 = new byte[4];
            Array.Copy(bytes, seek, byte1, 0, 4);

            string str = BitConverter.ToString(byte1).Replace("-", string.Empty);
            return str;
        }

        //intをbyte配列2バイト(ビッグエンディアン)に変換して戻す
        public static byte[] Gethex2(int hex)
        {
            string hexstr = hex.ToString("X");
            if (hexstr.Length == 1 || hexstr.Length == 3)
                hexstr = "0" + hexstr;

            if (hexstr.Length == 2)
                hexstr = "00" + hexstr;

            byte[] hexbyte = StringToBytes(hexstr);
            return hexbyte;
        }

        //intをbyte配列4バイト(ビッグエンディアン)に変換して戻す
        public static byte[] Gethex4(int hex)
        {
            string hexstr = hex.ToString("X");
            if (hexstr.Length == 1 || hexstr.Length == 3 || hexstr.Length == 5 || hexstr.Length == 7)
                hexstr = "0" + hexstr;

            if (hexstr.Length == 2)
                hexstr = "000000" + hexstr;

            else if (hexstr.Length == 4)
                hexstr = "0000" + hexstr;

            else if (hexstr.Length == 6)
                hexstr = "00" + hexstr;

            byte[] hexbyte = StringToBytes(hexstr);
            return hexbyte;
        }

        // 16進数文字列 => Byte配列
        public static byte[] StringToBytes(string str)
        {
            var bs = new List<byte>();
            for (int i = 0; i < str.Length / 2; i++)
            {
                bs.Add(Convert.ToByte(str.Substring(i * 2, 2), 16));
            }
            return bs.ToArray();
        }

        /// <summary>
        /// 大文字小文字を区別せずに、
        /// 文字列内に含まれている数字を考慮して文字列を比較します。
        /// </summary>
        public class LogicalStringComparer :
            System.Collections.IComparer,
            System.Collections.Generic.IComparer<string>
        {
            [System.Runtime.InteropServices.DllImport("shlwapi.dll",
                CharSet = System.Runtime.InteropServices.CharSet.Unicode,
                ExactSpelling = true)]
            private static extern int StrCmpLogicalW(string x, string y);

            public int Compare(string x, string y)
            {
                return StrCmpLogicalW(x, y);
            }

            public int Compare(object x, object y)
            {
                return this.Compare(x.ToString(), y.ToString());
            }
        }

        public byte[] ArrayReverse(byte[] array)
        {
            Array.Reverse(array);
            return array;
        }

        public void extractMDL3(ref FileStream fs, ref FileInfo fi, bool isfolder_body_s)
        {
            //string extractedPath = fi.DirectoryName + "\\extracted_mdl3";
            //Directory.CreateDirectory(extractedPath);

            byte[] infoBuffer1 = new byte[1];
            byte[] infoBuffer2 = new byte[2];
            byte[] infoBuffer4 = new byte[4];

            fs.Read(infoBuffer4, 0, 4); //magic
            fs.Read(infoBuffer4, 0, 4); //MDL3 Total Size
            var mdl3Size = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);

            //Get meshInfoTable and sTable count
            fs.Seek(0x14, SeekOrigin.Begin);
            fs.Read(infoBuffer2, 0, 2);
            uint sTableCount = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);

            //Enumerate Mesh Info Table
            List<List<uint>> meshInfoTable = new List<List<uint>>();
            fs.Seek(0x38, SeekOrigin.Begin);
            fs.Read(infoBuffer4, 0, 4);
            uint meshInfoTableAddress = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
            fs.Seek(meshInfoTableAddress, SeekOrigin.Begin);
            for (int i = 0; i < sTableCount; i++)
            {
                List<uint> meshInfoEnum = new List<uint>();
                fs.Read(infoBuffer1, 0, 1); meshInfoEnum.Add(infoBuffer1[0]);   //unk flag 1
                fs.Read(infoBuffer1, 0, 1); meshInfoEnum.Add(infoBuffer1[0]);   //unk flag 2
                fs.Read(infoBuffer2, 0, 2); uint meshIndex = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0); meshInfoEnum.Add(meshIndex);
                fs.Read(infoBuffer2, 0, 2); uint unkIndex = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0); meshInfoEnum.Add(unkIndex);
                fs.Read(infoBuffer2, 0, 2); uint null_MIT1 = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0); meshInfoEnum.Add(null_MIT1);
                fs.Read(infoBuffer4, 0, 4); uint vertexCount = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); meshInfoEnum.Add(vertexCount);
                fs.Read(infoBuffer4, 0, 4); uint null_MIT2 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); meshInfoEnum.Add(null_MIT2);
                fs.Read(infoBuffer4, 0, 4); uint null_MIT3 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); meshInfoEnum.Add(null_MIT3);
                fs.Read(infoBuffer4, 0, 4); uint faceIndexCount = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); meshInfoEnum.Add(faceIndexCount);
                fs.Read(infoBuffer4, 0, 4); uint null_MIT4 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); meshInfoEnum.Add(null_MIT4);
                fs.Read(infoBuffer4, 0, 4); uint null_MIT5 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); meshInfoEnum.Add(null_MIT5);
                fs.Read(infoBuffer4, 0, 4); uint null_MIT6 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); meshInfoEnum.Add(null_MIT6);
                fs.Read(infoBuffer1, 0, 1); meshInfoEnum.Add(infoBuffer1[0]);   //unk flag 3
                fs.Read(infoBuffer1, 0, 1); meshInfoEnum.Add(infoBuffer1[0]);   //unk flag 4
                fs.Read(infoBuffer2, 0, 2); uint unkCount1 = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0); meshInfoEnum.Add(unkCount1);
                fs.Read(infoBuffer4, 0, 4); uint null_MIT7 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); meshInfoEnum.Add(null_MIT7);
                fs.Read(infoBuffer4, 0, 4); uint null_MIT8 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); meshInfoEnum.Add(null_MIT8);
                meshInfoTable.Add(meshInfoEnum);
            }

            //Get fvfTable Count
            fs.Seek(0x18, SeekOrigin.Begin);
            fs.Read(infoBuffer2, 0, 2);
            uint fvfTableCount = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);

            //Enumerate fvfTable
            List<List<uint>> fvfTable = new List<List<uint>>();
            fs.Seek(0x40, SeekOrigin.Begin);
            fs.Read(infoBuffer4, 0, 4);
            uint fvfTableAddress = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
            fs.Seek(fvfTableAddress, SeekOrigin.Begin);
            for (int i = 0; i < fvfTableCount; i++)
            {
                List<uint> fvfTableEnum = new List<uint>();
                //def loadfvf(self, bs):
                //#FVF Table
                //for a in range(0, self.fvfCount):
                fs.Seek((fvfTableAddress + (i * 0x78)), SeekOrigin.Begin);  //bs.seek(self.fvfOffst + (a * 0x78), NOESEEK_ABS)
                                                                            //print(bs.tell())
                                                                            //fvfData = bs.read(">" + "6I4B23I")
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer1, 0, 1); fvfTableEnum.Add(infoBuffer1[0]);
                fs.Read(infoBuffer1, 0, 1); fvfTableEnum.Add(infoBuffer1[0]);
                fs.Read(infoBuffer1, 0, 1); fvfTableEnum.Add(infoBuffer1[0]);
                fs.Read(infoBuffer1, 0, 1); fvfTableEnum.Add(infoBuffer1[0]);
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                fs.Read(infoBuffer4, 0, 4); fvfTableEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));

                fs.Seek(fvfTableEnum[0], SeekOrigin.Begin);

                fvfTableEnum.Add((uint)fs.Position);

                fvfTable.Add(fvfTableEnum);
            }

            //bp++;

            //Get PMSH Address
            fs.Seek(0xD0, SeekOrigin.Begin);
            fs.Read(infoBuffer4, 0, 4);
            var pmshAddress = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);

            //Get zlibInfoAddress
            fs.Seek(0xDC, SeekOrigin.Begin);
            fs.Read(infoBuffer4, 0, 4);
            var zlibInfoAddress = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);

            ushort pmshTable1Count = 0;
            ushort pmshTable2Count = 0;
            ushort pmshTable3Count = 0;
            ushort pmshTable4Count = 0;
            uint pmshTable1Address = 0;
            uint pmshTable2Address = 0;
            uint pmshTable3Address = 0;
            uint pmshTable4Address = 0;
            //Parse PMSH
            fs.Seek(pmshAddress, SeekOrigin.Begin);
            fs.Read(infoBuffer4, 0, 4); //PMSH Magic
            fs.Read(infoBuffer2, 0, 2);
            fs.Read(infoBuffer2, 0, 2);
            fs.Read(infoBuffer4, 0, 4); //PMSH Address from beginning of MDL3 file
            fs.Read(infoBuffer4, 0, 4); //padding?
            fs.Read(infoBuffer2, 0, 2);
            pmshTable1Count = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
            fs.Read(infoBuffer2, 0, 2);
            pmshTable2Count = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
            fs.Read(infoBuffer2, 0, 2);
            pmshTable3Count = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
            fs.Read(infoBuffer2, 0, 2);
            pmshTable4Count = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
            fs.Read(infoBuffer4, 0, 4);
            pmshTable1Address = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
            fs.Read(infoBuffer4, 0, 4);
            pmshTable2Address = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
            fs.Read(infoBuffer4, 0, 4);
            pmshTable3Address = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
            fs.Read(infoBuffer4, 0, 4);
            pmshTable4Address = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);

            if (pmshTable1Count > 0)
            {
                fs.Seek(pmshTable1Address, SeekOrigin.Begin);

            }

            if (pmshTable2Count > 0)
            {
                fs.Seek(pmshTable2Address, SeekOrigin.Begin);
            }

            if (pmshTable3Count > 0)
            {
                fs.Seek(pmshTable3Address, SeekOrigin.Begin);
            }


            if (pmshTable4Count > 0)
            {
                List<uint> stringTableInfoPointers = new List<uint>();

                fs.Seek(pmshTable4Address, SeekOrigin.Begin);

                for (int i = 0; i < pmshTable4Count; i++)
                {
                    fs.Read(infoBuffer1, 0, 1); //flag1
                    fs.Read(infoBuffer1, 0, 1); //flag2
                    fs.Read(infoBuffer1, 0, 1); //flag3
                    fs.Read(infoBuffer1, 0, 1); //flag4
                    fs.Read(infoBuffer4, 0, 4); //unk1
                    fs.Read(infoBuffer4, 0, 4); //stringTableInfoPointer
                    stringTableInfoPointers.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                    fs.Read(infoBuffer4, 0, 4); //unk2
                }

                for (int i = 0; i < pmshTable4Count; i++)
                {
                    fs.Seek(stringTableInfoPointers[i], SeekOrigin.Begin);
                    fs.Read(infoBuffer2, 0, 2); // ushort unk01;
                    fs.Read(infoBuffer2, 0, 2); //ushort unk02;
                    fs.Read(infoBuffer2, 0, 2); //ushort unk03;
                    fs.Read(infoBuffer2, 0, 2); //ushort unk04;
                    fs.Read(infoBuffer2, 0, 2); //ushort unk05;
                    fs.Read(infoBuffer2, 0, 2); //ushort unk06;
                    fs.Read(infoBuffer2, 0, 2); //ushort unk07;
                    fs.Read(infoBuffer1, 0, 1); int entryType = infoBuffer1[0];
                    fs.Read(infoBuffer1, 0, 1); //byte unk08;
                    if (entryType == -2)
                    {
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL01;
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL02;
                        fs.Read(infoBuffer2, 0, 2); //ushort unkS03;
                        fs.Read(infoBuffer2, 0, 2); //ushort unkS04;
                        fs.Read(infoBuffer2, 0, 2); //ushort unkS05;
                        fs.Read(infoBuffer2, 0, 2); //ushort unkS06;
                    }
                    if (entryType == -94)
                    {
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL01;
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL02;
                        fs.Read(infoBuffer2, 0, 2); //ushort unkS03;
                        fs.Read(infoBuffer2, 0, 2); //ushort unkS04;
                        fs.Read(infoBuffer2, 0, 2); //ushort unkS05;
                        fs.Read(infoBuffer2, 0, 2); //ushort unkS06;
                    }
                    if (entryType == 2)
                    {
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL01;
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL02;
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL03;
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL04;
                    }
                    if (entryType == 4)
                    {
                        fs.Read(infoBuffer2, 0, 2); //ushort unkS01;
                        fs.Read(infoBuffer2, 0, 2); //ushort unkS02;
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL02;
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL03;
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL04;
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL05;
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL06;
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL07;
                        fs.Read(infoBuffer4, 0, 4); //ulong unkL08;
                    }
                    if (i == (pmshTable4Count - 1))
                    {
                        //bp++;
                    }
                }

                //Read data info start
                fs.Seek(zlibInfoAddress, SeekOrigin.Begin);

                dfs_path = fi.DirectoryName + "\\body_s.bin";
                //string dfs_path_check = "";
                if (isfolder_body_s == true)
                {
                    int foundIndex = dfs_path.LastIndexOf(@"\");
                    int nextIndex = foundIndex - 1;
                    if (nextIndex < dfs_path.Length)
                        foundIndex = dfs_path.LastIndexOf(@"\", nextIndex);

                    dfs_path = dfs_path.Remove(foundIndex);
                    string path_hq = fi.DirectoryName.ToString().Remove(0, fi.DirectoryName.Length - 2);
                    string path_race = fi.DirectoryName.ToString().Remove(0, fi.DirectoryName.Length - 4);
                    if (path_hq == "hq")
                    {
                        dfs_path = dfs_path + "_hq_texture" + @"\body_s.bin";
                        /*
                        dfs_path_check = dfs_path + @"_ex\";
                        if (!Directory.Exists(dfs_path_check + @"cmp\"))
                            Directory.CreateDirectory(dfs_path_check + @"cmp\");
                        if (!Directory.Exists(dfs_path_check + @"dcmp\"))
                            Directory.CreateDirectory(dfs_path_check + @"dcmp\");
                        */
                    }
                    if (path_race == "race")
                    {
                        dfs_path = dfs_path + "_race_texture" + @"\body_s.bin";
                        /*
                        dfs_path_check = dfs_path + @"_ex\";
                        if (!Directory.Exists(dfs_path_check + @"cmp\"))
                            Directory.CreateDirectory(dfs_path_check + @"cmp\");
                        if (!Directory.Exists(dfs_path_check + @"dcmp\"))
                            Directory.CreateDirectory(dfs_path_check + @"dcmp\");
                        */
                    }
                }

                //FileStream zfs = new FileStream(fi.DirectoryName + "\\body_s.multiZlib", FileMode.Open, FileAccess.Read);
                FileStream zfs = new FileStream(fi.DirectoryName + "\\body_s", FileMode.Open, FileAccess.Read);
                FileStream dfs = new FileStream(dfs_path, FileMode.Create, FileAccess.Write);

                fs.Read(infoBuffer2, 0, 2); uint zlibInfo0 = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                fs.Read(infoBuffer2, 0, 2); uint zlibInfo1 = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                fs.Read(infoBuffer2, 0, 2); uint zlibInfo2 = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                fs.Read(infoBuffer2, 0, 2); uint zlibInfo3 = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                fs.Read(infoBuffer4, 0, 4); uint zlibInfo4 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
                fs.Read(infoBuffer4, 0, 4); uint zlibInfo5 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);

                for (int i = 0; i < zlibInfo1; i++)
                {
                    fs.Seek((zlibInfo4 + (0xC * i)), SeekOrigin.Begin);
                    fs.Read(infoBuffer4, 0, 4); uint zlibHId = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
                    fs.Read(infoBuffer4, 0, 4); uint zlibHCount = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
                    fs.Read(infoBuffer4, 0, 4); uint zlibHoff = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);

                    for (int j = 0; j < zlibHCount; j++)
                    {
                        fs.Seek((zlibHoff + (0x18 * j)), SeekOrigin.Begin);
                        fs.Read(infoBuffer4, 0, 4); uint null00 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
                        fs.Read(infoBuffer4, 0, 4); uint chunkStart = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
                        fs.Read(infoBuffer2, 0, 2); uint chunkCount00 = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                        fs.Read(infoBuffer2, 0, 2); uint chunkCount01 = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                        fs.Read(infoBuffer4, 0, 4); uint chunkInfoOff00 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
                        fs.Read(infoBuffer2, 0, 2); uint chunkCount02 = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                        fs.Read(infoBuffer2, 0, 2); uint chunkCount03 = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                        fs.Read(infoBuffer4, 0, 4); uint chunkInfoOff01 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);

                        for (int k = 0; k < (chunkCount00 + chunkCount01); k++)
                        {
                            fs.Seek((chunkInfoOff00 + (0x8 * k)), SeekOrigin.Begin);
                            fs.Read(infoBuffer2, 0, 2); uint zFlag = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                            fs.Read(infoBuffer2, 0, 2); uint zSize = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);

                            //MessageBox.Show((chunkInfoOff00 + (0x8 * k)).ToString());
                            fs.Read(infoBuffer4, 0, 4); uint uSize = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);

                            zfs.Seek(chunkStart, SeekOrigin.Begin);
                            byte[] cmpData = new byte[zSize];
                            zfs.Read(cmpData, 0, cmpData.Length);

                            if (zFlag == 0)
                            {
                                dfs.Write(cmpData, 0, cmpData.Length);
                            }
                            else
                            {
                                //efs.Write(cmpData, 0, cmpData.Length);
                                byte[] dcmpData = Ionic.Zlib.DeflateStream.UncompressBuffer(cmpData);
                                dfs.Write(dcmpData, 0, dcmpData.Length);
                                
                                /*
                                FileStream dfs_check_cmp = new FileStream(dfs_path_check + @"cmp\" + k, FileMode.Create, FileAccess.Write);
                                dfs_check_cmp.Write(cmpData, 0, cmpData.Length);
                                dfs_check_cmp.Close();
                                FileStream dfs_check_dcmp = new FileStream(dfs_path_check + @"dcmp\" + k, FileMode.Create, FileAccess.Write);
                                dfs_check_dcmp.Write(dcmpData, 0, dcmpData.Length);
                                dfs_check_dcmp.Close();
                                */
                            }
                            chunkStart += zSize;
                        }

                        for (int k = 0; k < (chunkCount02 + chunkCount03); k++)
                        {
                            fs.Seek((chunkInfoOff01 + (0x10 * k)), SeekOrigin.Begin);
                            fs.Read(infoBuffer4, 0, 4); uint null00a = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
                            fs.Read(infoBuffer4, 0, 4); uint chunkStart2 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
                            fs.Read(infoBuffer2, 0, 2); uint chunkCount04 = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                            fs.Read(infoBuffer2, 0, 2); uint chunkCount05 = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                            fs.Read(infoBuffer4, 0, 4); uint zlibHoff2 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);

                            for (int l = 0; l < (chunkCount04 + chunkCount05); l++)
                            {
                                fs.Seek((zlibHoff2 + (0x8 * l)), SeekOrigin.Begin);
                                fs.Read(infoBuffer2, 0, 2); uint zFlag = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                                fs.Read(infoBuffer2, 0, 2); uint zSize = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                                fs.Read(infoBuffer4, 0, 4); uint uSize = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);

                                zfs.Seek(chunkStart2, SeekOrigin.Begin);
                                byte[] cmpData = new byte[zSize];
                                zfs.Read(cmpData, 0, cmpData.Length);
                                if (zFlag == 0)
                                {
                                    dfs.Write(cmpData, 0, cmpData.Length);
                                }
                                else
                                {
                                    byte[] dcmpData = Ionic.Zlib.DeflateStream.UncompressBuffer(cmpData);
                                    dfs.Write(dcmpData, 0, dcmpData.Length);
                                }
                                chunkStart2 += zSize;
                            }
                        }
                    }
                }

                zfs.Close();
                dfs.Close();
            }
        }

        public void compressMDL3(ref FileStream fs, ref FileInfo fi, bool isfolder_body_s)
        {
            FileStream dfs = new FileStream(dfs_path, FileMode.Create, FileAccess.Write);
        }
    }
}
