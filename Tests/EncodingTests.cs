using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using PDFtoZPL;
using static PDFtoZPL.Conversion;

namespace Tests
{
	[TestClass]
	public partial class EncodingTests
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
		[DataRow("SocialPreview.pdf", BitmapEncodingKind.Hexadecimal)]
		[DataRow("SocialPreview.pdf", BitmapEncodingKind.HexadecimalCompressed)]
		[DataRow("SocialPreview.pdf", BitmapEncodingKind.Base64)]
		[DataRow("SocialPreview.pdf", BitmapEncodingKind.Base64Compressed)]
		[DataRow("SocialPreview.png", null)]
		[DataRow("SocialPreview.png", BitmapEncodingKind.Hexadecimal)]
		[DataRow("SocialPreview.png", BitmapEncodingKind.HexadecimalCompressed)]
		[DataRow("SocialPreview.png", BitmapEncodingKind.Base64)]
		[DataRow("SocialPreview.png", BitmapEncodingKind.Base64Compressed)]
		[DataRow("Wikimedia_Commons_web.pdf", null)]
		[DataRow("Wikimedia_Commons_web.pdf", BitmapEncodingKind.Hexadecimal)]
		[DataRow("Wikimedia_Commons_web.pdf", BitmapEncodingKind.HexadecimalCompressed)]
		[DataRow("Wikimedia_Commons_web.pdf", BitmapEncodingKind.Base64)]
		[DataRow("Wikimedia_Commons_web.pdf", BitmapEncodingKind.Base64Compressed)]
		public void ConvertWithEncodings(string fileName, BitmapEncodingKind? encodingKind)
		{
			var expectedResult = _expectedResults[fileName][encodingKind ?? BitmapEncodingKind.HexadecimalCompressed];

			using var fileStream = new FileStream(Path.Combine("Assets", fileName), FileMode.Open, FileAccess.Read);

			var zplResult = fileName.EndsWith(".pdf")
				? ConvertPdfPage(fileStream, pdfOptions: new(Dpi: 203), zplOptions: new (EncodingKind: encodingKind ?? BitmapEncodingKind.HexadecimalCompressed))
				: ConvertBitmap(fileStream, zplOptions: new(EncodingKind: encodingKind ?? BitmapEncodingKind.HexadecimalCompressed));

			Assert.AreEqual(expectedResult, zplResult.Replace("\n", string.Empty));
		}

		[TestMethod]
		[DataRow("SocialPreview.pdf", null)]
		[DataRow("SocialPreview.pdf", BitmapEncodingKind.Hexadecimal)]
		[DataRow("SocialPreview.pdf", BitmapEncodingKind.HexadecimalCompressed)]
		[DataRow("SocialPreview.pdf", BitmapEncodingKind.Base64)]
		[DataRow("SocialPreview.pdf", BitmapEncodingKind.Base64Compressed)]
		[DataRow("SocialPreview.png", null)]
		[DataRow("SocialPreview.png", BitmapEncodingKind.Hexadecimal)]
		[DataRow("SocialPreview.png", BitmapEncodingKind.HexadecimalCompressed)]
		[DataRow("SocialPreview.png", BitmapEncodingKind.Base64)]
		[DataRow("SocialPreview.png", BitmapEncodingKind.Base64Compressed)]
		[DataRow("Wikimedia_Commons_web.pdf", null)]
		[DataRow("Wikimedia_Commons_web.pdf", BitmapEncodingKind.Hexadecimal)]
		[DataRow("Wikimedia_Commons_web.pdf", BitmapEncodingKind.HexadecimalCompressed)]
		[DataRow("Wikimedia_Commons_web.pdf", BitmapEncodingKind.Base64)]
		[DataRow("Wikimedia_Commons_web.pdf", BitmapEncodingKind.Base64Compressed)]
		public void ConvertWithEncodingsGraphicFieldOnly(string fileName, BitmapEncodingKind? encodingKind)
		{
			var expectedResult = _expectedResults[fileName][encodingKind ?? BitmapEncodingKind.HexadecimalCompressed];

			using var fileStream = new FileStream(Path.Combine("Assets", fileName), FileMode.Open, FileAccess.Read);

			var zplResult = fileName.EndsWith(".pdf")
				? ConvertPdfPage(fileStream, pdfOptions: new(Dpi: 203), zplOptions: new(EncodingKind: encodingKind ?? BitmapEncodingKind.HexadecimalCompressed, GraphicFieldOnly: true))
				: ConvertBitmap(fileStream, zplOptions: new(EncodingKind: encodingKind ?? BitmapEncodingKind.HexadecimalCompressed, GraphicFieldOnly: true));

			var trimmedExpected = expectedResult.Remove(0, "^XA".Length);

			Assert.AreEqual(trimmedExpected.Remove(trimmedExpected.Length - "^FS^XZ".Length), zplResult.Replace("\n", string.Empty));
		}
	}
}