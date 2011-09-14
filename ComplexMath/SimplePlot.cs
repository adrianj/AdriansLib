using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.IO;

namespace ComplexMath
{
    public partial class SimplePlot : UserControl
    {
		public enum AxisScaleType { Auto, Square, UserDefined };
		private AxisScaleType scaleType = AxisScaleType.Auto;
		public AxisScaleType ScaleType
		{
			get { return scaleType; }
			set
			{
				scaleType = value;
				scaleMenu.Text = "Axis Scale Type: " + ScaleType; 
				ScaleAxes();
				UpdateAxes();
			}
		}

		private int maxSamples = 10000;
		public int MaxSamples { get { return maxSamples; } set { maxSamples = value; } }
		private int decimation = 1;
        public ICFunction [] ComplexFunctions { get; set; }

        private double mAspectRatio = 0;
        public double AspectRatio
        {
            get
            {
                return mAspectRatio;
            }
            set
            {
                mAspectRatio = value;
                updateAspectRatio();
                scaleBackground();
            }
        }

		public static Color[] ColorList = { Color.Blue, Color.Red, Color.Green, Color.Orange, 
                                            Color.Purple, Color.Black, Color.Yellow, Color.Cyan,
                                            Color.LightSkyBlue, Color.Gray, Color.LimeGreen, Color.Maroon,
                                            Color.DarkViolet, Color.DarkGray, Color.SandyBrown, Color.Magenta
                                          };

        //public bool ContinuousUpdate { get; set; }
        public DateTime mStartTime { get; set; }

        public PointF YAxisRange { get; set; }
        public PointF XAxisRange { get; set; }

        private CurveList mCurveList = new CurveList();
		private CurveList highlightList = new CurveList();

        public string YLabel
        {
            get { return graph.GraphPane.YAxis.Title.Text; }
            set { graph.GraphPane.YAxis.Title.Text = value; this.Refresh(); }
        }
        public string XLabel
        {
            get { return graph.GraphPane.XAxis.Title.Text; }
            set { graph.GraphPane.XAxis.Title.Text = value; this.Refresh(); }
        }
        public string Title
        {
            get { return graph.GraphPane.Title.Text; }
            set { graph.GraphPane.Title.Text = value; this.Refresh(); }
        }

        private double[] mYScale = new double[]{1,1,1,1,1};
        public double []YScale { get { return mYScale; } set { mYScale = value; } }
        private double []mYOffset = new double[]{-2,-1,0,1,2};
        public double []YOffset { get { return mYOffset; } set { mYOffset = value; } }
        private bool mEnableScaling = false;
        public bool EnableScaling { get { return mEnableScaling; } set { mEnableScaling = value; } }

        public PointPairList[] PlotData
        {
            get {
                PointPairList []ret = new PointPairList[graph.GraphPane.CurveList.Count];
                for (int i = 0; i < ret.Length; i++)
                    ret[i] = graph.GraphPane.CurveList[i].Points as PointPairList;
                return ret;
            }
        }

        public string[] PlotLabels
        {
            get
            {
                string[] ret = new string[graph.GraphPane.CurveList.Count];
                for (int i = 0; i < ret.Length; i++)
                    ret[i] = graph.GraphPane.CurveList[i].Label.Text;
                return ret;
            }
        }


        public bool ShowLegend { get; set; }
        
        public string AxesString
        {
            get { return "" + XAxisRange.X + "," + XAxisRange.Y + "," + YAxisRange.X + "," + YAxisRange.Y; }
            set
            {
                string[] ax = value.Split(new char[] { ',' });
                try
                {
                    if (ax.Length != 4) throw new FormatException();
                    PointF p = new PointF(float.Parse(ax[0]), float.Parse(ax[1]));
                    XAxisRange = p;
                    p = new PointF(float.Parse(ax[2]), float.Parse(ax[3]));
                    YAxisRange = p;
					if (XAxisRange.X != 0 || XAxisRange.Y != 0 || YAxisRange.X != 0 || YAxisRange.Y != 0)
						this.ScaleType = AxisScaleType.UserDefined;
                }
                catch (FormatException) { }
            }
        }
        public string YScaleString
        {
            get {
                string ret = "";
                foreach (double d in YScale)
                    ret += "" + d + ",";
                return ret;
            }
            set
            {
                string[] sc = value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                YScale = new double[sc.Length];
                for (int i = 0; i < sc.Length; i++)
                {
                    double d = 1;
                    double.TryParse(sc[i], out d);
                    YScale[i] = d;
                }
            }
        }
        public string YOffsetString
        {
            get
            {
                string ret = "";
                foreach (double d in YOffset)
                    ret += "" + d + ",";
                return ret;
            }
            set
            {
                string[] sc = value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                YOffset = new double[sc.Length];
                for (int i = 0; i < sc.Length; i++)
                {
                    double d = 1;
                    double.TryParse(sc[i], out d);
                    YOffset[i] = d;
                }
            }
        }
        public string ComplexFunctionsString
        {
            get
            {
                return BaseFunction.FunctionsToString(ComplexFunctions);
            }
        }

		public SimplePlot()
		{
			InitializeComponent();
			InitializeContextMenu();
			graph.IsShowPointValues = true;
			Console.WriteLine("context; " + graph.ContextMenuStrip);
			this.BackgroundImageLayout = ImageLayout.None;
			YAxisRange = new Point(0, 0);
			XAxisRange = new Point(0, 0);
		}

        private string mChartFilename = null;
        public string ChartImageFilename
        {
            get { return mChartFilename; }
            set
            {
                if (value == null || value.Length < 1)
                {
                    mChartFilename = "";
                    ChartBackgroundImage = null;
                    return;
                }
                mChartFilename = value;
                if (!File.Exists(mChartFilename))
                { MessageBox.Show("Image file '" + mChartFilename + "' does not exist. Ignoring for background image."); return; }
                try
                {
                    ChartBackgroundImage = new Bitmap(mChartFilename);
                }
                catch (Exception e) { MessageBox.Show("Error loading image '" + mChartFilename + "'\n" + e); }
            }
        }

        private Image mBackground;
        public Image ChartBackgroundImage { get { return mBackground; } set { mBackground = value; scaleBackground(); } }


		public void SetComplexFunctions(string functionString, NodeLabelCallback FunctionCallback)
		{
			string[] ss = functionString.Split(new string[] { ";" }, StringSplitOptions.None);
			ICFunction[] cft = new ICFunction[ss.Length];
			for (int i = 0; i < ss.Length; i++)
			{
				cft[i] = BaseFunction.CreateNode(ss[i], FunctionCallback);
			}
			ComplexFunctions = cft;
		}
        private void scaleBackground()
        {
            if (mBackground != null)
            {
                RectangleF r = graph.GraphPane.Rect;
                Bitmap b = new Bitmap((int)r.Width, (int)r.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                Graphics g = Graphics.FromImage(b);

                g.InterpolationMode = InterpolationMode.High;
                g.DrawImage(mBackground, new Rectangle(0, 0, b.Width, b.Height));
                g.Dispose();
                graph.GraphPane.Chart.Fill = new Fill(b, System.Drawing.Drawing2D.WrapMode.Clamp);
                Refresh();
            }
            else
				graph.GraphPane.Chart.Fill = new Fill();
        }




        public void ScaleAxes()
		{
			GraphPane pane = graph.GraphPane;
			if (ScaleType == AxisScaleType.Auto)
			{
				double[] points = GetCurveMinMax();
				SetScaleToPoints(points);
				//AspectRatio = -1;
			}
			else if (ScaleType == AxisScaleType.UserDefined)
			{
				double[] points = new double []{ XAxisRange.X, XAxisRange.Y, YAxisRange.X, YAxisRange.Y };
				SetScaleToPoints(points);
				//AspectRatio = -1;
			}
			else if (ScaleType == AxisScaleType.Square)
			{
				double[] points = GetCurveMinMax();
				double max = Math.Max(points[1], points[3]);
				double min = Math.Min(points[0], points[2]);
				SetScaleToPoints(new double[] { min, max, min, max });
				//AspectRatio = 1;
			}
			pane.XAxis.MajorGrid.IsVisible = true;
			pane.YAxis.MajorGrid.IsVisible = true;
        }

		public void UpdateAxes()
		{
			this.graph.GraphPane.AxisChange();
			Console.WriteLine("" + Title + ", update");
		}


		void SetScaleToPoints(double[] points)
		{
			graph.GraphPane.XAxis.Scale.Min = points[0];
			graph.GraphPane.XAxis.Scale.Max = points[1];
			graph.GraphPane.YAxis.Scale.Min = points[2];
			graph.GraphPane.YAxis.Scale.Max = points[3];
		}

		double [] GetCurveMinMax()
		{
			double maxX = double.MinValue;
			double minX = double.MaxValue;
			double maxY = double.MinValue;
			double minY = double.MaxValue;
			foreach (CurveItem curve in mCurveList)
			{
				for(int i = 0; i < curve.Points.Count; i++)
				{
					PointPair pp = curve.Points[i];
					if (pp.X > maxX) maxX = pp.X;
					if (pp.X < minX) minX = pp.X;
					if (pp.Y > maxY) maxY = pp.Y;
					if (pp.Y < minY) minY = pp.Y;
				}
			}
			return new double[] { minX, maxX,minY, maxY };
		}


        public void PlotWaveform()
        {
            ICFunction[] cft = ComplexFunctions;
            if (cft == null) return;
            int nPlots = 0;
            foreach (ICFunction icf in cft) if (icf.Name.Equals("Plot")) nPlots++;
            double[][]x = new double[nPlots][];
            double[][]y = new double[nPlots][];
            Color[] colors = new Color[nPlots];
            ZedGraph.Symbol[] symbols = new ZedGraph.Symbol[nPlots];
            string[] labels = new string[nPlots];
            nPlots = 0;
            for (int i = 0; i < cft.Length; i++)
            {
                CDoubleArray cd = cft[i].Eval();
                if (cft[i].GetType().Name.Equals("Plot"))
                {
                    Plot plot = cft[i] as Plot;
                    if (cd != null)
                    {
                        x[nPlots] = cd.Real;
                        y[nPlots] = cd.Imag;
                        labels[nPlots] = plot.Legend;
                        symbols[nPlots] = GraphSymbol.GetSymbol(plot.SymbolString, plot.LineColor);
                        colors[nPlots] = plot.LineColor;
                    }
                    nPlots++;
                }
            }
            PlotWaveform(x, y, labels.ToArray(), symbols, colors);
        }

        private double mLineWidth = 3;
        /// <summary>
        /// The linewidth as a percentage of the maximum of the plot's height or width.
        /// Minimum width is 3.
        /// </summary>
        private const double LineWidthPercent = 0.5;

        private double[] scaleYAxis(double[] Y, int plotIndex)
        {
            double s = 1;
            double o = 0;
            return scaleYAxis(Y,plotIndex,out s, out o);
        }
        private double[] scaleYAxis(double[] Y, int plotIndex, out double scale, out double offset)
        {
            scale = 1;
            offset = 0;
            if (!EnableScaling) return Y;
            if (plotIndex < 0 || plotIndex >= PlotData.Length) return Y;
            if (YScale == null || plotIndex >= YScale.Length) return Y;
            if (YOffset == null || plotIndex >= YOffset.Length) return Y;
            double[] newY = new double[Y.Length];
            for (int i = 0; i < Y.Length; i++)
            {
                scale = YScale[plotIndex];
                offset = YOffset[plotIndex];
                newY[i] = (Y[i] * YScale[plotIndex]) + YOffset[plotIndex];
            }
            return newY;
        }

        public void PlotWaveform(double[][] X, double[][] Y)
        {
            PlotWaveform(X, Y, null,null,null);
        }
        public void PlotWaveform(double[][] X, double[][] Y, string [] labels, Symbol[] symbols, Color [] colors)
        {
            if (X.Length < 1 || Y.Length < 1) return;
            Clear();
            for (int i = 0; i < Math.Max(X.Length, Y.Length); i++)
            {
                double[] x = X[0];
                if (i < X.Length) x = X[i];
                double[] y = Y[0];
                if (i < Y.Length) y = Y[i];
                Color color = ColorList[i % ColorList.Length];
                if (colors != null && colors.Length > i && colors[i] != Plot.DefaultColor) color = colors[i];
                string label = " ";
                if (labels != null && labels.Length > i) label = labels[i];
                Symbol s = new Symbol(SymbolType.None, color);
                if (symbols != null && symbols.Length > i) s = symbols[i];
                double sc = 1, os = 0;
                y = scaleYAxis(y, i,out sc, out os);
                if (EnableScaling)
                    label += "  [*" + sc + "+" + os + "]";
                PlotWaveform(x, y, color,label, s);
            }
            DisplayCurves();
        }

		public void PlotWaveform(double[] X, double[] Y, Color color)
		{
			PlotWaveform(X, Y, color, " ", new Symbol(SymbolType.None, color));
		}
		public void PlotWaveform(double[] X, double[] Y, Color color, string label, Symbol symbol)
		{
			if (X == null || X.Length < 1 || Y == null || X.Length != Y.Length) return;

			if (symbol == null) symbol = new Symbol(SymbolType.None, color);
			float lineWidth = (float)mLineWidth;


			// If way too many samples, decimate it down.
			if (X.Length > MaxSamples)
			{
				decimation = X.Length / MaxSamples;
				double[] temp = new double[X.Length / decimation];
				for (int i = 0; i < temp.Length; i++)
					temp[i] = X[i * decimation];
				X = temp;
				temp = new double[Y.Length / decimation];
				for (int i = 0; i < temp.Length; i++)
					temp[i] = Y[i * decimation];
				Y = temp;
			}
			else
				decimation = 1;

			if (!ShowLegend) label = "";
			LineItem li = new LineItem(label, X, Y, color, SymbolType.UserDefined, lineWidth);
			li.Symbol = symbol;


			AddCurve(li);
		}
        public void AddCurve(CurveItem ci)
        {
            mCurveList.Add(ci);
        }
        public void DisplayCurves()
        {
			CurveList allCurves = new CurveList(mCurveList);
			allCurves.AddRange(highlightList);
            graph.GraphPane.CurveList = allCurves;
			
            ScaleAxes();
			if (ScaleType != AxisScaleType.UserDefined)
			{
				this.UpdateAxes();
			}
        }
        public void Clear()
        {
            mCurveList = new CurveList();
        }

		public void HighlightIndex(int index, int curveIndex, Color color)
		{
			if (curveIndex >= mCurveList.Count) return;
			CurveItem curve = mCurveList[curveIndex];
			int selectedIndex = index / decimation;
			if (selectedIndex >= curve.Points.Count) return;
			PointPair p = curve.Points[selectedIndex];

			LineItem li = new LineItem("", new double[] { p.X }, new double[] { p.Y }, color, SymbolType.UserDefined, 1);
			li.Symbol = GraphSymbol.GetSymbol("Highlight", color);
			highlightList.Clear();
			highlightList.Add(li);
			DisplayCurves();
			
		}

        private void updateAspectRatio()
        {
            if (mAspectRatio == 0 || (mAspectRatio < 0 && ChartBackgroundImage == null) )
            {
                graph.Dock = DockStyle.Fill;
                mLineWidth = Math.Max(3, Math.Max(graph.Width, graph.Height) * LineWidthPercent / 100);
                return;
            }
            double aspect = mAspectRatio;
            if (aspect < 0) // this option for FitToImage
            {
                aspect = (double)ChartBackgroundImage.Width / (double)ChartBackgroundImage.Height;
                mAspectRatio = -aspect;
            }
            graph.Dock = DockStyle.None;
            // resize graph.
            double controlAspect = (double)this.Width / (double)this.Height;
            if (aspect > controlAspect) // fit to control width.
            {
                graph.Width = this.Width;
                graph.Height = (int)((double)this.Width / aspect);
            }
            else
            {
                graph.Height = this.Height;
                graph.Width = (int)((double)this.Height * aspect);
            }
            mLineWidth = Math.Max(3, Math.Max(graph.Width, graph.Height) * LineWidthPercent / 100);

        }
        private void graph_Resize(object sender, EventArgs e)
        {
            updateAspectRatio();
            scaleBackground();
        }

        delegate void VoidCallback();
        public override void Refresh()
        {
            if (this.InvokeRequired)
            {
                VoidCallback d = new VoidCallback(Refresh);
                this.Invoke(d);
            }
            else
            {
                base.Refresh();
            }
        }

        public static SimplePlot GetPlotFromControl(Control con)
        {
            foreach (Control c in con.Controls)
                if (c.GetType().Equals(typeof(SimplePlot))) return c as SimplePlot;
            return null;
        }

        public void AutoGenerateScaleAndOffsets()
        {
            EnableScaling = false;
            PlotWaveform();
            PointPairList[] data = PlotData;
            double[] yScale = new double[data.Length];
            double[] yOffset = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                // find max and min of data
                double max = double.MinValue;
                double min = double.MaxValue;
                for (int k = 0; k < data[i].Count; k++)
                {
                    if (data[i][k].Y > max) max = data[i][k].Y;
                    if (data[i][k].Y < min) min = data[i][k].Y;
                }
                if (max == min) max = min + 1;
                double diff = max - min;
                diff = getNearestPowerOfTwo(1/diff);
                yScale[i] = diff;
                yOffset[i] = i;
            }
            YScale = yScale;
            YOffset = yOffset;
			EnableScaling = true;
			PlotWaveform();
        }

        private double getNearestPowerOfTwo(double d)
        {
            if (double.IsInfinity(d)) return d;
            if (double.IsNaN(d)) return d;
            if (d == double.MaxValue) return d;
            if (d == double.MinValue) return d;
            double s = 5;
            double n = 1;
            if (Math.Abs(d) < 1)
            {
                while (Math.Abs(n) > Math.Abs(d))
                {
                    if (s == 2) s = 5; else s = 2;
                    n /= s;
                }
                return n;
            }
            if (Math.Abs(d) > 1)
            {
                while (Math.Abs(n) <= Math.Abs(d))
                {
                    if (s == 2) s = 5; else s = 2;
                    n *= s;
                }
                return n/s;
            }
            return 1;
        }

		

		private void graph_DoubleClick(object sender, EventArgs e)
		{
			this.ScaleAxes();
			this.UpdateAxes();
			this.Refresh();
			this.OnDoubleClick(e);
		}

		#region Context Menu Additions


		ToolStripDropDownButton scaleMenu = new ToolStripDropDownButton();

		void InitializeContextMenu()
		{
			ScaleType = AxisScaleType.Auto;
			scaleMenu.DropDownItems.Clear();
			foreach (AxisScaleType axisType in Enum.GetValues(typeof(AxisScaleType)))
			{
				scaleMenu.DropDownItems.Add(new ToolStripButton("" + axisType, null, scaleMenuClick));
			}
		}

		void scaleMenuClick(object sender, EventArgs e)
		{
			if (!(sender is ToolStripItem)) return;
			ToolStripItem tsi = sender as ToolStripItem;
			AxisScaleType type = (AxisScaleType)Enum.Parse(typeof(AxisScaleType), tsi.Text);
			ScaleType = type;
		}

		private void graph_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
		{
			menuStrip.Items.Add(scaleMenu);
		}

		#endregion
	}

}
