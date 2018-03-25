using System;
using System.IO;
using System.Collections.Generic;
using SpinnakerNET;
using SpinnakerNET.GenApi;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointGrey_Cam_Acq
{
    class CamCtrl
    {
        public delegate void LogWriter(String logMessage);

        LogWriter writeLog;

        public CamCtrl(LogWriter logWriter)
        {
            // Default print to console
            writeLog = logWriter;
        }

        public int AcquireMono(IManagedCamera cam, INodeMap nodeMap, INodeMap nodeMapTLDevice)
        {
            int result = 0;

            writeLog(String.Format("\n*** BW IMAGE ACQUISITION ***\n\n"));

            try
            {
                // Set acquisition mode to continuous

                // Retrieve enumeration node from nodemap
                IEnum iAcquisitionMode = nodeMap.GetNode<IEnum>("AcquisitionMode");
                if (iAcquisitionMode == null || !iAcquisitionMode.IsWritable)
                {
                    writeLog(String.Format(
                        "Unable to set acquisition mode to continuous (node retrieval). Aborting...\n\n"));
                    return -1;
                }

                // Retrieve entry node from enumeration node
                IEnumEntry iAcquisitionModeContinuous = iAcquisitionMode.GetEntryByName("Continuous");
                if (iAcquisitionModeContinuous == null || !iAcquisitionMode.IsReadable)
                {
                    writeLog(String.Format(
                        "Unable to set acquisition mode to continuous (enum entry retrieval). Aborting...\n\n"));
                    return -1;
                }

                // Set symbolic from entry node as new value for enumeration node
                iAcquisitionMode.Value = iAcquisitionModeContinuous.Symbolic;

                writeLog(String.Format("Acquisition mode set to continuous...\n"));

                //
                // Begin acquiring images
                //
                // *** NOTES ***
                // What happens when the camera begins acquiring images depends 
                // on which acquisition mode has been set. Single frame captures 
                // only a single image, multi frame catures a set number of 
                // images, and continuous captures a continuous stream of images.
                // Because the example calls for the retrieval of 10 images, 
                // continuous mode has been set for the example.
                // 
                // *** LATER ***
                // Image acquisition must be ended when no more images are needed.
                //
                cam.BeginAcquisition();

                writeLog(String.Format("Acquiring images...\n"));

                //
                // Retrieve device serial number for filename
                //
                // *** NOTES ***
                // The device serial number is retrieved in order to keep 
                // different cameras from overwriting each other's images. 
                // Grabbing image IDs and frame IDs make good alternatives for 
                // this purpose.
                //
                String deviceSerialNumber = "";

                IString iDeviceSerialNumber = nodeMapTLDevice.GetNode<IString>("DeviceSerialNumber");
                if (iDeviceSerialNumber != null && iDeviceSerialNumber.IsReadable)
                {
                    deviceSerialNumber = iDeviceSerialNumber.Value;

                    writeLog(String.Format(
                        "Device serial number retrieved as {0}...\n", deviceSerialNumber));
                }
                writeLog(String.Format("\n"));

                // Retrieve, convert, and save images
                try
                {
                    //
                    // Retrieve next received image
                    //
                    // *** NOTES ***
                    // Capturing an image houses images on the camera buffer. 
                    // Trying to capture an image that does not exist will 
                    // hang the camera.
                    //
                    // Using-statements help ensure that images are released.
                    // If too many images remain unreleased, the buffer will
                    // fill, causing the camera to hang. Images can also be
                    // released manually by calling Release().
                    // 
                    using (IManagedImage rawImage = cam.GetNextImage())
                    {
                        //
                        // Ensure image completion
                        //
                        // *** NOTES ***
                        // Images can easily be checked for completion. This 
                        // should be done whenever a complete image is 
                        // expected or required. Alternatively, check image
                        // status for a little more insight into what 
                        // happened.
                        //
                        if (rawImage.IsIncomplete)
                        {
                            writeLog(String.Format(
                                "Image incomplete with image status {0}...\n", rawImage.ImageStatus));
                        }
                        else
                        {
                            // TODO: Need to return the acquired rawImage here.
                            return 0;
                        }
                    }
                }
                catch (SpinnakerException ex)
                {
                    writeLog(String.Format("Error: {0}\n", ex.Message));
                    result = -1;
                } 

                //
                // End acquisition
                //
                // *** NOTES ***
                // Ending acquisition appropriately helps ensure that devices 
                // clean up properly and do not need to be power-cycled to
                // maintain integrity.
                //
                cam.EndAcquisition();
            }
            catch (SpinnakerException ex)
            {
                writeLog(String.Format("Error: {0}\n", ex.Message));
                result = -1;
            }

            return result;
        }


        // Code below is directly copied from example_acquisition

        // This function acquires and saves 10 images from a device. 
        public int AcquireImages(IManagedCamera cam, INodeMap nodeMap, INodeMap nodeMapTLDevice)
        {
            int result = 0;

            writeLog(String.Format("\n*** IMAGE ACQUISITION ***\n\n"));

            try
            {
                //
                // Set acquisition mode to continuous
                //
                // *** NOTES ***
                // Because the example acquires and saves 10 images, setting 
                // acquisition mode to continuous lets the example finish. If 
                // set to single frame or multiframe (at a lower number of 
                // images), the example would just hang. This is because the 
                // example has been written to acquire 10 images while the 
                // camera would have been programmed to retrieve less than that.
                // 
                // Setting the value of an enumeration node is slightly more 
                // complicated than other node types. Two nodes are required: 
                // first, the enumeration node is retrieved from the nodemap and 
                // second, the entry node is retrieved from the enumeration node. 
                // The symbolic of the entry node is then set as the new value 
                // of the enumeration node.
                //
                // Notice that both the enumeration and entry nodes are checked 
                // for availability and readability/writability. Enumeration 
                // nodes are generally readable and writable whereas entry 
                // nodes are only ever readable.
                // 
                // Retrieve enumeration node from nodemap
                IEnum iAcquisitionMode = nodeMap.GetNode<IEnum>("AcquisitionMode");
                if (iAcquisitionMode == null || !iAcquisitionMode.IsWritable)
                {
                    writeLog(String.Format(
                        "Unable to set acquisition mode to continuous (node retrieval). Aborting...\n\n"));
                    return -1;
                }

                // Retrieve entry node from enumeration node
                IEnumEntry iAcquisitionModeContinuous = iAcquisitionMode.GetEntryByName("Continuous");
                if (iAcquisitionModeContinuous == null || !iAcquisitionMode.IsReadable)
                {
                    writeLog(String.Format(
                        "Unable to set acquisition mode to continuous (enum entry retrieval). Aborting...\n\n"));
                    return -1;
                }

                // Set symbolic from entry node as new value for enumeration node
                iAcquisitionMode.Value = iAcquisitionModeContinuous.Symbolic;

                writeLog(String.Format("Acquisition mode set to continuous...\n"));

                //
                // Begin acquiring images
                //
                // *** NOTES ***
                // What happens when the camera begins acquiring images depends 
                // on which acquisition mode has been set. Single frame captures 
                // only a single image, multi frame catures a set number of 
                // images, and continuous captures a continuous stream of images.
                // Because the example calls for the retrieval of 10 images, 
                // continuous mode has been set for the example.
                // 
                // *** LATER ***
                // Image acquisition must be ended when no more images are needed.
                //
                cam.BeginAcquisition();

                writeLog(String.Format("Acquiring images...\n"));

                //
                // Retrieve device serial number for filename
                //
                // *** NOTES ***
                // The device serial number is retrieved in order to keep 
                // different cameras from overwriting each other's images. 
                // Grabbing image IDs and frame IDs make good alternatives for 
                // this purpose.
                //
                String deviceSerialNumber = "";

                IString iDeviceSerialNumber = nodeMapTLDevice.GetNode<IString>("DeviceSerialNumber");
                if (iDeviceSerialNumber != null && iDeviceSerialNumber.IsReadable)
                {
                    deviceSerialNumber = iDeviceSerialNumber.Value;

                    writeLog(String.Format(
                        "Device serial number retrieved as {0}...\n", deviceSerialNumber));
                }
                writeLog(String.Format("\n"));

                // Retrieve, convert, and save images
                const int NumImages = 10;

                for (int imageCnt = 0; imageCnt < NumImages; imageCnt++)
                {
                    try
                    {
                        //
                        // Retrieve next received image
                        //
                        // *** NOTES ***
                        // Capturing an image houses images on the camera buffer. 
                        // Trying to capture an image that does not exist will 
                        // hang the camera.
                        //
                        // Using-statements help ensure that images are released.
                        // If too many images remain unreleased, the buffer will
                        // fill, causing the camera to hang. Images can also be
                        // released manually by calling Release().
                        // 
                        using (IManagedImage rawImage = cam.GetNextImage())
                        {
                            //
                            // Ensure image completion
                            //
                            // *** NOTES ***
                            // Images can easily be checked for completion. This 
                            // should be done whenever a complete image is 
                            // expected or required. Alternatively, check image
                            // status for a little more insight into what 
                            // happened.
                            //
                            if (rawImage.IsIncomplete)
                            {
                                writeLog(String.Format(
                                    "Image incomplete with image status {0}...\n", rawImage.ImageStatus));
                            }
                            else
                            {
                                //
                                // Print image information; width and height 
                                // recorded in pixels
                                //
                                // *** NOTES ***
                                // Images have quite a bit of available metadata 
                                // including CRC, image status, and offset 
                                // values to name a few.
                                //
                                uint width = rawImage.Width;

                                uint height = rawImage.Height;

                                writeLog(String.Format(
                                    "Grabbed image {0}, width = {1}, height = {1}\n", imageCnt, width, height));
                                writeLog(String.Format(
                                    "Pixel format is {0}\n", rawImage.PixelFormatName));

                                //
                                // Convert image to mono 8
                                //
                                // *** NOTES ***
                                // Images can be converted between pixel formats
                                // by using the appropriate enumeration value.
                                // Unlike the original image, the converted one 
                                // does not need to be released as it does not 
                                // affect the camera buffer.
                                // 
                                // Using statements are a great way to ensure code
                                // stays clean and avoids memory leaks.
                                // leaks.
                                //
                                using (IManagedImage convertedImage = rawImage.Convert(PixelFormatEnums.Mono8))
                                {
                                    // Create a unique filename
                                    String filename = "Acquisition-CSharp-";
                                    if (deviceSerialNumber != "")
                                    {
                                        filename = filename + deviceSerialNumber + "-";
                                    }
                                    filename = filename + imageCnt + ".jpg";

                                    //
                                    // Save image
                                    // 
                                    // *** NOTES ***
                                    // The standard practice of the examples is 
                                    // to use device serial numbers to keep 
                                    // images of one device from overwriting 
                                    // those of another.
                                    //
                                    convertedImage.Save(filename);

                                    writeLog(String.Format("Image saved at {0}\n\n", filename));
                                }
                            }
                        }
                    }
                    catch (SpinnakerException ex)
                    {
                        writeLog(String.Format("Error: {0}\n", ex.Message));
                        result = -1;
                    }
                }

                //
                // End acquisition
                //
                // *** NOTES ***
                // Ending acquisition appropriately helps ensure that devices 
                // clean up properly and do not need to be power-cycled to
                // maintain integrity.
                //
                cam.EndAcquisition();
            }
            catch (SpinnakerException ex)
            {
                writeLog(String.Format("Error: {0}\n", ex.Message));
                result = -1;
            }

            return result;
        }

        // This function prints the device information of the camera from the 
        // transport layer; please see NodeMapInfo_CSharp example for more 
        // in-depth comments on printing device information from the nodemap.
        public int PrintDeviceInfo(INodeMap nodeMap)
        {
            int result = 0;

            try
            {
                writeLog(String.Format("\n*** DEVICE INFORMATION ***\n"));

                ICategory category = nodeMap.GetNode<ICategory>("DeviceInformation");
                if (category != null && category.IsReadable)
                {
                    for (int i = 0; i < category.Children.Length; i++)
                    {
                        writeLog(String.Format(
                            "{0}: {1}\n", category.Children[i].Name, 
                            (category.Children[i].IsReadable ? 
                            category.Children[i].ToString() : "Node not available")));
                    }
                    writeLog(String.Format("\n"));
                }
                else
                {
                    writeLog(String.Format("Device control information not available.\n"));
                }
            }
            catch (SpinnakerException ex)
            {
                writeLog(String.Format("Error: {0}\n", ex.Message));
                result = -1;
            }

            return result;
        }

        // This function acts as the body of the example; please see 
        // NodeMapInfo_CSharp example for more in-depth comments on setting up 
        // cameras.
        public int RunSingleCamera(IManagedCamera cam)
        {
            int result = 0;

            try
            {
                // Retrieve TL device nodemap and print device information
                INodeMap nodeMapTLDevice = cam.GetTLDeviceNodeMap();

                result = PrintDeviceInfo(nodeMapTLDevice);

                // Initialize camera
                cam.Init();

                // Retrieve GenICam nodemap
                INodeMap nodeMap = cam.GetNodeMap();

                // Acquire images
                result = result | AcquireImages(cam, nodeMap, nodeMapTLDevice);

                // Deinitialize camera
                cam.DeInit();
            }
            catch (SpinnakerException ex)
            {
                writeLog(String.Format("Error: {0}\n", ex.Message));
                result = -1;
            }

            return result;
        }

        // Entry point of the Acquisition Example
        public void AcquisitionExample()
        {
            int result = 0;

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
                writeLog(String.Format(
                    "Failed to create file in current folder. Please check permissions."));
            }

            // Retrieve singleton reference to system object
            ManagedSystem system = new ManagedSystem();

            // Retrieve list of cameras from the system
            IList<IManagedCamera> camList = system.GetCameras();

            writeLog(String.Format("Number of cameras detected: {0}\n\n", camList.Count));

            // Finish if there are no cameras
            if (camList.Count == 0)
            {
                // Clear camera list before releasing system
                camList.Clear();

                // Release system
                system.Dispose();

                writeLog(String.Format("Not enough cameras!"));
                writeLog(String.Format("Done!"));
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
                    writeLog(String.Format("Running example for camera {0}...\n", index));

                    try
                    {
                        // Run example
                        result = result | this.RunSingleCamera(managedCamera);
                    }
                    catch (SpinnakerException ex)
                    {
                        writeLog(String.Format("Error: {0}\n", ex.Message));
                        result = -1;
                    }
                    writeLog(String.Format("Camera {0} example complete...\n", index++));
                }

            // Clear camera list before releasing system
            camList.Clear();

            // Release system
            system.Dispose();

            writeLog(String.Format("Done!\n"));
        }
    }
}
