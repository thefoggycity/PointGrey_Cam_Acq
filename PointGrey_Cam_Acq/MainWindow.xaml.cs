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
using SpinnakerNET;
using SpinnakerNET.GenApi;

namespace PointGrey_Cam_Acq
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Initialize Main Image Box
            ImgMain.Stretch = Stretch.Uniform;
        }

        private void BtnAcquire_Click(object sender, RoutedEventArgs e)
        {
            CamCtrl cam = new CamCtrl(
                str => {
                    TxtLog.AppendText(str);
                });

            // Default given example
            //cam.AcquisitionExample();

            // Retrieve a BW image and display accordingly
            IManagedImage result = cam.RetrieveMonoImage();   // TODO: Equate it to acquired result.
            if (result != null)
                ImgMain.Source = BitmapToImageSource(result.bitmap);
        }

        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
