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
                ParseArguments(args, out string? inputPath, out string? outputPath);

                if (inputPath == null)
                    throw new InvalidOperationException("There is no PDF or Bitmap file path.");

                if (outputPath == null)
                    throw new InvalidOperationException("There is no output ZPL plain text file path.");

                using var inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
                
                string zpl = (Path.GetExtension(inputPath).ToLower()) switch
                {
                    ".pdf" => OperatingSystem.IsWindows() || OperatingSystem.IsLinux()
#pragma warning disable CA1416
                        ? Conversion.ConvertPdfPage(inputStream)
#pragma warning restore CA1416
                        : throw new NotSupportedException("Only win-x86, win-x64, linux-x64 and osx-x64 are supported for PDF file conversion."),
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

        private static void ParseArguments(string[] args, out string? inputPath, out string? outputPath)
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
        }
    }
}
