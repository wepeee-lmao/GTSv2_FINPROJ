using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GTSv2_FINPROJ
{
    public partial class analytics : Form
    {
        private analytics_mode cmode;
        Image bg;

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;

        private DataTable quizdata;
        private string lastplayed = "";

        public enum analytics_mode { admin = 1, player = 2 }

        bool change = true, backhov = false, signhov = false, refhov = false;
        int color = 0;

        int cpage = 0; 
        bool lefthov = false, righthov = false;
        public analytics(analytics_mode mode)
        {
            InitializeComponent();
            this.ResizeRedraw = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.cmode = mode;
            setupform();
        }

        private void setupform()
        {
            bg = Image.FromFile("abg.png");

            grid.BackgroundColor = Color.FromArgb(240, 225, 195);
            grid.BorderStyle = BorderStyle.None;
            grid.DefaultCellStyle.Font = new Font("Book Antiqua", 10);
            grid.DefaultCellStyle.BackColor = Color.FromArgb(254, 244, 220);
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(60, 30, 20);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(180, 140, 80);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Antiquity Print", 10, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(100, 60, 20);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(254, 219, 112);
            grid.ColumnHeadersHeight = 35;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            feedbackgrid.BackgroundColor = Color.FromArgb(240, 225, 195);
            feedbackgrid.BorderStyle = BorderStyle.None;
            feedbackgrid.DefaultCellStyle.Font = new Font("Book Antiqua", 10);
            feedbackgrid.DefaultCellStyle.BackColor = Color.FromArgb(254, 244, 220);
            feedbackgrid.DefaultCellStyle.ForeColor = Color.FromArgb(60, 30, 20);
            feedbackgrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(180, 140, 80);
            feedbackgrid.DefaultCellStyle.SelectionForeColor = Color.White;
            feedbackgrid.ColumnHeadersDefaultCellStyle.Font = new Font("Antiquity Print", 10, FontStyle.Bold);
            feedbackgrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(100, 60, 20);
            feedbackgrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(254, 219, 112);
            feedbackgrid.ColumnHeadersHeight = 35;
            feedbackgrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            feedbackgrid.ReadOnly = true;
            feedbackgrid.AllowUserToAddRows = false;
            feedbackgrid.RowHeadersVisible = false;
            feedbackgrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            feedbackgrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            feedbackgrid.Visible = false; 

            timer1.Interval = 20;
            timer1.Start();
        }

        private void analytics_Load(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && this.ClientSize.Width < bg.Width)
                this.ClientSize = new Size(bg.Width, bg.Height);

            lock (bg)
            {
                originalSize = new Size(bg.Width, bg.Height);
            }
            scaleX = (float)this.ClientSize.Width / originalSize.Width;
            scaleY = (float)this.ClientSize.Height / originalSize.Height;

            this.Resize += (s, ev) =>
            {
                scaleX = (float)this.ClientSize.Width / originalSize.Width;
                scaleY = (float)this.ClientSize.Height / originalSize.Height;
                UpdateGridPosition();
                this.Invalidate();
            };

            if (this.WindowState == FormWindowState.Normal)
            {
                this.CenterToScreen();
            }

            if (playdata.currentid > 0)
            {
                dbconnect.loadprogress(playdata.accountid);
            }
            UpdateGridPosition();
            loaddata();
            this.Invalidate();
        }

        private void UpdateGridPosition()
        {
            if (cmode == analytics_mode.admin)
            {
                Rectangle gridarea = new Rectangle(
                    (int)(50 * scaleX), (int)(250 * scaleY),
                    (int)((bg.Width - 100) * scaleX),
                    (int)((bg.Height - 300) * scaleY));

                grid.Location = gridarea.Location;
                grid.Size = gridarea.Size;
                feedbackgrid.Location = gridarea.Location;
                feedbackgrid.Size = gridarea.Size;

                grid.Visible = (cpage == 0);
                feedbackgrid.Visible = (cpage == 1);
            }
            else
            {
                grid.Location = new Point((int)(80 * scaleX), (int)(280 * scaleY));
                grid.Size = new Size((int)((bg.Width - 160) * scaleX), (int)((bg.Height - 380) * scaleY));
            }

            float sf = Math.Min(scaleX, scaleY);
            grid.DefaultCellStyle.Font = new Font("Book Antiqua", Math.Max(7, 10 * sf));
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Antiquity Print", Math.Max(7, 10 * sf), FontStyle.Bold);
            feedbackgrid.DefaultCellStyle.Font = new Font("Book Antiqua", Math.Max(7, 10 * sf));
            feedbackgrid.ColumnHeadersDefaultCellStyle.Font = new Font("Antiquity Print", Math.Max(7, 10 * sf), FontStyle.Bold);
        }

        private void loaddata()
        {
            if (cmode == analytics_mode.admin)
                loadadmindata();
            else
            {
                loadplayerdata();
            }
        }

        private void loadadmindata()
        {
            DataTable raw = dbconnect.getallplayers();

            DataTable display = new DataTable();
            display.Columns.Add("Player");
            display.Columns.Add("Email");
            display.Columns.Add("Silver (Reales)");
            display.Columns.Add("Reputation");
            display.Columns.Add("Voyages");
            display.Columns.Add("% Done");
            display.Columns.Add("Cargo (kg)");
            display.Columns.Add("Boletas");
            display.Columns.Add("Last Played");
            display.Columns.Add("China");
            display.Columns.Add("Malacca");
            display.Columns.Add("Seville");
            display.Columns.Add("Lisbon");
            display.Columns.Add("Acapulco");
            display.Columns.Add("Cartagena");
            display.Columns.Add("Manila");

            var players = new Dictionary<string, DataRow>();

            foreach (DataRow row in raw.Rows)
            {
                string name = row["playername"].ToString();

                if (!players.ContainsKey(name))
                {
                    int compvoy = Convert.ToInt32(row["compvoy"]);
                    float pct = Math.Min(100f, (compvoy / 6f) * 100f);

                    string lp;

                    if (row["lastplayed"] != DBNull.Value)
                    {
                        DateTime lastPlayedDate = Convert.ToDateTime(row["lastplayed"]);
                        lp = lastPlayedDate.ToString("MMM dd, yyyy  hh:mm tt");
                    }
                    else
                    {
                        lp = "Never";
                    }

                    DataRow nr = display.NewRow();
                    nr["Player"] = name;
                    nr["Email"] = row["email"].ToString();
                    nr["Silver (Reales)"] = row["silver"].ToString();
                    nr["Reputation"] = row["reputation"].ToString();
                    nr["Voyages"] = compvoy + " / 6";
                    nr["% Done"] = pct.ToString("0") + "%";
                    nr["Cargo (kg)"] = row["kilos"].ToString();
                    nr["Boletas"] = row["boletas"].ToString();
                    nr["Last Played"] = lp;
                    nr["China"] = "-";
                    nr["Malacca"] = "-";
                    nr["Seville"] = "-";
                    nr["Lisbon"] = "-";
                    nr["Acapulco"] = "-";
                    nr["Cartagena"] = "-";
                    nr["Manila"] = "-";
                    players[name] = nr;
                    display.Rows.Add(nr);
                }

                if (row["portID"] != DBNull.Value)
                {
                    int portid = Convert.ToInt32(row["portID"]);
                    string scoretext = $"{row["reputitle"]} ({row["repuscore"]}/6)";
                    string[] portcols = { "", "China", "Malacca", "Seville", "Lisbon", "Acapulco", "Cartagena", "Manila" };
                    if (portid >= 1 && portid <= 7)
                    {
                        players[name][portcols[portid]] = scoretext;
                    }
                }

                DataTable fb = dbconnect.getallfeedback();
                DataTable fbdisplay = new DataTable();
                fbdisplay.Columns.Add("Player");
                fbdisplay.Columns.Add("Email");
                fbdisplay.Columns.Add("Rating");
                fbdisplay.Columns.Add("Feedback");
                fbdisplay.Columns.Add("Date Submitted");

                foreach (DataRow r in fb.Rows)
                {
                    DataRow nr = fbdisplay.NewRow();
                    nr["Player"] = r["playername"].ToString();
                    nr["Email"] = r["email"].ToString();
                    nr["Rating"] = new string('★', Convert.ToInt32(r["rating"])) +
                                           new string('☆', 5 - Convert.ToInt32(r["rating"]));
                    nr["Feedback"] = r["feedback"].ToString();
                    if (r["datesubmitted"] != DBNull.Value)
                    {
                        DateTime dateValue = Convert.ToDateTime(r["datesubmitted"]);
                        nr["Date Submitted"] = dateValue.ToString("MMM dd, yyyy  hh:mm tt");
                    }
                    else
                    {
                        nr["Date Submitted"] = "-";
                    }
                    fbdisplay.Rows.Add(nr);
                }
                feedbackgrid.DataSource = fbdisplay;
            }

            grid.DataSource = display;

            grid.CellFormatting += (s, ev) =>
            {
                if (ev.RowIndex < 0) return;
                if (grid.Columns[ev.ColumnIndex].Name == "% Done" && ev.Value != null)
                {
                    string val = ev.Value.ToString().Replace("%", "").Trim();
                    if (float.TryParse(val, out float pct))
                    {
                        if (pct >= 100f)
                        {
                            ev.CellStyle.ForeColor = Color.ForestGreen;
                        }
                        else if (pct >= 50f)
                        {
                            ev.CellStyle.ForeColor = Color.DarkOrange;
                        }
                        else
                        {
                            ev.CellStyle.ForeColor = Color.Firebrick;
                        }
                    }
                }
            };
        }

        private void loadplayerdata()
        {
            quizdata = dbconnect.getplayerquiz(playdata.currentid);

            DataTable display = new DataTable();
            display.Columns.Add("Port");
            display.Columns.Add("Quiz Score");
            display.Columns.Add("Reputation");

            foreach (DataRow row in quizdata.Rows)
            {
                DataRow nr = display.NewRow();
                nr["Port"] = row["portname"].ToString();
                nr["Quiz Score"] = row["repuscore"].ToString() + " / 6";
                nr["Reputation"] = row["reputitle"].ToString();
                display.Rows.Add(nr);
            }

            grid.DataSource = display;

            grid.CellFormatting += (s, ev) =>
            {
                if (ev.RowIndex < 0) return;
                if (grid.Columns[ev.ColumnIndex].Name == "Reputation" && ev.Value != null)
                {
                    switch (ev.Value.ToString())
                    {
                        case "Perito":
                            ev.CellStyle.ForeColor = Color.ForestGreen;
                            break;
                        case "Mercachifle":
                            ev.CellStyle.ForeColor = Color.DarkOrange;
                            break;
                        case "Zopenco":
                            ev.CellStyle.ForeColor = Color.Firebrick;
                            break;
                    }
                }
            };
        }
        private void timer1_Tick(object sender, EventArgs e)
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
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            lock (bg)
            {
                g.DrawImage(bg, this.ClientRectangle);
            }

            float sf = Math.Min(scaleX, scaleY);

            using (Font title = new Font("Antiquity Print", 20 * sf, FontStyle.Bold))
            using (Font sub1 = new Font("Antiquity Print", 12 * sf, FontStyle.Regular))
            using (Font sub = new Font("Antiquity Print", 10* sf, FontStyle.Regular))
            using (Font stat = new Font("Antiquity Print", 13 * sf, FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(color, 60, 140)))
            using (SolidBrush gold = new SolidBrush(Color.FromArgb(254, 219, 120)))
            using (SolidBrush brown = new SolidBrush(Color.FromArgb(60, 30, 20)))
            using (SolidBrush green = new SolidBrush(Color.ForestGreen))
            {
                if (cmode == analytics_mode.admin)
                {
                    g.DrawString("Logged in as: " + playdata.email, sub1, brown,new PointF(60 * scaleX, 180 * scaleY));
                    g.DrawString("Total players: " + (grid.Rows.Count), sub1, brown, new PointF(600 * scaleX, 180 * scaleY));
                   
                    Brush sbrush, rbrush;
                    if (signhov == true && refhov == false)
                    {
                        sbrush = Brushes.Red;
                        rbrush = Brushes.DarkBlue;
                    }
                    else if(signhov == false && refhov == true)
                    {
                        sbrush = Brushes.DarkBlue;
                        rbrush = Brushes.Red;
                    }
                    else
                    {
                        sbrush = Brushes.DarkBlue;
                        rbrush = Brushes.DarkBlue;
                    }
                    g.DrawString(">> Sign Out", sub1, sbrush, new PointF((bg.Width - 220) * scaleX, 120 * scaleY));
                    g.DrawString("↻ Refresh", sub1, rbrush, new PointF((bg.Width - 220) * scaleX, 170 * scaleY));

                    string[] pagetitles = { "Admin — Player Analytics", "Ratings & Feedback" };
                    g.DrawString(pagetitles[cpage], title, brush, new PointF(60 * scaleX, 70 * scaleY));

                    g.DrawString($"Page {cpage + 1} of 2", sub, brown,new PointF((bg.Width - 220) * scaleX, 30 * scaleY));

                    Brush lbrush;
                    if (lefthov && cpage > 0)
                    {
                        lbrush = Brushes.Red;
                    }
                    else
                    {
                        lbrush = Brushes.DarkBlue;
                    }

                    Brush rbrush2;
                    if (righthov && cpage < 1)
                    {
                        rbrush2 = Brushes.Red;
                    }
                    else
                    {
                        rbrush2 = Brushes.DarkBlue;
                    }

                    if (cpage > 0)
                    {
                        g.DrawString("<< Prev", sub1, lbrush, new PointF(1100 * scaleX, 180 * scaleY));
                    }
                    if (cpage < 1)
                    {
                        g.DrawString(">> Next", sub1, rbrush2, new PointF(1300 * scaleX, 180 * scaleY));
                    }
                }
                else
                {
                    float pct = Math.Min(100f, (playdata.compvoy / 6f) * 100f);
                    string pctstr = pct.ToString("0") + "%";

                    g.DrawString("Captain's Analytics", title, brush, new PointF(1320 * scaleX, 60 * scaleY));
                    g.DrawString("Captain: " + playdata.name, sub, brown, new PointF(80 * scaleX, 60 * scaleY));

                    g.DrawString("Last played: " + playdata.lastplayed, sub, brown, new PointF(570 * scaleX, 60 * scaleY));

                    int bx = 80, by = 130, bw = 220, bh = 80, gap = 20;
                    string[] labels = { "Silver", "Voyages", "Progress", "Cargo", "Boletas" };
                    string[] values = {
                        playdata.silver + " Reales",
                        playdata.compvoy + " / 6",
                        pctstr,
                        playdata.curkilos + " kg",
                        playdata.boletas.ToString()
                    };

                    for (int i = 0; i < labels.Length; i++)
                    {
                        Rectangle box = new Rectangle(
                            (int)((bx + i * (bw + gap)) * scaleX),
                            (int)(by * scaleY),
                            (int)(bw * scaleX),
                            (int)(bh * scaleY));

                        using (SolidBrush boxbg = new SolidBrush(Color.FromArgb(180, 100, 60, 20)))
                            g.FillRectangle(boxbg, box);

                        g.DrawString(labels[i], sub, gold,
                            new PointF(box.X + 8, box.Y + 6));

                        SolidBrush bbrush;
                        if (labels[i] == "Progress")
                        {
                            bbrush = green;
                        }
                        else
                        {
                            bbrush = brown;
                        }
                        g.DrawString(values[i], stat, bbrush, new PointF(box.X + 8, box.Y + 35));

                    }

                    int barx = (int)(80 * scaleX);
                    int bary = (int)(240 * scaleY);
                    int barw = (int)((bg.Width - 160) * scaleX);
                    int barh = (int)(30 * scaleY);

                    using (SolidBrush barbg = new SolidBrush(Color.FromArgb(100, 60, 20)))
                    { 
                        g.FillRectangle(barbg, barx, bary, barw, barh);
                    }

                    using (SolidBrush barfill = new SolidBrush(Color.ForestGreen))
                    {
                        g.FillRectangle(barfill, barx, bary, (int)(barw * pct / 100f), barh);
                    }

                    g.DrawString(pctstr + " complete", sub, gold, new PointF(barx + barw / 2 - 40, bary - 5 * scaleY));
                    
                    Brush nbrush;
                    if (backhov == true)
                    {
                        nbrush = Brushes.Red;
                    }
                    else
                    {
                        nbrush = Brushes.DarkBlue;
                    }

                    g.DrawString("<< Back", sub, nbrush, new PointF(100 * scaleX, (bg.Height - 80) * scaleY));
                }
            }
        }

        private void analytics_MouseClick(object sender, MouseEventArgs e)
        {
            float sf = Math.Min(scaleX, scaleY);

            if (cmode == analytics_mode.admin)
            {
                Rectangle prev = new Rectangle((int)(1100 * scaleX),(int)(180 * scaleY), (int)(120 * scaleX), (int)(35 * scaleY));
                Rectangle next = new Rectangle((int)(1300 * scaleX), (int)(180 * scaleY), (int)(120 * scaleX), (int)(35 * scaleY));

                if (prev.Contains(e.Location) && cpage > 0)
                {
                    cpage--;
                    UpdateGridPosition();
                    this.Invalidate();
                    return;
                }
                if (next.Contains(e.Location) && cpage < 1)
                {
                    cpage++;
                    UpdateGridPosition();
                    this.Invalidate();
                    return;
                }
            }
            else
            {
                Rectangle back = new Rectangle(
                    (int)(100 * scaleX),
                    (int)((bg.Height - 90) * scaleY),
                    (int)(150 * scaleX),
                    (int)(40 * scaleY));

                if (back.Contains(e.Location))
                {
                    nav.remember(this);
                    this.Close();
                    var n = new Intro();
                    nav.apply(n);
                    nav.go(n);
                    return;
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (bg != null)
            {
                bg.Dispose();
            }
            bg = null;
        }

        private void analytics_MouseMove(object sender, MouseEventArgs e)
        {
           Rectangle r = new Rectangle(
                    (int)(100 * scaleX),
                    (int)((bg.Height - 90) * scaleY),
                    (int)(150 * scaleX),
                    (int)(40 * scaleY));
            if (r.Contains(e.Location))
            {
                backhov = true;
            }
            else if (backhov)
            {
                backhov = false;
            }

            Rectangle signout = new Rectangle(
                     (int)((bg.Width - 220) * scaleX),
                     (int)(120 * scaleY),
                     (int)(180 * scaleX),
                     (int)(35 * scaleY));

            if(signout.Contains(e.Location))
            {
                signhov = true;
            }
            else if (signhov)
            {
                signhov = false;
            }

            Rectangle refresh = new Rectangle(
                    (int)((bg.Width - 220) * scaleX),
                    (int)(170 * scaleY),
                    (int)(180 * scaleX),
                    (int)(35 * scaleY));
            if (refresh.Contains(e.Location))
            {
                refhov = true;
            }
            else if (refhov)
            {
                refhov = false;
            }
            Rectangle prev = new Rectangle((int)(1100 * scaleX), (int)(180 * scaleY), (int)(120 * scaleX), (int)(35 * scaleY));
            Rectangle next = new Rectangle((int)(1300 * scaleX), (int)(180 * scaleY), (int)(120 * scaleX), (int)(35 * scaleY));
            lefthov = prev.Contains(e.Location) && cpage > 0;
            righthov = next.Contains(e.Location) && cpage < 1;
            this.Invalidate();
        }
    }
}