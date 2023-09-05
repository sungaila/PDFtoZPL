using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFtoImage;
using System;
using System.IO;
using static PDFtoZPL.Conversion;

namespace Tests
{
	[TestClass]
	public partial class PdfRotationTests
	{
		[TestInitialize]
		public void Initialize()
		{
#if NET6_0_OR_GREATER
			if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
				Assert.Inconclusive("This test must run on Windows, Linux or macOS.");
#endif
		}

#pragma warning disable CA1416

		[TestMethod]
		[DataRow("SocialPreview.pdf", null)]
		[DataRow("SocialPreview.pdf", PdfRotation.Rotate0)]
		[DataRow("SocialPreview.pdf", PdfRotation.Rotate90)]
		[DataRow("SocialPreview.pdf", PdfRotation.Rotate180)]
		[DataRow("SocialPreview.pdf", PdfRotation.Rotate270)]
		[DataRow("Wikimedia_Commons_web.pdf", null)]
		[DataRow("Wikimedia_Commons_web.pdf", PdfRotation.Rotate0)]
		[DataRow("Wikimedia_Commons_web.pdf", PdfRotation.Rotate90)]
		[DataRow("Wikimedia_Commons_web.pdf", PdfRotation.Rotate180)]
		[DataRow("Wikimedia_Commons_web.pdf", PdfRotation.Rotate270)]
		public void ConvertWithRotation(string fileName, PdfRotation? rotation)
		{
			var expectedResult = _expectedResults[fileName][rotation ?? PdfRotation.Rotate0];

			using var fileStream = new FileStream(Path.Combine("Assets", fileName), FileMode.Open, FileAccess.Read);

			var zplResult
				= rotation != null ? ConvertPdfPage(fileStream, encodingKind: BitmapEncodingKind.Base64Compressed, rotation: rotation.Value) : ConvertPdfPage(fileStream, encodingKind: BitmapEncodingKind.Base64Compressed);

			Assert.AreEqual(expectedResult, zplResult.Replace("\n", string.Empty));
		}
	}
}