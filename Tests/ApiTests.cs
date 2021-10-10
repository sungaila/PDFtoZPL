using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
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
            Assert.ThrowsException<ArgumentNullException>(() => ConvertBitmap((Bitmap)null!));
        }

        [TestMethod]
        public void StreamNullException()
        {
#if NETCOREAPP3_0_OR_GREATER
            Assert.ThrowsException<ArgumentNullException>(() => ConvertBitmap((Stream)null!));
#else
            Assert.ThrowsException<ArgumentException>(() => ConvertBitmap((Stream)null!));
#endif
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

#if NETCOREAPP3_0_OR_GREATER
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
    }
}