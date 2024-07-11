using Microsoft.AspNetCore.Components.Forms;
using PDFtoImage;
using SkiaSharp;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.InteropServices.Marshalling;

namespace PDFtoZPL.WebConverter.Models
{
    public class RenderRequest : IDisposable
    {
        public static readonly SKEncodedImageFormat[] FormatWhitelist =
        [
            SKEncodedImageFormat.Png,
            SKEncodedImageFormat.Jpeg,
            SKEncodedImageFormat.Webp
        ];

        private bool disposedValue;

        [Required(ErrorMessage = "Select a PDF file to convert.")]
        public IBrowserFile? File { get; set; }

        public string? Password { get; set; }

        [Required]
        public SKEncodedImageFormat Format { get; set; } = SKEncodedImageFormat.Png;

        [Required]
        public BitmapEncodingKind Encoding { get; set; } = BitmapEncodingKind.Base64Compressed;

        public static string GetEncodingLocalized(BitmapEncodingKind encoding) => encoding switch
        {
            BitmapEncodingKind.Hexadecimal => "Hexadecimal [not recommended]",
            BitmapEncodingKind.HexadecimalCompressed => "Hexadecimal with compression",
            BitmapEncodingKind.Base64 => "Base64 [not recommended]",
            BitmapEncodingKind.Base64Compressed => "Base64 with compression",
            _ => throw new ArgumentOutOfRangeException(nameof(encoding))
        };

        [Required]
        public bool WithAnnotations { get; set; } = true;

        [Required]
        [Range(0, 100, ErrorMessage = "Quality invalid (1-100).")]
        public int Quality { get; set; } = 100;

        [Range(1, int.MaxValue, ErrorMessage = "Width invalid (≥0).")]
        public int? Width { get; set; } = null;

        [Range(1, int.MaxValue, ErrorMessage = "Height invalid (≥0).")]
        public int? Height { get; set; } = null;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "DPI invalid (≥0).")]
        public int Dpi { get; set; } = 203;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Page number invalid (≥0).")]
        public int Page { get; set; } = 0;

        [Required]
        public bool GraphicFieldOnly { get; set; } = false;

        [Required]
        public bool IncludeStartFormat
        {
            get => !GraphicFieldOnly;
            set => GraphicFieldOnly = !value;
        }

        [Required]
        public bool SetLabelLength { get; set; } = true;

        [Required]
        public bool WithFormFill { get; set; } = true;

        [Required]
        public bool WithAspectRatio { get; set; } = true;

        [Required]
        public bool AntiAliasingText { get; set; } = true;

        [Required]
        public bool AntiAliasingImages { get; set; } = true;

        [Required]
        public bool AntiAliasingPaths { get; set; } = true;

        [Required]
        public string BackgroundColor { get; set; } = "#FFFFFF";

        [Range(byte.MinValue, byte.MaxValue, ErrorMessage = "Opacity invalid (0-255).")]
        public int Opacity { get; set; } = 255;

        [Required]
        public PdfRotation Rotation { get; set; } = PdfRotation.Rotate0;

        public static string GetRotationLocalized(PdfRotation rotation) => rotation switch
        {
            PdfRotation.Rotate0 => "0°",
            PdfRotation.Rotate90 => "90°",
            PdfRotation.Rotate180 => "180°",
            PdfRotation.Rotate270 => "270°",
            _ => throw new ArgumentOutOfRangeException(nameof(rotation))
        };

        public byte Threshold { get; set; } = 128;

        [Required]
        [Range(0, byte.MaxValue, ErrorMessage = "Threshold number invalid (must be between 0 and 255).")]
        public int ThresholdAsInt
        {
            get => Threshold;
            set => Threshold = (byte)value;
        }

        [Required]
        public DitheringKind Dithering { get; set; } = DitheringKind.None;

        [Required]
        public bool UseBounds { get; set; } = false;

        [Required]
        [Range(float.MinValue, float.MaxValue)]
        public float BoundsX { get; set; } = 0f;

        [Required]
        [Range(float.MinValue, float.MaxValue)]
        public float BoundsY { get; set; } = 0f;

        [Required]
        [Range(float.MinValue, float.MaxValue)]
        public float BoundsWidth { get; set; } = 1000f;

        [Required]
        [Range(float.MinValue, float.MaxValue)]
        public float BoundsHeight { get; set; } = 1000f;

        [Required]
        public bool UseTiling { get; set; } = true;

        public bool DpiRelativeToBounds { get; set; } = false;

        [Required]
        public bool UsePrintQuantity { get; set; } = false;

        [Required]
        [Range(1, 99999999)]
        public int PrintQuantity { get; set; } = 1;

        public static string GetDitheringLocalized(DitheringKind dithering) => dithering switch
        {
            DitheringKind.None => "None",
            DitheringKind.FloydSteinberg => "Floyd–Steinberg",
            DitheringKind.Atkinson => "Atkinson",
            _ => throw new ArgumentOutOfRangeException(nameof(dithering))
        };

        public static string GetMimeType(SKEncodedImageFormat format) => format switch
        {
            SKEncodedImageFormat.Png => "image/png",
            SKEncodedImageFormat.Jpeg => "image/jpeg",
            SKEncodedImageFormat.Webp => "image/webp",
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        public static string GetOutputFileName(RenderRequest model) => $"{model.File!.Name}.txt";

        public Stream? Input { get; set; }

        public string? Output { get; set; }

        public Stream? OutputPreviewImage { get; set; }

        public override string ToString()
        {
            return $"{nameof(RenderRequest)} {nameof(File)}={File?.Name ?? "<null>"}, {nameof(Password)}={(!string.IsNullOrEmpty(Password) ? "<password>" : "<null>")}, {nameof(Page)}={Page}, {nameof(Format)}={Format}, {nameof(Quality)}={Quality}, {nameof(Width)}={(Width != null ? Width.Value : "<null>")}, {nameof(Height)}={(Height != null ? Height.Value : "<null>")}, {nameof(Dpi)}={Dpi}, {nameof(Rotation)}={Rotation}, {nameof(WithAspectRatio)}={WithAspectRatio}, {nameof(WithAnnotations)}={WithAnnotations}, {nameof(WithFormFill)}={WithFormFill}";
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Input?.Dispose();
                    Input = null;
                    OutputPreviewImage?.Dispose();
                    OutputPreviewImage = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
