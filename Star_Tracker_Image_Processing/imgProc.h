// imgProc.h
#ifndef _IMGPROC_H_
#define _IMGPROC_H_

#include <cstdint>
#include <cstdio>

typedef struct {
    int pxDimens_H;  // Image dimensions in pixels
    int pxDimens_V;
    double aov_H;    // Image angle of view
    double aov_V;
    int pxBytes;     // Bytes per pixel
    int pxDataSize; // Number of bytes in image data stream
} imgSpecs_t;

/***** Private Functions *****/

/***** Export Functions *****/

// Quantize all the pixels of the given image
extern "C" __declspec(dllexport) void 
imgQuantize(imgSpecs_t imgSpecs, uint8_t imgData[], char logBuff[]);

// Note: logBuff[] is defined as 16384 chars.

#endif	// _IMGPROC_H_
