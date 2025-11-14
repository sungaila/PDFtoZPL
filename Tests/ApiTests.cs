using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static PDFtoZPL.Conversion;

namespace PDFtoZPL.Tests
{
    [TestClass]
    public class ApiTests : TestBase
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
        public void BitmapNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => ConvertBitmap((Stream)null!));
        }

#if NET6_0_OR_GREATER
#pragma warning disable CA1416
#endif
        [TestMethod]
        public void PdfStreamNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => ConvertPdfPage((Stream)null!, 0));
        }

        [TestMethod]
        public void PdfStringNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => ConvertPdfPage((string)null!, 0));
        }

        [TestMethod]
        public void PdfByteArrayNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => ConvertPdfPage((byte[])null!, 0));
        }

#if NET6_0_OR_GREATER
        [TestMethod]
        public void PageNumberException()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ConvertPdfPage(string.Empty, -1));
        }
#endif

        [TestMethod]
        public void PdfAllStreamNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => ConvertPdf((Stream)null!).ToList());
        }

        [TestMethod]
        public void PdfAllStringNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => ConvertPdf((string)null!).ToList());
        }

        [TestMethod]
        public void PdfAllByteArrayNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => ConvertPdf((byte[])null!).ToList());
        }

#if NET6_0_OR_GREATER
        [TestMethod]
        public async Task PdfAllAsyncStreamNullException()
        {
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
            {
                await foreach (var zplCode in ConvertPdfAsync((Stream)null!, cancellationToken: TestContext!.CancellationToken))
                {
                }
            });
        }

        [TestMethod]
        public async Task PdfAllAsyncStringNullException()
        {
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
            {
                await foreach (var zplCode in ConvertPdfAsync((string)null!, cancellationToken: TestContext!.CancellationToken))
                {
                }
            });
        }

        [TestMethod]
        public async Task PdfAllAsyncByteArrayNullException()
        {
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
            {
                await foreach (var zplCode in ConvertPdfAsync((byte[])null!, cancellationToken: TestContext!.CancellationToken))
                {
                }
            });
        }
#endif

        [TestMethod]
        public void ConvertPdfPageLeaveOpenDefault()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.pdf"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            ConvertPdfPage(fileStream, page: 0);
            Assert.IsFalse(fileStream.CanRead, "The stream should be closed when calling the default overload.");
        }

        [TestMethod]
        public void ConvertPdfPageLeaveOpenFalse()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.pdf"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            ConvertPdfPage(fileStream, 0, false);
            Assert.IsFalse(fileStream.CanRead, "The stream should be closed when calling leaveOpen with false.");
        }

        [TestMethod]
        public void ConvertPdfPageLeaveOpenTrue()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.pdf"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            ConvertPdfPage(fileStream, 0, true);
            Assert.IsTrue(fileStream.CanRead, "The stream should be open when calling leaveOpen with true.");
        }

        [TestMethod]
        public void ConvertPdfLeaveOpenDefault()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.pdf"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            var result = ConvertPdf(fileStream);
            Assert.IsTrue(fileStream.CanRead, "The stream should be open as long as the iterator is not used yet.");

            foreach (var _ in result) ;
            Assert.IsFalse(fileStream.CanRead, "The stream should be closed when calling the default overload.");
        }

        [TestMethod]
        public void ConvertPdfLeaveOpenFalse()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.pdf"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            var result = ConvertPdf(fileStream, false);
            Assert.IsTrue(fileStream.CanRead, "The stream should be open as long as the iterator is not used yet.");

            foreach (var _ in result) ;
            Assert.IsFalse(fileStream.CanRead, "The stream should be closed when calling leaveOpen with false.");
        }

        [TestMethod]
        public void ConvertPdfLeaveOpenTrue()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.pdf"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            var result = ConvertPdf(fileStream, true);
            Assert.IsTrue(fileStream.CanRead, "The stream should be open as long as the iterator is not used yet.");

            foreach (var _ in result) ;
            Assert.IsTrue(fileStream.CanRead, "The stream should be open when calling leaveOpen with true.");
        }

#if NET6_0_OR_GREATER
        [TestMethod]
        public async Task ConvertPdfAsyncLeaveOpenDefault()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.pdf"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            var result = ConvertPdfAsync(fileStream, cancellationToken: TestContext!.CancellationToken);
            Assert.IsTrue(fileStream.CanRead, "The stream should be open as long as the iterator is not used yet.");

            await foreach (var _ in result) ;
            Assert.IsFalse(fileStream.CanRead, "The stream should be closed when calling the default overload.");
        }

        [TestMethod]
        public async Task ConvertPdfAsyncLeaveOpenFalse()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.pdf"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            var result = ConvertPdfAsync(fileStream, false, cancellationToken: TestContext!.CancellationToken);
            Assert.IsTrue(fileStream.CanRead, "The stream should be open as long as the iterator is not used yet.");

            await foreach (var _ in result) ;
            Assert.IsFalse(fileStream.CanRead, "The stream should be closed when calling leaveOpen with false.");
        }

        [TestMethod]
        public async Task ConvertPdfAsyncLeaveOpenTrue()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.pdf"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            var result = ConvertPdfAsync(fileStream, true, cancellationToken: TestContext!.CancellationToken);
            Assert.IsTrue(fileStream.CanRead, "The stream should be open as long as the iterator is not used yet.");

            await foreach (var _ in result) ;
            Assert.IsTrue(fileStream.CanRead, "The stream should be open when calling leaveOpen with true.");
        }
#endif

        [TestMethod]
        public void ConvertBitmapLeaveOpenDefault()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.png"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            ConvertBitmap(fileStream);
            Assert.IsFalse(fileStream.CanRead, "The stream should be closed when calling the default overload.");
        }

        [TestMethod]
        public void ConvertBitmapLeaveOpenFalse()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.png"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            ConvertBitmap(fileStream, false);
            Assert.IsFalse(fileStream.CanRead, "The stream should be closed when calling leaveOpen with false.");
        }

        [TestMethod]
        public void ConvertBitmapLeaveOpenTrue()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.png"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            ConvertBitmap(fileStream, true);
            Assert.IsTrue(fileStream.CanRead, "The stream should be open when calling leaveOpen with true.");
        }
    }
}