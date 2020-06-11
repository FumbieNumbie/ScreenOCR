using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScreenOCR
{
	class ImageProcessor
	{
		public static Bitmap GetBitmap(int startX, int startY, int width, int height) {
			Bitmap bmp = new Bitmap(width, height);
			using (Graphics g = Graphics.FromImage(bmp)) {
				//g.CopyFromScreen(startX, startY, startX + width, startY + height, bmp.Size);
				g.CopyFromScreen(startX, startY, 0, 0, bmp.Size);

			}
			return bmp;
		}
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject([In] IntPtr hObject);

		public static ImageSource ImageSourceFromBitmap(Bitmap bmp) {
			var handle = bmp.GetHbitmap();
			try {
				return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			}
			finally { DeleteObject(handle); }
		}
		internal static void SetContrast(Bitmap bitmap, int threshold) {
			Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
			System.Drawing.Imaging.BitmapData bmpData =
				bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
				bitmap.PixelFormat);

			// Get the address of the first line.
			IntPtr ptr = bmpData.Scan0;

			// Declare an array to hold the bytes of the bitmap.
			int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
			byte[] rgbValues = new byte[bytes];

			// Copy the RGB values into the array.
			Marshal.Copy(ptr, rgbValues, 0, bytes);


			double contrast = Math.Pow(((100 + (double)threshold) / 100), 2);

			// Adjust contrast
			//First 3 bytes are colours, so we cicle through those and skip the 4th.
			for (int i = 0; i < rgbValues.Length - 3; i += 4) {
				for (int j = 0; j < 3; j++) {
					
					double newValue = (((((double)rgbValues[i + j] / 255) - 0.5) * (double)contrast) + 0.5) * 255;


					Console.WriteLine((double)rgbValues[i + j] + "  " + newValue);
					if (newValue > 255) newValue = 255;
					if (newValue < 0) newValue = 0;
					rgbValues[i + j] = (byte)(int)newValue;
				}
			}
			//PrintByteArray(rgbValues);

			// Copy the RGB values back to the bitmap
			Marshal.Copy(rgbValues, 0, ptr, bytes);

			// Unlock the bits.
			bitmap.UnlockBits(bmpData);
		}
		
	}
}
