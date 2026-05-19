using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace GTSv2_FINPROJ
{
    public partial class knowitems : Form
    {
        private int portid;
        private string portfolder;
        string portname = "";

        private kmode cmode = 0;
        Image bg;
        bool backhov = false, change = true, close = false;
        int color = 0;
        float transp0 = 0.0f;
        List<string> pfacts = new List<string>();

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;


        private bool originalSet = false;
        public knowitems(int id, kmode cmode)
        {
            InitializeComponent();
            setupform();
            this.cmode = cmode;
            this.portid = id;

            this.ResizeRedraw = true;
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
            pfacts = dbconnect.getpfacts(id);
            this.Opacity = 0;
            gtimer.Start();
        }

        private void knowitems_Load(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && this.ClientSize.Width < 100)
            {
                this.ClientSize = new Size(bg.Width, bg.Height);
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
        private void setupform()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            bg = Image.FromFile("knowitems.png");
            gtimer.Interval = 30;
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
            if (!close)
            {
                if (transp0 < 1.0f)
                {
                    transp0 += 0.05f;
                    this.Opacity = transp0;
                }
            }
            else
            {
                if (transp0 > 0.0f)
                {
                    transp0 -= 0.05f;
                }
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

        private void knowitems_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            float scaleFactor = Math.Min(scaleX, scaleY);

            if (bg != null && transp0 > 0)
            {
                lock (bg)
                {
                    transitioneffects.drawfade(g, bg, transp0, this.ClientRectangle);
                }
            }

            using (Font factfont = new Font("Book Antiqua", (int)(10 * scaleFactor), FontStyle.Bold))
            using (Font pfont = new Font("Antiquity Print", (int)(19 * scaleFactor), FontStyle.Bold))
            using (Font numfont = new Font("Antiquity Print", (int)(14 * scaleFactor), FontStyle.Bold))
            using (SolidBrush pbrush = new SolidBrush(Color.FromArgb(color, 60, 100)))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(60, 40, 20)))
            using (StringFormat format = new StringFormat())
            {
                if (pfacts != null)
                {
                    int yStart = 290;
                    for (int i = 0; i < pfacts.Count; i++)
                    {
                        if (i >= 0 && i <= 3)
                        {
                            g.DrawString((i + 1).ToString(), numfont, brush, ScalePoint(new Point(440, yStart)));
                            RectangleF factBox = ScaleRectangleF(new RectangleF(533, yStart + 5, 430, 150));
                            g.DrawString(pfacts[i], factfont, brush, factBox, format);
                            yStart += 130;
                        }
                        else
                        {
                            if (i == 4)
                            {
                                yStart = 150;
                            }
                            g.DrawString((i + 1).ToString(), numfont, brush, ScalePoint(new Point(1050, yStart)));
                            RectangleF factBox = ScaleRectangleF(new RectangleF(1133, yStart + 5, 460, 150));
                            g.DrawString(pfacts[i], factfont, brush, factBox, format);
                            yStart += 120;
                        }
                    }
                }
                g.DrawString("Port Information", pfont, pbrush, ScalePoint(new Point(425, 150)));
                Brush bbrush;

                if (backhov)
                {
                    bbrush = Brushes.CadetBlue;
                }
                else
                {
                    bbrush = Brushes.DarkBlue;
                }
                g.DrawString("<< Go back", numfont, bbrush, ScalePoint(new Point(440, 850)));
            }
        }

        private void knowitems_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle r = ScaleRectangle(new Rectangle(440, 850, 200, 200));

            if (r.Contains(e.Location))
            {
                backhov = true;
            }
            else
            {
                backhov = false;
            }
            if (backhov)
            {
                this.Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Rectangle b = ScaleRectangle(new Rectangle(440, 850, 200, 50));

            if (b.Contains(e.Location))
            {
                gtimer.Stop();
                nav.remember(this);
                this.Close();
                var n = new knowbuild(portid);
                nav.apply(n);
                nav.go(n);
            }

        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        { }
    }
}
