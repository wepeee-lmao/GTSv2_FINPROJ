namespace GTSv2_FINPROJ
{
    partial class wmap
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(wmap));
            startin = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // startin
            // 
            startin.Tick += startin_Tick;
            // 
            // wmap
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(1488, 857);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "wmap";
            Text = "start";
            Load += start_Load;
            Paint += start_Paint;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer startin;
    }
}