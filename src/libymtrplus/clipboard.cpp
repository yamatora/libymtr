#include "pch.h"
#include <Windows.h>

//	Read Clipboard
DLL_API bool hasClipboardImage() {
	bool result = false;
	bool isOpened = OpenClipboard(NULL);
	if (isOpened) {
		result = IsClipboardFormatAvailable(CF_DIB);
	}
	CloseClipboard();
	return result;
}
static BITMAPINFO* getLockedBitmapInfoPtr() {
	HANDLE hClipboardData = GetClipboardData(CF_DIB);
	BITMAPINFO* pData = (BITMAPINFO*)GlobalLock(hClipboardData);
	return pData;
}
DLL_API void getClipboardImageSize(int* width, int* height, int* bitsPerPixel) {
	bool isOpened = OpenClipboard(NULL);
	if (isOpened && IsClipboardFormatAvailable(CF_DIB)) {
		BITMAPINFO* pData = getLockedBitmapInfoPtr();
		if (pData != nullptr) {
			*width = pData->bmiHeader.biWidth;
			*height = pData->bmiHeader.biHeight;
			*bitsPerPixel = pData->bmiHeader.biBitCount;
		}
		GlobalUnlock(pData);
	}
	CloseClipboard();
}
DLL_API bool getClipboardImage(unsigned char* buffer) {
	bool result = false;
	if (OpenClipboard(NULL) && IsClipboardFormatAvailable(CF_DIB)) {
		BITMAPINFO* pData = getLockedBitmapInfoPtr();
		if (pData != nullptr) {
			int width, height, bitsPerPixel;
			getClipboardImageSize(&width, &height, &bitsPerPixel);

			//	Only 24bit(JPEG), 32bit(PNG-32)
			if (bitsPerPixel == 24 || bitsPerPixel == 32) {
				unsigned char* pImgData = (unsigned char*)pData + pData->bmiHeader.biSize;
				int bytesPerPixel = bitsPerPixel / 8;
				if (pData->bmiHeader.biCompression == BI_BITFIELDS) {
					pImgData += 4 * 3;
				}
				int bytesPerLine = width * bytesPerPixel;
				if (bytesPerLine % 4 != 0) {
					bytesPerLine += 4 - (bytesPerLine % 4);
				}

				unsigned char* dst = buffer;
				unsigned char* src = pImgData;
				for (int h = 0; h < height; h++) {
					memcpy(
						dst + (width * h * bytesPerPixel),
						src + (h * bytesPerLine),
						width * bytesPerPixel
					);
				}
				result = true;
			}
		}
		GlobalUnlock(pData);
	}
	CloseClipboard();
	return result;
}

//	Write Clipboard(32bit only)
DLL_API bool setClipboardImage(unsigned char* data, int width, int height) {
	bool result = false;
	if (OpenClipboard(NULL)) {
		int size = width * height * 4;

		HGLOBAL hg = GlobalAlloc(GMEM_MOVEABLE, sizeof(BITMAPINFOHEADER) + size);
		BITMAPINFO* pInfo = (BITMAPINFO*)GlobalLock(hg);

		pInfo->bmiHeader.biSize = sizeof(BITMAPINFO);
		pInfo->bmiHeader.biWidth = width;
		pInfo->bmiHeader.biHeight = height;
		pInfo->bmiHeader.biPlanes = 1;
		pInfo->bmiHeader.biBitCount = 32;
		pInfo->bmiHeader.biCompression = BI_RGB;
		pInfo->bmiHeader.biSizeImage = width * height * 4;
		pInfo->bmiHeader.biXPelsPerMeter = 0;
		pInfo->bmiHeader.biYPelsPerMeter = 0;
		pInfo->bmiHeader.biClrUsed = 0;
		pInfo->bmiHeader.biClrImportant = 0;

		memcpy(pInfo->bmiColors, data, size);
		GlobalUnlock(hg);

		EmptyClipboard();
		SetClipboardData(CF_DIB, hg);
		result = true;
	}
	CloseClipboard();
	return result;
}