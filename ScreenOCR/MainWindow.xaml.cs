using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using Tesseract;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ScreenOCR
{


	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			CheckClipboard();
		}
		private void CheckClipboard()
		{
			if (Clipboard.ContainsImage())
			{
				BitmapSource source = Clipboard.GetImage();
				image.Source = source;
				Bitmap bitmap = BitmapFromSource(source);
				DoOCR(bitmap);
			}
			else
			{
				message.Text = "Clipboard contains no image";
			}
		}

		public static Bitmap BitmapFromSource(BitmapSource source)
		{
			using (MemoryStream outStream = new MemoryStream())
			{
				BitmapEncoder enc = new PngBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(source));
				enc.Save(outStream);
				Bitmap bitmap = new Bitmap(outStream);

				return new Bitmap(bitmap);
			}
		}
		private void DoOCR(Bitmap img)
		{

			try
			{
				using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
				{
					//using (var img = ClipboardImgToBitmap())
					//using (var img = Pix.LoadFromFile(imagePath))

					using (var page = engine.Process(img))
					{
						var text = page.GetText();
						text = text.Replace(" ", "");
						text = Regex.Replace(Regex.Replace(text, @"\n\n", "\n"), @"\n\n", "\n");
						outputBlock.Text = text;
					}


				}
			}
			catch (Exception e)
			{
				Debug.Indent();
				Trace.TraceError(e.ToString());
				Debug.WriteLine("Unexpected Error: " + e.Message);
				Debug.WriteLine("Details: ");
				Debug.WriteLine(e.ToString());
			}
		}

		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			CheckClipboard();
		}
	}
}
