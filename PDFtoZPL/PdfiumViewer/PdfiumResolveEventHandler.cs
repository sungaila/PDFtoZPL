using System;

namespace PDFtoZPL.PdfiumViewer
{
    internal class PdfiumResolveEventArgs : EventArgs
    {
        public string? PdfiumFileName { get; set; }
    }

    internal delegate void PdfiumResolveEventHandler(object? sender, PdfiumResolveEventArgs e);
}
