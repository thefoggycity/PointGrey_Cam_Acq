// imgProc.cpp
#include "imgProc.h"

// Quantize all the pixels of the given image
void imgQuantize(imgSpecs_t imgSpecs, uint8_t imgData[], char logBuff[])
{
	const int THRD = 15;	// Threshold 15 above average
	double pxSum = 0;

	for (int i = 0; i < imgSpecs.pxDataSize; i++) 
		pxSum += imgData[i];
	pxSum /= imgSpecs.pxDataSize;	// Average intensity
	pxSum += 15;	// Get the real threshold
	
	for (int i = 0; i < imgSpecs.pxDataSize; i++)
		imgData[i] = (imgData[i] > pxSum) ? 0xFF : 0x0;

	logBuff += sprintf(logBuff, 
		"\n[DLL]: Quantization completed with THRD=%d.\n\n", THRD);

	return;
}
