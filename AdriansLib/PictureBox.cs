/* Title:
 * PictureBox with zoom and scroll functionallity
 * 
 * Author:
 * Alexander Kloep Apr. 2005
 * Alexander.Kloep@gmx.net
 * 
 * Reason:
 * In a past project i designed a GUI with a PictureBox control on it. Because of the low screen 
 * resolution i couldn´t make the GUI big enough to show the whole picture. So i decided to develop
 * my own scrollable picturebox with the special highlight of zooming functionallity.
 * 
 * The solution: 
 * When the mouse cursor enters the ctrl, the cursorstyle changes and you are able to zoom in or out 
 * with the mousewheel. The princip of the zooming effect is to raise or to lower the inner picturebox 
 * size by a fixed zooming factor. The scroolbars appear automatically when the inner picturebox
 * gets bigger than the ctrl.
 *  
 * Here it is...
 * 
 * Last modification: 06/04/2005
 * 
 * Modified by Adrian Jongenelen: 22/03/2011
 *      MouseWheel events not working, so added context menu with Zoom in and out menu items.
 *      Added public access to the Image.
 *      private PicBox aspect ratio is based on Image.
 */

#region Usings

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data;
using System.Windows.Forms;

#endregion

namespace DTALib
{
	/// <summary>
	/// Summary for the PictureBox Ctrl
	/// </summary>
	public class ZoomablePictureBox : System.Windows.Forms.UserControl
	{
		#region Members

		private System.Windows.Forms.PictureBox PicBox;
        private System.Windows.Forms.Panel OuterPanel;
        private IContainer components;
		private string m_sPicName = "";
        private Image mImage;
        public Image Image
        {
            get { return mImage; }
            set
            {
                if (value != null)
                {
                    mImage = value;
                    // Correct aspect ratio.
                    float panelRatio = (float)OuterPanel.Width / (float)OuterPanel.Height;
                    float picRatio = (float)value.Width / (float)value.Height;
                    if (panelRatio > picRatio) // Set image height = panel height.
                    {
                        PicBox.Height = OuterPanel.Height - 2;
                        PicBox.Width = (int)((float)PicBox.Height * picRatio);
                    }
                    else // Set image width = panel width
                    {
                        PicBox.Width = OuterPanel.Width - 2;
                        PicBox.Height = (int)((float)PicBox.Width / picRatio);
                    }
                    ResizeImage(PicBox.Size);
                }
            }
        }

		#endregion

		#region Constants

		private double ZOOMFACTOR = 2;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem zoomInToolStripMenuItem;
        private ToolStripMenuItem zoomOutToolStripMenuItem;	
		private int MINMAX = 8;				// 8 times bigger or smaller than the ctrl

		#endregion

		#region Designer generated code

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.PicBox = new System.Windows.Forms.PictureBox();
            this.OuterPanel = new System.Windows.Forms.Panel();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.PicBox)).BeginInit();
            this.OuterPanel.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // PicBox
            // 
            this.PicBox.Location = new System.Drawing.Point(0, 0);
            this.PicBox.Name = "PicBox";
            this.PicBox.Size = new System.Drawing.Size(150, 140);
            this.PicBox.TabIndex = 3;
            this.PicBox.TabStop = false;
            // 
            // OuterPanel
            // 
            this.OuterPanel.AutoScroll = true;
            this.OuterPanel.ContextMenuStrip = this.contextMenuStrip;
            this.OuterPanel.Controls.Add(this.PicBox);
            this.OuterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OuterPanel.Location = new System.Drawing.Point(0, 0);
            this.OuterPanel.Name = "OuterPanel";
            this.OuterPanel.Size = new System.Drawing.Size(210, 190);
            this.OuterPanel.TabIndex = 4;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(120, 48);
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.zoomInToolStripMenuItem.Text = "Zoom in";
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.zoomInToolStripMenuItem_Click);
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.zoomOutToolStripMenuItem.Text = "Zoom out";
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.zoomOutToolStripMenuItem_Click);
            // 
            // ZoomablePictureBox
            // 
            this.Controls.Add(this.OuterPanel);
            this.Name = "ZoomablePictureBox";
            this.Size = new System.Drawing.Size(210, 190);
            ((System.ComponentModel.ISupportInitialize)(this.PicBox)).EndInit();
            this.OuterPanel.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region Constructors

		public ZoomablePictureBox()
		{
			InitializeComponent ();
			InitCtrl ();	// my special settings for the ctrl
		}

		#endregion

		#region Properties

		/// <summary>
		/// Property to select the picture which is displayed in the picturebox. If the 
		/// file doesn´t exist or we receive an exception, the picturebox displays 
		/// a red cross.
		/// </summary>
		/// <value>Complete filename of the picture, including path information</value>
		/// <remarks>Supported fileformat: *.gif, *.tif, *.jpg, *.bmp</remarks>
		[ Browsable ( false ) ]
		public string Picture
		{
			get { return m_sPicName; }
			set 
			{
				if ( null != value )
				{
					if ( System.IO.File.Exists ( value ) )
					{
						try
						{
							PicBox.Image = Image.FromFile ( value );
							m_sPicName = value;
						}
						catch ( OutOfMemoryException)
						{
							RedCross ();
						}
					}
					else
					{				
						RedCross ();
					}
				}
			}
		}

        /*
		/// <summary>
		/// Set the frametype of the picturbox
		/// </summary>
		[ Browsable ( false ) ]
		public BorderStyle Border
		{
			get { return OuterPanel.BorderStyle; }
			set { OuterPanel.BorderStyle = value; }
		}
         */

		#endregion

		#region Other Methods

		/// <summary>
		/// Special settings for the picturebox ctrl
		/// </summary>
		private void InitCtrl ()
		{
			//PicBox.SizeMode = PictureBoxSizeMode.StretchImage;
			PicBox.Location = new Point ( 0, 0 );
			OuterPanel.Dock = DockStyle.Fill;
			OuterPanel.Cursor = System.Windows.Forms.Cursors.NoMove2D;
			OuterPanel.AutoScroll = true;
			OuterPanel.MouseClick += new MouseEventHandler(PicBox_MouseClick);
            PicBox.MouseClick += new MouseEventHandler(PicBox_MouseClick);
			OuterPanel.MouseWheel += new MouseEventHandler(PicBox_MouseWheel);
		}


		/// <summary>
		/// Create a simple red cross as a bitmap and display it in the picturebox
		/// </summary>
		private void RedCross ()
		{
            /*
			Bitmap bmp = new Bitmap ( OuterPanel.Width, OuterPanel.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555 );
            
			Graphics gr;
			gr = Graphics.FromImage ( bmp );
			Pen pencil = new Pen ( Color.Red, 5 );
			gr.DrawLine ( pencil, 0, 0, OuterPanel.Width, OuterPanel.Height );
			gr.DrawLine ( pencil, 0, OuterPanel.Height, OuterPanel.Width, 0  );
			gr.Dispose ();
             
            PicBox.Image = bmp;
            */
		}

		#endregion

		#region Zooming Methods

        public void ResizeImage(Size size)
        {
            Bitmap original = (Bitmap)mImage;
            Bitmap b = new Bitmap(size.Width, size.Height, PixelFormat.Format16bppRgb555);
            Graphics g = Graphics.FromImage((Image)b);
            // If going from low res to high res, I want to keep it pixelated
            if (size.Width > original.Width && size.Height > original.Height)
            {
                float scaleW = size.Width / original.Width;
                float scaleH = size.Height / original.Height;
                for (int x = 0; x < original.Width; x++)
                    for (int y = 0; y < original.Height; y++)
                    {
                        Color c = original.GetPixel(x, y);
                        g.FillRectangle(new SolidBrush(c), x * scaleW, y * scaleH, scaleW, scaleH);
                    }
            }
            else
            {
                g.DrawImage(original, 0, 0, size.Width, size.Height);
            }

            g.Dispose();

            PicBox.Image = b;
        }

		/// <summary>
		/// Make the PictureBox dimensions larger to effect the Zoom.
		/// </summary>
		/// <remarks>Maximum 5 times bigger</remarks>
		private void ZoomIn() 
		{
			if ( ( PicBox.Width < ( MINMAX * OuterPanel.Width ) ) &&
				( PicBox.Height < ( MINMAX * OuterPanel.Height ) ) )
			{
                int newW = Convert.ToInt32(PicBox.Width * ZOOMFACTOR);
                int newH = Convert.ToInt32(PicBox.Height * ZOOMFACTOR);
                PicBox.Width = newW;
                PicBox.Height = newH;
                if (newH * newW < 2000000)
                {
                    PicBox.SizeMode = PictureBoxSizeMode.Normal;
                    ResizeImage(PicBox.Size);
                }
                else
                {
                    PicBox.SizeMode = PictureBoxSizeMode.StretchImage;
                }
			}
		}

		/// <summary>
		/// Make the PictureBox dimensions smaller to effect the Zoom.
		/// </summary>
		/// <remarks>Minimum 5 times smaller</remarks>
		private void ZoomOut() 
		{
            if ((PicBox.Width > (OuterPanel.Width / MINMAX)) &&
                (PicBox.Height > (OuterPanel.Height / MINMAX)))
            {
                int newW = Convert.ToInt32(PicBox.Width / ZOOMFACTOR);
                int newH = Convert.ToInt32(PicBox.Height / ZOOMFACTOR);
                PicBox.Width = newW;
                PicBox.Height = newH;
                if (newH * newW < 2000000)
                {
                    PicBox.SizeMode = PictureBoxSizeMode.Normal;
                    ResizeImage(PicBox.Size);
                }
                else
                {
                    PicBox.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
		}

		#endregion

		#region Mouse events

		/// <summary>
		/// We use the mousewheel to zoom the picture in or out
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PicBox_MouseWheel(object sender, MouseEventArgs e)
		{
            Console.WriteLine("Wheeling: " + e.Delta);
			if ( e.Delta < 0 )
			{
				ZoomIn ();
			}
			else
			{
				ZoomOut ();
			}
		}


        void PicBox_MouseClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Clicked on pixturebox control");
            if (PicBox.Focused == false)
            {
                PicBox.Focus();
            }
        }

		#endregion

		#region Disposing

		/// <summary>
		/// Die verwendeten Ressourcen bereinigen.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#endregion

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }
	}
}
