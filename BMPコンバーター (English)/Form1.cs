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

namespace BMPコンバーター
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Openfolderbutton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdconvert = new FolderBrowserDialog();

            //上部に表示する説明テキストを指定する
            fbdconvert.Description = "フォルダを指定してください";
            //ルートフォルダを指定する
            //デフォルトでDesktop
            fbdconvert.RootFolder = Environment.SpecialFolder.Desktop;
            //最初に選択するフォルダを指定する
            //RootFolder以下にあるフォルダである必要がある
            fbdconvert.SelectedPath = @"C:\";

            //ダイアログを表示する
            if (fbdconvert.ShowDialog() == DialogResult.OK)
                Folderpathtext.Text = fbdconvert.SelectedPath;
        }

        private void Extractbutton_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog fbdextract = new FolderBrowserDialog();

            //上部に表示する説明テキストを指定する
            fbdextract.Description = "フォルダを指定してください";
            //ルートフォルダを指定する
            //デフォルトでDesktop
            fbdextract.RootFolder = Environment.SpecialFolder.Desktop;
            //最初に選択するフォルダを指定する
            //RootFolder以下にあるフォルダである必要がある
            fbdextract.SelectedPath = @"C:\";

            //ダイアログを表示する
            if (fbdextract.ShowDialog() == DialogResult.OK)
                Extractpathtext.Text = fbdextract.SelectedPath;
        }

        private void Convertbutton_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = null;
            FileInfo[] files = null;
            try
            {
                di = new DirectoryInfo(Folderpathtext.Text);
                files = di.GetFiles("*.bmp", SearchOption.AllDirectories);
            }
            catch(ArgumentException)
            {
                MessageBox.Show("コンバートするBMPの入ったフォルダが指定されていません");
            }

            if (files == null)
                goto labelfinish;

            string Extractpath = "";
            if (Extractpathtext.Text == "")
            {
                string user = Environment.UserName;
                Extractpath = @"C:\Users\" + user + @"\Desktop\BMP\";

                if (!Directory.Exists(Extractpath))
                    Directory.CreateDirectory(Extractpath);
            }

            byte[] binary = new byte[] { 0x42, 0x4D, 0x76, 0x70, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x76, 0x00, 0x00, 0x00,
                    0x28, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0xE0, 0x00, 0x00, 0x00, 0x01, 0x00, 0x04, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x70, 0x00, 0x00, 0xF0, 0x0A, 0x00, 0x00, 0xF0, 0x0A, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00,
                    0x10, 0x00, 0x00, 0x00 };

            for (int i = 0; i < files.Count(); i++)
            {
                FileStream fsr = new FileStream(Folderpathtext.Text + @"\" + files.ElementAt(i).ToString(), FileMode.Open, FileAccess.Read);
                byte[] fsrbyte = new byte[fsr.Length];
                fsr.Read(fsrbyte, 0, (int)fsr.Length);
                byte[] colortortalbyte = new byte[1];
                fsr.Seek(46, SeekOrigin.Begin);
                fsr.Read(colortortalbyte, 0, 1);
                string colortortalstring = BitConverter.ToString(colortortalbyte, 0);
                if (colortortalstring.Length == 1)
                    colortortalstring = "0" + colortortalstring;
                int colortortal = Convert.ToInt32(colortortalstring, 16);
                byte[] colorbyte = new byte[colortortal * 4];
                int seek = 138;
                fsr.Seek(seek, SeekOrigin.Begin);
                fsr.Read(colorbyte, 0, colortortal * 4);
                seek = seek + colortortal * 4;

                byte[] nocolor = new byte[] { 0xFF, 0xFF, 0xFF, 0x00 };
                int nocolortortal = 16 - colortortal;

                FileStream fsw = File.Create(Extractpath + files.ElementAt(i).ToString());
                int seek_w = 54;
                fsw.Write(binary, 0, 54);
                fsw.Seek(54, SeekOrigin.Begin);
                fsw.Write(colorbyte, 0, colorbyte.Length);
                seek_w = seek_w + colorbyte.Length;

                for (int j = 0; j < nocolortortal; j++)
                {
                    fsw.Write(nocolor, 0, 4);
                    seek_w = seek_w + 4;
                }
                while(true)
                {
                    fsw.Write(fsrbyte, seek, 1);
                    seek = seek + 1;

                    if (seek == fsrbyte.Length)
                        break;
                }
                fsw.Close();
                //MessageBox.Show(colortortalstring);
            }

        labelfinish:;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
