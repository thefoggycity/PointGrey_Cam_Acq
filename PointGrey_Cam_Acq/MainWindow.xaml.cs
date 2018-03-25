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
        }

        private void BtnAcquire_Click(object sender, RoutedEventArgs e)
        {
            int result = 0;

            TxtLog.AppendText("Started running...\n");

            CamCtrl cam = new CamCtrl(
                str => {
                    TxtLog.AppendText(str);
                });

            // Since this application saves images in the current folder
            // we must ensure that we have permission to write to this folder.
            // If we do not have permission, fail right away.
            FileStream fileStream;
            try
            {
                fileStream = new FileStream(@"test.txt", FileMode.Create);
                fileStream.Close();
                File.Delete("test.txt");
            }
            catch
            {
                TxtLog.AppendText(String.Format(
                    "Failed to create file in current folder. Please check permissions."));
            }

            // Retrieve singleton reference to system object
            ManagedSystem system = new ManagedSystem();

            // Retrieve list of cameras from the system
            IList<IManagedCamera> camList = system.GetCameras();

            TxtLog.AppendText(String.Format("Number of cameras detected: {0}\n\n", camList.Count));

            // Finish if there are no cameras
            if (camList.Count == 0)
            {
                // Clear camera list before releasing system
                camList.Clear();

                // Release system
                system.Dispose();

                TxtLog.AppendText(String.Format("Not enough cameras!"));
                TxtLog.AppendText(String.Format("Done!"));
            }

            //
            // Run example on each camera
            //
            // *** NOTES ***
            // Cameras can either be retrieved as their own IManagedCamera
            // objects or from camera lists using the [] operator and an index.
            //
            // Using-statements help ensure that cameras are disposed of when
            // they are no longer needed; otherwise, cameras can be disposed of
            // manually by calling Dispose(). In C#, if cameras are not disposed
            // of before the system is released, the system will do so
            // automatically.
            //
            int index = 0;

            foreach (IManagedCamera managedCamera in camList)
                using (managedCamera)
                {
                    TxtLog.AppendText(String.Format("Running example for camera {0}...\n", index));

                    try
                    {
                        // Run example
                        result = result | cam.RunSingleCamera(managedCamera);
                    }
                    catch (SpinnakerException ex)
                    {
                        TxtLog.AppendText(String.Format("Error: {0}\n", ex.Message));
                        result = -1;
                    }
                    TxtLog.AppendText(String.Format("Camera {0} example complete...\n", index++));
                }

            // Clear camera list before releasing system
            camList.Clear();

            // Release system
            system.Dispose();

            TxtLog.AppendText(String.Format("Done!\n"));
        }
    }
}
