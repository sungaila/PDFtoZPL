# ![PDFtoZPL Logo](https://raw.githubusercontent.com/sungaila/PDFtoZPL/master/Icon_64.png) PDFtoZPL

[![GitHub Workflow Build Status](https://img.shields.io/github/actions/workflow/status/sungaila/PDFtoZPL/dotnet.yml?event=push&style=flat-square&logo=github&logoColor=white)](https://github.com/sungaila/PDFtoZPL/actions/workflows/dotnet.yml)
[![GitHub Workflow Test Runs Succeeded](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fgist.githubusercontent.com%2Fsungaila%2F47230c16cb63a1be5b5604830579714d%2Fraw&query=%24.stats.runs_succ&suffix=%20passed&style=flat-square&logo=github&logoColor=white&label=tests&color=45cc11)](https://github.com/sungaila/PDFtoZPL/actions/workflows/dotnet.yml)
[![SonarCloud Quality Gate](https://img.shields.io/sonar/quality_gate/sungaila_PDFtoZPL?server=https%3A%2F%2Fsonarcloud.io&style=flat-square&logo=sonarcloud&logoColor=white)](https://sonarcloud.io/project/overview?id=sungaila_PDFtoZPL)
[![NuGet version](https://img.shields.io/nuget/v/PDFtoZPL.svg?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/PDFtoZPL/)
[![NuGet downloads](https://img.shields.io/nuget/dt/PDFtoZPL.svg?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/PDFtoZPL/)
[![Website](https://img.shields.io/website?up_message=online&down_message=offline&url=https%3A%2F%2Fsungaila.github.io%2FPDFtoZPL%2F&style=flat-square&label=website)](https://sungaila.github.io/PDFtoZPL/)
[![GitHub license](https://img.shields.io/github/license/sungaila/PDFtoZPL?style=flat-square)](https://github.com/sungaila/PDFtoZPL/blob/master/LICENSE)

A .NET library to convert [PDF files](https://en.wikipedia.org/wiki/PDF) (and bitmaps) into [Zebra Programming Language commands](https://en.wikipedia.org/wiki/Zebra_(programming_language)).

This .NET library is built on top of
* [PDFium](https://pdfium.googlesource.com/pdfium/) (native PDF renderer)
* [PdfiumViewer](https://github.com/pvginkel/PdfiumViewer) (wrapper for PDFium)
* [SkiaSharp](https://github.com/mono/SkiaSharp) (cross-platform 2D graphics API)

You can use [Labelary Online ZPL Viewer](http://labelary.com/viewer.html) to render the resulting ZPL code.

## Getting started
Just call one of the following static methods:
* `PDFtoZPL.Conversion.ConvertPdfPage()`
* `PDFtoZPL.Conversion.ConvertPdf()`
* `PDFtoZPL.Conversion.ConvertPdfAsync()`
* `PDFtoZPL.Conversion.ConvertBitmap()`

### How does it work?
0. Use PDFium to render a bitmap (for PDF files)
1. Make the bitmap monochrome
2. Convert the bitmap into a ^GF (Graphic Field) command
3. Compress the command hexdecimal data to shrink the ZPL code in size
4. Return the generated ZPL code
