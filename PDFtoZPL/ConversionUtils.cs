﻿using ICSharpCode.SharpZipLib.Zip.Compression;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFtoZPL
{
    internal static class ConversionUtils
    {
        private static readonly IReadOnlyDictionary<string, byte> _hexLookupTable;

        static ConversionUtils()
        {
            _hexLookupTable = new Dictionary<string, byte>();

            for (int i = 0; i <= 255; i++)
            {
                ((Dictionary<string, byte>)_hexLookupTable).Add(i.ToString("X2"), (byte)i);
            }
        }

        public static byte[] HexToByteArray(string input)
        {
            var result = new List<byte>();

            for (int i = 0; i < input.Length; i += 2)
                result.Add(_hexLookupTable[input.Substring(i, 2)]);

            return [.. result];
        }

        public static string ConvertBitmapToHex(SKBitmap pdfBitmap, byte threshold, out int binaryByteCount, out int bytesPerRow)
        {
            var zplBuilder = new StringBuilder();

            bytesPerRow = pdfBitmap.Width % 8 > 0
                ? pdfBitmap.Width / 8 + 1
                : pdfBitmap.Width / 8;
            binaryByteCount = pdfBitmap.Height * bytesPerRow;

            int colorBits = 0;
            int j = 0;

            int width = pdfBitmap.Width;
            int height = pdfBitmap.Height;

            SKColor[] data = pdfBitmap.Pixels;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pixel = data[y * width + x];

                    byte red = pixel.Red;
                    byte green = pixel.Green;
                    byte blue = pixel.Blue;

                    bool blackPixel = ((red + green + blue) / 3) < threshold;

                    if (blackPixel)
                        colorBits |= 1 << (7 - j);

                    j++;

                    if (j == 8 || x == (width - 1))
                    {
                        zplBuilder.Append(colorBits.ToString("X2"));
                        colorBits = 0;
                        j = 0;
                    }
                }
                zplBuilder.Append('\n');
            }

            return zplBuilder.ToString();
        }

        public static string CompressHex(string code, int widthBytes)
        {
            int maxlinea = widthBytes * 2;
            var sbCode = new StringBuilder();
            var sbLinea = new StringBuilder();
            string? previousLine = null;
            int counter = 0;
            char aux = code[0];
            bool firstChar = false;

            foreach (char item in code)
            {
                if (firstChar)
                {
                    aux = item;
                    firstChar = false;
                    continue;
                }
                if (item == '\n')
                {
                    if (counter >= maxlinea && aux == '0')
                    {
                        sbLinea.Append(',');
                    }
                    else if (counter >= maxlinea && aux == 'F')
                    {
                        sbLinea.Append('!');
                    }
                    else if (counter > 20)
                    {
                        int multi20 = (counter / 20) * 20;
                        sbLinea.Append(CompressionCountMapping[Math.Min(multi20, 400)]);

                        int restover400 = multi20 / 400;
                        if (restover400 > 0)
                        {
                            for (; restover400 > 1; restover400--)
                                sbLinea.Append(CompressionCountMapping[400]);

                            int restto400 = (counter % 400) / 20 * 20;

                            if (restto400 > 0)
                                sbLinea.Append(CompressionCountMapping[restto400]);
                        }

                        int resto20 = (counter % 20);

                        if (resto20 != 0)
                            sbLinea.Append(CompressionCountMapping[resto20]).Append(aux);
                        else
                            sbLinea.Append(aux);
                    }
                    else
                    {
                        sbLinea.Append(CompressionCountMapping[counter]).Append(aux);
                    }
                    counter = 1;
                    firstChar = true;

                    if (sbLinea.ToString().Equals(previousLine))
                        sbCode.Append(':');
                    else
                        sbCode.Append(sbLinea);

                    previousLine = sbLinea.ToString();
                    sbLinea.Length = 0;
                    continue;
                }

                if (aux == item)
                {
                    counter++;
                }
                else
                {
                    if (counter > 20)
                    {
                        int multi20 = (counter / 20) * 20;
                        sbLinea.Append(CompressionCountMapping[Math.Min(multi20, 400)]);

                        int restover400 = multi20 / 400;
                        if (restover400 > 0)
                        {
                            for (; restover400 > 1; restover400--)
                                sbLinea.Append(CompressionCountMapping[400]);

                            int restto400 = (counter % 400) / 20 * 20;

                            if (restto400 > 0)
                                sbLinea.Append(CompressionCountMapping[restto400]);
                        }

                        int resto20 = (counter % 20);

                        if (resto20 != 0)
                            sbLinea.Append(CompressionCountMapping[resto20]).Append(aux);
                        else
                            sbLinea.Append(aux);
                    }
                    else
                    {
                        sbLinea.Append(CompressionCountMapping[counter]).Append(aux);
                    }
                    counter = 1;
                    aux = item;
                }
            }

            return sbCode.ToString();
        }

        /// <summary>
        /// The mapping table used for compression.
        /// Each character count (the key) is represented by a certain char (the value).
        /// </summary>
        private static readonly IReadOnlyDictionary<int, string> CompressionCountMapping = new Dictionary<int, string>()
        {
            { 1, "G" },
            { 2, "H" },
            { 3, "I" },
            { 4, "J" },
            { 5, "K" },
            { 6, "L" },
            { 7, "M" },
            { 8, "N" },
            { 9, "O" },
            { 10, "P" },
            { 11, "Q" },
            { 12, "R" },
            { 13, "S" },
            { 14, "T" },
            { 15, "U" },
            { 16, "V" },
            { 17, "W" },
            { 18, "X" },
            { 19, "Y" },
            { 20, "g" },
            { 40, "h" },
            { 60, "i" },
            { 80, "j" },
            { 100, "k" },
            { 120, "l" },
            { 140, "m" },
            { 160, "n" },
            { 180, "o" },
            { 200, "p" },
            { 220, "q" },
            { 240, "r" },
            { 260, "s" },
            { 280, "t" },
            { 300, "u" },
            { 320, "v" },
            { 340, "w" },
            { 360, "x" },
            { 380, "y" },
            { 400, "z" }
        };

        public static ushort ComputeBitmapChecksum(string input)
        {
            var array = Encoding.ASCII.GetBytes(input);

            // CRC-CCIIT 0xFFFF
            // 1 + x + x^5 + x^12 + x^16 is irreducible polynomial
            // https://introcs.cs.princeton.edu/java/61data/CRC16CCITT.java
            const ushort polynomial = 0x1021;
            ushort crc = 0x0000;

            foreach (byte b in array)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool bit = (b >> (7 - i) & 1) == 1;
                    bool c15 = (crc >> 15 & 1) == 1;
                    crc <<= 1;

                    if (c15 ^ bit)
                        crc ^= polynomial;
                }
            }

            crc &= 0xffff;
            return crc;
        }

        public static byte[] Deflate(byte[] input)
        {
            using var memoryStream = new MemoryStream();

            var deflater = new Deflater(Deflater.BEST_COMPRESSION);
            deflater.SetInput(input);
            deflater.Finish();

            byte[] buffer = new byte[1024];
            while (!deflater.IsFinished)
            {
                int count = deflater.Deflate(buffer);
                memoryStream.Write(buffer, 0, count);
            }

            return memoryStream.ToArray();
        }
    }
}