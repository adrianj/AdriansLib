using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace DTALib.Forms
{
    /// <summary>
    /// Defined types of messages: Success/Warning/Error.
    /// </summary>
    public enum TypeOfMessage
    {
        Success,
        Warning,
        Error,
		SameAsPrevious
    }
    /// <summary>
    /// Initiate instance of SplashScreen
    /// </summary>
    public class SplashScreen
    {

        static SplashScreenForm sf = null;
		static Form callingForm = null;
		static string formTitle = "";

		public static void StartSplashScreen()
		{
			StartSplashScreen(null);
		}

		/// <summary>
		/// Starts a new Splash Screen thread, hides the main form and shows the splash screen form.
		/// </summary>
		/// <param name="mainForm">The main form that is to be loaded.</param>
		public static void StartSplashScreen(Form mainForm)
		{
			StartSplashScreen(mainForm, null);
		}

		public static void StartSplashScreen(Form mainForm, SplashScreenOptions options)
		{
			callingForm = mainForm;
			formTitle = callingForm.Text;
			callingForm.Hide();
			Thread splashthread = new Thread(new ParameterizedThreadStart(SplashScreen.ShowSplashScreen));
			splashthread.IsBackground = true;
			splashthread.Start(options);
			Thread.Sleep(100);
			callingForm.Activate();
		}

        /// <summary>
        /// Displays the splashscreen
        /// </summary>
        static void ShowSplashScreen(object param)
		{
			if (sf == null)
			{
				SplashScreenOptions options;
				if (param is SplashScreenOptions)
					options = param as SplashScreenOptions;
				else
					options = new SplashScreenOptions();
				sf = new SplashScreenForm(options);
				sf.ShowSplashScreen();
			}
        }


        /// <summary>
        /// Closes the SplashScreen
        /// </summary>
        public static void CloseSplashScreen()
		{
			callingForm.Show();
			if (sf != null)
			{
				sf.CloseSplashScreen();
				sf = null;
			}

			callingForm.Activate();
        }

		void Close()
		{
			if (sf != null)
			{
				sf.CloseSplashScreen();
				sf = null;
			}
		}

        /// <summary>
        /// Update text in default green color of success message
        /// </summary>
        /// <param name="Text">Message</param>
        public static void UdpateStatusText(string Text)
        {
			UdpateStatusTextWithStatus(Text, TypeOfMessage.Success);
        }

        
        /// <summary>
        /// Update text with message color defined as green/yellow/red/ for success/warning/failure
        /// </summary>
        /// <param name="Text">Message</param>
        /// <param name="tom">Type of Message</param>
        public static void UdpateStatusTextWithStatus(string Text,TypeOfMessage tom)
		{
			if (sf != null)
				sf.UdpateStatusTextWithStatus(Text, tom);
        }


    }

}
