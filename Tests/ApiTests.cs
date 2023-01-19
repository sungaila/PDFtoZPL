using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static PDFtoZPL.Conversion;

namespace Tests
{
    [TestClass]
    public class ApiTests
    {
        [TestMethod]
        public void BitmapNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ConvertBitmap((Stream)null!));
        }

#pragma warning disable CA1416
        [TestMethod]
        public void PdfStreamNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ConvertPdfPage((Stream)null!));
        }

        [TestMethod]
        public void PdfStringNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ConvertPdfPage((string)null!));
        }

        [TestMethod]
        public void PdfByteArrayNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ConvertPdfPage((byte[])null!));
        }

        [TestMethod]
        public void PageNumberException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => ConvertPdfPage(string.Empty, page: -1));
        }

        [TestMethod]
        public void PdfAllStreamNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ConvertPdf((Stream)null!).ToList());
        }

        [TestMethod]
        public void PdfAllStringNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ConvertPdf((string)null!).ToList());
        }

        [TestMethod]
        public void PdfAllByteArrayNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ConvertPdf((byte[])null!).ToList());
        }

#if NET6_0_OR_GREATER
        [TestMethod]
        public async Task PdfAllAsyncStreamNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await foreach (var zplCode in ConvertPdfAsync((Stream)null!))
                {
                }
            });
        }

        [TestMethod]
        public async Task PdfAllAsyncStringNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await foreach (var zplCode in ConvertPdfAsync((string)null!))
                {
                }
            });
        }

        [TestMethod]
        public async Task PdfAllAsyncByteArrayNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await foreach (var zplCode in ConvertPdfAsync((byte[])null!))
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

            ConvertPdfPage(fileStream, false, page: 0);
            Assert.IsFalse(fileStream.CanRead, "The stream should be closed when calling leaveOpen with false.");
        }

        [TestMethod]
        public void ConvertPdfPageLeaveOpenTrue()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.pdf"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            ConvertPdfPage(fileStream, true, page: 0);
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

            var result = ConvertPdfAsync(fileStream);
            Assert.IsTrue(fileStream.CanRead, "The stream should be open as long as the iterator is not used yet.");

            await foreach (var _ in result) ;
            Assert.IsFalse(fileStream.CanRead, "The stream should be closed when calling the default overload.");
        }

        [TestMethod]
        public async Task ConvertPdfAsyncLeaveOpenFalse()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.pdf"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            var result = ConvertPdfAsync(fileStream, false);
            Assert.IsTrue(fileStream.CanRead, "The stream should be open as long as the iterator is not used yet.");

            await foreach (var _ in result) ;
            Assert.IsFalse(fileStream.CanRead, "The stream should be closed when calling leaveOpen with false.");
        }

        [TestMethod]
        public async Task ConvertPdfAsyncLeaveOpenTrue()
        {
            using var fileStream = new FileStream(Path.Combine("Assets", "SocialPreview.pdf"), FileMode.Open, FileAccess.Read);
            Assert.IsTrue(fileStream.CanRead);

            var result = ConvertPdfAsync(fileStream, true);
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