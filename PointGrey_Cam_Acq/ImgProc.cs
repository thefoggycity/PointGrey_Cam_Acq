﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PointGrey_Cam_Acq
{
    class ImgProc
    {
        /***** Constants and DLL Import Declaration *****/

        private const int LOGBUFFSIZE = 16384;
        private const string DLLNAME = "libImgProc.dll";

        [DllImport(DLLNAME, EntryPoint = "imgQuantize",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void
            ImgQuantize(ImgSpecs imgSpecs, IntPtr imgDataPtr, IntPtr logPtr);

        /***** Delegates Declaration *****/

        public delegate void LogWriter(String logMessage);
        LogWriter writeLog;

        private delegate void
            ProcMethod(ImgSpecs imgSpecs, IntPtr imgDataPtr, IntPtr logPtr);

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

        /***** Class Member Methods *****/

        private void InvokeMethod(ProcMethod procMethod, byte[] pxData)
        {
            IntPtr imgDataPtr = Marshal.AllocHGlobal(specs.pxDataSize);
            IntPtr logPtr = Marshal.AllocHGlobal(LOGBUFFSIZE);
            byte[] logBuffer = new byte[LOGBUFFSIZE];

            // To zero-init the logBuffer and pass the image pixels
            Marshal.Copy(logBuffer, 0, imgDataPtr, LOGBUFFSIZE);
            Marshal.Copy(pxData, 0, imgDataPtr, specs.pxDataSize);

            procMethod(specs, imgDataPtr, logPtr);

            // Copy back to original place
            Marshal.Copy(imgDataPtr, pxData, 0, specs.pxDataSize);
            Marshal.FreeHGlobal(imgDataPtr);

            // Print the log generated by the dll
            Marshal.Copy(logPtr, logBuffer, 0, LOGBUFFSIZE);
            Marshal.FreeHGlobal(logPtr);
            using (StreamReader streamReader = new StreamReader(
                new MemoryStream(logBuffer), Encoding.ASCII))
            {
                while (streamReader.Peek() != 0)
                    writeLog(streamReader.ReadLine() + "\n");
            }

        }

        public void QuantizePixels(byte[] pxData)
        {
            InvokeMethod(ImgQuantize, pxData);
        }

    }
}
