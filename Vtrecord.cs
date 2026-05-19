using GTSv2_FINPROJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GTSv2_FINPROJ
{
    public partial class Vtrecord : Form
    {
        Image bgvtr, frontp, secp;
        Image menhov, mennorm;

        float menuop = 0.0f, pageop = 0.0f;

        bool showmenu = false, showpage = false;
        bool backhov = false, nexthov = false;

        int cpage = 0;

        List<PictureBox> menuButtons = new List<PictureBox>();
        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;

        public Vtrecord()
        {
            InitializeComponent();
            setupvtr();

        }

        protected override void OnPaintBackground(PaintEventArgs e)
        { }
        private void Vtrecord_Load(object sender, EventArgs e)
        {
            if (playdata.currentid > 0)
            {
                dbconnect.loadprogress(playdata.accountid);
            }

            if (this.ClientSize.Width < bgvtr.Width || this.ClientSize.Height < bgvtr.Height)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.ClientSize = new Size(bgvtr.Width, bgvtr.Height);
                }
            }

            originalSize = new Size(bgvtr.Width, bgvtr.Height);
            scaleX = (float)this.ClientSize.Width / originalSize.Width;
            scaleY = (float)this.ClientSize.Height / originalSize.Height;

            this.Resize += new EventHandler(nventunit_Resize);

            if (this.WindowState == FormWindowState.Normal)
                this.CenterToScreen();

            UpdateButtonPositions();

            if (this.Opacity >= 1.0)
            {
                showpage = true;
                showmenu = true;
                pageop = 1.0f;
                menuop = 1.0f;
                foreach (var bt in menuButtons)
                    if (bt.Name == "menh") bt.Visible = true;
            }
            else
            {
                this.Opacity = 0;
                vtrin.Start();
            }

            this.Invalidate();
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

        private void vtrtimer_Tick(object sender, EventArgs e)
        {
            showpage = true;
            showmenu = true;
            if (this.Opacity < 1.0)
            {
                this.Opacity += 0.09;
            }
            else
            {
                pageop += 0.05f;
                menuop += 0.05f;
                if (menuop >= 1.0f && pageop >= 1.0f)
                {
                    vtrin.Stop();
                    foreach (var bt in menuButtons)
                    {
                        if (bt.Name == "menh") { bt.Visible = true; }
                    }
                }
            }
            this.Invalidate();
        }

        private void changebutton()
        {
            foreach (var bt in menuButtons)
            {
                if (bt.Name == "menh")
                {
                    bt.Visible = true;
                }
            }
        }
        private void Vtrecord_MouseClick_1(object sender, MouseEventArgs e)
        {
            int back = (int)(this.ClientSize.Width * 0.56);

            if (e.X < back)
            {
                if (cpage > 0)
                {
                    cpage--;
                    pageop = 0.0f;
                    vtrin.Start();
                    changebutton();
                    this.Invalidate();
                }
            }
            else
            {
                if (cpage < 2)
                {
                    cpage++;

                    if (cpage == 2) 
                    {
                        vtrin.Stop();
                        nav.remember(this);
                        this.Close();
                        var n = new nventitems(playdata.currentid, mode.viewcargo);
                        nav.apply(n);
                        nav.go(n);
                        return;
                    }

                    pageop = 0.0f;
                    vtrin.Start();
                    changebutton();
                    this.Invalidate();
                }
            }
        }
        private void Vtrecord_MouseMove_1(object sender, MouseEventArgs e)
        {
            int back = (int)(this.ClientSize.Width * 0.56);

            if (e.X < back && cpage > 0)
            {
                this.Cursor = Cursors.PanWest;
                backhov = true;
                nexthov = false;
            }
            else if (e.X > back && cpage < 2)
            {
                this.Cursor = Cursors.PanEast;
                backhov = false;
                nexthov = true;
            }
            else
            {
                this.Cursor = Cursors.Default;
                backhov = false;
                nexthov = false;
            }

            if (backhov || nexthov)
            {
                this.Invalidate();
            }
        }

        private void initializebuttons()
        {
            hovereff mbtn = new hovereff(mennorm, menhov, new Point(200, 890), exit_Click);
            mennorm = Image.FromFile(mbtn.getpath("viewtrade", "menbut.png"));
            menhov = Image.FromFile(mbtn.getpath("viewtrade", "menhov.png"));

            mbtn.Name = "menh";
            mbtn.Visible = false;
            mbtn.BringToFront();
            this.Controls.Add(mbtn);
            menuButtons.Add(mbtn);

        }
        private void setupvtr()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            bgvtr = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "viewtrade", "bgtraderec.png"));

            frontp = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "viewtrade", "frontp.png"));
            secp = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "viewtrade", "secp.png"));

            hovereff mbtn = new hovereff(mennorm, menhov, new Point(200, 890), exit_Click);
            mennorm = Image.FromFile(mbtn.getpath("viewtrade", "menbut.png"));
            menhov = Image.FromFile(mbtn.getpath("viewtrade", "menhov.png"));

            vtrin.Interval = 30;
            initializebuttons();
        }
        private void Vtrecord_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            if(bgvtr != null)
            {
                lock (bgvtr)
                {
                    g.DrawImage(bgvtr, this.ClientRectangle);
                }
            }

            if (showpage)
            {
                Image activep = frontp;
                if (cpage == 1) activep = secp;

                if (cpage == 0)
                {
                    g.drawfade(frontp, pageop, this.ClientRectangle);
                }
                else if (cpage == 1)
                {
                    g.DrawImage(activep, this.ClientRectangle);
                    DrawPlayerProgress(g);
                }
            }

        }
        private void DrawPlayerProgress(Graphics g)
        {
            float scaleFactor = Math.Min(scaleX, scaleY);

            Font statsFont = new Font("Antiquity Print", 16 * scaleFactor, FontStyle.Bold);
            Font nav = new Font("Antiquity Print", 14 * scaleFactor, FontStyle.Regular);
            Brush statsBrush = new SolidBrush(Color.FromArgb(60, 30, 20));
            {
                int ls = 65;
                g.DrawString(playdata.name, statsFont, statsBrush, ScalePoint(new Point(1100, 415)));
                g.DrawString(playdata.silver.ToString() + " Reales", statsFont, statsBrush, ScalePoint(new Point(1060, 420 + (ls * 1))));
                g.DrawString(playdata.repu, statsFont, statsBrush, ScalePoint(new Point(950, 440 + (ls * 2))));
                g.DrawString(playdata.boletas.ToString(), statsFont, statsBrush, ScalePoint(new Point(1150, 450 + (ls * 3))));
                g.DrawString(playdata.compvoy.ToString(), statsFont, statsBrush, ScalePoint(new Point(1150, 460 + (ls * 4))));
                g.DrawString(playdata.curkilos.ToString() + " kg", statsFont, statsBrush, ScalePoint(new Point(1100, 480 + (ls * 5))));
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
                g.DrawString("<< Back", nav, bbrush, ScalePoint(new Point(650, 950)));
                g.DrawString(">> Next", nav, nbrush, ScalePoint(new Point(1200, 950)));
            }
        }
        private void exit_Click(object sender, EventArgs e)
        {
            vtrin.Stop();

            nav.remember(this);
            this.Close();
            var n = new Intro();
            nav.apply(n);
            nav.go(n);
            return;
        }
    }

}
