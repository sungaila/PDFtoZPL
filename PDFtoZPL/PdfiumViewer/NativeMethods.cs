using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PDFtoZPL.PdfiumViewer
{
    internal static partial class NativeMethods
    {
        static NativeMethods()
        {
            // Load the platform dependent Pdfium.dll if it exists.
            if (!TryLoadNativeLibrary(AppDomain.CurrentDomain.RelativeSearchPath!))
                TryLoadNativeLibrary(Path.GetDirectoryName(typeof(NativeMethods).Assembly.Location)!);
        }

        private static bool TryLoadNativeLibrary(string path)
        {
            if (path == null)
                return false;

            path = Path.Combine(path, "runtimes");

#if NETCOREAPP3_0_OR_GREATER
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                path = Path.Combine(path, Environment.Is64BitProcess ? "win-x64" : "win-x86");
                path = Path.Combine(path, "native");
                path = Path.Combine(path, "pdfium.dll");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                path = Path.Combine(path, Environment.Is64BitProcess ? "linux-x64" : throw new NotSupportedException("Only x86-64 is supported on Linux."));
                path = Path.Combine(path, "native");
                path = Path.Combine(path, "libpdfium.so");
            }
            else
            {
                throw new NotSupportedException("Only win-x86, win-x64 and linux-x64 are supported.");
            }

             return File.Exists(path) && NativeLibrary.Load(path) != IntPtr.Zero;
#else
            path = Path.Combine(path, Environment.Is64BitProcess ? "win-x64" : "win-x86");
                path = Path.Combine(path, "native");
                path = Path.Combine(path, "pdfium.dll");

            return File.Exists(path) && LoadLibrary(path) != IntPtr.Zero;
#endif
        }

#if !NETCOREAPP3_0_OR_GREATER
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);
#endif
    }
}