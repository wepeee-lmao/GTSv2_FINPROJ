namespace GTSv2_FINPROJ
{
    partial class Enter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Enter));
            gtimer = new System.Windows.Forms.Timer(components);
            label = new Label();
            textname = new TextBox();
            SuspendLayout();
            // 
            // gtimer
            // 
            gtimer.Tick += gtimer_Tick;
            // 
            // label
            // 
            label.AutoSize = true;
            label.BackColor = Color.White;
            label.Font = new Font("Antiquity print", 26F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label.ForeColor = SystemColors.ButtonHighlight;
            label.Location = new Point(151, 253);
            label.Name = "label";
            label.Size = new Size(0, 76);
            label.TabIndex = 0;
            // 
            // textname
            // 
            textname.BackColor = SystemColors.Info;
            textname.Font = new Font("Antiquity print", 20F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textname.Location = new Point(140, 351);
            textname.Name = "textname";
            textname.Size = new Size(1226, 66);
            textname.TabIndex = 1;
            textname.KeyDown += txtName_KeyDown;
            // 
            // Enter
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(1482, 848);
            Controls.Add(textname);
            Controls.Add(label);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Enter";
            Text = "Enter";
            Load += Enter_Load;
            Paint += FormPaint;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer gtimer;
        private Label label;
        private TextBox textname;
    }
}