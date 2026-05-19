using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace GTSv2_FINPROJ
{
    public partial class portloading : Form
    {
        Image boat, bg, strip;

        private int portid;
        private string portfolder;

        string genfact = "", pfact = "";
        string portname = "";

        int bgidx, totframe = 0, endframe = 0;
        int scrollx = 0, speed = 0;
        int gidx = 0, pidx = 0;
        int countg = 0, countp = 0;

        int boatx = 60, boaty = 200;
        float transp0 = 1.0f, transp1 = 0.0f;
        int gstate = 0, wait = 100;

        List<Image> loadbg = new List<Image>();
        List<string> bgimage = new List<string>();

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;

        public portloading(int id)
        {
            InitializeComponent();
            SetupPortLoad();
            setupanimate();
            this.portid = id;

            switch (portid)
            {
                case 1:
                    portfolder = "chinaport";
                    portname = "China";
                    break;
                case 2:
                    portfolder = "malport";
                    portname = "Malacca";
                    break;
                case 3:
                    portfolder = "sevport";
                    portname = "Seville";
                    break;
                case 4:
                    portfolder = "lisport";
                    portname = "Lisbon";
                    break;
                case 5:
                    portfolder = "acaport";
                    portname = "Acapulco";
                    break;
                case 6:
                    portfolder = "cartaport";
                    portname = "Cartagena";
                    break;
                case 7:
                    portfolder = "manilaport";
                    portname = "Manila";
                    break;
            }
            string[] fac = dbconnect.getfacts(this.portid);
            genfact = fac[0];
            pfact = fac[1];
            countg = int.Parse(fac[2]);
            countp = int.Parse(fac[3]);

            dbconnect.saveknowjourn(playdata.currentid, countp, countg);
        }
        private void gtimer_Tick(object sender, EventArgs e)
        {

            if (gstate == 0)
            {
                if (transp0 > 0)
                {
                    transp1 += 0.07f;
                    transp0 -= 0.07f;
                }
                else
                {
                    gstate = 1;
                    transp0 = 0.0f;
                    transp1 = 1.0f;
                }
            }
            else if (gstate == 1)
            {
                bool trans = (bgidx == 2 || bgidx == 6);

                scrollx -= 50;

                if (!trans)
                {
                    boatx = 60 + (int)(Math.Sin(Environment.TickCount * 0.005) * 10);
                }

                if (scrollx <= -this.Width)
                {
                    scrollx = 0;
                    bgidx++;

                    if (bgidx >= loadbg.Count - 1)
                    {
                        bgidx = loadbg.Count - 1;
                        gstate = 2;
                    }
                }

                speed++;
                if (speed >= 1)
                {
                    if (gidx < genfact.Length)
                    {
                        gidx++;
                    }
                    if (pidx < pfact.Length)
                    {
                        pidx++;
                    }
                    speed = 0;
                }

            }
            else if (gstate == 2)
            {
                if (this.Opacity < 1.0)
                {
                    this.Opacity += 0.05;
                }
                if (wait > 0)
                {
                    wait--;
                }
                else
                {
                    gtimer.Stop();
                    int pid = portid;
                    nav.remember(this);
                    this.Close();

                    if (playdata.compvoy >= 6)
                    {
                        nav.go(new finalboss());
                    }
                    else
                    {
                        nav.go(new ports(pid));
                    }
                    return;
                }
            }
            this.Invalidate();
        }

        private void portloading_Load(object sender, EventArgs e)
        {
            if (this.ClientSize.Width < bg.Width || this.ClientSize.Height < bg.Height)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.ClientSize = new Size(bg.Width, bg.Height);
                }
            }

            originalSize = new Size(bg.Width, bg.Height);
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
        private RectangleF ScaleRectangleF(RectangleF original)
        {
            return new RectangleF(
                original.X * scaleX,
                original.Y * scaleY,
                original.Width * scaleX,
                original.Height * scaleY
            );
        }
        private void SetupPortLoad()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            bgimage = Directory.GetFiles("storm", "*.png")
                .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f)))
                .ToList();

            boat = Image.FromFile("boat.gif");
            strip = Image.FromFile("strip.png");

            foreach (string p in bgimage)
            {
                loadbg.Add(Image.FromFile(p));
            }
            if (loadbg.Count > 0)
            {
                bg = loadbg[0];
                this.ClientSize = new Size(bg.Width, bg.Height);
            }
            gtimer.Interval = 30;
            gtimer.Start();
        }

        private void setupanimate()
        {
            ImageAnimator.Animate(boat, this.OnFrameChanged);
            foreach (var im in loadbg)
            {
                ImageAnimator.Animate(im, this.OnFrameChanged);
            }
            FrameDimension dim = new FrameDimension(boat.FrameDimensionsList[0]);
            totframe = boat.GetFrameCount(dim);
            endframe = totframe;
        }
        private void OnFrameChanged(object? sender, EventArgs e)
        {
            if (boat != null)
            {
                lock (boat)
                {
                    ImageAnimator.UpdateFrames(boat);
                }
            }

            var imgs = loadbg.ToArray();
            foreach (var im in imgs)
            {
                lock (im)
                {
                    ImageAnimator.UpdateFrames(im);
                }
            }
        }


        private void portpaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            float scaleFactor = Math.Min(scaleX, scaleY);
            if (loadbg == null || loadbg.Count == 0) return;

            if (gstate == 0)
            {
                transitioneffects.drawfade(g, loadbg[0], transp1, this.ClientRectangle);
                if (boat != null)
                {
                    lock (boat)
                    {
                        Rectangle st = ScaleRectangle(new Rectangle(boatx, boaty, boat.Width - 400, boat.Height - 400));
                        transitioneffects.drawbutfade(g, boat, boatx, boaty, transp1, st);
                    }
                }
                if (strip != null)
                {
                    Rectangle sc = ScaleRectangle(new Rectangle(80, 850, strip.Width, strip.Height));

                    if (gstate == 0)
                    {
                        transitioneffects.drawfade(g, strip, transp1, sc);
                    }
                    else
                    {
                        g.DrawImage(strip, sc);

                        RectangleF pd = ScaleRectangleF(new RectangleF(100, 90, 900, 300));
                        RectangleF genf = ScaleRectangleF(new RectangleF(190, 875, strip.Width - 200, 60));
                        RectangleF portf = ScaleRectangleF(new RectangleF(190, 945, strip.Width - 190, 60));

                        using (Font factFont = new Font("Book Antiqua", 11*scaleFactor, FontStyle.Bold))
                        using (Font font = new Font("Antiquity Print", 16*scaleFactor, FontStyle.Regular))

                        using (SolidBrush gbrush = new SolidBrush(Color.DarkOliveGreen))
                        using (SolidBrush pbrush = new SolidBrush(Color.DarkSlateBlue))
                        {
                            string portdt = $"Travelling to {portname}....";
                            g.DrawString(portdt, font, Brushes.Gold, pd);

                            string dy = $"Did you know? {genfact}";
                            int glen = Math.Min(gidx + 14, dy.Length);
                            string dyg = dy.Substring(0, glen);
                            g.DrawString(dyg, factFont, gbrush, genf);

                            string ap = $"About port: {pfact}";
                            int plen = Math.Min(pidx + 14, ap.Length);
                            string app = ap.Substring(0, plen);
                            g.DrawString(app, factFont, pbrush, portf);

                        }
                    }
                }
            }
            if (gstate >= 1)
            {
                bool trans = (bgidx == 2 || bgidx == 6);

                if (trans)
                {
                    lock (loadbg[bgidx])
                    {
                        g.DrawImage(loadbg[bgidx], 0, 0, this.Width, this.Height);
                    }

                    float fade = (float)Math.Abs(scrollx) / this.Width;
                    ColorMatrix cm = new ColorMatrix { Matrix33 = fade };
                    using (ImageAttributes attr = new ImageAttributes())
                    {
                        attr.SetColorMatrix(cm);
                        lock (loadbg[bgidx + 1])
                        {
                            g.DrawImage(loadbg[bgidx + 1], new Rectangle(0, 0, this.Width, this.Height),
                                0, 0, loadbg[bgidx + 1].Width, loadbg[bgidx + 1].Height, GraphicsUnit.Pixel, attr);
                        }
                    }
                }
                else
                {
                    lock (loadbg[bgidx])
                    {
                        g.DrawImage(loadbg[bgidx], scrollx, 0, this.Width, this.Height);
                    }
                    if (bgidx + 1 < loadbg.Count)
                    {
                        lock (loadbg[bgidx + 1])
                        {
                            g.DrawImage(loadbg[bgidx + 1], scrollx + this.Width, 0, this.Width, this.Height);
                        }
                    }
                }

                if (boat != null)
                {
                    lock (boat)
                    {
                        Rectangle st = ScaleRectangle(new Rectangle(boatx, boaty, boat.Width - 400, boat.Height - 400));
                        transitioneffects.drawbutfade(g, boat, boatx, boaty, transp1, st);
                    }
                }
                if (strip != null)
                {
                    Rectangle sc = ScaleRectangle(new Rectangle(80, 850, strip.Width, strip.Height));

                    if (gstate == 0)
                    {
                        transitioneffects.drawfade(g, strip, transp1, sc);
                    }
                    else
                    {
                        g.DrawImage(strip, sc);

                        RectangleF pd = ScaleRectangleF(new RectangleF(100, 90, 900, 300));
                        RectangleF genf = ScaleRectangleF(new RectangleF(190, 875, strip.Width - 200, 60));
                        RectangleF portf = ScaleRectangleF(new RectangleF(190, 945, strip.Width - 190, 60));

                        using (Font factFont = new Font("Book Antiqua", 11*scaleFactor, FontStyle.Bold))
                        using (Font font = new Font("Antiquity Print", 16*scaleFactor, FontStyle.Regular))

                        using (SolidBrush gbrush = new SolidBrush(Color.DarkOliveGreen))
                        using (SolidBrush pbrush = new SolidBrush(Color.DarkSlateBlue))
                        {
                            string portdt = $"Travelling to {portname}....";
                            g.DrawString(portdt, font, Brushes.Gold, pd);

                            string dy = $"Did you know? {genfact}";
                            int glen = Math.Min(gidx + 14, dy.Length);
                            string dyg = dy.Substring(0, glen);
                            g.DrawString(dyg, factFont, gbrush, genf);

                            string ap = $"About port: {pfact}";
                            int plen = Math.Min(pidx + 14, ap.Length);
                            string app = ap.Substring(0, plen);
                            g.DrawString(app, factFont, pbrush, portf);

                        }
                    }
                }
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            gtimer.Stop();

            if (boat != null)
            {
                ImageAnimator.StopAnimate(boat, OnFrameChanged);
                boat.Dispose();
                boat = null;
            }

            if (strip != null)
            {
                strip.Dispose();
                strip = null;
            }

            foreach (var im in loadbg)
            {
                if (im != null)
                {
                    ImageAnimator.StopAnimate(im, OnFrameChanged);
                    im.Dispose();
                }
            }
            loadbg.Clear();
            bg = null;
        }
    }
}
