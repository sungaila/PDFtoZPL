using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFtoZPL;
using System;
using System.IO;
using static PDFtoZPL.Conversion;

namespace Tests
{
    [TestClass]
    public partial class LabelTopTests
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
        [DataRow("SocialPreview.pdf", 120)]
        [DataRow("SocialPreview.pdf", -120)]
        [DataRow("SocialPreview.pdf", (int)sbyte.MaxValue)]
        [DataRow("SocialPreview.pdf", (int)sbyte.MinValue)]
        [DataRow("SocialPreview.png", null)]
        [DataRow("SocialPreview.png", 0)]
        [DataRow("SocialPreview.png", 120)]
        [DataRow("SocialPreview.png", -120)]
        [DataRow("SocialPreview.png", (int)sbyte.MaxValue)]
        [DataRow("SocialPreview.png", (int)sbyte.MinValue)]
        [DataRow("Wikimedia_Commons_web.pdf", null)]
        [DataRow("Wikimedia_Commons_web.pdf", 0)]
        [DataRow("Wikimedia_Commons_web.pdf", 120)]
        [DataRow("Wikimedia_Commons_web.pdf", -120)]
        [DataRow("Wikimedia_Commons_web.pdf", (int)sbyte.MaxValue)]
        [DataRow("Wikimedia_Commons_web.pdf", (int)sbyte.MinValue)]
        public void ConvertWithEncodings(string fileName, int? labelTopInput)
        {
            sbyte? labelTop = (sbyte?)labelTopInput;
            var expectedResult = _expectedResults[fileName][labelTop ?? 0];

            using var fileStream = new FileStream(Path.Combine("Assets", fileName), FileMode.Open, FileAccess.Read);

            var zplResult = fileName.EndsWith(".pdf")
                ? (labelTop != null ? ConvertPdfPage(fileStream, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed, LabelTop: labelTop.Value)) : ConvertPdfPage(fileStream, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed)))
                : (labelTop != null ? ConvertBitmap(fileStream, zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed, LabelTop: labelTop.Value)) : ConvertBitmap(fileStream, zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed)));

            Assert.AreEqual(expectedResult, zplResult.Replace("\n", string.Empty));
        }
    }
}