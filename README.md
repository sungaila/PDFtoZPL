<img src="https://www.nuget.org/Content/gallery/img/default-package-icon.svg" align="left" width="64" height="64" alt="PDFtoZPL Placeholder Logo">

# PDFtoZPL
[![NuGet version](https://img.shields.io/nuget/v/PDFtoZPL.svg?style=flat-square)](https://www.nuget.org/packages/PDFtoZPL/)
[![NuGet downloads](https://img.shields.io/nuget/dt/PDFtoZPL.svg?style=flat-square)](https://www.nuget.org/packages/PDFtoZPL/)
[![GitHub license](https://img.shields.io/github/license/sungaila/PDFtoZPL?style=flat-square)](https://github.com/sungaila/PDFtoZPL/blob/master/LICENSE)

A .NET Standard library to convert [PDF files](https://en.wikipedia.org/wiki/PDF) into [Zebra Programming Language commands](https://en.wikipedia.org/wiki/Zebra_(programming_language)).

First your PDF is rasterized into a monochrome bitmap image. Then the ^GF (Graphic Field) command is applied to the bitmap. The bitmap is compressed to shrink the ZPL code in size.

The PDF rasterizer used in this project is a stripped down version of [PdfiumViewer](https://github.com/pvginkel/PdfiumViewer) which is built on top of [PDFium](https://pdfium.googlesource.com/pdfium/). I recommend the [Labelary Online ZPL Viewer](http://labelary.com/viewer.html) to check the created ZPL code. Only Windows x86 and x86-64 are supported as platforms.
