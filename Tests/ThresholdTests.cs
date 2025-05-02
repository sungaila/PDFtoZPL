using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFtoZPL;
using System;
using System.IO;
using static PDFtoZPL.Conversion;

namespace Tests
{
    [TestClass]
    public partial class ThresholdTests
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
        [DataRow("SocialPreview.pdf", (byte)128)]
        [DataRow("SocialPreview.pdf", (byte)0)]
        [DataRow("SocialPreview.pdf", (byte)50)]
        [DataRow("SocialPreview.pdf", (byte)205)]
        [DataRow("SocialPreview.pdf", (byte)255)]
        [DataRow("Wikimedia_Commons_web.pdf", null)]
        [DataRow("Wikimedia_Commons_web.pdf", (byte)128)]
        [DataRow("Wikimedia_Commons_web.pdf", (byte)0)]
        [DataRow("Wikimedia_Commons_web.pdf", (byte)50)]
        [DataRow("Wikimedia_Commons_web.pdf", (byte)205)]
        [DataRow("Wikimedia_Commons_web.pdf", (byte)255)]
        [DataRow("SocialPreview.png", null)]
        [DataRow("SocialPreview.png", (byte)128)]
        [DataRow("SocialPreview.png", (byte)0)]
        [DataRow("SocialPreview.png", (byte)50)]
        [DataRow("SocialPreview.png", (byte)205)]
        [DataRow("SocialPreview.png", (byte)255)]
        public void ConvertWithThreshold(string fileName, byte? threshold)
        {
            var expectedResult = _expectedResults[fileName][threshold ?? 128];

            using var fileStream = new FileStream(Path.Combine("Assets", fileName), FileMode.Open, FileAccess.Read);

            var zplResult = fileName.EndsWith(".pdf")
                ? threshold != null ? ConvertPdfPage(fileStream, 0, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed, Threshold: threshold.Value)) : ConvertPdfPage(fileStream, 0, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed))
                : threshold != null ? ConvertBitmap(fileStream, zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed, Threshold: threshold.Value)) : ConvertBitmap(fileStream, zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed));

            Assert.AreEqual(expectedResult, zplResult.Replace("\n", string.Empty));
        }
    }
}