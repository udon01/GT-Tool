using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Ionic.Zip;
using Ionic.Zlib;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace GT6_モデル抽出ツール
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

                FileStream fs_s = new FileStream(path[b], FileMode.Open, FileAccess.Read);
                FileInfo fi_s = new FileInfo(path[b]);
                extractMDL3_HIGHLOD(ref fs_s, ref fi_s);

                string expath_base = Path.GetDirectoryName(path[b]) + @"\" + "body_s_extracted";
                string expath = Path.GetDirectoryName(path[b]) + @"\" + "body_s_extracted";
                for (int i = 0; i < 3; i++)
                {
                    string expath_py = expath.Replace(@"\", "/");
                    StreamWriter sw = new StreamWriter(expath + @"\obj2blender.py");
                    sw.WriteLine("import os");
                    sw.WriteLine("import bpy" + sw.NewLine);
                    sw.WriteLine("path_to_obj_dir = os.path.join(\"" + expath_py + "\")");
                    sw.WriteLine("# ディレクトリ内のすべてのファイルのリストを取得します");
                    sw.WriteLine("file_list = sorted(os.listdir(path_to_obj_dir))" + sw.NewLine);
                    sw.WriteLine("#「obj」で終わるファイルのリストを取得します");
                    sw.WriteLine("obj_list = [item for item in file_list if item[-3:] == 'obj'] # obj, fbx, vrm, etc..." + sw.NewLine);
                    sw.WriteLine("# obj_listの文字列をループし、ファイルをシーンに追加します");
                    sw.WriteLine("for item in obj_list:");
                    sw.WriteLine("    path_to_file = os.path.join(path_to_obj_dir, item)");
                    sw.WriteLine("    bpy.ops.import_scene.obj(filepath = path_to_file) ## obj, fbx, vrm, etc..." + sw.NewLine);
                    sw.Write("#ソース：https://koshishirai.com/blender-import/ 【Blender】複数の3Dモデルを一括でインポートする方法");
                    sw.Close();
                    if (i == 0)
                    {
                        if (Directory.Exists(expath_base + @"\boundingBox"))
                            expath = expath_base + @"\boundingBox";
                    }
                    if (i == 1)
                    {
                        if (Directory.Exists(expath_base + @"\standard"))
                            expath = expath_base + @"\standard";
                    }
                }

            labelfinish:;
            }
        }

        public byte[] ArrayReverse(byte[] array)
        {
            Array.Reverse(array);
            return array;
        }

        public void writeSpecifiedObjFile(
    List<List<float>> posList,
    List<List<ushort>> idxListUshort,
    List<List<ulong>> idxListUlong,
    string outputPath,
    ref FileInfo ffi,
    int vertexOrderIndex,
    string outputName,
    string meshName,
    int facesMethod)
        {
            StreamWriter objSw = new StreamWriter(
                outputPath + "\\" + Path.GetFileNameWithoutExtension(outputName) + meshName + ".obj");
            foreach (List<float> lf in posList)
            {
                string x, y, z;
                if (lf[0].ToString().Contains("E") == true)
                {
                    x = lf[0].ToString("F9");
                }
                else
                {
                    x = lf[0].ToString();
                }
                if (lf[1].ToString().Contains("E"))
                {
                    y = lf[1].ToString("F9");
                }
                else
                {
                    y = lf[1].ToString();
                }
                if (lf[2].ToString().Contains("E"))
                {
                    z = lf[2].ToString("F9");
                }
                else
                {
                    z = lf[2].ToString();
                }
                if (vertexOrderIndex == 0)
                {
                    objSw.WriteLine("v " + x + " " + y + " " + z);
                }
                if (vertexOrderIndex == 1)
                {
                    objSw.WriteLine("v " + x + " " + z + " " + y);
                }
                if (vertexOrderIndex == 2)
                {
                    objSw.WriteLine("v " + y + " " + x + " " + z);
                }
                if (vertexOrderIndex == 3)
                {
                    objSw.WriteLine("v " + y + " " + z + " " + x);
                }
                if (vertexOrderIndex == 4)
                {
                    objSw.WriteLine("v " + z + " " + x + " " + y);
                }
                if (vertexOrderIndex == 5)
                {
                    objSw.WriteLine("v " + z + " " + y + " " + x);
                }
            }
            objSw.WriteLine("g " + ffi.Name);
            //objSw.WriteLine("usemtl " + matList[(int)objGeoSubCountCounter].ToString()); //Add .mtl generation in the future
            if (facesMethod == 0 || facesMethod == 2 || facesMethod == 3) //USHORT or TRI-STRIP or Generated faces
            {
                foreach (List<ushort> lS in idxListUshort)
                {
                    objSw.WriteLine("f  " + (lS[0] + 1).ToString() + "/" + (lS[0] + 1).ToString() + "/" + (lS[0] + 1).ToString() + " "
                                          + (lS[1] + 1).ToString() + "/" + (lS[1] + 1).ToString() + "/" + (lS[1] + 1).ToString() + " "
                                          + (lS[2] + 1).ToString() + "/" + (lS[2] + 1).ToString() + "/" + (lS[2] + 1).ToString());
                }
            }
            if (facesMethod == 1 || facesMethod == 4) //ULONG or (24bit as ULONG)
            {
                foreach (List<ulong> lL in idxListUlong)
                {
                    objSw.WriteLine("f  " + (lL[0] + 1).ToString() + "/" + (lL[0] + 1).ToString() + "/" + (lL[0] + 1).ToString() + " "
                                          + (lL[1] + 1).ToString() + "/" + (lL[1] + 1).ToString() + "/" + (lL[1] + 1).ToString() + " "
                                          + (lL[2] + 1).ToString() + "/" + (lL[2] + 1).ToString() + "/" + (lL[2] + 1).ToString());
                }
            }
            objSw.Close();
        }


        //generic no faces
        public void writeSpecifiedObjFile(
            List<List<float>> posList,
            string outputPath,
            ref FileInfo ffi,
            int vertexOrderIndex,
            string outputName,
            string meshName)
        {
            StreamWriter objSw = new StreamWriter(
                outputPath + "\\" + Path.GetFileNameWithoutExtension(outputName) + meshName + ".obj");
            foreach (List<float> lf in posList)
            {
                string x, y, z;
                if (lf[0].ToString().Contains("E") == true)
                {
                    x = lf[0].ToString("F9");
                }
                else
                {
                    x = lf[0].ToString();
                }
                if (lf[1].ToString().Contains("E"))
                {
                    y = lf[1].ToString("F9");
                }
                else
                {
                    y = lf[1].ToString();
                }
                if (lf[2].ToString().Contains("E"))
                {
                    z = lf[2].ToString("F9");
                }
                else
                {
                    z = lf[2].ToString();
                }
                if (vertexOrderIndex == 0)
                {
                    objSw.WriteLine("v " + x + " " + y + " " + z);
                }
                if (vertexOrderIndex == 1)
                {
                    objSw.WriteLine("v " + x + " " + z + " " + y);
                }
                if (vertexOrderIndex == 2)
                {
                    objSw.WriteLine("v " + y + " " + x + " " + z);
                }
                if (vertexOrderIndex == 3)
                {
                    objSw.WriteLine("v " + y + " " + z + " " + x);
                }
                if (vertexOrderIndex == 4)
                {
                    objSw.WriteLine("v " + z + " " + x + " " + y);
                }
                if (vertexOrderIndex == 5)
                {
                    objSw.WriteLine("v " + z + " " + y + " " + x);
                }
            }
            objSw.WriteLine("g " + ffi.Name);
            /*
            for (int i = 0; i < posList.Count / 3; i++)
            {
                objSw.WriteLine("f  " + (i + 1).ToString() + "/" + (i + 1).ToString() + "/" + (i + 1).ToString() + " "
                                      + (i + 1).ToString() + "/" + (i + 1).ToString() + "/" + (i + 1).ToString() + " "
                                      + (i + 1).ToString() + "/" + (i + 1).ToString() + "/" + (i + 1).ToString());
            }
            */
            objSw.Close();
        }
        
        //Extract MDL3 Standard and (HIGHLOD vertices only)
        public void extractMDL3_HIGHLOD(ref FileStream fs, ref FileInfo fi)
        {
            //string extractedPath = fi.DirectoryName + "\\extracted_mdl3";
            //Directory.CreateDirectory(extractedPath);

            byte[] infoBuffer1 = new byte[1];
            byte[] infoBuffer2 = new byte[2];
            byte[] infoBuffer4 = new byte[4];

            /*
            if (File.Exists(fi.DirectoryName + "\\body_s.bin"))
            {
                
            }
            else
            {
            */
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
                //FileStream zfs = new FileStream(fi.DirectoryName + "\\body_s.multiZlib", FileMode.Open, FileAccess.Read);
                FileStream zfs = new FileStream(fi.DirectoryName + "\\body_s", FileMode.Open, FileAccess.Read);
                FileStream dfs = new FileStream(fi.DirectoryName + "\\body_s.bin", FileMode.Create, FileAccess.Write);

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

                //Enumerate body_s.bin header and main table
                FileStream bfs = new FileStream(fi.DirectoryName + "\\body_s.bin", FileMode.Open, FileAccess.Read);
                FileInfo bfi = new FileInfo(fi.DirectoryName + "\\body_s.bin");

                //Enumerate body_s.bin header
                bfs.Read(infoBuffer4, 0, 4); uint sNull00 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
                bfs.Read(infoBuffer4, 0, 4); uint sAddress1 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); //unknown data address
                bfs.Read(infoBuffer4, 0, 4); uint sAddress2 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); //data near end of file
                bfs.Read(infoBuffer4, 0, 4); uint sAddress3 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); //unknown data address or size
                bfs.Read(infoBuffer4, 0, 4); uint sNull01 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
                bfs.Read(infoBuffer2, 0, 2); uint sTable1Count = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                bfs.Read(infoBuffer2, 0, 2); uint sCount2 = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0);
                bfs.Read(infoBuffer4, 0, 4); uint sCount3 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0);
                bfs.Read(infoBuffer4, 0, 4); uint sAddress4 = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); //pointer to another table

                List<List<uint>> sTable1 = new List<List<uint>>();
                for (int i = 0; i < sTable1Count; i++)
                {
                    List<uint> sTable1Enum = new List<uint>();
                    bfs.Read(infoBuffer4, 0, 4); uint dataId = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); sTable1Enum.Add(dataId);
                    bfs.Read(infoBuffer4, 0, 4); uint vAddress = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); sTable1Enum.Add(vAddress);
                    bfs.Read(infoBuffer4, 0, 4); uint fAddress = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); sTable1Enum.Add(fAddress);
                    bfs.Read(infoBuffer4, 0, 4); uint u1Address = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); sTable1Enum.Add(u1Address);
                    bfs.Read(infoBuffer4, 0, 4); uint u2Address = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); sTable1Enum.Add(u2Address);
                    bfs.Read(infoBuffer4, 0, 4); uint u3Address = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); sTable1Enum.Add(u3Address);
                    bfs.Read(infoBuffer4, 0, 4); uint u4Address = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); sTable1Enum.Add(u4Address);
                    bfs.Read(infoBuffer4, 0, 4); uint dataTypeFlag = BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0); sTable1Enum.Add(dataTypeFlag);
                    sTable1.Add(sTable1Enum);
                }

                //Parse and extract sTable1[i][7] //SO FAR, (known as the mesh data signaling flag when its 0x00000000, 0x00000001, and 0x00000002)
                for (int i = 0; i < sTable1Count; i++)
                {
                    if (sTable1[i][7] == 0) //standard lod
                    {
                        //Process known mesh data
                        int meshInfoTableIndex = (int)sTable1[i][0];
                        uint vCount = meshInfoTable[meshInfoTableIndex][5];
                        uint fCount = meshInfoTable[meshInfoTableIndex][8];
                        uint vAddress = sTable1[i][1];
                        uint fAddress = sTable1[i][2];
                        uint vStride = fvfTable[(int)meshInfoTable[(int)sTable1[i][0]][2]][7];

                        //Enumerate vertices
                        List<List<float>> verticesList = new List<List<float>>();
                        bfs.Seek(vAddress, SeekOrigin.Begin);
                        //MessageBox.Show(i.ToString() + " " + "vAddress" + " " + vAddress.ToString("X"));
                        for (int j = 0; j < vCount; j++)
                        {
                            List<float> vEnum = new List<float>();
                            if (vStride >= 12)
                            {
                                bfs.Read(infoBuffer4, 0, 4); vEnum.Add(-BitConverter.ToSingle(ArrayReverse(infoBuffer4), 0));
                                bfs.Read(infoBuffer4, 0, 4); vEnum.Add(BitConverter.ToSingle(ArrayReverse(infoBuffer4), 0));
                                bfs.Read(infoBuffer4, 0, 4); vEnum.Add(BitConverter.ToSingle(ArrayReverse(infoBuffer4), 0));
                                bfs.Seek((vStride - 12), SeekOrigin.Current);
                                verticesList.Add(vEnum);
                            }
                            else
                            {

                            }
                        }

                        //Enumerate Faces
                        List<List<ushort>> facesList = new List<List<ushort>>();
                        bfs.Seek(fAddress, SeekOrigin.Begin);
                        //MessageBox.Show(i.ToString() + " " + "fAddress" + " " + fAddress.ToString("X"));
                        for (int j = 0; j < (fCount / 3); j++)
                        {
                            List<ushort> fEnum = new List<ushort>();
                            if (vStride >= 12)
                            {
                                bfs.Read(infoBuffer2, 0, 2); fEnum.Add(BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0));
                                bfs.Read(infoBuffer2, 0, 2); fEnum.Add(BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0));
                                bfs.Read(infoBuffer2, 0, 2); fEnum.Add(BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0));
                                facesList.Add(fEnum);
                            }
                        }

                        //Write .obj file
                        if (vStride >= 12)
                        {
                            string outputPath = bfi.DirectoryName + "\\" + Path.GetFileNameWithoutExtension(bfi.Name) + "_extracted" + @"\standard\";
                            Directory.CreateDirectory(outputPath);
                            writeSpecifiedObjFile(verticesList, facesList, null, outputPath, ref bfi, 1, bfi.Name, "__" + readString(ref fs, fvfTable[(int)meshInfoTable[(int)sTable1[i][0]][2]][0x21]) + "_" + meshInfoTableIndex.ToString("X"), 0);
                        }
                    }
                    else //highest lod
                    {
                        //Read bounding box
                        bfs.Seek(sTable1[i][3], SeekOrigin.Begin);
                        List<List<float>> boundingBox = new List<List<float>>();
                        for (int j = 0; j < 8; j++)
                        {
                            List<float> bbEnum = new List<float>();
                            bfs.Read(infoBuffer4, 0, 4); bbEnum.Add(BitConverter.ToSingle(ArrayReverse(infoBuffer4), 0));
                            bfs.Read(infoBuffer4, 0, 4); bbEnum.Add(BitConverter.ToSingle(ArrayReverse(infoBuffer4), 0));
                            bfs.Read(infoBuffer4, 0, 4); bbEnum.Add(BitConverter.ToSingle(ArrayReverse(infoBuffer4), 0));
                            boundingBox.Add(bbEnum);
                        }

                        string outputPathbb = bfi.DirectoryName + "\\" + Path.GetFileNameWithoutExtension(bfi.Name) + "_extracted" + @"\boundingBox";
                        Directory.CreateDirectory(outputPathbb);
                        writeSpecifiedObjFile(boundingBox, outputPathbb, ref bfi, 1, bfi.Name, "_" + "boundingBox_" + sTable1[i][0].ToString("X"));

                        //Read info1
                        bfs.Seek(sTable1[i][4], SeekOrigin.Begin);
                        List<ushort> info1 = new List<ushort>();
                        bfs.Read(infoBuffer2, 0, 2); info1.Add(BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0));
                        bfs.Read(infoBuffer2, 0, 2); info1.Add(BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0));
                        bfs.Read(infoBuffer2, 0, 2); info1.Add(BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0));
                        bfs.Read(infoBuffer2, 0, 2); info1.Add(BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0));
                        bfs.Read(infoBuffer2, 0, 2); info1.Add(BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0));
                        bfs.Read(infoBuffer2, 0, 2); info1.Add(BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0));
                        bfs.Read(infoBuffer2, 0, 2); info1.Add(BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0));
                        bfs.Read(infoBuffer2, 0, 2); info1.Add(BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0));

                        //Read info for mesh data
                        bfs.Seek(sTable1[i][5], SeekOrigin.Begin);
                        List<List<uint>> meshDataInfo = new List<List<uint>>();
                        for (int j = 0; j < sTable1[i][7]; j++) //find this count if it exists
                        {
                            List<uint> mdiEnum = new List<uint>();
                            bfs.Read(infoBuffer4, 0, 4); mdiEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                            bfs.Read(infoBuffer4, 0, 4); mdiEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                            bfs.Read(infoBuffer4, 0, 4); mdiEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                            bfs.Read(infoBuffer4, 0, 4); mdiEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                            bfs.Read(infoBuffer4, 0, 4); mdiEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                            bfs.Read(infoBuffer4, 0, 4); mdiEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                            bfs.Read(infoBuffer4, 0, 4); mdiEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                            bfs.Read(infoBuffer4, 0, 4); mdiEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                            bfs.Read(infoBuffer4, 0, 4); mdiEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                            bfs.Read(infoBuffer4, 0, 4); mdiEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                            bfs.Read(infoBuffer4, 0, 4); mdiEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                            bfs.Read(infoBuffer4, 0, 4); mdiEnum.Add(BitConverter.ToUInt32(ArrayReverse(infoBuffer4), 0));
                            meshDataInfo.Add(mdiEnum);
                        }

                        //Process HIGHLOD vertices
                        for (int j = 0; j < sTable1[i][7]; j++)
                        {
                            bfs.Seek(meshDataInfo[j][0], SeekOrigin.Begin);
                            int test = 0;
                            int test2 = 0;

                            //Enumerate ushort vertices as tris
                            List<List<float>> vList0 = new List<List<float>>();
                            int objunknown = 0;
                            /*
                            for (int testcount = 0; testcount < meshDataInfo[j].Count(); testcount++)
                            {
                                if (i == 5)
                                MessageBox.Show(i.ToString() + " " + "meshDataInfo" + " " + meshDataInfo[j][testcount].ToString());
                            }
                            */
                            for (int k = 0; k < meshDataInfo[j][9]; k++)
                            {
                                List<float> vEnum = new List<float>();
                                float vxF = 0; float vyF = 0; float vzF = 0;
                                bfs.Read(infoBuffer2, 0, 2); vxF = -BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0); vxF /= 0xFFFF; vEnum.Add(vxF);
                                bfs.Read(infoBuffer2, 0, 2); vyF = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0); vyF /= 0xFFFF; vEnum.Add(vyF);
                                bfs.Read(infoBuffer2, 0, 2); vzF = BitConverter.ToUInt16(ArrayReverse(infoBuffer2), 0); vzF /= 0xFFFF; vEnum.Add(vzF);
                                if (vxF == 0 & vyF == 0 & vzF == 0)
                                {
                                    objunknown += 1;
                                    if (objunknown >= 10)
                                        break;
                                }
                                else
                                {
                                    vList0.Add(vEnum);
                                    test += 6;
                                    test2 += 1;
                                }
                            }
                            if (objunknown < 10)
                            {
                                //MessageBox.Show(test2.ToString());
                                //MessageBox.Show(i.ToString() + " " + "meshDataInfo1" + " " + meshDataInfo[j][0].ToString("X"));
                                //MessageBox.Show(i.ToString() + " " + "meshDataInfo2" + " " + (meshDataInfo[j][0] + test).ToString("X"));
                                string outputPath = bfi.DirectoryName + "\\" + Path.GetFileNameWithoutExtension(bfi.Name) + "_extracted";
                                Directory.CreateDirectory(outputPath);
                                //MessageBox.Show(i.ToString());
                                writeSpecifiedObjFile(vList0, outputPath, ref bfi, 1, bfi.Name, "_" + i.ToString() + "_" + sTable1[i][0].ToString("X"));
                            }
                        }

                    }
                }

                //USE FOR TEXTURE DATA MAYBE
                //bfs.Seek(sAddress4, SeekOrigin.Begin); //Texture Data

                //bp++;

                bfs.Close();
            }
        }
        //Read a zero terminated string at specified address
        //指定されたアドレスのゼロ終端文字列を読み取ります
        public string readString(ref FileStream fs, uint specifiedAddress)
        {
            byte[] infoBuffer1 = new byte[1];

            string stringToBuild = String.Empty;
            bool endReached = false;
            fs.Seek(specifiedAddress, SeekOrigin.Begin);
            fs.Read(infoBuffer1, 0, 1);
            stringToBuild += Encoding.ASCII.GetString(infoBuffer1);
            while (endReached == false)
            {
                fs.Read(infoBuffer1, 0, 1);
                if (infoBuffer1[0] == 0)
                {
                    endReached = true;
                }
                else
                {
                    stringToBuild += Encoding.ASCII.GetString(infoBuffer1);
                }
            }
            return stringToBuild;
        }
    }
}
