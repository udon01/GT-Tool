using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GT2用obj編集ツール
{
    public partial class Form1 : Form
    {
        public string controlfile;
        public string line;
        public string line1;
        public string line2;
        public int i = 0;
        public string linef;
        public bool flags = false;
        public bool otog = false;
        public bool saveline = false;
        public string controlfilesave;
        public OpenFileDialog ofd = new OpenFileDialog();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            string[] path = Environment.GetCommandLineArgs();

            for (int j = 0; j < path.Count(); j++)
            {
                string fileextension = Path.GetExtension(path[j].ToString());
                // obj以外であればイベントハンドラを抜ける
                if (fileextension != ".obj" && fileextension != ".OBJ")
                    goto labelfinish;

                // ドラッグ＆ドロップされたファイル
                StreamReader read = new StreamReader(path[j]);
                controlfilesave = path[j];
                controlfilesave = controlfilesave + ".txt";
                StreamWriter save = new StreamWriter(controlfilesave);
                save.NewLine = "\n";
                while (read.Peek() > -1)
                {
                    i = i + 1;
                    flags = false;
                    otog = false;
                    saveline = false;
                    line = read.ReadLine();

                    if (line.Contains("flags") == true)
                    {
                        line1 = line.Replace(".000", "").Replace(".001", "").Replace(".002", "").Replace(".003", "")
                            .Replace(".004", "").Replace(".005", "").Replace(".006", "").Replace(".007", "")
                            .Replace(".008", "").Replace(".009", "").Replace(".010", "");
                        if (line1.IndexOf("_") >= 0)
                            line1 = line1.Remove(line1.IndexOf("_"));
                        flags = true;
                        goto label1;
                    }

                    if (line.Contains("o ") == true)
                    {
                        line2 = line.Replace("o ", "g ");
                        if (line2.IndexOf("_") >= 0)
                            line2 = line2.Remove(line2.IndexOf("_"));
                        otog = true;
                        goto label1;
                    }

                label1:;
                    if (line.Contains("#") == false && line.Contains("s ") == false)
                    {
                        if (flags == true)
                        {
                            save.WriteLine(line1);
                            saveline = true;
                        }
                        if (otog == true)
                        {
                            save.WriteLine(line2);
                            saveline = true;
                        }
                        if (saveline == false)
                            save.WriteLine(line);
                    }
                    else if (line.Contains("#") == true)
                    {
                        i = i - 1;
                    }
                    if (i == 3)
                    {
                        linef = "f 1 1 1";
                        save.WriteLine(linef);
                    }
                    if (i == 5)
                    {
                        linef = "f 2 2 2";
                        save.WriteLine(linef);
                    }
                    if (i == 7)
                    {
                        linef = "f 3 3 3";
                        save.WriteLine(linef);
                    }
                    if (i == 9)
                    {
                        linef = "f 4 4 4";
                        save.WriteLine(linef);
                    }
                }

                read.Close();
                save.Close();
                File.Delete(path[j]);
                File.Copy(path[j] + ".txt", path[j]);
                File.Delete(path[j] + ".txt");
                i = 0;

                labelfinish:;
                Close();
            }
        }
    }
}
