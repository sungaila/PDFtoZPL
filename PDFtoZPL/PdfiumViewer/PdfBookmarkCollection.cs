using System.Collections.ObjectModel;

#pragma warning disable 1591

namespace PDFtoZPL.PdfiumViewer
{
    internal class PdfBookmark
    {
        public string? Title { get; set; }
        public int PageIndex { get; set; }

        public PdfBookmarkCollection Children { get; }

        public PdfBookmark()
        {
            Children = new PdfBookmarkCollection();
        }

        public override string? ToString()
        {
            return Title;
        }
    }

    internal class PdfBookmarkCollection : Collection<PdfBookmark>
    {
    }
}
