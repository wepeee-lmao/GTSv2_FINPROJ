namespace GTSv2_FINPROJ
{
    partial class knowbuild
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(knowbuild));
            gtimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // gtimer
            // 
            gtimer.Tick += gtimer_Tick;
            // 
            // knowbuild
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(1450, 847);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "knowbuild";
            Text = "knowbuild";
            Load += knowbuild_Load;
            Paint += knowbuild_Paint;
            MouseMove += knowbuild_MouseMove;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer gtimer;
    }
}