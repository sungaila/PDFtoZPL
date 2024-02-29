using System;
using System.IO;
using System.Reflection;
using static PDFtoZPL.Conversion;

namespace PDFtoZPL.Console
{
#if NET8_0_OR_GREATER
#pragma warning disable CA1510 // Use ArgumentNullException throw helper
#endif
    public static class Program
    {
        public static int Main(string[] args)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            System.Console.WriteLine($"{entryAssembly?.GetName()?.ToString() ?? "PDFtoZPL"}");
            System.Console.WriteLine();

            try
            {
                ParseArguments(args, out string? inputPath, out string? outputPath, out int page, out int dpi, out bool withAnnotations, out bool withFormFill, out var encodingKind);

                if (inputPath == null)
                    throw new InvalidOperationException("There is no PDF file path.");

                if (outputPath == null)
                    throw new InvalidOperationException("There is no output ZPL plain text file path.");

                using var inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read);

                string zpl = Path.GetExtension(inputPath).ToLower() switch
                {
                    ".pdf" =>
#if NET6_0_OR_GREATER
					OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()
#else
                    true
#endif
#pragma warning disable CA1416
                        ? Conversion.ConvertPdfPage(inputStream, page: page - 1, pdfOptions: new (Dpi: dpi, WithAnnotations: withAnnotations, WithFormFill: withFormFill), zplOptions: new (EncodingKind: encodingKind))
#pragma warning restore CA1416
                        : throw new NotSupportedException("Only win-x86, win-x64, win-arm64, linux-x64, linux-arm, linux-arm64, osx-x64 and osx-arm64 are supported for PDF file conversion."),
                    _ => throw new InvalidOperationException("The given input file path must have pdf as file extension."),
                };

                File.WriteAllText(outputPath, zpl);
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine("Failed to convert file.");
                System.Console.Error.WriteLine(ex);

                return -1;
            }

            return 0;
        }

        private static void ParseArguments(string[] args, out string? inputPath, out string? outputPath, out int page, out int dpi, out bool withAnnotations, out bool withFormFill, out BitmapEncodingKind encodingKind)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            if (args.Length >= 1)
            {
                inputPath = args[0];
            }
            else
            {
                System.Console.Write("Enter the path to the PDF file: ");
                inputPath = System.Console.ReadLine();
            }

            inputPath = inputPath!.Trim('\"');

            if (args.Length >= 2)
            {
                outputPath = args[1];
            }
            else
            {
                System.Console.Write("Enter the output path to the ZPL plain text file: ");
                outputPath = System.Console.ReadLine();

                if (string.IsNullOrWhiteSpace(outputPath))
                {
                    outputPath = Path.Combine(Path.GetDirectoryName(inputPath)!, Path.GetFileNameWithoutExtension(inputPath) + ".txt");
                    System.Console.WriteLine($"Output defaulting to \"{outputPath}\".");
                }
            }

            outputPath = outputPath.Trim('\"');

            if (!Path.GetExtension(inputPath).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                page = default;
                dpi = default;
                withAnnotations = false;
                withFormFill = false;
                encodingKind = BitmapEncodingKind.Base64Compressed;
                return;
            }

            page = 1;

            if (args.Length >= 3)
            {
                outputPath = args[2];
            }
            else
            {
                System.Console.Write("Enter PDF page number: ");

                if (!int.TryParse(System.Console.ReadLine(), out page) || page <= 0)
                {
                    page = 1;
                    System.Console.WriteLine($"PDF page number defaulting to {page}.");
                }
            }

            dpi = 300;

            if (args.Length >= 4)
            {
                outputPath = args[3];
            }
            else
            {
                System.Console.Write("Enter the target resolution in DPI: ");

                if (!int.TryParse(System.Console.ReadLine(), out dpi) || dpi <= 0)
                {
                    dpi = 203;
                    System.Console.WriteLine($"Target DPI defaulting to {dpi}.");
                }
            }

            withAnnotations = false;

            if (args.Length >= 5)
            {
                outputPath = args[4];
            }
            else
            {
                System.Console.Write("Should annotations be rendered (y/n): ");

                var input = System.Console.ReadLine();
                if (input?.ToLowerInvariant() == "y")
                {
                    withAnnotations = true;
                }
                else if (input?.ToLowerInvariant() == "n")
                {
                    withAnnotations = false;
                }
                else
                {
                    withAnnotations = false;
                    System.Console.WriteLine($"Annotations not rendered by default.");
                }
            }

            withFormFill = false;

            if (args.Length >= 6)
            {
                outputPath = args[5];
            }
            else
            {
                System.Console.Write("Should form filling be rendered (y/n): ");

                var input = System.Console.ReadLine();
                if (input?.ToLowerInvariant() == "y")
                {
                    withFormFill = true;
                }
                else if (input?.ToLowerInvariant() == "n")
                {
                    withFormFill = false;
                }
                else
                {
                    withFormFill = false;
                    System.Console.WriteLine($"Form filling not rendered by default.");
                }
            }

            encodingKind = BitmapEncodingKind.Base64Compressed;

            if (args.Length >= 7)
            {
                outputPath = args[6];
            }
            else
            {
                System.Console.Write("Select an encoding type (hex/hexc/b64/z64): ");

                var input = System.Console.ReadLine();
                if (input?.ToLowerInvariant() == "hex")
                {
                    encodingKind = BitmapEncodingKind.Hexadecimal;
                }
                else if (input?.ToLowerInvariant() == "hexc")
                {
                    encodingKind = BitmapEncodingKind.HexadecimalCompressed;
                }
                else if (input?.ToLowerInvariant() == "b64")
                {
                    encodingKind = BitmapEncodingKind.Base64;
                }
                else if (input?.ToLowerInvariant() == "z64")
                {
                    encodingKind = BitmapEncodingKind.Base64Compressed;
                }
                else
                {
                    encodingKind = BitmapEncodingKind.Base64Compressed;
                    System.Console.WriteLine("Target DPI defaulting to z64.");
                }
            }
        }
    }
}