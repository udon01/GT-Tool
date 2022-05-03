using System;
using System.IO;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void File_open_Click(object sender, EventArgs e)
        {
            //「ファイル名」で表示される文字列を指定する
            ofd.FileName = ".obj";
            //表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            ofd.InitialDirectory = "";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            ofd.Filter = ".obj(*.obj;)|*.obj;";
            //[ファイルの種類]ではじめに選択されるものを指定する
            ofd.FilterIndex = 1;
            //タイトルを設定する
            ofd.Title = "開くobjファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            
            if (ofd.ShowDialog() == DialogResult.OK)
                OBJpath.Text = ofd.FileName;
        }

        private void Convert_Click(object sender, EventArgs e)
        {
            StreamReader read = new StreamReader(OBJpath.Text);
            controlfilesave = OBJpath.Text;
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
                    line1 = line.Replace(".001", "").Replace(".002", "").Replace(".003", "").Replace(".004", "")
                        .Replace(".005", "").Replace(".006", "").Replace(".007", "").Replace(".008", "")
                        .Replace(".009", "").Replace(".010", "").Replace("_NONE", "");
                    flags = true;
                    goto label1;
                }

                if (line.Contains("o ") == true)
                {
                    line2 = line.Replace("o ", "g ");
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
            File.Delete(OBJpath.Text);
            File.Copy(OBJpath.Text + ".txt", OBJpath.Text);
            File.Delete(OBJpath.Text + ".txt");
            i = 0;
            MessageBox.Show("終了しました");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
