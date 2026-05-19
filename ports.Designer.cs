namespace GTSv2_FINPROJ
{
    partial class ports
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ports));
            ptimein = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // ptimein
            // 
            ptimein.Tick += ptimein_Tick;
            // 
            // ports
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(1491, 848);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ports";
            Text = "ports";
            Load += ports_Load;
            Paint += portspaint;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer ptimein;
    }
}