using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using Tesseract;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Clipboard = System.Windows.Clipboard;
using Point = System.Windows.Point;
using System.Text;

namespace ScreenOCR
{

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private MouseHook mh;
		public MainWindow() {
			InitializeComponent();
			thresholdLabel.Content = slider.Value;
			Mask.onMouseUp += new Mask.MouseUpHandler(GetImage);
			Mask.onMouseUp += () => Show();
			slider.ValueChanged += Slider_ValueChanged;

		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			thresholdLabel.Content = (int)e.NewValue;

		}
		#region Getting a mouse coordinates during mouse down and up events
		internal static Point startPoint = new Point();
		internal static Point endPoint = new Point();
		public void GetMouseDown(object sender, MouseHookEventArgs e) {
			startPoint.X = e.position.X;
			startPoint.Y = e.position.Y;
			Debug.WriteLine("{0},{1}", startPoint.X, startPoint.Y);
		}
		public void GetMouseUp(object sender, MouseHookEventArgs e) {
			endPoint.X = e.position.X;
			endPoint.Y = e.position.Y;
			if (mh != null) {
				mh.Dispose();
			}
			Debug.WriteLine("{0},{1}", endPoint.X, endPoint.Y);

		}

		#endregion


		public static Bitmap BitmapFromSource(BitmapSource source) {
			using (MemoryStream outStream = new MemoryStream()) {
				BitmapEncoder enc = new PngBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(source));
				enc.Save(outStream);
				Bitmap bitmap = new Bitmap(outStream);

				return new Bitmap(bitmap);
			}
		}
		private void DoOCR(Bitmap img) {
			try {
				using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
					//using (var img = ClipboardImgToBitmap())
					//using (var img = Pix.LoadFromFile(imagePath))

					using (var page = engine.Process(img)) {
						var text = page.GetText();
						text = text.Replace(" ", "");
						text = Regex.Replace(Regex.Replace(text, @"\n\n", "\n"), @"\n\n", "\n");
						outputBlock.Text = text;
					}


				}
			}
			catch (Exception e) {
				Debug.Indent();
				Trace.TraceError(e.ToString());
				Debug.WriteLine("Unexpected Error: " + e.Message);
				Debug.WriteLine("Details: ");
				Debug.WriteLine(e.ToString());
			}
		}

		private void GetImage() {

			int width = Math.Abs((int)endPoint.X - (int)startPoint.X - 4);
			int height = Math.Abs((int)endPoint.Y - (int)startPoint.Y - 4);
			if (height > 10 && width > 10) {
				using (Bitmap bitmap = ImageProcessor.GetBitmap((int)startPoint.X + 2, (int)startPoint.Y + 2, width, height)) {
					ImageProcessor.SetContrast(bitmap, (int)slider.Value);
					image.Source = ImageProcessor.ImageSourceFromBitmap(bitmap);
					DoOCR(bitmap);
				}
			}
		}

		

		private static void PrintByteArray(byte[] rgbValues) {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < 300; i++) {
				sb.Append(rgbValues[i] + ",");
			}
			Console.WriteLine(sb.ToString());
		}

		private void SelectAreaButton_Click(object sender, RoutedEventArgs e) {
			mh = new MouseHook();
			mh.MouseDown += new MouseHook.MouseDownEventHandler(GetMouseDown);
			mh.MouseUp += new MouseHook.MouseUpEventHandler(GetMouseUp);
			Hide();
			Mask mask = new Mask();
			mask.Show();
		}

	}
}
