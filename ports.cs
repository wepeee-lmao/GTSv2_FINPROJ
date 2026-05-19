using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace GTSv2_FINPROJ
{
    public partial class ports : Form
    {
        private int portid;
        private string portfolder;

        float transp0 = 0.0f;
        bool close = false;

        Image playact;
        Image walkleft, walkright, walkup, walkdown, frontidle, backidle;
        Image portbg;

        Point playerPos = new Point(850, 900);
        Rectangle nventDoor = new Rectangle(510, 405, 200, 200);
        Rectangle knowDoor = new Rectangle(1415, 515, 200, 200);
        Rectangle exit = new Rectangle(850, 900, 200, 200);

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;

        private string portname;

        int color = 0;
        bool change = true;

        public ports(int id)
        {
            InitializeComponent();
            this.ResizeRedraw = true;
            this.portid = id;
            switch (id)
            {
                case 1:
                    portname = "China Port";
                    portfolder = "chinaport";
                    nventDoor = ScaleRectangle(new Rectangle(510, 405, 200, 200));
                    knowDoor = ScaleRectangle(new Rectangle(1415, 515, 200, 200));
                    break;
                case 2:
                    portname = "Malacca Port";
                    portfolder = "malport";
                    nventDoor = ScaleRectangle(new Rectangle(480, 400, 200, 200));
                    knowDoor = ScaleRectangle(new Rectangle(1440, 450, 200, 200));
                    break;
                case 3:
                    portname = "Seville Port";
                    portfolder = "sevport";
                    nventDoor = ScaleRectangle(new Rectangle(550, 350, 200, 200));
                    knowDoor = ScaleRectangle(new Rectangle(1420, 410, 200, 200));
                    break;
                case 4:
                    portname = "Lisbon Port";
                    portfolder = "lisport";
                    nventDoor = ScaleRectangle(new Rectangle(510, 405, 200, 200));
                    knowDoor = ScaleRectangle(new Rectangle(1420, 480, 200, 200));
                    break;
                case 5:
                    portname = "Acapulco Port";
                    portfolder = "acaport";
                    nventDoor = ScaleRectangle(new Rectangle(480, 405, 200, 200));
                    knowDoor = ScaleRectangle(new Rectangle(1420, 480, 200, 200));
                    break;
                case 6:
                    portname = "Cartagena Port";
                    portfolder = "cartaport";
                    nventDoor = ScaleRectangle(new Rectangle(490, 405, 100, 100));
                    knowDoor = ScaleRectangle(new Rectangle(1430, 400, 100, 100));
                    break;
                case 7:
                    portname = "Manila Port";
                    portfolder = "manilaport";
                    nventDoor = ScaleRectangle(new Rectangle(490, 405, 100, 100));
                    knowDoor = ScaleRectangle(new Rectangle(1430, 400, 100, 100));
                    break;
            }

            dbconnect.portcompvoy(this.portid);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            LoadAssets();
            playact = frontidle;
        }
        private void ptimein_Tick(object sender, EventArgs e)
        {
            if (!close)
            {
                transp0 = Math.Min(1.0f, transp0 + 0.09f);
            }
            else
            {
                transp0 = Math.Max(0.0f, transp0 - 0.09f);
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
       
        private void LoadAssets()
        {
            walkleft = sprite.walkleft;
            walkright = sprite.walkright;
            walkup = sprite.walkup;
            walkdown = sprite.walkdown;
            frontidle = sprite.frontidle;
            backidle = sprite.backidle;

            sprite.register(OnFrameChanged);

            portbg = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, portfolder, "bgport.png"));
            ptimein.Interval = 30;
            ptimein.Start();

        }

        private void ports_Load(object sender, EventArgs e)
        {
            if (this.ClientSize.Width < portbg.Width || this.ClientSize.Height < portbg.Height)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.ClientSize = new Size(portbg.Width, portbg.Height);
                }
            }

            originalSize = new Size(portbg.Width, portbg.Height);
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

        private Rectangle ScaleRectangle(Rectangle original)
        {
            return new Rectangle(
                (int)(original.X * scaleX),
                (int)(original.Y * scaleY),
                (int)(original.Width * scaleX),
                (int)(original.Height * scaleY)
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

            Rectangle playerHitbox = new Rectangle(playerPos.X, playerPos.Y, 50, 50);

            if (e.KeyCode == Keys.Enter)
            {
                if (playerHitbox.IntersectsWith(nventDoor))
                {
                    ptimein.Stop();
                    nav.remember(this);
                    this.Close();
                    var n = new nventbuild(portid);
                    nav.apply(n);
                    nav.go(n);
                    return;
                }
                else if (playerHitbox.IntersectsWith(knowDoor))
                {
                    ptimein.Stop();
                    nav.remember(this);
                    this.Close();
                    var n = new knowbuild(portid);
                    nav.apply(n);
                    nav.go(n);
                    return;
                }
                else if (playerHitbox.IntersectsWith(exit))
                {
                    ptimein.Stop();
                    nav.remember(this);
                    this.Close();
                    var n = new wmap();
                    nav.apply(n);
                    nav.go(n);
                    return;
                }
            }
            this.Invalidate();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            playact = frontidle;
            this.Invalidate();
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        { }

        private void portspaint(object sender, PaintEventArgs e)
        {
            if (portbg == null || playact == null) return;

            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            if (portbg != null)
            {
                lock (portbg)
                {
                    transitioneffects.drawfade(g, portbg, transp0, this.ClientRectangle);
                }
            }

            using (Font font1 = new Font("Antiquity Print", 14 * Math.Min(scaleX, scaleY), FontStyle.Bold))
            using (Font font = new Font("Antiquity Print", 18 * Math.Min(scaleX, scaleY), FontStyle.Bold))
            using (Font font2 = new Font("Antiquity Print", 10 * Math.Min(scaleX, scaleY), FontStyle.Bold))
            using (SolidBrush sub = new SolidBrush(Color.FromArgb(200, Color.Black)))
            using (SolidBrush pulse = new SolidBrush(Color.FromArgb(color, 255, 100)))
            using (SolidBrush brush = new SolidBrush(Color.Gold))
            {
                g.DrawString(">>  " + portname + "  <<", font, pulse, new PointF(100 * scaleX, 900 * scaleY));
                string reputitle = dbconnect.getrepu(playdata.currentid, portid).title;
                g.DrawString("Port reputation: " + reputitle, font1, brush, new PointF(100 * scaleX, 990 * scaleY));
            }

            if (playact != null)
            {
                ImageAnimator.UpdateFrames(playact);
                Rectangle r = ScaleRectangle(new Rectangle(playerPos.X, playerPos.Y, 100, 100));
                transitioneffects.drawbutfade(g, playact, playerPos.X, playerPos.Y, transp0, r);
            }

        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            sprite.unregister(OnFrameChanged);

            if (portbg != null)
            {
                portbg.Dispose();
                portbg = null;
            }
        }
        private void OnFrameChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}
