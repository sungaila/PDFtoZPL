namespace PDFtoZPL
{
    /// <summary>
    /// The used dithering algorithem after downsampling to 1 bit monochrome.
    /// </summary>
    public enum DitheringKind
    {
        /// <summary>
        /// No dithering.
        /// </summary>
        None,

        /// <summary>
        /// Use the Robert W. Floyd and Louis Steinberg dithering algorithm.
        /// </summary>
        FloydSteinberg,

        /// <summary>
        /// Use the Bill Atkinson dithering algorithm.
        /// </summary>
        Atkinson
    }
}