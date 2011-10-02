using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DTALib.Forms
{
	public class SplashScreenOptions : Component
	{
		private string startText = "Loading... ";
		public string StartingText { get { return startText; } set { startText = value; } }

		private Color successColor = Color.Black;
		public Color SuccessColor { get { return successColor; } set { successColor = value; } }
		private Color warningColor = Color.DarkOrange;
		public Color WarningColor { get { return warningColor; } set { warningColor = value; } }
		private Color errorColor = Color.Red;
		public Color ErrorColor { get { return errorColor; } set { errorColor = value; } }
		private Color bgColor = Color.Gray;
		public Color BackgroundColor { get { return bgColor; } set { bgColor = value; } }
		private Color transColor = Color.Transparent;
		public Color TransparencyColor { get { return transColor; } set { transColor = value; } }


		public Image SplashScreenImage { get; set; }

	}
}
