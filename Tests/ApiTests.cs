using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.IO;
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
    }
}