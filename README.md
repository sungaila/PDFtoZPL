# ![PDFtoZPL Logo](https://raw.githubusercontent.com/sungaila/PDFtoZPL/master/Icon_64.png) PDFtoZPL

[![Azure DevOps builds (branch)](https://img.shields.io/azure-devops/build/sungaila/fa66f1e9-b342-4f33-bcd4-40f7b082949d/4/master?style=flat-square)](https://dev.azure.com/sungaila/PDFtoZPL/_build/latest?definitionId=4&branchName=master)
[![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/sungaila/PDFtoZPL/4/master?style=flat-square)](https://dev.azure.com/sungaila/PDFtoZPL/_build/latest?definitionId=4&branchName=master)
[![SonarCloud Quality Gate](https://img.shields.io/sonar/quality_gate/sungaila_PDFtoZPL?server=https%3A%2F%2Fsonarcloud.io&style=flat-square)](https://sonarcloud.io/dashboard?id=sungaila_PDFtoZPL)
[![NuGet version](https://img.shields.io/nuget/v/PDFtoZPL.svg?style=flat-square)](https://www.nuget.org/packages/PDFtoZPL/)
[![NuGet downloads](https://img.shields.io/nuget/dt/PDFtoZPL.svg?style=flat-square)](https://www.nuget.org/packages/PDFtoZPL/)
[![GitHub license](https://img.shields.io/github/license/sungaila/PDFtoZPL?style=flat-square)](https://github.com/sungaila/PDFtoZPL/blob/master/LICENSE)

A .NET library to convert [PDF files](https://en.wikipedia.org/wiki/PDF) (and bitmaps) into [Zebra Programming Language commands](https://en.wikipedia.org/wiki/Zebra_(programming_language)).

This .NET library is built on top of
* [PDFium](https://pdfium.googlesource.com/pdfium/) (native PDF renderer)
* [PdfiumViewer](https://github.com/pvginkel/PdfiumViewer) (wrapper for PDFium)

You can use [Labelary Online ZPL Viewer](http://labelary.com/viewer.html) to render the resulting ZPL code.

## Getting started
Just call one of the following static methods:
* `PDFtoZPL.Conversion.ConvertPdfPage()`
* `PDFtoZPL.Conversion.ConvertPdf()`
* `PDFtoZPL.Conversion.ConvertPdfAsync()`
* `PDFtoZPL.Conversion.ConvertBitmap()`

## Prerequisite libgdiplus
On platforms other than Windows you will have to have [libgdiplus](https://www.mono-project.com/docs/gui/libgdiplus/) installed.
### Debian-based Linux distributions
```console
sudo apt-get install libgdiplus
```

### macOS (via [Homebrew](https://brew.sh/))
```console
brew install mono-libgdiplus
```

### How does it work?
0. Use PDFium to render a bitmap (for PDF files)
1. Make the bitmap monochrome
2. Convert the bitmap into a ^GF (Graphic Field) command
3. Compress the command hexdecimal data to shrink the ZPL code in size
4. Return the generated ZPL code
