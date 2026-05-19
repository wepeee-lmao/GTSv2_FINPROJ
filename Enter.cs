using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GTSv2_FINPROJ
{
    public partial class Enter : Form
    {
        Image bg;
        Image loadinggif;

        float transp0 = 0.0f;
        float lab0 = 0.0f;

        int idx = 0;
        int speed = 0;
        int stage = 0;
        int wait = 100;
        //0 - nameprompt
        //1 - wait input
        //2 - fadein bg
        //3 - type loading
        Point playerPos = new Point(850, 900);
        Rectangle nventDoor = new Rectangle(490, 405, 100, 100);
        Rectangle knowDoor = new Rectangle(1430, 400, 100, 100);
        Rectangle exit = new Rectangle(850, 900, 200, 200);

        private Size originalSize;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;

        int x = 50, y = 830;
        string text = "What's your name captain?";

        bool loading = false;

        public Enter()
        {
            InitializeComponent();

            this.ResizeRedraw = true;
            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
            setupimage();
            setupcontrols();
            textname.Visible = false;

            gtimer.Start();
        }

        private void Enter_Load(object sender, EventArgs e)
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
                return;


            scaleX = (float)this.ClientSize.Width / originalSize.Width;
            scaleY = (float)this.ClientSize.Height / originalSize.Height;

            label.Location = new Point((int)(500 * scaleX), (int)(400 * scaleY));
            label.Size = new Size((int)(1100 * scaleX), (int)(500 * scaleY));
            label.Font = new Font("Antiquity Print", Math.Max(28, 28 * Math.Min(scaleX, scaleY)));

            textname.Location = new Point((int)(400 * scaleX), (int)(550 * scaleY));
            textname.Size = new Size((int)(1100 * scaleX), (int)(200 * scaleY));
            textname.Font = new Font(textname.Font.FontFamily, Math.Max(28, 28 * Math.Min(scaleX, scaleY)));
          
            x = (int)(50 * scaleX);
            y = (int)(830 * scaleY);

            this.Invalidate();
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
 
        private void setupimage()
        {
            bg = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "6.png"));
            loadinggif = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "loading.gif"));

            ImageAnimator.Animate(loadinggif, this.OnFrameChangedHandler);
            FrameDimension dimension = new FrameDimension(loadinggif.FrameDimensionsList[0]);
            int totframe, endframe;

            totframe = loadinggif.GetFrameCount(dimension);
            endframe = totframe;

            gtimer.Interval = 30;
        }

        private void OnFrameChangedHandler(object? sender, EventArgs e)
        {
            if (loadinggif != null)
            {
                lock (loadinggif)
                {
                    ImageAnimator.UpdateFrames(loadinggif);
                }
            }
        }
        private void setupcontrols()
        {
            label.AutoSize = true; 
            label.BackColor = Color.Transparent;
            label.ForeColor = Color.White;
            label.Location = new Point(500, 400);
            label.Size = new Size(1100, 500);
            label.Font = new Font("Antiquity Print", 14);

            textname.Size = new Size(1100, 500);
            textname.Location = new Point(400, 550);
            textname.BorderStyle = BorderStyle.None;
            textname.BackColor = Color.White;
            textname.ForeColor = Color.Black;
            textname.Font = new Font(textname.Font.FontFamily, 14);
        }
        private void FormPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            if (bg != null && transp0 > 0)
            {
                transitioneffects.drawfade(g, bg, transp0, this.ClientRectangle);
            }
            if (loadinggif!= null && transp0 > 0.05)
            {
                ColorMatrix cm = new ColorMatrix { Matrix33 = 1.0f };
                ImageAttributes atr = new ImageAttributes();
                atr.SetColorMatrix(cm);

                Rectangle rec = ScaleRectangle(new Rectangle(x, y, 100, 100));
                lock (loadinggif)
                {
                    g.drawbutfade(loadinggif, x, y, transp0, rec);
                }
            }
        }


        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                playdata.name = textname.Text;

                if (!dbconnect.hasplayprogress(playdata.accountid))
                {
                    if (dbconnect.saveprog(playdata.name, playdata.silver, 0, playdata.compvoy, playdata.accountid))
                    {
                        dbconnect.loadprogress(playdata.accountid);
                        textname.Visible = false;
                        label.Visible = false;
                        stage = 2;
                        gtimer.Start();
                    }
                }
                else
                {
                    dbconnect.loadprogress(playdata.accountid);
                    textname.Visible = false;
                    label.Visible = false;
                    stage = 2;
                    gtimer.Start();
                }
            }
        }

        private void gtimer_Tick(object sender, EventArgs e)
        {
            switch (stage)
            {
                case 0:
                    speed++;
                    if (speed >= 3)
                    {
                        if (idx < text.Length)
                        {
                            label.Text = texteff.Typewrite(text, idx++);
                            label.ForeColor = texteff.getfadecolor(Color.White,Color.White, lab0);
                            speed = 0;
                        }
                        else
                        {
                            textname.Visible = true;
                            textname.Focus();
                            gtimer.Stop();
                            stage = 2;
                        }
                    }
                    break;
                case 2:
                    if (transp0<1.0f)
                    {
                        transp0 += 0.05f;
                    }
                    else
                    {
                        label.Text = "";
                        label.Visible = true;
                        idx = 0;
                        stage = 3;
                    }
                    break;
                case 3:
                    string text2 = "Loading......";
                    speed++;
                    if (speed >= 8)
                    {
                        if (idx < text2.Length)
                        {
                            label.Location = new Point(150, 850);
                            label.Text += text2[idx++];
                        }
                        else
                        {
                            if (wait > 0)
                            {
                                wait--;
                            }
                            else
                            {
                                stage = 4;
                            }
                        }
                    }
                    break;
                case 4:
                    if (transp0 > 0.0f)
                    {
                        transp0 -= 0.05f;

                        if (lab0 < 1.0f)
                        {
                            lab0 += 0.05f;
                        }
                        label.ForeColor = texteff.getfadecolor(Color.White, Color.Black, lab0);
                    }
                    else
                    {
                        gtimer.Stop();
                        nav.remember(this);
                        this.Close();
                        var n = new Intro();
                        nav.apply(n);
                        nav.go(n);
                        return;
                    }
                    break;
            }
            this.Invalidate();
        }
    }
}
