namespace ComplexMath
{
    partial class SimplePlot
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.graph = new ZedGraph.ZedGraphControl();
			this.SuspendLayout();
			// 
			// graph
			// 
			this.graph.Dock = System.Windows.Forms.DockStyle.Fill;
			this.graph.Location = new System.Drawing.Point(0, 0);
			this.graph.Name = "graph";
			this.graph.ScrollGrace = 0D;
			this.graph.ScrollMaxX = 0D;
			this.graph.ScrollMaxY = 0D;
			this.graph.ScrollMaxY2 = 0D;
			this.graph.ScrollMinX = 0D;
			this.graph.ScrollMinY = 0D;
			this.graph.ScrollMinY2 = 0D;
			this.graph.Size = new System.Drawing.Size(610, 437);
			this.graph.TabIndex = 0;
			this.graph.DoubleClick += new System.EventHandler(this.graph_DoubleClick);
			// 
			// SimplePlot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Controls.Add(this.graph);
			this.Name = "SimplePlot";
			this.Size = new System.Drawing.Size(610, 437);
			this.Resize += new System.EventHandler(this.graph_Resize);
			this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl graph;
    }
}
