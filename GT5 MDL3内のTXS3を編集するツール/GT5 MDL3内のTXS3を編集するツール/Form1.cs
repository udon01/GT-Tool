﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace GT5_MDL3内のTXS3を編集するツール
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

                string[] img_files = null;
                string MDL3path = path[b];
                
                var isDirectory = System.IO.File.GetAttributes(path[b]).HasFlag(FileAttributes.Directory);
                if (isDirectory == true)
                {
                    img_files = Directory.GetFiles(path[b], "*.img");
                    Array.Sort(img_files, new LogicalStringComparer());

                    MDL3path = MDL3path.Remove(MDL3path.LastIndexOf("_texture"), 8);
                    if (!System.IO.File.Exists(MDL3path))
                        goto labelfinish;
                }
                
                // MDL3ファイル
                FileStream fsr = new FileStream(MDL3path, FileMode.Open, FileAccess.Read);
                byte[] bs = new byte[fsr.Length];
                fsr.Read(bs, 0, bs.Length);
                fsr.Close();

                int TXS3_header_pointer = Getbyteint4(bs, 72);
                int TXS3_count = Getbyteint4(bs, TXS3_header_pointer + 20);
                filecount = TXS3_count;
                int TXS3_pointer = TXS3_header_pointer + 64;
                int TXS3_tex_seek = Getbyteint4(bs, TXS3_pointer);

                if (isDirectory == true)
                    goto labelfolder;

                byte[] MDL3 = new byte[4];
                Array.Copy(bs, 0, MDL3, 0, 4);
                string MDL3_str = System.Text.Encoding.ASCII.GetString(MDL3);
                if (MDL3_str != "MDL3")
                    goto labelfinish;

                string folderpath = MDL3path + "_texture";
                int writefilecount = 0;

                for (a = 0; a < TXS3_count; a++)
                {
                    string DXT = Getbytestr1(bs, TXS3_pointer + 9);
                    if (DXT == "A5")
                        goto labelTXS3finish;
                    else
                        writefilecount += 1;

                    int lod_count = Getbyteint1(bs, TXS3_pointer + 10);
                    if (lod_count > 1)
                        lod_count -= 4;

                    TXS3_tex_seek = Getbyteint4(bs, TXS3_pointer);
                    int TXS3_tex_length = Getbyteint4(bs, TXS3_pointer + 4);

                    byte[] TXS3_yoko = new byte[2];
                    Array.Copy(bs, TXS3_pointer + 12, TXS3_yoko, 0, 2);
                    int TXS3_yoko_int = Getbyteint2(TXS3_yoko, 0);
                    /*TXS3converter1.1.3
                    if (DXT == "86")
                    {
                        TXS3_yoko_int /= 2;
                        TXS3_yoko = Gethex2(TXS3_yoko_int);
                    }
                    */

                    byte[] TXS3_tate = new byte[2];
                    Array.Copy(bs, TXS3_pointer + 14, TXS3_tate, 0, 2);
                    int TXS3_tate_int = Getbyteint2(TXS3_tate, 0);

                    for (int j = 0; j < lod_count; j++)
                    {
                        if (lod_count > 1)
                        {
                            TXS3_tex_length = TXS3_yoko_int * TXS3_tate_int;
                            if (DXT == "86")
                                TXS3_tex_length /= 2;
                            else if (DXT == "85")
                                TXS3_tex_length *= 4;
                        }
                        if (TXS3_tex_length <= 128)
                            goto labelTXS3finish;

                        byte[] TXS3_tex = new byte[TXS3_tex_length];
                        Array.Copy(bs, TXS3_tex_seek, TXS3_tex, 0, TXS3_tex_length);

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

                        TXS3_yoko = Gethex2(TXS3_yoko_int);
                        Array.Resize(ref TXS3_header, TXS3_header.Length + 2);
                        Array.Copy(TXS3_yoko, 0, TXS3_header, TXS3_header.Length - 2, 2);
                        TXS3_tate = Gethex2(TXS3_tate_int);
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
                        else if (DXT == "87")
                            Array.Copy(TXS3_DXT3, 0, TXS3_header, TXS3_header.Length - 1, 1);
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

                        if (!Directory.Exists(folderpath))
                            Directory.CreateDirectory(folderpath);

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

                        string newpath = folderpath + @"\" + writefilecount + "_" + (j + 1) + "_";
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

                        if (lod_count > 1)
                        {
                            TXS3_tex_seek += TXS3_tex_length;
                            TXS3_yoko_int /= 2;
                            TXS3_tate_int /= 2;
                        }
                    }

                labelTXS3finish:;

                    TXS3_pointer += 32;
                    bgWorker.ReportProgress(a);
                }
                goto labelfinish;

            labelfolder:;
                
                string folderpath_file = path[b];
                folderpath_file = folderpath_file.Remove(folderpath_file.LastIndexOf("_texture"), 8);

                if (!System.IO.File.Exists(folderpath_file))
                    goto labelfinish;

                if (!Directory.Exists(Path.GetDirectoryName(folderpath_file) + @"\new\"))
                    Directory.CreateDirectory(Path.GetDirectoryName(folderpath_file) + @"\new\");

                string filename = Path.GetFileName(folderpath_file);
                if (filename.Substring(filename.Length - 3, 3) == "_hq")
                {
                    filename = @"hq\" + filename.Substring(0, filename.Length - 3);
                    if (!Directory.Exists(Path.GetDirectoryName(folderpath_file) + @"\new\" + @"hq\"))
                        Directory.CreateDirectory(Path.GetDirectoryName(folderpath_file) + @"\new\" + @"hq\");
                }
                else if (filename.Substring(filename.Length - 5, 5) == "_race")
                {
                    filename = @"race\" + filename.Substring(0, filename.Length - 5);
                    if (!Directory.Exists(Path.GetDirectoryName(folderpath_file) + @"\new\" + @"race\"))
                        Directory.CreateDirectory(Path.GetDirectoryName(folderpath_file) + @"\new\" + @"race\");
                }

                string folderpath_newfile = Path.GetDirectoryName(folderpath_file) + @"\new\" + filename;

                FileStream fsw_fo = new FileStream(folderpath_newfile, FileMode.Create, FileAccess.Write);
                fsw_fo.Write(bs, 0, bs.Length);
                int newfilecount = -1;
                
                for (a = 0; a < TXS3_count; a++)
                {
                labelnewfilestart:;

                    string DXT = Getbytestr1(bs, TXS3_pointer + 9);
                    if (DXT == "A5")
                    {
                        TXS3_pointer += 32;
                        goto labelnewfilestart;
                    }

                    int lod_count = Getbyteint1(bs, TXS3_pointer + 10);
                    if (lod_count > 1)
                        lod_count -= 4;

                    TXS3_tex_seek = Getbyteint4(bs, TXS3_pointer);
                    int TXS3_tex_length = Getbyteint4(bs, TXS3_pointer + 4);

                    for (int j = 0; j < lod_count; j++)
                    {
                        newfilecount += 1;

                        if (lod_count > 1)
                        {
                            if (img_files.Count() == newfilecount)
                                goto lodcountfinish;
                            string img_files_check = img_files[newfilecount];
                            img_files_check = Path.GetFileName(img_files_check);
                            img_files_check = img_files_check.Substring(img_files_check.IndexOf("_") + 1, 1);
                            if (Regex.IsMatch(img_files_check, "[1-9]") == true)
                            {
                                int img_files_lod_num = int.Parse(img_files_check);
                                if (img_files_lod_num - 1 != j)
                                {
                                    newfilecount -= 1;
                                    goto lodcountfinish;
                                }
                            }
                            else
                            {
                                newfilecount -= 1;
                                goto lodcountfinish;
                            }
                        }
                        
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
                        
                        TXS3_tex_seek += TXS3_tex_new.Length;

                    lodcountfinish:;
                    }
                    TXS3_pointer += 32;
                    bgWorker.ReportProgress(a);
                }
                fsw_fo.Close();

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
    }
}
