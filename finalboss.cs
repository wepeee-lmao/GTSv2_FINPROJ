using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace GTSv2_FINPROJ
{
    public partial class finalboss : Form
    {
        private int currentq = 0;
        private int score = 0;
        private bool answered = false;
        private int selch = 0;
        private int lastchose = -1;
        private bool bossSaved = false;

        private int timeLeft = 6;
        private int tickcount = 0;
        private bool timedout = false;

        private bool playerHit = false;
        private bool bossHit = false;
        private int hitTimer = 0;

        private float cballx = 0, cbally = 0;
        private float cballtargetx = 0, cballtargety = 0;
        private bool cballactive = false;
        private bool cballtowardboss = false;

        private List<QuizQuestion> questions = new List<QuizQuestion>();
        private List<string> portnames;

        Image bg, pship, bship, cball;

        System.Windows.Forms.Timer gtimer = new System.Windows.Forms.Timer();

        int color = 0;
        bool change = true;

        private Size originalSize;
        private float scaleX = 1.0f, scaleY = 1.0f;

        private const int PSHIP_X = 30, PSHIP_Y = 300;
        private const int PSHIP_W = 1400, PSHIP_H = 800; 
        private const int BSHIP_X = 500, BSHIP_Y = 300;
        private const int BSHIP_W = 1400, BSHIP_H = 800;
        private const int CBALL_W = 300, CBALL_H = 300;

        private int playerHP = 5;
        private int bossHP = 5;

        public finalboss()
        {
            InitializeComponent();
            this.ResizeRedraw = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            
            bg = Image.FromFile("bgfinal.png");
            pship = Image.FromFile("boat.gif");
            bship = Image.FromFile("boss.gif");
            cball = Image.FromFile("cannonball.gif");
            setupanimate();
            portnames = dbconnect.getportnames();
            buildquestions();

            gtimer.Interval = 30;
            gtimer.Tick += gtimer_Tick;
            gtimer.Start();
        }
        private void finalboss_Load_1(object sender, EventArgs e)
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
                this.Invalidate();
            };

            if (this.WindowState == FormWindowState.Normal)
            {
                this.CenterToScreen();
            }
        }
        private void setupanimate()
        {
            ImageAnimator.Animate(bg, this.OnFrameChanged);
            ImageAnimator.Animate(pship, this.OnFrameChanged);
            ImageAnimator.Animate(bship, this.OnFrameChanged);
            ImageAnimator.Animate(cball, this.OnFrameChanged);
        }

        private void OnFrameChanged(object? sender, EventArgs e)
        {
            if (bg != null)
            {
                lock (bg)
                { 
                    ImageAnimator.UpdateFrames(bg); 
                }
            }
            if (pship != null)
            {
                lock (pship)
                {
                    ImageAnimator.UpdateFrames(pship);
                }
            }
            if (bship != null)
            {
                lock (bship)
                {
                    ImageAnimator.UpdateFrames(bship);
                }
            }
            if (cball != null)
            {
                lock (cball)
                {
                    ImageAnimator.UpdateFrames(cball);
                }
            }
        }


        private Rectangle ScaleRect(int x, int y, int w, int h) =>
            new Rectangle((int)(x * scaleX), (int)(y * scaleY),
                          (int)(w * scaleX), (int)(h * scaleY));

        private void buildquestions()
        {
            var rng = new Random();
            var allquestions = new List<QuizQuestion>();

            List<int> allport = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            List<int> portids = new List<int>();

            for (int i = 0; i < 6; i++)
            {
                int rannidx = rng.Next(allport.Count);

                portids.Add(allport[rannidx]);

                allport.RemoveAt(rannidx);
            }
            foreach (int pid in portids)
            {
                var facts = dbconnect.getquizfacts(pid, 1);
                if (facts.Count == 0) continue;

                string correct = facts[0].fact;
                var wrongpool = dbconnect.getwrongfacts(pid, 3);
                var choices = wrongpool.Take(3).ToList();
                choices.Add(correct);
                for (int i = choices.Count - 1; i > 0; i--)
                {
                    int randidx = rng.Next(i + 1);

                    string temp = choices[i];
                    choices[i] = choices[randidx];
                    choices[randidx] = temp;
                }

                int coridx = -1;
                for (int i = 0; i < choices.Count; i++)
                {
                    if (choices[i] == correct)
                    {
                        coridx = i; 
                        break;
                    }
                }
                string portlabel = portnames[pid - 1]; 
                                                      
                portlabel = char.ToUpper(portlabel[0]) + portlabel.Substring(1);

                allquestions.Add(new QuizQuestion
                {
                    quest = $"Which of the following is true about {portlabel}?",
                    choices = choices,
                    coridx = coridx,
                    truefalse = false
                });
            }

            for (int i = 0; i < 4; i++)
            {
                int randport = rng.Next(1, 8);
                var facts = dbconnect.getquizfacts(randport, 1);
                if (facts.Count == 0) continue;

                string fact = facts[0].fact;
                bool makefalse = rng.Next(2) == 0;
                string display = fact;
                bool isTrue = true;

                if (makefalse)
                {
                    string swapped = swapportname(fact, randport);
                    if (swapped != fact) { display = swapped; isTrue = false; }
                }

                allquestions.Add(new QuizQuestion
                {
                    quest = display,
                    choices = new List<string> { "True", "False" },
                    coridx = isTrue ? 0 : 1,
                    truefalse = true
                });
            }

            questions = allquestions.OrderBy(_ => rng.Next()).ToList();
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

            if (cballactive)
            {
                float speed = 25f;
                float dx = cballtargetx - cballx;
                float dy = cballtargety - cbally;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                if (dist < speed)
                {
                    cballactive = false;
                    cballx = cballtargetx;
                    cbally = cballtargety;
                }
                else
                {
                    cballx += dx / dist * speed;
                    cbally += dy / dist * speed;
                }
            }

            if (hitTimer > 0)
            {
                hitTimer--;
                if (hitTimer == 0)
                {
                    playerHit = false;
                    bossHit = false;
                    cballactive = false;

                    if (playerHP <= 0 || bossHP <= 0 || currentq >= questions.Count)
                    {
                        endgame();
                        return;
                    }

                    answered = false;
                    timedout = false;
                    lastchose = -1;
                    currentq++;
                    timeLeft = 10;
                    tickcount = 0;
                    selch = 0;
                }
                this.Invalidate();
                return;
            }
                
            if (currentq < questions.Count && !answered && !timedout)
            {
                tickcount++;
                if (tickcount >= 33)
                {
                    tickcount = 0;
                    timeLeft--;

                    if (timeLeft <= 0)
                    {
                        timedout = true;
                        playerHP--;
                        playerHit = true;
                        bossHit = false;
                        firecball(towardboss: false);
                        hitTimer = 60;
                    }
                }
            }

            if (currentq >= questions.Count && !bossSaved)
            {
                endgame();
                return;
            }

            this.Invalidate();
        }

        private void firecball(bool towardboss)
        {
            cballtowardboss = towardboss;
            cballactive = true;

            if (towardboss)
            {
                lock (this)
                {
                    cballx = BSHIP_X * scaleX;
                    cbally = (BSHIP_Y + BSHIP_H / 2) * scaleY;
                    cballtargetx = (PSHIP_X + PSHIP_W) * scaleX;
                    cballtargety = (PSHIP_Y + PSHIP_H / 2) * scaleY;

                }
            }
            else
            {
                lock (this)
                {
                    cballx = (PSHIP_X + PSHIP_W) * scaleX;
                    cbally = (PSHIP_Y + PSHIP_H / 2) * scaleY;
                    cballtargetx = BSHIP_X * scaleX;
                    cballtargety = (BSHIP_Y + BSHIP_H / 2) * scaleY;
                }
            }
        }

        private void endgame()
        {
            bossSaved = true;
            gtimer.Stop();
            bool won = bossHP <= 0 || (playerHP > 0 && score > questions.Count / 2);
            this.Close();
            nav.go(new gameend(won, score, questions.Count));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (currentq >= questions.Count || answered || timedout) return;

            var q = questions[currentq];

            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W) { selch = (selch - 1 + q.choices.Count) % q.choices.Count; }
            else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S) { selch = (selch + 1) % q.choices.Count; }
            else if (e.KeyCode == Keys.Enter)
            {
                lastchose = selch;
                answered = true;

                if (selch == q.coridx)
                {
                    score++;
                    bossHP--;
                    bossHit = true;
                    playerHit = false;
                    firecball(towardboss: true);
                }
                else
                {
                    playerHP--;
                    playerHit = true;
                    bossHit = false;
                    firecball(towardboss: false);
                }
                hitTimer = 60;
            }

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            float sf = Math.Min(scaleX, scaleY);

            if (bg != null)
            {
                lock (bg)
                {
                    g.DrawImage(bg, this.ClientRectangle);
                }
            }

            if (pship != null)
            {
                lock (pship)
                {
                    Rectangle pr = ScaleRect(PSHIP_X, PSHIP_Y, PSHIP_W, PSHIP_H);
                    if (playerHit)
                    {
                        ColorMatrix cm = new ColorMatrix(new float[][]
                        {
                        new float[] { 1, 0, 0, 0, 0 },
                        new float[] { 0, 0, 0, 0, 0 },
                        new float[] { 0, 0, 0, 0, 0 },
                        new float[] { 0, 0, 0, 1, 0 },
                        new float[] { 0.5f, 0, 0, 0, 1 }
                        });
                        using (ImageAttributes ia = new ImageAttributes())
                        {
                            ia.SetColorMatrix(cm);
                            g.DrawImage(pship, pr, 0, 0, pship.Width, pship.Height, GraphicsUnit.Pixel, ia);
                        }
                    }
                    else
                    {
                        g.DrawImage(pship, pr);
                    }
                }
            }

            if (bship != null)
            {
                lock (bship)
                {
                    Rectangle br = ScaleRect(BSHIP_X, BSHIP_Y, BSHIP_W, BSHIP_H);
                    if (bossHit)
                    {
                        ColorMatrix cm = new ColorMatrix(new float[][]
                        {
                        new float[] { 1, 0, 0, 0, 0 },
                        new float[] { 0, 0, 0, 0, 0 },
                        new float[] { 0, 0, 0, 0, 0 },
                        new float[] { 0, 0, 0, 1, 0 },
                        new float[] { 0.5f, 0, 0, 0, 1 }
                        });
                        using (ImageAttributes ia = new ImageAttributes())
                        {
                            ia.SetColorMatrix(cm);
                            g.DrawImage(bship, br, 0, 0, bship.Width, bship.Height, GraphicsUnit.Pixel, ia);
                        }
                    }
                    else
                    {
                        g.DrawImage(bship, br);
                    }
                }
            }

            if (cballactive && cball != null)
            {
                lock (cball)
                {
                    int cbw = (int)(CBALL_W * scaleX), cbh = (int)(CBALL_H * scaleY);
                    g.DrawImage(cball, (int)cballx, (int)cbally, cbw, cbh);
                }
            }

            drawHPbar(g, ScaleRect(200,1020, 300, 18), playerHP, 5, "Your Ship");
            drawHPbar(g, ScaleRect(1500, 1020, 300, 18), bossHP, 5, "Enemy");

            if (currentq >= questions.Count) return;

            var q = questions[currentq];

            using (Font qfont = new Font("Antiquity Print", Math.Max(8, (int)(13 * sf)), FontStyle.Bold))
            using (Font cfont = new Font("Book Antiqua", Math.Max(8, (int)(12 * sf)), FontStyle.Bold))
            using (Font timerfont = new Font("Antiquity Print", Math.Max(8, (int)(14 * sf)), FontStyle.Bold))
            using (SolidBrush pulse = new SolidBrush(Color.FromArgb(color, 60, 100)))
            using (SolidBrush dark = new SolidBrush(Color.Black))
            {
                Color timercolor;

                if (timeLeft <= 3)
                {
                    timercolor = Color.Red;
                }
                else
                {
                    timercolor = Color.DarkBlue;
                }
                using (SolidBrush tb = new SolidBrush(timercolor))
                {
                    g.DrawString($"Answer in ({timeLeft})s", timerfont, tb, new PointF(100 * scaleX, 30 * scaleY));
                    g.DrawString("Final Bossfight", timerfont, Brushes.Red, new PointF(890 * scaleX, 1000 * scaleY));
                }

                RectangleF qbox;

                if(q.truefalse)
                {
                    qbox = new RectangleF(100 * scaleX, 100 * scaleY, (this.ClientSize.Width - 400 * scaleX), 100 * scaleY);
                }
                else
                {
                    qbox = new RectangleF(100 * scaleX, 100 * scaleY, (this.ClientSize.Width - 100 * scaleX), 100 * scaleY);
                }
                g.DrawString(q.quest, qfont, pulse, qbox);

                string[] labels = { "A)", "B)", "C)", "D)" };
                float ystart;

                if(q.truefalse)
                {
                    ystart = 200 * scaleY;
                }
                else
                {
                    ystart = 150 * scaleY;
                }
                
                float ystep;

                if (q.truefalse)
                {
                    ystep = 65 * scaleY;
                }
                else
                {
                    ystep = 65 * scaleY;
                }

                for (int i = 0; i < q.choices.Count; i++)
                {
                    Brush cb = dark;
                    if (!answered && !timedout && i == selch)
                    {
                        cb = Brushes.ForestGreen;
                    }
                    if (answered || timedout)
                    {
                        if (i == q.coridx)
                        {
                            cb = Brushes.ForestGreen;
                        }
                        else if (i == lastchose)
                        {
                            cb = Brushes.Red;
                        }
                    }

                    if (!answered && !timedout && i == selch)
                    {
                        g.DrawString("➤", cfont, Brushes.ForestGreen, new PointF(50 * scaleX, ystart));
                    }

                    string label;

                    if (q.truefalse)
                    {
                        label = "";
                    }
                    else
                    {
                        label = labels[i] + " ";
                    }
                    RectangleF cbox = new RectangleF( 100 * scaleX, ystart, this.ClientSize.Width - 150 * scaleX, ystep);
                    g.DrawString(label + q.choices[i], cfont, cb, cbox);
                    ystart += ystep;
                }

                g.DrawString($"Score: {score}/{currentq}", cfont, pulse, new PointF(this.ClientSize.Width - 200 * scaleX, 80 * scaleY));

                if (timedout)
                {
                    g.DrawString("Too slow! Your ship takes a hit!", qfont, Brushes.Red, new PointF(200 * scaleX, 420 * scaleY));
                }
                else if (answered && bossHit)
                {
                    g.DrawString("Correct! Enemy ship hit!", qfont, Brushes.LightGreen, new PointF(200 * scaleX, 420 * scaleY));
                }
                else if (answered && playerHit)
                {
                    g.DrawString("Wrong! Your ship takes a hit!", qfont, Brushes.Red, new PointF(200 * scaleX, 420 * scaleY));
                }
            }
        }

        private void drawHPbar(Graphics g, Rectangle r, int hp, int maxhp, string label)
        {
            float sf = Math.Min(scaleX, scaleY);
            using (Font f = new Font("Antiquity Print", Math.Max(7, (int)(10 * sf)), FontStyle.Bold))
            using (SolidBrush bg2 = new SolidBrush(Color.FromArgb(80, 0, 0)))
            using (SolidBrush fg = new SolidBrush(Color.FromArgb(200, 30, 30)))
            using (SolidBrush white = new SolidBrush(Color.White))
            {
                g.FillRectangle(bg2, r);
                int fillw = (int)(r.Width * ((float)hp / maxhp));
                g.FillRectangle(fg, r.X, r.Y, fillw, r.Height);
                g.DrawString($"{label} {hp}/{maxhp}", f, white, r.X, r.Y - (int)(16 * scaleY));
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            gtimer.Stop();

            if (bg != null) { ImageAnimator.StopAnimate(bg, OnFrameChanged); bg.Dispose(); bg = null; }
            if (pship != null) { ImageAnimator.StopAnimate(pship, OnFrameChanged); pship.Dispose(); pship = null; }
            if (bship != null) { ImageAnimator.StopAnimate(bship, OnFrameChanged); bship.Dispose(); bship = null; }
            if (cball != null) { ImageAnimator.StopAnimate(cball, OnFrameChanged); cball.Dispose(); cball = null; }
        }
    }
}