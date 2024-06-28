using PDFtoImage;
using SkiaSharp;
using System.Drawing;

namespace PDFtoZPL
{
    /// <summary>
    /// Contains all relevant information to render a PDF page into an image.
    /// </summary>
    /// <param name="Dpi">The DPI scaling to use for rasterization of the PDF.</param>
    /// <param name="Width">The width of the desired page. Use <see langword="null"/> if the original width should be used.</param>
    /// <param name="Height">The height of the desired page. Use <see langword="null"/> if the original height should be used.</param>
    /// <param name="WithAnnotations">Specifies whether annotations be rendered.</param>
    /// <param name="WithFormFill">Specifies whether form filling will be rendered.</param>
    /// <param name="WithAspectRatio">Specifies that <paramref name="Width"/> or <paramref name="Height"/> should be adjusted for aspect ratio (either one must be <see langword="null"/>).</param>
    /// <param name="Rotation">Specifies the rotation at 90 degree intervals.</param>
    /// <param name="AntiAliasing">Specifies which parts of the PDF should be anti-aliasing for rendering.</param>
    /// <param name="BackgroundColor">Specifies the background color. Defaults to <see cref="SKColors.White"/>.</param>
    /// <param name="Bounds">Specifies the bounds for the page relative to <see cref="PDFtoImage.Conversion.GetPageSizes(string,string)"/>. This can be used for clipping (bounds inside of page) or additional margins (bounds outside of page). The bound units are relative to the PDF size (at 72 DPI).</param>
    /// <param name="UseTiling">Specifies that the PDF should be rendered as several segments and merged into the final image. This can help in cases where the output image is too large, causing corrupted images (e.g. missing text) or crashes.</param>
    /// <param name="DpiRelativeToBounds">Specifies that <see cref="Dpi"/> and <see cref="WithAspectRatio"/> will be calculated relative to <see cref="Bounds"/> instead of the original PDF.</param>
    public readonly record struct PdfOptions(
        int Dpi = 203,
        int? Width = null,
        int? Height = null,
        bool WithAnnotations = false,
        bool WithFormFill = false,
        bool WithAspectRatio = false,
        PdfRotation Rotation = PdfRotation.Rotate0,
        PdfAntiAliasing AntiAliasing = PdfAntiAliasing.All,
        SKColor? BackgroundColor = null,
        RectangleF? Bounds = null,
        bool UseTiling = false,
        bool DpiRelativeToBounds = false) : IRenderOptions
    {
        /// <summary>
        /// Constructs <see cref="PdfOptions"/> with default values.
        /// </summary>
        public PdfOptions() : this(203, null, null, false, false, false, PdfRotation.Rotate0, PdfAntiAliasing.All, null, null, false, false) { }

        /// <summary>
        /// Implicit conversion to <see cref="PDFtoImage.RenderOptions"/>.
        /// </summary>
        /// <param name="pdfOptions">The options to convert.</param>
        public static implicit operator RenderOptions(PdfOptions pdfOptions) => new(
                pdfOptions.Dpi,
                pdfOptions.Width,
                pdfOptions.Height,
                pdfOptions.WithAnnotations,
                pdfOptions.WithFormFill,
                pdfOptions.WithAspectRatio,
                pdfOptions.Rotation,
                pdfOptions.AntiAliasing,
                pdfOptions.BackgroundColor,
                pdfOptions.Bounds,
                pdfOptions.UseTiling,
                pdfOptions.DpiRelativeToBounds);
    }
}