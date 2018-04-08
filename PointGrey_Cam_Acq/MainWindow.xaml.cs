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

            // Initialize Main Image Box and its buffer
            ImgMain.Stretch = Stretch.Uniform;
            bmpMain = null;
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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
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
            UpdateImg();
        }

        private void BtnQuantize_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnClearLog_Click(object sender, RoutedEventArgs e)
        {
            TxtLog.Clear();
        }
    }
}
