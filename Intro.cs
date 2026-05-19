using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 

namespace GTSv2_FINPROJ
{

    public partial class Intro : Form
    {
        Image hman, hmanwalk, hmanidle, bg;
        Image start, kj, vtr, exitn, about, delete;
        Image shov, knowhov, tradehov, xithov, abouthov, deletehov;

        int bgpos = 0, totframe = 0, endframe = 0;
        int playx = 350, playy = 900;
        float transp0 = 1.0f, transp1 = 0.0f;
        int wait = 50, gstate = 0;

        float menuop = 0.0f;
        bool showmenu = false;

        List<string> bgimage = new List<string>();
        List<Image> loadbg = new List<Image>();

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;

        public Intro()
        {
            InitializeComponent();
            SetupForm();
            SetupAnimation();

            gtimer.Start();
        }
        private void Intro_Load(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && this.ClientSize.Width < 100)
            {
                this.ClientSize = new Size(bg.Width, bg.Height);
            }
            if(bg != null)
            {
                lock(bg)
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
        List<PictureBox> menuButtons = new List<PictureBox>();

        private void initializeMenu()
        {
            hovereff bstart = new hovereff(start, shov, new Point(740, 230), BtnStart_Click);
            start = Image.FromFile(bstart.getpath("menubuttons", "startbut.png"));
            shov = Image.FromFile(bstart.getpath("menubuttons", "starthov.png"));

            hovereff bkj = new hovereff(kj, knowhov, new Point(340, 360), Btnkj_Click);
            kj = Image.FromFile(bkj.getpath("menubuttons", "kjbut.png"));
            knowhov = Image.FromFile(bkj.getpath("menubuttons", "kjhov.png"));

            hovereff bvtr = new hovereff(vtr, tradehov, new Point(340, 500), Btnvtr_Click);
            vtr = Image.FromFile(bvtr.getpath("menubuttons", "vtrbut.png"));
            tradehov = Image.FromFile(bvtr.getpath("menubuttons", "vtrhov.png"));

            hovereff babout = new hovereff(about, abouthov, new Point(1140, 360), about_Click);
            about = Image.FromFile(babout.getpath("menubuttons", "aboutbut.png"));
            abouthov = Image.FromFile(babout.getpath("menubuttons", "abouthov.png"));

            hovereff bdelete = new hovereff(delete, deletehov, new Point(1140, 500), delete_Click);
            delete = Image.FromFile(bdelete.getpath("menubuttons", "deletebut.png"));
            deletehov = Image.FromFile(bdelete.getpath("menubuttons", "deletehov.png"));

            hovereff bexit = new hovereff(exitn, xithov, new Point(740, 630), exit_Click);
            exitn = Image.FromFile(bexit.getpath("menubuttons", "exitbut.png"));
            xithov = Image.FromFile(bexit.getpath("menubuttons", "exithov.png"));


            this.Controls.Add(bstart);
            this.Controls.Add(bkj);
            this.Controls.Add(bvtr);
            this.Controls.Add(bexit);
            this.Controls.Add(babout);
            this.Controls.Add(bdelete);

            menuButtons.Add(bstart);
            menuButtons.Add(bkj);
            menuButtons.Add(bvtr);
            menuButtons.Add(bexit);
            menuButtons.Add(babout);
            menuButtons.Add(bdelete);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        { }
        private void gtimer_Tick(object sender, EventArgs e)
        {
            if (gstate == 0)
            {
                if (wait > 0)
                {
                    wait--;
                }
                else if (transp0 > 0)
                {
                    transp1 += 0.07f;
                    transp0 -= 0.07f;
                }
                else
                {
                    gstate = 1;
                    transp0 = 0.02f;
                    transp1 = 1.0f;
                }
            }
            else if (gstate == 1)
            {
                int max = -this.Width;
                if (bgpos > max)
                {
                    bgpos -= 30;
                    hman = hmanwalk;
                }
                else
                {
                    bgpos = max;
                    hman = hmanidle;
                    showmenu = true;
                }
                if (showmenu && menuop < 1.0f)
                {
                    menuop += 0.10f;
                }
            }
            this.Invalidate();
        }
        private void SetupForm()
        {
            hovereff bstart = new hovereff(start, shov, new Point(740, 230), BtnStart_Click);
            start = Image.FromFile(bstart.getpath("menubuttons", "startbut.png"));
            shov = Image.FromFile(bstart.getpath("menubuttons", "starthov.png"));

            hovereff bkj = new hovereff(kj, knowhov, new Point(340, 360), Btnkj_Click);
            kj = Image.FromFile(bkj.getpath("menubuttons", "kjbut.png"));
            knowhov = Image.FromFile(bkj.getpath("menubuttons", "kjhov.png"));

            hovereff bvtr = new hovereff(vtr, tradehov, new Point(340, 500), Btnvtr_Click);
            vtr = Image.FromFile(bvtr.getpath("menubuttons", "vtrbut.png"));
            tradehov = Image.FromFile(bvtr.getpath("menubuttons", "vtrhov.png"));

            hovereff babout = new hovereff(about, abouthov, new Point(1140, 360), about_Click);
            about = Image.FromFile(babout.getpath("menubuttons", "aboutbut.png"));
            abouthov = Image.FromFile(babout.getpath("menubuttons", "abouthov.png"));

            hovereff bdelete = new hovereff(delete, deletehov, new Point(1140, 500), exit_Click);
            delete = Image.FromFile(bdelete.getpath("menubuttons", "deletebut.png"));
            deletehov = Image.FromFile(bdelete.getpath("menubuttons", "deletehov.png"));

            hovereff bexit = new hovereff(exitn, xithov, new Point(740, 630), exit_Click);
            exitn = Image.FromFile(bexit.getpath("menubuttons", "exitbut.png"));
            xithov = Image.FromFile(bexit.getpath("menubuttons", "exithov.png"));

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            bgimage = Directory.GetFiles("background", "*.gif").ToList();

            hmanwalk = Image.FromFile("horsemanwalk.gif");      
            hmanidle = Image.FromFile("horsemanidle.gif");

            hman = hmanidle;
            foreach (string p in bgimage)
            {
                loadbg.Add(Image.FromFile(p));
            }
            if (loadbg.Count > 0)
            {
                bg = loadbg[0];
                this.ClientSize = new Size(bg.Width, bg.Height);
            }
            initializeMenu();

            gtimer.Interval = 30; 
        }
        private void delete_Click(object? sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
            "Are you sure you want to delete your account? This cannot be undone.",
            "Delete Account",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                if (dbconnect.deleteall(playdata.accountid))
                {
                    MessageBox.Show("Account deleted.");
                    playdata.accountid = 0;
                    playdata.currentid = 0;
                    playdata.name = null;
                    playdata.email = null;
                    playdata.role = null;
                    playdata.silver = 2000;
                    playdata.compvoy = 0;
                    playdata.boletas = 6;
                    playdata.curkilos = 1000;
                    playdata.repu = "";

                    this.Close();
                    nav.go(new signpage());
                }
            }
        }

        private void SetupAnimation()
        {
            ImageAnimator.Animate(hmanwalk, this.OnFrameChangedHandler);
            ImageAnimator.Animate(hmanidle, this.OnFrameChangedHandler);
            foreach (var img in loadbg)
            {
                ImageAnimator.Animate(img, this.OnFrameChangedHandler);
            }
            FrameDimension dimention = new FrameDimension(hman.FrameDimensionsList[0]);
            totframe = hman.GetFrameCount(dimention);
            endframe = totframe;
        }

        private void OnFrameChangedHandler(object? sender, EventArgs e)
        {
            if (hmanwalk != null)
            {
                lock (hmanwalk)
                {
                    ImageAnimator.UpdateFrames(hmanwalk);
                }
            }
            if (hmanidle != null)
            {
                lock (hmanidle)
                {
                    ImageAnimator.UpdateFrames(hmanidle);
                }
            }

            var imgs = loadbg.ToArray();
            foreach (var img in imgs)
            {
                lock (img)
                {
                    ImageAnimator.UpdateFrames(img);
                }
            }
        }
        private void Formpaintevent(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            float scaleFactor = Math.Min(scaleX, scaleY);

            if (gstate == 0)
            {
                if(bg != null)
                {
                    lock (bg)
                    {
                        transitioneffects.drawfade(g, loadbg[0], transp0, this.ClientRectangle);
                        transitioneffects.drawfade(g, loadbg[1], transp1, this.ClientRectangle);
                    }
                    
                }

                if (hman != null)
                {
                    ColorMatrix cm = new ColorMatrix { Matrix33 = transp1 };
                    ImageAttributes atr = new ImageAttributes();
                    atr.SetColorMatrix(cm);

                    lock (hman)
                    {
                        int scaledX = (int)(playx * scaleX);
                        int scaledY = (int)(playy * scaleY);
                        int scaledW = (int)(hman.Width * scaleX);
                        int scaledH = (int)(hman.Height * scaleY);

                        Rectangle destRect = new Rectangle(scaledX, scaledY, scaledW, scaledH);
                        g.DrawImage(hman, destRect, 0, 0, hman.Width, hman.Height, GraphicsUnit.Pixel, atr);
                    }
                }
            }
            else
            {
                if (loadbg != null && loadbg.Count > 1)
                {
                    lock (loadbg[1])
                    {
                        g.DrawImage(loadbg[1], bgpos, 0, this.Width, this.Height);
                    }
                    if (loadbg.Count > 2)
                    {
                        lock (loadbg[2])
                        {
                            g.DrawImage(loadbg[2], bgpos + this.Width-3, 0, this.Width, this.Height);

                        }
                    }
                }

                if (hman != null)
                {
                    ColorMatrix cm = new ColorMatrix { Matrix33 = transp1 };
                    ImageAttributes atr = new ImageAttributes();
                    atr.SetColorMatrix(cm);

                    lock (hman)
                    {
                        int scaledX = (int)(playx * scaleX);
                        int scaledY = (int)(playy * scaleY);
                        int scaledW = (int)(hman.Width * scaleX);
                        int scaledH = (int)(hman.Height * scaleY);

                        Rectangle destRect = new Rectangle(scaledX, scaledY, scaledW, scaledH);
                        g.DrawImage(hman, destRect, 0, 0, hman.Width, hman.Height, GraphicsUnit.Pixel, atr);
                    }
                }

                if (showmenu)
                {
                    Rectangle st = ScaleRectangle(new Rectangle(740, 230, start.Width, start.Height));
                    Rectangle knowj = ScaleRectangle(new Rectangle(340, 360, kj.Width, kj.Height));
                    Rectangle viewtr = ScaleRectangle(new Rectangle(340, 500, vtr.Width, vtr.Height));
                    Rectangle exitb = ScaleRectangle(new Rectangle(740, 630, exitn.Width, exitn.Height));
                    Rectangle aboutrec = ScaleRectangle(new Rectangle(1140, 360, about.Width, about.Height));
                    Rectangle deleterec = ScaleRectangle(new Rectangle(1140, 500, delete.Width, delete.Height));
                    if (menuop < 1.0f)
                    {
                        transitioneffects.drawbutfade(g, start, 740, 230, menuop, st);
                        transitioneffects.drawbutfade(g, kj, 340, 360, menuop, knowj);
                        transitioneffects.drawbutfade(g, vtr, 340, 500, menuop, viewtr);
                        transitioneffects.drawbutfade(g, exitn, 740, 630, menuop, exitb);
                        transitioneffects.drawbutfade(g, about, 1140, 360, menuop, aboutrec);
                        transitioneffects.drawbutfade(g, delete, 1140, 500, menuop, deleterec);
                    }
                    else
                    {
                        foreach (var btn in menuButtons)
                        {
                            if (!btn.Visible)
                            {
                                btn.Visible = true;
                            }
                        }
                    }

                }
            }
        }
        private void about_Click(object? sender, EventArgs e)
        {
            gtimer.Stop();
            nav.remember(this);
            this.Close();
            var n = new aboutgame();
            nav.apply(n);
            nav.go(n);
            return;
        }

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            gtimer.Stop();
            nav.remember(this);
            this.Close();

            if (playdata.compvoy == 0) 
            {
                var n = new tutorial();
                nav.apply(n);
                nav.go(n);
            }
            else
            {
                var n = new wmap();
                nav.apply(n);
                nav.go(n);
            }
            return;
        }

        private void Btnvtr_Click(object? sender, EventArgs e)
        {
            gtimer.Stop();
            nav.remember(this);
            this.Close();
            var n = new Vtrecord();
            nav.apply(n);
            nav.go(n);
            return;
        }
        private void Btnkj_Click(object? sender, EventArgs e)
        {
            gtimer.Stop();
            nav.remember(this);
            this.Close();
            var n = new Knowjourn();
            nav.apply(n);
            nav.go(n);
            return;
        }
        private void exit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
    public static class transitioneffects
    {
        public static void drawfade(this Graphics g, Image img, float op, Rectangle rec)
        {
            if (op <= 0)
            {
                return;
            }
            if (op > 1)
            {
                op = 1;
            }
            ColorMatrix color = new ColorMatrix { Matrix33 = op };
            using (ImageAttributes at = new ImageAttributes())
            {
                at.SetColorMatrix(color, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                lock (img)
                {
                    g.DrawImage(img, rec, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, at);
                }
            }
        }
        public static void drawbutfade(this Graphics g, Image img, int x, int y, float op, Rectangle rec)
        {
            if (op <= 0)
            {
                return;
            }
            if (op > 1)
            {
                op = 1;
            }
            ColorMatrix color = new ColorMatrix { Matrix33 = op };
            using (ImageAttributes at = new ImageAttributes())
            {
                at.SetColorMatrix(color, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                lock (img)
                {
                    g.DrawImage(img, rec, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, at);
                }
            }
        }
    }
}



