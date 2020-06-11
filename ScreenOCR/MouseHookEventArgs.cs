using System.Drawing;

namespace ScreenOCR
{
	public class MouseHookEventArgs
	{
		public Point position;
		
		public MouseHookEventArgs(Point cursorCoordinates) {
			position = cursorCoordinates;
		}

	}
}
