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
using Gma.UserActivityMonitor;
using System.Windows.Forms;
using Clipboard = System.Windows.Clipboard;
using Point = System.Drawing.Point;

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
			HookManager.MouseDown += new MouseEventHandler(GetMouseDown);
			HookManager.MouseUp += new MouseEventHandler(GetMouseUp);
			Mask.onMouseUp += new Mask.MouseUpHandler(ProcessImage);
			Mask.onMouseUp += () => Show();
		}
		#region Getting a mouse coordinates during mouse down and up events
		private static Point startPoint = new Point();
		private static Point endPoint = new Point();
		public void GetMouseDown(object sender, MouseEventArgs e)
		{
			startPoint.X = e.X;
			startPoint.Y = e.Y;
			Debug.WriteLine("{0},{1}", startPoint.X, startPoint.Y);
		}
		public void GetMouseUp(object sender, MouseEventArgs e)
		{
			endPoint.X = e.X;
			endPoint.Y = e.Y;

			Debug.WriteLine("{0},{1}", endPoint.X, endPoint.Y);

		}

		#endregion


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
		
		private void ProcessImage()
		{
			int width = Math.Abs(endPoint.X - startPoint.X);
			int height = Math.Abs(endPoint.Y - startPoint.Y);
			if (height < 5 || width < 5)
			{
				message.Text = "The image is too small.";
			}
			else
			{

				Bitmap bitmap = AcquireImage.GetBitmap(startPoint.X, startPoint.Y, width, height);
				image.Source = AcquireImage.ImageSourceFromBitmap(bitmap);
				DoOCR(bitmap);
			}
		}
		private void SelectAreaButton_Click(object sender, RoutedEventArgs e)
		{
			Hide();
			Mask mask = new Mask();
			mask.Show();
		}
		
	}
}
