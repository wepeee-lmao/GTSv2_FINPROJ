namespace GTSv2_FINPROJ
{
    partial class signpage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(signpage));
            emailLbl = new Label();
            passLbl = new Label();
            emailBox = new TextBox();
            passBox = new TextBox();
            confirmLbl = new Label();
            confirmBox = new TextBox();
            gtimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // emailLbl
            // 
            emailLbl.AutoSize = true;
            emailLbl.Location = new Point(83, 60);
            emailLbl.Name = "emailLbl";
            emailLbl.Size = new Size(77, 25);
            emailLbl.TabIndex = 0;
            emailLbl.Text = "emailLbl";
            // 
            // passLbl
            // 
            passLbl.AutoSize = true;
            passLbl.Location = new Point(430, 60);
            passLbl.Name = "passLbl";
            passLbl.Size = new Size(71, 25);
            passLbl.TabIndex = 1;
            passLbl.Text = "passLbl";
            // 
            // emailBox
            // 
            emailBox.Location = new Point(85, 108);
            emailBox.Name = "emailBox";
            emailBox.Size = new Size(150, 31);
            emailBox.TabIndex = 2;
            // 
            // passBox
            // 
            passBox.Location = new Point(403, 108);
            passBox.Name = "passBox";
            passBox.Size = new Size(150, 31);
            passBox.TabIndex = 3;
            // 
            // confirmLbl
            // 
            confirmLbl.AutoSize = true;
            confirmLbl.Location = new Point(658, 60);
            confirmLbl.Name = "confirmLbl";
            confirmLbl.Size = new Size(96, 25);
            confirmLbl.TabIndex = 4;
            confirmLbl.Text = "confirmLbl";
            // 
            // confirmBox
            // 
            confirmBox.Location = new Point(658, 108);
            confirmBox.Name = "confirmBox";
            confirmBox.Size = new Size(150, 31);
            confirmBox.TabIndex = 5;
            // 
            // gtimer
            // 
            gtimer.Tick += gtimer_Tick_1;
            // 
            // signpage
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1439, 845);
            Controls.Add(confirmBox);
            Controls.Add(confirmLbl);
            Controls.Add(passBox);
            Controls.Add(emailBox);
            Controls.Add(passLbl);
            Controls.Add(emailLbl);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "signpage";
            Text = "Galleon Trade Simulator";
            Load += signpage_Load;
            MouseMove += signpage_MouseMove;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label emailLbl;
        private Label passLbl;
        private TextBox emailBox;
        private TextBox passBox;
        private Label confirmLbl;
        private TextBox confirmBox;
        private System.Windows.Forms.Timer gtimer;
    }
}