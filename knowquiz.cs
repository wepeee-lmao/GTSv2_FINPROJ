using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTSv2_FINPROJ
{
    public partial class knowquiz : Form
    {
        private int portid;
        private int currentq = 0;
        private int score = 0;
        private bool answered = false;
        private int selch = 0;

        int color = 0, giftime = 0, wait = 40;

        private List<QuizQuestion> questions = new List<QuizQuestion>();
        private List<string> portnames;

        float transp0 = 0.0f;
        bool close = false, change = true, resstart = false;
        Image bg, bg2, bg3;
        private kmode cmode = 0;

        System.Windows.Forms.Timer gtimer = new System.Windows.Forms.Timer();

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;

        private bool originalSet = false;
        public knowquiz(int id, kmode cmode)
        {
            InitializeComponent();

            this.ResizeRedraw = true;
            this.portid = id;
            this.cmode = cmode;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            bg = Image.FromFile("knowquiz.png");
            bg2 = Image.FromFile("quizres1.gif");
            bg3 = Image.FromFile("quizres.png");
            if (ImageAnimator.CanAnimate(bg2))
            {
                ImageAnimator.Animate(bg2, (s, a) => this.Invalidate());
            }
            portnames = dbconnect.getportnames();
            buildquestions();

            gtimer.Interval = 30;
            gtimer.Tick += gtimer_Tick;
            gtimer.Start();
        }

        private void knowquiz_Load(object sender, EventArgs e)
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
        private void nventunit_Resize(object sender, EventArgs e)
        {
            if (originalSize.Width == 0 || originalSize.Height == 0)
            {
                return;
            }
            if (!originalSet)
            {
                originalSet = true;
                originalSize = new Size(bg.Width, bg.Height);
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

      
        private RectangleF ScaleRectangleF(RectangleF original)
        {
            return new RectangleF(
                original.X * scaleX,
                original.Y * scaleY,
                original.Width * scaleX,
                original.Height * scaleY
            );
        }
        private void buildquestions()
        {
            var facts = dbconnect.getquizfacts(portid, 3);
            var wrongpool = dbconnect.getwrongfacts(portid, 9);
            var rng = new Random();

            for (int i = 0; i < facts.Count; i++)
            {
                string correct = facts[i].fact;
                var choices = wrongpool.Skip(i * 3).Take(3).ToList();
                choices.Add(correct);
                int n = choices.Count;
                while (n > 1)
                {
                    n--;

                    int k = rng.Next(n + 1);

                    string value = choices[k];
                    choices[k] = choices[n];
                    choices[n] = value;
                }

                int coridx = choices.IndexOf(correct);

                questions.Add(new QuizQuestion
                {
                    quest = "Which of the following is true about this port?",
                    choices = choices,
                    coridx = coridx,
                    truefalse = false
                });
            }

            var tffacts = dbconnect.getquizfacts(portid, 3);
            foreach (var factObj in tffacts)
            {
                string fact = factObj.fact;
                bool makefalse = rng.Next(2) == 0;
                string display = fact;
                bool isTrue = true;

                if (makefalse)
                {
                    string swapped = swapportname(fact, portid);
                    if (swapped != fact)
                    {
                        display = swapped;
                        isTrue = false;
                    }
                }

                int fincor = 1;
                if (isTrue == true)
                {
                    fincor = 0;
                }

                questions.Add(new QuizQuestion
                {
                    quest = display,
                    choices = new List<string> { "True", "False" },
                    coridx = fincor,
                    truefalse = true
                });
            }
        }

        private string swapportname(string fact, int cpid)
        {
            var rng = new Random();
            string curport = portnames[cpid - 1];

            int idx = fact.IndexOf(curport, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                var others = portnames.Where(p => !p.Equals(curport, StringComparison.OrdinalIgnoreCase)).ToList();
                string replace = others[rng.Next(others.Count)];
                replace = char.ToUpper(replace[0]) + replace.Substring(1);
                return fact.Substring(0, idx) + replace + fact.Substring(idx + curport.Length);
            }

            foreach (string pname in portnames.OrderBy(_ => rng.Next()))
            {
                int fidx = fact.IndexOf(pname, StringComparison.OrdinalIgnoreCase);
                if (fidx >= 0)
                {
                    var others = portnames.Where(p => !p.Equals(pname, StringComparison.OrdinalIgnoreCase)).ToList();
                    string replace = others[rng.Next(others.Count)];
                    replace = char.ToUpper(replace[0]) + replace.Substring(1);
                    return fact.Substring(0, fidx) + replace + fact.Substring(fidx + pname.Length);
                }
            }

            return fact;
        }

        private void gtimer_Tick(object sender, EventArgs e)
        {
            if (close == false)
            {
                transp0 = Math.Min(1.0f, transp0 + 0.09f);
            }
            else
            {
                transp0 = Math.Max(0.0f, transp0 - 0.09f);
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

            if (currentq >= questions.Count)
            {
                if (!resstart)
                {
                    resstart = true;
                    giftime = 0;
                }

                if (giftime < wait)
                {
                    giftime++;
                }
            }

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            float scaleFactor = Math.Min(scaleX, scaleY);

            if (bg == null)
            {
                return;
            }
            if (currentq >= questions.Count)
            {
                if (bg2 != null && bg3 != null)
                {
                    if (giftime < wait)
                    {
                        lock (bg2)
                        {
                            ImageAnimator.UpdateFrames(bg2);
                            g.DrawImage(bg2, this.ClientRectangle);
                        }
                    }
                    else
                    {
                        lock (bg3)
                        {
                            g.DrawImage(bg3, this.ClientRectangle);
                            drawresults(g);
                        }
                    }
                    return;
                }
            }
            else
            {
                if (bg != null)
                {
                    lock (bg)
                    {
                        transitioneffects.drawfade(g, bg, transp0, this.ClientRectangle);
                    }
                }
            }

            var q = questions[currentq];

            using (Font qfont = new Font("Antiquity Print", (int)(14 * scaleFactor), FontStyle.Bold))
            using (Font cfont = new Font("Book Antiqua", (int)(14 * scaleFactor), FontStyle.Bold))
            using (Font cfont2 = new Font("Antiquity Print", (int)(14 * scaleFactor), FontStyle.Regular))
            using (Font hdfont = new Font("Antiquity Print", (int)(18 * scaleFactor), FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(color, 60, 100)))
            using (SolidBrush white = new SolidBrush(Color.FromArgb(60, 40, 20)))
            {
                string header = "";
                if (q.truefalse == true)
                {
                    header = "Question " + (currentq + 1) + " — True or False?";
                }
                else
                {
                    header = "Question " + (currentq + 1) + " — Multiple Choice";
                }
                g.DrawString(header, hdfont, brush, ScalePoint(new Point(200, 150)));

                RectangleF qbox = ScaleRectangleF(new RectangleF(200, 230, bg.Width - 400, 200));
                g.DrawString(q.quest, qfont, white, qbox);

                string[] labels = { "A)", "B)", "C)", "D)" };
                int ystart = 300;
                for (int i = 0; i < q.choices.Count; i++)
                {
                    Brush choicebrush = white;

                    if (answered == false && i == selch)
                    {
                        choicebrush = Brushes.DarkOliveGreen;
                    }

                    if (answered == true)
                    {
                        if (i == q.coridx)
                        {
                            choicebrush = Brushes.ForestGreen;
                        }
                        else if (i == lastchose && lastchose != q.coridx)
                        {
                            choicebrush = Brushes.Red;
                        }
                    }

                    string label = "";
                    if (q.truefalse == false)
                    {
                        label = labels[i] + " ";
                    }
                   
                    if (answered == false && i == selch)
                    {
                        if (q.truefalse == true)
                        {
                            g.DrawString("➤", cfont2, Brushes.DarkOliveGreen, ScalePoint(new Point(160, ystart+80)));
                        }
                        else
                        {
                            g.DrawString("➤", cfont, Brushes.DarkOliveGreen, ScalePoint(new Point(160, ystart)));
                        }
                    }

                    RectangleF cbox;
                    if (q.truefalse == true)
                    {
                        cbox = ScaleRectangleF(new RectangleF(200, ystart+80, bg.Width - 500, 100));
                        g.DrawString(label + q.choices[i], cfont2, choicebrush, cbox);
                        ystart += 100;
                    }
                    else
                    {
                        cbox = ScaleRectangleF(new RectangleF(200, ystart, bg.Width - 500, 100));
                        g.DrawString(label + q.choices[i], cfont, choicebrush, cbox);
                        ystart += 130;
                    }
                }

                if (answered == true)
                {
                    string fb = "Wrong!";
                    Brush fbb = Brushes.Red;

                    if (lastchose == q.coridx)
                    {
                        fb = "Correct!";
                        fbb = Brushes.ForestGreen;
                    }

                    g.DrawString(fb, hdfont, fbb, ScalePoint(new Point(200, 850)));
                    g.DrawString("Press ENTER to continue...", cfont, white, ScalePoint(new Point(200, 910)));
                }
                else
                {
                    g.DrawString("Use ↑↓ to select, ENTER to confirm", cfont, Brushes.SaddleBrown, ScalePoint(new Point(200, 910)));
                }

                g.DrawString("Score: " + score + "/" + currentq, cfont, brush, ScalePoint(new Point(bg.Width - 300, 150)));
            }
        }

        private int lastchose = -1;

        private void drawresults(Graphics g)
        {
            int total = questions.Count;
            float pct = (float)score / total * 100f;
            string title = gettitle(pct);
            float scaleFactor = Math.Min(scaleX, scaleY);

            using (Font big = new Font("Antiquity Print", (int)(22 * scaleFactor), FontStyle.Bold))
            using (Font med = new Font("Antiquity Print", (int)(14 * scaleFactor), FontStyle.Regular))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(color, 0, 50)))
            using (SolidBrush brown = new SolidBrush(Color.DarkOliveGreen))
            {
                g.DrawString("Quiz Complete!", big, brush, ScalePoint(new Point(400, 230)));
                g.DrawString("You scored " + score + " out of " + total + " (" + pct.ToString("0") + "%)", med, brown, ScalePoint(new Point(400, 350)));
                g.DrawString("Reputation at this port: ", med, brown, ScalePoint(new Point(400, 460)));
                g.DrawString(title, med, brush, ScalePoint(new Point(850, 460)));

                string effect = "";
                if (title == "Perito")
                {
                    effect = "You get 20% discount when buying and 20% bonus when selling here!";
                }
                else if (title == "Mercachifle")
                {
                    effect = "You get 15% discount when buying and 15% bonus when selling here!";
                }
                else
                {
                    effect = "Merchants here don't trust you. Prices are 20% worse when buying and selling.";
                }

                RectangleF ebox = ScaleRectangleF(new RectangleF(400, 570, bg.Width - 900, 150));
                g.DrawString(effect, med, Brushes.DarkOliveGreen, ebox);
                g.DrawString("Press ENTER to return.", med, Brushes.SaddleBrown, ScalePoint(new Point(400, 760)));
            }

            dbconnect.saverepu(playdata.currentid, portid, score, title);
            playdata.repu = title;
        }

        private string gettitle(float pct)
        {
            if (pct >= 90f)
            {
                return "Perito";
            }
            if (pct >= 50f)
            {
                return "Mercachifle";
            }
            return "Zopenco";
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (currentq >= questions.Count)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gtimer.Stop();
                    nav.remember(this); 
                    this.Close();
                    var n = new knowbuild(portid);
                    nav.apply(n);     
                    nav.go(n);
                }
                return;
            }

            var q = questions[currentq];

            if (answered == true)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    answered = false;
                    lastchose = -1;
                    currentq++;
                    selch = 0; 
                    this.Invalidate();
                }
                return;
            }

            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
            {
                selch--;
                if (selch < 0)
                {
                    selch = q.choices.Count - 1; 
                }
                this.Invalidate();
            }
            else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
            {
                selch++;
                if (selch >= q.choices.Count)
                {
                    selch = 0; 
                }
                this.Invalidate();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                lastchose = selch;
                answered = true;
                if (selch == q.coridx)
                {
                    score++;
                }
                this.Invalidate();
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

            if (bg2 != null)
            {
                bg2.Dispose();
                bg2 = null;
            }
        }
    }

    public class QuizQuestion
    {
        public string quest { get; set; }
        public List<string> choices { get; set; }
        public int coridx { get; set; }
        public bool truefalse { get; set; }
    }
}