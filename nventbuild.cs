using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace GTSv2_FINPROJ
{
    public partial class nventbuild : Form
    {
        private int portid;
        private string portfolder;

        int wait = 30, idx = 0, tspeed = 0;
        float transp0 = 0.0f, transp1 = 1.0f;

        private mode cmode;
        Image interiorBg, strip;
        Image playact, walkleft, walkright, walkup, walkdown, frontidle, backidle;

        Point playerPos = new Point(840, 900);
        List<Rectangle> walls = new List<Rectangle>();


        bool near = false, buy = false, sell = false, close = false;
        string wel = "", say = "";

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;
        //boundaries b = new boundaries();

        public nventbuild(int id)
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
            say = $"{wel}! Welcome to the merchant building. What would you like to do today?";

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            LoadInteriorAssets();
            playact = frontidle;

        }
        private void gtimer_Tick(object sender, EventArgs e)
        {
            if (!close)
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

            interiorBg = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, portfolder, "bgmerchant.png"));
            strip = Image.FromFile("strip.png");

            gtimer.Interval = 30;
            gtimer.Start();
        }
        
        private void nventbuild_Load(object sender, EventArgs e)
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
        private void nventbuild_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle b = ScaleRectangle(new Rectangle(300, 950, 100, 100));
            Rectangle s = ScaleRectangle(new Rectangle(1400, 950, 100, 100));

            if (b.Contains(e.Location))
            {
                buy = true;
                sell = false;
            }
            else if (s.Contains(e.Location))
            {
                buy = false;
                sell = true;
            }
            else
            {
                buy = false;
                sell = false;
            }

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
            Rectangle exitDoor = new Rectangle(800, 900, 400, 500);
            Rectangle invent = new Rectangle(800, 610, 400, 205);
            if (e.KeyCode == Keys.Enter && nextBox.IntersectsWith(exitDoor))
            {
                gtimer.Stop();
                nav.remember(this);
                this.Close();
                var n = new ports(portid);
                nav.apply(n);
                nav.go(n);
                return;
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
            Rectangle b =  ScaleRectangle(new Rectangle(300, 950, 100, 100));
            Rectangle s =  ScaleRectangle(new Rectangle(1400, 950, 100, 100));

            if (b.Contains(e.Location))
            {
                gtimer.Stop();
                nav.remember(this);
                this.Close();
                var n = new nventitems(portid, mode.buy);
                nav.apply(n);
                nav.go(n);
                return;
            }
            else if (s.Contains(e.Location))
            {
                gtimer.Stop();
                nav.remember(this);
                this.Close();
                var n = new nventitems(portid, mode.sell);
                nav.apply(n);
                nav.go(n);
                return;
            }
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {}
        protected override void OnKeyUp(KeyEventArgs e)
        {
            playact = frontidle;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;


            float scaleFactor = Math.Min(scaleX, scaleY);

            if (interiorBg == null || strip == null) return;

            if(interiorBg != null && transp0 > 0)
            {
                lock (interiorBg)
                {
                    transitioneffects.drawfade(g, interiorBg, transp0, this.ClientRectangle);
                }
            }
            if (playact != null)
            {
                ImageAnimator.UpdateFrames(playact);
                Rectangle r = ScaleRectangle(new Rectangle(playerPos.X, playerPos.Y, 300, 300));
                transitioneffects.drawbutfade(g, playact, playerPos.X, playerPos.Y, transp0, r);
            }

            Rectangle sc = ScaleRectangle(new Rectangle(80, 850, strip.Width, strip.Height));
            if (strip != null)
            {
                if (near)
                {
                    g.DrawImage(strip, sc);

                    RectangleF box = ScaleRectangleF(new RectangleF(290, 890, strip.Width - 200, 60));
                    using (Font font = new Font("Antiquity Print", 12 * scaleFactor, FontStyle.Regular))
                    using (Font text = new Font("Antiquity Print", 14 * scaleFactor, FontStyle.Regular))
                    using (SolidBrush brush = new SolidBrush(Color.DarkOliveGreen))
                    {
                        int blen = Math.Min(idx + 12, say.Length);
                        string syg = say.Substring(0, blen);
                        g.DrawString(syg, text, brush, box);

                        Brush bbrush, nbrush;
                        if (buy == true && sell == false)
                        {
                            bbrush = Brushes.Brown;
                            nbrush = Brushes.Green;
                        }
                        else if (buy == false && sell == true)
                        {
                            bbrush = Brushes.Green;
                            nbrush = Brushes.Brown;
                        }
                        else
                        {
                            bbrush = Brushes.Green;
                            nbrush = Brushes.Green;
                        }
                        g.DrawString(">> Buy <<", font, bbrush, ScalePoint(new Point(300, 950)));
                        g.DrawString(">> Sell <<", font, nbrush, ScalePoint(new Point(1400, 950)));
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
