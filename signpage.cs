using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTSv2_FINPROJ
{
    public partial class signpage : Form
    {
        Image bg;
        float transp0 = 0.0f;
        bool isregistering = false, backhov = false, nhov = false;

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;

        bool change = true;
        int color = 0;

        public signpage()
        {
            InitializeComponent();
            this.ResizeRedraw = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            setupform();
        }

        private void setupform()
        {
            bg = Image.FromFile("signbg.png");
            this.BackColor = Color.Black;

            confirmLbl.Text = "Confirm Password:";
            confirmLbl.Visible = false;
            emailLbl.Text = "Email:";
            emailLbl.Visible = false;
            passLbl.Text = "Password:";
            passLbl.Visible = false;

            emailBox.Font = new Font("Antiquity Print", 14);
            emailBox.BorderStyle = BorderStyle.None;
            emailBox.BackColor = Color.White;

            passBox.Font = new Font("Antiquity Print", 14);
            passBox.BorderStyle = BorderStyle.None;
            passBox.BackColor = Color.White;
            passBox.PasswordChar = '●';

            confirmBox.Font = new Font("Antiquity Print", 14);
            confirmBox.BorderStyle = BorderStyle.None;
            confirmBox.BackColor = Color.White;
            confirmBox.PasswordChar = '●';
            confirmBox.Visible = false;

            gtimer.Interval = 30;
            gtimer.Start();
        }

        private void signpage_Load(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && this.ClientSize.Width < 100)
            {
                this.ClientSize = new Size(bg.Width, bg.Height);
            }
            if (bg != null)
            {
                lock (bg)
                {
                    originalSize = new Size(bg.Width, bg.Height);
                }
            }
            scaleX = (float)this.ClientSize.Width / originalSize.Width;
            scaleY = (float)this.ClientSize.Height / originalSize.Height;
            this.Resize += new EventHandler(nventunit_Resize);

            if (this.WindowState == FormWindowState.Normal)
            {
                this.CenterToScreen();
            }

            UpdateControlPositions();
        }
        private void nventunit_Resize(object sender, EventArgs e)
        {
            if (originalSize.Width == 0 || originalSize.Height == 0)
            {
                return;
            }

            scaleX = (float)this.ClientSize.Width / originalSize.Width;
            scaleY = (float)this.ClientSize.Height / originalSize.Height;

            UpdateControlPositions();

            this.Invalidate();
        }
        private void UpdateControlPositions()
        {
            emailLbl.Location = ScalePoint(new Point(400, 400));
            emailBox.Location = ScalePoint(new Point(400, 440));
            emailBox.Size = new Size((int)(500 * scaleX), (int)(40 * scaleY));

            passLbl.Location = ScalePoint(new Point(1000, 400));
            passBox.Location = ScalePoint(new Point(1000, 440));
            passBox.Size = new Size((int)(500 * scaleX), (int)(40 * scaleY));

            confirmLbl.Location = ScalePoint(new Point(400, 560));
            confirmBox.Location = ScalePoint(new Point(400, 600));
            confirmBox.Size = new Size((int)(500 * scaleX), (int)(40 * scaleY));
        }

        private Point ScalePoint(Point original)
        {
            return new Point(
                (int)(original.X * scaleX),
                (int)(original.Y * scaleY)
            );
        }
        private Rectangle ScaleRectangle(Rectangle original)
        {
            return new Rectangle(
                (int)(original.X * scaleX),
                (int)(original.Y * scaleY),
                (int)(original.Width * scaleX),
                (int)(original.Height * scaleY)
            );
        }

        private void gtimer_Tick_1(object sender, EventArgs e)
        {
            if (transp0 < 1.0f)
            {
                transp0 = Math.Min(1.0f, transp0 + 0.05f);
            }
            if (change)
            {
                color += 5;
                if (color >= 200)
                {
                    change = false;
                }
            }
            else
            {
                color -= 5;
                if (color <= 50)
                {
                    change = true;
                }
            }
            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            if (bg != null)
            {
                lock (bg)
                {
                    transitioneffects.drawfade(g, bg, transp0, this.ClientRectangle);
                }
            }
            float sf = Math.Min(scaleX, scaleY);
            using (Font title = new Font("Antiquity Print", 24 * sf, FontStyle.Bold))
            using (Font font = new Font("Antiquity Print", 14 * sf, FontStyle.Bold))
            using (Font hint = new Font("Antiquity Print", 11 * sf, FontStyle.Regular))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(color, 240, 140)))
            using (SolidBrush gold = new SolidBrush(Color.FromArgb(254, 219, 112)))
            using (SolidBrush white = new SolidBrush(Color.White))
            {
                g.DrawString("Welcome to the Galleon Trade Simulator!", title, brush, ScalePoint(new Point(340, 150)));
                g.DrawString(emailLbl.Text, font, gold, emailLbl.Location);

                if(isregistering)
                {
                    g.DrawString(confirmLbl.Text, font, gold, confirmLbl.Location);
                }
                else
                {
                    g.DrawString("", font, gold, confirmLbl.Location);
                }
                g.DrawString(passLbl.Text, font, gold, passLbl.Location);

                string mode;
                if (isregistering == true)
                {
                    mode = "Create Account";
                }
                else
                {
                    mode = "Sign In";
                }

                g.DrawString(mode, title, gold, ScalePoint(new Point(400, 280)));

                string toggle;
                if (isregistering)
                {
                    toggle = "Already have an account? Click here to sign in.";
                }
                else
                {
                    toggle = "New here? Click here to create an account.";
                }
                Brush nbrush;
                if (nhov == true)
                {
                    nbrush = Brushes.Yellow;
                }
                else
                {
                    nbrush = white;
                }
                g.DrawString(toggle, hint, nbrush, ScalePoint(new Point(400, 830)));

                string action;
                if (isregistering)
                {
                    action = "Register";
                }
                else
                {
                    action = "Login";
                }

                Brush bbrush;
                if (backhov == true)
                {
                    bbrush = brush;
                }
                else
                {
                    bbrush = gold;
                }
                g.DrawString(action, title, bbrush, ScalePoint(new Point(400, 750)));
            }
        }
        private void signpage_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle r = ScaleRectangle(new Rectangle(400, 750, 300, 50));
            Rectangle r2 = ScaleRectangle(new Rectangle(400, 830, 600, 40));

            if (r.Contains(e.Location))
            {
                backhov = true;
            }
            else
            {
                backhov = false;
            }
            if(r2.Contains(e.Location))
            {
                nhov = true;
            }
            else
            {
                nhov = false;
            }
            if (backhov || nhov)
            {
                this.Invalidate();
            }
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            Rectangle actionBtn = ScaleRectangle(new Rectangle(400, 750, 300, 50));

            if (actionBtn.Contains(e.Location))
            {
                if (isregistering)
                {
                    doregister();
                }
                else
                {
                    dologin();
                }
                return;
            }

            Rectangle toggleBtn = ScaleRectangle(new Rectangle(400, 830, 600, 40));

            if (toggleBtn.Contains(e.Location))
            {
                isregistering = !isregistering;
                confirmBox.Visible = isregistering;
                this.Invalidate();
            }
        }

        private void dologin()
        {
            string email = emailBox.Text.Trim();
            string pass = passBox.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Please enter your email and password.");
                return;
            }

            var (accountid, role) = dbconnect.login(email, pass);

            if (accountid == -1)
            {
                MessageBox.Show("Incorrect email or password.");
                return;
            }

            playdata.accountid = accountid;
            playdata.email = email;
            playdata.role = role;

            gtimer.Stop();
            nav.remember(this);

            if (role == "admin")
            {
                nav.remember(this);
                this.Close();
                var n = new analytics(analytics.analytics_mode.admin);
                nav.apply(n);
                nav.go(n);
            }
            else
            {
                if (dbconnect.hasplayprogress(accountid))
                {
                    dbconnect.loadprogress(accountid);

                    dbconnect.updatelastplayed(playdata.currentid);

                    nav.remember(this);
                    this.Close();
                    var n = new Intro();
                    nav.apply(n);
                    nav.go(n);
                }
                else
                {
                    nav.remember(this);
                    this.Close();
                    var n = new Enter();
                    nav.apply(n);
                    nav.go(n);
                }
            }
        }

        private void doregister()
        {
            string email = emailBox.Text.Trim();
            string pass = passBox.Text;
            string confirm = confirmBox.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (pass != confirm)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            if (pass.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.");
                return;
            }

            int accountid = dbconnect.register(email, pass);
            if (accountid == -1) return;

            playdata.accountid = accountid;
            playdata.email = email;
            playdata.role = "player";

            gtimer.Stop();
            this.Close();
            var n = new Enter();
            nav.apply(n);
            nav.go(n);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            gtimer.Stop();
            if (bg != null)
            {
                bg.Dispose();
                bg = null;
            }
        }
    }
}