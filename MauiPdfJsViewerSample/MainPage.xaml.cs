using iText.IO.Image;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using System.Net;
using iText.Layout;

namespace MauiPdfJsViewerSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

#if ANDROID
            Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("pdfviewer", (handler, View) =>
            {
                handler.PlatformView.Settings.AllowFileAccess = true;
                handler.PlatformView.Settings.AllowFileAccessFromFileURLs = true;
                handler.PlatformView.Settings.AllowUniversalAccessFromFileURLs = true;
            });

            pdfview.Source = $"file:///android_asset/pdfjs/web/viewer.html?file=file:///android_asset/{WebUtility.UrlEncode("mypdf.pdf")}";
#else
            pdfview.Source = "mypdf.pdf";
#endif
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            string fileName = "mauidotnet.pdf";

#if ANDROID
		var docsDirectory = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments);
		var filePath = Path.Combine(docsDirectory.AbsoluteFile.Path, fileName);
#else
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
#endif
            using (PdfWriter writer = new PdfWriter(filePath))
            {
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);
                Paragraph header = new Paragraph("MAUI PDF Sample")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(20);

                document.Add(header);
                Paragraph subheader = new Paragraph("Welcome to .NET Multi-platform App UI")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(15);
                document.Add(subheader);
                LineSeparator ls = new LineSeparator(new SolidLine());
                document.Add(ls);
                var imgStream = await ConvertImageSourceToStreamAsync("dotnet_bot.png");
                iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory
                    .Create(imgStream))
                    .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);

                document.Add(image);

                Paragraph footer = new Paragraph("Don't forget to like and subscribe!")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY)
                    .SetFontSize(14);

                document.Add(footer);
                document.Close();
            }

#if ANDROID
            pdfview.Source = $"file:///android_asset/pdfjs/web/viewer.html?file=file://{WebUtility.UrlEncode(filePath)}";
#else
            pdfview.Source = filePath;
#endif
        }

        private async Task<byte[]> ConvertImageSourceToStreamAsync(string imageName)
        {
            using var ms = new MemoryStream();
            using (var stream = await FileSystem.OpenAppPackageFileAsync(imageName))
                await stream.CopyToAsync(ms);
            return ms.ToArray();
        }
    }

}
