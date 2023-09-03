using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using PDFtoZPL.WebConverter.Models;
using System;
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

		private async void OnFilesHandled(object? sender, Program.HandledFileArgs args)
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

		private async Task OnInputFileChange(InputFileChangeEventArgs e)
		{
			Model.File = e.File;
			Model.Input?.Dispose();
			Model.Input = null;
			Model.Output = null;
			StateHasChanged();
		}

		private void Reset()
		{
			Model.Dispose();
			Model = new();
			LastException = null;
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

				if (Model.Input == null)
				{
					Model.Input = new MemoryStream();
					await Model.File!.OpenReadStream(MaxAllowedSize).CopyToAsync(Model.Input);
				}

				Model.Input.Position = 0;

				await Task.Run(() =>
				{
					if (Model.File?.ContentType == "application/pdf")
					{
						Model.Output = PDFtoZPL.Conversion.ConvertPdfPage(
							Model.Input,
							leaveOpen: true,
							password: !string.IsNullOrEmpty(Model.Password) ? Model.Password : null,
							page: Model.Page,
							dpi: Model.Dpi,
							width: Model.Width,
							height: Model.Height,
							withAnnotations: Model.WithAnnotations,
							withFormFill: Model.WithFormFill,
							encodingKind: Conversion.BitmapEncodingKind.Base64Compressed,
							graphicFieldOnly: false,
							withAspectRatio: Model.WithAspectRatio,
							setLabelLength: false,
							rotation: Model.Rotation
						);
					}
					else
					{
						Model.Output = PDFtoZPL.Conversion.ConvertBitmap(
							Model.Input,
							leaveOpen: true,
							encodingKind: Model.Encoding,
							graphicFieldOnly: Model.GraphicFieldOnly,
							setLabelLength: Model.SetLabelLength
						);
					}
				});
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