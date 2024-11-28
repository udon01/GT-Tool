using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GT5_モデル抽出ツール
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
        public static bool notDirectoryfiles2 = false;
        public static bool notMDL3 = false;
        public static byte[] byte2 = new byte[2];
        public static byte[] byte4 = new byte[4];

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

                string[] files = null;
                string MDL3_path = path[b];
                string obj_path = path[b];

                var isDirectory = File.GetAttributes(path[b]).HasFlag(FileAttributes.Directory);
                if (isDirectory == true)
                {
                    bool notMDL3infolder = true;
                    bool notobjinfolder = true;
                    files = Directory.GetFiles(path[b], "*");
                    if (files.Count() == 2)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            if (Path.GetExtension(files[i]).ToLower() == "")
                            {
                                MDL3_path = files[i];
                                notMDL3infolder = false;
                            }
                            else if (Path.GetExtension(files[i]).ToLower() == ".obj")
                            {
                                obj_path = files[i];
                                notobjinfolder = false;
                            }
                        }
                    }

                    if (notMDL3infolder == true || notobjinfolder == true)
                    {
                        notDirectoryfiles2 = true;
                        goto labelfinish;
                    }
                }

                else if (isDirectory == false && Path.GetExtension(path[b]).ToLower() != "")
                {
                    notMDL3 = true;
                    goto labelfinish;
                }

                FileStream fsr = new FileStream(MDL3_path, FileMode.Open, FileAccess.Read);
                byte[] bs_MDL3 = new byte[fsr.Length];
                fsr.Read(bs_MDL3, 0, bs_MDL3.Length);
                fsr.Close();

                int mesh_count = Getbyteint2(bs_MDL3, 0x14);
                int pointer_mesh = Getbyteint4(bs_MDL3, 0x38);
                int model_pointer_unk1 = Getbyteint4(bs_MDL3, 0x90);
                int model_pointer_unk2 = Getbyteint4(bs_MDL3, 0xB0);
                int model_pointer_unk3 = Getbyteint4(bs_MDL3, 0xB8);
                int model_pointer_unk4 = Getbyteint4(bs_MDL3, 0xBC);

                if (MDL3_path != obj_path)
                    goto label_toMDL3;

                //MDL3をobjに変換する
                StreamWriter sw1 = new StreamWriter(Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileNameWithoutExtension(path[b]) + "_近距離.obj");
                StreamWriter sw2 = new StreamWriter(Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileNameWithoutExtension(path[b]) + "_影.obj");
                StreamWriter sw3 = new StreamWriter(Path.GetDirectoryName(path[b]) + @"\" + Path.GetFileNameWithoutExtension(path[b]) + "_遠距離.obj");
                int vertex_count_total = 0;
                int vertex_data_unk = 0;

                for (int i = 0; i < mesh_count; i++)
                {
                    if (i == 0)
                        sw1.WriteLine("o GT5_MDL3_" + (i + 1).ToString());
                    else if (i == 6)
                        sw3.WriteLine("o GT5_MDL3_" + (i + 1).ToString());
                    else
                        sw2.WriteLine("o GT5_MDL3_" + (i + 1).ToString());

                    int vertex_count = Getbyteint4(bs_MDL3, pointer_mesh + (i * 0x30) + 0x8);
                    int pointer_vertex = Getbyteint4(bs_MDL3, pointer_mesh + (i * 0x30) + 0xC);
                    int face_count = Getbyteint4(bs_MDL3, pointer_mesh + (i * 0x30) + 0x24);
                    int pointer_face = Getbyteint4(bs_MDL3, pointer_mesh + (i * 0x30) + 0x18);
                    if (i == 0 || i == 6)
                        vertex_data_unk = 0x14;
                    else if (i == 5)
                        vertex_data_unk = 0x18;
                    else
                        vertex_data_unk = 0xC;

                    for (int j = 0; j < vertex_count; j++)
                    {
                        Array.Copy(bs_MDL3, pointer_vertex + (j * vertex_data_unk), byte4, 0, 4);
                        Array.Reverse(byte4);
                        float vx = BitConverter.ToSingle(byte4, 0);
                        Array.Copy(bs_MDL3, pointer_vertex + 0x4 + (j * vertex_data_unk), byte4, 0, 4);
                        Array.Reverse(byte4);
                        float vy = BitConverter.ToSingle(byte4, 0);
                        Array.Copy(bs_MDL3, pointer_vertex + 0x8 + (j * vertex_data_unk), byte4, 0, 4);
                        Array.Reverse(byte4);
                        float vz = BitConverter.ToSingle(byte4, 0);
                        //近距離モデルのみ
                        Array.Copy(bs_MDL3, pointer_vertex + 0xC + (j * vertex_data_unk), byte4, 0, 4);
                        Array.Reverse(byte4);
                        float vn = BitConverter.ToSingle(byte4, 0);
                        Array.Copy(bs_MDL3, pointer_vertex + 0x10 + (j * vertex_data_unk), byte4, 0, 4);
                        Array.Reverse(byte4);
                        byte[] brightness_unk = byte4;

                        if (i == 0)
                        {
                            sw1.WriteLine("v " + vx.ToString("F9") + " " + vy.ToString("F9") + " " + vz.ToString("F9"));
                            sw1.Flush();
                        }
                        else if (i == 6)
                        {
                            sw3.WriteLine("v " + vx.ToString("F9") + " " + vy.ToString("F9") + " " + vz.ToString("F9"));
                            sw3.Flush();
                        }
                        else
                        {
                            sw2.WriteLine("v " + vx.ToString("F9") + " " + vy.ToString("F9") + " " + vz.ToString("F9"));
                            sw2.Flush();
                        }
                    }
                    if (i == 0)
                    {
                        sw1.WriteLine("usemtl None");
                        sw1.WriteLine("s 1");
                    }
                    else if (i == 6)
                    {
                        sw3.WriteLine("usemtl None");
                        sw3.WriteLine("s 1");
                    }
                    else
                    {
                        sw2.WriteLine("usemtl None");
                        sw2.WriteLine("s 1");
                    }

                    for (int j = 0; j < face_count; j++)
                    {
                        Array.Copy(bs_MDL3, pointer_face + (j * 0x6), byte2, 0, 2);
                        Array.Reverse(byte2);
                        int f1 = BitConverter.ToInt16(byte2, 0) + 1;
                        Array.Copy(bs_MDL3, pointer_face + 0x2 + (j * 0x6), byte2, 0, 2);
                        Array.Reverse(byte2);
                        int f2 = BitConverter.ToInt16(byte2, 0) + 1;
                        Array.Copy(bs_MDL3, pointer_face + 0x4 + (j * 0x6), byte2, 0, 2);
                        Array.Reverse(byte2);
                        int f3 = BitConverter.ToInt16(byte2, 0) + 1;
                        if (i > 0 && i < 6)
                        {
                            f1 += vertex_count_total;
                            f2 += vertex_count_total;
                            f3 += vertex_count_total;
                        }
                        if (i == 0)
                        {
                            sw1.Write("f " + f1.ToString() + "// " + f2.ToString() + "// " + f3.ToString() + "//");
                            if (j < face_count - 1)
                                sw1.Write("\r\n");
                            sw1.Flush();
                        }
                        else if (i == 6)
                        {
                            sw3.Write("f " + f1.ToString() + "// " + f2.ToString() + "// " + f3.ToString() + "//");
                            if (j < face_count - 1)
                                sw3.Write("\r\n");
                            sw3.Flush();
                        }
                        else
                        {
                            sw2.Write("f " + f1.ToString() + "// " + f2.ToString() + "// " + f3.ToString() + "//");
                            if (i < mesh_count - 2 || j < face_count - 1)
                                sw2.Write("\r\n");
                            sw2.Flush();
                        }
                    }
                    if (i > 0 && i < 6)
                        vertex_count_total += vertex_count;
                    //sw1.Flush();
                    //sw2.Flush();
                    //sw3.Flush();
                }
                //MessageBox.Show("a");
                sw1.Close();
                sw2.Close();
                sw3.Close();
                goto labelfinish;


            label_toMDL3:;
                //objをMDL3に変換する(近距離のみ)
                StreamReader sr_re = new StreamReader(obj_path);
                List<float> vertex_new = new List<float> { };
                List<int> face_new = new List<int> { };
                while (sr_re.Peek() > -1)
                {
                    string line = sr_re.ReadLine();
                    if (line.Substring(0, 2) == "v ")
                    {
                        string line2 = line.Substring(2, line.Length - 2);
                        string vertex_str = line2.Substring(0, line2.IndexOf(" "));
                        line2 = line2.Substring(line2.IndexOf(" ") + 1, line2.Length - line2.IndexOf(" ") - 1);
                        vertex_new.Add(float.Parse(vertex_str));
                        vertex_str = line2.Substring(0, line2.IndexOf(" "));
                        line2 = line2.Substring(line2.IndexOf(" ") + 1, line2.Length - line2.IndexOf(" ") - 1);
                        vertex_new.Add(float.Parse(vertex_str));
                        vertex_new.Add(float.Parse(line2));
                    }
                    else if (line.Substring(0, 2) == "f ")
                    {
                        string line2 = line.Substring(2, line.Length - 2);
                        string face_str = line2.Substring(0, line2.IndexOf("/"));
                        line2 = line2.Substring(line2.IndexOf(" ") + 1, line2.Length - line2.IndexOf(" ") - 1);
                        int face_int = int.Parse(face_str) - 1;
                        face_new.Add(face_int);
                        face_str = line2.Substring(0, line2.IndexOf("/"));
                        line2 = line2.Substring(line2.IndexOf(" ") + 1, line2.Length - line2.IndexOf(" ") - 1);
                        face_int = int.Parse(face_str) - 1;
                        face_new.Add(face_int);
                        face_str = line2.Substring(0, line2.IndexOf("/"));
                        face_int = int.Parse(face_str) - 1;
                        face_new.Add(face_int);
                    }
                }
                sr_re.Close();

                byte[] bs_new_vertex = new byte[0];
                byte[] bs_new_face = new byte[0];
                int new_vertex_count = 0;
                int new_face_count = 0;

                for (int i = 0; i < vertex_new.Count(); i++)
                {
                    byte4 = BitConverter.GetBytes(vertex_new[i]);
                    Array.Reverse(byte4);
                    Array.Resize(ref bs_new_vertex, bs_new_vertex.Length + 4);
                    Array.Copy(byte4, 0, bs_new_vertex, bs_new_vertex.Length - 4, 4);

                    if (i % 3 == 2)
                    {
                        //法線
                        byte4 = new byte[4] { 0x40, 0x00, 0x00, 0x00 };
                        Array.Resize(ref bs_new_vertex, bs_new_vertex.Length + 4);
                        Array.Copy(byte4, 0, bs_new_vertex, bs_new_vertex.Length - 4, 4);
                        //光沢(？)
                        byte4 = new byte[4] { 0x40, 0x40, 0x40, 0xFF };
                        Array.Resize(ref bs_new_vertex, bs_new_vertex.Length + 4);
                        Array.Copy(byte4, 0, bs_new_vertex, bs_new_vertex.Length - 4, 4);
                    }
                    new_vertex_count += 1;

                    //元々の頂点数(増やす方法を調べる)
                    if (i + 1 == 8354 * 3)
                        break;
                }

                while (bs_new_vertex.Length < 0x28D20)
                {
                    Array.Resize(ref bs_new_vertex, bs_new_vertex.Length + 4);
                    Array.Copy(zero4, 0, bs_new_vertex, bs_new_vertex.Length - 4, 4);
                }

                for (int i = 0; i < face_new.Count(); i++)
                {
                    byte2 = BitConverter.GetBytes((short)face_new[i]);
                    Array.Reverse(byte2);
                    Array.Resize(ref bs_new_face, bs_new_face.Length + 2);
                    Array.Copy(byte2, 0, bs_new_face, bs_new_face.Length - 2, 2);

                    new_face_count += 1;

                    //元々の面数(増やす方法を調べる)
                    if (i + 1 == 16704 * 3)
                        break;
                }

                int face_length = Getbyteint4(bs_MDL3, pointer_mesh + 0x24) * 6;
                while (bs_new_face.Length < 0x18780)
                {
                    Array.Resize(ref bs_new_face, bs_new_face.Length + 2);
                    Array.Copy(zero4, 0, bs_new_face, bs_new_face.Length - 2, 2);
                }
                int new_pointer_vertex = Getbyteint4(bs_MDL3, pointer_mesh + 0xC);
                int new_pointer_face = Getbyteint4(bs_MDL3, pointer_mesh + 0x18);

                byte[] bs_new_header = new byte[pointer_mesh];
                //メッシュデータまで丸ごとコピー
                Array.Copy(bs_MDL3, 0, bs_new_header, 0, pointer_mesh);
                //まだしらべてない
                Array.Resize(ref bs_new_header, bs_new_header.Length + 8);
                Array.Copy(bs_MDL3, pointer_mesh, bs_new_header, bs_new_header.Length - 8, 8);
                //頂点の数
                byte4 = BitConverter.GetBytes(new_vertex_count);
                Array.Reverse(byte4);
                Array.Resize(ref bs_new_header, bs_new_header.Length + 4);
                Array.Copy(byte4, 0, bs_new_header, bs_new_header.Length - 4, 4);
                //頂点データの開始位置
                Array.Resize(ref bs_new_header, bs_new_header.Length + 4);
                Array.Copy(bs_MDL3, pointer_mesh + 0xC, bs_new_header, bs_new_header.Length - 4, 4);
                //まだしらべてない
                Array.Resize(ref bs_new_header, bs_new_header.Length + 8);
                Array.Copy(bs_MDL3, pointer_mesh + 0x10, bs_new_header, bs_new_header.Length - 8, 8);
                //面データの開始位置
                Array.Resize(ref bs_new_header, bs_new_header.Length + 4);
                Array.Copy(bs_MDL3, pointer_mesh + 0x18, bs_new_header, bs_new_header.Length - 4, 4);
                //まだしらべてない
                Array.Resize(ref bs_new_header, bs_new_header.Length + 8);
                Array.Copy(bs_MDL3, pointer_mesh + 0x1C, bs_new_header, bs_new_header.Length - 8, 8);
                //面の数
                byte4 = BitConverter.GetBytes(new_face_count);
                Array.Reverse(byte4);
                Array.Resize(ref bs_new_header, bs_new_header.Length + 4);
                Array.Copy(byte4, 0, bs_new_header, bs_new_header.Length - 4, 4);
                //頂点データの開始位置まで丸ごとコピー
                int copy_length = new_pointer_vertex - pointer_mesh - 0x28;
                Array.Resize(ref bs_new_header, bs_new_header.Length + copy_length);
                Array.Copy(bs_MDL3, pointer_mesh + 0x28, bs_new_header, bs_new_header.Length - copy_length, copy_length);

                //頂点データの開始位置(2つ目)
                int pointer_vertex_2_new = Getbyteint4(bs_MDL3, pointer_mesh + 0x3C);
                byte[] bs_new_footer = new byte[bs_MDL3.Length - pointer_vertex_2_new];
                Array.Copy(bs_MDL3, pointer_vertex_2_new, bs_new_footer, 0, bs_MDL3.Length - pointer_vertex_2_new);

                string MDL3_name = Path.GetFileNameWithoutExtension(MDL3_path);
                string MDL3_path_new = Path.GetDirectoryName(MDL3_path);
                MDL3_path_new = MDL3_path_new.Substring(0, MDL3_path_new.LastIndexOf(@"\")) + @"\new\";
                if (!Directory.Exists(MDL3_path_new))
                    Directory.CreateDirectory(MDL3_path_new);
                //MessageBox.Show(MDL3_name.Substring(MDL3_name.Length - 3, 3));
                if (MDL3_name.Substring(MDL3_name.Length - 3, 3) == "_hq")
                {
                    MDL3_path_new = MDL3_path_new + @"hq\";
                    if (!Directory.Exists(MDL3_path_new))
                        Directory.CreateDirectory(MDL3_path_new);
                }
                else if (MDL3_name.Substring(MDL3_name.Length - 5, 5) == "_race")
                {
                    MDL3_path_new = MDL3_path_new + @"race\";
                    if (!Directory.Exists(MDL3_path_new))
                        Directory.CreateDirectory(MDL3_path_new);
                }

                if (MDL3_name.Length > 8)
                    MDL3_name = MDL3_name.Substring(0, 8);
                MDL3_path_new = MDL3_path_new + MDL3_name;

                FileStream fsw = new FileStream(MDL3_path_new, FileMode.Create, FileAccess.Write);
                fsw.Write(bs_new_header, 0, bs_new_header.Length);
                fsw.Write(bs_new_vertex, 0, bs_new_vertex.Length);
                fsw.Write(bs_new_face, 0, bs_new_face.Length);
                fsw.Write(bs_new_footer, 0, bs_new_footer.Length);
                fsw.Close();

            labelfinish:;
                if (notMDL3 == true)
                    MessageBox.Show("このファイルはMDL3(拡張子なし、carフォルダの中にあるファイル)ではありません！");
                if (notDirectoryfiles2 == true)
                    MessageBox.Show("フォルダの中にobjと元々の車ファイル(サンプル車両のみ、拡張子なし)を一つずつ入れてください！");
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
    }
}
