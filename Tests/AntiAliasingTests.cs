using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFtoImage;
using PDFtoZPL;
using System;
using System.IO;
using static PDFtoZPL.Conversion;

namespace Tests
{
    [TestClass]
    public partial class AntiAliasingTests
    {
        [TestInitialize]
        public void Initialize()
        {
#if NET6_0_OR_GREATER
            if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
                Assert.Inconclusive("This test must run on Windows, Linux or macOS.");
#endif
        }

#if NET6_0_OR_GREATER
#pragma warning disable CA1416
#endif

        [TestMethod]
        [DataRow("SocialPreview.pdf", null, DisplayName = "Default (None)")]
        [DataRow("SocialPreview.pdf", PdfAntiAliasing.None, DisplayName = "None")]
        [DataRow("SocialPreview.pdf", PdfAntiAliasing.Text, DisplayName = "Text")]
        [DataRow("SocialPreview.pdf", PdfAntiAliasing.Images, DisplayName = "Images")]
        [DataRow("SocialPreview.pdf", PdfAntiAliasing.Paths, DisplayName = "Paths")]
        [DataRow("SocialPreview.pdf", PdfAntiAliasing.Text | PdfAntiAliasing.Images, DisplayName = "Text | Images")]
        [DataRow("SocialPreview.pdf", PdfAntiAliasing.Text | PdfAntiAliasing.Paths, DisplayName = "Text | Paths")]
        [DataRow("SocialPreview.pdf", PdfAntiAliasing.Images | PdfAntiAliasing.Paths, DisplayName = "Images | Paths")]
        [DataRow("SocialPreview.pdf", PdfAntiAliasing.Text | PdfAntiAliasing.Images | PdfAntiAliasing.Paths, DisplayName = "Text | Images | Paths")]
        [DataRow("SocialPreview.pdf", PdfAntiAliasing.All, DisplayName = "All")]
        [DataRow("Wikimedia_Commons_web.pdf", null, DisplayName = "Default (None)")]
        [DataRow("Wikimedia_Commons_web.pdf", PdfAntiAliasing.None, DisplayName = "None")]
        [DataRow("Wikimedia_Commons_web.pdf", PdfAntiAliasing.Text, DisplayName = "Text")]
        [DataRow("Wikimedia_Commons_web.pdf", PdfAntiAliasing.Images, DisplayName = "Images")]
        [DataRow("Wikimedia_Commons_web.pdf", PdfAntiAliasing.Paths, DisplayName = "Paths")]
        [DataRow("Wikimedia_Commons_web.pdf", PdfAntiAliasing.Text | PdfAntiAliasing.Images, DisplayName = "Text | Images")]
        [DataRow("Wikimedia_Commons_web.pdf", PdfAntiAliasing.Text | PdfAntiAliasing.Paths, DisplayName = "Text | Paths")]
        [DataRow("Wikimedia_Commons_web.pdf", PdfAntiAliasing.Images | PdfAntiAliasing.Paths, DisplayName = "Images | Paths")]
        [DataRow("Wikimedia_Commons_web.pdf", PdfAntiAliasing.Text | PdfAntiAliasing.Images | PdfAntiAliasing.Paths, DisplayName = "Text | Images | Paths")]
        [DataRow("Wikimedia_Commons_web.pdf", PdfAntiAliasing.All, DisplayName = "All")]
        public void ConvertWithAntiAliasing(string fileName, PdfAntiAliasing? antiAliasing)
        {
            var expectedResult = _expectedResults[fileName][antiAliasing ?? PdfAntiAliasing.All];

            using var fileStream = new FileStream(Path.Combine("Assets", fileName), FileMode.Open, FileAccess.Read);

            var zplResult
                = antiAliasing != null ? ConvertPdfPage(fileStream, pdfOptions: new(Dpi: 203, AntiAliasing: antiAliasing.Value), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed)) : ConvertPdfPage(fileStream, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed));

            Assert.AreEqual(expectedResult, zplResult.Replace("\n", string.Empty));
        }
    }
}