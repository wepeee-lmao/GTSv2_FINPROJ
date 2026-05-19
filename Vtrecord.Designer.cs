namespace GTSv2_FINPROJ
{
    partial class Vtrecord
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Vtrecord));
            vtrin = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // vtrin
            // 
            vtrin.Interval = 30;
            vtrin.Tick += vtrtimer_Tick;
            // 
            // Vtrecord
            // 
            AutoScaleDimensions = new SizeF(15F, 26F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(2412, 914);
            Font = new Font("Antiquity print", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "Vtrecord";
            Text = "Galleon Trade Simulator";
            Load += Vtrecord_Load;
            Paint += Vtrecord_Paint;
            MouseClick += Vtrecord_MouseClick_1;
            MouseMove += Vtrecord_MouseMove_1;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer vtrin;
    }
}