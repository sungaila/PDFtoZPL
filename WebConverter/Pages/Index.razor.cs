using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using PDFtoImage;
using PDFtoZPL.WebConverter.Models;
using SkiaSharp;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Thinktecture.Blazor.WebShare.Models;

namespace PDFtoZPL.WebConverter.Pages
{
    public partial class Index : IDisposable
    {
        private DotNetObjectReference<Index>? _objRef;

        public RenderRequest Model { get; set; } = new();

        public bool IsWebShareSupported { get; private set; } = false;

        public bool IsLoading { get; private set; }

        public Exception? LastException { get; private set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await SetupDotNetHelper();
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            Program.FilesHandled -= OnFilesHandled;
            Program.FilesHandled += OnFilesHandled;

            IsWebShareSupported = await WebShareService.IsSupportedAsync();

            await base.OnInitializedAsync();
        }

        private async Task SetupDotNetHelper()
        {
            _objRef = DotNetObjectReference.Create(this);
            await JS.InvokeAsync<string>("setDotNetHelper", _objRef);
        }

        private async void OnFilesHandled(object? sender, Program.HandledFileEventArgs args)
        {
            if (args.File == null)
                return;

            SetFile(
                new DummyFile(
                    await args.File.GetNameAsync(),
                    await args.File.GetLastModifiedAsync(),
                    (long)await args.File.GetSizeAsync(),
                    await args.File.GetTypeAsync()
                ),
                new MemoryStream(await args.File.ArrayBufferAsync())
            );
        }

        private void SetFile(IBrowserFile file, Stream stream)
        {
            if (file == null)
                return;

            Logger.LogInformation("Handle file {FileName}.", file.Name);

            Model.File = file;
            Model.Input = stream;

            StateHasChanged();
        }

        private void OnInputFileChange(InputFileChangeEventArgs e)
        {
            Model.File = e.File;
            Model.Input?.Dispose();
            Model.Input = null;
            Model.Output = null;
            Model.OutputPreviewImage?.Dispose();
            Model.OutputPreviewImage = null;
            StateHasChanged();
        }

        private async Task Reset()
        {
            Model.Dispose();
            Model = new();
            LastException = null;
            await JS.InvokeVoidAsync("resetImage", "outputImage");
        }

        private const long MaxAllowedSize = 250 * 1000 * 1000;

        private async Task Submit()
        {
            Logger.LogInformation("Converting {Model}.", Model);

            try
            {
                IsLoading = true;
                LastException = null;

                Model.Output = null;
                Model.OutputPreviewImage?.Dispose();
                Model.OutputPreviewImage = null;

                if (Model.Input == null)
                {
                    Model.Input = new MemoryStream();
                    await Model.File!.OpenReadStream(MaxAllowedSize).CopyToAsync(Model.Input);
                }

                Model.Input.Position = 0;
                bool encodeSuccess = false;
                PdfAntiAliasing antiAliasing = default;
                var backgroundColor = SKColors.White;

                if (Model.AntiAliasingText)
                    antiAliasing |= PdfAntiAliasing.Text;
                if (Model.AntiAliasingImages)
                    antiAliasing |= PdfAntiAliasing.Images;
                if (Model.AntiAliasingPaths)
                    antiAliasing |= PdfAntiAliasing.Paths;

                if (Model.BackgroundColor != null && Model.BackgroundColor.StartsWith('#') && uint.TryParse(Model.BackgroundColor[1..], NumberStyles.HexNumber, null, out var parsed))
                {
                    parsed += (uint)(Model.Opacity << 24);
                    backgroundColor = (SKColor)parsed;
                }

                await Task.Factory.StartNew(() =>
                {
                    SKBitmap? inputToConvert = null;

                    try
                    {
                        if (Model.File?.ContentType == "application/pdf")
                        {
                            inputToConvert = PDFtoImage.Conversion.ToImage(
                                Model.Input,
                                leaveOpen: true,
                                password: !string.IsNullOrEmpty(Model.Password) ? Model.Password : null,
                                page: Model.Page,
                                dpi: Model.Dpi,
                                width: Model.Width,
                                height: Model.Height,
                                withAnnotations: Model.WithAnnotations,
                                withFormFill: Model.WithFormFill,
                                withAspectRatio: Model.WithAspectRatio,
                                rotation: Model.Rotation,
                                antiAliasing: antiAliasing,
                                backgroundColor: backgroundColor
                                );
                        }
                        else
                        {
                            using var memoryStream = new MemoryStream();
                            Model.Input.CopyTo(memoryStream);
                            memoryStream.Position = 0;
                            inputToConvert = SKBitmap.Decode(memoryStream);
                        }


                        Model.Output = PDFtoZPL.Conversion.ConvertBitmap(
                            inputToConvert,
                            encodingKind: Model.Encoding,
                            graphicFieldOnly: Model.GraphicFieldOnly,
                            setLabelLength: Model.SetLabelLength,
                            threshold: Model.Threshold,
                            ditheringKind: Model.Dithering
                        );

                        Model.OutputPreviewImage = new MemoryStream();

                        using var monochromeBitmap = inputToConvert.ToMonochrome(Model.Threshold, Model.Dithering);
                        encodeSuccess = monochromeBitmap.Encode(Model.OutputPreviewImage, SKEncodedImageFormat.Png, 100);
                    }
                    finally
                    {
                        inputToConvert?.Dispose();
                    }
                }, TaskCreationOptions.LongRunning);

                if (encodeSuccess)
                {
                    await SetImage();
                }
                else
                {
                    Model.OutputPreviewImage?.Dispose();
                    Model.OutputPreviewImage = null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to convert {Model}.", Model);
                LastException = ex;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CopyToClipboard()
        {
            if (Model.Output == null)
                return;

            await JS.InvokeVoidAsync("navigator.clipboard.writeText", Model.Output);
        }

        private async Task Download()
        {
            if (Model.Output == null)
                return;

            try
            {
                await JS.InvokeVoidAsync("downloadFileFromText", RenderRequest.GetOutputFileName(Model), Model.Output);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to download {Model}.", Model);
            }
        }

        private async Task Share()
        {
            if (Model.Output == null)
                return;

            try
            {
                var data = new WebShareDataModel
                {
                    Text = Model.Output
                };

                if (!await WebShareService.CanShareAsync(data))
                {
                    Logger.LogWarning("Cannot web share {Model}.", Model);
                    return;
                }

                await WebShareService.ShareAsync(data);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to web share {Model}.", Model);
            }
        }

        private async Task SetImage()
        {
            if (Model.OutputPreviewImage == null)
            {
                await JS.InvokeVoidAsync("resetImage", "outputImage");
                return;
            }

            Model.OutputPreviewImage.Position = 0;
            using var fs = new MemoryStream();
            await Model.OutputPreviewImage.CopyToAsync(fs);
            fs.Position = 0;

            using var streamRef = new DotNetStreamReference(fs);
            await JS.InvokeVoidAsync("setImage", "outputImage", RenderRequest.GetMimeType(Model.Format), streamRef);
        }

        [JSInvokable]
        public async Task ReceiveWebShareTargetAsync(string filesStringyfied)
        {
            try
            {
                var converted = JsonSerializer.Deserialize<FilesStringyfied>(filesStringyfied);
                var file = converted?.Files?.FirstOrDefault();

                if (file == null)
                    return;

                var data = Convert.FromBase64String(file.GetData());
                var ms = new MemoryStream(data.Length);
                await ms.WriteAsync(data);

                SetFile(
                    new DummyFile(
                        file.Name,
                        file.LastModified,
                        file.Size,
                        file.Type
                    ), ms);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to receive web share {FilesStringyfied}.", filesStringyfied);
            }
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _objRef?.Dispose();
                    _objRef = null;
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
