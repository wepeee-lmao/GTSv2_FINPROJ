namespace GTSv2_FINPROJ
{
    partial class gameend
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(gameend));
            feedbox = new TextBox();
            gtimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // feedbox
            // 
            feedbox.Location = new Point(200, 226);
            feedbox.Name = "feedbox";
            feedbox.Size = new Size(727, 31);
            feedbox.TabIndex = 0;
            // 
            // gameend
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1182, 803);
            Controls.Add(feedbox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "gameend";
            Text = "gameend";
            Load += gameend_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox feedbox;
        private System.Windows.Forms.Timer gtimer;
    }
}