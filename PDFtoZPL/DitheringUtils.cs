using SkiaSharp;
using System;
using System.Runtime.CompilerServices;
using static PDFtoZPL.Conversion;

[assembly: InternalsVisibleTo("PDFtoZPL.WebConverter")]

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

		private static byte HandleByteError(byte input, int error)
		{
			if ((input & 0xff) + error < 0)
				return 0;

			else if ((input & 0xff) + error > 255)
				return 255;

			return (byte)(input + error);
		}

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
			bool right2 = col != (width - 2);
			bool below = row != (height - 1);
			bool below2 = row != (height - 2);

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