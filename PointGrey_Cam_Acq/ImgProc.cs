using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PointGrey_Cam_Acq
{
    class ImgProc
    {
        /***** DLL Import Declaration *****/

        const string DLLNAME = "libImgProc.dll";

        [DllImport(DLLNAME, EntryPoint = "imgQuantize",
            CallingConvention = CallingConvention.Cdecl)]
        unsafe private static extern void
            ImgQuantize(ImgSpecs imgSpecs, IntPtr ptr_imgData);

        /***** Logger Declaration *****/

        public delegate void LogWriter(String logMessage);
        LogWriter writeLog;

        /***** Image Attributes Declaration *****/

        [StructLayout(LayoutKind.Sequential)]
        struct ImgSpecs
        {
            public int pxDimens_H;  // Image dimensions in pixels
            public int pxDimens_V;
            public double aov_H;    // Image angle of view
            public double aov_V;
            public int pxBytes;     // Bytes per pixel
            public int pxDataSize; // Number of bytes in image data stream
        }
        ImgSpecs specs;

        /***** Class Initializer *****/

        public ImgProc(LogWriter logWriter, Tuple<int,int> pixelDimensions,
            Tuple<double,double> AoV, int bytesPerPx, int bytesInImage)
        {
            specs.pxDimens_H = pixelDimensions.Item1;
            specs.pxDimens_V = pixelDimensions.Item2;
            specs.aov_H = AoV.Item1;
            specs.aov_V = AoV.Item2;
            specs.pxBytes = bytesPerPx;
            specs.pxDataSize = bytesInImage;
            writeLog = logWriter;
        }

        unsafe public void QuantizePixels(byte[] pxData)
        {
            IntPtr ptr_imgData = Marshal.AllocHGlobal(specs.pxDataSize);
            Marshal.Copy(pxData, 0, ptr_imgData, specs.pxDataSize);
            ImgQuantize(specs, ptr_imgData);
            Marshal.Copy(ptr_imgData, pxData, 0, specs.pxDataSize);
            Marshal.FreeHGlobal(ptr_imgData);
        }

    }
}
