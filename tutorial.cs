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
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.AxHost;

namespace GTSv2_FINPROJ
{
    public partial class tutorial : Form
    {
        private int portid;

        float transp0 = 0.0f;
        bool close = false;

        Image playact;
        Image walkleft, walkright, walkup, walkdown, frontidle, backidle;
        Image portbg;

        private int tutpage = 0;
        private int totalpages = 11;
        private List<Image> tutpages = new List<Image>();
        private bool tutdone = false;

        Point playerPos = new Point(850, 900);
        Rectangle nventDoor = new Rectangle(490, 405, 200, 200);
        Rectangle knowDoor = new Rectangle(1430, 200, 200, 200);
        Rectangle exit = new Rectangle(850, 900, 200, 200);

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;

        private string portname;

        public tutorial()
        {
            InitializeComponent();
            portname = "Manila Port";
            this.ResizeRedraw = true;
            this.portid = 7;
            
            nventDoor = new Rectangle(490, 405, 100, 200);
            knowDoor = new Rectangle(1430, 400, 100, 200);
            exit = new Rectangle(800, 880, 400, 550);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            LoadAssets();
            playact = frontidle;
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

            portbg = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "manilaport", "bgport.png"));

            for (int i = 1; i <= totalpages; i++)
            {
                tutpages.Add(Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tutorial", $"{i}.png")));
            }

            ptimein.Interval = 30;
            ptimein.Start();
        }

        private void tutorial_Load(object sender, EventArgs e)
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
            if (!tutdone)
            {
                if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D || e.KeyCode == Keys.Enter)
                {
                    if (tutpage < totalpages - 1)
                    {
                        tutpage++;
                    }
                    else
                    {
                        tutdone = true;
                    }
                }
                else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
                {
                    if (tutpage > 0)
                    {
                        tutpage--;
                    }
                }
                this.Invalidate();
                return;
            }

            int speed = 10;
            int nextX = playerPos.X;
            int nextY = playerPos.Y;

            if (e.KeyCode == Keys.A) { nextX -= speed; playact = walkleft; }
            else if (e.KeyCode == Keys.D) { nextX += speed; playact = walkright; }
            else if (e.KeyCode == Keys.W) { nextY -= speed; playact = walkup; }
            else if (e.KeyCode == Keys.S) { nextY += speed; playact = walkdown; }

            playerPos.X = nextX;
            playerPos.Y = nextY;

            Rectangle playerHitbox = new Rectangle(playerPos.X, playerPos.Y, 50, 50);

            Rectangle exitDoor = new Rectangle(800, 880, 400, 550);  
            Rectangle nventDoorCheck = new Rectangle(490, 405, 100, 100); 
            Rectangle knowDoorCheck = new Rectangle(1430, 400, 100, 100);

            if (e.KeyCode == Keys.Enter)
            {
                if (playerHitbox.IntersectsWith(nventDoorCheck))  
                {
                    ptimein.Stop();
                    nav.remember(this);
                    this.Close();
                    var n = new nventbuild(portid);
                    nav.apply(n);
                    nav.go(n);
                    return;
                }
                else if (playerHitbox.IntersectsWith(knowDoorCheck))
                {
                    ptimein.Stop();
                    nav.remember(this);
                    this.Close();
                    var n = new knowbuild(portid);
                    nav.apply(n);
                    nav.go(n);
                    return;
                }
                else if (playerHitbox.IntersectsWith(exitDoor))
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
        private void OnFrameChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }
       
        private void tutorial_Paint(object sender, PaintEventArgs e)
        {
            if (portbg == null || playact == null) return;

            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            float sf = Math.Min(scaleX, scaleY);

            if (!tutdone)
            {
                if (tutpages.Count > tutpage && tutpages[tutpage] != null)
                {
                    lock (tutpages[tutpage])
                    {
                        g.DrawImage(tutpages[tutpage], this.ClientRectangle);
                    }
                }

                using (Font text = new Font("Antiquity Print", 11 * sf, FontStyle.Bold))
                using (SolidBrush gold = new SolidBrush(Color.Gold))
                using (SolidBrush dark = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
                {
                   
                    g.FillRectangle(dark, 0, (int)(this.Height - 60 * scaleY), this.Width, (int)(60 * scaleY));

                    string nav;

                    if (tutpage == totalpages - 1)
                    {
                        nav = "Press ENTER to start playing";
                    }
                    else
                    {
                        nav = $"A / D or ENTER to continue   |   Page {tutpage + 1} of {totalpages}";
                    }

                    g.DrawString(nav, text, gold, new PointF(50 * scaleX, 50 * scaleY));
                }
                return;
            }

            lock (portbg)
            {
                transitioneffects.drawfade(g, portbg, transp0, this.ClientRectangle);
            }

            using (Font font = new Font("Antiquity Print", 18 * sf, FontStyle.Bold))
            using (Font font2 = new Font("Antiquity Print", 10 * sf, FontStyle.Bold))
            using (SolidBrush sub = new SolidBrush(Color.FromArgb(200, Color.Black)))
            using (SolidBrush brush = new SolidBrush(Color.Gold))
            {
                g.DrawString(">>  " + portname + "  <<", font, brush, new PointF(100 * scaleX, 970 * scaleY));

            }

            if (playact != null)
            {
                ImageAnimator.UpdateFrames(playact);
                Rectangle r = ScaleRectangle(new Rectangle(playerPos.X, playerPos.Y, 100, 100));
                transitioneffects.drawbutfade(g, playact, playerPos.X, playerPos.Y, transp0, r);
            }
        }
        private void ptimein_Tick(object sender, EventArgs e)
        {
            if (transp0 < 1.0f)
            {
                transp0 += 0.05f;
                if (transp0 >= 1.0f)
                {
                    transp0 = 1.0f;
                }
            }
            this.Invalidate();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            sprite.unregister(OnFrameChanged);


            tutpages.Clear();

            if (portbg != null)
            {
                portbg.Dispose();
                portbg = null;
            }
        }
    }
}
