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

        /// <summary>
        /// Adds the ^PQ command to set the total quantity of labels to print. Accepted values are 1 to 99,999,999. Ignored if set to 0.
        /// </summary>
        uint PrintQuantity { get; init; }

        /// <summary>
        /// Adds the ^LT command to move the entire label up or down from its current position (in relation to the top edge of the label). Valid values are between <c>120</c> (move label down) and <c>-120</c> (move label up) dot rows. Values outside of this range are ignored and some printers might accept even fewer.
        /// </summary>
        sbyte LabelTop { get; init; }

        /// <summary>
        /// Adds the ^LS command to move the entire label left or right from its current position. Valid values are between <c>9999</c> (move label left) and <c>-9999</c> (move label right). Values outside of this range are ignored.
        /// </summary>
        short LabelShift { get; init; }
    }
}