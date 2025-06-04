using SkiaSharp;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("PDFtoZPL.WebConverter, PublicKey=0024000004800000940000000602000000240000525341310004000001000100fd0018feed1ba4fac91744d868cb1bb8a4b55a97eec0e02e90a8d57db56bff5a32f03813b1a6c8a7ccd50eed8880be4e91ad281c9ee81fd4182de0905c0590019e9bf87a8f83ce93c09da9c4eaae5bd8aa63816c4a3bef8bd16bb358d3aed64cc2ec3ae17698c336b63df4fbd719dc13bd9a88d9fcfa87a2426d34db78d05fb7")]

namespace PDFtoZPL
{
    internal static class DitheringUtils
    {
        private static readonly uint _black = (uint)new SKColor(0, 0, 0, 255);
        private static readonly uint _white = (uint)new SKColor(255, 255, 255, 255);

        public static SKBitmap ToMonochrome(this SKBitmap input, byte threshold = 128, DitheringKind dithering = DitheringKind.None)
        {
            int width = input.Width;
            int height = input.Height;
            var output = input.Copy(SKColorType.Rgba8888);

            IntPtr pixelsAddr = output.GetPixels();

            unsafe
            {
                unchecked
                {
                    uint* ptr = (uint*)pixelsAddr.ToPointer();

                    if (dithering == DitheringKind.None)
                        NoDithering(threshold, width, height, ptr);
                    else
                        WithDithering(threshold, dithering, width, height, ptr);
                }
            }

            return output;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void NoDithering(byte threshold, int width, int height, uint* ptr)
        {
            unchecked
            {
                Parallel.For(0, height, row =>
                {
                    for (int col = 0; col < width; col++)
                    {
                        int index = row * width + col;
                        uint oldPixel = ptr[index];

                        uint sum = oldPixel & 0xFF;
                        sum += (oldPixel >> 8) & 0xFF;
                        sum += (oldPixel >> 16) & 0xFF;

                        uint newPixel = (sum / 3) < threshold ? _black : _white;
                        ptr[index] = newPixel;
                    }
                });
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void WithDithering(byte threshold, DitheringKind dithering, int width, int height, uint* ptr)
        {
            unchecked
            {
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        int index = row * width + col;
                        uint oldPixel = ptr[index];

                        uint sum = oldPixel & 0xFF;
                        sum += (oldPixel >> 8) & 0xFF;
                        sum += (oldPixel >> 16) & 0xFF;

                        uint newPixel = (sum / 3) < threshold ? _black : _white;
                        ptr[index] = newPixel;

                        if (dithering == DitheringKind.FloydSteinberg)
                        {
                            DitherFloydSteinberg(oldPixel, newPixel, row, col, width, height, ptr);
                        }
                        else if (dithering == DitheringKind.Atkinson)
                        {
                            DitherAtkinson(oldPixel, newPixel, row, col, width, height, ptr);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(nameof(dithering));
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte HandleByteError(byte input, int error)
        {
#if NETFRAMEWORK
            int value = input + error;

            if (value < 0) return 0;
            if (value > 255) return 255;

            return (byte)value;
#else
            int value = input + error;
            return (byte)Math.Clamp(value, 0, 255);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint HandlePixel(uint input, int red, int green, int blue)
        {
            uint result = HandleByteError((byte)(input & 0xFF), red);
            result += (uint)HandleByteError((byte)((input >> 8) & 0xFF), green) << 8;
            result += (uint)HandleByteError((byte)((input >> 16) & 0xFF), blue) << 16;
            result += 0xFF000000;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void DitherFloydSteinberg(uint oldPixel, uint newPixel, int row, int col, int width, int height, uint* basePtr)
        {
            CalculateErrorRGB(oldPixel, newPixel, out int errorR, out int errorG, out int errorB);

            int index = row * width + col;

            if (col + 1 < width)
            {
                basePtr[index + 1] = HandlePixel(basePtr[index + 1], (errorR * 7) >> 4, (errorG * 7) >> 4, (errorB * 7) >> 4);
                if (row + 1 < height)
                {
                    basePtr[index + 1 + width] = HandlePixel(basePtr[index + 1 + width], (errorR * 1) >> 4, (errorG * 1) >> 4, (errorB * 1) >> 4);
                }
            }

            if (row + 1 < height)
            {
                basePtr[index + width] = HandlePixel(basePtr[index + width], (errorR * 5) >> 4, (errorG * 5) >> 4, (errorB * 5) >> 4);
                if (col - 1 >= 0)
                {
                    basePtr[index - 1 + width] = HandlePixel(basePtr[index - 1 + width], (errorR * 3) >> 4, (errorG * 3) >> 4, (errorB * 3) >> 4);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void DitherAtkinson(uint oldPixel, uint newPixel, int row, int col, int width, int height, uint* basePtr)
        {
            CalculateErrorRGB(oldPixel, newPixel, out int errorR, out int errorG, out int errorB);

            int index = row * width + col;

            if (col + 1 < width)
            {
                basePtr[index + 1] = HandlePixel(basePtr[index + 1], errorR >> 3, errorG >> 3, errorB >> 3);
                if (row + 1 < height)
                {
                    basePtr[index + 1 + width] = HandlePixel(basePtr[index + 1 + width], errorR >> 3, errorG >> 3, errorB >> 3);
                }
            }

            if (row + 1 < height)
            {
                basePtr[index + width] = HandlePixel(basePtr[index + width], errorR >> 3, errorG >> 3, errorB >> 3);
                if (col - 1 >= 0)
                {
                    basePtr[index - 1 + width] = HandlePixel(basePtr[index - 1 + width], errorR >> 3, errorG >> 3, errorB >> 3);
                }
            }

            if (col + 2 < width)
            {
                basePtr[index + 2] = HandlePixel(basePtr[index + 2], errorR >> 3, errorG >> 3, errorB >> 3);
            }

            if (row + 2 < height)
            {
                basePtr[index + width * 2] = HandlePixel(basePtr[index + width * 2], errorR >> 3, errorG >> 3, errorB >> 3);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CalculateErrorRGB(uint oldPixel, uint newPixel, out int r, out int g, out int b)
        {
            r = (int)((oldPixel & 0xFF) - (newPixel & 0xFF));
            g = (int)(((oldPixel >> 8) & 0xFF) - ((newPixel >> 8) & 0xFF));
            b = (int)(((oldPixel >> 16) & 0xFF) - ((newPixel >> 16) & 0xFF));
        }
    }
}
