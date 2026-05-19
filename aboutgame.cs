using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTSv2_FINPROJ
{
    public partial class aboutgame : Form
    {
        Image bg1, bg2, bg3;
        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;
        private LinkLabel videoLink;

        bool backhov = false, nexthov = false;
        private int cpage = 0;

        Image menhov, mennorm;
        List<PictureBox> menuButtons = new List<PictureBox>();

        public aboutgame()
        {
            InitializeComponent();
            InitializeVideo();
            bg1 = Image.FromFile("abg1.png");
            bg2 = Image.FromFile("abg2.png");
            bg3 = Image.FromFile("abg3.png");

            this.ResizeRedraw = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            mennorm = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "viewtrade", "menbut.png"));
            menhov = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "viewtrade", "menhov.png"));

            hovereff mbtn = new hovereff(mennorm, menhov, new Point(60, 950), exit_Click);

            mbtn.BringToFront();
            mbtn.Visible = true;
            this.Controls.Add(mbtn);
            menuButtons.Add(mbtn);
        }

        private void exit_Click(object? sender, EventArgs e)
        {
            nav.remember(this);
            this.Close();
            var n = new Intro();
            nav.apply(n);
            nav.go(n);
            return;
        }

       
        private void aboutgame_Load(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && this.ClientSize.Width < 100)
            {
                this.ClientSize = new Size(bg1.Width, bg1.Height);
            }
            if (bg1 != null)
            {
                lock (bg1)
                {
                    originalSize = new Size(bg1.Width, bg1.Height);
                }
            }
            scaleX = (float)this.ClientSize.Width / originalSize.Width;
            scaleY = (float)this.ClientSize.Height / originalSize.Height;
            this.Resize += new EventHandler(nventunit_Resize);

            if (this.WindowState == FormWindowState.Normal)
            {
                this.CenterToScreen();
            }

            UpdateButtonPositions();
        }
        private void nventunit_Resize(object sender, EventArgs e)
        {
            if (originalSize.Width == 0 || originalSize.Height == 0)
            {
                return;
            }

            scaleX = (float)this.ClientSize.Width / originalSize.Width;
            scaleY = (float)this.ClientSize.Height / originalSize.Height;

            UpdateButtonPositions();

            this.Invalidate();
        }
        private void UpdateButtonPositions()
        {
            foreach (Control control in this.Controls)
            {
                if (control is hovereff btn)
                {
                    btn.UpdateScale(scaleX, scaleY);
                }
            }

            int originalX = 1080;
            int originalY = 100;
            int originalW = 730;
            int originalH = 780;

            webView21.Left = (int)(originalX * scaleX);
            webView21.Top = (int)(originalY * scaleY);
            webView21.Width = (int)(originalW * scaleX);
            webView21.Height = (int)(originalH * scaleY);
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
        async void InitializeVideo()
        {
            await webView21.EnsureCoreWebView2Async();

            webView21.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0 Safari/537.36";

            string link = "https://share.google/0bfllE3wAqj6TWzrw";
            string embed = link.Replace("watch?v=", "embed/") + "?autoplay=1";

            webView21.Source = new Uri(embed);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            Rectangle r = ScaleRectangle(new Rectangle(120, 1000, 100, 100));
            Rectangle m = ScaleRectangle(new Rectangle(1670, 1000, 100, 100));

            if (m.Contains(e.Location) && cpage < 3)
            {
                cpage++;
                if (cpage == 3)
                {
                    nav.remember(this);
                    this.Close();
                    var n = new analytics(analytics.analytics_mode.player);
                    nav.apply(n);
                    nav.go(n);
                    return;
                }
                this.Invalidate();
            }
            else if (r.Contains(e.Location) && cpage > 0)
            {
                cpage--;
                this.Invalidate();
            }

        }
        private void aboutgame_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle r = ScaleRectangle(new Rectangle(120, 1000, 100, 100));
            Rectangle n = ScaleRectangle(new Rectangle(1670, 1000, 100, 100));

            if (r.Contains(e.Location) && cpage > 0)
            {
                backhov = true;
                nexthov = false;
            }
            else if (n.Contains(e.Location) && cpage < 3)
            {
                backhov = false;
                nexthov = true;
            }
            else
            {
                backhov = false;
                nexthov = false;
            }

            this.Invalidate();
        }
        private Point ScalePoint(Point original)
        {
            return new Point(
                (int)(original.X * scaleX),
                (int)(original.Y * scaleY)
            );
        }
        private void aboutgame_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            Image currentBg = bg1;

            if (cpage == 1) currentBg = bg2;
            if (cpage == 2) currentBg = bg3;

            if (currentBg != null)
            {
                lock (currentBg)
                {
                    g.DrawImage(currentBg, this.ClientRectangle);
                }
            }

            float scaleFactor = Math.Min(scaleX, scaleY);

            Font nav = new Font("Antiquity Print", 14 * scaleFactor, FontStyle.Regular);
            Brush bbrush, nbrush;
            if (backhov == true && nexthov == false)
            {
                bbrush = Brushes.Red;
                nbrush = Brushes.Brown;
            }
            else if (backhov == false && nexthov == true)
            {
                bbrush = Brushes.Brown;
                nbrush = Brushes.Red;
            }
            else
            {
                bbrush = Brushes.Brown;
                nbrush = Brushes.Brown;
            }

            if (cpage > 0)
            {
                g.DrawString("<< Back", nav, bbrush, ScalePoint(new Point(120, 1000)));
            }

            if (cpage < 3)
            {
                g.DrawString(">> Next", nav, nbrush, ScalePoint(new Point(1670, 1000)));
            }
        }

       
    }
}
