using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace GTSv2_FINPROJ
{
    public partial class knowbuild : Form
    {
        private int portid;
        private string portfolder;

        int idx = 0, tspeed = 0;
        float transp0 = 0.0f;
        bool close = false;
        private mode cmode;
        Image interiorBg, strip;
        Image playact, walkleft, walkright, walkup, walkdown, frontidle, backidle;

        Point playerPos = new Point(840, 900);
        List<Rectangle> walls = new List<Rectangle>();

        bool near = false, ask = false, quiz = false;
        string wel = "", say = "";

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;
        public knowbuild(int id)
        {
            InitializeComponent();

            this.ResizeRedraw = true;
            this.portid = id;

            switch (id)
            {
                case 1:
                    portfolder = "chinaport";
                    wel = "Nǐ hǎo";
                    break;
                case 2:
                    portfolder = "malport";
                    wel = "Selamat sejahtera";
                    break;
                case 3:
                    portfolder = "sevport";
                    wel = "Buenos días";
                    break;
                case 4:
                    portfolder = "lisport";
                    wel = "Bom dia";
                    break;
                case 5:
                    portfolder = "acaport";
                    wel = "Bueno día";
                    break;
                case 6:
                    portfolder = "cartaport";
                    wel = "Buenoh";
                    break;
                case 7:
                    portfolder = "manilaport";
                    wel = "Magandang araw";
                    break;
            }

            say = $"{wel}! Welcome to the knowledge building. How may I help you?";
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            gtimer.Interval = 30;
            gtimer.Start();

            LoadInteriorAssets();
            playact = frontidle;
        }
        private void gtimer_Tick(object sender, EventArgs e)
        {
            if (close == false)
            {
                transp0 = Math.Min(1.0f, transp0 + 0.09f);
            }
            else
            {
                transp0 = Math.Max(0.0f, transp0 - 0.09f);
            }

            if (near)
            {
                tspeed++;
                if (tspeed >= 1)
                {
                    if (idx < say.Length) idx++;
                    tspeed = 0;
                }
            }
            this.Invalidate();
        }
        private void LoadInteriorAssets()
        {
            walkleft = sprite.walkleft;
            walkright = sprite.walkright;
            walkup = sprite.walkup;
            walkdown = sprite.walkdown;
            frontidle = sprite.frontidle;
            backidle = sprite.backidle;

            sprite.register(OnFrameChanged);

            interiorBg = Image.FromFile(Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, portfolder, "bgknow.png"));
            strip = Image.FromFile("strip.png");

        }
        private void knowbuild_Load(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && this.ClientSize.Width < 100)
            {
                this.ClientSize = new Size(interiorBg.Width, interiorBg.Height);
            }

            originalSize = new Size(interiorBg.Width, interiorBg.Height);
            scaleX = (float)this.ClientSize.Width / originalSize.Width;
            scaleY = (float)this.ClientSize.Height / originalSize.Height;
            this.Resize += new EventHandler(nventunit_Resize);

            if (this.WindowState == FormWindowState.Normal)
            {
                this.CenterToScreen();
            }
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
        private RectangleF ScaleRectangleF(RectangleF original)
        {
            return new RectangleF(
                original.X * scaleX,
                original.Y * scaleY,
                original.Width * scaleX,
                original.Height * scaleY
            );
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            int speed = 10;

            int nextX = playerPos.X;
            int nextY = playerPos.Y;

            if (e.KeyCode == Keys.A) { nextX -= speed; playact = walkleft; }
            else if (e.KeyCode == Keys.D) { nextX += speed; playact = walkright; }
            else if (e.KeyCode == Keys.W) { nextY -= speed; playact = walkup; }
            else if (e.KeyCode == Keys.S) { nextY += speed; playact = walkdown; }

            playerPos.X = nextX;
            playerPos.Y = nextY;

            Rectangle nextBox = new Rectangle(nextX, nextY, 50, 50);
            Rectangle exitDoor = new Rectangle(800, 880, 400, 550);
            Rectangle invent = new Rectangle(800, 450, 400, 205);

            if (e.KeyCode == Keys.Enter)
            {
                if (nextBox.IntersectsWith(exitDoor))
                {
                    nav.remember(this);
                    this.Close();
                    var n = new ports(portid);
                    nav.apply(n);
                    nav.go(n);

                }
            }
            if (nextBox.IntersectsWith(invent))
            {
                near = true;
            }
            else
            {
                near = false;
                idx = 0;
            }
            this.Invalidate();
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            int ux = (int)(e.X / scaleX);
            int uy = (int)(e.Y / scaleY);

            Rectangle b = new Rectangle(320, 950, 450, 100);
            Rectangle s = new Rectangle(1100, 950, 450, 100);

            Point unscaled = new Point(ux, uy);

            if (b.Contains(unscaled))
            {
                gtimer.Stop();
                nav.remember(this);
                this.Close();
                var n = new knowitems(portid, kmode.ask);
                nav.apply(n);
                nav.go(n);
            }
            else if (s.Contains(unscaled))
            {
                gtimer.Stop();
                nav.remember(this);
                this.Close();      
                var n = new knowquiz(portid, kmode.quiz);
                nav.apply(n);
                nav.go(n);
            }
        }
        private void knowbuild_MouseMove(object sender, MouseEventArgs e)
        {
            int ux = (int)(e.X / scaleX);
            int uy = (int)(e.Y / scaleY);
            Point unscaled = new Point(ux, uy);

            Rectangle b = new Rectangle(320, 950, 450, 100);
            Rectangle s = new Rectangle(1100, 950, 450, 100);

            if (b.Contains(unscaled))
            {
                ask = true;
                quiz = false;
            }
            else if (s.Contains(unscaled))
            {
                ask = false;
                quiz = true;
            }
            else
            {
                ask = false;
                quiz = false;
            }
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {}
        protected override void OnKeyUp(KeyEventArgs e)
        {
            playact = frontidle;
            this.Invalidate();
        }
        private void knowbuild_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;


            float scaleFactor = Math.Min(scaleX, scaleY);

            if (interiorBg != null && transp0 > 0)
            {
                lock (interiorBg)
                {
                    transitioneffects.drawfade(g, interiorBg, transp0, this.ClientRectangle);
                }
            }
            if (playact != null)
            {
                ImageAnimator.UpdateFrames(playact);
                Rectangle r = ScaleRectangle(new Rectangle(playerPos.X, playerPos.Y, 230, 250));
                transitioneffects.drawbutfade(g, playact, playerPos.X, playerPos.Y, transp0, r);
            }

            Rectangle sc = ScaleRectangle(new Rectangle(80, 850, strip.Width, strip.Height));
            if (strip != null)
            {
                if (near)
                {
                    g.DrawImage(strip, sc);

                    RectangleF box = ScaleRectangleF(new RectangleF(320, 890, strip.Width - 200, 60));
                    using (Font font = new Font("Antiquity Print", 12 * scaleFactor, FontStyle.Regular))
                    using (Font text = new Font("Antiquity Print", 14 * scaleFactor, FontStyle.Regular))
                    using (SolidBrush brush = new SolidBrush(Color.CadetBlue))
                    {
                        int blen = Math.Min(idx + 12, say.Length);
                        string syg = say.Substring(0, blen);
                        g.DrawString(syg, text, brush, box);

                        Brush bbrush, nbrush;
                        if (ask == true && quiz == false)
                        {
                            bbrush = Brushes.Brown;
                            nbrush = Brushes.ForestGreen;
                        }
                        else if (ask == false && quiz == true)
                        {
                            bbrush = Brushes.ForestGreen;
                            nbrush = Brushes.Brown;
                        }
                        else
                        {
                            bbrush = Brushes.ForestGreen;
                            nbrush = Brushes.ForestGreen;
                        }
                        g.DrawString(">> Can I know more about this port? <<", font, bbrush, ScalePoint(new Point(320, 950)));
                        g.DrawString(">> I'd like to test my knowledge <<", font, nbrush, ScalePoint(new Point(1100, 950)));
                    }
                }
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            sprite.unregister(OnFrameChanged);

            if (interiorBg != null)
            {
                interiorBg.Dispose();
            }
            if (strip != null)
            {
                strip.Dispose();
            }
            interiorBg = null;
            strip = null;
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}
