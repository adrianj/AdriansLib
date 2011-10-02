using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DTALib.Forms
{
    partial class SplashScreenForm : Form
    {
        delegate void StringParameterWithStatusDelegate(string Text, TypeOfMessage tom);
        delegate void SplashShowCloseDelegate();

		SplashScreenOptions options;

        /// <summary>
        /// To ensure splash screen is closed using the API and not by keyboard or any other things
        /// </summary>
        bool CloseSplashScreenFlag = false;

        /// <summary>
        /// Base constructor
        /// </summary>
        public SplashScreenForm(SplashScreenOptions options)
        {
			if (options != null)
				this.options = options;
			else
				this.options = new SplashScreenOptions();
            InitializeComponent();
            this.label1.Parent = this.pictureBox1;
            this.label1.BackColor = Color.Transparent;
			label1.ForeColor = options.SuccessColor;
			this.TransparencyKey = options.TransparencyColor;
			this.BackColor = options.BackgroundColor;
			this.label1.Text = options.StartingText;
			this.Text = "Loading... ";
			if (options.SplashScreenImage != null)
			{
				this.pictureBox1.Image = options.SplashScreenImage;
			}
			if(this.pictureBox1.Image != null)
			{
				Size size = this.pictureBox1.Image.Size;
				size.Height += this.progressBar1.Height;
				this.Size = size;
			}

            progressBar1.Show();
        }

        /// <summary>
        /// Displays the splashscreen
        /// </summary>
        public void ShowSplashScreen()
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new SplashShowCloseDelegate(ShowSplashScreen));
                return;
            }
			this.Show();
            Application.Run(this);
        }

        /// <summary>
        /// Closes the SplashScreen
        /// </summary>
        public void CloseSplashScreen()
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new SplashShowCloseDelegate(CloseSplashScreen));
                return;
            }
            CloseSplashScreenFlag = true;
            this.Close();
        }



        /// <summary>
        /// Update text with message color defined as green/yellow/red/ for success/warning/failure
        /// </summary>
        /// <param name="Text">Message</param>
        /// <param name="tom">Type of Message</param>
        public void UdpateStatusTextWithStatus(string Text, TypeOfMessage tom)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new StringParameterWithStatusDelegate(UdpateStatusTextWithStatus), new object[] { Text, tom });
                return;
            }
            // Must be on the UI thread if we've got this far
            switch (tom)
            {
                case TypeOfMessage.Error:
					label1.ForeColor = options.ErrorColor;
                    break;
                case TypeOfMessage.Warning:
					label1.ForeColor = options.WarningColor;
                    break;
                case TypeOfMessage.Success:
					label1.ForeColor = options.SuccessColor;
                    break;
            }
            label1.Text = Text;

        }

        /// <summary>
        /// Prevents the closing of form other than by calling the CloseSplashScreen function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SplashForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseSplashScreenFlag == false)
                e.Cancel = true;
        }
    }
}
