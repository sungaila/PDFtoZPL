using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFtoZPL;
using System;
using System.IO;
using static PDFtoZPL.Conversion;

namespace Tests
{
    [TestClass]
    public partial class LabelShiftTests
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
        [DataRow("SocialPreview.pdf", 0)]
        [DataRow("SocialPreview.pdf", 9999)]
        [DataRow("SocialPreview.pdf", -9999)]
        [DataRow("SocialPreview.pdf", (int)short.MaxValue)]
        [DataRow("SocialPreview.pdf", (int)short.MinValue)]
        [DataRow("SocialPreview.png", null)]
        [DataRow("SocialPreview.png", 0)]
        [DataRow("SocialPreview.png", 9999)]
        [DataRow("SocialPreview.png", -9999)]
        [DataRow("SocialPreview.png", (int)short.MaxValue)]
        [DataRow("SocialPreview.png", (int)short.MinValue)]
        [DataRow("Wikimedia_Commons_web.pdf", null)]
        [DataRow("Wikimedia_Commons_web.pdf", 0)]
        [DataRow("Wikimedia_Commons_web.pdf", 9999)]
        [DataRow("Wikimedia_Commons_web.pdf", -9999)]
        [DataRow("Wikimedia_Commons_web.pdf", (int)short.MaxValue)]
        [DataRow("Wikimedia_Commons_web.pdf", (int)short.MinValue)]
        public void ConvertWithEncodings(string fileName, int? labelShiftInput)
        {
            short? labelShift = (short?)labelShiftInput;
            var expectedResult = _expectedResults[fileName][labelShift ?? 0];

            using var fileStream = new FileStream(Path.Combine("Assets", fileName), FileMode.Open, FileAccess.Read);

            var zplResult = fileName.EndsWith(".pdf")
                ? (labelShift != null ? ConvertPdfPage(fileStream, 0, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed, LabelShift: labelShift.Value)) : ConvertPdfPage(fileStream, 0, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed)))
                : (labelShift != null ? ConvertBitmap(fileStream, zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed, LabelShift: labelShift.Value)) : ConvertBitmap(fileStream, zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed)));

            Assert.AreEqual(expectedResult, zplResult.Replace("\n", string.Empty));
        }
    }
}