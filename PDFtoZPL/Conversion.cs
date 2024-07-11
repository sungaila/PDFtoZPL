using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using static PDFtoZPL.ConversionUtils;

namespace PDFtoZPL
{
    /// <summary>
    /// Provides methods to convert PDFs and <see cref="SKBitmap"/>s into ZPL code.
    /// </summary>
#if NET8_0_OR_GREATER
#pragma warning disable CA1510 // Use ArgumentNullException throw helper
#endif
    public static class Conversion
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
        [SupportedOSPlatform("Android31.0")]
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
        [SupportedOSPlatform("Android31.0")]
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
        [SupportedOSPlatform("Android31.0")]
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

        /// <summary>
        /// Converts all pages of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfAsBase64String">The PDF encoded as Base64.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="pdfOptions">Additional options for PDF rendering.</param>
        /// <param name="zplOptions">Additional options for the ZPL output.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static IEnumerable<string> ConvertPdf(string pdfAsBase64String, string? password = null, PdfOptions pdfOptions = default, ZplOptions zplOptions = default)
        {
            if (pdfAsBase64String == null)
                throw new ArgumentNullException(nameof(pdfAsBase64String));

            foreach (var zplCode in ConvertPdf(Convert.FromBase64String(pdfAsBase64String), password, pdfOptions, zplOptions))
            {
                yield return zplCode;
            }
        }

        /// <summary>
        /// Converts all pages of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfAsByteArray">The PDF as a byte array.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="pdfOptions">Additional options for PDF rendering.</param>
        /// <param name="zplOptions">Additional options for the ZPL output.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static IEnumerable<string> ConvertPdf(byte[] pdfAsByteArray, string? password = null, PdfOptions pdfOptions = default, ZplOptions zplOptions = default)
        {
            if (pdfAsByteArray == null)
                throw new ArgumentNullException(nameof(pdfAsByteArray));

            // Base64 string -> byte[] -> MemoryStream
            using var pdfStream = new MemoryStream(pdfAsByteArray, false);

            foreach (var zplCode in ConvertPdf(pdfStream, false, password, pdfOptions, zplOptions))
            {
                yield return zplCode;
            }
        }

        /// <summary>
        /// Converts all pages of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfStream">The PDF as a stream.</param>
        /// <param name="leaveOpen"><see langword="true"/> to leave the <paramref name="pdfStream"/> open after the PDF document is loaded; otherwise, <see langword="false"/>.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="pdfOptions">Additional options for PDF rendering.</param>
        /// <param name="zplOptions">Additional options for the ZPL output.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static IEnumerable<string> ConvertPdf(Stream pdfStream, bool leaveOpen = false, string? password = null, PdfOptions pdfOptions = default, ZplOptions zplOptions = default)
        {
            if (pdfStream == null)
                throw new ArgumentNullException(nameof(pdfStream));

            if (pdfOptions == default)
                pdfOptions = new PdfOptions();

            // Stream ->PdfiumViewer.PdfDocument -> Image
            foreach (var image in PDFtoImage.Conversion.ToImages(pdfStream, leaveOpen, password, pdfOptions))
            {
                // Bitmap -> ZPL code
                yield return ConvertBitmap(image, zplOptions);
            }
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Converts all pages of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfAsBase64String">The PDF encoded as Base64.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="pdfOptions">Additional options for PDF rendering.</param>
        /// <param name="zplOptions">Additional options for the ZPL output.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the conversion.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static async IAsyncEnumerable<string> ConvertPdfAsync(string pdfAsBase64String, string? password = null, PdfOptions pdfOptions = default, ZplOptions zplOptions = default, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (pdfAsBase64String == null)
                throw new ArgumentNullException(nameof(pdfAsBase64String));

            await foreach (var zplCode in ConvertPdfAsync(Convert.FromBase64String(pdfAsBase64String), password, pdfOptions, zplOptions, cancellationToken))
            {
                yield return zplCode;
            }
        }

        /// <summary>
        /// Converts all pages of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfAsByteArray">The PDF as a byte array.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="pdfOptions">Additional options for PDF rendering.</param>
        /// <param name="zplOptions">Additional options for the ZPL output.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the conversion.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static async IAsyncEnumerable<string> ConvertPdfAsync(byte[] pdfAsByteArray, string? password = null, PdfOptions pdfOptions = default, ZplOptions zplOptions = default, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (pdfAsByteArray == null)
                throw new ArgumentNullException(nameof(pdfAsByteArray));

            // Base64 string -> byte[] -> MemoryStream
            using var pdfStream = new MemoryStream(pdfAsByteArray, false);

            await foreach (var zplCode in ConvertPdfAsync(pdfStream, false, password, pdfOptions, zplOptions, cancellationToken))
            {
                yield return zplCode;
            }
        }

        /// <summary>
        /// Converts all pages of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfStream">The PDF as a stream.</param>
        /// <param name="leaveOpen"><see langword="true"/> to leave the <paramref name="pdfStream"/> open after the PDF document is loaded; otherwise, <see langword="false"/>.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="pdfOptions">Additional options for PDF rendering.</param>
        /// <param name="zplOptions">Additional options for the ZPL output.</param>
        /// /// <param name="cancellationToken">The cancellation token to cancel the conversion.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static async IAsyncEnumerable<string> ConvertPdfAsync(Stream pdfStream, bool leaveOpen = false, string? password = null, PdfOptions pdfOptions = default, ZplOptions zplOptions = default, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (pdfStream == null)
                throw new ArgumentNullException(nameof(pdfStream));

            if (pdfOptions == default)
                pdfOptions = new PdfOptions();

            // Stream -> PdfiumViewer.PdfDocument -> Image
            await foreach (var image in PDFtoImage.Conversion.ToImagesAsync(pdfStream, leaveOpen, password, pdfOptions, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Bitmap -> ZPL code
                yield return await Task.Run(() => ConvertBitmap(image, zplOptions), cancellationToken);
            }
        }
#endif

        /// <summary>
        /// Converts a given image into ZPL code.
        /// </summary>
        /// <param name="bitmapPath">The file path of the image to convert.</param>
        /// <param name="zplOptions">Additional options for the ZPL output.</param>
        /// <returns>The converted <see cref="SKBitmap"/> as ZPL code.</returns>
        public static string ConvertBitmap(string bitmapPath, ZplOptions zplOptions = default)
        {
            using var bitmap = SKBitmap.Decode(bitmapPath);
            return ConvertBitmap(bitmap, zplOptions);
        }

        /// <summary>
        /// Converts a given <see cref="SKBitmap"/> into ZPL code.
        /// </summary>
        /// <param name="bitmapAsStream">The <see cref="SKBitmap"/> to convert.</param>
        /// <param name="leaveOpen"><see langword="true"/> to leave the <paramref name="bitmapAsStream"/> open after the PDF document is loaded; otherwise, <see langword="false"/>.</param>
        /// <param name="zplOptions">Additional options for the ZPL output.</param>
        /// <returns>The converted <see cref="SKBitmap"/> as ZPL code.</returns>
        public static string ConvertBitmap(Stream bitmapAsStream, bool leaveOpen = false, ZplOptions zplOptions = default)
        {
            if (bitmapAsStream == null)
                throw new ArgumentNullException(nameof(bitmapAsStream));

            if (leaveOpen)
            {
                // SKBitmap.Decode will close the stream
                using var memoryStream = new MemoryStream();
                bitmapAsStream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                using var bitmap = SKBitmap.Decode(memoryStream);
                return ConvertBitmap(bitmap, zplOptions);
            }

            bitmapAsStream.Position = 0;
            using var bitmap2 = SKBitmap.Decode(bitmapAsStream);
            return ConvertBitmap(bitmap2, zplOptions);
        }

        /// <summary>
        /// Converts a given image into ZPL code.
        /// </summary>
        /// <param name="bitmapAsByteArray">The image as byte array to convert.</param>
        /// <param name="zplOptions">Additional options for the ZPL output.</param>
        /// <returns>The converted <see cref="SKBitmap"/> as ZPL code.</returns>
        public static string ConvertBitmap(byte[] bitmapAsByteArray, ZplOptions zplOptions = default)
        {
            using var bitmap = SKBitmap.Decode(bitmapAsByteArray);
            return ConvertBitmap(bitmap, zplOptions);
        }

        /// <summary>
        /// Converts a given <see cref="SKBitmap"/> into ZPL code.
        /// </summary>
        /// <param name="bitmap">The <see cref="SKBitmap"/> to convert.</param>
        /// <param name="zplOptions">Additional options for the ZPL output.</param>
        /// <returns>The converted <see cref="SKBitmap"/> as ZPL code.</returns>
        public static string ConvertBitmap(SKBitmap bitmap, ZplOptions zplOptions = default)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            return ConvertBitmapImpl(bitmap, zplOptions);
        }

        private static string ConvertBitmapImpl(SKBitmap pdfBitmap, ZplOptions zplOptions)
        {
            SKBitmap inputBitmap = pdfBitmap;
            SKBitmap? bitmapReplacement = null;

            if (zplOptions == default)
                zplOptions = new();

            try
            {
                if (zplOptions.DitheringKind != DitheringKind.None)
                {
                    bitmapReplacement = pdfBitmap.ToMonochrome(zplOptions.Threshold, zplOptions.DitheringKind);
                    inputBitmap = bitmapReplacement;
                }

                // first convert the bitmap into ZPL hex values (representing the bitmap)
                string bitmapAsHex = ConvertBitmapToHex(inputBitmap, zplOptions.Threshold, out int binaryByteCount, out int bytesPerRow);
                string bitmapPayload;

                if (zplOptions.EncodingKind == BitmapEncodingKind.Hexadecimal)
                {
                    bitmapPayload = bitmapAsHex;
                }
                else if (zplOptions.EncodingKind == BitmapEncodingKind.HexadecimalCompressed)
                {
                    bitmapPayload = CompressHex(bitmapAsHex, bytesPerRow);
                }
                else if (zplOptions.EncodingKind == BitmapEncodingKind.Base64 || zplOptions.EncodingKind == BitmapEncodingKind.Base64Compressed)
                {
                    bitmapPayload = bitmapAsHex.Replace("\n", string.Empty);

                    string encodingId = "B64";
                    byte[] bitmapAsBytes = HexToByteArray(bitmapPayload);

                    if (zplOptions.EncodingKind == BitmapEncodingKind.Base64Compressed)
                    {
                        encodingId = "Z64";
                        bitmapAsBytes = Deflate(bitmapAsBytes);
                    }

                    var base64 = Convert.ToBase64String(bitmapAsBytes);
                    ushort csc = ComputeBitmapChecksum(base64);

                    bitmapPayload = $":{encodingId}:{base64}:{csc:X4}";
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(zplOptions), $"Unknown {nameof(BitmapEncodingKind)} '{zplOptions.EncodingKind}'.");
                }

                // build the Graphic Field (^GFA) command
                string graphicField = $"^GFA,{binaryByteCount},{binaryByteCount},{bytesPerRow},{bitmapPayload}";

                // return the ^GF and nothing else
                if (zplOptions.GraphicFieldOnly)
                    return graphicField;

                var zpl = $"^XA{graphicField}^FS^XZ";

                // set ^LL for continuous media (labels without gaps, spaces, notches, etc)
                if (zplOptions.SetLabelLength)
                    zpl = zpl.Replace("^XA", $"^XA^LL{inputBitmap.Height}");

                // set the ^PQ to control the number of labels to print
                if (zplOptions.PrintQuantity > 0)
                    zpl = zpl.Replace("^XZ", $"^PQ{Math.Min(zplOptions.PrintQuantity, 99999999)}^XZ");

                // finally return the complete ZPL code
                return zpl;
            }
            finally
            {
                bitmapReplacement?.Dispose();
            }
        }
    }
}