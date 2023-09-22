using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;

//単純減色と誤差拡散とディザリング - 午後わてんのブログ
//https://gogowaten.hatenablog.com/entry/15394008
namespace _20180302_単純減色と誤差拡散ディザ
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        BitmapSource OriginBitmap;
        WriteableBitmap wb;
        string ImageFileFullPath;

        public MainWindow()
        {
            InitializeComponent();
            this.Title = this.ToString();
            this.AllowDrop = true;
            this.Drop += MainWindow_Drop;
            ButtonConvert.Click += ButtonConvert_Click;
            ButtonConvertErrorToRight.Click += ButtonConvertErrorToRight_Click;
            ButtonConvertFloydSteinberg.Click += ButtonConvertFloydSteinberg_Click;
            ButtonOrigin.Click += ButtonOrigin_Click;
            NumericScrollBar.ValueChanged += NumericScrollBar_ValueChanged;
            NumericScrollBar.MouseWheel += NumericScrollBar_MouseWheel;
            NumericTextBox.MouseWheel += NumericTextBox_MouseWheel;
            NumericTextBox.GotFocus += NumericTextBox_GotFocus;
            NumericTextBox.TextChanged += NumericTextBox_TextChanged;
            //float f1 = -1.8f;
            //float f2 = -1.5f;
            //float f3 = -1.2f;
            //float f4 = -0.8f;
            //float f5 = -0.5f;
            //float f6 = -0.2f;
            //float gosa = 0f;
            //byte c = 0;
            //int rate = 0;
            //float freqcency = 255f / 2f;
            //for (int i = 0; i < 256; ++i)
            //{
            //    var neko = i / freqcency;
            //    rate = (int)Math.Floor(i / freqcency);
            //    c = (byte)(freqcency * rate);
            //    gosa = i - c;
            //}
        }

        private void ButtonConvertFloydSteinberg_Click(object sender, RoutedEventArgs e)
        {
            if (OriginBitmap == null) { return; }
            MyImage.Source = FloydSteinberg(OriginBitmap, (int)NumericScrollBar.Value);
        }

        /// <summary>
        /// 単純減色＋右隣への誤差拡散
        /// 対象画像はPixelFormatがPbgra32のBitmapSource
        /// </summary>
        /// <param name="source">Pbgra32のBitmapSource</param>
        /// <param name="division">階調数、2から256を指定</param>
        private BitmapSource ErrorDiffusionToRight(BitmapSource source, int division)
        {
            //int division = (int)NumericScrollBar.Value;
            float frequencyThreshold = 255f / division;//1階調分の閾値
            //4階調のとき、0, 63.75, 127.5, 191.25, 255
            float frequencyValue = 255f / (division - 1f);//1階調分の値
            //4階調の時、0, 85, 170, 255

            wb = new WriteableBitmap(source);
            int h = wb.PixelHeight;
            int w = wb.PixelWidth;
            int stride = wb.BackBufferStride;
            var pixels = new byte[h * stride];
            wb.CopyPixels(pixels, stride, 0);//byte配列にCopyPixel
            //float配列作成してコピー、誤差拡散計算用
            float[] iPixels = new float[pixels.Length];
            for (int i = 0; i < iPixels.Length; ++i)
            {
                iPixels[i] = pixels[i];
            }

            long p = 0;//対象ピクセルの配列の中での位置
            int newValue;
            float gosa = 0, oldValue = 0, rate = 0;

            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    p = y * stride + (x * 4);
                    for (int i = 0; i < 3; ++i)
                    {
                        oldValue = iPixels[p + i];//判定する値
                        //0以下なら0に変換、誤差はマイナス分そのものになる
                        if (oldValue <= 0)//未満じゃなくて以下が都合がいい
                        {
                            gosa = oldValue;
                            newValue = 0;
                        }
                        //255以上なら255に変換、超えた分が誤差になる
                        else if (oldValue >= 255)
                        {
                            gosa = oldValue - 255f;
                            newValue = 255;
                        }
                        //0より大きくて255未満の時
                        else
                        {
                            //何番目の閾値かを求める、値を閾値で割った整数部分、これが1階調分にかける倍率になる
                            rate = (int)Math.Floor(oldValue / frequencyThreshold);//Floorで切り捨てて整数部分取得
                            //閾値は未満と以上で分けたいので閾値ピッタリのときは倍率を一個下げるため
                            //割り切れたときは倍率を一個下げる
                            if (oldValue % frequencyThreshold == 0) { rate--; }
                            //変換後の値、1階調分に倍率をかけた値
                            newValue = (int)(frequencyValue * rate);
                            gosa = oldValue - newValue;
                        }
                        iPixels[p + i] = newValue;//変換
                        if (p + i + 4 < iPixels.Length)
                        {
                            iPixels[p + i + 4] += gosa;//誤差拡散
                        }
                    }
                }
            }
            for (int i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = (byte)iPixels[i];
            }
            wb.WritePixels(new Int32Rect(0, 0, w, h), pixels, stride, 0);
            return wb;
        }

        /// <summary>
        /// 単純減色＋フロイドスタインバーグ式誤差拡散
        /// </summary>
        /// <param name="source">Pbgra32のBitmapSource</param>
        /// <param name="division">階調数、2から256を指定</param>
        /// <returns>Pbgra32のBitmapSource</returns>
        private BitmapSource FloydSteinberg(BitmapSource source, int division)
        {
            //int division = (int)NumericScrollBar.Value;
            float frequencyThreshold = 255f / division;//1階調分の閾値
            //4階調のとき、0, 63.75, 127.5, 191.25, 255
            float frequencyValue = 255f / (division - 1f);//1階調分の値
            //4階調の時、0, 85, 170, 255

            wb = new WriteableBitmap(source);
            int h = wb.PixelHeight;
            int w = wb.PixelWidth;
            int stride = wb.BackBufferStride;
            var pixels = new byte[h * stride];
            wb.CopyPixels(pixels, stride, 0);//byte配列にCopyPixel
            //int配列作成してコピー
            float[] iPixels = new float[pixels.Length];
            for (int i = 0; i < iPixels.Length; ++i)
            {
                iPixels[i] = pixels[i];
            }

            long p = 0;
            float gosa = 0, oldValue = 0, newValue = 0, rate = 0;

            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    p = y * stride + (x * 4);
                    for (int i = 0; i < 3; ++i)
                    {
                        oldValue = iPixels[p + i];//判定する値
                        //0以下なら0に変換、誤差はマイナス分そのものになる
                        if (oldValue <= 0)//未満じゃなくて以下が都合がいい
                        {
                            gosa = oldValue;
                            newValue = 0;
                        }
                        //255以上なら255に変換、超えた分が誤差になる
                        else if (oldValue >= 255)
                        {
                            gosa = oldValue - 255f;
                            newValue = 255;
                        }
                        //0より大きくて255未満の時
                        else
                        {
                            //何番目の閾値かを求める、値を閾値で割った整数部分、これが1階調分にかける倍率になる
                            rate = (int)Math.Floor(oldValue / frequencyThreshold);//Floorで切り捨てて整数部分取得
                            //閾値は未満と以上で分けたいので閾値ピッタリのときは倍率を一個下げるため
                            //割り切れたときは倍率を一個下げる
                            if (oldValue % frequencyThreshold == 0) { rate--; }
                            //変換後の値、1階調分に倍率をかけた値
                            newValue = (frequencyValue * rate);
                            gosa = oldValue - newValue;
                        }

                        iPixels[p + i] = newValue;//変換

                        //誤差拡散
                        if (p + i + 4 < iPixels.Length)
                        {
                            iPixels[p + i + 4] += (gosa / 16f) * 7f;//右隣
                        }

                        if (y < h - 1)//1行下
                        {
                            if (x != 0)
                            {
                                iPixels[p + i + stride - 4] += (gosa / 16f) * 3f;//左下
                            }
                            iPixels[p + i + stride] += (gosa / 16f) * 5f;//真下
                            if (x < w - 1)
                            {
                                iPixels[p + i + stride + 4] += (gosa / 16f) * 1f;//右下
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < pixels.Length; ++i)
            {
                //pixels[i] = (byte)iPixels[i];//多分切り捨てになる
                pixels[i] = (byte)Math.Round(iPixels[i], MidpointRounding.AwayFromZero);//四捨五入
            }
            wb.WritePixels(new Int32Rect(0, 0, w, h), pixels, stride, 0);
            return wb;
        }



        /// <summary>
        /// 単純減色の変換対応表作成
        /// </summary>
        /// <param name="division">分割数(階調数)</param>
        /// <returns></returns>
        private byte[] GetConverterArray(int division)
        {
            //範囲            
            float frequency = 256f / division;//1範囲の大きさ、周波数
            float[] range = new float[division + 1];//3分割なら0，85，170，255になる
            for (int i = 0; i < range.Length; ++i)
            {
                range[i] = i * frequency;
            }

            //指定する値
            frequency = 255f / (division - 1);
            byte[] color = new byte[division];//3分割なら0，127，255になる
            for (int i = 0; i < color.Length; ++i)
            {
                color[i] = (byte)(i * frequency);
            }

            //元の256階調全てに対する変換結果の配列作成、対応表
            byte[] converter = new byte[256];
            int j = 0;
            for (int i = 0; i < 256; ++i)
            {
                if (i >= range[j + 1])
                {
                    j++;
                }
                converter[i] = color[j];
            }
            return converter;
        }
        //        ポスタリゼーション（階調変更）
        //http://www.sm.rim.or.jp/~shishido/post.html
        //対応表を作成しておいて、それに当てはめて判定、速い
        private void GensyokuNumeric2Table()
        {
            int division = (int)NumericScrollBar.Value;//分割数
            //変換対応表取得
            byte[] converter = GetConverterArray(division);

            wb = new WriteableBitmap(OriginBitmap);
            int h = wb.PixelHeight;
            int w = wb.PixelWidth;
            int stride = wb.BackBufferStride;
            byte[] pixles = new byte[h * stride];
            wb.CopyPixels(pixles, stride, 0);
            long p = 0;

            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    p = y * stride + (x * 4);
                    //対応表に当てはめて色変換
                    pixles[p + 2] = converter[pixles[p + 2]];
                    pixles[p + 1] = converter[pixles[p + 1]];
                    pixles[p + 0] = converter[pixles[p + 0]];
                }
            }
            wb.WritePixels(new Int32Rect(0, 0, w, h), pixles, stride, 0);
            MyImage.Source = wb;
            TextBlockColorCount.Text = Math.Pow(division, 3).ToString();
        }









        #region イベント


        private void ButtonConvertErrorToRight_Click(object sender, RoutedEventArgs e)
        {
            if (OriginBitmap == null) { return; }
            MyImage.Source = ErrorDiffusionToRight(OriginBitmap, (int)NumericScrollBar.Value);
        }

        private void NumericTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            this.Dispatcher.InvokeAsync(() => { Task.Delay(10); box.SelectAll(); });
        }

        private void NumericScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TextBlockColorCount.Text = Math.Pow(NumericScrollBar.Value, 3).ToString();
        }

        private void ButtonConvert_Click(object sender, RoutedEventArgs e)
        {
            if (OriginBitmap == null) { return; }
            GensyokuNumeric2Table();
        }

        private void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            double d;
            if (!double.TryParse(textBox.Text, out d))
            {
                textBox.Text = System.Text.RegularExpressions.Regex.Replace(textBox.Text, "[^0-9]", "");
            }
        }

        private void NumericTextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //if (e.Delta > 0) { NumericScrollBar.Value++; }
            //else { NumericScrollBar.Value--; }
            NumericScrollBar.Value = (e.Delta > 0) ? NumericScrollBar.Value + 1 : NumericScrollBar.Value - 1;
        }

        private void NumericScrollBar_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) { NumericScrollBar.Value++; }
            else { NumericScrollBar.Value--; }
        }

        private void ButtonOrigin_Click(object sender, RoutedEventArgs e)
        {
            if (OriginBitmap == null) { return; }
            MyImage.Source = OriginBitmap;
        }


        //画像ファイルドロップ時
        //PixelFormat.Pbgr32に変換してBitmapSource取得
        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) == false) { return; }
            string[] filePath = (string[])e.Data.GetData(DataFormats.FileDrop);
            OriginBitmap = GetBitmapSourceWithChangePixelFormat2(filePath[0], PixelFormats.Pbgra32, 96, 96);

            if (OriginBitmap == null)
            {
                MessageBox.Show("画像ではありません");
            }
            else
            {
                MyImage.Source = OriginBitmap;
                ImageFileFullPath = filePath[0];
            }
        }
        #endregion

        /// <summary>
        ///  ファイルパスとPixelFormatを指定してBitmapSourceを取得、dpiの変更は任意
        /// </summary>
        /// <param name="filePath">画像ファイルのフルパス</param>
        /// <param name="pixelFormat">PixelFormatsの中からどれかを指定</param>
        /// <param name="dpiX">無指定なら画像ファイルで指定されているdpiになる</param>
        /// <param name="dpiY">無指定なら画像ファイルで指定されているdpiになる</param>
        /// <returns></returns>
        private BitmapSource GetBitmapSourceWithChangePixelFormat2(
            string filePath, PixelFormat pixelFormat, double dpiX = 0, double dpiY = 0)
        {
            BitmapSource source = null;
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var bf = BitmapFrame.Create(fs);
                    var convertedBitmap = new FormatConvertedBitmap(bf, pixelFormat, null, 0);
                    int w = convertedBitmap.PixelWidth;
                    int h = convertedBitmap.PixelHeight;
                    int stride = (w * pixelFormat.BitsPerPixel + 7) / 8;
                    byte[] pixels = new byte[h * stride];
                    convertedBitmap.CopyPixels(pixels, stride, 0);
                    //dpi指定がなければ元の画像と同じdpiにする
                    if (dpiX == 0) { dpiX = bf.DpiX; }
                    if (dpiY == 0) { dpiY = bf.DpiY; }
                    //dpiを指定してBitmapSource作成
                    source = BitmapSource.Create(
                        w, h, dpiX, dpiY,
                        convertedBitmap.Format,
                        convertedBitmap.Palette, pixels, stride);
                };
            }
            catch (Exception)
            {

            }
            return source;
        }

        private void Buttonload_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = "";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            ofd.InitialDirectory = "";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            ofd.Filter = "画像ファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            //2番目の「すべてのファイル」が選択されているようにする
            ofd.FilterIndex = 1;
            //タイトルを設定する
            ofd.Title = "開くファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;

            //ダイアログを表示する
            if (ofd.ShowDialog() == true)
            {
                OriginBitmap = GetBitmapSourceWithChangePixelFormat2(ofd.FileName, PixelFormats.Pbgra32, 96, 96);

                if (OriginBitmap == null)
                {
                    MessageBox.Show("画像ではありません");
                }
                else
                {
                    MyImage.Source = OriginBitmap;
                    ImageFileFullPath = ofd.FileName;
                }
            }
        }

        private void Buttonsave_Click(object sender, RoutedEventArgs e)
        {
            //SaveFileDialogクラスのインスタンスを作成
            SaveFileDialog sfd = new SaveFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            sfd.FileName = "newfile";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            sfd.InitialDirectory = "";
            //[ファイルの種類]に表示される選択肢を指定する
            sfd.Filter = "PNG|*.png";
            //[ファイルの種類]ではじめに選択されるものを指定する
            sfd.FilterIndex = 1;
            //タイトルを設定する
            sfd.Title = "保存先のファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = true;

            //ダイアログを表示する
            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = new FileStream(sfd.FileName, FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(wb));
                    encoder.Save(stream);
                }
            }
        }
    }
}
