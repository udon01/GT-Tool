using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IDをリネームする
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
            DirectoryInfo olddi = null;
            try
            {
                olddi = new DirectoryInfo(Folderpathtext.Text);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("コンバートするcsvの入ったフォルダが指定されていません");
                goto labelfinish;
            }

            string Extractpath = "";
            if (Extractpathtext.Text == "")
            {
                string user = Environment.UserName;
                Extractpath = @"C:\Users\" + user + @"\Desktop\ConvertID\";

                if (!Directory.Exists(Extractpath))
                    Directory.CreateDirectory(Extractpath);
            }

            if (IsOnlyAlphanumeric2(OldID.Text) == true && OldID.Text.Length == 5)
            {

            }

            else
            {
                MessageBox.Show("元のIDは5文字かつ半角英数字にしてください");
                goto labelfinish;
            }

            if (IsOnlyAlphanumeric2(NewID.Text) == true && NewID.Text.Length == 5)
            {

            }

            else
            {
                MessageBox.Show("変換するIDは5文字かつ半角英数字にしてください");
                goto labelfinish;
            }

            //ここから変換処理
            string csvpath = Folderpathtext.Text + @"\";
            DirectoryInfo zerodi = null;
            string DataSplitter = "";
            string olddistr = "";

            for (int j = 0; j < 19; j++)
            {
                if (j == 0)
                    DataSplitter = "Brake";
                else if (j == 1)
                    DataSplitter = "BrakeController";
                else if (j == 2)
                    DataSplitter = "Clutch";
                else if (j == 3)
                    DataSplitter = "Computer";
                else if (j == 4)
                    DataSplitter = "Displacement";
                else if (j == 5)
                    DataSplitter = "EngineBalance";
                else if (j == 6)
                    DataSplitter = "Flywheel";
                else if (j == 7)
                    DataSplitter = "Gear";
                else if (j == 8)
                    DataSplitter = "Intercooler";
                else if (j == 9)
                    DataSplitter = "Lightweight";
                else if (j == 10)
                    DataSplitter = "LSD";
                else if (j == 11)
                    DataSplitter = "Muffler";
                else if (j == 12)
                    DataSplitter = "NATune";
                else if (j == 13)
                    DataSplitter = "PortPolish";
                else if (j == 14) 
                    DataSplitter = "PropellerShaft";
                else if (j == 15) 
                    DataSplitter = "Suspension";
                else if (j == 16)
                    DataSplitter = "TiresFront";
                else if (j == 17)
                    DataSplitter = "TiresRear";
                else if (j == 18)
                    DataSplitter = "TurbineKit";
                
                olddi = new DirectoryInfo(csvpath + DataSplitter + @"\" + OldID.Text);
                olddistr = csvpath + DataSplitter + @"\" + OldID.Text;

                if (Directory.Exists(olddistr) == true)
                {
                    FileInfo[] files = olddi.GetFiles("*.csv", SearchOption.AllDirectories);
                    DirectoryInfo newdi = Directory.CreateDirectory(Extractpath + DataSplitter + @"\" + NewID.Text);
                    for (int i = 0; i < files.Count(); i++)
                    {
                        string csvtotxt = files[i].ToString();
                        csvtotxt = csvtotxt.Replace(".csv", "");
                        csvtotxt = csvtotxt + ".txt";
                        StreamReader sr = new StreamReader(csvpath + DataSplitter + @"\" + OldID.Text + @"\" + files[i].ToString(), Encoding.GetEncoding("UTF-8"));
                        string text1 = sr.ReadLine();
                        string text2 = sr.ReadLine();
                        sr.Close();
                        StreamWriter sw = new StreamWriter(Extractpath + DataSplitter + @"\" + NewID.Text + @"\" + csvtotxt, false, Encoding.GetEncoding("UTF-8"));
                        sw.WriteLine(text1);
                        text2 = text2.Replace(OldID.Text, NewID.Text);
                        sw.Write(text2);
                        sw.Close();
                        string csv = Path.ChangeExtension(Extractpath + DataSplitter + @"\" + NewID.Text + @"\" + csvtotxt, "csv");
                        try
                        {
                            File.Move(Extractpath + DataSplitter + @"\" + NewID.Text + @"\" + csvtotxt, csv);
                        }
                        catch (IOException)
                        {
                            File.Delete(Extractpath + DataSplitter + @"\" + NewID.Text + @"\" + csvtotxt);
                        }
                    }
                }

                else if (Directory.Exists(olddistr) == false)
                {
                    zerodi = new DirectoryInfo(csvpath + DataSplitter + @"\" + OldID.Text);
                    string zerodistr = csvpath + DataSplitter + @"\" + OldID.Text;

                    if (Directory.Exists(zerodistr) == true)
                    {
                        FileInfo[] files = zerodi.GetFiles("*.csv", SearchOption.AllDirectories);
                        DirectoryInfo newdi = Directory.CreateDirectory(Extractpath + DataSplitter + @"\" + "00000");
                        File.Move(csvpath + DataSplitter + @"\" + "00000" + @"\" + files[0].ToString(), Extractpath + DataSplitter + @"\" + "00000" + @"\" + files[0].ToString());
                    }
                }
            }

            for (int j = 0; j < 4; j++)
            {
                if (j == 0)
                    DataSplitter = "Car";
                else if (j == 1)
                    DataSplitter = "Chassis";
                else if (j == 2)
                    DataSplitter = "Drivetrain";
                else if (j == 3)
                    DataSplitter = "Engine";
                
                olddi = new DirectoryInfo(csvpath + DataSplitter);
                olddistr = csvpath + DataSplitter;

                if (Directory.Exists(olddistr) == true)
                {
                    FileInfo[] files = olddi.GetFiles("*.csv", SearchOption.AllDirectories);
                    DirectoryInfo di = Directory.CreateDirectory(Extractpath + DataSplitter);
                    for (int i = 0; i < files.Count(); i++)
                    {
                        string csvtotxt = NewID.Text + ".txt";
                        StreamReader sr = new StreamReader(csvpath + DataSplitter + @"\" + files[i].ToString(), Encoding.GetEncoding("UTF-8"));
                        string text1 = sr.ReadLine();
                        string text2 = sr.ReadLine();
                        sr.Close();
                        StreamWriter sw = new StreamWriter(Extractpath + DataSplitter + @"\" + csvtotxt, false, Encoding.GetEncoding("UTF-8"));
                        sw.WriteLine(text1);
                        text2 = text2.Replace(OldID.Text, NewID.Text);
                        sw.Write(text2);
                        sw.Close();
                        string csv = Path.ChangeExtension(Extractpath + DataSplitter + @"\" + csvtotxt, "csv");
                        try
                        {
                            File.Move(Extractpath + DataSplitter + @"\" + csvtotxt, csv);
                        }
                        catch (IOException)
                        {
                            File.Delete(Extractpath + DataSplitter + @"\" + csvtotxt);
                        }
                    }
                }
            }

            DataSplitter = "RacingModify";
            olddi = new DirectoryInfo(csvpath + DataSplitter + @"\" + OldID.Text);
            olddistr = csvpath + DataSplitter + @"\" + OldID.Text;

            if (Directory.Exists(olddistr) == true)
            {
                FileInfo[] files = olddi.GetFiles("*.csv", SearchOption.AllDirectories);
                DirectoryInfo newdi = Directory.CreateDirectory(Extractpath + DataSplitter + @"\" + NewID.Text);
                for (int i = 0; i < files.Count(); i++)
                {
                    string csvtotxt = files[i].ToString();
                    csvtotxt = csvtotxt.Replace(".csv", "");
                    csvtotxt = csvtotxt + ".txt";
                    string filename = files[i].ToString();
                    filename = filename.Replace(OldID.Text, NewID.Text);

                    StreamReader sr = new StreamReader(csvpath + DataSplitter + @"\" + OldID.Text + @"\" + files[i].ToString(), Encoding.GetEncoding("UTF-8"));
                    string text1 = sr.ReadLine();
                    string text2 = sr.ReadLine();
                    sr.Close();
                    StreamWriter sw = new StreamWriter(Extractpath + DataSplitter + @"\" + NewID.Text + @"\" + csvtotxt, false, Encoding.GetEncoding("UTF-8"));
                    sw.WriteLine(text1);
                    text2 = text2.Replace(OldID.Text, NewID.Text);
                    sw.Write(text2);
                    sw.Close();
                    string csv = Path.ChangeExtension(Extractpath + DataSplitter + @"\" + NewID.Text + @"\" + csvtotxt, "csv");
                    try
                    {
                    File.Move(Extractpath + DataSplitter + @"\" + NewID.Text + @"\" + csvtotxt, csv);
                    }
                    catch (IOException)
                    {
                        File.Delete(Extractpath + DataSplitter + @"\" + NewID.Text + @"\" + csvtotxt);
                    }
                    try
                    {
                    File.Move(csv, Extractpath + DataSplitter + @"\" + NewID.Text + @"\" + filename);
                    }
                    catch (IOException)
                    {
                        File.Delete(csv);
                    }
                }
            }

        labelfinish:;
        }
        
        /// <summary>
        /// 引数の文字列が半角英数字のみで構成されているかを調べる。
        /// </summary>
        /// <param name="text">チェック対象の文字列。</param>
        /// <returns>引数が英数字のみで構成されていればtrue、そうでなければfalseを返す。</returns>
        public static bool IsOnlyAlphanumeric2(string text)
        {
            // 文字列の先頭から末尾までが、英数字のみとマッチするかを調べる。
            return Regex.IsMatch(text, @"^[0-9a-zA-Z]+$");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
