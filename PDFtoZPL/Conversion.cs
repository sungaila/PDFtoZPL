using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PDFtoZPL.ConversionUtils;

namespace PDFtoZPL
{
    /// <summary>
    /// Provides methods to convert PDFs and <see cref="SKBitmap"/>s into ZPL code.
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
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <param name="withAspectRatio">Specifies that width and height should be adjusted for aspect ratio if either is <see langword="null"/>.</param>
        /// <returns>The converted PDF page as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static string ConvertPdfPage(string pdfAsBase64String, string? password = null, int page = 0, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false, bool withAspectRatio = false)
        {
            if (pdfAsBase64String == null)
                throw new ArgumentNullException(nameof(pdfAsBase64String));

            return ConvertPdfPage(Convert.FromBase64String(pdfAsBase64String), password, page, dpi, width, height, withAnnotations, withFormFill, encodingKind, graphicFieldOnly, withAspectRatio);
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
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <param name="withAspectRatio">Specifies that width and height should be adjusted for aspect ratio if either is <see langword="null"/>.</param>
        /// <returns>The converted PDF page as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static string ConvertPdfPage(byte[] pdfAsByteArray, string? password = null, int page = 0, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false, bool withAspectRatio = false)
        {
            if (pdfAsByteArray == null)
                throw new ArgumentNullException(nameof(pdfAsByteArray));

            // Base64 string -> byte[] -> MemoryStream
            using var pdfStream = new MemoryStream(pdfAsByteArray, false);

            return ConvertPdfPage(pdfStream, password, page, dpi, width, height, withAnnotations, withFormFill, encodingKind, graphicFieldOnly, withAspectRatio);
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
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <param name="withAspectRatio">Specifies that width and height should be adjusted for aspect ratio if either is <see langword="null"/>.</param>
        /// <returns>The converted PDF page as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static string ConvertPdfPage(Stream pdfStream, string? password = null, int page = 0, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false, bool withAspectRatio = false)
        {
            return ConvertPdfPage(pdfStream, false, password, page, dpi, width, height, withAnnotations, withFormFill, encodingKind, graphicFieldOnly, withAspectRatio);
        }

        /// <summary>
        /// Converts a single page of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfStream">The PDF as a stream.</param>
        /// <param name="leaveOpen"><see langword="true"/> to leave the <paramref name="pdfStream"/> open after the PDF document is loaded; otherwise, <see langword="false"/>.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="page">The specific page to be converted.</param>
        /// <param name="dpi">The DPI scaling to use for rasterization of the PDF.</param>
        /// <param name="width">The width of the desired <paramref name="page"/>. Use <see langword="null"/> if the original width should be used.</param>
        /// <param name="height">The height of the desired <paramref name="page"/>. Use <see langword="null"/> if the original height should be used.</param>
        /// <param name="withAnnotations">Specifies whether annotations will be rendered.</param>
        /// <param name="withFormFill">Specifies whether form filling will be rendered.</param>
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <param name="withAspectRatio">Specifies that width and height should be adjusted for aspect ratio if either is <see langword="null"/>.</param>
        /// <returns>The converted PDF page as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static string ConvertPdfPage(Stream pdfStream, bool leaveOpen, string? password = null, int page = 0, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false, bool withAspectRatio = false)
        {
            if (pdfStream == null)
                throw new ArgumentNullException(nameof(pdfStream));

            if (page < 0)
                throw new ArgumentOutOfRangeException(nameof(page), "The page number must 0 or greater.");

            // Stream ->PdfiumViewer.PdfDocument -> Image
            var pdfBitmap = PDFtoImage.Conversion.ToImage(pdfStream, leaveOpen, password, page, dpi, width, height, withAnnotations, withFormFill, withAspectRatio);

            // Bitmap -> ZPL code
            return ConvertBitmap(pdfBitmap, encodingKind, graphicFieldOnly);
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
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <param name="withAspectRatio">Specifies that width and height should be adjusted for aspect ratio if either is <see langword="null"/>.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static IEnumerable<string> ConvertPdf(string pdfAsBase64String, string? password = null, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false, bool withAspectRatio = false)
        {
            if (pdfAsBase64String == null)
                throw new ArgumentNullException(nameof(pdfAsBase64String));

            foreach (var zplCode in ConvertPdf(Convert.FromBase64String(pdfAsBase64String), password, dpi, width, height, withAnnotations, withFormFill, encodingKind, graphicFieldOnly, withAspectRatio))
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
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <param name="withAspectRatio">Specifies that width and height should be adjusted for aspect ratio if either is <see langword="null"/>.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static IEnumerable<string> ConvertPdf(byte[] pdfAsByteArray, string? password = null, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false, bool withAspectRatio = false)
        {
            if (pdfAsByteArray == null)
                throw new ArgumentNullException(nameof(pdfAsByteArray));

            // Base64 string -> byte[] -> MemoryStream
            using var pdfStream = new MemoryStream(pdfAsByteArray, false);

            foreach (var zplCode in ConvertPdf(pdfStream, password, dpi, width, height, withAnnotations, withFormFill, encodingKind, graphicFieldOnly, withAspectRatio))
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
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <param name="withAspectRatio">Specifies that width and height should be adjusted for aspect ratio if either is <see langword="null"/>.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static IEnumerable<string> ConvertPdf(Stream pdfStream, string? password = null, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false, bool withAspectRatio = false)
        {
            return ConvertPdf(pdfStream, false, password, dpi, width, height, withAnnotations, withFormFill, encodingKind, graphicFieldOnly, withAspectRatio);
        }

        /// <summary>
        /// Converts all pages of a given PDF into ZPL code.
        /// </summary>
        /// <param name="pdfStream">The PDF as a stream.</param>
        /// <param name="leaveOpen"><see langword="true"/> to leave the <paramref name="pdfStream"/> open after the PDF document is loaded; otherwise, <see langword="false"/>.</param>
        /// <param name="password">The password for opening the PDF. Use <see langword="null"/> if no password is needed.</param>
        /// <param name="dpi">The DPI scaling to use for rasterization of the PDF.</param>
        /// <param name="width">The width of the all pages. Use <see langword="null"/> if the original width (per page) should be used.</param>
        /// <param name="height">The height of all pages. Use <see langword="null"/> if the original height (per page) should be used.</param>
        /// <param name="withAnnotations">Specifies whether annotations will be rendered.</param>
        /// <param name="withFormFill">Specifies whether form filling will be rendered.</param>
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <param name="withAspectRatio">Specifies that width and height should be adjusted for aspect ratio if either is <see langword="null"/>.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static IEnumerable<string> ConvertPdf(Stream pdfStream, bool leaveOpen, string? password = null, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false, bool withAspectRatio = false)
        {
            if (pdfStream == null)
                throw new ArgumentNullException(nameof(pdfStream));

            // Stream ->PdfiumViewer.PdfDocument -> Image
            foreach (var image in PDFtoImage.Conversion.ToImages(pdfStream, leaveOpen, password, dpi, width, height, withAnnotations, withFormFill, withAspectRatio))
            {
                // Bitmap -> ZPL code
                yield return ConvertBitmap(image, encodingKind, graphicFieldOnly);
            }
        }

#if NET6_0_OR_GREATER
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
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <param name="withAspectRatio">Specifies that width and height should be adjusted for aspect ratio if either is <see langword="null"/>.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the conversion.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
        public static async IAsyncEnumerable<string> ConvertPdfAsync(string pdfAsBase64String, string? password = null, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false, bool withAspectRatio = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (pdfAsBase64String == null)
                throw new ArgumentNullException(nameof(pdfAsBase64String));

            await foreach (var zplCode in ConvertPdfAsync(Convert.FromBase64String(pdfAsBase64String), password, dpi, width, height, withAnnotations, withFormFill, encodingKind, graphicFieldOnly, withAspectRatio, cancellationToken))
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
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <param name="withAspectRatio">Specifies that width and height should be adjusted for aspect ratio if either is <see langword="null"/>.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the conversion.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static async IAsyncEnumerable<string> ConvertPdfAsync(byte[] pdfAsByteArray, string? password = null, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false, bool withAspectRatio = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (pdfAsByteArray == null)
                throw new ArgumentNullException(nameof(pdfAsByteArray));

            // Base64 string -> byte[] -> MemoryStream
            using var pdfStream = new MemoryStream(pdfAsByteArray, false);

            await foreach (var zplCode in ConvertPdfAsync(pdfStream, password, dpi, width, height, withAnnotations, withFormFill, encodingKind, graphicFieldOnly, withAspectRatio, cancellationToken))
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
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <param name="withAspectRatio">Specifies that width and height should be adjusted for aspect ratio if either is <see langword="null"/>.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the conversion.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static async IAsyncEnumerable<string> ConvertPdfAsync(Stream pdfStream, string? password = null, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false, bool withAspectRatio = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var zplCode in ConvertPdfAsync(pdfStream, false, password, dpi, width, height, withAnnotations, withFormFill, encodingKind, graphicFieldOnly, withAspectRatio, cancellationToken))
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
        /// <param name="dpi">The DPI scaling to use for rasterization of the PDF.</param>
        /// <param name="width">The width of the all pages. Use <see langword="null"/> if the original width (per page) should be used.</param>
        /// <param name="height">The height of all pages. Use <see langword="null"/> if the original height (per page) should be used.</param>
        /// <param name="withAnnotations">Specifies whether annotations will be rendered.</param>
        /// <param name="withFormFill">Specifies whether form filling will be rendered.</param>
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <param name="withAspectRatio">Specifies that width and height should be adjusted for aspect ratio if either is <see langword="null"/>.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the conversion.</param>
        /// <returns>The converted PDF pages as ZPL code.</returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("macOS")]
        [SupportedOSPlatform("Android31.0")]
#endif
        public static async IAsyncEnumerable<string> ConvertPdfAsync(Stream pdfStream, bool leaveOpen, string? password = null, int dpi = 203, int? width = null, int? height = null, bool withAnnotations = false, bool withFormFill = false, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false, bool withAspectRatio = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (pdfStream == null)
                throw new ArgumentNullException(nameof(pdfStream));

            // Stream -> PdfiumViewer.PdfDocument -> Image
            await foreach (var image in PDFtoImage.Conversion.ToImagesAsync(pdfStream, leaveOpen, password, dpi, width, height, withAnnotations, withFormFill, withAspectRatio, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Bitmap -> ZPL code
                yield return await Task.Run(() => ConvertBitmap(image, encodingKind, graphicFieldOnly), cancellationToken);
            }
        }
#endif

        /// <summary>
        /// Converts a given image into ZPL code.
        /// </summary>
        /// <param name="bitmapPath">The file path of the image to convert.</param>
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <returns>The converted <see cref="SKBitmap"/> as ZPL code.</returns>
        public static string ConvertBitmap(string bitmapPath, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false)
        {
            return ConvertBitmap(SKBitmap.Decode(bitmapPath), encodingKind, graphicFieldOnly);
        }

        /// <summary>
        /// Converts a given <see cref="SKBitmap"/> into ZPL code.
        /// </summary>
        /// <param name="bitmapAsStream">The <see cref="SKBitmap"/> to convert.</param>
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <returns>The converted <see cref="SKBitmap"/> as ZPL code.</returns>
        public static string ConvertBitmap(Stream bitmapAsStream, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false)
        {
            return ConvertBitmap(bitmapAsStream, false, encodingKind, graphicFieldOnly);
        }

        /// <summary>
        /// Converts a given <see cref="SKBitmap"/> into ZPL code.
        /// </summary>
        /// <param name="bitmapAsStream">The <see cref="SKBitmap"/> to convert.</param>
        /// <param name="leaveOpen"><see langword="true"/> to leave the <paramref name="bitmapAsStream"/> open after the PDF document is loaded; otherwise, <see langword="false"/>.</param>
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <returns>The converted <see cref="SKBitmap"/> as ZPL code.</returns>
        public static string ConvertBitmap(Stream bitmapAsStream, bool leaveOpen, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false)
        {
            if (bitmapAsStream == null)
                throw new ArgumentNullException(nameof(bitmapAsStream));

            if (leaveOpen)
            {
                // SKBitmap.Decode will close the stream
                using var memoryStream = new MemoryStream();
                bitmapAsStream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                return ConvertBitmap(SKBitmap.Decode(memoryStream), encodingKind, graphicFieldOnly);
            }

            bitmapAsStream.Position = 0;
            return ConvertBitmap(SKBitmap.Decode(bitmapAsStream), encodingKind, graphicFieldOnly);
        }

        /// <summary>
        /// Converts a given image into ZPL code.
        /// </summary>
        /// <param name="bitmapAsByteArray">The image as byte array to convert.</param>
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <returns>The converted <see cref="SKBitmap"/> as ZPL code.</returns>
        public static string ConvertBitmap(byte[] bitmapAsByteArray, BitmapEncodingKind encodingKind = BitmapEncodingKind.HexadecimalCompressed, bool graphicFieldOnly = false)
        {
            return ConvertBitmap(SKBitmap.Decode(bitmapAsByteArray), encodingKind, graphicFieldOnly);
        }

        /// <summary>
        /// Converts a given <see cref="SKBitmap"/> into ZPL code.
        /// </summary>
        /// <param name="bitmap">The <see cref="SKBitmap"/> to convert.</param>
        /// <param name="encodingKind">The encoding used for embedding the bitmap.</param>
        /// <param name="graphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
        /// <param name="withAspectRatio">Specifies that width and height should be adjusted for aspect ratio if either is <see langword="null"/>.</param>
        /// <returns>The converted <see cref="SKBitmap"/> as ZPL code.</returns>
        internal static string ConvertBitmap(SKBitmap bitmap, BitmapEncodingKind encodingKind, bool graphicFieldOnly)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            return ConvertBitmapImpl(bitmap, encodingKind, graphicFieldOnly);
        }

        private static string ConvertBitmapImpl(SKBitmap pdfBitmap, BitmapEncodingKind encodingKind, bool graphicFieldOnly)
        {
            // first convert the bitmap into ZPL hex values (representing the bitmap)
            string bitmapAsHex = ConvertBitmapToHex(pdfBitmap, out int binaryByteCount, out int bytesPerRow);
            string bitmapPayload;

            if (encodingKind == BitmapEncodingKind.Hexadecimal)
            {
                bitmapPayload = bitmapAsHex;
            }
            else if (encodingKind == BitmapEncodingKind.HexadecimalCompressed)
            {
                bitmapPayload = CompressHex(bitmapAsHex, bytesPerRow);
            }
            else if (encodingKind == BitmapEncodingKind.Base64 || encodingKind == BitmapEncodingKind.Base64Compressed)
            {
                bitmapPayload = bitmapAsHex.Replace("\n", string.Empty);

                string encodingId = "B64";
                byte[] bitmapAsBytes = HexToByteArray(bitmapPayload);

                if (encodingKind == BitmapEncodingKind.Base64Compressed)
                {
                    encodingId = "Z64";
                    bitmapAsBytes = Deflate(bitmapAsBytes);
                }

                var base64 = Convert.ToBase64String(bitmapAsBytes);
                var csc = ComputeBitmapChecksum(base64);

                bitmapPayload = $":{encodingId}:{base64}:{csc}";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(encodingKind), $"Unknown {nameof(BitmapEncodingKind)} '{encodingKind}'.");
            }

            // build the Graphic Field (^GFA) command
            string graphicField = $"^GFA,{binaryByteCount},{binaryByteCount},{bytesPerRow},{bitmapPayload}";

            if (graphicFieldOnly)
                return graphicField;

            // finally return the complete ZPL code
            return $"^XA{graphicField}^FS^XZ";
        }

        /// <summary>
        /// The supported encoding options for the bitmap within the generated ZPL code.
        /// </summary>
        public enum BitmapEncodingKind
        {
            /// <summary>
            /// <b>Not recommended.</b> The bitmap is encoded as hexadecimals.<br/>
            /// Its output might be to large for the printer's bitmap storage area.
            /// </summary>
            Hexadecimal,

            /// <summary>
            /// The bitmap is encoded as hexadecimals and then compressed (via ZPL ASCII compression).<br/>
            /// It's significantly more space-saving than <see cref="Hexadecimal"/>.
            /// </summary>
            HexadecimalCompressed,

            /// <summary>
            /// <b>Not recommended.</b> The bitmap is encoded as Base64 (MIME). This encoding is referred to as <b>B64</b> in the ZPL II programming guide.<br/>
            /// Its output might be to large for the printer's bitmap storage area. Still more space-saving than <see cref="Hexadecimal"/> though.
            /// </summary>
            Base64,

            /// <summary>
            /// Recommended. The bitmap is compressed with Deflate (RFC 1951) and then encoded as Base64 (MIME). This encoding is referred to as <b>Z64</b> in the ZPL II programming guide.
            /// </summary>
            Base64Compressed
        }
    }
}