namespace GTSv2_FINPROJ
{
    partial class Intro
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Intro));
            gtimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // gtimer
            // 
            gtimer.Tick += gtimer_Tick;
            // 
            // Intro
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(1494, 846);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Intro";
            Text = "Galleon Trade Simulator";
            Load += Intro_Load;
            Paint += Formpaintevent;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer gtimer;
    }
}