using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GTSv2_FINPROJ
{
    public partial class Knowjourn : Form
    {
        Image bgkj, frontp, genp, portp;
       
        Image asianorm, asiahov, eunorm;
        Image euhov, usnorm, ushov;

        Image port1f, port2f;
        Image menhov, mennorm;

        float menuop = 0.0f, pageop = 0.0f, butop = 0.0f;
        bool showmenu = false, showpage = false;
        bool backhov = false, nexthov = false;

        int cpage = 0, currentCont = 0;

        List<PictureBox> menuButtons = new List<PictureBox>();
        List<string> gfacts = new List<string>();
        List<string> pfacts = new List<string>();

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;

        public Knowjourn()
        {
            InitializeComponent();
            setupkj();
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        { }
        private void Knowjourn_Load(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && this.ClientSize.Width < 100)
                this.ClientSize = new Size(bgkj.Width, bgkj.Height);

            originalSize = new Size(bgkj.Width, bgkj.Height);
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
                butop = 1.0f;
                UpdateControlVisibility();
            }
            else
            {
                this.Opacity = 0;
                kjin.Start();
            }

            this.Invalidate();
        }

        private void kjin_Tick(object sender, EventArgs e)
        {
            showpage = true;
            showmenu = true;

            if (this.Opacity < 1.0)
            {
                this.Opacity += 0.09;
            }
            else
            {
                if(cpage == 0 && pageop < 1.0f)
                {
                    pageop += 0.05f;
                    menuop += 0.045f;
                }
                else
                {
                    pageop = 1.0f;
                    menuop = 1.0f;
                    butop = 1.0f;
                    this.Invalidate();
                    UpdateControlVisibility();
                    kjin.Stop();
                }
                this.Invalidate();
            }
  
        }

        private void UpdateControlVisibility()
        {
            foreach(var bt in menuButtons)
            {
                if (bt.Name == "menh")
                {
                    bt.Visible = true;
                }
                else if (bt.Name == "asianorm" || bt.Name == "eunorm" || bt.Name == "usnorm")
                {
                    if(cpage == 2)
                    {
                        bt.Visible = true;
                    }
                    else
                    {
                        bt.Visible = false;
                    }
                }

                if (bt.Visible)
                {
                    bt.BringToFront();
                }
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
        private void Knowjourn_MouseClick(object sender, MouseEventArgs e)
        {
            int back = (int)(this.ClientSize.Width * 0.5);

            if (e.X < back)
            {
                if (cpage == 3)
                {
                    cpage = 2;
                }
                else if (cpage == 4)
                {
                    cpage = 3;

                    int firstPortID = 0;
                    if (currentCont == 1)
                    {
                        firstPortID = 1; 
                    }
                    else if (currentCont == 2)
                    {
                        firstPortID = 3;
                    }
                    else if (currentCont == 3)
                    {
                        firstPortID = 5; 
                    }

                    pfacts = dbconnect.getunlockedfacts(playdata.currentid, firstPortID, false);
                }
                else if (cpage > 0)
                {
                    cpage--;
                    if (cpage == 1)
                    {
                        gfacts = dbconnect.getunlockedfacts(playdata.currentid, 0, true);
                    }
                }
                resfade();
            }
            else
            {
                int secondPortID = 0;
                if (cpage == 2)
                {
                    gfacts = dbconnect.getunlockedfacts(playdata.currentid, 0, true);
                }
                else if (cpage == 3)
                {
                    cpage = 4;

                    if (currentCont == 1) 
                    { 
                        secondPortID = 2; 
                    }
                    else if (currentCont == 2) 
                    { 
                        secondPortID = 4; 
                    } 
                    else if (currentCont == 3) 
                    { 
                        secondPortID = 6; 
                    }
                    pfacts = dbconnect.getunlockedfacts(playdata.currentid, secondPortID, false);
                }
                else if (cpage == 0)
                {
                    cpage = 1;
                    gfacts = dbconnect.getunlockedfacts(playdata.currentid, 0, true);
                }
                else if (cpage == 4)
                {
                    return;
                }
                else if (cpage < 2)
                {
                        cpage++;
                }
                resfade();
            }
            this.Invalidate();
        }

        private void resfade()
        {
            if (cpage == 0)
            {
                pageop = 0.0f;
                kjin.Start();
            }
            else
            {
                pageop = 1.0f;
                menuop = 1.0f;
                UpdateControlVisibility();
                this.Invalidate();
                kjin.Stop();
            }
        }
        private void Knowjourn_MouseMove(object sender, MouseEventArgs e)
        {
            int back = (int)(this.ClientSize.Width * 0.5);

            if (e.X < back && cpage > 0)
            {
                this.Cursor = Cursors.PanWest;
                backhov = true;
                nexthov = false;
            }
            else if (e.X > back && cpage < 5)
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
            bool ob = backhov;

            if (backhov)
            {
                this.Invalidate(); 
            }
        }
        private void initializebuttons()
        {
            hovereff mbtn = new hovereff(mennorm, menhov, new Point(60, 870), exit_Click);
            mennorm = Image.FromFile(mbtn.getpath("viewtrade", "menbut.png"));
            menhov = Image.FromFile(mbtn.getpath("viewtrade", "menhov.png"));

            hovereff asia = new hovereff(asianorm, asiahov, new Point(500, 230), (s, e) => getPage(3, 1));
            asianorm = Image.FromFile(asia.getpath("knowjourn", "asianorm.png"));
            asiahov = Image.FromFile(asia.getpath("knowjourn", "asiahov.png"));

            hovereff eu = new hovereff(eunorm, euhov, new Point(1200, 230), (s, e) => getPage(3, 3));
            eunorm = Image.FromFile(eu.getpath("knowjourn", "eunorm.png"));
            euhov = Image.FromFile(eu.getpath("knowjourn", "euhov.png"));

            hovereff us = new hovereff(usnorm, ushov, new Point(840, 500), (s, e) => getPage(3, 5));
            usnorm = Image.FromFile(us.getpath("knowjourn", "usnorm.png"));
            ushov = Image.FromFile(us.getpath("knowjourn", "ushov.png"));

            mbtn.Name = "menh";
            asia.Name = "asianorm";
            eu.Name = "eunorm";
            us.Name = "usnorm";

            this.Controls.Add(mbtn);
            this.Controls.Add(asia);
            this.Controls.Add(eu);
            this.Controls.Add(us);

            menuButtons.Add(mbtn);
            menuButtons.Add(asia);
            menuButtons.Add(eu);
            menuButtons.Add(us);
        }
        private void getPage(int pnum, int portdbid)
        {
            cpage = pnum;
            pageop = 0.0f;
            butop = 0.0f;

            if (portdbid == 1)
            {
                currentCont = 1; 
                port1f = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "knowjourn", "1.png"));
                port2f = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "knowjourn", "2.png"));
            }
            else if (portdbid == 3)
            {
                currentCont = 2; 
                port1f = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "knowjourn", "3.png"));
                port2f = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "knowjourn", "4.png"));
            }
            else if (portdbid == 5)
            {
                currentCont = 3; 
                port1f = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "knowjourn", "5.png"));
                port2f = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "knowjourn", "6.png"));
            }

            gfacts = dbconnect.getunlockedfacts(playdata.currentid, 0, true);
            pfacts = dbconnect.getunlockedfacts(playdata.currentid, portdbid, false);

            kjin.Start();
        }
        private void setupkj()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            bgkj = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "knowjourn", "bgkjournal.png"));

            frontp = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "knowjourn", "frontp.png"));
            genp = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "knowjourn", "genp.png"));
            portp = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "knowjourn", "portp.png"));

            hovereff mbtn = new hovereff(mennorm, menhov, new Point(60, 870), exit_Click);
            mennorm = Image.FromFile(mbtn.getpath("viewtrade", "menbut.png"));
            menhov = Image.FromFile(mbtn.getpath("viewtrade", "menhov.png"));

            hovereff asia = new hovereff(asianorm, asiahov, new Point(500, 230), (s, e) => getPage(3, 1));
            asianorm = Image.FromFile(asia.getpath("knowjourn", "asianorm.png"));
            asiahov = Image.FromFile(asia.getpath("knowjourn", "asiahov.png"));

            hovereff eu = new hovereff(eunorm, euhov, new Point(1200, 230), (s, e) => getPage(3, 3));
            eunorm = Image.FromFile(eu.getpath("knowjourn", "eunorm.png"));
            euhov = Image.FromFile(eu.getpath("knowjourn", "euhov.png"));

            hovereff us = new hovereff(usnorm, ushov, new Point(840, 500), (s, e) => getPage(3, 5));
            usnorm = Image.FromFile(us.getpath("knowjourn", "usnorm.png"));
            ushov = Image.FromFile(us.getpath("knowjourn", "ushov.png"));

            kjin.Interval = 30;
            initializebuttons();

            this.MouseClick += new MouseEventHandler(Knowjourn_MouseClick);
            this.MouseMove += new MouseEventHandler(Knowjourn_MouseMove);
        }

        private void Knowjourn_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            if (bgkj != null)
            {
                lock (bgkj)
                {
                    g.DrawImage(bgkj, this.ClientRectangle);
                }
            }

            float scaleFactor = Math.Min(scaleX, scaleY);
            if (showpage)
            {
                Image activep = frontp;

                if(activep != null)
                {
                    lock (activep)
                    {
                        if (cpage == 1)
                        {
                            activep = genp;
                        }
                        else if (cpage == 2)
                        {
                            activep = portp;

                        }
                        else if (cpage == 3)
                        {
                            activep = port1f;
                        }
                        else if (cpage == 4)
                        {
                            activep = port2f;
                        }
                        if (cpage == 0)
                        {
                            g.drawfade(frontp, pageop, this.ClientRectangle);
                        }
                        else
                        {
                            g.DrawImage(activep, this.ClientRectangle);
                        }
                    }
                }
                if (cpage == 1 || cpage == 3)
                {
                    Font nav = new Font("Antiquity Print", 14*scaleFactor, FontStyle.Regular);
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
                    g.DrawString("<< Back", nav, bbrush, ScalePoint(new Point(420, 850)));
                    g.DrawString(">> Next", nav, nbrush, ScalePoint(new Point(1430, 850)));
                }
                if (cpage == 2 || cpage == 4)
                {
                    Font nav = new Font("Antiquity Print", 14 * scaleFactor, FontStyle.Regular);
                    Brush bbrush;
                    if (backhov == true && nexthov == false)
                    {
                        bbrush = Brushes.Red;
                    }
                    else if (backhov == false && nexthov == true)
                    {
                        bbrush = Brushes.Brown;
                    }
                    else
                    {
                        bbrush = Brushes.Brown;
                    }
                    g.DrawString("<< Back", nav, bbrush, ScalePoint(new Point(420, 850)));
                }
                
                using (Font factfont = new Font("Book Antiqua", 10 * scaleFactor, FontStyle.Bold))
                using (Font numfont = new Font("Antiquity Print", 14 * scaleFactor, FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(60, 40, 20)))
                using (StringFormat format = new StringFormat())
                {
                    if (cpage == 1)
                    {
                        if (gfacts != null)
                        {
                            int yStart = 350;
                            for (int i = 0; i < gfacts.Count; i++)
                            {
                                if(i >= 0 && i<= 3)
                                {
                                    g.DrawString((i + 1).ToString(), numfont, brush, ScalePoint(new Point(447, yStart)));
                                    RectangleF factBox = ScaleRectangleF(new RectangleF(533, yStart + 5, 430, 150));
                                    g.DrawString(gfacts[i], factfont, brush, factBox, format);
                                    yStart += 120;
                                }
                                else
                                {
                                    if (i == 4)
                                    {
                                        yStart = 150;
                                    }
                                    g.DrawString((i + 1).ToString(), numfont, brush, ScalePoint(new Point(1050, yStart)));
                                    RectangleF factBox = ScaleRectangleF(new RectangleF(1133, yStart + 5, 430, 150));
                                    g.DrawString(gfacts[i], factfont, brush, factBox, format);
                                    yStart += 120;
                                }
                            }
                        }
                    }
                    else if (cpage == 3 || cpage == 4)
                    {
                        if (pfacts != null)
                        {
                            int yStart = 350;
                            for (int i = 0; i < pfacts.Count; i++)
                            {
                                if (i >= 0 && i <= 3)
                                {
                                    g.DrawString((i + 1).ToString(), numfont, brush, ScalePoint(new Point(447, yStart)));
                                    RectangleF factBox = ScaleRectangleF(new RectangleF(533, yStart + 5, 430, 150));
                                    g.DrawString(pfacts[i], factfont, brush, factBox, format);
                                    yStart += 120;
                                }
                                else
                                {
                                    if(i == 4)
                                    {
                                        yStart = 150;
                                    }
                                    g.DrawString((i + 1).ToString(), numfont, brush, ScalePoint(new Point(1050, yStart)));
                                    RectangleF factBox = ScaleRectangleF(new RectangleF(1133, yStart + 5, 430, 150));
                                    g.DrawString(pfacts[i], factfont, brush, factBox, format);
                                    yStart += 120;
                                }
                            }
                        }
                    }
                }
                if (showmenu && menuop < 1.0f)
                {
                    Rectangle m = ScaleRectangle(new Rectangle(60, 870, mennorm.Width, mennorm.Height));
                    g.drawbutfade(mennorm, 60, 870, menuop, m);
                }

                if (cpage == 2 && butop < 1.0f)
                {
                    if (butop < 1.0f)
                    {
                        Rectangle asiab = ScaleRectangle(new Rectangle(500, 230, asianorm.Width, asianorm.Height));
                        Rectangle eub = ScaleRectangle(new Rectangle(1200, 230, eunorm.Width, eunorm.Height));
                        Rectangle usb = ScaleRectangle(new Rectangle(840, 500, usnorm.Width, usnorm.Height));

                        g.drawbutfade(asianorm, 500, 230, butop, asiab);
                        g.drawbutfade(eunorm, 1200, 230, butop, eub);
                        g.drawbutfade(usnorm, 840, 500, butop, usb);
                    }
                }
            }
        }
        
        private void exit_Click(object sender, EventArgs e)
        {
            kjin.Stop();
            nav.remember(this);
            this.Close();
            var n = new Intro();
            nav.apply(n);
            nav.go(n);
            return;
        }
    }
}

