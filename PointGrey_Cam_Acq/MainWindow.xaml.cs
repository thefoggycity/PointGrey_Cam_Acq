using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using SpinnakerNET;
using SpinnakerNET.GenApi;
using Microsoft.Win32;

namespace PointGrey_Cam_Acq
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Bitmap bmpMain;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize Main Image Buffer
            bmpMain = null;
        }

        private void UpdateImg()
        {
            BitmapImage bitmapImage = new BitmapImage();

            if (bmpMain != null)
                using (MemoryStream memory = new MemoryStream())
                {
                    bmpMain.Save(memory, ImageFormat.Bmp);
                    memory.Position = 0;
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }

            ImgMain.Source = bitmapImage;
        }

        private void BtnAcquire_Click(object sender, RoutedEventArgs e)
        {
            CamCtrl cam = new CamCtrl(
                str => {
                    TxtLog.AppendText(str);
                });

            // Default given example
            //cam.AcquisitionExample();

            // Retrieve a BW image and display it accordingly
            IManagedImage result = cam.RetrieveMonoImage();
            if (result != null)
                bmpMain = result.bitmap;    //PixelFormat: Format8bppIndexed
            UpdateImg();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (bmpMain == null)
            {
                MessageBox.Show("No image loaded.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveFileDialog svf = new SaveFileDialog
            {
                InitialDirectory = Environment.CurrentDirectory,
                Title = "Save Image as...",
                Filter = "Bitmap (*.bmp)|*.bmp",
                ValidateNames = true,
                AddExtension = true
            };

            if (svf.ShowDialog() == false)
            {
                TxtLog.AppendText("\nImage not saved.\n");
                return;
            }

            bmpMain.Save(svf.FileName, ImageFormat.Bmp);
            TxtLog.AppendText("\n" + svf.FileName + " written.\n");
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog
            {
                InitialDirectory = Environment.CurrentDirectory,
                Title = "Open Image from...",
                Filter = 
                "Bitmap (*.bmp)|*.bmp|JPEG Image (*.jpg)|*.jpg|PNG Image (*.png)|*.png",
                AddExtension = true,
                ValidateNames = true,
                CheckPathExists = true,
                Multiselect = false
            };

            if (opf.ShowDialog() == false)
            {
                TxtLog.AppendText("\nImage not loaded.\n");
                return;
            }

            bmpMain = new Bitmap(opf.FileName);
            TxtLog.AppendText("\nImage loaded from " + opf.FileName + ".\n");
            TxtLog.AppendText("Image size: " + bmpMain.Size.ToString() + ".\n");
            TxtLog.AppendText("Pixel format: " + bmpMain.PixelFormat + ".\n");
            UpdateImg();
        }

        private void BtnQuantize_Click(object sender, RoutedEventArgs e)
        {
            if (bmpMain == null)
            {
                MessageBox.Show("No image loaded.", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            const int bytePerPx = 1;    // 8-bit color depth, Mono 8 palette
            int imgByteSize = bytePerPx * bmpMain.Height * bmpMain.Width;
            byte[] pxArr = new byte[imgByteSize];

            using (MemoryStream imgdata = new MemoryStream())
            {
                byte[] buff32 = new byte[4];
                UInt32 pxArrOfs;

                ImgProc imgProc = new ImgProc(  // Initialize image processor
                    str => { TxtLog.AppendText(str); },
                    new Tuple<int, int>(bmpMain.Width, bmpMain.Height), // pxDimens (H,V)
                    new Tuple<double, double>(6.86, 3.62),  // Image AoVs (H,V)
                    bytePerPx, imgByteSize
                    );

                bmpMain.Save(imgdata, ImageFormat.Bmp);
                imgdata.Seek(10, SeekOrigin.Begin); // Find out Pixel Array's offset
                imgdata.Read(buff32, 0, 4);
                pxArrOfs = BitConverter.ToUInt32(buff32, 0);

                imgdata.Seek(pxArrOfs, SeekOrigin.Begin);   // Jump to Pixel Array
                imgdata.Read(pxArr, 0, imgByteSize);    // Read Pixel Array data

                imgProc.QuantizePixels(pxArr);  // Quantize the pixels

                imgdata.Seek(pxArrOfs, SeekOrigin.Begin);   // Jump to Pixel Array
                imgdata.Write(pxArr, 0, imgByteSize);   // Write data back to stream
                bmpMain = new Bitmap(imgdata);  // Generate new image based on updated stream

                UpdateImg();
            }

            TxtLog.AppendText("Image quantization completed.\n");
        }

        private void BtnClearLog_Click(object sender, RoutedEventArgs e)
        {
            TxtLog.Clear();
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            // stub
        }
    }
}
