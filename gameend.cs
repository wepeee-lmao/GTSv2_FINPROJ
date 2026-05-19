using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GTSv2_FINPROJ
{
    public partial class gameend : Form
    {
        private int finalscore;
        private int totalq;
        private bool won;
        private bool feedbacksent = false;
        private int selectedrating = 0;

        Image bg;
        float transp0 = 0.0f;
        float scaleX = 1.0f, scaleY = 1.0f;
        Size originalSize;

        private int hoveredstar = 0;

        int color = 0;
        bool change = true;

        public gameend(bool won, int score, int total)
        {
            InitializeComponent();
            this.ResizeRedraw = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.won = won;
            this.finalscore = score;
            this.totalq = total;

            bg = Image.FromFile("gameend.png");

            feedbox.Multiline = true;
            feedbox.ScrollBars = ScrollBars.Vertical;
            feedbox.Font = new Font("Book Antiqua", 12);
            feedbox.BackColor = Color.FromArgb(254, 244, 220);
            feedbox.ForeColor = Color.FromArgb(60, 30, 20);
            feedbox.BorderStyle = BorderStyle.FixedSingle;
            feedbox.Text = "Write your feedback here...";
            feedbox.ForeColor = Color.Gray;
            feedbox.Enter += (s, e) =>
            {
                if (feedbox.Text == "Write your feedback here...")
                {
                    feedbox.Text = "";
                    feedbox.ForeColor = Color.FromArgb(60, 30, 20);
                }
            };
            feedbox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(feedbox.Text))
                {
                    feedbox.Text = "Write your feedback here...";
                    feedbox.ForeColor = Color.Gray;
                }
            };

            gtimer.Interval = 30;
            gtimer.Tick += gtimer_Tick;
            gtimer.Start();
        }

        private void gameend_Load(object sender, EventArgs e)
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

            this.Resize += (s, ev) =>
            {
                scaleX = (float)this.ClientSize.Width / originalSize.Width;
                scaleY = (float)this.ClientSize.Height / originalSize.Height;
                UpdateControlPositions();
                this.Invalidate();
            };

            if (this.WindowState == FormWindowState.Normal)
            {
                this.CenterToScreen();
            }

            UpdateControlPositions();
        }
        private void gtimer_Tick(object sender, EventArgs e)
        {
            transp0 = Math.Min(1.0f, transp0 + 0.05f);

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
        private void UpdateControlPositions()
        {
            feedbox.Location = new Point((int)(200 * scaleX), (int)(600 * scaleY));
            feedbox.Size = new Size((int)(1100 * scaleX), (int)(120 * scaleY));
            feedbox.Font = new Font("Book Antiqua", Math.Max(8, 12 * Math.Min(scaleX, scaleY)));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            float sf = Math.Min(scaleX, scaleY);

            transitioneffects.drawfade(g, bg, transp0, this.ClientRectangle);

            using (SolidBrush pulse = new SolidBrush(Color.FromArgb(color, 0, 25)))
            using (Font title = new Font("Antiquity Print", Math.Max(10, (int)(24 * sf)), FontStyle.Bold))
            using (Font sub = new Font("Antiquity Print", Math.Max(8, (int)(16 * sf)), FontStyle.Bold))
            using (Font body = new Font("Book Antiqua", Math.Max(8, (int)(13 * sf)), FontStyle.Regular))
            using (Font hint = new Font("Antiquity Print", Math.Max(7, (int)(11 * sf)), FontStyle.Regular))
            using (SolidBrush gold = new SolidBrush(Color.FromArgb(254, 219, 112)))
            using (SolidBrush brown = new SolidBrush(Color.FromArgb(60, 30, 20)))
            using (SolidBrush green = new SolidBrush(Color.ForestGreen))
            using (SolidBrush red = new SolidBrush(Color.Firebrick))
            {
                string ttext;

                Brush brush;
                if (won)
                {
                    brush = pulse;
                    ttext = "Victory! The Galleons Have Arrived!";
                }
                else
                {
                    brush = red;
                    ttext = "Defeated... The Sea Claims Its Own.";
                }

                g.DrawString(ttext, title, brush, new PointF(600 * scaleX, 850 * scaleY));

                string status = "Status: Completed";
                g.DrawString(status, sub, green, new PointF(200 * scaleX, 140 * scaleY));

                int bx = 200, by = 200, bw = 280, bh = 100, gap = 30;

                string datestr;

                if (string.IsNullOrEmpty(playdata.lastplayed))
                {
                    datestr = DateTime.Now.ToString("MMM dd, yyyy  hh:mm tt");
                }
                else
                {
                    datestr = playdata.lastplayed;
                }

                float pct = Math.Min(100f, (playdata.compvoy / 6f) * 100f);

                string[] slabels = { "Captain", "Date Finished", "Final Score", "Reputation", "Progress" };
                string[] svalues = { playdata.name, datestr, $"{finalscore}/{totalq}", playdata.repu, pct.ToString("0") + "%" };

                Color statusColor;

                if (won)
                {
                    statusColor = Color.ForestGreen;
                }
                else
                {
                    statusColor = Color.Firebrick;
                }

                Color[] scolors = {
                    Color.FromArgb(254, 219, 112),
                    Color.FromArgb(60, 30, 20),
                    statusColor,
                    Color.FromArgb(139, 90, 43),
                    Color.ForestGreen
                };

                for (int i = 0; i < slabels.Length; i++)
                {
                    Rectangle box = new Rectangle(
                        (int)((bx + i * (bw + gap)) * scaleX),
                        (int)(by * scaleY),
                        (int)(bw * scaleX),
                        (int)(bh * scaleY));

                    using (SolidBrush boxbg = new SolidBrush(Color.FromArgb(160, 80, 50, 10)))
                    {
                        g.FillRectangle(boxbg, box);
                    }

                    using (Pen border = new Pen(Color.FromArgb(139, 90, 43), 1))
                    {
                        g.DrawRectangle(border, box);
                    }

                    g.DrawString(slabels[i], hint, brown, new PointF(box.X + 8, box.Y + 6));
                    using (SolidBrush vbrush = new SolidBrush(scolors[i]))
                    {
                        g.DrawString(svalues[i], body, vbrush, new RectangleF(box.X + 8, box.Y + 32, box.Width - 16, box.Height - 38));
                    }
                }
                g.DrawString("Rate your experience:", sub, brown, new PointF(200 * scaleX, 350 * scaleY));

                drawstars(g, sf);

                g.DrawString("Leave a message for the developer:", hint, brown, new PointF(200 * scaleX, 550 * scaleY));

                if (!feedbacksent)
                {
                    Rectangle submitbtn = getsubmitbtn();
                    using (SolidBrush btnbg = new SolidBrush(Color.FromArgb(139, 90, 43)))
                    {
                        g.FillRectangle(btnbg, submitbtn);
                        g.DrawString("Submit Feedback", body, gold, new PointF(submitbtn.X + 10, submitbtn.Y + 8));
                    }
                }
                else
                {
                    g.DrawString("✓ Feedback submitted! Thank you, Captain.", body, green, new PointF(200 * scaleX, 820 * scaleY));
                }

                Rectangle contbtn = getcontbtn();
                using (SolidBrush btnbg = new SolidBrush(Color.FromArgb(80, 50, 10)))
                {
                    g.FillRectangle(btnbg, contbtn);
                    g.DrawString(">> Return to Menu", body, gold, new PointF(contbtn.X + 10, contbtn.Y + 8));
                }
            }
        }

        private void drawstars(Graphics g, float sf)
        {
            int starsize = (int)(40 * sf);
            int starx = (int)(200 * scaleX);
            int stary = (int)(420 * scaleY);
            int gap = (int)(10 * scaleX);

            for (int i = 1; i <= 5; i++)
            {
                Rectangle starbox = new Rectangle(starx + (i - 1) * (starsize + gap), stary, starsize, starsize);
                bool filled = i <= selectedrating || i <= hoveredstar;
                Color starcolor;

                if (filled)
                {
                    starcolor = Color.FromArgb(254, 219, 112);
                }
                else
                {
                    starcolor = Color.FromArgb(120, 100, 60);
                }

                using (SolidBrush sb = new SolidBrush(starcolor))
                {
                    PointF center = new PointF(starbox.X + starbox.Width / 2f, starbox.Y + starbox.Height / 2f);
                    float outer = starbox.Width / 2f;
                    float inner = outer * 0.4f;
                    PointF[] pts = new PointF[10];
                    for (int j = 0; j < 10; j++)
                    {
                        float angle = (float)(Math.PI / 5 * j - Math.PI / 2);
                        float r;
                        if (j % 2 == 0)
                        {
                            r = outer;
                        }
                        else
                        {
                            r = inner;
                        }

                        pts[j] = new PointF(
                            center.X + r * (float)Math.Cos(angle),
                            center.Y + r * (float)Math.Sin(angle));
                    }
                    g.FillPolygon(sb, pts);
                }
            }
            if (selectedrating > 0)
            {
                string[] ratinglabels = { "", "Poor", "Fair", "Good", "Great", "Excellent!" };
                float sf2 = Math.Min(scaleX, scaleY);
                using (Font rlabel = new Font("Antiquity Print", Math.Max(8, (int)(13 * sf2)), FontStyle.Bold))
                using (SolidBrush gold = new SolidBrush(Color.DarkGoldenrod))
                {
                    g.DrawString(ratinglabels[selectedrating], rlabel, gold, new PointF(200 * scaleX, (int)(470 * scaleY)));
                }
            }
        }

        private Rectangle getsubmitbtn() =>
            new Rectangle((int)(200 * scaleX), (int)(820 * scaleY),
                          (int)(280 * scaleX), (int)(45 * scaleY));

        private Rectangle getcontbtn() =>
            new Rectangle((int)(200 * scaleX), (int)(880 * scaleY),
                          (int)(280 * scaleX), (int)(45 * scaleY));

        private int getstarindex(Point loc)
        {
            float sf = Math.Min(scaleX, scaleY);
            int starsize = (int)(40 * sf);
            int starx = (int)(200 * scaleX);
            int stary = (int)(420 * scaleY);
            int gap = (int)(10 * scaleX);

            for (int i = 1; i <= 5; i++)
            {
                Rectangle starbox = new Rectangle(starx + (i - 1) * (starsize + gap), stary, starsize, starsize);
                if (starbox.Contains(loc)) return i;
            }
            return 0;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            int prev = hoveredstar;
            hoveredstar = getstarindex(e.Location);
            if (hoveredstar != prev) this.Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            hoveredstar = 0;
            this.Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            int star = getstarindex(e.Location);
            if (star > 0)
            {
                selectedrating = star;
                this.Invalidate();
                return;
            }

            if (!feedbacksent && getsubmitbtn().Contains(e.Location))
            {
                if (selectedrating == 0)
                {
                    MessageBox.Show("Please select a star rating before submitting.");
                    return;
                }

                string fbtext = feedbox.Text.Trim();
                if (fbtext == "Write your feedback here...")
                {
                    fbtext = "";
                }

                if (dbconnect.savefeedback(playdata.currentid, selectedrating, fbtext))
                {
                    feedbacksent = true;
                    this.Invalidate();
                }
                return;
            }

            if (getcontbtn().Contains(e.Location))
            {
                gtimer.Stop();
                this.Close();
                nav.go(new signpage());
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            gtimer.Stop();

            if (bg != null)
            {
                bg.Dispose();
                bg = null;
            }
        }
    }
}