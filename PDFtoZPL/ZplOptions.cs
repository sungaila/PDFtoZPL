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
    /// <param name="PrintQuantity">Adds the ^PQ command to set the total quantity of labels to print. Accepted values are 1 to 99,999,999. Ignored if set to 0.</param>
    /// <param name="LabelTop">Adds the ^LT command to move the entire label up or down from its current position (in relation to the top edge of the label). Valid values are between <c>120</c> (move label down) and <c>-120</c> (move label up) dot rows. Values outside of this range are ignored and some printers might accept even fewer.</param>
    /// <param name="LabelShift">Adds the ^LS command to move the entire label left or right from its current position. Valid values are between <c>9999</c> (move label left) and <c>-9999</c> (move label right). Values outside of this range are ignored.</param>
    public readonly record struct ZplOptions(
        BitmapEncodingKind EncodingKind = BitmapEncodingKind.HexadecimalCompressed,
        bool GraphicFieldOnly = false,
        bool SetLabelLength = false,
        byte Threshold = 128,
        DitheringKind DitheringKind = DitheringKind.None,
        uint PrintQuantity = 0,
        sbyte LabelTop = 0,
        short LabelShift = 0) : IZplOptions
    {
        /// <summary>
        /// Constructs <see cref="ZplOptions"/> with default values.
        /// </summary>
        public ZplOptions() : this(BitmapEncodingKind.HexadecimalCompressed, false, false, 128, DitheringKind.None, 0, 0, 0) { }
    }
}
#if NETSTANDARD || NETFRAMEWORK
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
#endif