using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFtoZPL;
using System;
using System.IO;
using static PDFtoZPL.Conversion;

namespace Tests
{
    [TestClass]
    public partial class DitheringTests
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
        [DataRow("SocialPreview.pdf", null)]
        [DataRow("SocialPreview.pdf", DitheringKind.None)]
        [DataRow("SocialPreview.pdf", DitheringKind.FloydSteinberg)]
        [DataRow("SocialPreview.pdf", DitheringKind.Atkinson)]
        [DataRow("Wikimedia_Commons_web.pdf", null)]
        [DataRow("Wikimedia_Commons_web.pdf", DitheringKind.None)]
        [DataRow("Wikimedia_Commons_web.pdf", DitheringKind.FloydSteinberg)]
        [DataRow("Wikimedia_Commons_web.pdf", DitheringKind.Atkinson)]
        [DataRow("SocialPreview.png", null)]
        [DataRow("SocialPreview.png", DitheringKind.None)]
        [DataRow("SocialPreview.png", DitheringKind.FloydSteinberg)]
        [DataRow("SocialPreview.png", DitheringKind.Atkinson)]
        public void ConvertWithDithering(string fileName, DitheringKind? ditheringKind)
        {
            var expectedResult = _expectedResults[fileName][ditheringKind ?? DitheringKind.None];

            using var fileStream = new FileStream(Path.Combine("Assets", fileName), FileMode.Open, FileAccess.Read);

            var zplResult = fileName.EndsWith(".pdf")
                ? ditheringKind != null ? ConvertPdfPage(fileStream, 0, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed, DitheringKind: ditheringKind.Value)) : ConvertPdfPage(fileStream, 0, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed))
                : ditheringKind != null ? ConvertBitmap(fileStream, zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed, DitheringKind: ditheringKind.Value)) : ConvertBitmap(fileStream, zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed));

            Assert.AreEqual(expectedResult, zplResult.Replace("\n", string.Empty));
        }
    }
}