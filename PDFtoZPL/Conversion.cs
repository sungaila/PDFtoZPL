using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PDFtoZPL
{
    /// <summary>
    /// Provides methods to convert PDFs and <see cref="Bitmap"/>s into ZPL code.
    /// </summary>
    public static class Conversion
    {
        /// <summary>
        /// Converts a single page of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfAsBase64String">The PDF encoded as Base64.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="page">The specific page to be converted.</param>
        /// <param name="dpi">The DPI scaling to use for rasterization of the PDF.</param>
        /// <param name="width">The width of the desired <paramref name="page"/>. Use <see langword="null"/> if the original width should be used.</param>
        /// <param name="height">The height of the desired <paramref name="page"/>. Use <see langword="null"/> if the original height should be used.</param>
        /// <param name="withAnnotations">Specifies whether annotations will be rendered.</param>
        /// <param name="withFormFill">Specifies whether form filling will be rendered.</param>
        /// <returns>The converted PDF page as ZPL code.</returns>
#if NET5_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
#endif
        public static string ConvertPdfPage(string pdfAsBase64String, string? password = null, int page = 0, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false)
        {
            if (pdfAsBase64String == null)
                throw new ArgumentNullException(nameof(pdfAsBase64String));

            return ConvertPdfPage(Convert.FromBase64String(pdfAsBase64String), password, page, dpi, width, height, withAnnotations, withFormFill);
        }

        /// <summary>
        /// Converts a single page of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfAsByteArray">The PDF as a byte array.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="page">The specific page to be converted.</param>
        /// <param name="dpi">The DPI scaling to use for rasterization of the PDF.</param>
        /// <param name="width">The width of the desired <paramref name="page"/>. Use <see langword="null"/> if the original width should be used.</param>
        /// <param name="height">The height of the desired <paramref name="page"/>. Use <see langword="null"/> if the original height should be used.</param>
        /// <param name="withAnnotations">Specifies whether annotations will be rendered.</param>
        /// <param name="withFormFill">Specifies whether form filling will be rendered.</param>
        /// <returns>The converted PDF page as ZPL code.</returns>
#if NET5_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
#endif
        public static string ConvertPdfPage(byte[] pdfAsByteArray, string? password = null, int page = 0, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false)
        {
            if (pdfAsByteArray == null)
                throw new ArgumentNullException(nameof(pdfAsByteArray));

            // Base64 string -> byte[] -> MemoryStream
            using var pdfStream = new MemoryStream(pdfAsByteArray, false);

            return ConvertPdfPage(pdfStream, password, page, dpi, width, height, withAnnotations, withFormFill);
        }

        /// <summary>
        /// Converts a single page of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfStream">The PDF as a stream.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="page">The specific page to be converted.</param>
        /// <param name="dpi">The DPI scaling to use for rasterization of the PDF.</param>
        /// <param name="width">The width of the desired <paramref name="page"/>. Use <see langword="null"/> if the original width should be used.</param>
        /// <param name="height">The height of the desired <paramref name="page"/>. Use <see langword="null"/> if the original height should be used.</param>
        /// <param name="withAnnotations">Specifies whether annotations will be rendered.</param>
        /// <param name="withFormFill">Specifies whether form filling will be rendered.</param>
        /// <returns>The converted PDF page as ZPL code.</returns>
#if NET5_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
#endif
        public static string ConvertPdfPage(Stream pdfStream, string? password = null, int page = 0, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false)
        {
            if (pdfStream == null)
                throw new ArgumentNullException(nameof(pdfStream));

            if (page < 0)
                throw new ArgumentOutOfRangeException(nameof(page), "The page number must 0 or greater.");

            // Stream ->PdfiumViewer.PdfDocument -> Image
            var pdfBitmap = PDFtoImage.Conversion.ToImage(pdfStream, password, page, dpi, width, height, withAnnotations, withFormFill);

            // Bitmap -> ZPL code
            return ConvertBitmap((Bitmap)pdfBitmap);
        }

        /// <summary>
        /// Converts all pages of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfAsBase64String">The PDF encoded as Base64.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="dpi">The DPI scaling to use for rasterization of the PDF.</param>
        /// <param name="width">The width of the all pages. Use <see langword="null"/> if the original width (per page) should be used.</param>
        /// <param name="height">The height of all pages. Use <see langword="null"/> if the original height (per page) should be used.</param>
        /// <param name="withAnnotations">Specifies whether annotations will be rendered.</param>
        /// <param name="withFormFill">Specifies whether form filling will be rendered.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET5_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
#endif
        public static IEnumerable<string> ConvertPdf(string pdfAsBase64String, string? password = null, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false)
        {
            if (pdfAsBase64String == null)
                throw new ArgumentNullException(nameof(pdfAsBase64String));

            foreach (var zplCode in ConvertPdf(Convert.FromBase64String(pdfAsBase64String), password, dpi, width, height, withAnnotations, withFormFill))
            {
                yield return zplCode;
            }
        }

        /// <summary>
        /// Converts all pages of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfAsByteArray">The PDF as a byte array.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="dpi">The DPI scaling to use for rasterization of the PDF.</param>
        /// <param name="width">The width of the all pages. Use <see langword="null"/> if the original width (per page) should be used.</param>
        /// <param name="height">The height of all pages. Use <see langword="null"/> if the original height (per page) should be used.</param>
        /// <param name="withAnnotations">Specifies whether annotations will be rendered.</param>
        /// <param name="withFormFill">Specifies whether form filling will be rendered.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET5_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
#endif
        public static IEnumerable<string> ConvertPdf(byte[] pdfAsByteArray, string? password = null, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false)
        {
            if (pdfAsByteArray == null)
                throw new ArgumentNullException(nameof(pdfAsByteArray));

            // Base64 string -> byte[] -> MemoryStream
            using var pdfStream = new MemoryStream(pdfAsByteArray, false);

            foreach (var zplCode in ConvertPdf(pdfStream, password, dpi, width, height, withAnnotations, withFormFill))
            {
                yield return zplCode;
            }
        }

        /// <summary>
        /// Converts all pages of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfStream">The PDF as a stream.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="dpi">The DPI scaling to use for rasterization of the PDF.</param>
        /// <param name="width">The width of the all pages. Use <see langword="null"/> if the original width (per page) should be used.</param>
        /// <param name="height">The height of all pages. Use <see langword="null"/> if the original height (per page) should be used.</param>
        /// <param name="withAnnotations">Specifies whether annotations will be rendered.</param>
        /// <param name="withFormFill">Specifies whether form filling will be rendered.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET5_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
#endif
        public static IEnumerable<string> ConvertPdf(Stream pdfStream, string? password = null, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false)
        {
            if (pdfStream == null)
                throw new ArgumentNullException(nameof(pdfStream));

            // Stream ->PdfiumViewer.PdfDocument -> Image
            foreach (var image in PDFtoImage.Conversion.ToImages(pdfStream, password, dpi, width, height, withAnnotations, withFormFill))
            {
                // Bitmap -> ZPL code
                yield return ConvertBitmap((Bitmap)image);
            }
        }

#if NETCOREAPP3_0_OR_GREATER
        /// <summary>
        /// Converts all pages of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfAsBase64String">The PDF encoded as Base64.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="dpi">The DPI scaling to use for rasterization of the PDF.</param>
        /// <param name="width">The width of the all pages. Use <see langword="null"/> if the original width (per page) should be used.</param>
        /// <param name="height">The height of all pages. Use <see langword="null"/> if the original height (per page) should be used.</param>
        /// <param name="withAnnotations">Specifies whether annotations will be rendered.</param>
        /// <param name="withFormFill">Specifies whether form filling will be rendered.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the conversion.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET5_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
#endif
        public static async IAsyncEnumerable<string> ConvertPdfAsync(string pdfAsBase64String, string? password = null, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (pdfAsBase64String == null)
                throw new ArgumentNullException(nameof(pdfAsBase64String));

            await foreach (var zplCode in ConvertPdfAsync(Convert.FromBase64String(pdfAsBase64String), password, dpi, width, height, withAnnotations, withFormFill, cancellationToken))
            {
                yield return zplCode;
            }
        }

        /// <summary>
        /// Converts all pages of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfAsByteArray">The PDF as a byte array.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="dpi">The DPI scaling to use for rasterization of the PDF.</param>
        /// <param name="width">The width of the all pages. Use <see langword="null"/> if the original width (per page) should be used.</param>
        /// <param name="height">The height of all pages. Use <see langword="null"/> if the original height (per page) should be used.</param>
        /// <param name="withAnnotations">Specifies whether annotations will be rendered.</param>
        /// <param name="withFormFill">Specifies whether form filling will be rendered.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the conversion.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET5_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
#endif
        public static async IAsyncEnumerable<string> ConvertPdfAsync(byte[] pdfAsByteArray, string? password = null, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (pdfAsByteArray == null)
                throw new ArgumentNullException(nameof(pdfAsByteArray));

            // Base64 string -> byte[] -> MemoryStream
            using var pdfStream = new MemoryStream(pdfAsByteArray, false);

            await foreach (var zplCode in ConvertPdfAsync(pdfStream, password, dpi, width, height, withAnnotations, withFormFill, cancellationToken))
            {
                yield return zplCode;
            }
        }

        /// <summary>
        /// Converts all pages of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfStream">The PDF as a stream.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="dpi">The DPI scaling to use for rasterization of the PDF.</param>
        /// <param name="width">The width of the all pages. Use <see langword="null"/> if the original width (per page) should be used.</param>
        /// <param name="height">The height of all pages. Use <see langword="null"/> if the original height (per page) should be used.</param>
        /// <param name="withAnnotations">Specifies whether annotations will be rendered.</param>
        /// <param name="withFormFill">Specifies whether form filling will be rendered.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the conversion.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET5_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
#endif
        public static async IAsyncEnumerable<string> ConvertPdfAsync(Stream pdfStream, string? password = null, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (pdfStream == null)
                throw new ArgumentNullException(nameof(pdfStream));

            // Stream -> PdfiumViewer.PdfDocument -> Image
            await foreach (var image in PDFtoImage.Conversion.ToImagesAsync(pdfStream, password, dpi, width, height, withAnnotations, withFormFill, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Bitmap -> ZPL code
                yield return await Task.Run(() => ConvertBitmap((Bitmap)image), cancellationToken);
            }
        }
#endif

        /// <summary>
        /// Converts a given <see cref="Bitmap"/> into ZPL code.
        /// </summary>
        /// <param name="bitmapAsStream">The <see cref="Bitmap"/> to convert.</param>
        /// <returns>The converted <see cref="Bitmap"/> as ZPL code.</returns>
        public static string ConvertBitmap(Stream bitmapAsStream)
        {
            return ConvertBitmap(new Bitmap(bitmapAsStream));
        }

        /// <summary>
        /// Converts a given <see cref="Bitmap"/> into ZPL code.
        /// </summary>
        /// <param name="bitmap">The <see cref="Bitmap"/> to convert.</param>
        /// <returns>The converted <see cref="Bitmap"/> as ZPL code.</returns>
        public static string ConvertBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            return ConvertBitmapImpl(bitmap);
        }

        private static string ConvertBitmapImpl(Bitmap pdfBitmap)
        {
            // first convert the bitmap into ZPL hex values (representing the bitmap)
            string bitmapAsHex = ConvertBitmapToHex(pdfBitmap, out int binaryByteCount, out int bytesPerRow);

            // then compress the hex values to reduce its size
            bitmapAsHex = CompressHex(bitmapAsHex, bytesPerRow);

            // build the Graphic Field (^GFA) command
            string graphicField = $"^GFA,{binaryByteCount},{binaryByteCount},{bytesPerRow},{bitmapAsHex}^FS";

            // finally return the complete ZPL code
            return $"^XA{graphicField}^XZ";
        }

        private static string ConvertBitmapToHex(Bitmap pdfBitmap, out int binaryByteCount, out int bytesPerRow)
        {
            var zplBuilder = new StringBuilder();

            bytesPerRow = pdfBitmap.Width % 8 > 0
                ? pdfBitmap.Width / 8 + 1
                : pdfBitmap.Width / 8;
            binaryByteCount = pdfBitmap.Height * bytesPerRow;

            int colorBits = 0;
            int j = 0;

            int width = pdfBitmap.Width;
            int height = pdfBitmap.Height;

            BitmapData? data = null;
            byte[] rgbValues = Array.Empty<byte>();
            int stride;

            try
            {
                data = pdfBitmap.LockBits(new Rectangle(Point.Empty, pdfBitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                stride = data.Stride;
                int bytes = stride * pdfBitmap.Height;
                rgbValues = new byte[bytes];
                Marshal.Copy(data.Scan0, rgbValues, 0, bytes);
            }
            finally
            {
                if (data != null)
                    pdfBitmap.UnlockBits(data);
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte red = rgbValues[y * stride + x * 3];
                    byte green = rgbValues[y * stride + x * 3 + 1];
                    byte blue = rgbValues[y * stride + x * 3 + 2];

                    bool blackPixel = ((red + green + blue) / 3) < 128;

                    if (blackPixel)
                        colorBits |= 1 << (7 - j);

                    j++;

                    if (j == 8 || x == (width - 1))
                    {
                        zplBuilder.Append(colorBits.ToString("X2"));
                        colorBits = 0;
                        j = 0;
                    }
                }
                zplBuilder.Append('\n');
            }

            return zplBuilder.ToString();
        }

        private static string CompressHex(string code, int widthBytes)
        {
            int maxlinea = widthBytes * 2;
            var sbCode = new StringBuilder();
            var sbLinea = new StringBuilder();
            string? previousLine = null;
            int counter = 0;
            char aux = code[0];
            bool firstChar = false;

            foreach (char item in code)
            {
                if (firstChar)
                {
                    aux = item;
                    firstChar = false;
                    continue;
                }
                if (item == '\n')
                {
                    if (counter >= maxlinea && aux == '0')
                    {
                        sbLinea.Append(',');
                    }
                    else if (counter >= maxlinea && aux == 'F')
                    {
                        sbLinea.Append('!');
                    }
                    else if (counter > 20)
                    {
                        int multi20 = (counter / 20) * 20;
                        sbLinea.Append(CompressionCountMapping[Math.Min(multi20, 400)]);

                        int restover400 = multi20 / 400;
                        if (restover400 > 0)
                        {
                            for (; restover400 > 1; restover400--)
                                sbLinea.Append(CompressionCountMapping[400]);

                            int restto400 = (counter % 400) / 20 * 20;

                            if (restto400 > 0)
                                sbLinea.Append(CompressionCountMapping[restto400]);
                        }

                        int resto20 = (counter % 20);

                        if (resto20 != 0)
                            sbLinea.Append(CompressionCountMapping[resto20]).Append(aux);
                        else
                            sbLinea.Append(aux);
                    }
                    else
                    {
                        sbLinea.Append(CompressionCountMapping[counter]).Append(aux);
                    }
                    counter = 1;
                    firstChar = true;

                    if (sbLinea.ToString().Equals(previousLine))
                        sbCode.Append(':');
                    else
                        sbCode.Append(sbLinea);

                    previousLine = sbLinea.ToString();
                    sbLinea.Length = 0;
                    continue;
                }

                if (aux == item)
                {
                    counter++;
                }
                else
                {
                    if (counter > 20)
                    {
                        int multi20 = (counter / 20) * 20;
                        sbLinea.Append(CompressionCountMapping[Math.Min(multi20, 400)]);

                        int restover400 = multi20 / 400;
                        if (restover400 > 0)
                        {
                            for (; restover400 > 1; restover400--)
                                sbLinea.Append(CompressionCountMapping[400]);

                            int restto400 = (counter % 400) / 20 * 20;

                            if (restto400 > 0)
                                sbLinea.Append(CompressionCountMapping[restto400]);
                        }

                        int resto20 = (counter % 20);

                        if (resto20 != 0)
                            sbLinea.Append(CompressionCountMapping[resto20]).Append(aux);
                        else
                            sbLinea.Append(aux);
                    }
                    else
                    {
                        sbLinea.Append(CompressionCountMapping[counter]).Append(aux);
                    }
                    counter = 1;
                    aux = item;
                }
            }

            return sbCode.ToString();
        }

        /// <summary>
        /// The mapping table used for compression.
        /// Each character count (the key) is represented by a certain char (the value).
        /// </summary>
        private static readonly Dictionary<int, string> CompressionCountMapping = new()
        {
            { 1, "G" },
            { 2, "H" },
            { 3, "I" },
            { 4, "J" },
            { 5, "K" },
            { 6, "L" },
            { 7, "M" },
            { 8, "N" },
            { 9, "O" },
            { 10, "P" },
            { 11, "Q" },
            { 12, "R" },
            { 13, "S" },
            { 14, "T" },
            { 15, "U" },
            { 16, "V" },
            { 17, "W" },
            { 18, "X" },
            { 19, "Y" },
            { 20, "g" },
            { 40, "h" },
            { 60, "i" },
            { 80, "j" },
            { 100, "k" },
            { 120, "l" },
            { 140, "m" },
            { 160, "n" },
            { 180, "o" },
            { 200, "p" },
            { 220, "q" },
            { 240, "r" },
            { 260, "s" },
            { 280, "t" },
            { 300, "u" },
            { 320, "v" },
            { 340, "w" },
            { 360, "x" },
            { 380, "y" },
            { 400, "z" }
        };
    }
}