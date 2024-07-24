using SkiaSharp;
using System;
using System.Runtime.CompilerServices;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("PDFtoZPL.WebConverter, PublicKey=0024000004800000940000000602000000240000525341310004000001000100fd0018feed1ba4fac91744d868cb1bb8a4b55a97eec0e02e90a8d57db56bff5a32f03813b1a6c8a7ccd50eed8880be4e91ad281c9ee81fd4182de0905c0590019e9bf87a8f83ce93c09da9c4eaae5bd8aa63816c4a3bef8bd16bb358d3aed64cc2ec3ae17698c336b63df4fbd719dc13bd9a88d9fcfa87a2426d34db78d05fb7")]

namespace PDFtoZPL
{
    internal static class DitheringUtils
    {
        private static readonly uint _black = (uint)new SKColor(0, 0, 0, 255);
        private static readonly uint _white = (uint)new SKColor(255, 255, 255, 255);

        public static SKBitmap ToMonochrome(this SKBitmap input, byte threshold = 128, DitheringKind dithering = DitheringKind.None)
        {
            var width = input.Width;
            var height = input.Height;

            var output = input.Copy(SKColorType.Rgba8888);

            IntPtr pixelsAddr = output.GetPixels();

            unsafe
            {
                unchecked
                {
                    uint* ptr = (uint*)pixelsAddr.ToPointer();

                    for (int row = 0; row < height; row++)
                    {
                        for (int col = 0; col < width; col++)
                        {
                            uint oldPixel = *ptr;

                            uint sum = oldPixel % 256;
                            sum += (oldPixel >> 8) % 256;
                            sum += (oldPixel >> 16) % 256;

                            uint newPixel = (sum / 3) < threshold
                                ? _black
                                : _white;

                            *ptr = newPixel;

                            if (dithering == DitheringKind.FloydSteinberg)
                            {
                                DitherFloydSteinberg(oldPixel, newPixel, row, col, width, height, ptr);
                            }
                            else if (dithering == DitheringKind.Atkinson)
                            {
                                DitherAtkinson(oldPixel, newPixel, row, col, width, height, ptr);
                            }
                            else if (dithering != DitheringKind.None)
                            {
                                throw new ArgumentOutOfRangeException(nameof(dithering));
                            }

                            ptr++;
                        }
                    }
                }
            }

            return output;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte HandleByteError(byte input, int error)
        {
            if ((input & 0xff) + error < 0)
                return 0;

            else if ((input & 0xff) + error > 255)
                return 255;

            return (byte)(input + error);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint HandlePixel(uint input, int red, int green, int blue)
        {
            uint result = HandleByteError((byte)(input % 256), red);
            result += (uint)HandleByteError((byte)((input >> 8) % 256), green) << 8;
            result += (uint)HandleByteError((byte)((input >> 16) % 256), blue) << 16;
            result += 0xFF000000;

            return result;
        }

        private static unsafe void DitherFloydSteinberg(uint oldPixel, uint newPixel, int row, int col, int width, int height, uint* ptr)
        {
            int errorR = ((byte)(oldPixel % 256)) - ((byte)(newPixel % 256));
            int errorG = ((byte)((oldPixel >> 8) % 256)) - ((byte)((newPixel >> 8) % 256));
            int errorB = ((byte)((oldPixel >> 16) % 256)) - ((byte)((newPixel >> 16) % 256));

            bool left = col != 0;
            bool right = col != (width - 1);
            bool below = row != (height - 1);

            if (right)
            {
                *(ptr+1) = HandlePixel(*(ptr+1), (errorR * 7) >> 4, (errorG * 7) >> 4, (errorB * 7) >> 4);

                if (below)
                {
                    *(ptr+1+width) = HandlePixel(*(ptr+1+width), (errorR * 1) >> 4, (errorG * 1) >> 4, (errorB * 1) >> 4);
                }
            }

            if (below)
            {
                *(ptr+width) = HandlePixel(*(ptr+width), (errorR * 5) >> 4, (errorG * 5) >> 4, (errorB * 5) >> 4);

                if (left)
                {
                    *(ptr-1+width) = HandlePixel(*(ptr-1+width), (errorR * 3) >> 4, (errorG * 3) >> 4, (errorB * 3) >> 4);
                }
            }
        }

        private static unsafe void DitherAtkinson(uint oldPixel, uint newPixel, int row, int col, int width, int height, uint* ptr)
        {
            int errorR = ((byte)(oldPixel % 256)) - ((byte)(newPixel % 256));
            int errorG = ((byte)((oldPixel >> 8) % 256)) - ((byte)((newPixel >> 8) % 256));
            int errorB = ((byte)((oldPixel >> 16) % 256)) - ((byte)((newPixel >> 16) % 256));

            bool left = col != 0;
            bool right = col != (width - 1);
            bool right2 = right && col != (width - 2);
            bool below = row != (height - 1);
            bool below2 = below && row != (height - 2);

            if (right)
            {
                *(ptr+1) = HandlePixel(*(ptr+1), errorR >> 3, errorG >> 3, errorB >> 3);

                if (below)
                {
                    *(ptr+1+width) = HandlePixel(*(ptr+1+width), errorR >> 3, errorG >> 3, errorB >> 3);
                }
            }

            if (below)
            {
                *(ptr+width) = HandlePixel(*(ptr+width), errorR >> 3, errorG >> 3, errorB >> 3);

                if (left)
                {
                    *(ptr-1+width) = HandlePixel(*(ptr-1+width), errorR >> 3, errorG >> 3, errorB >> 3);
                }
            }

            if (right2)
            {
                *(ptr+2) = HandlePixel(*(ptr+2), errorR >> 3, errorG >> 3, errorB >> 3);
            }

            if (below2)
            {
                *(ptr+width*2) = HandlePixel(*(ptr+width*2), errorR >> 3, errorG >> 3, errorB >> 3);
            }
        }
    }
}
