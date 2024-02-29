namespace PDFtoZPL
{
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