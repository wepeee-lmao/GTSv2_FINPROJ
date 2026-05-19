using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace GTSv2_FINPROJ
{
    public partial class nventitems : Form
    {
        private int portid { get; set; }
        private string portfolder;
        private int selectid = -1;
        private int quanti = 1;
        private int qty = 0;
        private mode cmode = 0;
        Image bg, bn, bh, sn, sh, un, uh;

        float lab0 = 0.0f, transp0 = 0.0f, butop = 0.0f;
        int idx1 = 0, idx2 = 0, speed = 0;

        bool x = false, close = false;
        bool backhov = false;

        List<hovereff> menuButtons = new List<hovereff>();

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;

        private Point originalItemnmLoc;
        private Point originalUnitLoc;
        private Point originalKilosLoc;
        private Point originalPriceLoc;
        private Point originalAskLoc;
        private Point originalDescLoc;
        private Point originalQuestionLoc;


        public nventitems(int id, mode cmode)
        {
            InitializeComponent();
            this.ResizeRedraw = true;
            this.portid = id;
            this.cmode = cmode;
            switch (id)
            {
                case 1:
                    portfolder = "chinaport";
                    break;
                case 2:
                    portfolder = "malport";
                    break;
                case 3:
                    portfolder = "sevport";
                    break;
                case 4:
                    portfolder = "lisport";
                    break;
                case 5:
                    portfolder = "acaport";
                    break;
                case 6:
                    portfolder = "cartaport";
                    break;
                case 7:
                    portfolder = "manilaport";
                    break;
            }
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            setupform();
        }

        private void gtimer_Tick(object sender, EventArgs e)
        {
            if (!close)
            {
                if (transp0 < 1.0f)
                {
                    transp0 += 0.09f;
                }
            }
            else
            {
                if (transp0 > 0.0f)
                {
                    transp0 -= 0.09f;
                }
            }

            if (transp0 >= 0.0f && butop < 1.0f)
            {
                butop += 0.02f;
                if (butop > 1.0f)
                {
                    butop = 1.0f;
                }
            }
            else if (butop >= 1.0f)
            {
                x = true;
            }
            string ch = "";

            string q = "";
            switch (cmode)
            {
                case mode.buy:
                    q = "Please choose quantity";
                    ch = "buy";
                    break;
                case mode.sell:
                    q = "Please choose quantity";
                    ch = "sell";
                    break;
                case mode.viewcargo:
                    q = "Current item quantity";
                    ch = "view";
                    break;
            }
            string l = $"What would you like to {ch}?";

            if (lab0 < 1.0f)
            {
                lab0 += 0.01f;
                price.ForeColor = texteff.getfadecolor(Color.Red, Color.Green, lab0);
            }

            speed++;
            if (speed >= 1)
            {
                if (idx1 < q.Length)
                {
                    ask.Text = texteff.Typewrite(q, idx1++);
                    speed = 0;
                }
                if (idx2 < l.Length)
                {
                    question.Text = texteff.Typewrite(l, idx2++);
                    speed = 0;
                }
            }

            this.Invalidate();
        }

        private void nventitems_Load(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && this.ClientSize.Width < 100)
            {
                this.ClientSize = new Size(bg.Width, bg.Height);
            }

            originalSize = new Size(bg.Width, bg.Height);

            scaleX = (float)this.ClientSize.Width / originalSize.Width;
            scaleY = (float)this.ClientSize.Height / originalSize.Height;

            this.Resize += new EventHandler(nventitems_Resize);

            if (this.WindowState == FormWindowState.Normal)
            {
                this.CenterToScreen();
            }
            originalItemnmLoc = new Point(1110, 200);
            originalUnitLoc = new Point(1110, 300);
            originalKilosLoc = new Point(1600, 300);
            originalPriceLoc = new Point(320, 55);
            originalAskLoc = new Point(1110, 515);
            originalDescLoc = new Point(1110, 360);
            originalQuestionLoc = new Point(1110, 80);

            UpdateLabelPositions();
            UpdateButtonPositions();

            itemnm.Visible = false;
            unit.Visible = false;
            kilos.Visible = false;
            price.Visible = false;
            ask.Visible = false;
            desc.Visible = false;
            question.Visible = false;

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(nventitems_KeyDown);
            this.MouseClick += new MouseEventHandler(nventitems_MouseClick);

            this.Invalidate();
        }

        private void nventitems_Resize(object sender, EventArgs e)
        {
            if (originalSize.Width == 0 || originalSize.Height == 0)
            {
                return;
            }

            scaleX = (float)this.ClientSize.Width / originalSize.Width;
            scaleY = (float)this.ClientSize.Height / originalSize.Height;

            UpdateLabelPositions();
            UpdateButtonPositions();

            if (selectid != -1)
            {
                updatequantity();
            }

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
        private void UpdateLabelPositions()
        {
            itemnm.Location = ScalePoint(originalItemnmLoc);
            unit.Location = ScalePoint(originalUnitLoc);
            kilos.Location = ScalePoint(originalKilosLoc);
            price.Location = ScalePoint(originalPriceLoc);
            ask.Location = ScalePoint(originalAskLoc);
            desc.Location = ScalePoint(originalDescLoc);
            question.Location = ScalePoint(originalQuestionLoc);
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

        private void setupform()
        {
            bg = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nventnorm", "nventbg.png"));

            bn = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nventnorm", "buynorm.png"));
            bh = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nventhov", "buyhov.png"));

            sn = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nventnorm", "sellnorm.png"));
            sh = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nventhov", "sellhov.png"));

            un = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nventnorm", "unitnorm.png"));
            uh = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nventhov", "unithov.png"));

            hovereff unitBtn = new hovereff(un, uh, new Point(1650, 645), (s, e) => openunit());
            unitBtn.Visible = true;
            this.Controls.Add(unitBtn);
            unitBtn.BringToFront();

            Point[] slots = new Point[] {
                new Point(180, 190), new Point(450, 190), new Point(710, 190),
                new Point(180, 390), new Point(450, 390), new Point(710, 390),
                new Point(180, 590), new Point(450, 590), new Point(710, 590),
                new Point(450, 790)
            };

            switch (cmode)
            {
                case mode.buy:
                    hovereff buyb = new hovereff(bn, bh, new Point(1120, 950), (s, e) => btnConfirmBuy_Click(s, e));
                    buyb.Visible = true;
                    this.Controls.Add(buyb);
                    buyb.BringToFront();

                    for (int i = 0; i < 10; i++)
                    {
                        int fn = ((this.portid - 1) * 10) + (i + 1);
                        createbut(fn, slots[i]);
                    }
                    break;

                case mode.sell:
                    hovereff sellb = new hovereff(sn, sh, new Point(1120, 950), (s, e) => btnConfirmSell_Click(s, e));
                    sellb.Visible = true;
                    this.Controls.Add(sellb);
                    sellb.BringToFront();

                    DataTable dt = dbconnect.getnventory(playdata.currentid);

                    for (int i = 0; i < dt.Rows.Count && i < 10; i++)
                    {
                        int myid = Convert.ToInt32(dt.Rows[i]["itemID"]);
                        string name = dt.Rows[i]["itemname"].ToString();
                        qty = Convert.ToInt32(dt.Rows[i]["quantity"]);

                        createbut(myid, slots[i]);
                    }
                    break;
                case mode.viewcargo:
                    DataTable dtv = dbconnect.getnventory(playdata.currentid);
                    for (int i = 0; i < dtv.Rows.Count && i < 10; i++)
                    {
                        int myid = Convert.ToInt32(dtv.Rows[i]["itemID"]);
                        qty = Convert.ToInt32(dtv.Rows[i]["quantity"]);
                        createbut(myid, slots[i]);
                    }
                    break;
            }
            gtimer.Start();
            gtimer.Interval = 20;
        }

        private void openunit()
        {
            using (nventunit unitForm = new nventunit(portid, cmode))
            {
                unitForm.ShowDialog(this);
            }
        }

        private void createbut(int itemid, Point loc)
        {
            try
            {
                string normPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nventnorm", $"{itemid}.png");
                string hovPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nventhov", $"{itemid}.png");

                Image norm = Image.FromFile(normPath);
                Image hov = Image.FromFile(hovPath);

                hovereff itemBtn = new hovereff(norm, hov, loc, (s, e) =>
                {
                    hovereff click = (hovereff)s;
                    this.selectid = click.dataid;

                    dbconnect db = new dbconnect();
                    db.itemclick(s, e, itemnm, price, unit, kilos, desc);

                    this.quanti = 1;
                    updatequantity();
                });

                itemBtn.dataid = itemid;
                itemBtn.Visible = true;
                this.Controls.Add(itemBtn);
                itemBtn.BringToFront();
                menuButtons.Add(itemBtn);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading item {itemid}: {ex.Message}");
            }
        }

        private void nventitems_MouseClick(object sender, MouseEventArgs e)
        {
            Rectangle backButton = ScaleRectangle(new Rectangle(50, 1010, 200, 50));
            if (backButton.Contains(e.Location))
            {
                gtimer.Stop();
                nav.remember(this);
                this.Close();

                if (cmode == mode.viewcargo)
                {
                    var n = new Vtrecord();
                    nav.apply(n);
                    nav.go(n);
                }
                else
                {
                    var n = new nventbuild(portid);
                    nav.apply(n);
                    nav.go(n);
                }
                return;
            }

            Rectangle unitbutton = ScaleRectangle(new Rectangle(1110, 380, 750, 250));
            if (unitbutton.Contains(e.Location))
            {
                openunit();
            }
        }

        private void nventitems_Mousemove(object sender, MouseEventArgs e)
        {
            Rectangle r = ScaleRectangle(new Rectangle(50, 1010, 200, 200));

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

        private void nventitems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
            {
                if (quanti > 1) quanti--;
                updatequantity();
            }
            else if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
            {
                if (quanti < 5) quanti++;
                updatequantity();
            }
        }

        private void updatequantity()
        {
            System.Windows.Forms.Label[] qLabels = { q1, q2, q3, q4, q5 };
            string unitText = "units";
            if (!string.IsNullOrEmpty(unit.Text))
            {
                unitText = unit.Text.Split(' ').Last();
            }

            int x = (int)(1250 * scaleX);
            int y = (int)(640 * scaleY);
            int space = (int)(50 * scaleY);

            switch (cmode)
            {
                case mode.buy:
                    for (int i = 0; i < qLabels.Length; i++)
                    {
                        int currentVal = i + 1;
                        qLabels[i].Location = new Point(x, y + (i * space));
                        qLabels[i].Font = new Font("Antiquity Print", 12 * Math.Min(scaleX, scaleY), FontStyle.Regular);
                        qLabels[i].BringToFront();

                        if (currentVal == quanti)
                        {
                            qLabels[i].ForeColor = Color.FromArgb(188, 0, 0);
                            qLabels[i].Text = $">> I'll buy {currentVal} {unitText}";
                        }
                        else
                        {
                            qLabels[i].ForeColor = Color.Black;
                            qLabels[i].Text = $"I'll buy {currentVal} {unitText}";
                        }
                    }
                    break;

                case mode.sell:
                    int disp = Math.Min(qLabels.Length, qty);
                    for (int i = 0; i < qLabels.Length; i++)
                    {
                        int currentVal = i + 1;
                        qLabels[i].Font = new Font("Antiquity Print", 12 * Math.Min(scaleX, scaleY), FontStyle.Regular);
                        qLabels[i].BringToFront();

                        if (i < disp)
                        {
                            qLabels[i].Visible = true;
                            qLabels[i].Location = new Point(x, y + (i * space));
                            qLabels[i].BringToFront();
                            if (currentVal == quanti)
                            {
                                qLabels[i].ForeColor = Color.FromArgb(188, 0, 0);
                                qLabels[i].Text = $">> I'll sell {currentVal} {unitText}";
                            }
                            else
                            {
                                qLabels[i].ForeColor = Color.Black;
                                qLabels[i].Text = $"I'll sell {currentVal} {unitText}";
                            }
                        }
                        else
                        {
                            qLabels[i].Visible = false;
                        }
                    }
                    break;
                case mode.viewcargo:
                    int dispv = Math.Min(qLabels.Length, qty);
                    for (int i = 0; i < qLabels.Length; i++)
                    {
                        qLabels[i].Font = new Font("Antiquity Print", 12 * Math.Min(scaleX, scaleY), FontStyle.Regular);
                        qLabels[i].BringToFront();

                        if (i == 0 && qty > 0)
                        {
                            qLabels[i].Visible = true;
                            x = (int)(1150 * scaleX);
                            y = (int)(730 * scaleY);
                            qLabels[i].Location = new Point(x, y);
                            qLabels[i].ForeColor = Color.DarkBlue;
                            qLabels[i].Text = $"You currently have {qty} in cargo.";
                        }
                        else
                        {
                            qLabels[i].Visible = false;
                        }
                    }
                    break;
            }
        }

        private void btnConfirmBuy_Click(object sender, EventArgs e)
        {
            if (selectid == -1) return;

            try
            {
                string wval = kilos.Text.Split(' ')[0];
                int iweight = int.Parse(wval);

                string pval = price.Text.Split(' ')[0];
                int iprice = int.Parse(pval);

                int totweightp = iweight * quanti;

                if (dbconnect.BuyItem(selectid, quanti, iweight, iprice, portid))
                {
                    MessageBox.Show($"Bought {quanti} units! Total Weight: {totweightp}kg\n" +
                                    $"Remaining Cargo Space: {playdata.curkilos}kg");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Input error: " + ex.Message);
            }
        }

        private void btnConfirmSell_Click(object sender, EventArgs e)
        {
            if (selectid == -1) return;

            try
            {
                string wval = kilos.Text.Split(' ')[0];
                int iweight = int.Parse(wval);

                string pval = price.Text.Split(' ')[0];
                int iprice = int.Parse(pval);

                int totfree = iweight * quanti;
                int totgain = iprice * quanti;

                if (dbconnect.sellitem(playdata.currentid, selectid, quanti, iprice, iweight))
                {
                    MessageBox.Show($"Sold {quanti} units!");

                    foreach (var btn in menuButtons)
                    {
                        this.Controls.Remove(btn);
                        btn.Dispose();
                    }

                    menuButtons.Clear();

                    this.selectid = -1;
                    gtimer.Stop();
                    setupform();
                    this.Invalidate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Selling error: " + ex.Message);
            }
        }

        private void nventitems_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            if (bg != null && transp0 > 0)
            {
                lock (bg)
                {
                    transitioneffects.drawfade(g, bg, transp0, this.ClientRectangle);
                }
            }
            float scaleFactor = Math.Min(scaleX, scaleY);

            using (SolidBrush ibrush = new SolidBrush(Color.FromArgb(255, 244, 235)))
            using (SolidBrush dbrush = new SolidBrush(Color.FromArgb(208, 255, 121)))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 117, 31)))
            using (SolidBrush mbrush = new SolidBrush(Color.FromArgb(254, 219, 112)))

            using (Font bfont = new Font("Antiquity Print", 12 * scaleFactor, FontStyle.Regular))
            using (Font subfont = new Font("Antiquity Print", 16 * scaleFactor, FontStyle.Bold))
            using (Font mainfont = new Font("Antiquity Print", 22 * scaleFactor, FontStyle.Bold))
            using (Font dfont = new Font("Book Antiqua", 14 * scaleFactor, FontStyle.Bold))
            using (Font infofont = new Font("Antiquity Print", 18 * scaleFactor, FontStyle.Regular))
            {
                Color textColor = Color.FromArgb((int)(butop * 255), Color.White);

                g.DrawString(question.Text, mainfont, ibrush, question.Location);
                g.DrawString(ask.Text, infofont, ibrush, ask.Location);
                g.DrawString(kilos.Text, subfont, brush, kilos.Location);
                g.DrawString(unit.Text, subfont, brush, unit.Location);
                g.DrawString(itemnm.Text, mainfont, mbrush, itemnm.Location);
                g.DrawString(price.Text, subfont, Brushes.ForestGreen, price.Location);

                RectangleF d = new RectangleF(
                    1110 * scaleX,
                    380 * scaleY,
                    750 * scaleX,
                    250 * scaleY
                );
                g.DrawString(desc.Text, dfont, dbrush, d);

                Brush bbrush;
                if (backhov == true)
                {
                    bbrush = Brushes.Brown;
                }
                else
                {
                    bbrush = Brushes.Black;
                }
                g.DrawString("<< Go back", bfont, bbrush, ScalePoint(new Point(50, 1010)));
            }

            if (!x)
            {
                foreach (var btn in menuButtons)
                {
                    btn.Visible = false;
                    Rectangle r = new Rectangle(btn.Left, btn.Top, btn.Width, btn.Height);
                    transitioneffects.drawbutfade(g, btn.Image, btn.Left, btn.Top, butop, r);
                }
            }
            else
            {
                foreach (var btn in menuButtons)
                {
                    btn.Visible = true;
                }
            }
        }
    }
}