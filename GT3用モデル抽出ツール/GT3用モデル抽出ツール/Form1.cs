using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace GT3用モデル抽出ツール
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int Search(byte[] src, byte[] pattern)
        {
            int c = src.Length - pattern.Length + 1;
            int j;
            for (int i = 0; i < c; i++)
            {
                if (src[i] != pattern[0]) continue;
                for (j = pattern.Length - 1; j >= 1 && src[i + j] == pattern[j]; j--) ;
                if (j == 0) return i;
            }
            return -1;
        }

        public static string[] path = new string[] { "" };
        public static int j = 0;
        public static int GTM1header = 0;
        public static int facelod0 = 1;
        public static int facelod1 = 1;
        public static int facelod2 = 1;
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
            for (int i = 0; i < path.Count(); i++)
            {
                int facelod0 = 1;
                int facelod1 = 1;
                int facelod2 = 1;
                string fileextension = Path.GetExtension(path[i].ToString());

                if (fileextension != ".prm" && fileextension != ".PRM" && fileextension != ".bin" && fileextension != ".BIN")
                    goto labelfinish;

                // ドラッグ＆ドロップされたファイル
                FileStream fsr = new FileStream(path[i].ToString(), FileMode.Open, FileAccess.Read);

                byte[] bs = new byte[fsr.Length];
                fsr.Read(bs, 0, bs.Length);
                fsr.Close();

                int GTM1start = 0;
                if (fileextension == ".bin" || fileextension == ".BIN")
                    GTM1start = Getbyte(bs, 8);
                else if (fileextension == ".prm" || fileextension == ".PRM")
                    GTM1start = Getbyte(bs, 44);
                int GTM1end = Getbyte(bs, 12);
                int GTM1length = GTM1end - GTM1start;
                byte[] GTM1 = new byte[GTM1length];
                Array.Copy(bs, GTM1start, GTM1, 0, GTM1length);

                // ヘッダーまでの長さを取得
                int headerstart = Getbyte(GTM1, 36);

                // ヘッダーの終わりを取得
                int headerfinish = Getbyte(GTM1, 48);
                int headerlength = headerfinish - headerstart;
                GTM1header = headerlength / 4;

                string objwrite0 = "o lod0\r\nusemtl none\r\ns 1\r\n";
                string objwrite1 = "o lod1\r\nusemtl none\r\ns 1\r\n";
                string objwrite2 = "o lod2\r\nusemtl none\r\ns 1\r\n";
                string objpath = Path.GetDirectoryName(path[i].ToString());
                string objname = Path.GetFileNameWithoutExtension(path[i].ToString());

                for (j = 0; j < GTM1header; j++)
                {
                    // 頂点部分ヘッダーまでの長さを取得
                    int vertexheaderstart = Getbyte(GTM1, headerstart + (4 * j));

                    // 頂点部分ヘッダー(2つ目)のモデル情報を取得
                    // 1なら近景、2なら中景、0なら遠景
                    int headermodel = Getbyte1(GTM1, vertexheaderstart + 8);

                    // 頂点部分ヘッダー(2つ目)の個数を取得
                    int vertexheader2count = Getbyte2(GTM1, vertexheaderstart + 10);

                    for (int k = 0; k < vertexheader2count; k++)
                    {
                        // 頂点データまでの長さを取得
                        int vertexdata = Getbyte(GTM1, vertexheaderstart + (8 * k) + 16);
                        int seekvertexdata_start = vertexheaderstart + vertexdata;

                        int seekvertexdata = seekvertexdata_start + 20;
                        int floatcount = 0;
                        int float3 = 0;

                        while (true)
                        {
                            // 頂点データの浮動小数点を取得
                            int seekvertexfloat1 = seekvertexdata + (4 * floatcount);
                            int seekvertexfloat2 = seekvertexdata + (4 * floatcount) + 1;
                            int seekvertexfloat3 = seekvertexdata + (4 * floatcount) + 2;
                            int seekvertexfloat4 = seekvertexdata + (4 * floatcount) + 3;

                            byte[] vertexfloat1_byte = new byte[1];
                            Array.Copy(GTM1, seekvertexfloat1, vertexfloat1_byte, 0, 1);
                            byte[] vertexfloat2_byte = new byte[1];
                            Array.Copy(GTM1, seekvertexfloat2, vertexfloat2_byte, 0, 1);
                            byte[] vertexfloat3_byte = new byte[1];
                            Array.Copy(GTM1, seekvertexfloat3, vertexfloat3_byte, 0, 1);
                            byte[] vertexfloat4_byte = new byte[1];
                            Array.Copy(GTM1, seekvertexfloat4, vertexfloat4_byte, 0, 1);

                            string vertexfloat1_str = BitConverter.ToString(vertexfloat1_byte);
                            string vertexfloat2_str = BitConverter.ToString(vertexfloat2_byte);
                            string vertexfloat3_str = BitConverter.ToString(vertexfloat3_byte);
                            string vertexfloat4_str = BitConverter.ToString(vertexfloat4_byte);

                            if (vertexfloat4_str == "62")
                                break;

                            string vertexfloat16 = vertexfloat4_str + vertexfloat3_str + vertexfloat2_str + vertexfloat1_str;

                            uint vertexint = Convert.ToUInt32(vertexfloat16, 16);
                            float vertexfloat = BitConverter.ToSingle(BitConverter.GetBytes(vertexint), 0);

                            string vertexfloatstr = vertexfloat.ToString("0.######");
                            if (vertexfloatstr.Length > 8)
                            {
                                vertexfloat /= 10;
                                vertexfloatstr = vertexfloat.ToString("0.#######");
                                if (vertexfloatstr.IndexOf("-") >= 0)
                                {
                                    vertexfloatstr = vertexfloatstr.Replace("-", "");
                                    vertexfloatstr = vertexfloatstr.Remove(0, 2);
                                    vertexfloatstr = vertexfloatstr.Insert(1, ".");
                                    vertexfloatstr = "-" + vertexfloatstr;
                                }
                                else
                                {
                                    vertexfloatstr = vertexfloatstr.Remove(0, 2);
                                    vertexfloatstr = vertexfloatstr.Insert(1, ".");
                                }
                                if (vertexfloatstr.IndexOf("000000") >= 0)
                                    vertexfloatstr = vertexfloatstr.Remove(vertexfloatstr.IndexOf("000000"));
                            }
                            else
                                vertexfloatstr = vertexfloat.ToString("0.######");
                            int strinvertex = 0;

                            if (headermodel == 1)
                            {
                                if (objwrite0.IndexOf("vt") >= 0)
                                    strinvertex = objwrite0.IndexOf("vt");
                                else
                                    strinvertex = objwrite0.IndexOf("usemtl");
                            }
                            else if (headermodel == 2)
                            {
                                if (objwrite1.IndexOf("vt") >= 0)
                                    strinvertex = objwrite1.IndexOf("vt");
                                else
                                    strinvertex = objwrite1.IndexOf("usemtl");
                            }
                            else
                            {
                                if (objwrite2.IndexOf("vt") >= 0)
                                    strinvertex = objwrite2.IndexOf("vt");
                                else
                                    strinvertex = objwrite2.IndexOf("usemtl");
                            }

                            if (float3 == 0)
                            {
                                if (headermodel == 1)
                                    objwrite0 = objwrite0.Insert(strinvertex, "v \r\n");
                                else if (headermodel == 2)
                                    objwrite1 = objwrite1.Insert(strinvertex, "v \r\n");
                                else
                                    objwrite2 = objwrite2.Insert(strinvertex, "v \r\n");
                            }

                            if (float3 == 0)
                            {
                                if (headermodel == 1)
                                    objwrite0 = objwrite0.Insert(strinvertex + 2, vertexfloatstr + " ");
                                else if (headermodel == 2)
                                    objwrite1 = objwrite1.Insert(strinvertex + 2, vertexfloatstr + " ");
                                else
                                    objwrite2 = objwrite2.Insert(strinvertex + 2, vertexfloatstr + " ");
                            }

                            else if (float3 == 1)
                            {
                                if (headermodel == 1)
                                    objwrite0 = objwrite0.Insert(strinvertex - 2, vertexfloatstr + " ");
                                else if (headermodel == 2)
                                    objwrite1 = objwrite1.Insert(strinvertex - 2, vertexfloatstr + " ");
                                else
                                    objwrite2 = objwrite2.Insert(strinvertex - 2, vertexfloatstr + " ");
                            }

                            else if (float3 == 2)
                            {
                                if (headermodel == 1)
                                    objwrite0 = objwrite0.Insert(strinvertex - 2, vertexfloatstr);
                                else if (headermodel == 2)
                                    objwrite1 = objwrite1.Insert(strinvertex - 2, vertexfloatstr);
                                else
                                    objwrite2 = objwrite2.Insert(strinvertex - 2, vertexfloatstr);
                            }
                            floatcount += 1;
                            float3 += 1;
                            if (float3 == 3)
                                float3 = 0;
                        }

                        int facecount = floatcount / 3;
                        int texturecount = floatcount / 3 * 2;
                        int seekcount = floatcount;
                        float3 = 0;

                        while (true)
                        {
                            // 法線の浮動小数点の開始位置まで移動
                            int seekvectorstart4 = seekvertexdata + (4 * seekcount) + 3;

                            byte[] vectorstart4_byte = new byte[1];
                            Array.Copy(GTM1, seekvectorstart4, vectorstart4_byte, 0, 1);

                            string vectorstart4_str = BitConverter.ToString(vectorstart4_byte);

                            if (vectorstart4_str == "68")
                                break;

                            seekcount += 1;
                        }

                        seekcount += 1;

                        for (int m = 0; m < floatcount; m++)
                        {
                            // 法線の浮動小数点を取得
                            int seekvectorfloat1 = seekvertexdata + (4 * seekcount);
                            int seekvectorfloat2 = seekvertexdata + (4 * seekcount) + 1;
                            int seekvectorfloat3 = seekvertexdata + (4 * seekcount) + 2;
                            int seekvectorfloat4 = seekvertexdata + (4 * seekcount) + 3;

                            byte[] vectorfloat1_byte = new byte[1];
                            Array.Copy(GTM1, seekvectorfloat1, vectorfloat1_byte, 0, 1);
                            byte[] vectorfloat2_byte = new byte[1];
                            Array.Copy(GTM1, seekvectorfloat2, vectorfloat2_byte, 0, 1);
                            byte[] vectorfloat3_byte = new byte[1];
                            Array.Copy(GTM1, seekvectorfloat3, vectorfloat3_byte, 0, 1);
                            byte[] vectorfloat4_byte = new byte[1];
                            Array.Copy(GTM1, seekvectorfloat4, vectorfloat4_byte, 0, 1);

                            string vectorfloat1_str = BitConverter.ToString(vectorfloat1_byte);
                            string vectorfloat2_str = BitConverter.ToString(vectorfloat2_byte);
                            string vectorfloat3_str = BitConverter.ToString(vectorfloat3_byte);
                            string vectorfloat4_str = BitConverter.ToString(vectorfloat4_byte);
                            string vectorfloat16 = vectorfloat4_str + vectorfloat3_str + vectorfloat2_str + vectorfloat1_str;

                            uint vectorint = Convert.ToUInt32(vectorfloat16, 16);
                            float vectorfloat = BitConverter.ToSingle(BitConverter.GetBytes(vectorint), 0);

                            string vectorfloatstr = vectorfloat.ToString("0.######");
                            if (vectorfloatstr.Length > 8)
                            {
                                vectorfloat /= 10;
                                vectorfloatstr = vectorfloat.ToString("0.#######");
                                if (vectorfloatstr.IndexOf("-") >= 0)
                                {
                                    vectorfloatstr = vectorfloatstr.Replace("-", "");
                                    vectorfloatstr = vectorfloatstr.Remove(0, 2);
                                    vectorfloatstr = vectorfloatstr.Insert(1, ".");
                                    vectorfloatstr = "-" + vectorfloatstr;
                                }
                                else
                                {
                                    vectorfloatstr = vectorfloatstr.Remove(0, 2);
                                    vectorfloatstr = vectorfloatstr.Insert(1, ".");
                                }
                                if (vectorfloatstr.IndexOf("000000") >= 0)
                                    vectorfloatstr = vectorfloatstr.Remove(vectorfloatstr.IndexOf("000000"));
                            }
                            else
                                vectorfloatstr = vectorfloat.ToString("0.######");

                            int strinvector = 0;
                            if (headermodel == 1)
                                strinvector = objwrite0.IndexOf("usemtl");
                            else if (headermodel == 2)
                                strinvector = objwrite1.IndexOf("usemtl");
                            else
                                strinvector = objwrite2.IndexOf("usemtl");
                            int vectorfloatstrlength = 0;

                            if (float3 == 0)
                            {
                                if (headermodel == 1)
                                    objwrite0 = objwrite0.Insert(strinvector, "vn \r\n");
                                else if (headermodel == 2)
                                    objwrite1 = objwrite1.Insert(strinvector, "vn \r\n");
                                else
                                    objwrite2 = objwrite2.Insert(strinvector, "vn \r\n");
                                vectorfloatstrlength = vectorfloatstr.Length;
                            }

                            if (float3 == 0)
                            {
                                if (headermodel == 1)
                                    objwrite0 = objwrite0.Insert(strinvector + 3, vectorfloatstr + " ");
                                else if (headermodel == 2)
                                    objwrite1 = objwrite1.Insert(strinvector + 3, vectorfloatstr + " ");
                                else
                                    objwrite2 = objwrite2.Insert(strinvector + 3, vectorfloatstr + " ");
                            }
                            else if (float3 == 1)
                            {
                                if (headermodel == 1)
                                    objwrite0 = objwrite0.Insert(strinvector - 2, vectorfloatstr + " ");
                                else if (headermodel == 2)
                                    objwrite1 = objwrite1.Insert(strinvector - 2, vectorfloatstr + " ");
                                else
                                    objwrite2 = objwrite2.Insert(strinvector - 2, vectorfloatstr + " ");
                            }
                            else
                            {
                                if (headermodel == 1)
                                    objwrite0 = objwrite0.Insert(strinvector - 2, vectorfloatstr);
                                else if (headermodel == 2)
                                    objwrite1 = objwrite1.Insert(strinvector - 2, vectorfloatstr);
                                else
                                    objwrite2 = objwrite2.Insert(strinvector - 2, vectorfloatstr);
                            }

                            seekcount += 1;
                            float3 += 1;
                            if (float3 == 3)
                                float3 = 0;
                        }

                        while (true)
                        {
                            // テクスチャの浮動小数点の開始位置まで移動
                            int seekvectorstart4 = seekvertexdata + (4 * seekcount) + 3;

                            byte[] vectorstart4_byte = new byte[1];
                            Array.Copy(GTM1, seekvectorstart4, vectorstart4_byte, 0, 1);

                            string vectorstart4_str = BitConverter.ToString(vectorstart4_byte);

                            if (vectorstart4_str == "74")
                                break;

                            seekcount += 1;
                        }

                        int float2 = 0;
                        seekcount += 1;

                        for (int o = 0; o < texturecount; o++)
                        {
                            // テクスチャの浮動小数点を取得
                            int seektexturefloat1 = seekvertexdata + (4 * seekcount);
                            int seektexturefloat2 = seekvertexdata + (4 * seekcount) + 1;
                            int seektexturefloat3 = seekvertexdata + (4 * seekcount) + 2;
                            int seektexturefloat4 = seekvertexdata + (4 * seekcount) + 3;

                            byte[] texturefloat1_byte = new byte[1];
                            Array.Copy(GTM1, seektexturefloat1, texturefloat1_byte, 0, 1);
                            byte[] texturefloat2_byte = new byte[1];
                            Array.Copy(GTM1, seektexturefloat2, texturefloat2_byte, 0, 1);
                            byte[] texturefloat3_byte = new byte[1];
                            Array.Copy(GTM1, seektexturefloat3, texturefloat3_byte, 0, 1);
                            byte[] texturefloat4_byte = new byte[1];
                            Array.Copy(GTM1, seektexturefloat4, texturefloat4_byte, 0, 1);

                            string texturefloat1_str = BitConverter.ToString(texturefloat1_byte);
                            string texturefloat2_str = BitConverter.ToString(texturefloat2_byte);
                            string texturefloat3_str = BitConverter.ToString(texturefloat3_byte);
                            string texturefloat4_str = BitConverter.ToString(texturefloat4_byte);
                            string texturefloat16 = texturefloat4_str + texturefloat3_str + texturefloat2_str + texturefloat1_str;

                            uint textureint = Convert.ToUInt32(texturefloat16, 16);
                            float texturefloat = BitConverter.ToSingle(BitConverter.GetBytes(textureint), 0);

                            string texturefloatstr = texturefloat.ToString("0.######");
                            if (texturefloatstr.Length > 8)
                            {
                                texturefloat /= 10;
                                texturefloatstr = texturefloat.ToString("0.#######");
                                if (texturefloatstr.IndexOf("-") >= 0)
                                {
                                    texturefloatstr = texturefloatstr.Replace("-", "");
                                    texturefloatstr = texturefloatstr.Remove(0, 2);
                                    texturefloatstr = texturefloatstr.Insert(1, ".");
                                    texturefloatstr = "-" + texturefloatstr;
                                }
                                else
                                {
                                    texturefloatstr = texturefloatstr.Remove(0, 2);
                                    texturefloatstr = texturefloatstr.Insert(1, ".");
                                }
                                if (texturefloatstr.IndexOf("000000") >= 0)
                                    texturefloatstr = texturefloatstr.Remove(texturefloatstr.IndexOf("000000"));
                            }
                            else
                                texturefloatstr = texturefloat.ToString("0.######");

                            int strintexture = 0;
                            if (headermodel == 1)
                            {
                                if (objwrite0.IndexOf("vn") >= 0)
                                    strintexture = objwrite0.IndexOf("vn");
                                else
                                    strintexture = objwrite0.IndexOf("usemtl");
                            }
                            else if (headermodel == 2)
                            {
                                if (objwrite1.IndexOf("vn") >= 0)
                                    strintexture = objwrite1.IndexOf("vn");
                                else
                                    strintexture = objwrite1.IndexOf("usemtl");
                            }
                            else
                            {
                                if (objwrite2.IndexOf("vn") >= 0)
                                    strintexture = objwrite2.IndexOf("vn");
                                else
                                    strintexture = objwrite2.IndexOf("usemtl");
                            }
                            int texturefloatstrlength = 0;

                            if (float2 == 0)
                            {
                                if (headermodel == 1)
                                    objwrite0 = objwrite0.Insert(strintexture, "vt \r\n");
                                else if (headermodel == 2)
                                    objwrite1 = objwrite1.Insert(strintexture, "vt \r\n");
                                else
                                    objwrite2 = objwrite2.Insert(strintexture, "vt \r\n");
                                texturefloatstrlength = texturefloatstr.Length;
                            }

                            if (float2 == 0)
                            {
                                if (headermodel == 1)
                                    objwrite0 = objwrite0.Insert(strintexture + 3, texturefloatstr + " ");
                                else if (headermodel == 2)
                                    objwrite1 = objwrite1.Insert(strintexture + 3, texturefloatstr + " ");
                                else
                                    objwrite2 = objwrite2.Insert(strintexture + 3, texturefloatstr + " ");
                            }
                            else
                            {
                                if (headermodel == 1)
                                    objwrite0 = objwrite0.Insert(strintexture - 2 + texturefloatstrlength, texturefloatstr);
                                else if (headermodel == 2)
                                    objwrite1 = objwrite1.Insert(strintexture - 2 + texturefloatstrlength, texturefloatstr);
                                else
                                    objwrite2 = objwrite2.Insert(strintexture - 2 + texturefloatstrlength, texturefloatstr);
                            }

                            seekcount += 1;
                            float2 += 1;
                            if (float2 == 2)
                                float2 = 0;
                        }

                        for (int n = 0; n < facecount - 2; n++)
                        {
                            int strinface = 0;
                            if (headermodel == 1)
                            {
                                if (facecount != 0)
                                {
                                    strinface = objwrite0.Length;
                                    objwrite0 = objwrite0.Insert(strinface, "f " + facelod0.ToString() + "/" + facelod0.ToString() +
                                        "/" + facelod0.ToString() + " " + (facelod0 + 1).ToString() + "/" + facelod0.ToString() +
                                        "/" + facelod0.ToString() + " " + (facelod0 + 2).ToString() + "/" + facelod0.ToString() +
                                        "/" + facelod0.ToString() + "\r\n");
                                    facelod0 += 1;
                                }
                                else
                                    facelod0 += 1;
                            }
                            else if (headermodel == 2)
                            {
                                strinface = objwrite1.Length;
                                objwrite1 = objwrite1.Insert(strinface, "f " + facelod1.ToString() + "/" + facelod1.ToString() +
                                    "/" + facelod1.ToString() + " " + (facelod1 + 1).ToString() + "/" + facelod1.ToString() +
                                    "/" + facelod1.ToString() + " " + (facelod1 + 2).ToString() + "/" + facelod1.ToString() +
                                    "/" + facelod1.ToString() + "\r\n");
                                facelod1 += 1;
                            }
                            else
                            {
                                strinface = objwrite2.Length;
                                objwrite2 = objwrite2.Insert(strinface, "f " + facelod2.ToString() + "/" + facelod2.ToString() +
                                    "/" + facelod2.ToString() + " " + (facelod2 + 1).ToString() + "/" + facelod2.ToString() +
                                    "/" + facelod2.ToString() + " " + (facelod2 + 2).ToString() + "/" + facelod2.ToString() +
                                    "/" + facelod2.ToString() + "\r\n");
                                facelod2 += 1;
                            }
                        }

                        if (headermodel == 1)
                            facelod0 += 2;
                        else if (headermodel == 2)
                            facelod1 += 2;
                        else if (headermodel == 0)
                            facelod2 += 2;
                    }
                    bgWorker.ReportProgress(j);
                }
                StreamWriter sw0 = new StreamWriter(objpath + @"\" + objname + "_lod0" + ".obj");
                StreamWriter sw1 = new StreamWriter(objpath + @"\" + objname + "_lod1" + ".obj");
                StreamWriter sw2 = new StreamWriter(objpath + @"\" + objname + "_lod2" + ".obj");
                sw0.Write(objwrite0);
                sw1.Write(objwrite1);
                sw2.Write(objwrite2);
                sw0.Close();
                sw1.Close();
                sw2.Close();


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
    }
}