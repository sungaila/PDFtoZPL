namespace PDFtoZPL
{
    /// <summary>
    /// Contains all relevant information to render a PDF page into an image.
    /// </summary>
    /// <param name="EncodingKind">The encoding used for embedding the bitmap.</param>
    /// <param name="GraphicFieldOnly">If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.</param>
    /// <param name="SetLabelLength">If <see langword="true"/> then the returned ZPL sets the label length to the height of the image, using the ^LL command. Otherwise it returns ^XA^GF^FS^XZ.</param>
    /// <param name="Threshold">The threshold below which a pixel is considered black. Lower values mean darker, higher mean lighter.</param>
    /// <param name="DitheringKind">The dithering algorithm used when downsampling to a 1-bit monochrome image.</param>
    public readonly record struct ZplOptions(
        BitmapEncodingKind EncodingKind = BitmapEncodingKind.HexadecimalCompressed,
        bool GraphicFieldOnly = false,
        bool SetLabelLength = false,
        byte Threshold = 128,
        DitheringKind DitheringKind = DitheringKind.None)
    {
        /// <summary>
        /// Constructs <see cref="ZplOptions"/> with default values.
        /// </summary>
        public ZplOptions() : this(BitmapEncodingKind.HexadecimalCompressed, false, false, 128, DitheringKind.None) { }
    }
}
#if NETSTANDARD || MONOANDROID || NETFRAMEWORK
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
#endif