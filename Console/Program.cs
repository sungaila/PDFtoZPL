using System;
using System.IO;
using System.Reflection;

namespace PDFtoZPL.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            System.Console.WriteLine($"{entryAssembly?.GetName()?.ToString() ?? "PDFtoZPL"}");
            System.Console.WriteLine();

            try
            {
                ParseArguments(args, out string? inputPath, out string? outputPath, out int page, out int dpi, out bool withAnnotations, out bool withFormFill);

                if (inputPath == null)
                    throw new InvalidOperationException("There is no PDF or Bitmap file path.");

                if (outputPath == null)
                    throw new InvalidOperationException("There is no output ZPL plain text file path.");

                using var inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read);

                string zpl = (Path.GetExtension(inputPath).ToLower()) switch
                {
                    ".pdf" =>
#if NET5_0_OR_GREATER
                    OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()
#else
                    true
#endif
#pragma warning disable CA1416
                        ? Conversion.ConvertPdfPage(inputStream, page: page - 1, dpi: dpi, withAnnotations: withAnnotations, withFormFill: withFormFill)
#pragma warning restore CA1416
                        : throw new NotSupportedException("Only win-x86, win-x64, linux-x64, osx-x64 and osx-arm64 are supported for PDF file conversion."),
                    ".bmp" => Conversion.ConvertBitmap(inputStream),
                    _ => throw new InvalidOperationException("The given input file path must have pdf or bmp as file extension."),
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

        private static void ParseArguments(string[] args, out string? inputPath, out string? outputPath, out int page, out int dpi, out bool withAnnotations, out bool withFormFill)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            if (args.Length >= 1)
            {
                inputPath = args[0];
            }
            else
            {
                System.Console.Write("Enter the path to the PDF or Bitmap file: ");
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

            if (Path.GetExtension(inputPath).ToLower() != ".pdf")
            {
                page = default;
                dpi = default;
                withAnnotations = false;
                withFormFill = false;
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
        }
    }
}
