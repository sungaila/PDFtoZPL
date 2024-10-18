using SkiaSharp;
using System;
using System.IO;
using System.Runtime.Versioning;

namespace PDFtoZPL
{
    /// <summary>
    /// Provides methods to convert PDFs and <see cref="SKBitmap"/>s into ZPL code.
    /// </summary>
#if NET8_0_OR_GREATER
#pragma warning disable CA1510 // Use ArgumentNullException throw helper
#endif
    public static partial class Conversion
    {
        /// <summary>
        /// Converts a single page of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfAsBase64String">The PDF encoded as Base64.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="page">The specific page to be converted.</param>
        /// <param name="pdfOptions">Additional options for PDF rendering.</param>
        /// <param name="zplOptions">Additional options for the ZPL output.</param>
        /// <returns>The converted PDF page as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("iOS13.6")]
        [SupportedOSPlatform("Android31.0")]
        [Obsolete("This method is deprecated and will be removed in a future release. Use SaveJpeg with a System.Index instead.")]
#endif
        public static string ConvertPdfPage(string pdfAsBase64String, string? password = null, int page = 0, PdfOptions pdfOptions = default, ZplOptions zplOptions = default)
        {
            if (pdfAsBase64String == null)
                throw new ArgumentNullException(nameof(pdfAsBase64String));

            return ConvertPdfPage(Convert.FromBase64String(pdfAsBase64String), password, page, pdfOptions, zplOptions);
        }

        /// <summary>
        /// Converts a single page of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfAsByteArray">The PDF as a byte array.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="page">The specific page to be converted.</param>
        /// <param name="pdfOptions">Additional options for PDF rendering.</param>
        /// <param name="zplOptions">Additional options for the ZPL output.</param>
        /// <returns>The converted PDF page as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("iOS13.6")]
        [SupportedOSPlatform("Android31.0")]
        [Obsolete("This method is deprecated and will be removed in a future release. Use SaveJpeg with a System.Index instead.")]
#endif
        public static string ConvertPdfPage(byte[] pdfAsByteArray, string? password = null, int page = 0, PdfOptions pdfOptions = default, ZplOptions zplOptions = default)
        {
            if (pdfAsByteArray == null)
                throw new ArgumentNullException(nameof(pdfAsByteArray));

            // Base64 string -> byte[] -> MemoryStream
            using var pdfStream = new MemoryStream(pdfAsByteArray, false);

            return ConvertPdfPage(pdfStream, false, password, page, pdfOptions, zplOptions);
        }

        /// <summary>
        /// Converts a single page of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfStream">The PDF as a stream.</param>
        /// <param name="leaveOpen"><see langword="true"/> to leave the <paramref name="pdfStream"/> open after the PDF document is loaded; otherwise, <see langword="false"/>.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="page">The specific page to be converted.</param>
        /// <param name="pdfOptions">Additional options for PDF rendering.</param>
        /// <param name="zplOptions">Additional options for the ZPL output.</param>
        /// <returns>The converted PDF page as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("iOS13.6")]
        [SupportedOSPlatform("Android31.0")]
        [Obsolete("This method is deprecated and will be removed in a future release. Use SaveJpeg with a System.Index instead.")]
#endif
        public static string ConvertPdfPage(Stream pdfStream, bool leaveOpen = false, string? password = null, int page = 0, PdfOptions pdfOptions = default, ZplOptions zplOptions = default)
        {
            if (pdfStream == null)
                throw new ArgumentNullException(nameof(pdfStream));

            if (pdfOptions == default)
                pdfOptions = new PdfOptions();

            // Stream ->PdfiumViewer.PdfDocument -> Image
            var pdfBitmap = PDFtoImage.Conversion.ToImage(pdfStream, leaveOpen, password, page, pdfOptions);

            // Bitmap -> ZPL code
            return ConvertBitmap(pdfBitmap, zplOptions);
        }
    }
}