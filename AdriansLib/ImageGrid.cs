using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AdriansLib
{
    /// <summary>
    /// A control for displaying a 2Dimensional grid of images.
    /// Each element is a PictureBox which can contain an Image.
    /// By default, this image is a Bitmap stored in memory.
    /// </summary>
    public partial class ImageGrid : UserControl
    {
        /// <summary>
        /// The array of PictureBox elements. Indexing counts along the width first, then height.
        /// </summary>
        public PictureBox[] Elements { get; set; }
        /// <summary>
        /// The Images associated with the elements. Indexing counts along the width first, then height.
        /// </summary>
        public Image[] ElementImages { get; set; }
        private int mHeight = 2;
        private int mWidth = 2;
        /// <summary>
        /// The default width in pixels of Bitmap images associated with the elements.
        /// </summary>
        public int DefaultImageWidth { get; set; }
        /// <summary>
        /// The default height in pixels of Bitmap images associated with the elements.
        /// </summary>
        public int DefaultImageHeight { get; set; }
        
        /// <summary>
        /// The number of rows in the ImageGrid
        /// Changing this value causes all elements to be recreated.
        /// </summary>
        public int ElementHeight
        {
            get { return mHeight; }
            set
            {
                if (value >= 1 && value != mHeight)
                {
                    mHeight = value;
                    RecreateElements();
                }
            }
        }
        /// <summary>
        /// The number of columns in the ImageGrid.
        /// Changing this value causes all elements to be recreated.
        /// </summary>
        public int ElementWidth
        {
            get { return mWidth; }
            set
            {
                if (value >= 1 && value != mWidth)
                {
                    mWidth = value;
                    RecreateElements();
                }
            }
        }

        private void RecreateElements()
        {
            if (Elements != null)
            {
                foreach(PictureBox el in Elements)
                    this.Controls.Remove(el);
            }
            Elements = new PictureBox[ElementWidth*ElementHeight];
            ElementImages = new Bitmap[ElementWidth * ElementHeight];
            this.SuspendLayout();
            for (int i = 0; i < ElementWidth * ElementHeight; i++)
            {
                Elements[i] = new PictureBox();
                Elements[i].Name = "Element_" + i;
                Elements[i].BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                Elements[i].BackgroundImageLayout = ImageLayout.Stretch;
                ElementImages[i] = new Bitmap(DefaultImageWidth, DefaultImageHeight);
                Elements[i].BackgroundImage = ElementImages[i];
                Elements[i].Tag = (int)i;
                this.Controls.Add(Elements[i]);
            }
            ResizeElements();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ResizeElements()
        {
            for (int i = 0; i < ElementWidth * ElementHeight; i++)
            {
                int x = i % ElementWidth;
                int y = i / ElementWidth;
                Elements[i].Location = new Point(x * this.Width / ElementWidth, y * this.Height / ElementHeight);
                Elements[i].Size = new Size(this.Width / ElementWidth, Height / ElementHeight);
            }
            Refresh();
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ImageGrid()
        {
            DefaultImageWidth = 128;
            DefaultImageHeight = 128;
            RecreateElements();
            InitializeComponent();
        }

        private void ImageGrid_Paint(object sender, PaintEventArgs e)
        {
        }

        private void ImageGrid_Resize(object sender, EventArgs e)
        {
            ResizeElements();
        }

    }
}
