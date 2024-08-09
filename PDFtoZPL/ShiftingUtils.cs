using SkiaSharp;
using System;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("PDFtoZPL.WebConverter, PublicKey=0024000004800000940000000602000000240000525341310004000001000100fd0018feed1ba4fac91744d868cb1bb8a4b55a97eec0e02e90a8d57db56bff5a32f03813b1a6c8a7ccd50eed8880be4e91ad281c9ee81fd4182de0905c0590019e9bf87a8f83ce93c09da9c4eaae5bd8aa63816c4a3bef8bd16bb358d3aed64cc2ec3ae17698c336b63df4fbd719dc13bd9a88d9fcfa87a2426d34db78d05fb7")]

namespace PDFtoZPL
{
    internal static class ShiftingUtils
    {
        public static SKBitmap ApplyShift(this SKBitmap input, sbyte labelTop, short labelShift, SKColor backgroundColor)
        {
            labelTop = (sbyte)Math.Min((short)120, Math.Max((short)0, labelTop));
            labelShift = Math.Max((short)-9999, Math.Min((short)0, labelShift));

            if (labelTop == 0 && labelShift == 0)
                return input;

            var output = new SKBitmap(input.Width, input.Height, SKColorType.Bgra8888, SKAlphaType.Premul);

            using var canvas = new SKCanvas(output);
            canvas.Clear(backgroundColor);
            canvas.DrawBitmap(input, new SKPoint(-labelShift, labelTop));
            canvas.Flush();

            return output;
        }
    }
}