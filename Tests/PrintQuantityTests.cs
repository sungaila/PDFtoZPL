using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFtoZPL;
using System;
using System.IO;
using static PDFtoZPL.Conversion;

namespace Tests
{
    [TestClass]
    public partial class PrintQuantityTests
    {
        [TestInitialize]
        public void Initialize()
        {
#if NET6_0_OR_GREATER
            if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
                Assert.Inconclusive("This test must run on Windows, Linux or macOS.");
#endif
        }

#pragma warning disable CA1416

        [TestMethod]
        [DataRow("SocialPreview.pdf", null)]
        [DataRow("SocialPreview.pdf", 0u)]
        [DataRow("SocialPreview.pdf", 1u)]
        [DataRow("SocialPreview.pdf", 99999999u)]
        [DataRow("SocialPreview.pdf", 100000000u)]
        [DataRow("SocialPreview.png", null)]
        [DataRow("SocialPreview.png", 0u)]
        [DataRow("SocialPreview.png", 1u)]
        [DataRow("SocialPreview.png", 99999999u)]
        [DataRow("SocialPreview.png", 100000000u)]
        [DataRow("Wikimedia_Commons_web.pdf", null)]
        [DataRow("Wikimedia_Commons_web.pdf", 0u)]
        [DataRow("Wikimedia_Commons_web.pdf", 1u)]
        [DataRow("Wikimedia_Commons_web.pdf", 99999999u)]
        [DataRow("Wikimedia_Commons_web.pdf", 100000000u)]
        public void ConvertWithEncodings(string fileName, uint? printQuantity)
        {
            var expectedResult = _expectedResults[fileName][Math.Min(printQuantity ?? 0, 99999999)];

            using var fileStream = new FileStream(Path.Combine("Assets", fileName), FileMode.Open, FileAccess.Read);

            var zplResult = fileName.EndsWith(".pdf")
                ? (printQuantity != null ? ConvertPdfPage(fileStream, 0, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed, PrintQuantity: printQuantity.Value)) : ConvertPdfPage(fileStream, 0, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed)))
                : (printQuantity != null ? ConvertBitmap(fileStream, zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed, PrintQuantity: printQuantity.Value)) : ConvertBitmap(fileStream, zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed)));

            Assert.AreEqual(expectedResult, zplResult.Replace("\n", string.Empty));
        }
    }
}