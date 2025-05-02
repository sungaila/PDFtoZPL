using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFtoZPL;
using System;
using System.IO;
using static PDFtoZPL.Conversion;

namespace Tests
{
    [TestClass]
    public partial class BackgroundColorTests
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
        [DataRow("SocialPreview.pdf", null, DisplayName = "Default (White)")]
        [DataRow("SocialPreview.pdf", (uint)0xFFFFFFFF, DisplayName = "White")]
        [DataRow("SocialPreview.pdf", (uint)0x64FFFFFF, DisplayName = "White (100 alpha)")]
        [DataRow("SocialPreview.pdf", (uint)0xFFFF0000, DisplayName = "Red")]
        [DataRow("SocialPreview.pdf", (uint)0x64FF0000, DisplayName = "Red (100 alpha)")]
        [DataRow("SocialPreview.pdf", (uint)0xFF00FF00, DisplayName = "Green")]
        [DataRow("SocialPreview.pdf", (uint)0x6400FF00, DisplayName = "Green (100 alpha)")]
        [DataRow("SocialPreview.pdf", (uint)0xFF0000FF, DisplayName = "Blue")]
        [DataRow("SocialPreview.pdf", (uint)0x640000FF, DisplayName = "Blue (100 alpha)")]
        [DataRow("SocialPreview.pdf", (uint)0xFFFFFF00, DisplayName = "Yellow")]
        [DataRow("SocialPreview.pdf", (uint)0x64FFFF00, DisplayName = "Yellow (100 alpha)")]
        [DataRow("SocialPreview.pdf", (uint)0xFFFF00FF, DisplayName = "Magenta")]
        [DataRow("SocialPreview.pdf", (uint)0x64FF00FF, DisplayName = "Magenta (100 alpha)")]
        [DataRow("SocialPreview.pdf", (uint)0xFF00FFFF, DisplayName = "Cyan")]
        [DataRow("SocialPreview.pdf", (uint)0x6400FFFF, DisplayName = "Cyan (100 alpha)")]
        [DataRow("SocialPreview.pdf", (uint)0xFF000000, DisplayName = "Black")]
        [DataRow("SocialPreview.pdf", (uint)0x64000000, DisplayName = "Black (100 alpha)")]
        [DataRow("SocialPreview.pdf", (uint)0x00FFFFFF, DisplayName = "Transparent")]
        [DataRow("Wikimedia_Commons_web.pdf", null, DisplayName = "Default (White)")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0xFFFFFFFF, DisplayName = "White")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0x64FFFFFF, DisplayName = "White (100 alpha)")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0xFFFF0000, DisplayName = "Red")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0x64FF0000, DisplayName = "Red (100 alpha)")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0xFF00FF00, DisplayName = "Green")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0x6400FF00, DisplayName = "Green (100 alpha)")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0xFF0000FF, DisplayName = "Blue")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0x640000FF, DisplayName = "Blue (100 alpha)")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0xFFFFFF00, DisplayName = "Yellow")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0x64FFFF00, DisplayName = "Yellow (100 alpha)")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0xFFFF00FF, DisplayName = "Magenta")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0x64FF00FF, DisplayName = "Magenta (100 alpha)")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0xFF00FFFF, DisplayName = "Cyan")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0x6400FFFF, DisplayName = "Cyan (100 alpha)")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0xFF000000, DisplayName = "Black")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0x64000000, DisplayName = "Black (100 alpha)")]
        [DataRow("Wikimedia_Commons_web.pdf", (uint)0x00FFFFFF, DisplayName = "Transparent")]
        public void ConvertWithBackgroundColor(string fileName, uint? backgroundColor)
        {
            var expectedResult = _expectedResults[fileName][backgroundColor ?? 0xFFFFFFFF];

            using var fileStream = new FileStream(Path.Combine("Assets", fileName), FileMode.Open, FileAccess.Read);

            var zplResult
                = backgroundColor != null ? ConvertPdfPage(fileStream, 0, pdfOptions: new(Dpi: 203, BackgroundColor: backgroundColor.Value), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed)) : ConvertPdfPage(fileStream, 0, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: BitmapEncodingKind.Base64Compressed));

            Assert.AreEqual(expectedResult, zplResult.Replace("\n", string.Empty));
        }
    }
}