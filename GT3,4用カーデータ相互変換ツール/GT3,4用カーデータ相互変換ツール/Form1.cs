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
            byte[] bs_none1 = new byte[1] { 0x00 };

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
                    goto convertGTM1;

                filecheck = Getbytestr(bs, 64);

                if (filecheck == "47544349")
                    goto convertMDLS;

                else
                    goto labelfinish;

                convertMDLS:;

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

                //GTM1を分割する
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

                //MDLSを分割する
                FileStream fsrGT4car = new FileStream(readmenufiles[readmenufilesnum[0]].ToString(), FileMode.Open, FileAccess.Read);
                byte[] bsMDLSall = new byte[fsrGT4car.Length];
                fsrGT4car.Read(bsMDLSall, 0, bsMDLSall.Length);
                fsrGT4car.Close();

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

                bool modelunknownnot5 = false;
                bool modelunknown110 = false;

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
                        modelunknownnot5 = false;
                        modelunknown110 = false;
                        int seek_k = k * 8;
                        int modelstartget = Getbyte(bsMDLS2, modelheaderstart + 16 + seek_k);
                        int verticestart = modelheaderstart + modelstartget + 20;
                        int verticecount = Getbyte1(bsMDLS2, modelheaderstart + modelstartget + 18);

                        int modelstart = modelheaderstart + modelstartget;
                        byte[] modelunknown1 = new byte[20];
                        Array.Copy(bsMDLS2, modelstart, modelunknown1, 0, 20);
                        int verticelength = verticecount * 3 * 4;
                        byte[] modelvertice = new byte[verticelength];
                        Array.Copy(bsMDLS2, verticestart, modelvertice, 0, verticelength);
                        byte[] modelunknown2 = new byte[4];
                        byte[] modelnormal = new byte[verticelength];
                        int normalto = verticestart + verticelength + 4; 
                        int modelnormal_none_length = verticecount * 4;
                        byte[] modelnormal_none = new byte[modelnormal_none_length];
                        byte[] modelunknown3 = new byte[4];
                        int texturelength = verticecount * 2 * 4;
                        byte[] modeltexture = new byte[texturelength];

                        int modelunknown2if = Getbyte1(bsMDLS2, verticestart + verticelength + 3);
                        int modelunknown4int = 0;
                        int modelunknown4length = 0;

                        byte[] modelunknown4header = new byte[4];
                        byte[] modelunknown4 = new byte[0];
                        
                        if (modelunknown2if == 5)
                        {
                            modelunknown110 = true;
                            Array.Copy(bsMDLS2, verticestart + verticelength + 4, modelunknown2, 0, 4);
                            Array.Copy(bsMDLS2, normalto + 4, modelnormal_none, 0, modelnormal_none_length);
                            int modelunknown3if = Getbyte1(bsMDLS2, normalto + 4 + modelnormal_none_length + 3);
                            int modelunknown3int = Getbyte(bsMDLS2, normalto + 4 + modelnormal_none_length);

                            if (modelunknown3if != 5)
                            {
                                modelunknownnot5 = true;
                                modelunknown3int += 268435456;
                                modelunknown3 = BitConverter.GetBytes(modelunknown3int);
                                int modelunknown4if = Getbyte1(bsMDLS2, normalto + 4 + modelnormal_none_length + 4 + texturelength + 3);
                                if (modelunknown4if != 5)
                                {
                                    texturelength = verticelength;
                                    Array.Resize(ref modeltexture, texturelength);
                                }
                                Array.Copy(bsMDLS2, normalto + 4 + modelnormal_none_length + 4, modeltexture, 0, texturelength);
                                modelunknown4int = Getbyte(bsMDLS2, normalto + 4 + modelnormal_none_length + 4 + texturelength + 12);
                                modelunknown4int -= 268435456;
                                modelunknown4header = BitConverter.GetBytes(modelunknown4int);
                                modelunknown4length = Getbyte1(bsMDLS2, normalto + 4 + modelnormal_none_length + 4 + texturelength + 14);

                                while (true)
                                {
                                    if (modelunknown4length % 4 == 0)
                                        break;
                                    else
                                        modelunknown4length += 1;
                                }

                                Array.Resize(ref modelunknown4, modelunknown4length);
                                Array.Copy(bsMDLS2, normalto + 4 + modelnormal_none_length + 4 + texturelength + 16, modelunknown4, 0, modelunknown4length);
                            }

                            else
                            {
                                modelunknown4int = Getbyte(bsMDLS2, normalto + 4 + modelnormal_none_length + 12);
                                modelunknown4int -= 268435456;
                                modelunknown4header = BitConverter.GetBytes(modelunknown4int);
                                modelunknown4length = Getbyte1(bsMDLS2, normalto + 4 + modelnormal_none_length + 14);

                                while (true)
                                {
                                    if (modelunknown4length % 4 == 0)
                                        break;
                                    else
                                        modelunknown4length += 1;
                                }

                                Array.Resize(ref modelunknown4, modelunknown4length);
                                Array.Copy(bsMDLS2, normalto + 4 + modelnormal_none_length + 16, modelunknown4, 0, modelunknown4length);
                            }
                        }

                        else
                        {
                            Array.Copy(bsMDLS2, verticestart + verticelength, modelunknown2, 0, 4);
                            Array.Copy(bsMDLS2, normalto, modelnormal, 0, verticelength);
                            int modelunknown3if = Getbyte1(bsMDLS2, normalto + verticelength + 3);
                            int modelunknown3int = Getbyte(bsMDLS2, normalto + verticelength);

                            if (modelunknown3if != 5)
                            {
                                modelunknownnot5 = true;
                                modelunknown3int += 268435456;
                                modelunknown3 = BitConverter.GetBytes(modelunknown3int);
                                int modelunknown4if = Getbyte1(bsMDLS2, normalto + verticelength + 4 + texturelength + 3);
                                if (modelunknown4if != 5)
                                {
                                    texturelength = verticelength;
                                    Array.Resize(ref modeltexture, texturelength);
                                }
                                Array.Copy(bsMDLS2, normalto + verticelength + 4, modeltexture, 0, texturelength);
                                modelunknown4int = Getbyte(bsMDLS2, normalto + verticelength + 4 + texturelength + 12);
                                modelunknown4int -= 268435456;
                                modelunknown4header = BitConverter.GetBytes(modelunknown4int);
                                modelunknown4length = Getbyte1(bsMDLS2, normalto + verticelength + 4 + texturelength + 14);

                                while (true)
                                {
                                    if (modelunknown4length % 4 == 0)
                                        break;
                                    else
                                        modelunknown4length += 1;
                                }

                                Array.Resize(ref modelunknown4, modelunknown4length);
                                Array.Copy(bsMDLS2, normalto + verticelength + 4 + texturelength + 16, modelunknown4, 0, modelunknown4length);
                            }

                            else
                            {
                                modelunknown4int = Getbyte(bsMDLS2, normalto + verticelength + 12);
                                modelunknown4int -= 268435456;
                                modelunknown4header = BitConverter.GetBytes(modelunknown4int);
                                modelunknown4length = Getbyte1(bsMDLS2, normalto + verticelength + 14);

                                while (true)
                                {
                                    if (modelunknown4length % 4 == 0)
                                        break;
                                    else
                                        modelunknown4length += 1;
                                }

                                Array.Resize(ref modelunknown4, modelunknown4length);
                                Array.Copy(bsMDLS2, normalto + verticelength + 16, modelunknown4, 0, modelunknown4length);
                            }
                        }

                        //変換先のbyte配列を用意する
                        byte[] newmodel = new byte[0];
                        int newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelunknown1.Length);
                        Array.Copy(modelunknown1, 0, newmodel, newmodellength, modelunknown1.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelvertice.Length);
                        Array.Copy(modelvertice, 0, newmodel, newmodellength, modelvertice.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelunknown4header.Length);
                        Array.Copy(modelunknown4header, 0, newmodel, newmodellength, modelunknown4header.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelunknown4.Length);
                        Array.Copy(modelunknown4, 0, newmodel, newmodellength, modelunknown4.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelunknown2.Length);
                        Array.Copy(modelunknown2, 0, newmodel, newmodellength, modelunknown2.Length);
                        newmodellength = newmodel.Length;

                        if (modelunknown110 == true)
                        {
                            Array.Resize(ref newmodel, newmodel.Length + modelnormal_none.Length);
                            Array.Copy(modelnormal_none, 0, newmodel, newmodellength, modelnormal_none.Length);
                        }
                        else
                        {
                            Array.Resize(ref newmodel, newmodel.Length + modelnormal.Length);
                            Array.Copy(modelnormal, 0, newmodel, newmodellength, modelnormal.Length);
                        }
                        newmodellength = newmodel.Length;

                        if (modelunknownnot5 == true)
                        {
                            Array.Resize(ref newmodel, newmodel.Length + modelunknown3.Length);
                            Array.Copy(modelunknown3, 0, newmodel, newmodellength, modelunknown3.Length);
                            newmodellength = newmodel.Length;
                            Array.Resize(ref newmodel, newmodel.Length + modeltexture.Length);
                            Array.Copy(modeltexture, 0, newmodel, newmodellength, modeltexture.Length);
                            newmodellength = newmodel.Length;
                        }

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

                //Tex1
                int Tex1headerstart = Getbyte(bsMDLS2, 68);
                int Tex1start1 = Getbyte(bsMDLS2, Tex1headerstart + 4);
                int Tex1end1 = Getbyte(bsMDLS2, Tex1start1 + 12);

                byte[] newTex1header = new byte[12];
                Array.Copy(bsMDLS2, Tex1start1, newTex1header, 0, newTex1header.Length);

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

                Array.Resize(ref newTex1header, newTex1header.Length + 4);
                Array.Copy(newTex1end1_byte, 0, newTex1header, newTex1header.Length - 4, 4);

                Array.Resize(ref newTex1data_1, newTex1data_1.Length + 4);
                Array.Copy(newTex1end1footer_byte, 0, newTex1data_1, newTex1data_1.Length - 4, 4);

                for (int k = 0; k < 2; k++)
                {
                    Array.Resize(ref newTex1data_1, newTex1data_1.Length + 4);
                    Array.Copy(bs_none, 0, newTex1data_1, newTex1data_1.Length - 4, 4);
                }

                byte[] newTex1 = new byte[0];
                Array.Resize(ref newTex1, newTex1header.Length);
                Array.Copy(newTex1header, 0, newTex1, 0, newTex1header.Length);
                Array.Resize(ref newTex1, newTex1.Length + newTex1data_1.Length);
                Array.Copy(newTex1data_1, 0, newTex1, newTex1header.Length, newTex1data_1.Length);

                int newTex1headerlength1 = GTM1header_Tex1headerlength_new + 16;
                byte[] newTex1headerlength1_byte = Gethex(newTex1headerlength1);
                newfile_GTM1length = newfile_GTM1.Length;
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                Array.Copy(newTex1headerlength1_byte, 0, newfile_GTM1, newfile_GTM1length, 4);

                for (int k = 0; k < 3; k++)
                {
                    newfile_GTM1length = newfile_GTM1.Length;
                    Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + 4);
                    Array.Copy(bs_none, 0, newfile_GTM1, newfile_GTM1length, 4);
                }

                newfile_GTM1length = newfile_GTM1.Length;
                Array.Resize(ref newfile_GTM1, newfile_GTM1.Length + newTex1.Length);
                Array.Copy(newTex1, 0, newfile_GTM1, newfile_GTM1length, newTex1.Length);

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

            convertGTM1:;

                string menufolder_GTM1 = Path.GetDirectoryName(path[i]) + @"\menu";
                if (Directory.Exists(menufolder_GTM1) == false)
                    goto labellodcheck;

                string[] readmenufiles_GTM1 = Directory.GetFiles(menufolder_GTM1, "*", SearchOption.AllDirectories);
                for (int k = 0; k < readmenufiles_GTM1.Count(); k++)
                {
                    if (Path.GetExtension(readmenufiles_GTM1[k]) == ".bin" || Path.GetExtension(readmenufiles_GTM1[k]) == ".BIN")
                        readmenufilesextension.Add(k);
                }

                for (int k = 0; k < readmenufilesextension.Count(); k++)
                {
                    FileStream fsr2 = new FileStream(readmenufiles_GTM1[readmenufilesextension[k]].ToString(), FileMode.Open, FileAccess.Read);
                    byte[] bs2 = new byte[fsr2.Length];
                    fsr2.Read(bs2, 0, bs2.Length);
                    if (Getbytestr(bs2, 64) == "47544349")
                        readmenufilesnum.Add(k);
                    fsr2.Close();
                }

                if (readmenufilesnum.Count != 1)
                    goto labellodcheck;
                else
                    goto labelmenu;

                labellodcheck:;

                string lodfolder_GTM1 = Path.GetDirectoryName(path[i]) + @"\lod";
                List<int> readlodfilesextension_GTM1 = new List<int>();
                List<int> readlodfilesnum_GTM1 = new List<int>();
                if (Directory.Exists(lodfolder_GTM1) == false)
                    goto labelfinish;

                string[] readlodfiles_GTM1 = Directory.GetFiles(lodfolder_GTM1, "*", SearchOption.AllDirectories);
                for (int k = 0; k < readlodfiles_GTM1.Count(); k++)
                {
                    if (Path.GetExtension(readlodfiles_GTM1[k]) == ".bin" || Path.GetExtension(readlodfiles_GTM1[k]) == ".BIN")
                        readlodfilesextension_GTM1.Add(k);
                }

                for (int k = 0; k < readlodfilesextension_GTM1.Count(); k++)
                {
                    FileStream fsrlod2 = new FileStream(readlodfiles_GTM1[readlodfilesextension_GTM1[k]].ToString(), FileMode.Open, FileAccess.Read);
                    byte[] bslod2 = new byte[fsrlod2.Length];
                    fsrlod2.Read(bslod2, 0, bslod2.Length);
                    if (Getbytestr(bslod2, 64) == "47544349")
                        readlodfilesnum_GTM1.Add(readlodfilesextension_GTM1[k]);
                    fsrlod2.Close();
                }

                if (readlodfilesnum_GTM1.Count != 1)
                    goto labelfinish;
                else
                    goto labellod;

                labelmenu:;
                goto labelfinish;

            labellod:;
                
                byte[] bsGT4carall = new byte[bs.Length];
                Array.Copy(bs, 0, bsGT4carall, 0, bs.Length);

                //MDLSを分割する
                int GT4carlengthstart = Getbyte(bsGT4carall, 24);
                byte[] bsMDLSon = new byte[GT4carlengthstart - 64];
                Array.Copy(bsGT4carall, 64, bsMDLSon, 0, GT4carlengthstart - 64);

                GT4carlengthstart = Getbyte(bsGT4carall, 24);
                int GT4carlengthend = Getbyte(bsGT4carall, 32);
                int GT4carlength2 = GT4carlengthend - GT4carlengthstart;
                byte[] bsMDLS = new byte[GT4carlength2];
                Array.Copy(bsGT4carall, GT4carlengthstart, bsMDLS, 0, GT4carlength2);

                GT4carlengthstart = Getbyte(bsGT4carall, 32);
                GT4carlengthend = Getbyte(bsGT4carall, 36);
                GT4carlength2 = GT4carlengthend - GT4carlengthstart;
                byte[] bsMDLS2nd = new byte[GT4carlength2];
                Array.Copy(bsGT4carall, GT4carlengthstart, bsMDLS2nd, 0, GT4carlength2);

                GT4carlengthstart = Getbyte(bsGT4carall, 36);
                GT4carlengthend = Getbyte(bsGT4carall, 44);
                GT4carlength2 = GT4carlengthend - GT4carlengthstart;
                byte[] bspat0 = new byte[GT4carlength2];
                Array.Copy(bsGT4carall, GT4carlengthstart, bspat0, 0, GT4carlength2);

                GT4carlengthstart = Getbyte(bsGT4carall, 44);
                GT4carlengthend = Getbyte(bsGT4carall, 48);
                GT4carlength2 = GT4carlengthend - GT4carlengthstart;
                byte[] bsMDLSX = new byte[GT4carlength2];
                Array.Copy(bsGT4carall, GT4carlengthstart, bsMDLSX, 0, GT4carlength2);

                GT4carlengthstart = Getbyte(bsGT4carall, 48);
                GT4carlengthend = Getbyte(bsGT4carall, 52);
                GT4carlength2 = GT4carlengthend - GT4carlengthstart;
                byte[] bsMDLSX2nd = new byte[GT4carlength2];
                Array.Copy(bsGT4carall, GT4carlengthstart, bsMDLSX2nd, 0, GT4carlength2);

                GT4carlengthstart = Getbyte(bsGT4carall, 52);
                GT4carlengthend = Getbyte(bsGT4carall, 8);
                GT4carlength2 = GT4carlengthend - GT4carlengthstart;
                byte[] bsMDLSd = new byte[GT4carlength2];
                Array.Copy(bsGT4carall, GT4carlengthstart, bsMDLSd, 0, GT4carlength2);

                //GTM1を分割する
                FileStream fsrGTM1 = new FileStream(readlodfiles_GTM1[readlodfilesnum_GTM1[0]].ToString(), FileMode.Open, FileAccess.Read);
                byte[] bsGT3car = new byte[fsrGTM1.Length];
                fsrGTM1.Read(bsGT3car, 0, bsGT3car.Length);
                fsrGTM1.Close();

                int GTM1lengthstart2 = Getbyte(bsGT3car, 8);
                int GTM1lengthend2 = Getbyte(bsGT3car, 12);
                int GTM1length2_2 = GTM1lengthend2 - GTM1lengthstart2;
                byte[] bsGTM12 = new byte[GTM1length2_2];
                Array.Copy(bsGT3car, GTM1lengthstart2, bsGTM12, 0, GTM1length2_2);

                //GTM1のヘッダーまでの長さ
                int GTM1headerstart = Getbyte(bsGTM12, 36);
                //GTM1のヘッダー終わりまでの長さ
                int GTM1headerend = Getbyte(bsGTM12, 48);

                GTM1_MDLSheader = GTM1headerend - GTM1headerstart;
                GTM1_MDLSheader /= 4;
                
                int GTM1lengthTex1header = Getbyte(bsGTM12, 44);

                //MDLSのヘッダーまでの長さ
                int MDLSheaderlength2 = Getbyte(bsMDLS, 60);
                //MDLSのヘッダー終わりまでの長さ
                int MDLSheaderlengthend2 = Getbyte(bsMDLS, 56);

                int MDLSlengthend_unknown2 = Getbyte(bsMDLS, 4);
                int MDLSlengthend = Getbyte(bsMDLS, 16);
                int MDLSlengthTex1header = Getbyte(bsMDLS, 68);
                int MDLSlengthend_unknown = Getbyte(bsMDLS, 100);

                byte[] MDLSend_unknown = new byte[MDLSlengthend_unknown2 - MDLSlengthend_unknown];
                Array.Copy(bsMDLS, MDLSlengthend_unknown, MDLSend_unknown, 0, MDLSlengthend_unknown2 - MDLSlengthend_unknown);
                byte[] MDLSend_unknown2 = new byte[MDLSlengthend - MDLSlengthend_unknown2];
                Array.Copy(bsMDLS, MDLSlengthend_unknown2, MDLSend_unknown2, 0, MDLSlengthend - MDLSlengthend_unknown2);

                byte[] MDLSheader_unknown = new byte[8];
                Array.Copy(bsMDLS, 8, MDLSheader_unknown, 0, 8);
                byte[] MDLSheader_unknown2 = new byte[36];
                Array.Copy(bsMDLS, 20, MDLSheader_unknown2, 0, 36);
                byte[] MDLSheader_unknown3 = new byte[4];
                Array.Copy(bsMDLS, 64, MDLSheader_unknown3, 0, 4);
                byte[] MDLSheader_unknown4 = new byte[28];
                Array.Copy(bsMDLS, 72, MDLSheader_unknown4, 0, 28);
                byte[] MDLSheader_unknown5 = new byte[MDLSheaderlength2 - 104];
                Array.Copy(bsMDLS, 104, MDLSheader_unknown5, 0, MDLSheaderlength2 - 104);

                byte[] MDLS_byte = new byte[4] { 0x4D, 0x44, 0x4C, 0x53 };

                int modelheaderlength_2 = Getbyte(bsMDLS, MDLSheaderlength2);
                byte[] modelunknown_2 = new byte[modelheaderlength_2 - MDLSheaderlengthend2];
                Array.Copy(bsMDLS, MDLSheaderlengthend2, modelunknown_2, 0, modelheaderlength_2 - MDLSheaderlengthend2);

                byte[] newMDLSheader = new byte[0];
                byte[] newMDLS = new byte[0];
                List<int> newMDLSheadernum = new List<int>();

                byte[] unknown05 = new byte[4] { 0x00, 0x00, 0x00, 0x05 };
                byte[] unknown12byte = new byte[12] { 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x20, 0x3F, 0x3F, 0x3F, 0x3F };

                bool modelunknown104 = false;
                
                for (j = 0; j < GTM1_MDLSheader; j++)
                {
                    byte[] newmodelall = new byte[0];
                    
                    int seek_j = j * 4;
                    //モデルデータのヘッダーまでの長さ
                    int modelheaderstart = Getbyte(bsGTM12, GTM1headerstart + seek_j);
                    
                    if (modelheaderstart == 0)
                        break;

                    int newMDLSheaderlength2 = MDLSheaderlength2 + modelunknown_2.Length + newMDLS.Length;

                    int modelheadercount = Getbyte2(bsGTM12, modelheaderstart + 10);
                    byte[] newmodelheader = new byte[16];
                    Array.Copy(bsGTM12, modelheaderstart, newmodelheader, 0, 16);
                    
                    for (int k = 0; k < modelheadercount; k++)
                    {
                        modelunknown104 = false;
                        int seek_k = k * 8;
                        int modelstartget = Getbyte(bsGTM12, modelheaderstart + 16 + seek_k);
                        int verticestart = modelheaderstart + modelstartget + 20;
                        int verticecount = Getbyte1(bsGTM12, modelheaderstart + modelstartget + 18);
                        
                        int modelstart = modelheaderstart + modelstartget;
                        byte[] modelunknown1 = new byte[20];
                        Array.Copy(bsGTM12, modelstart, modelunknown1, 0, 20);
                        int verticelength = verticecount * 3 * 4;
                        byte[] modelvertice = new byte[verticelength];
                        Array.Copy(bsGTM12, verticestart, modelvertice, 0, verticelength);
                        
                        int modelunknown2int = Getbyte(bsGTM12, verticestart + verticelength);
                        modelunknown2int += 268435456;
                        byte[] modelunknown2header = new byte[4];
                        modelunknown2header = BitConverter.GetBytes(modelunknown2int);

                        int modelunknown2length = Getbyte1(bsGTM12, verticestart + verticelength + 2);
                        
                        while (true)
                        {
                            if (modelunknown2length % 4 == 0)
                                break;
                            else
                                modelunknown2length += 1;
                        }
                        
                        byte[] modelunknown2 = new byte[modelunknown2length];
                        Array.Copy(bsGTM12, verticestart + verticelength + 4, modelunknown2, 0, modelunknown2length);

                        byte[] modelunknown3 = new byte[4];
                        Array.Copy(bsGTM12, verticestart + verticelength + 4 + modelunknown2length, modelunknown3, 0, 4);

                        int normalto = verticestart + verticelength + 4 + modelunknown2length + 4;
                        int modelnormal_none_length = verticecount * 4;
                        byte[] modelnormal_none = new byte[modelnormal_none_length];
                        byte[] modelnormal = new byte[verticelength];
                        byte[] modelunknown4 = new byte[4];
                        int texturelength = verticecount * 2 * 4;
                        byte[] modeltexture = new byte[texturelength];

                        int modelunknown3if = Getbyte1(bsGTM12, verticestart + verticelength + 4 + modelunknown2length + 3);

                        if (modelunknown3if != 104)
                        {
                            Array.Copy(bsGTM12, normalto, modelnormal_none, 0, modelnormal_none_length);
                            int modelunknown4int = Getbyte(bsGTM12, normalto + modelnormal_none_length);
                            modelunknown4int -= 268435456;
                            modelunknown4 = BitConverter.GetBytes(modelunknown4int);
                            Array.Copy(bsGTM12, normalto + modelnormal_none_length + 4, modeltexture, 0, texturelength);
                        }

                        else
                        {
                            modelunknown104 = true;
                            Array.Copy(bsGTM12, normalto, modelnormal, 0, verticelength);
                            int modelunknown4int = Getbyte(bsGTM12, normalto + verticelength);
                            modelunknown4int -= 268435456;
                            modelunknown4 = BitConverter.GetBytes(modelunknown4int);
                            Array.Copy(bsGTM12, normalto + verticelength + 4, modeltexture, 0, texturelength);
                        }

                        //変換先のbyte配列を用意する
                        byte[] newmodel = new byte[0];
                        int newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelunknown1.Length);
                        Array.Copy(modelunknown1, 0, newmodel, newmodellength, modelunknown1.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelvertice.Length);
                        Array.Copy(modelvertice, 0, newmodel, newmodellength, modelvertice.Length);
                        newmodellength = newmodel.Length;
                        
                        if (modelunknown104 == true)
                        {
                            Array.Resize(ref newmodel, newmodel.Length + modelunknown3.Length);
                            Array.Copy(modelunknown3, 0, newmodel, newmodellength, modelunknown3.Length);
                            newmodellength = newmodel.Length;
                            Array.Resize(ref newmodel, newmodel.Length + modelnormal.Length);
                            Array.Copy(modelnormal, 0, newmodel, newmodellength, modelnormal.Length);
                            newmodellength = newmodel.Length;
                        }
                        else
                        {
                            Array.Resize(ref newmodel, newmodel.Length + unknown05.Length);
                            Array.Copy(unknown05, 0, newmodel, newmodellength, unknown05.Length);
                            newmodellength = newmodel.Length;
                            Array.Resize(ref newmodel, newmodel.Length + modelunknown3.Length);
                            Array.Copy(modelunknown3, 0, newmodel, newmodellength, modelunknown3.Length);
                            newmodellength = newmodel.Length;
                            Array.Resize(ref newmodel, newmodel.Length + modelnormal_none.Length);
                            Array.Copy(modelnormal_none, 0, newmodel, newmodellength, modelnormal_none.Length);
                            newmodellength = newmodel.Length;
                        }

                        Array.Resize(ref newmodel, newmodel.Length + modelunknown4.Length);
                        Array.Copy(modelunknown4, 0, newmodel, newmodellength, modelunknown4.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modeltexture.Length);
                        Array.Copy(modeltexture, 0, newmodel, newmodellength, modeltexture.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + unknown12byte.Length);
                        Array.Copy(unknown12byte, 0, newmodel, newmodellength, unknown12byte.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelunknown2header.Length);
                        Array.Copy(modelunknown2header, 0, newmodel, newmodellength, modelunknown2header.Length);
                        newmodellength = newmodel.Length;
                        Array.Resize(ref newmodel, newmodel.Length + modelunknown2.Length);
                        Array.Copy(modelunknown2, 0, newmodel, newmodellength, modelunknown2.Length);
                        newmodellength = newmodel.Length;
                        
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
                        Array.Copy(bsGTM12, modelheaderstart + seek_k + 22, newmodelunknown_byte, 0, 2);
                        Array.Resize(ref newmodelheader, newmodelheader.Length + 2);
                        Array.Copy(newmodelunknown_byte, 0, newmodelheader, newmodelheader.Length - 2, newmodelunknown_byte.Length);

                        int newmodelalllength = newmodelall.Length;
                        Array.Resize(ref newmodelall, newmodelall.Length + newmodel.Length);
                        Array.Copy(newmodel, 0, newmodelall, newmodelalllength, newmodel.Length);
                    }

                    newMDLSheadernum.Add(newMDLSheaderlength2);

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

                    int newMDLSlength = newMDLS.Length;
                    Array.Resize(ref newMDLS, newMDLS.Length + newmodelheader.Length);
                    Array.Copy(newmodelheader, 0, newMDLS, newMDLSlength, newmodelheader.Length);
                    newMDLSlength = newMDLS.Length;
                    Array.Resize(ref newMDLS, newMDLS.Length + newmodelall.Length);
                    Array.Copy(newmodelall, 0, newMDLS, newMDLSlength, newmodelall.Length);
                }
                
                int newMDLSheaderlength = newMDLSheadernum.Count * 4;
                string newMDLSheaderlengthhex = newMDLSheaderlength.ToString("X");
                string newMDLSheaderlengthhex0 = newMDLSheaderlengthhex.Substring(newMDLSheaderlengthhex.Length - 1, 1);
                int kurikaeshi2 = 0;

                if (newMDLSheaderlengthhex0 != "0")
                {
                    while (true)
                    {
                        kurikaeshi2 += 1;
                        newMDLSheaderlength += 4;

                        newMDLSheaderlengthhex = newMDLSheaderlength.ToString("X");
                        newMDLSheaderlengthhex0 = newMDLSheaderlengthhex.Substring(newMDLSheaderlengthhex.Length - 1, 1);

                        if (newMDLSheaderlengthhex0 == "0")
                            break;
                    }
                }
                
                for (int k = 0; k < newMDLSheadernum.Count; k++)
                {
                    byte[] newMDLSheaderlength2_byte = new byte[4];
                    newMDLSheaderlength2_byte = Gethex(newMDLSheadernum[k] + newMDLSheaderlength);
                    Array.Resize(ref newMDLSheader, newMDLSheader.Length + 4);
                    Array.Copy(newMDLSheaderlength2_byte, 0, newMDLSheader, newMDLSheader.Length - 4, newMDLSheaderlength2_byte.Length);
                }
                
                for (int k = 0; k < kurikaeshi2; k++)
                {
                    Array.Resize(ref newMDLSheader, newMDLSheader.Length + 4);
                    Array.Copy(bs_none, 0, newMDLSheader, newMDLSheader.Length - 4, 4);
                }
                
                int MDLSTex1headerlength_new = MDLSheaderlength2 + newMDLSheader.Length + modelunknown_2.Length + newMDLS.Length;

                int MDLSTex1start_unknown = Getbyte(bsMDLS, MDLSlengthTex1header + 20);
                int MDLSTex1start_unknown2 = Getbyte(bsMDLS, MDLSlengthTex1header + 24);
                byte[] MDLSTex1_unknown = new byte[0];
                byte[] MDLSTex1_unknown2 = new byte[0];
                
                if (MDLSTex1start_unknown != 0)
                {
                    int MDLSTex1length_unknown = 128;
                    Array.Resize(ref MDLSTex1_unknown, MDLSTex1length_unknown);
                    Array.Copy(bsMDLS, MDLSTex1start_unknown, MDLSTex1_unknown, 0, MDLSTex1length_unknown);

                    if (MDLSTex1start_unknown2 != 0)
                    {
                        int MDLSTex1length_unknown2 = Getbyte(bsMDLS, MDLSTex1start_unknown2 + 12);
                        Array.Resize(ref MDLSTex1_unknown2, MDLSTex1length_unknown2);
                        Array.Copy(bsMDLS, MDLSTex1start_unknown2, MDLSTex1_unknown2, 0, MDLSTex1length_unknown2);
                    }
                }
                
                byte[] GTM1Tex1header = new byte[12] { 0x54, 0x65, 0x78, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                int GTM1Tex1start_1 = Getbyte(bsGTM12, GTM1lengthTex1header);
                int GTM1Tex1start_2 = Getbyte(bsGTM12, GTM1lengthTex1header + 4);
                int GTM1Tex1start_3 = Getbyte(bsGTM12, GTM1lengthTex1header + 8);
                int GTM1Tex1start_4 = Getbyte(bsGTM12, GTM1lengthTex1header + 12);
                int GTM1Tex1length_1 = Getbyte(bsGTM12, GTM1Tex1start_1 + 12);
                GTM1Tex1length_1 -= 16;
                int GTM1Tex1length_2 = Getbyte(bsGTM12, GTM1Tex1start_2 + 12);
                GTM1Tex1length_2 -= 16;
                int GTM1Tex1length_3 = Getbyte(bsGTM12, GTM1Tex1start_3 + 12);
                GTM1Tex1length_3 -= 16;
                int GTM1Tex1length_4 = Getbyte(bsGTM12, GTM1Tex1start_4 + 12);
                GTM1Tex1length_4 -= 16;
                byte[] GTM1Tex1_1 = new byte[GTM1Tex1length_1 - 16];
                Array.Copy(bsGTM12, GTM1Tex1start_1 + 16, GTM1Tex1_1, 0, GTM1Tex1length_1 - 16);
                byte[] GTM1Tex1_2 = new byte[GTM1Tex1length_2 - 16];
                Array.Copy(bsGTM12, GTM1Tex1start_2 + 16, GTM1Tex1_2, 0, GTM1Tex1length_2 - 16);
                byte[] GTM1Tex1_3 = new byte[GTM1Tex1length_3 - 16];
                Array.Copy(bsGTM12, GTM1Tex1start_3 + 16, GTM1Tex1_3, 0, GTM1Tex1length_3 - 16);
                byte[] GTM1Tex1_4 = new byte[GTM1Tex1length_4 - 16];
                Array.Copy(bsGTM12, GTM1Tex1start_4 + 16, GTM1Tex1_4, 0, GTM1Tex1length_4 - 16);

                byte[] GTM1Tex1length_1_new = new byte[4];
                GTM1Tex1length_1_new = Gethex(GTM1Tex1length_1);
                byte[] GTM1Tex1length_2_new = new byte[4];
                GTM1Tex1length_2_new = Gethex(GTM1Tex1length_2);
                byte[] GTM1Tex1length_3_new = new byte[4];
                GTM1Tex1length_3_new = Gethex(GTM1Tex1length_3);
                byte[] GTM1Tex1length_4_new = new byte[4];
                GTM1Tex1length_4_new = Gethex(GTM1Tex1length_4);

                int MDLSTex1start_1_new = MDLSTex1headerlength_new + 32;
                byte[] MDLSTex1start_1_byte = new byte[4];
                MDLSTex1start_1_byte = Gethex(MDLSTex1start_1_new);
                int MDLSTex1start_2_new = MDLSTex1headerlength_new + 32 + GTM1Tex1length_1;
                byte[] MDLSTex1start_2_byte = new byte[4];
                MDLSTex1start_2_byte = Gethex(MDLSTex1start_2_new);
                int MDLSTex1start_3_new = MDLSTex1headerlength_new + 32 + GTM1Tex1length_1 + GTM1Tex1length_2;
                byte[] MDLSTex1start_3_byte = new byte[4];
                MDLSTex1start_3_byte = Gethex(MDLSTex1start_3_new);
                int MDLSTex1start_4_new = MDLSTex1headerlength_new + 32 + GTM1Tex1length_1 + GTM1Tex1length_2 + GTM1Tex1length_3;
                byte[] MDLSTex1start_4_byte = new byte[4];
                MDLSTex1start_4_byte = Gethex(MDLSTex1start_4_new);
                int MDLSTex1start_unknown_new = 0;
                byte[] MDLSTex1start_unknown_byte = new byte[4];
                int MDLSTex1start_unknown2_new = 0;
                byte[] MDLSTex1start_unknown2_byte = new byte[4];

                if (MDLSTex1start_unknown != 0)
                {
                    MDLSTex1start_unknown_new = MDLSTex1start_4_new + GTM1Tex1length_4;
                    MDLSTex1start_unknown_byte = Gethex(MDLSTex1start_unknown_new);
                    if (MDLSTex1start_unknown2 != 0)
                    {
                        MDLSTex1start_unknown2_new = MDLSTex1start_4_new + GTM1Tex1length_4 + MDLSTex1_unknown.Length;
                        MDLSTex1start_unknown2_byte = Gethex(MDLSTex1start_unknown2_new);
                    }
                }

                byte[] MDLSTex1header_new = new byte[4];
                MDLSTex1header_new = Gethex(MDLSTex1start_1_new - 28);

                byte[] newfile_Tex1 = new byte[0];
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 4);
                Array.Copy(MDLSTex1header_new, 0, newfile_Tex1, 0, 4);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 4);
                Array.Copy(MDLSTex1start_1_byte, 0, newfile_Tex1, newfile_Tex1.Length - 4, 4);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 4);
                Array.Copy(MDLSTex1start_2_byte, 0, newfile_Tex1, newfile_Tex1.Length - 4, 4);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 4);
                Array.Copy(MDLSTex1start_3_byte, 0, newfile_Tex1, newfile_Tex1.Length - 4, 4);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 4);
                Array.Copy(MDLSTex1start_4_byte, 0, newfile_Tex1, newfile_Tex1.Length - 4, 4);

                if (MDLSTex1start_unknown != 0)
                {
                    Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 4);
                    Array.Copy(MDLSTex1start_unknown_byte, 0, newfile_Tex1, newfile_Tex1.Length - 4, 4);
                    if (MDLSTex1start_unknown2 != 0)
                    {
                        Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 4);
                        Array.Copy(MDLSTex1start_unknown2_byte, 0, newfile_Tex1, newfile_Tex1.Length - 4, 4);
                    }
                }

                string newTex1headerlengthhex = newfile_Tex1.Length.ToString("X");
                string newTex1headerlengthhex0 = newTex1headerlengthhex.Substring(newTex1headerlengthhex.Length - 1, 1);

                if (newTex1headerlengthhex0 != "0")
                {
                    while (true)
                    {
                        Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + bs_none.Length);
                        Array.Copy(bs_none, 0, newfile_Tex1, newfile_Tex1.Length - 4, bs_none.Length);

                        newTex1headerlengthhex = newfile_Tex1.Length.ToString("X");
                        newTex1headerlengthhex0 = newTex1headerlengthhex.Substring(newTex1headerlengthhex.Length - 1, 1);

                        if (newTex1headerlengthhex0 == "0")
                            break;
                    }
                }

                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 12);
                Array.Copy(GTM1Tex1header, 0, newfile_Tex1, newfile_Tex1.Length - 12, 12);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 4);
                Array.Copy(GTM1Tex1length_1_new, 0, newfile_Tex1, newfile_Tex1.Length - 4, 4);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + GTM1Tex1_1.Length);
                Array.Copy(GTM1Tex1_1, 0, newfile_Tex1, newfile_Tex1.Length - GTM1Tex1_1.Length, GTM1Tex1_1.Length);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 12);
                Array.Copy(GTM1Tex1header, 0, newfile_Tex1, newfile_Tex1.Length - 12, 12);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 4);
                Array.Copy(GTM1Tex1length_2_new, 0, newfile_Tex1, newfile_Tex1.Length - 4, 4);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + GTM1Tex1_2.Length);
                Array.Copy(GTM1Tex1_2, 0, newfile_Tex1, newfile_Tex1.Length - GTM1Tex1_2.Length, GTM1Tex1_2.Length);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 12);
                Array.Copy(GTM1Tex1header, 0, newfile_Tex1, newfile_Tex1.Length - 12, 12);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 4);
                Array.Copy(GTM1Tex1length_3_new, 0, newfile_Tex1, newfile_Tex1.Length - 4, 4);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + GTM1Tex1_3.Length);
                Array.Copy(GTM1Tex1_3, 0, newfile_Tex1, newfile_Tex1.Length - GTM1Tex1_3.Length, GTM1Tex1_3.Length);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 12);
                Array.Copy(GTM1Tex1header, 0, newfile_Tex1, newfile_Tex1.Length - 12, 12);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + 4);
                Array.Copy(GTM1Tex1length_4_new, 0, newfile_Tex1, newfile_Tex1.Length - 4, 4);
                Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + GTM1Tex1_4.Length);
                Array.Copy(GTM1Tex1_4, 0, newfile_Tex1, newfile_Tex1.Length - GTM1Tex1_4.Length, GTM1Tex1_4.Length);

                if (MDLSTex1start_unknown != 0)
                {
                    Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + MDLSTex1_unknown.Length);
                    Array.Copy(MDLSTex1_unknown, 0, newfile_Tex1, newfile_Tex1.Length - MDLSTex1_unknown.Length, MDLSTex1_unknown.Length);
                    if (MDLSTex1start_unknown2 != 0)
                    {
                        Array.Resize(ref newfile_Tex1, newfile_Tex1.Length + MDLSTex1_unknown2.Length);
                        Array.Copy(MDLSTex1_unknown2, 0, newfile_Tex1, newfile_Tex1.Length - MDLSTex1_unknown2.Length, MDLSTex1_unknown2.Length);
                    }
                }

                byte[] newfile_MDLS = new byte[0];
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + 4);
                Array.Copy(MDLS_byte, 0, newfile_MDLS, 0, 4);

                int MDLSheaderlength2_new = MDLSheaderlength2;
                byte[] MDLSheaderlength2_new_byte = new byte[4];
                MDLSheaderlength2_new_byte = Gethex(MDLSheaderlength2_new);
                int MDLSheaderlengthend2_new = MDLSheaderlength2 + newMDLSheader.Length;
                byte[] MDLSheaderlengthend2_new_byte = new byte[4];
                MDLSheaderlengthend2_new_byte = Gethex(MDLSheaderlengthend2_new);
                int MDLSlengthend_unknown2_new = MDLSheaderlength2 + newMDLSheader.Length + modelunknown_2.Length + newMDLS.Length + newfile_Tex1.Length + MDLSend_unknown.Length;
                byte[] MDLSlengthend_unknown2_new_byte = new byte[4];
                MDLSlengthend_unknown2_new_byte = Gethex(MDLSlengthend_unknown2_new);
                int MDLSlengthend_new = MDLSheaderlength2 + newMDLSheader.Length + modelunknown_2.Length + newMDLS.Length + newfile_Tex1.Length + MDLSend_unknown.Length + MDLSend_unknown2.Length;
                byte[] MDLSlengthend_new_byte = new byte[4];
                MDLSlengthend_new_byte = Gethex(MDLSlengthend_new);
                int MDLSlengthTex1header_new = MDLSheaderlength2 + newMDLSheader.Length + modelunknown_2.Length + newMDLS.Length;
                byte[] MDLSlengthTex1header_new_byte = new byte[4];
                MDLSlengthTex1header_new_byte = Gethex(MDLSlengthTex1header_new);
                int MDLSlengthend_unknown_new = MDLSheaderlength2 + newMDLSheader.Length + modelunknown_2.Length + newMDLS.Length + newfile_Tex1.Length;
                byte[] MDLSlengthend_unknown_new_byte = new byte[4];
                MDLSlengthend_unknown_new_byte = Gethex(MDLSlengthend_unknown_new);

                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + 4);
                Array.Copy(MDLSlengthend_unknown2_new_byte, 0, newfile_MDLS, newfile_MDLS.Length - 4, 4);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + MDLSheader_unknown.Length);
                Array.Copy(MDLSheader_unknown, 0, newfile_MDLS, newfile_MDLS.Length - MDLSheader_unknown.Length, MDLSheader_unknown.Length);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + 4);
                Array.Copy(MDLSlengthend_new_byte, 0, newfile_MDLS, newfile_MDLS.Length - 4, 4);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + MDLSheader_unknown2.Length);
                Array.Copy(MDLSheader_unknown2, 0, newfile_MDLS, newfile_MDLS.Length - MDLSheader_unknown2.Length, MDLSheader_unknown2.Length);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + 4);
                Array.Copy(MDLSheaderlengthend2_new_byte, 0, newfile_MDLS, newfile_MDLS.Length - 4, 4);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + 4);
                Array.Copy(MDLSheaderlength2_new_byte, 0, newfile_MDLS, newfile_MDLS.Length - 4, 4);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + 4);
                Array.Copy(MDLSheader_unknown3, 0, newfile_MDLS, newfile_MDLS.Length - 4, 4);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + 4);
                Array.Copy(MDLSlengthTex1header_new_byte, 0, newfile_MDLS, newfile_MDLS.Length - 4, 4);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + MDLSheader_unknown4.Length);
                Array.Copy(MDLSheader_unknown4, 0, newfile_MDLS, newfile_MDLS.Length - MDLSheader_unknown4.Length, MDLSheader_unknown4.Length);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + 4);
                Array.Copy(MDLSlengthend_unknown_new_byte, 0, newfile_MDLS, newfile_MDLS.Length - 4, 4);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + MDLSheader_unknown5.Length);
                Array.Copy(MDLSheader_unknown5, 0, newfile_MDLS, newfile_MDLS.Length - MDLSheader_unknown5.Length, MDLSheader_unknown5.Length);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + newMDLSheader.Length);
                Array.Copy(newMDLSheader, 0, newfile_MDLS, newfile_MDLS.Length - newMDLSheader.Length, newMDLSheader.Length);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + modelunknown_2.Length);
                Array.Copy(modelunknown_2, 0, newfile_MDLS, newfile_MDLS.Length - modelunknown_2.Length, modelunknown_2.Length);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + newMDLS.Length);
                Array.Copy(newMDLS, 0, newfile_MDLS, newfile_MDLS.Length - newMDLS.Length, newMDLS.Length);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + newfile_Tex1.Length);
                Array.Copy(newfile_Tex1, 0, newfile_MDLS, newfile_MDLS.Length - newfile_Tex1.Length, newfile_Tex1.Length);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + MDLSend_unknown.Length);
                Array.Copy(MDLSend_unknown, 0, newfile_MDLS, newfile_MDLS.Length - MDLSend_unknown.Length, MDLSend_unknown.Length);
                Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + MDLSend_unknown2.Length);
                Array.Copy(MDLSend_unknown2, 0, newfile_MDLS, newfile_MDLS.Length - MDLSend_unknown2.Length, MDLSend_unknown2.Length);

                string newfile_MDLSlengthhex = newfile_MDLS.Length.ToString("X");
                string newfile_MDLSlengthhex0 = newfile_MDLSlengthhex.Substring(newfile_MDLSlengthhex.Length - 1, 1);

                if (newfile_MDLSlengthhex0 != "0")
                {
                    while (true)
                    {
                        Array.Resize(ref newfile_MDLS, newfile_MDLS.Length + 1);
                        Array.Copy(bs_none1, 0, newfile_MDLS, newfile_MDLS.Length - 1, bs_none1.Length);

                        newfile_MDLSlengthhex = newfile_MDLS.Length.ToString("X");
                        newfile_MDLSlengthhex0 = newfile_MDLSlengthhex.Substring(newfile_MDLSlengthhex.Length - 1, 1);

                        if (newfile_MDLSlengthhex0 == "0")
                            break;
                    }
                }

                byte[] CAR4_byte = new byte[4] { 0x43, 0x41, 0x52, 0x34 };
                byte[] newfile_CAR4 = new byte[0];
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 4);
                Array.Copy(CAR4_byte, 0, newfile_CAR4, 0, 4);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 4);
                Array.Copy(bs_none, 0, newfile_CAR4, newfile_CAR4.Length - 4, 4);
                int CAR4length_new = 64 + bsMDLSon.Length + newfile_MDLS.Length + bsMDLS2nd.Length + bspat0.Length + bsMDLSX.Length + bsMDLSX2nd.Length + bsMDLSd.Length;
                byte[] CAR4length_new_byte = new byte[4];
                CAR4length_new_byte = Gethex(CAR4length_new);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 4);
                Array.Copy(CAR4length_new_byte, 0, newfile_CAR4, newfile_CAR4.Length - 4, 4);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 4);
                Array.Copy(bs_none, 0, newfile_CAR4, newfile_CAR4.Length - 4, 4);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 8);
                Array.Copy(bsGT4carall, 16, newfile_CAR4, newfile_CAR4.Length - 8, 8);
                byte[] CAR4header = new byte[4];
                CAR4header = Gethex(64 + bsMDLSon.Length);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 4);
                Array.Copy(CAR4header, 0, newfile_CAR4, newfile_CAR4.Length - 4, 4);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 4);
                Array.Copy(bs_none, 0, newfile_CAR4, newfile_CAR4.Length - 4, 4);
                CAR4header = Gethex(64 + bsMDLSon.Length + newfile_MDLS.Length);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 4);
                Array.Copy(CAR4header, 0, newfile_CAR4, newfile_CAR4.Length - 4, 4);
                CAR4header = Gethex(64 + bsMDLSon.Length + newfile_MDLS.Length + bsMDLS2nd.Length);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 4);
                Array.Copy(CAR4header, 0, newfile_CAR4, newfile_CAR4.Length - 4, 4);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 4);
                Array.Copy(bs_none, 0, newfile_CAR4, newfile_CAR4.Length - 4, 4);
                CAR4header = Gethex(64 + bsMDLSon.Length + newfile_MDLS.Length + bsMDLS2nd.Length + bspat0.Length);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 4);
                Array.Copy(CAR4header, 0, newfile_CAR4, newfile_CAR4.Length - 4, 4);
                CAR4header = Gethex(64 + bsMDLSon.Length + newfile_MDLS.Length + bsMDLS2nd.Length + bspat0.Length + bsMDLSX.Length);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 4);
                Array.Copy(CAR4header, 0, newfile_CAR4, newfile_CAR4.Length - 4, 4);
                CAR4header = Gethex(64 + bsMDLSon.Length + newfile_MDLS.Length + bsMDLS2nd.Length + bspat0.Length + bsMDLSX.Length + bsMDLSX2nd.Length);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 4);
                Array.Copy(CAR4header, 0, newfile_CAR4, newfile_CAR4.Length - 4, 4);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 4);
                Array.Copy(bs_none, 0, newfile_CAR4, newfile_CAR4.Length - 4, 4);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + 4);
                Array.Copy(bs_none, 0, newfile_CAR4, newfile_CAR4.Length - 4, 4);
                
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + bsMDLSon.Length);
                Array.Copy(bsMDLSon, 0, newfile_CAR4, newfile_CAR4.Length - bsMDLSon.Length, bsMDLSon.Length);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + newfile_MDLS.Length);
                Array.Copy(newfile_MDLS, 0, newfile_CAR4, newfile_CAR4.Length - newfile_MDLS.Length, newfile_MDLS.Length);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + bsMDLS2nd.Length);
                Array.Copy(bsMDLS2nd, 0, newfile_CAR4, newfile_CAR4.Length - bsMDLS2nd.Length, bsMDLS2nd.Length);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + bspat0.Length);
                Array.Copy(bspat0, 0, newfile_CAR4, newfile_CAR4.Length - bspat0.Length, bspat0.Length);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + bsMDLSX.Length);
                Array.Copy(bsMDLSX, 0, newfile_CAR4, newfile_CAR4.Length - bsMDLSX.Length, bsMDLSX.Length);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + bsMDLSX2nd.Length);
                Array.Copy(bsMDLSX2nd, 0, newfile_CAR4, newfile_CAR4.Length - bsMDLSX2nd.Length, bsMDLSX2nd.Length);
                Array.Resize(ref newfile_CAR4, newfile_CAR4.Length + bsMDLSd.Length);
                Array.Copy(bsMDLSd, 0, newfile_CAR4, newfile_CAR4.Length - bsMDLSd.Length, bsMDLSd.Length);

                FileStream fsw2 = new FileStream(Path.GetDirectoryName(path[i]) + @"\" + Path.GetFileNameWithoutExtension(path[i]) + "_new", FileMode.Create, FileAccess.Write);
                fsw2.Write(newfile_CAR4, 0, newfile_CAR4.Length);
                fsw2.Close();

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
