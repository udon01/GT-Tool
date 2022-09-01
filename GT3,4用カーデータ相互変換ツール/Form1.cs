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

namespace GT3_4用カーデータ相互変換ツール
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

                string filecheck = Getbytestr(bs, 0);
                if (filecheck == "43415234")
                    goto GTM1;

                filecheck = Getbytestr(bs, 64);

                if (filecheck == "47544349")
                    goto MDLS;

                else
                    goto labelfinish;

                MDLS:;

                string menufolder = Path.GetDirectoryName(path[i]) + @"\menu";
                if (Directory.Exists(menufolder) == false)
                    goto labelfinish;

                string[] readmenufiles = Directory.GetFiles(menufolder, "*", SearchOption.AllDirectories);
                for (int k = 0; k < readmenufiles.Count(); k++)
                {
                    if (Path.GetExtension(readmenufiles[k]) == "")
                        readmenufilesextension.Add(k);
                }

                for (int k = 0; k < readmenufilesextension.Count(); k++)
                {
                    FileStream fsr2 = new FileStream(readmenufiles[readmenufilesextension[k]].ToString(), FileMode.Open, FileAccess.Read);
                    byte[] bs2 = new byte[fsr2.Length];
                    fsr2.Read(bs2, 0, bs2.Length);
                    if (Getbytestr(bs2, 0) == "43415234")
                        readmenufilesnum.Add(readmenufilesextension[k]);
                    fsr2.Close();
                }

                if (readmenufilesnum.Count != 1)
                    goto labelfinish;

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

                /*
                string wheelfolder = Path.GetDirectoryName(path[i]) + @"\wheel"; 
                List<int> readwheelfilesextension = new List<int>();
                List<int> readwheelfilesnum = new List<int>();
                if (Directory.Exists(wheelfolder) == false)
                    goto labelfinish;

                string[] readwheelfiles = Directory.GetFiles(wheelfolder, "*", SearchOption.AllDirectories);
                for (int k = 0; k < readwheelfiles.Count(); k++)
                {
                    if (Path.GetExtension(readwheelfiles[k]) == "" || Path.GetExtension(readwheelfiles[k]) == "")
                        readwheelfilesextension.Add(k);
                }

                for (int k = 0; k < readwheelfilesextension.Count(); k++)
                {
                    FileStream fsrwheel2 = new FileStream(readwheelfiles[readwheelfilesextension[k]].ToString(), FileMode.Open, FileAccess.Read);
                    byte[] bswheel2 = new byte[fsrwheel2.Length];
                    fsrwheel2.Read(bswheel2, 0, bswheel2.Length);
                    if (Getbytestr(bswheel2, 64) == "4D444C53")
                        readwheelfilesnum.Add(k);
                    fsrwheel2.Close();
                }

                if (readwheelfilesnum.Count != 1)
                    goto labelfinish;
                */

                byte[] bsGTM1all = new byte[bs.Length];
                Array.Copy(bs, 0, bsGTM1all, 0, bs.Length);

                //ファイルを分割する
                int GTM1lengthstart = Getbyte(bsGTM1all, 4);
                byte[] bsGTM1header = new byte[GTM1lengthstart];
                Array.Copy(bsGTM1all, 0, bsGTM1header, 0, GTM1lengthstart);

                GTM1lengthstart = Getbyte(bsGTM1all, 4);
                int GTM1lengthend = Getbyte(bsGTM1all, 8);
                int GTM1length2 = GTM1lengthend - GTM1lengthstart;
                byte[] bsGTM1GTCI = new byte[GTM1length2];
                Array.Copy(bsGTM1all, GTM1lengthstart, bsGTM1GTCI, 0, GTM1length2);

                GTM1lengthstart = Getbyte(bsGTM1all, 8);
                GTM1lengthend = Getbyte(bsGTM1all, 12);
                GTM1length2 = GTM1lengthend - GTM1lengthstart;
                byte[] bsGTM1 = new byte[GTM1length2];
                Array.Copy(bsGTM1all, GTM1lengthstart, bsGTM1, 0, GTM1length2);

                GTM1lengthstart = Getbyte(bsGTM1all, 12);
                GTM1lengthend = Getbyte(bsGTM1all, 16);
                GTM1length2 = GTM1lengthend - GTM1lengthstart;
                byte[] bsGTM1GTTR = new byte[GTM1length2];
                Array.Copy(bsGTM1all, GTM1lengthstart, bsGTM1GTTR, 0, GTM1length2);

                GTM1lengthstart = Getbyte(bsGTM1all, 16);
                GTM1lengthend = Getbyte(bsGTM1all, 20);
                GTM1length2 = GTM1lengthend - GTM1lengthstart;
                byte[] bsGTM1GTTW = new byte[GTM1length2];
                Array.Copy(bsGTM1all, GTM1lengthstart, bsGTM1GTTW, 0, GTM1length2);

                GTM1lengthstart = Getbyte(bsGTM1all, 20);
                GTM1lengthend = bsGTM1all.Length;
                GTM1length2 = GTM1lengthend - GTM1lengthstart;
                byte[] bsGTM1dummy = new byte[GTM1length2];
                Array.Copy(bsGTM1all, GTM1lengthstart, bsGTM1dummy, 0, GTM1length2);

                //ファイルを分割する
                FileStream fsrMDLS = new FileStream(readmenufiles[readmenufilesnum[0]].ToString(), FileMode.Open, FileAccess.Read);
                byte[] bsMDLSall = new byte[fsrMDLS.Length];
                fsrMDLS.Read(bsMDLSall, 0, bsMDLSall.Length);
                fsrMDLS.Close();

                int MDLSlengthstart2 = Getbyte(bsMDLSall, 24);
                int MDLSlengthend2 = Getbyte(bsMDLSall, 28);
                int MDLSlength2_2 = MDLSlengthend2 - MDLSlengthstart2;
                byte[] bsMDLS2 = new byte[MDLSlength2_2];
                Array.Copy(bsMDLSall, MDLSlengthstart2, bsMDLS2, 0, MDLSlength2_2);

                byte[] GTM1headertop = new byte[32];
                Array.Copy(bsGTM1, 0, GTM1headertop, 0, 32);
                int GTM1header_unknownstart = Getbyte(bsGTM1, 32);
                //GTM1のヘッダーまでの長さ
                int GTM1header_unknownend = Getbyte(bsGTM1, 36);
                int GTM1header_unknown2 = Getbyte(bsGTM1, 40);
                int GTM1header_Tex1headerlength = Getbyte(bsGTM1, 44);
                //GTM1のヘッダー終わりまでの長さ
                int GTM1header_headerend = Getbyte(bsGTM1, 48);
                int GTM1header_unknown3 = Getbyte(bsGTM1, 56);
                int GTM1_unknown1 = Getbyte(bsGTM1, GTM1header_unknownstart);
                int GTM1_unknown2 = Getbyte(bsGTM1, GTM1header_unknownstart + 4);
                int GTM1_unknown3 = Getbyte(bsGTM1, GTM1header_unknownstart + 8);
                int GTM1_unknown4 = Getbyte(bsGTM1, GTM1header_unknownstart + 12);
                int GTM1_unknown5 = Getbyte(bsGTM1, GTM1header_unknownstart + 16);
                int GTM1_unknown6 = Getbyte(bsGTM1, GTM1header_unknownstart + 20);
                int GTM1_unknown7 = Getbyte(bsGTM1, GTM1header_unknownstart + 24);

                byte[] GTM1copy2 = new byte[GTM1header_unknownstart - 64];
                Array.Copy(bsGTM1, 64, GTM1copy2, 0, GTM1header_unknownstart - 64);

                //MDLSのヘッダーまでの長さ
                int MDLSheaderstart = Getbyte(bsMDLS2, 60);
                //MDLSのヘッダー終わりまでの長さ
                int MDLSheaderend = Getbyte(bsMDLS2, 56);

                int GTM1_headerlength = GTM1header_headerend - GTM1header_unknownend;
                int MDLSheaderlength = MDLSheaderend - MDLSheaderstart;

                GTM1_MDLSheader = MDLSheaderend - MDLSheaderstart;
                GTM1_MDLSheader /= 4;

                int modelheaderlength_1 = Getbyte(bsGTM1, GTM1header_unknownend);
                byte[] modelunknown = new byte[modelheaderlength_1 - GTM1header_headerend];
                Array.Copy(bsGTM1, GTM1header_headerend, modelunknown, 0, modelheaderlength_1 - GTM1header_headerend);

                byte[] newGTM1header = new byte[0];
                byte[] newGTM1 = new byte[0];
                List<int> newGTM1headernum = new List<int>();

                for (j = 0; j < GTM1_MDLSheader; j++)
                {
                    byte[] newmodelall = new byte[0];

                    int seek_j = j * 4;
                    //モデルデータのヘッダーまでの長さ
                    int modelheaderstart = Getbyte(bsMDLS2, MDLSheaderstart + seek_j);

                    if (modelheaderstart == 0)
                        break;

                    int newGTM1headerlength2 = GTM1header_unknownend + modelunknown.Length + newGTM1.Length;

                    int modelheadercount = Getbyte2(bsMDLS2, modelheaderstart + 10);
                    byte[] newmodelheader = new byte[16];
                    Array.Copy(bsMDLS2, modelheaderstart, newmodelheader, 0, 16);

                    for (int k = 0; k < modelheadercount; k++)
                    {
                        int seek_k = k * 8;
                        int modelstartget = Getbyte(bsMDLS2, modelheaderstart + 16 + seek_k);
                        int verticestart = modelheaderstart + modelstartget + 20;
                        int increase = 0;
                        int seek_ver = 0;
                        int verticecount = 0;

                        while (true)
                        {
                            seek_ver = increase * 4;
                            byte[] verticebyte4 = new byte[1];
                            Array.Copy(bsMDLS2, verticestart + 3 + seek_ver, verticebyte4, 0, 1);

                            string verticestr4 = BitConverter.ToString(verticebyte4);
                            int verticestr4int = Convert.ToInt32(verticestr4, 16);
                            if (verticestr4int == 104)
                            {
                                verticecount = increase * 4 / 3;
                                break;
                            }

                            if (verticestr4int == 5)
                                goto label1;

                            increase += 1;
                        }
                        int modelstart = modelheaderstart + modelstartget;
                        byte[] modelunknown1 = new byte[20];
                        Array.Copy(bsMDLS2, modelstart, modelunknown1, 0, 20);
                        int verticelength = verticecount * 3;
                        byte[] modelvertice = new byte[verticelength];
                        Array.Copy(bsMDLS2, verticestart, modelvertice, 0, verticelength);
                        byte[] modelunknown2 = new byte[4];
                        Array.Copy(bsMDLS2, verticestart + verticelength, modelunknown2, 0, 4);
                        byte[] modelnormal = new byte[verticelength];
                        int normalto = verticestart + verticelength + 4;
                        Array.Copy(bsMDLS2, normalto, modelnormal, 0, verticelength);

                        int modelunknown3int = Getbyte(bsMDLS2, normalto + verticelength);
                        modelunknown3int += 268435456;
                        byte[] modelunknown3 = new byte[4];
                        modelunknown3 = BitConverter.GetBytes(modelunknown3int);

                        int texturelength = verticecount * 2;
                        byte[] modeltexture = new byte[texturelength];
                        Array.Copy(bsMDLS2, normalto + verticelength + 4, modeltexture, 0, texturelength);

                        int unknown4 = normalto + verticelength + 4 + texturelength;

                        //ここはコピーしない(不明)
                        int unknown4length = Getbyte1(bsMDLS2, unknown4 + 3);
                        if (unknown4length % 4 != 0)
                        {
                            int unknown4amari = unknown4length % 4;
                            int unknown4amari4 = 4 - unknown4amari;
                            unknown4length += unknown4amari4;
                        }
                        //ここまで

                        int modelunknown5int = Getbyte(bsMDLS2, unknown4 + unknown4length + 4);
                        modelunknown5int -= 268435456;
                        byte[] modelunknown5 = new byte[4];
                        modelunknown5 = BitConverter.GetBytes(modelunknown5int);

                        int unknown5length = Getbyte1(bsMDLS2, unknown4 + unknown4length + 4 + 2);
                        if (unknown5length % 4 != 0)
                        {
                            int unknown5amari = unknown5length % 4;
                            int unknown5amari4 = 4 - unknown5amari;
                            unknown5length += unknown5amari4;
                        }

                        byte[] modelunknown5length = new byte[unknown5length];
                        Array.Copy(bsMDLS2, unknown4 + unknown4length + 8, modelunknown5length, 0, unknown5length);

                        //変換先のbyte配列を用意する
                        byte[] newmodel = new byte[0];
                        int newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelunknown1.Length);
                        Array.Copy(modelunknown1, 0, newmodel, newmodellength, modelunknown1.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelvertice.Length);
                        Array.Copy(modelvertice, 0, newmodel, newmodellength, modelvertice.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelunknown5.Length);
                        Array.Copy(modelunknown5, 0, newmodel, newmodellength, modelunknown5.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelunknown5length.Length);
                        Array.Copy(modelunknown5length, 0, newmodel, newmodellength, modelunknown5length.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelunknown2.Length);
                        Array.Copy(modelunknown2, 0, newmodel, newmodellength, modelunknown2.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelnormal.Length);
                        Array.Copy(modelnormal, 0, newmodel, newmodellength, modelnormal.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelunknown3.Length);
                        Array.Copy(modelunknown3, 0, newmodel, newmodellength, modelunknown3.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modeltexture.Length);
                        Array.Copy(modeltexture, 0, newmodel, newmodellength, modeltexture.Length);

                        int newmodelheaderlength = modelheadercount * 8 + newmodelall.Length + 16;
                        if (modelheadercount % 2 != 0)
                            newmodelheaderlength += 8;

                        byte[] modelstartlength = new byte[4];
                        modelstartlength = Gethex(newmodelheaderlength);
                        Array.Resize(ref newmodelheader, newmodelheader.Length + 4);
                        Array.Copy(modelstartlength, 0, newmodelheader, newmodelheader.Length - 4, modelstartlength.Length);

                        string newmodellengthhex = newmodel.Length.ToString("X");
                        string newmodellengthhex0 = newmodellengthhex.Substring(newmodellengthhex.Length - 1, 1);

                        if (newmodellengthhex0 != "0")
                        {
                            while (true)
                            {
                                newmodellength = newmodel.Length;
                                Array.Resize(ref newmodel, newmodel.Length + bs_none.Length);
                                Array.Copy(bs_none, 0, newmodel, newmodellength, bs_none.Length);

                                newmodellengthhex = newmodel.Length.ToString("X");
                                newmodellengthhex0 = newmodellengthhex.Substring(newmodellengthhex.Length - 1, 1);

                                if (newmodellengthhex0 == "0")
                                    break;
                            }
                        }

                        string newmodellengthhex10bai = newmodellengthhex.Substring(0, newmodellengthhex.Length - 1);

                        if (newmodellengthhex10bai.Length == 1)
                            newmodellengthhex10bai = "0" + newmodellengthhex10bai + "00";

                        else if (newmodellengthhex10bai.Length == 2)
                            newmodellengthhex10bai = newmodellengthhex10bai + "00";

                        else if (newmodellengthhex10bai.Length == 3)
                        {
                            string newmodellengthhex10bai1 = newmodellengthhex10bai.Substring(0, 1);
                            string newmodellengthhex10bai2 = newmodellengthhex10bai.Substring(1, 2);
                            newmodellengthhex10bai = newmodellengthhex10bai2 + "0" + newmodellengthhex10bai1;
                        }

                        else if (newmodellengthhex10bai.Length == 4)
                        {
                            string newmodellengthhex10bai1 = newmodellengthhex10bai.Substring(0, 2);
                            string newmodellengthhex10bai2 = newmodellengthhex10bai.Substring(2, 2);
                            newmodellengthhex10bai = newmodellengthhex10bai2 + newmodellengthhex10bai1;
                        }

                        byte[] newmodellength_byte = new byte[2];
                        newmodellength_byte = StringToBytes(newmodellengthhex10bai);
                        Array.Resize(ref newmodelheader, newmodelheader.Length + 2);
                        Array.Copy(newmodellength_byte, 0, newmodelheader, newmodelheader.Length - 2, newmodellength_byte.Length);

                        byte[] newmodelunknown_byte = new byte[2];
                        Array.Copy(bsMDLS2, modelheaderstart + seek_k + 22, newmodelunknown_byte, 0, 2);
                        Array.Resize(ref newmodelheader, newmodelheader.Length + 2);
                        Array.Copy(newmodelunknown_byte, 0, newmodelheader, newmodelheader.Length - 2, newmodelunknown_byte.Length);

                        int newmodelalllength = newmodelall.Length;
                        Array.Resize(ref newmodelall, newmodelall.Length + newmodel.Length);
                        Array.Copy(newmodel, 0, newmodelall, newmodelalllength, newmodel.Length);
                    }

                    newGTM1headernum.Add(newGTM1headerlength2);

                    string newmodelheaderlengthhex = newmodelheader.Length.ToString("X");
                    string newmodelheaderlengthhex0 = newmodelheaderlengthhex.Substring(newmodelheaderlengthhex.Length - 1, 1);

                    if (newmodelheaderlengthhex0 != "0")
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            Array.Resize(ref newmodelheader, newmodelheader.Length + 4);
                            Array.Copy(bs_none, 0, newmodelheader, newmodelheader.Length - 4, bs_none.Length);
                        }
                    }

                    int newGTM1length = newGTM1.Length;
                    Array.Resize(ref newGTM1, newGTM1.Length + newmodelheader.Length);
                    Array.Copy(newmodelheader, 0, newGTM1, newGTM1length, newmodelheader.Length);
                    newGTM1length = newGTM1.Length;
                    Array.Resize(ref newGTM1, newGTM1.Length + newmodelall.Length);
                    Array.Copy(newmodelall, 0, newGTM1, newGTM1length, newmodelall.Length);

                    label1:;
                }

                int newGTM1headerlength = newGTM1headernum.Count * 4;
                string newGTM1headerlengthhex = newGTM1headerlength.ToString("X");
                string newGTM1headerlengthhex0 = newGTM1headerlengthhex.Substring(newGTM1headerlengthhex.Length - 1, 1);
                int kurikaeshi = 0;

                if (newGTM1headerlengthhex0 != "0")
                {
                    while (true)
                    {
                        kurikaeshi += 1;
                        newGTM1headerlength += 4;

                        newGTM1headerlengthhex = newGTM1headerlength.ToString("X");
                        newGTM1headerlengthhex0 = newGTM1headerlengthhex.Substring(newGTM1headerlengthhex.Length - 1, 1);

                        if (newGTM1headerlengthhex0 == "0")
                            break;
                    }
                }

                for (int k = 0; k < newGTM1headernum.Count; k++)
                {
                    byte[] newGTM1headerlength2_byte = new byte[4];
                    newGTM1headerlength2_byte = Gethex(newGTM1headernum[k] + newGTM1headerlength);
                    Array.Resize(ref newGTM1header, newGTM1header.Length + 4);
                    Array.Copy(newGTM1headerlength2_byte, 0, newGTM1header, newGTM1header.Length - 4, newGTM1headerlength2_byte.Length);
                }

                for (int k = 0; k < kurikaeshi; k++)
                {
                    Array.Resize(ref newGTM1header, newGTM1header.Length + 4);
                    Array.Copy(bs_none, 0, newGTM1header, newGTM1header.Length - 4, 4);
                }

                byte[] newfile_GTM1 = new byte[0];
                Array.Resize(ref newfile_GTM1, GTM1headertop.Length);
                Array.Copy(GTM1headertop, 0, newfile_GTM1, 0, GTM1headertop.Length);

                int GTM1header_unknownstart_new = GTM1header_unknownstart;
                int GTM1header_unknownend_new = GTM1header_unknownend;
                int GTM1header_unknown2_new = GTM1header_unknown2;
                int GTM1header_Tex1headerlength_new = GTM1header_unknownend + newGTM1header.Length + modelunknown.Length + newGTM1.Length;
                int GTM1header_headerend_new = GTM1header_unknownend + newGTM1header.Length;
                int GTM1header_unknown3_new = GTM1header_unknown3;
                int GTM1_unknown1_new = GTM1_unknown1 - GTM1_headerlength + newGTM1header.Length;
                int GTM1_unknown2_new = GTM1_unknown2 - GTM1_headerlength + newGTM1header.Length;
                int GTM1_unknown3_new = GTM1_unknown3 - GTM1_headerlength + newGTM1header.Length;
                int GTM1_unknown4_new = GTM1_unknown4 - GTM1_headerlength + newGTM1header.Length;
                int GTM1_unknown5_new = GTM1_unknown5 - GTM1_headerlength + newGTM1header.Length;
                int GTM1_unknown6_new = GTM1_unknown6 - GTM1_headerlength + newGTM1header.Length;
                int GTM1_unknown7_new = GTM1_unknown7 - GTM1_headerlength + newGTM1header.Length;
                int modelheaderlength_new = GTM1header_unknownend + newGTM1header.Length + modelunknown.Length;

                byte[] GTM1headercopy = new byte[4];
                GTM1headercopy = Gethex(GTM1header_unknownstart_new);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(GTM1headercopy, 0, newfile_GTM1, newfile_GTM1.Length - 4, GTM1headercopy.Length);
                GTM1headercopy = Gethex(GTM1header_unknownend_new);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(GTM1headercopy, 0, newfile_GTM1, newfile_GTM1.Length - 4, GTM1headercopy.Length);
                GTM1headercopy = Gethex(GTM1header_unknown2_new);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(GTM1headercopy, 0, newfile_GTM1, newfile_GTM1.Length - 4, GTM1headercopy.Length);
                GTM1headercopy = Gethex(GTM1header_Tex1headerlength_new);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(GTM1headercopy, 0, newfile_GTM1, newfile_GTM1.Length - 4, GTM1headercopy.Length);

                GTM1headercopy = Gethex(GTM1header_headerend_new);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(GTM1headercopy, 0, newfile_GTM1, newfile_GTM1.Length - 4, GTM1headercopy.Length);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(bs_none, 0, newfile_GTM1, newfile_GTM1.Length - 4, bs_none.Length);
                GTM1headercopy = Gethex(GTM1header_unknown3_new);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(GTM1headercopy, 0, newfile_GTM1, newfile_GTM1.Length - 4, GTM1headercopy.Length);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(bs_none, 0, newfile_GTM1, newfile_GTM1.Length - 4, bs_none.Length);

                int newfile_GTM1length = newfile_GTM1.Length;
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + GTM1copy2.Length);
                Array.Copy(GTM1copy2, 0, newfile_GTM1, newfile_GTM1length, GTM1copy2.Length);

                GTM1headercopy = Gethex(GTM1_unknown1_new);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(GTM1headercopy, 0, newfile_GTM1, newfile_GTM1.Length - 4, GTM1headercopy.Length);
                GTM1headercopy = Gethex(GTM1_unknown2_new);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(GTM1headercopy, 0, newfile_GTM1, newfile_GTM1.Length - 4, GTM1headercopy.Length);
                GTM1headercopy = Gethex(GTM1_unknown3_new);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(GTM1headercopy, 0, newfile_GTM1, newfile_GTM1.Length - 4, GTM1headercopy.Length);
                GTM1headercopy = Gethex(GTM1_unknown4_new);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(GTM1headercopy, 0, newfile_GTM1, newfile_GTM1.Length - 4, GTM1headercopy.Length);
                GTM1headercopy = Gethex(GTM1_unknown5_new);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(GTM1headercopy, 0, newfile_GTM1, newfile_GTM1.Length - 4, GTM1headercopy.Length);
                GTM1headercopy = Gethex(GTM1_unknown6_new);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(GTM1headercopy, 0, newfile_GTM1, newfile_GTM1.Length - 4, GTM1headercopy.Length);
                GTM1headercopy = Gethex(GTM1_unknown7_new);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(GTM1headercopy, 0, newfile_GTM1, newfile_GTM1.Length - 4, GTM1headercopy.Length);
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(bs_none, 0, newfile_GTM1, newfile_GTM1.Length - 4, bs_none.Length);

                newfile_GTM1length = newfile_GTM1.Length;
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + newGTM1header.Length);
                Array.Copy(newGTM1header, 0, newfile_GTM1, newfile_GTM1length, newGTM1header.Length);
                newfile_GTM1length = newfile_GTM1.Length;
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + modelunknown.Length);
                Array.Copy(modelunknown, 0, newfile_GTM1, newfile_GTM1length, modelunknown.Length);
                newfile_GTM1length = newfile_GTM1.Length;
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + newGTM1.Length);
                Array.Copy(newGTM1, 0, newfile_GTM1, newfile_GTM1length, newGTM1.Length);

                FileStream fsrlod = new FileStream(readlodfiles[readlodfilesnum[0]].ToString(), FileMode.Open, FileAccess.Read);
                byte[] bslod = new byte[fsrlod.Length];
                fsrlod.Read(bslod, 0, bslod.Length);
                fsrlod.Close();

                int lodstart = Getbyte(bslod, 24);
                int lodend = Getbyte(bslod, 32);
                byte[] bslodMDLS = new byte[lodend - lodstart];
                Array.Copy(bslod, lodstart, bslodMDLS, 0, bslodMDLS.Length);

                bool Tex1hitotsu = false;
                int Tex1headerstart = Getbyte(bslodMDLS, 68);
                //int Tex1start1 = Getbyte(bslodMDLS, Tex1headerstart + 4);
                int Tex1headerstart_menu = Getbyte(bsMDLS2, 68);
                int Tex1start1 = Getbyte(bsMDLS2, Tex1headerstart_menu + 4);
                int Tex1end1 = Getbyte(bsMDLS2, Tex1start1 + 12);
                int Tex1start2 = Getbyte(bslodMDLS, Tex1headerstart + 8);
                int Tex1end2 = Getbyte(bslodMDLS, Tex1start2 + 12);
                int Tex1start3 = Getbyte(bslodMDLS, Tex1headerstart + 12);
                int Tex1end3 = Getbyte(bslodMDLS, Tex1start3 + 12);
                int Tex1start4 = Getbyte(bslodMDLS, Tex1headerstart + 16);
                int Tex1end4 = Getbyte(bslodMDLS, Tex1start4 + 12);
                
                if (Getbyte(bslodMDLS, Tex1headerstart + 20) == 0)
                    Tex1hitotsu = true;

                if (Tex1hitotsu == false)
                {
                    byte[] newTex1header_1 = new byte[12];
                    Array.Copy(bsMDLS2, Tex1start1, newTex1header_1, 0, newTex1header_1.Length);
                    byte[] newTex1header_2 = new byte[12];
                    Array.Copy(bslodMDLS, Tex1start2, newTex1header_2, 0, newTex1header_2.Length);
                    byte[] newTex1header_3 = new byte[12];
                    Array.Copy(bslodMDLS, Tex1start3, newTex1header_3, 0, newTex1header_3.Length);
                    byte[] newTex1header_4 = new byte[12];
                    Array.Copy(bslodMDLS, Tex1start4, newTex1header_4, 0, newTex1header_4.Length);

                    byte[] newTex1data_1 = new byte[Tex1end1 - 16];
                    Array.Copy(bsMDLS2, Tex1start1 + 16, newTex1data_1, 0, newTex1data_1.Length);
                    byte[] newTex1data_2 = new byte[Tex1end2 - 16];
                    Array.Copy(bslodMDLS, Tex1start2 + 16, newTex1data_2, 0, newTex1data_2.Length);
                    byte[] newTex1data_3 = new byte[Tex1end3 - 16];
                    Array.Copy(bslodMDLS, Tex1start3 + 16, newTex1data_3, 0, newTex1data_3.Length);
                    byte[] newTex1data_4 = new byte[Tex1end4 - 16];
                    Array.Copy(bslodMDLS, Tex1start4 + 16, newTex1data_4, 0, newTex1data_4.Length);

                    byte[] Tex1footer = new byte[4] { 0x01, 0x00, 0x00, 0x00 };
                    Array.Resize(ref newTex1data_1, newTex1data_1.Length + 4);
                    Array.Copy(Tex1footer, 0, newTex1data_1, newTex1data_1.Length - 4, 4);
                    Array.Resize(ref newTex1data_2, newTex1data_2.Length + 4);
                    Array.Copy(Tex1footer, 0, newTex1data_2, newTex1data_2.Length - 4, 4);
                    Array.Resize(ref newTex1data_3, newTex1data_3.Length + 4);
                    Array.Copy(Tex1footer, 0, newTex1data_3, newTex1data_3.Length - 4, 4);
                    Array.Resize(ref newTex1data_4, newTex1data_4.Length + 4);
                    Array.Copy(Tex1footer, 0, newTex1data_4, newTex1data_4.Length - 4, 4);

                    int newTex1end1 = Tex1end1 + 16;
                    int newTex1end2 = Tex1end2 + 16;
                    int newTex1end3 = Tex1end3 + 16;
                    int newTex1end4 = Tex1end4 + 16;
                    byte[] newTex1end1_byte = new byte[4];
                    newTex1end1_byte = Gethex(newTex1end1);
                    byte[] newTex1end2_byte = new byte[4];
                    newTex1end2_byte = Gethex(newTex1end2);
                    byte[] newTex1end3_byte = new byte[4];
                    newTex1end3_byte = Gethex(newTex1end3);
                    byte[] newTex1end4_byte = new byte[4];
                    newTex1end4_byte = Gethex(newTex1end4);
                    int Tex1end1footer = newTex1end1 - 8;
                    int Tex1end2footer = newTex1end2 - 8;
                    int Tex1end3footer = newTex1end3 - 8;
                    int Tex1end4footer = newTex1end4 - 8;
                    byte[] newTex1end1footer_byte = new byte[4];
                    newTex1end1footer_byte = Gethex(Tex1end1footer);
                    byte[] newTex1end2footer_byte = new byte[4];
                    newTex1end2footer_byte = Gethex(Tex1end2footer);
                    byte[] newTex1end3footer_byte = new byte[4];
                    newTex1end3footer_byte = Gethex(Tex1end3footer);
                    byte[] newTex1end4footer_byte = new byte[4];
                    newTex1end4footer_byte = Gethex(Tex1end4footer);

                    Array.Resize(ref newTex1header_1, newTex1header_1.Length + 4);
                    Array.Copy(newTex1end1_byte, 0, newTex1header_1, newTex1header_1.Length - 4, 4);
                    Array.Resize(ref newTex1header_2, newTex1header_2.Length + 4);
                    Array.Copy(newTex1end2_byte, 0, newTex1header_2, newTex1header_2.Length - 4, 4);
                    Array.Resize(ref newTex1header_3, newTex1header_3.Length + 4);
                    Array.Copy(newTex1end3_byte, 0, newTex1header_3, newTex1header_3.Length - 4, 4);
                    Array.Resize(ref newTex1header_4, newTex1header_4.Length + 4);
                    Array.Copy(newTex1end4_byte, 0, newTex1header_4, newTex1header_4.Length - 4, 4);

                    Array.Resize(ref newTex1data_1, newTex1data_1.Length + 4);
                    Array.Copy(newTex1end1footer_byte, 0, newTex1data_1, newTex1data_1.Length - 4, 4);
                    Array.Resize(ref newTex1data_2, newTex1data_2.Length + 4);
                    Array.Copy(newTex1end2footer_byte, 0, newTex1data_2, newTex1data_2.Length - 4, 4);
                    Array.Resize(ref newTex1data_3, newTex1data_3.Length + 4);
                    Array.Copy(newTex1end3footer_byte, 0, newTex1data_3, newTex1data_3.Length - 4, 4);
                    Array.Resize(ref newTex1data_4, newTex1data_4.Length + 4);
                    Array.Copy(newTex1end4footer_byte, 0, newTex1data_4, newTex1data_4.Length - 4, 4);

                    for (int k = 0; k < 2; k++)
                    {
                        Array.Resize(ref newTex1data_1, newTex1data_1.Length + 4);
                        Array.Copy(bs_none, 0, newTex1data_1, newTex1data_1.Length - 4, 4);
                        Array.Resize(ref newTex1data_2, newTex1data_2.Length + 4);
                        Array.Copy(bs_none, 0, newTex1data_2, newTex1data_2.Length - 4, 4);
                        Array.Resize(ref newTex1data_3, newTex1data_3.Length + 4);
                        Array.Copy(bs_none, 0, newTex1data_3, newTex1data_3.Length - 4, 4);
                        Array.Resize(ref newTex1data_4, newTex1data_4.Length + 4);
                        Array.Copy(bs_none, 0, newTex1data_4, newTex1data_4.Length - 4, 4);
                    }

                    byte[] newTex1_1 = new byte[0];
                    Array.Resize(ref newTex1_1, newTex1header_1.Length);
                    Array.Copy(newTex1header_1, 0, newTex1_1, 0, newTex1header_1.Length);
                    Array.Resize(ref newTex1_1, newTex1_1.Length + newTex1data_1.Length);
                    Array.Copy(newTex1data_1, 0, newTex1_1, newTex1header_1.Length, newTex1data_1.Length);
                    byte[] newTex1_2 = new byte[0];
                    Array.Resize(ref newTex1_2, newTex1header_2.Length);
                    Array.Copy(newTex1header_2, 0, newTex1_2, 0, newTex1header_2.Length);
                    Array.Resize(ref newTex1_2, newTex1_2.Length + newTex1data_2.Length);
                    Array.Copy(newTex1data_2, 0, newTex1_2, newTex1header_2.Length, newTex1data_2.Length);
                    byte[] newTex1_3 = new byte[0];
                    Array.Resize(ref newTex1_3, newTex1header_3.Length);
                    Array.Copy(newTex1header_3, 0, newTex1_3, 0, newTex1header_3.Length);
                    Array.Resize(ref newTex1_3, newTex1_3.Length + newTex1data_3.Length);
                    Array.Copy(newTex1data_3, 0, newTex1_3, newTex1header_3.Length, newTex1data_3.Length);
                    byte[] newTex1_4 = new byte[0];
                    Array.Resize(ref newTex1_4, newTex1header_4.Length);
                    Array.Copy(newTex1header_4, 0, newTex1_4, 0, newTex1header_4.Length);
                    Array.Resize(ref newTex1_4, newTex1_4.Length + newTex1data_4.Length);
                    Array.Copy(newTex1data_4, 0, newTex1_4, newTex1header_4.Length, newTex1data_4.Length);

                    int newTex1headerlength1 = GTM1header_Tex1headerlength_new + 32;
                    byte[] newTex1headerlength1_byte = Gethex(newTex1headerlength1);
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                    Array.Copy(newTex1headerlength1_byte, 0, newfile_GTM1, newfile_GTM1length, 4);
                    int newTex1headerlength2 = newTex1headerlength1 + newTex1_1.Length;
                    byte[] newTex1headerlength2_byte = Gethex(newTex1headerlength2);
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                    Array.Copy(newTex1headerlength2_byte, 0, newfile_GTM1, newfile_GTM1length, 4);
                    int newTex1headerlength3 = newTex1headerlength2 + newTex1_2.Length;
                    byte[] newTex1headerlength3_byte = Gethex(newTex1headerlength3);
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                    Array.Copy(newTex1headerlength3_byte, 0, newfile_GTM1, newfile_GTM1length, 4);
                    int newTex1headerlength4 = newTex1headerlength3 + newTex1_3.Length;
                    byte[] newTex1headerlength4_byte = Gethex(newTex1headerlength4);
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                    Array.Copy(newTex1headerlength4_byte, 0, newfile_GTM1, newfile_GTM1length, 4);

                    for (int k = 0; k < 4; k++)
                    {
                        newfile_GTM1length = newfile_GTM1.Length;
                        Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                        Array.Copy(bs_none, 0, newfile_GTM1, newfile_GTM1length, 4);
                    }

                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + newTex1_1.Length);
                    Array.Copy(newTex1_1, 0, newfile_GTM1, newfile_GTM1length, newTex1_1.Length);
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + newTex1_2.Length);
                    Array.Copy(newTex1_2, 0, newfile_GTM1, newfile_GTM1length, newTex1_2.Length);
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + newTex1_3.Length);
                    Array.Copy(newTex1_3, 0, newfile_GTM1, newfile_GTM1length, newTex1_3.Length);
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + newTex1_4.Length);
                    Array.Copy(newTex1_4, 0, newfile_GTM1, newfile_GTM1length, newTex1_4.Length);
                }

                else
                {
                    byte[] newTex1header_1 = new byte[12];
                    Array.Copy(bsMDLS2, Tex1start1, newTex1header_1, 0, newTex1header_1.Length);

                    byte[] newTex1data_1 = new byte[Tex1end1 - 16];
                    Array.Copy(bsMDLS2, Tex1start1 + 16, newTex1data_1, 0, newTex1data_1.Length);

                    byte[] Tex1footer = new byte[4] { 0x01, 0x00, 0x00, 0x00 };
                    Array.Resize(ref newTex1data_1, newTex1data_1.Length + 4);
                    Array.Copy(Tex1footer, 0, newTex1data_1, newTex1data_1.Length - 4, 4);

                    int newTex1end1 = Tex1end1 + 16;
                    byte[] newTex1end1_byte = new byte[4];
                    newTex1end1_byte = Gethex(newTex1end1);
                    int Tex1end1footer = newTex1end1 - 8;
                    byte[] newTex1end1footer_byte = new byte[4];
                    newTex1end1footer_byte = Gethex(Tex1end1footer);

                    Array.Resize(ref newTex1header_1, newTex1header_1.Length + 4);
                    Array.Copy(newTex1end1_byte, 0, newTex1header_1, newTex1header_1.Length - 4, 4);

                    Array.Resize(ref newTex1data_1, newTex1data_1.Length + 4);
                    Array.Copy(newTex1end1footer_byte, 0, newTex1data_1, newTex1data_1.Length - 4, 4);

                    for (int k = 0; k < 2; k++)
                    {
                        Array.Resize(ref newTex1data_1, newTex1data_1.Length + 4);
                        Array.Copy(bs_none, 0, newTex1data_1, newTex1data_1.Length - 4, 4);
                    }

                    Tex1start2 = Getbyte(bsGTM1, GTM1header_Tex1headerlength + 4);
                    Tex1end2 = Getbyte(bsGTM1, Tex1start2 + 12);
                    Tex1start3 = Getbyte(bsGTM1, GTM1header_Tex1headerlength + 8);
                    Tex1end3 = Getbyte(bsGTM1, Tex1start3 + 12);
                    Tex1start4 = Getbyte(bsGTM1, GTM1header_Tex1headerlength + 12);
                    Tex1end4 = Getbyte(bsGTM1, Tex1start4 + 12);

                    byte[] newTex1_1 = new byte[0];
                    Array.Resize(ref newTex1_1, newTex1header_1.Length);
                    Array.Copy(newTex1header_1, 0, newTex1_1, 0, newTex1header_1.Length);
                    Array.Resize(ref newTex1_1, newTex1_1.Length + newTex1data_1.Length);
                    Array.Copy(newTex1data_1, 0, newTex1_1, newTex1header_1.Length, newTex1data_1.Length);
                    byte[] newTex1_2 = new byte[0];
                    Array.Resize(ref newTex1_2, Tex1end2);
                    Array.Copy(bsGTM1, Tex1start2, newTex1_2, 0, Tex1end2);
                    byte[] newTex1_3 = new byte[0];
                    Array.Resize(ref newTex1_3, Tex1end3);
                    Array.Copy(bsGTM1, Tex1start3, newTex1_3, 0, Tex1end3);
                    byte[] newTex1_4 = new byte[0];
                    Array.Resize(ref newTex1_4, Tex1end4);
                    Array.Copy(bsGTM1, Tex1start4, newTex1_4, 0, Tex1end4);

                    int newTex1headerlength1 = GTM1header_Tex1headerlength_new + 32;
                    byte[] newTex1headerlength1_byte = Gethex(newTex1headerlength1);
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                    Array.Copy(newTex1headerlength1_byte, 0, newfile_GTM1, newfile_GTM1length, 4);
                    int newTex1headerlength2 = newTex1headerlength1 + newTex1_1.Length;
                    byte[] newTex1headerlength2_byte = Gethex(newTex1headerlength2);
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                    Array.Copy(newTex1headerlength2_byte, 0, newfile_GTM1, newfile_GTM1length, 4);
                    int newTex1headerlength3 = newTex1headerlength2 + newTex1_2.Length;
                    byte[] newTex1headerlength3_byte = Gethex(newTex1headerlength3);
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                    Array.Copy(newTex1headerlength3_byte, 0, newfile_GTM1, newfile_GTM1length, 4);
                    int newTex1headerlength4 = newTex1headerlength3 + newTex1_3.Length;
                    byte[] newTex1headerlength4_byte = Gethex(newTex1headerlength4);
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                    Array.Copy(newTex1headerlength4_byte, 0, newfile_GTM1, newfile_GTM1length, 4);

                    for (int k = 0; k < 4; k++)
                    {
                        newfile_GTM1length = newfile_GTM1.Length;
                        Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                        Array.Copy(bs_none, 0, newfile_GTM1, newfile_GTM1length, 4);
                    }

                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + newTex1_1.Length);
                    Array.Copy(newTex1_1, 0, newfile_GTM1, newfile_GTM1length, newTex1_1.Length);
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + newTex1_2.Length);
                    Array.Copy(newTex1_2, 0, newfile_GTM1, newfile_GTM1length, newTex1_2.Length);
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + newTex1_3.Length);
                    Array.Copy(newTex1_3, 0, newfile_GTM1, newfile_GTM1length, newTex1_3.Length);
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + newTex1_4.Length);
                    Array.Copy(newTex1_4, 0, newfile_GTM1, newfile_GTM1length, newTex1_4.Length);
                }

                /*
                FileStream fsrwheel = new FileStream(readwheelfiles[readwheelfilesnum[0]].ToString(), FileMode.Open, FileAccess.Read);
                byte[] bswheelall = new byte[fsrwheel.Length];
                fsrwheel.Read(bswheelall, 0, bswheelall.Length);
                fsrwheel.Close();
                */
                
                byte[] newfileheader = new byte[12];
                Array.Copy(bsGTM1header, 0, newfileheader, 0, 12);

                int GTM1dif = bsGTM1.Length - newfile_GTM1.Length;
                int GTM1headerGTTR = Getbyte(bsGTM1header, 12);
                GTM1headerGTTR -= GTM1dif;
                byte[] GTM1headerGTTR_byte = new byte[4];
                GTM1headerGTTR_byte = Gethex(GTM1headerGTTR);
                int GTM1headerGTTW = Getbyte(bsGTM1header, 16);
                GTM1headerGTTW -= GTM1dif;
                byte[] GTM1headerGTTW_byte = new byte[4];
                GTM1headerGTTW_byte = Gethex(GTM1headerGTTW);
                int GTM1headerdummy = Getbyte(bsGTM1header, 20);
                GTM1headerdummy -= GTM1dif;
                byte[] GTM1headerdummy_byte = new byte[4];
                GTM1headerdummy_byte = Gethex(GTM1headerdummy);

                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(GTM1headerGTTR_byte, 0, newfileheader, newfileheader.Length - 4, 4);
                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(GTM1headerGTTW_byte, 0, newfileheader, newfileheader.Length - 4, 4);
                Array.Resize(ref newfileheader, newfileheader.Length + 4);
                Array.Copy(GTM1headerdummy_byte, 0, newfileheader, newfileheader.Length - 4, 4);

                for (int k = 0; k < 10; k++)
                {
                    Array.Resize(ref newfileheader, newfileheader.Length + 4);
                    Array.Copy(bs_none, 0, newfileheader, newfileheader.Length - 4, 4);
                }
                
                FileStream fsw = new FileStream(Path.GetDirectoryName(path[i]) + @"\" + Path.GetFileNameWithoutExtension(path[i]) + "_new.bin", FileMode.Create, FileAccess.Write);
                fsw.Write(newfileheader, 0, newfileheader.Length);
                fsw.Write(bsGTM1GTCI, 0, bsGTM1GTCI.Length);
                fsw.Write(newfile_GTM1, 0, newfile_GTM1.Length);
                fsw.Write(bsGTM1GTTR, 0, bsGTM1GTTR.Length);
                fsw.Write(bsGTM1GTTW, 0, bsGTM1GTTW.Length);
                fsw.Write(bsGTM1dummy, 0, bsGTM1dummy.Length);
                fsw.Close();

                goto labelfinish;

            GTM1:;
                //途中

                for (int k = 0; k < readfiles.Count(); k++)
                {
                    if (Path.GetExtension(readfiles[k]) == "")
                        readmenufilesextension.Add(k);
                }

                for (int k = 0; k < readmenufilesextension.Count(); k++)
                {
                    FileStream fsr2 = new FileStream(readfiles[readmenufilesextension[k]].ToString(), FileMode.Open, FileAccess.Read);
                    byte[] bs2 = new byte[fsr2.Length];
                    fsr2.Read(bs2, 0, bs2.Length);
                    if (Getbytestr(bs2, 0) == "43415234")
                        readmenufilesnum.Add(k);
                    fsr2.Close();
                }

                if (readmenufilesnum.Count > 1)
                    goto labelfinish;

                FileStream fsrMDLS2 = new FileStream(readfiles[readmenufilesnum[0]].ToString(), FileMode.Open, FileAccess.Read);
                byte[] bsMDLSall2 = new byte[fsrMDLS2.Length];
                fsrMDLS2.Read(bsMDLSall2, 0, bsMDLSall2.Length);
                fsrMDLS2.Close();

                //ファイルを分割する
                int MDLSlengthstart = Getbyte(bsMDLSall2, 24);
                byte[] bsMDLSon = new byte[MDLSlengthstart];
                Array.Copy(bsMDLSall2, 0, bsMDLSon, 0, MDLSlengthstart);

                MDLSlengthstart = Getbyte(bsMDLSall2, 24);
                int MDLSlengthend = Getbyte(bsMDLSall2, 28);
                int MDLSlength2 = MDLSlengthend - MDLSlengthstart;
                byte[] bsMDLS = new byte[MDLSlength2];
                Array.Copy(bsMDLSall2, MDLSlengthstart, bsMDLS, 0, MDLSlength2);

                MDLSlengthstart = Getbyte(bsMDLSall2, 28);
                MDLSlengthend = bsMDLSall2.Length;
                MDLSlength2 = MDLSlengthend - MDLSlengthstart;
                byte[] bsMDLSunder = new byte[MDLSlength2];
                Array.Copy(bsMDLSall2, MDLSlengthstart, bsMDLSunder, 0, MDLSlength2);

                int GTM1lengthstart2 = Getbyte(bs, 8);
                int GTM1lengthend2 = Getbyte(bs, 12);
                int GTM1length2_2 = GTM1lengthend2 - GTM1lengthstart2;
                byte[] bsGTM12 = new byte[GTM1length2_2];
                Array.Copy(bs, GTM1lengthstart2, bsGTM12, 0, GTM1length2_2);

                //GTM1のヘッダーまでの長さ
                int GTM1headerlength = Getbyte(bsGTM12, 36);
                //GTM1のヘッダー終わりまでの長さ
                int GTM1headerlengthend = Getbyte(bsGTM12, 48);

                GTM1_MDLSheader = GTM1headerlengthend - GTM1headerlength;
                GTM1_MDLSheader /= 4;

                //MDLSのヘッダーまでの長さ
                int MDLSheaderlength2 = Getbyte(bsMDLS, 60);
                //MDLSのヘッダー終わりまでの長さ
                int MDLSheaderlengthend2 = Getbyte(bsMDLS, 56);

                for (j = 0; j < GTM1_MDLSheader; j++)
                {

                }

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
