using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTSv2_FINPROJ
{
    public partial class wmap : Form
    {
        Image bgstart, cport;

        Image asianorm, asiahov;
        Image eunorm, euhov;
        Image usnorm, ushov;

        Image chinabut, chinahov;
        Image malbut,malhov;

        Image acabut, acahov;
        Image cartabut, cartahov;

        Image sevbut, sevhov;
        Image lisbut,lishov;
        Image menhov,mennorm;
        float menuop = 0.0f, pageop = 0.0f, butop = 0.0f, butop2 = 0.0f;
        bool showpage, showmenu;
        int cpage = 0,region = 0;

        List<PictureBox> menuButtons = new List<PictureBox>();

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;

        public wmap()
        {
            InitializeComponent();
            setupstart();
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        { }
        private void start_Load(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && this.ClientSize.Width < 100)
            {
                this.ClientSize = new Size(bgstart.Width, bgstart.Height);
            }

            originalSize = new Size(bgstart.Width, bgstart.Height);
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
     
        private void startin_Tick(object sender, EventArgs e)
        {
            showpage = true;
            showmenu = true;

            if (this.Opacity < 1.0)
            {
                this.Opacity += 0.09;
            }
            else
            {
                if (pageop < 1.0f) pageop += 0.09f;
                if (menuop < 1.0f) menuop += 0.09f;
                if (butop < 1.0f) butop += 0.09f;
                if (butop2 < 1.0f) butop2 += 0.09f;

                if (pageop >= 1.0f && menuop >= 1.0f && (butop >= 1.0f || butop2>=1.0f))
                {
                    UpdateControlVisibility();
                    startin.Stop();
                }
                this.Invalidate();
            }
        }
        private void UpdateControlVisibility()
        {
            foreach (var bt in menuButtons)
            {
                if (bt.Name == "menh")
                {
                    bt.Visible = (menuop >= 1.0f);
                }
                else if (bt.Name == "asianorm" || bt.Name == "eunorm" || bt.Name == "usnorm")
                {
                    bt.Visible = (cpage == 0 && butop >= 1.0f);
                }
                else if (bt.Name == "chinanorm" || bt.Name == "malnorm")
                {
                    bt.Visible = (region == 1 && cpage == 1 && butop2 >= 1.0f);
                }
                else if (bt.Name == "lisnorm" || bt.Name == "sevnorm")
                {
                    bt.Visible = (region == 2 && cpage == 1 && butop2 >= 1.0f);
                }
                else if (bt.Name == "acanorm" || bt.Name == "cartanorm")
                {
                    bt.Visible = (region == 3 && cpage == 1 && butop2 >= 1.0f);
                }
                else
                {
                    bt.Visible = false;
                }
                
                if (bt.Visible)
                {
                    bt.BringToFront();
                }
            }
        }
        private void start_MouseClick(object sender, MouseEventArgs e)
        {
            if (startin.Enabled)
            {
                return;
            }

            int back = (int)(this.Width * 0.5);

            if (e.X < back)
            {
                if (cpage > 0)
                {
                    cpage--;
                    resfade();
                }
            }
            else
            {
                if (cpage < 1)
                {
                    cpage++;
                    resfade();
                }
            }
            this.Invalidate();
        }
        private void resfade()
        {
            butop = 0.0f;
            pageop = 0.0f;
            butop2 = 0.0f;

            foreach(var bt in menuButtons)
            {
                if (bt.Name != "menh")
                {
                    bt.Visible = false;
                }
            }

            this.Invalidate();
            startin.Start();
        }
        private void start_MouseMove(object sender, MouseEventArgs e)
        {
            int back = (int)(this.Width * 0.5);

            if (e.X < back && cpage > 0)
            {
                this.Cursor = Cursors.PanWest;
            }
            else if (e.X > back && cpage < 1) 
            {
                this.Cursor = Cursors.PanEast;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }
        private void initializebuttons()
        {
            hovereff mbtn = new hovereff(mennorm, menhov, new Point(60, 870), exit_Click);
            mennorm = Image.FromFile(mbtn.getpath("viewtrade", "menbut.png"));
            menhov = Image.FromFile(mbtn.getpath("viewtrade", "menhov.png"));

            hovereff asia = new hovereff(asianorm, asiahov, new Point(1350, 470), (s, e) => getPage(1, 1));
            asianorm = Image.FromFile(asia.getpath("start", "asianorm.png"));
            asiahov = Image.FromFile(asia.getpath("start", "asiahov.png"));

            hovereff eu = new hovereff(eunorm, euhov, new Point(600, 400), (s, e) => getPage(1, 2));
            eunorm = Image.FromFile(eu.getpath("start", "eunorm.png"));
            euhov = Image.FromFile(eu.getpath("start", "euhov.png"));

            hovereff us = new hovereff(usnorm, ushov, new Point(190, 575), (s, e) => getPage(1, 3));
            usnorm = Image.FromFile(us.getpath("start", "usnorm.png"));
            ushov = Image.FromFile(us.getpath("start", "ushov.png"));

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


            hovereff ch = new hovereff(chinabut, chinahov, new Point(1000, 200), china);
            chinabut = Image.FromFile(ch.getpath("start", "chbut.png"));
            chinahov = Image.FromFile(ch.getpath("start", "chhov.png"));

            hovereff mal = new hovereff(malbut, malhov, new Point(1400, 200), china);
            malbut = Image.FromFile(mal.getpath("start", "malbut.png"));
            malhov = Image.FromFile(mal.getpath("start", "malhov.png"));

            ch.Name = "chinanorm";
            mal.Name = "malnorm";
            this.Controls.Add(ch);
            this.Controls.Add(mal);
            menuButtons.Add(ch);
            menuButtons.Add(mal);

            hovereff sev = new hovereff(sevbut, sevhov, new Point(1000, 200), china);
            sevbut = Image.FromFile(mbtn.getpath("start", "sevbut.png"));
            sevhov = Image.FromFile(mbtn.getpath("start", "sevhov.png"));

            hovereff lis = new hovereff(lisbut, lishov, new Point(1400, 200), china);
            lisbut = Image.FromFile(mbtn.getpath("start", "lisbut.png"));
            lishov = Image.FromFile(mbtn.getpath("start", "lishov.png"));

            sev.Name = "sevnorm";
            lis.Name = "lisnorm";
            this.Controls.Add(sev);
            this.Controls.Add(lis);
            menuButtons.Add(sev);
            menuButtons.Add(lis);

            hovereff aca = new hovereff(acabut, acahov, new Point(1000, 200), china);
            acabut = Image.FromFile(mbtn.getpath("start", "acabut.png"));
            acahov = Image.FromFile(mbtn.getpath("start", "acahov.png"));

            hovereff carta = new hovereff(cartabut, cartahov, new Point(1400, 200), china);
            cartabut = Image.FromFile(mbtn.getpath("start", "cartabut.png"));
            cartahov = Image.FromFile(mbtn.getpath("start", "cartahov.png"));

            aca.Name = "acanorm";
            carta.Name = "cartanorm";
            this.Controls.Add(aca);
            this.Controls.Add(carta);
            menuButtons.Add(aca);
            menuButtons.Add(carta);
        }

        private void setupstart()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            bgstart = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "start", "bgwmap.png"));
            this.ClientSize = new Size(bgstart.Width, bgstart.Height);

            cport = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "start", "cport.png"));

            hovereff mbtn = new hovereff(mennorm, menhov, new Point(60, 870), exit_Click);
            mennorm = Image.FromFile(mbtn.getpath("viewtrade", "menbut.png"));
            menhov = Image.FromFile(mbtn.getpath("viewtrade", "menhov.png"));

            hovereff asia = new hovereff(asianorm, asiahov, new Point(1350, 470), (s, e) => getPage(1, 1));
            asianorm = Image.FromFile(asia.getpath("start", "asianorm.png"));
            asiahov = Image.FromFile(asia.getpath("start", "asiahov.png"));

            hovereff eu = new hovereff(eunorm, euhov, new Point(600, 400), (s, e) => getPage(1, 2));
            eunorm = Image.FromFile(eu.getpath("start", "eunorm.png"));
            euhov = Image.FromFile(eu.getpath("start", "euhov.png"));

            hovereff us = new hovereff(usnorm, ushov, new Point(190, 575), (s, e) => getPage(1, 3));
            usnorm = Image.FromFile(us.getpath("start", "usnorm.png"));
            ushov = Image.FromFile(us.getpath("start", "ushov.png"));


            hovereff ch = new hovereff(chinabut, chinahov, new Point(1000, 200), china);
            chinabut = Image.FromFile(ch.getpath("start", "chbut.png"));
            chinahov = Image.FromFile(ch.getpath("start", "chhov.png"));

            hovereff mal = new hovereff(malbut, malhov, new Point(1400, 200), china);
            malbut = Image.FromFile(mal.getpath("start", "malbut.png"));
            malhov = Image.FromFile(mal.getpath("start", "malhov.png"));

            hovereff sev = new hovereff(sevbut, sevhov, new Point(1000, 200), china);
            sevbut = Image.FromFile(mbtn.getpath("start", "sevbut.png"));
            sevhov = Image.FromFile(mbtn.getpath("start", "sevhov.png"));

            hovereff lis = new hovereff(lisbut, lishov, new Point(1400, 200), china);
            lisbut = Image.FromFile(mbtn.getpath("start", "lisbut.png"));
            lishov = Image.FromFile(mbtn.getpath("start", "lishov.png"));


            hovereff aca = new hovereff(acabut, acahov, new Point(1000, 200), china);
            acabut = Image.FromFile(mbtn.getpath("start", "acabut.png"));
            acahov = Image.FromFile(mbtn.getpath("start", "acahov.png"));

            hovereff carta = new hovereff(cartabut, cartahov, new Point(1400, 200), china);
            cartabut = Image.FromFile(mbtn.getpath("start", "cartabut.png"));
            cartahov = Image.FromFile(mbtn.getpath("start", "cartahov.png"));

            startin.Interval = 30;

            initializebuttons();

            this.MouseClick += new MouseEventHandler(start_MouseClick);
            this.MouseMove += new MouseEventHandler(start_MouseMove);

            startin.Start();
        }
        private void getPage(int pnum, int reg)
        {
            cpage = pnum;
            region = reg;
            pageop = 0.0f;
            butop = 0.0f;
            butop2 = 0.0f;

            foreach (var bt in menuButtons)
            {
                if (bt.Name != "menh")
                {
                    bt.Visible = false;
                }
            }
            startin.Start();
        }

        private void start_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(bgstart, this.ClientRectangle);
            if (showpage)
            {
                if (cpage == 0)
                {
                    g.drawfade(bgstart, pageop, this.ClientRectangle);
                }
                else if(cpage == 1)
                {
                    g.drawfade(cport, pageop, this.ClientRectangle);
                }
            }

            if (showmenu && menuop < 1.0f)
            {
                Rectangle m = ScaleRectangle(new Rectangle(60, 870, mennorm.Width, mennorm.Height));
                g.drawbutfade(mennorm, 60, 870, menuop, m);
            }
            if (cpage == 0 && butop > 0.01f && butop < 1.0f)
            {
                Rectangle asiab = ScaleRectangle(new Rectangle(1350, 470, asianorm.Width, asianorm.Height));
                Rectangle eub = ScaleRectangle(new Rectangle(600, 400, eunorm.Width, eunorm.Height));
                Rectangle usb = ScaleRectangle(new Rectangle(190, 575, usnorm.Width, usnorm.Height));

                g.drawbutfade(asianorm, 1350, 470, butop, asiab);
                g.drawbutfade(eunorm, 600, 400, butop, eub);
                g.drawbutfade(usnorm, 190, 575, butop, usb);

            }
            if (cpage == 1 && region == 1 && butop2 > 0.01f && butop2 < 1.0f)
            {
                Rectangle chb = ScaleRectangle(new Rectangle(1000, 200, chinabut.Width, chinabut.Height));
                Rectangle mlb = ScaleRectangle(new Rectangle(1400, 200, malbut.Width, malbut.Height));

                g.drawbutfade(chinabut, 1000, 200, butop2, chb);
                g.drawbutfade(malbut, 1400, 200, butop2, mlb);
            }
            if (cpage == 1 && region == 2 && butop2 > 0.01f && butop2 < 1.0f)
            {
                Rectangle sev = ScaleRectangle(new Rectangle(1000, 200, sevbut.Width, sevbut.Height));
                Rectangle lis = ScaleRectangle(new Rectangle(1400, 200, lisbut.Width, lisbut.Height));

                g.drawbutfade(sevbut, 1000, 200, butop2, sev);
                g.drawbutfade(lisbut, 1400, 200, butop2, lis);
            }
            if (cpage == 1 && region == 3 && butop2 > 0.01f && butop2 < 1.0f)
            {
                Rectangle acb = ScaleRectangle(new Rectangle(1000, 200, acabut.Width, acabut.Height));
                Rectangle crb = ScaleRectangle(new Rectangle(1400, 200, cartabut.Width, cartabut.Height));

                g.drawbutfade(acabut, 1000, 200, butop2, acb);
                g.drawbutfade(cartabut, 1400, 200, butop2, crb);
            }
        }
        private void exit_Click(object sender, EventArgs e)
        {
            startin.Stop();
            nav.remember(this);
            this.Close();
            var n = new Intro();
            nav.apply(n);
            nav.go(n);
            return;
        }

        private void china(object sender, EventArgs e)
        {
            hovereff click = (hovereff)sender;
            int portid = 0;

            switch (click.Name)
            {
                case "chinanorm":
                    portid = 1;
                    break;
                case "malnorm":
                    portid = 2;
                    break;
                case "sevnorm":
                    portid = 3;
                    break;
                case "lisnorm":
                    portid = 4;
                    break;
                case "acanorm":
                    portid = 5;
                    break;
                case "cartanorm":
                    portid = 6;
                    break;
            }
            startin.Stop();
            nav.remember(this);
            this.Close();
            var n = new portloading(portid);
            nav.apply(n);
            nav.go(n);
            return;


        }
    }
}
