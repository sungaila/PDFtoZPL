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
            Assert.ThrowsException<ArgumentNullException>(() => ConvertBitmap((Stream)null!));
        }

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