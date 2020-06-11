using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using Point = System.Windows.Point;

namespace ScreenOCR
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Mask : Window
	{
		public Mask()
		{
			InitializeComponent();
		}
		bool mouseDown = false; // Set to 'true' when mouse is held down.
		Point mouseDownPos; // The point where the mouse button was clicked down.


		private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			// Capture and track the mouse.
			mouseDown = true;
			mouseDownPos = e.GetPosition(theGrid);
			theGrid.CaptureMouse();
			// Initial placement of the drag selection box.         
			Canvas.SetLeft(selectionBox, mouseDownPos.X);
			Canvas.SetTop(selectionBox, mouseDownPos.Y);
			selectionBox.Width = 0;
			selectionBox.Height = 0;
			// Make the drag selection box visible.
			selectionBox.Visibility = Visibility.Visible;
		}

		private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
		{
			// Release the mouse capture and stop tracking it.
			mouseDown = false;
			theGrid.ReleaseMouseCapture();
			// Hide the drag selection box.
			selectionBox.Visibility = Visibility.Collapsed;
			Point mouseUpPos = e.GetPosition(theGrid);
			//int dX = Math.Abs((int)(mouseDownPos.X - mouseUpPos.X));
			//int dY = Math.Abs((int)(mouseDownPos.Y - mouseUpPos.Y));
			//int resultX = (int)((mouseUpPos.X > mouseDownPos.X) ? mouseDownPos.X : mouseUpPos.X);
			//int resultY = (int)((mouseUpPos.Y > mouseDownPos.Y) ? mouseDownPos.Y : mouseUpPos.Y);
			//MainWindow.bitmap = AcquireImage.GetBitmap(resultX, resultY, dX, dY);
			//Close the mask window
			
			onMouseUp?.Invoke();
			Close();
		}
		public delegate void MouseUpHandler();
		public static event MouseUpHandler onMouseUp;

		private void Grid_MouseMove(object sender, MouseEventArgs e)
		{
			if (mouseDown)
			{
				// When the mouse is held down, reposition the drag selection box.

				Point mousePos = e.GetPosition(theGrid);

				if (mouseDownPos.X < mousePos.X)
				{
					Canvas.SetLeft(selectionBox, Math.Max(0,mouseDownPos.X));
					selectionBox.Width = mousePos.X - mouseDownPos.X ;
				}
				else
				{
					Canvas.SetLeft(selectionBox, Math.Max(0,mousePos.X));
					selectionBox.Width = mouseDownPos.X - mousePos.X ;
				}

				if (mouseDownPos.Y < mousePos.Y)
				{
					Canvas.SetTop(selectionBox, Math.Max(0,mouseDownPos.Y));
					selectionBox.Height = mousePos.Y - mouseDownPos.Y ;
				}
				else
				{
					Canvas.SetTop(selectionBox, Math.Max(0, mousePos.Y));
					selectionBox.Height = mouseDownPos.Y - mousePos.Y ;
				}
			}
		}
	}

}
