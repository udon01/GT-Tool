using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BMPConverter
{
    public partial class Form1 : Form
    {
        public bool braketexture = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Openfolderbutton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdconvert = new FolderBrowserDialog();
            
            fbdconvert.Description = "Please specify the folder";
            fbdconvert.RootFolder = Environment.SpecialFolder.Desktop;
            fbdconvert.SelectedPath = @"C:\";
            
            if (fbdconvert.ShowDialog() == DialogResult.OK)
                Folderpathtext.Text = fbdconvert.SelectedPath;
        }

        private void Extractbutton_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog fbdextract = new FolderBrowserDialog();
            
            fbdextract.Description = "Please specify the folder";
            fbdextract.RootFolder = Environment.SpecialFolder.Desktop;
            fbdextract.SelectedPath = @"C:\";
            
            if (fbdextract.ShowDialog() == DialogResult.OK)
                Extractpathtext.Text = fbdextract.SelectedPath;
        }

        private void Convertbutton_Click(object sender, EventArgs e)
        {
            bool daburi = false;
            DirectoryInfo di = null;
            FileInfo[] files = null;
            try
            {
                di = new DirectoryInfo(Folderpathtext.Text);
                files = di.GetFiles("*.bmp", SearchOption.AllDirectories);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("The folder containing the BMPs to be converted is not specified");
            }

            if (files == null)
                goto labelfinish;

            if (IsOnlyAlphanumeric2(BrakeTexture.Text) == true || BrakeTexture.Text == "")
                braketexture = true;

            else
            {
                MessageBox.Show("Please enter up to 2 digits for the brake texture number");
                goto labelfinish;
            }

            string Extractpath = "";
            if (Extractpathtext.Text == "")
            {
                string user = Environment.UserName;
                Extractpath = @"C:\Users\" + user + @"\Desktop\";

                if (!Directory.Exists(Extractpath + "BMP"))
                    Directory.CreateDirectory(Extractpath + "BMP");
            }

            //Conversion process from here
            //BMP conversion
            byte[] binaryBMP = new byte[] { 0x42, 0x4D, 0x76, 0x70, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x76, 0x00, 0x00, 0x00,
                    0x28, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0xE0, 0x00, 0x00, 0x00, 0x01, 0x00, 0x04, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x70, 0x00, 0x00, 0xF0, 0x0A, 0x00, 0x00, 0xF0, 0x0A, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00,
                    0x10, 0x00, 0x00, 0x00 };
            int BMPallnumber = 0;
            int colortortalmax = 0;

            for (int i = 0; i < files.Count(); i++)
            {
                FileStream fsrBMP = new FileStream(Folderpathtext.Text + @"\" + files.ElementAt(i).ToString(), FileMode.Open, FileAccess.Read);
                byte[] fsrbyte = new byte[fsrBMP.Length];
                fsrBMP.Read(fsrbyte, 0, (int)fsrBMP.Length);
                byte[] colortortalbyte = new byte[1];
                fsrBMP.Seek(46, SeekOrigin.Begin);
                fsrBMP.Read(colortortalbyte, 0, 1);
                string colortortalstring = BitConverter.ToString(colortortalbyte, 0);
                if (colortortalstring.Length == 1)
                    colortortalstring = "0" + colortortalstring;
                int colortortal = Convert.ToInt32(colortortalstring, 16);

                if (colortortalmax < colortortal)
                {
                    BMPallnumber = i;
                    colortortalmax = colortortal;
                }

                byte[] colorbyte = new byte[colortortal * 4];
                int seek = 138;
                fsrBMP.Seek(seek, SeekOrigin.Begin);
                fsrBMP.Read(colorbyte, 0, colortortal * 4);
                seek = seek + colortortal * 4;
                fsrBMP.Close();

                byte[] nocolor = new byte[] { 0xFF, 0xFF, 0xFF, 0x00 };
                int nocolortortal = 16 - colortortal;

                FileStream fswBMP = File.Create(Extractpath + @"BMP\" + files.ElementAt(i).ToString());
                int seek_w = 54;
                fswBMP.Write(binaryBMP, 0, 54);
                fswBMP.Seek(54, SeekOrigin.Begin);
                fswBMP.Write(colorbyte, 0, colorbyte.Length);
                seek_w = seek_w + colorbyte.Length;

                for (int j = 0; j < nocolortortal; j++)
                {
                    fswBMP.Write(nocolor, 0, 4);
                    seek_w = seek_w + 4;
                }
                while (true)
                {
                    fswBMP.Write(fsrbyte, seek, 1);
                    seek = seek + 1;

                    if (seek == fsrbyte.Length)
                        break;
                }
                fswBMP.Close();
            }

            //BMP (summary) generation
            FileStream fsrBMPall = File.Create(Extractpath + @"BMP\" + "BMPall" + ".bmp");
            string BMPall = Extractpath + @"BMP\" + "BMPall" + ".bmp";
            int seek_BMPall = 118;
            FileStream fsrBMPwheel = new FileStream(Extractpath + @"BMP\" + files.ElementAt(BMPallnumber).ToString(), FileMode.Open, FileAccess.Read);
            byte[] fsrBMPall_dainyu = new byte[118];
            fsrBMPwheel.Read(fsrBMPall_dainyu, 0, 118);
            fsrBMPwheel.Close();
            fsrBMPall.Write(fsrBMPall_dainyu, 0, 118);
            fsrBMPall.Seek(seek_BMPall, SeekOrigin.Begin);
            int braketextureint = 99;

            if (braketexture == true)
            {
                for (int i = 0; i < files.Count(); i++)
                {
                    if (files.ElementAt(i).ToString().IndexOf(BrakeTexture.Text) >= 0)
                    {
                        braketextureint = i;
                        break;
                    }
                }
            }

            byte[] fsrBMPall_all0 = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            for (int i = 0; i < 1792; i++)
            {
                fsrBMPall.Write(fsrBMPall_all0, 0, 16);
                seek_BMPall = seek_BMPall + 16;
                fsrBMPall.Seek(seek_BMPall, SeekOrigin.Begin);
            }

            for (int i = 0; i < files.Count(); i++)
            {
                if (i != braketextureint)
                {
                    FileStream fsrBMP = new FileStream(Extractpath + @"BMP\" + files.ElementAt(i).ToString(), FileMode.Open, FileAccess.Read);
                    int seek_BMPall_dubble = 118;
                    for (int j = 0; j < 28672; j++)
                    {
                        byte[] fsrBMPall_read = new byte[1];
                        byte[] fsrBMP_read = new byte[1];
                        fsrBMPall.Seek(seek_BMPall_dubble, SeekOrigin.Begin);
                        fsrBMP.Seek(seek_BMPall_dubble, SeekOrigin.Begin);
                        fsrBMP.Read(fsrBMP_read, 0, 1);
                        if (fsrBMP_read[0] != 0x00)
                        {
                            fsrBMPall.Read(fsrBMPall_read, 0, 1);
                            if (fsrBMPall_read[0] == 0x00)
                            {
                                fsrBMPall.Seek(seek_BMPall_dubble, SeekOrigin.Begin);
                                fsrBMPall.Write(fsrBMP_read, 0, 1);
                            }
                            else
                            {
                                fsrBMPall.Seek(seek_BMPall_dubble, SeekOrigin.Begin);
                                fsrBMPall.Read(fsrBMPall_read, 0, 1);
                                byte[] seek_BMPall_write = new byte[1];
                                string fsrBMPall_dainyu_string;
                                string fsrBMPall_read_string = BitConverter.ToString(fsrBMPall_read);
                                string fsrBMPall_read_string1 = fsrBMPall_read_string.Substring(0, 1);
                                string fsrBMPall_read_string2 = fsrBMPall_read_string.Substring(1, 1);
                                string fsrBMP_read_string = BitConverter.ToString(fsrBMP_read);
                                string fsrBMP_read_string1 = fsrBMP_read_string.Substring(0, 1);
                                string fsrBMP_read_string2 = fsrBMP_read_string.Substring(1, 1);

                                if (fsrBMPall_read_string1 == "0" && fsrBMP_read_string2.IndexOf("0") >= 0)
                                {
                                    fsrBMPall_dainyu_string = fsrBMP_read_string1 + fsrBMPall_read_string2;
                                    seek_BMPall_write = StringToBytes(fsrBMPall_dainyu_string);
                                    fsrBMPall.Seek(seek_BMPall_dubble, SeekOrigin.Begin);
                                    fsrBMPall.Write(seek_BMPall_write, 0, 1);
                                }

                                else if (fsrBMPall_read_string2 == "0" && fsrBMP_read_string1.IndexOf("0") >= 0)
                                {
                                    fsrBMPall_dainyu_string = fsrBMPall_read_string1 + fsrBMP_read_string2;
                                    seek_BMPall_write = StringToBytes(fsrBMPall_dainyu_string);
                                    fsrBMPall.Seek(seek_BMPall_dubble, SeekOrigin.Begin);
                                    fsrBMPall.Write(seek_BMPall_write, 0, 1);
                                }

                                else
                                    daburi = true;
                            }

                        }
                        seek_BMPall_dubble = seek_BMPall_dubble + 1;
                    }
                }
            }

            fsrBMPall.Close();


            //pal generation
            byte[] binarypal = new byte[] { 0x4A, 0x41, 0x53, 0x43, 0x2D, 0x50, 0x41, 0x4C, 0x0D, 0x0A, 0x30, 0x31, 0x30, 0x30,
                    0x0D, 0x0A, 0x31, 0x36, 0x0D, 0x0A };
            byte[] number0 = new byte[] { 0x30 };
            byte[] number1 = new byte[] { 0x31 };
            byte[] number2 = new byte[] { 0x32 };
            byte[] number3 = new byte[] { 0x33 };
            byte[] number4 = new byte[] { 0x34 };
            byte[] number5 = new byte[] { 0x35 };
            byte[] number6 = new byte[] { 0x36 };
            byte[] number7 = new byte[] { 0x37 };
            byte[] number8 = new byte[] { 0x38 };
            byte[] number9 = new byte[] { 0x39 };
            byte[] color1or2 = new byte[] { 0x20 };
            byte[] color3 = new byte[] { 0x0D, 0x0A };

            if (!Directory.Exists(Extractpath + "colour00"))
                Directory.CreateDirectory(Extractpath + "colour00");

            for (int i = 0; i < files.Count(); i++)
            {
                string BMPstring = files.ElementAt(i).ToString();
                if (BMPstring.IndexOf("palette") <= 0)
                {
                    BMPstring = BMPstring.Substring(1);
                    BMPstring = "P" + BMPstring;
                    BMPstring = BMPstring.Substring(0, BMPstring.Length - 4);
                    if (BMPstring.Length == 8)
                    {
                        string palettenumber = BMPstring.Substring(7, 1);
                        BMPstring = BMPstring.Substring(0, BMPstring.Length - 1);
                        BMPstring = BMPstring + "0" + palettenumber;
                    }
                }
                FileStream fsrBMP = new FileStream(Extractpath + @"BMP\" + files.ElementAt(i).ToString(), FileMode.Open, FileAccess.Read);
                byte[] colorbyte = new byte[64];
                int seek = 54;
                fsrBMP.Seek(seek, SeekOrigin.Begin);
                fsrBMP.Read(colorbyte, 0, 64);
                fsrBMP.Close();

                FileStream fswpal = File.Create(Extractpath + @"colour00\" + "Colour" + BMPstring + ".pal");
                fswpal.Write(binarypal, 0, 20);
                int seek_copy = 0;
                int seek_fswpal = 20;
                fswpal.Seek(seek_fswpal, SeekOrigin.Begin);
                for (int j = 0; j < 16; j++)
                {
                    byte[] fswpal1byte1_1 = new byte[] { 0x00 };
                    byte[] fswpal1byte1_2 = new byte[] { 0x00 };
                    byte[] fswpal1byte1_3 = new byte[] { 0x00 };
                    byte[] fswpal1byte2_1 = new byte[] { 0x00 };
                    byte[] fswpal1byte2_2 = new byte[] { 0x00 };
                    byte[] fswpal1byte2_3 = new byte[] { 0x00 };
                    byte[] fswpal1byte3_1 = new byte[] { 0x00 };
                    byte[] fswpal1byte3_2 = new byte[] { 0x00 };
                    byte[] fswpal1byte3_3 = new byte[] { 0x00 };

                    for (int k = 0; k < 3; k++)
                    {
                        byte[] fswpal1 = new byte[1];
                        Array.Copy(colorbyte, seek_copy, fswpal1, 0, 1);
                        seek_copy = seek_copy + 1;
                        string fswpal1string = BitConverter.ToString(fswpal1, 0);
                        string fswpal1string1 = "no";
                        string fswpal1string2 = "no";
                        string fswpal1string3 = "no";
                        int fswpal1int = Convert.ToInt32(fswpal1string, 16);
                        fswpal1string = fswpal1int.ToString();

                        if (fswpal1string.Length == 2)
                        {
                            fswpal1string1 = fswpal1string.Substring(0, 1);
                            fswpal1string2 = fswpal1string.Substring(1, 1);
                        }
                        else if (fswpal1string.Length == 3)
                        {
                            fswpal1string1 = fswpal1string.Substring(0, 1);
                            fswpal1string2 = fswpal1string.Substring(1, 1);
                            fswpal1string3 = fswpal1string.Substring(2, 1);
                        }
                        else
                            fswpal1string1 = fswpal1string.Substring(0, 1);

                        if (k == 0)
                        {
                            if (fswpal1string1 == "0")
                                fswpal1byte1_1[0] = number0[0];
                            else if (fswpal1string1 == "1")
                                fswpal1byte1_1[0] = number1[0];
                            else if (fswpal1string1 == "2")
                                fswpal1byte1_1[0] = number2[0];
                            else if (fswpal1string1 == "3")
                                fswpal1byte1_1[0] = number3[0];
                            else if (fswpal1string1 == "4")
                                fswpal1byte1_1[0] = number4[0];
                            else if (fswpal1string1 == "5")
                                fswpal1byte1_1[0] = number5[0];
                            else if (fswpal1string1 == "6")
                                fswpal1byte1_1[0] = number6[0];
                            else if (fswpal1string1 == "7")
                                fswpal1byte1_1[0] = number7[0];
                            else if (fswpal1string1 == "8")
                                fswpal1byte1_1[0] = number8[0];
                            else if (fswpal1string1 == "9")
                                fswpal1byte1_1[0] = number9[0];
                        }

                        if (k == 1)
                        {
                            if (fswpal1string1 == "0")
                                fswpal1byte2_1[0] = number0[0];
                            else if (fswpal1string1 == "1")
                                fswpal1byte2_1[0] = number1[0];
                            else if (fswpal1string1 == "2")
                                fswpal1byte2_1[0] = number2[0];
                            else if (fswpal1string1 == "3")
                                fswpal1byte2_1[0] = number3[0];
                            else if (fswpal1string1 == "4")
                                fswpal1byte2_1[0] = number4[0];
                            else if (fswpal1string1 == "5")
                                fswpal1byte2_1[0] = number5[0];
                            else if (fswpal1string1 == "6")
                                fswpal1byte2_1[0] = number6[0];
                            else if (fswpal1string1 == "7")
                                fswpal1byte2_1[0] = number7[0];
                            else if (fswpal1string1 == "8")
                                fswpal1byte2_1[0] = number8[0];
                            else if (fswpal1string1 == "9")
                                fswpal1byte2_1[0] = number9[0];
                        }

                        if (k == 2)
                        {
                            if (fswpal1string1 == "0")
                                fswpal1byte3_1[0] = number0[0];
                            else if (fswpal1string1 == "1")
                                fswpal1byte3_1[0] = number1[0];
                            else if (fswpal1string1 == "2")
                                fswpal1byte3_1[0] = number2[0];
                            else if (fswpal1string1 == "3")
                                fswpal1byte3_1[0] = number3[0];
                            else if (fswpal1string1 == "4")
                                fswpal1byte3_1[0] = number4[0];
                            else if (fswpal1string1 == "5")
                                fswpal1byte3_1[0] = number5[0];
                            else if (fswpal1string1 == "6")
                                fswpal1byte3_1[0] = number6[0];
                            else if (fswpal1string1 == "7")
                                fswpal1byte3_1[0] = number7[0];
                            else if (fswpal1string1 == "8")
                                fswpal1byte3_1[0] = number8[0];
                            else if (fswpal1string1 == "9")
                                fswpal1byte3_1[0] = number9[0];
                        }

                        if (k == 0)
                        {
                            if (fswpal1string2 == "0")
                                fswpal1byte1_2[0] = number0[0];
                            else if (fswpal1string2 == "1")
                                fswpal1byte1_2[0] = number1[0];
                            else if (fswpal1string2 == "2")
                                fswpal1byte1_2[0] = number2[0];
                            else if (fswpal1string2 == "3")
                                fswpal1byte1_2[0] = number3[0];
                            else if (fswpal1string2 == "4")
                                fswpal1byte1_2[0] = number4[0];
                            else if (fswpal1string2 == "5")
                                fswpal1byte1_2[0] = number5[0];
                            else if (fswpal1string2 == "6")
                                fswpal1byte1_2[0] = number6[0];
                            else if (fswpal1string2 == "7")
                                fswpal1byte1_2[0] = number7[0];
                            else if (fswpal1string2 == "8")
                                fswpal1byte1_2[0] = number8[0];
                            else if (fswpal1string2 == "9")
                                fswpal1byte1_2[0] = number9[0];
                        }

                        if (k == 1)
                        {
                            if (fswpal1string2 == "0")
                                fswpal1byte2_2[0] = number0[0];
                            else if (fswpal1string2 == "1")
                                fswpal1byte2_2[0] = number1[0];
                            else if (fswpal1string2 == "2")
                                fswpal1byte2_2[0] = number2[0];
                            else if (fswpal1string2 == "3")
                                fswpal1byte2_2[0] = number3[0];
                            else if (fswpal1string2 == "4")
                                fswpal1byte2_2[0] = number4[0];
                            else if (fswpal1string2 == "5")
                                fswpal1byte2_2[0] = number5[0];
                            else if (fswpal1string2 == "6")
                                fswpal1byte2_2[0] = number6[0];
                            else if (fswpal1string2 == "7")
                                fswpal1byte2_2[0] = number7[0];
                            else if (fswpal1string2 == "8")
                                fswpal1byte2_2[0] = number8[0];
                            else if (fswpal1string2 == "9")
                                fswpal1byte2_2[0] = number9[0];
                        }

                        if (k == 2)
                        {
                            if (fswpal1string2 == "0")
                                fswpal1byte3_2[0] = number0[0];
                            else if (fswpal1string2 == "1")
                                fswpal1byte3_2[0] = number1[0];
                            else if (fswpal1string2 == "2")
                                fswpal1byte3_2[0] = number2[0];
                            else if (fswpal1string2 == "3")
                                fswpal1byte3_2[0] = number3[0];
                            else if (fswpal1string2 == "4")
                                fswpal1byte3_2[0] = number4[0];
                            else if (fswpal1string2 == "5")
                                fswpal1byte3_2[0] = number5[0];
                            else if (fswpal1string2 == "6")
                                fswpal1byte3_2[0] = number6[0];
                            else if (fswpal1string2 == "7")
                                fswpal1byte3_2[0] = number7[0];
                            else if (fswpal1string2 == "8")
                                fswpal1byte3_2[0] = number8[0];
                            else if (fswpal1string2 == "9")
                                fswpal1byte3_2[0] = number9[0];
                        }

                        if (k == 0)
                        {
                            if (fswpal1string3 == "0")
                                fswpal1byte1_3[0] = number0[0];
                            else if (fswpal1string3 == "1")
                                fswpal1byte1_3[0] = number1[0];
                            else if (fswpal1string3 == "2")
                                fswpal1byte1_3[0] = number2[0];
                            else if (fswpal1string3 == "3")
                                fswpal1byte1_3[0] = number3[0];
                            else if (fswpal1string3 == "4")
                                fswpal1byte1_3[0] = number4[0];
                            else if (fswpal1string3 == "5")
                                fswpal1byte1_3[0] = number5[0];
                            else if (fswpal1string3 == "6")
                                fswpal1byte1_3[0] = number6[0];
                            else if (fswpal1string3 == "7")
                                fswpal1byte1_3[0] = number7[0];
                            else if (fswpal1string3 == "8")
                                fswpal1byte1_3[0] = number8[0];
                            else if (fswpal1string3 == "9")
                                fswpal1byte1_3[0] = number9[0];
                        }

                        if (k == 1)
                        {
                            if (fswpal1string3 == "0")
                                fswpal1byte2_3[0] = number0[0];
                            else if (fswpal1string3 == "1")
                                fswpal1byte2_3[0] = number1[0];
                            else if (fswpal1string3 == "2")
                                fswpal1byte2_3[0] = number2[0];
                            else if (fswpal1string3 == "3")
                                fswpal1byte2_3[0] = number3[0];
                            else if (fswpal1string3 == "4")
                                fswpal1byte2_3[0] = number4[0];
                            else if (fswpal1string3 == "5")
                                fswpal1byte2_3[0] = number5[0];
                            else if (fswpal1string3 == "6")
                                fswpal1byte2_3[0] = number6[0];
                            else if (fswpal1string3 == "7")
                                fswpal1byte2_3[0] = number7[0];
                            else if (fswpal1string3 == "8")
                                fswpal1byte2_3[0] = number8[0];
                            else if (fswpal1string3 == "9")
                                fswpal1byte2_3[0] = number9[0];
                        }

                        if (k == 2)
                        {
                            if (fswpal1string3 == "0")
                                fswpal1byte3_3[0] = number0[0];
                            else if (fswpal1string3 == "1")
                                fswpal1byte3_3[0] = number1[0];
                            else if (fswpal1string3 == "2")
                                fswpal1byte3_3[0] = number2[0];
                            else if (fswpal1string3 == "3")
                                fswpal1byte3_3[0] = number3[0];
                            else if (fswpal1string3 == "4")
                                fswpal1byte3_3[0] = number4[0];
                            else if (fswpal1string3 == "5")
                                fswpal1byte3_3[0] = number5[0];
                            else if (fswpal1string3 == "6")
                                fswpal1byte3_3[0] = number6[0];
                            else if (fswpal1string3 == "7")
                                fswpal1byte3_3[0] = number7[0];
                            else if (fswpal1string3 == "8")
                                fswpal1byte3_3[0] = number8[0];
                            else if (fswpal1string3 == "9")
                                fswpal1byte3_3[0] = number9[0];
                        }
                    }
                    fswpal.Write(fswpal1byte3_1, 0, 1);
                    seek_fswpal = seek_fswpal + 1;
                    fswpal.Seek(seek_fswpal, SeekOrigin.Begin);
                    if (fswpal1byte3_2[0] != 0x00)
                    {
                        fswpal.Write(fswpal1byte3_2, 0, 1);
                        seek_fswpal = seek_fswpal + 1;
                        fswpal.Seek(seek_fswpal, SeekOrigin.Begin);
                    }
                    if (fswpal1byte3_3[0] != 0x00)
                    {
                        fswpal.Write(fswpal1byte3_3, 0, 1);
                        seek_fswpal = seek_fswpal + 1;
                        fswpal.Seek(seek_fswpal, SeekOrigin.Begin);
                    }

                    fswpal.Write(color1or2, 0, 1);
                    seek_fswpal = seek_fswpal + 1;
                    fswpal.Seek(seek_fswpal, SeekOrigin.Begin);

                    fswpal.Write(fswpal1byte2_1, 0, 1);
                    seek_fswpal = seek_fswpal + 1;
                    fswpal.Seek(seek_fswpal, SeekOrigin.Begin);
                    if (fswpal1byte2_2[0] != 0x00)
                    {
                        fswpal.Write(fswpal1byte2_2, 0, 1);
                        seek_fswpal = seek_fswpal + 1;
                        fswpal.Seek(seek_fswpal, SeekOrigin.Begin);
                    }
                    if (fswpal1byte2_3[0] != 0x00)
                    {
                        fswpal.Write(fswpal1byte2_3, 0, 1);
                        seek_fswpal = seek_fswpal + 1;
                        fswpal.Seek(seek_fswpal, SeekOrigin.Begin);
                    }

                    fswpal.Write(color1or2, 0, 1);
                    seek_fswpal = seek_fswpal + 1;
                    fswpal.Seek(seek_fswpal, SeekOrigin.Begin);

                    fswpal.Write(fswpal1byte1_1, 0, 1);
                    seek_fswpal = seek_fswpal + 1;
                    fswpal.Seek(seek_fswpal, SeekOrigin.Begin);
                    if (fswpal1byte1_2[0] != 0x00)
                    {
                        fswpal.Write(fswpal1byte1_2, 0, 1);
                        seek_fswpal = seek_fswpal + 1;
                        fswpal.Seek(seek_fswpal, SeekOrigin.Begin);
                    }
                    if (fswpal1byte1_3[0] != 0x00)
                    {
                        fswpal.Write(fswpal1byte1_3, 0, 1);
                        seek_fswpal = seek_fswpal + 1;
                        fswpal.Seek(seek_fswpal, SeekOrigin.Begin);
                    }

                    fswpal.Write(color3, 0, 2);
                    seek_fswpal = seek_fswpal + 2;
                    fswpal.Seek(seek_fswpal, SeekOrigin.Begin);

                    seek_copy = seek_copy + 1;
                }
            }

            if (daburi == true)
                MessageBox.Show("The conversion could not be done correctly because some of the colors are double painted");
            else
                MessageBox.Show("Done!");
            labelfinish:;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
        public static bool IsOnlyAlphanumeric2(string text)
        {
            return Regex.IsMatch(text, @"^[0-9]+$");
        }

        // Hexadecimal string to byte array
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
