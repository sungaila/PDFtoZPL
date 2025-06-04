using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFtoZPL;
using System;
using System.IO;
using static PDFtoZPL.Conversion;

namespace Tests
{
    [TestClass]
    public partial class LabelLengthTests
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
#pragma warning disable IDE0079
#pragma warning disable CA1416
#pragma warning restore IDE0079
#endif

        [TestMethod]
        [DataRow("SocialPreview.pdf", null)]
        [DataRow("SocialPreview.pdf", false)]
        [DataRow("SocialPreview.pdf", true)]
        [DataRow("SocialPreview.png", null)]
        [DataRow("SocialPreview.png", false)]
        [DataRow("SocialPreview.png", true)]
        [DataRow("Wikimedia_Commons_web.pdf", null)]
        [DataRow("Wikimedia_Commons_web.pdf", false)]
        [DataRow("Wikimedia_Commons_web.pdf", true)]
        public void ConvertWithEncodings(string fileName, bool? setLabelLength)
        {
            var expectedResult = _expectedResults[fileName][setLabelLength ?? false];

            using var fileStream = new FileStream(Path.Combine("Assets", fileName), FileMode.Open, FileAccess.Read);

            var zplResult = fileName.EndsWith(".pdf")
                ? (setLabelLength != null ? ConvertPdfPage(fileStream, 0, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed, SetLabelLength: setLabelLength.Value)) : ConvertPdfPage(fileStream, 0, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed)))
                : (setLabelLength != null ? ConvertBitmap(fileStream, zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed, SetLabelLength: setLabelLength.Value)) : ConvertBitmap(fileStream, zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed)));

            Assert.AreEqual(expectedResult, zplResult.Replace("\n", string.Empty));
        }
    }
}