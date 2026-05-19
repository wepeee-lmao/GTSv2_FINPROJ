using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace GTSv2_FINPROJ
{
    public partial class nventunit : Form
    {
        private int portid;
        private int catid;
        private string cat = "Units Information";
        private mode cmode;
        private string portfolder;
        string portname = "";

        Image bg;
        bool next = false, prev = false;
        bool change = true, close = false;
        int color = 0;
        float transp0 = 1.0f;  
        List<unitdata> pfacts = new List<unitdata>();

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;

        int cpage = 1;

        public nventunit(int portid, mode cmode)
        {
            InitializeComponent();

            this.portid = portid;
            this.cmode = cmode;
            this.catid = 1;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            if (System.IO.File.Exists("unitbg.png"))
            {
                bg = Image.FromFile("unitbg.png");
            }
            this.UpdateStyles();
            gtimer.Interval = 30;
        }

        private void nventunit_Load(object sender, EventArgs e)
        {
            this.Opacity = 1.0; 
            originalSize = new Size(bg.Width, bg.Height);
            this.ClientSize = originalSize;
            this.CenterToScreen();

            this.Resize += new EventHandler(nventunit_Resize);

            pfacts = dbconnect.getunits();

            gtimer.Start();
        }

        private void nventunit_Resize(object sender, EventArgs e)
        {
            if (originalSize.Width == 0 || originalSize.Height == 0)
            {
                return;
            }

            scaleX = (float)this.ClientSize.Width / originalSize.Width;
            scaleY = (float)this.ClientSize.Height / originalSize.Height;

            this.Invalidate();
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

        private void gtimer_Tick(object sender, EventArgs e)
        {
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

        private void nventunit_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            if (bg != null)
            {
                g.DrawImage(bg, this.ClientRectangle);  
            }

            float scale = Math.Min(scaleX, scaleY);

            using (Font tfont = new Font("Antiquity Print", 20 * scale, FontStyle.Bold))
            using (Font catefont = new Font("Book Antiqua", 12 * scale, FontStyle.Bold | FontStyle.Underline))
            using (Font namefont = new Font("Antiquity Print", 10 * scale, FontStyle.Bold))
            using (Font descfont = new Font("Book Antiqua", 10 * scale, FontStyle.Bold))
            using (SolidBrush tbrush = new SolidBrush(Color.FromArgb(color, 60, 100)))
            using (SolidBrush txbrush = new SolidBrush(Color.FromArgb(60, 40, 20)))
            using (SolidBrush headbrush = new SolidBrush(Color.FromArgb(120, 50, 20)))
            using (StringFormat format = new StringFormat())
            {
                g.DrawString("Unit Guide", tfont, tbrush, ScalePoint(new Point(1550, 40)));

                if (cpage == 1)
                {
                    SolidBrush nextBrush;
                    if (next == true)
                    {
                        nextBrush = new SolidBrush(Color.CadetBlue);
                    }
                    else
                    {
                        nextBrush = new SolidBrush(Color.DarkBlue);
                    }
                    g.DrawString("Next Page >>", namefont, nextBrush, ScalePoint(new Point(1600, 950)));
                    nextBrush.Dispose();
                }
                else if (cpage == 2)
                {
                    g.DrawString("Press ENTER to return", namefont, txbrush, ScalePoint(new Point(1400, 950)));

                    SolidBrush prevBrush;
                    if (prev == true)
                    {
                        prevBrush = new SolidBrush(Color.CadetBlue);
                    }
                    else
                    {
                        prevBrush = new SolidBrush(Color.DarkBlue);
                    }
                    g.DrawString("<< Previous Page", namefont, prevBrush, ScalePoint(new Point(100, 950)));
                    prevBrush.Dispose();
                }

                if (pfacts != null && pfacts.Count > 0)
                {
                    string left = "";
                    string right = "";

                    if (cpage == 1)
                    {
                        left = "Traditional & Regional Units";
                        right = "Bulk Shipping & Industrial Units";
                    }
                    else if (cpage == 2)
                    {
                        left = "Small & Luxury Containers";
                        right = "Countable & Miscellaneous Units";
                    }

                    int leftY = 50;
                    int rightY = 50;

                    g.DrawString(left, catefont, headbrush, ScalePoint(new Point(80, leftY)));
                    leftY += 45;

                    g.DrawString(right, catefont, headbrush, ScalePoint(new Point(900, rightY)));
                    rightY += 45;

                    for (int i = 0; i < pfacts.Count; i++)
                    {
                        unitdata item = pfacts[i];

                        if (item.Category == left)
                        {
                            g.DrawString(item.Name, namefont, Brushes.DarkOliveGreen, ScalePoint(new Point(90, leftY)));
                            leftY += 35;

                            RectangleF descBox = ScaleRectangleF(new RectangleF(90, leftY, 640, 100));
                            g.DrawString(item.Info, descfont, txbrush, descBox, format);

                            SizeF textSize = g.MeasureString(item.Info, descfont, (int)(640 * scaleX));
                            leftY += (int)(textSize.Height / scaleY) + 10;
                        }
                        else if (item.Category == right)
                        {
                            g.DrawString(item.Name, namefont, Brushes.DarkOliveGreen, ScalePoint(new Point(910, rightY)));
                            rightY += 35;

                            RectangleF descBox = ScaleRectangleF(new RectangleF(910, rightY, 640, 100));
                            g.DrawString(item.Info, descfont, txbrush, descBox, format);

                            SizeF txsize = g.MeasureString(item.Info, descfont, (int)(640 * scaleX));
                            rightY += (int)(txsize.Height / scaleY) + 10;
                        }
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Rectangle nextb = ScaleRectangle(new Rectangle(1600, 950, 200, 50));
            Rectangle prevb = ScaleRectangle(new Rectangle(100, 950, 200, 50));

            if (cpage == 1 && nextb.Contains(e.Location))
            {
                next = true;
            }
            else
            {
                next = false;
            }

            if (cpage == 2 && prevb.Contains(e.Location))
            {
                prev = true;
            }
            else
            {
                prev = false;
            }

            this.Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                gtimer.Stop();
                this.Close();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Rectangle nextb = ScaleRectangle(new Rectangle(1600, 950, 200, 50));
            Rectangle prevb = ScaleRectangle(new Rectangle(100, 950, 200, 50));

            if (cpage == 1)
            {
                if (nextb.Contains(e.Location))
                {
                    cpage = 2;
                    this.Invalidate();
                }
            }
            else if (cpage == 2)
            {
                if (prevb.Contains(e.Location))
                {
                    cpage = 1;
                    this.Invalidate();
                }
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        { }
    }
}