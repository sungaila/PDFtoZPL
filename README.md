<img src="https://raw.githubusercontent.com/sungaila/PDFtoZPL/master/Icon.png" align="left" width="64" height="64" alt="PDFtoZPL Logo">

# PDFtoZPL
[![Azure DevOps builds (branch)](https://img.shields.io/azure-devops/build/sungaila/fa66f1e9-b342-4f33-bcd4-40f7b082949d/4/master?style=flat-square)](https://dev.azure.com/sungaila/PDFtoZPL/_build/latest?definitionId=4&branchName=master)
[![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/sungaila/PDFtoZPL/4/master?style=flat-square)](https://dev.azure.com/sungaila/PDFtoZPL/_build/latest?definitionId=4&branchName=master)
[![NuGet version](https://img.shields.io/nuget/v/PDFtoZPL.svg?style=flat-square)](https://www.nuget.org/packages/PDFtoZPL/)
[![NuGet downloads](https://img.shields.io/nuget/dt/PDFtoZPL.svg?style=flat-square)](https://www.nuget.org/packages/PDFtoZPL/)
[![GitHub license](https://img.shields.io/github/license/sungaila/PDFtoZPL?style=flat-square)](https://github.com/sungaila/PDFtoZPL/blob/master/LICENSE)

A .NET library to convert [PDF files](https://en.wikipedia.org/wiki/PDF) (and bitmaps) into [Zebra Programming Language commands](https://en.wikipedia.org/wiki/Zebra_(programming_language)).

This .NET library is built on top of
* [PDFium](https://pdfium.googlesource.com/pdfium/) (native PDF renderer)
* [PdfiumViewer](https://github.com/pvginkel/PdfiumViewer) (wrapper for PDFium)

You can use [Labelary Online ZPL Viewer](http://labelary.com/viewer.html) to render the resulting ZPL code.

### How does it work?
0. Use PDFium to render a bitmap (for PDF files)
1. Make the bitmap monochrome
2. Convert the bitmap into a ^GF (Graphic Field) command
3. Compress the command hexdecimal data to shrink the ZPL code in size
4. Return the generated ZPL code