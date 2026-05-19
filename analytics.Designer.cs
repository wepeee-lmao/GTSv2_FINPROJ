namespace GTSv2_FINPROJ
{
    partial class analytics
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(analytics));
            grid = new DataGridView();
            timer1 = new System.Windows.Forms.Timer(components);
            feedbackgrid = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)feedbackgrid).BeginInit();
            SuspendLayout();
            // 
            // grid
            // 
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid.Location = new Point(46, 39);
            grid.Name = "grid";
            grid.RowHeadersWidth = 62;
            grid.Size = new Size(1355, 785);
            grid.TabIndex = 0;
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // feedbackgrid
            // 
            feedbackgrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            feedbackgrid.Location = new Point(35, 39);
            feedbackgrid.Name = "feedbackgrid";
            feedbackgrid.RowHeadersWidth = 62;
            feedbackgrid.Size = new Size(1375, 791);
            feedbackgrid.TabIndex = 1;
            // 
            // analytics
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1450, 861);
            Controls.Add(feedbackgrid);
            Controls.Add(grid);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "analytics";
            Text = "analytics";
            Load += analytics_Load;
            MouseClick += analytics_MouseClick;
            MouseMove += analytics_MouseMove;
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ((System.ComponentModel.ISupportInitialize)feedbackgrid).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView grid;
        private System.Windows.Forms.Timer timer1;
        private DataGridView feedbackgrid;
    }
}