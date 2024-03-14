namespace PDFtoZPL
{
    /// <summary>
    /// Contains all relevant information to render a PDF page into an image.
    /// </summary>
    public interface IZplOptions
    {
        /// <summary>
        /// The encoding used for embedding the bitmap.
        /// </summary>
        BitmapEncodingKind EncodingKind { get; init; }

        /// <summary>
        /// If <see langword="true"/> then only the ^GF part of the ZPL code is returned. Otherwise it returns ^XA^GF^FS^XZ.
        /// </summary>
        bool GraphicFieldOnly { get; init; }

        /// <summary>
        /// If <see langword="true"/> then the returned ZPL sets the label length to the height of the image, using the ^LL command. Otherwise it returns ^XA^GF^FS^XZ.
        /// </summary>
        bool SetLabelLength { get; init; }

        /// <summary>
        /// The threshold below which a pixel is considered black. Lower values mean darker, higher mean lighter.
        /// </summary>
        byte Threshold { get; init; }

        /// <summary>
        /// The dithering algorithm used when downsampling to a 1-bit monochrome image.
        /// </summary>
        DitheringKind DitheringKind { get; init; }
    }
}