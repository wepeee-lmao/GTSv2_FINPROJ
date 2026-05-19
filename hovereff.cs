using GTSv2_FINPROJ;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GTSv2_FINPROJ.Intro;

namespace GTSv2_FINPROJ
{
    internal class hovereff : PictureBox
    {
        public int dataid { get; set; }

        public Point origloc { get; set; }
        private Image normimage;
        private Image orignormimage;
        private Image orighovimage;
        private Image hovimage;

        public hovereff(Image norm, Image hov, Point loc, EventHandler cevent)
        {
            this.normimage = norm;
            this.hovimage = hov;
            this.orignormimage = norm;
            this.orighovimage = hov;
            this.origloc = loc;

            this.Image = norm;
            this.Location = loc;
            this.SizeMode = PictureBoxSizeMode.AutoSize;
            this.BackColor = Color.Transparent;
            this.Visible = false;

            this.MouseEnter += (s, e) => { this.Image = hovimage; };
            this.MouseLeave += (s, e) => { this.Image = normimage; };
            this.Click += cevent;
        }

        public void UpdateScale(float scaleX, float scaleY)
        {
            this.Location = new Point(
                (int)(origloc.X * scaleX),
                (int)(origloc.Y * scaleY)
            );

            int newWidth = (int)(orignormimage.Width * scaleX);
            int newHeight = (int)(orignormimage.Height * scaleY);
            this.Size = new Size(newWidth, newHeight);
            this.SizeMode = PictureBoxSizeMode.StretchImage;

            bool isHovering = (this.Image == hovimage);
            if (isHovering)
            {
                this.Image = hovimage;
            }
            else
            {
                this.Image = normimage;
            }
        }

        internal string getpath(string folder, string fname)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder, fname);
        }
    }

    internal static class texteff
    {
        public static Color getfadecolor(Color start, Color end, float prog)
        {
            prog = Math.Max(0f, Math.Min(1f, prog));
            int r = (int)(start.R + (end.R - start.R) * prog);
            int g = (int)(start.G + (end.G - start.G) * prog);
            int b = (int)(start.B + (end.B - start.B) * prog);
            return Color.FromArgb(r, g, b);

        }
        public static string Typewrite(string text, int idx)
        {
            if (idx < 0)
            {
                return "";
            }
            if (idx > text.Length)
            {
                return text;
            }
            return text.Substring(0, idx + 1);

        }

    }
    public static class sprite
    {
        public static Image walkleft { get; private set; }
        public static Image walkright { get; private set; }
        public static Image walkup { get; private set; }
        public static Image walkdown { get; private set; }
        public static Image frontidle { get; private set; }
        public static Image backidle { get; private set; }

        private static bool loaded = false;

        public static void Load()
        {
            if (loaded)
            {
                return;
            }
            string movePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "movement");
            walkleft = Image.FromFile(Path.Combine(movePath, "walkleft.gif"));
            walkright = Image.FromFile(Path.Combine(movePath, "walkright.gif"));
            walkup = Image.FromFile(Path.Combine(movePath, "walkup.gif"));
            walkdown = Image.FromFile(Path.Combine(movePath, "walkdown.gif"));
            frontidle = Image.FromFile(Path.Combine(movePath, "frontidle.png"));
            backidle = Image.FromFile(Path.Combine(movePath, "backidle.png"));
            loaded = true;
        }
        public static void register(EventHandler onFrame)
        {
            ImageAnimator.Animate(walkleft, onFrame);
            ImageAnimator.Animate(walkright, onFrame);
            ImageAnimator.Animate(walkup, onFrame);
            ImageAnimator.Animate(walkdown, onFrame);
            ImageAnimator.Animate(frontidle, onFrame);
            ImageAnimator.Animate(backidle, onFrame);
        }

        public static void unregister(EventHandler onFrame)
        {
            ImageAnimator.StopAnimate(walkleft, onFrame);
            ImageAnimator.StopAnimate(walkright, onFrame);
            ImageAnimator.StopAnimate(walkup, onFrame);
            ImageAnimator.StopAnimate(walkdown, onFrame);
            ImageAnimator.StopAnimate(frontidle, onFrame);
            ImageAnimator.StopAnimate(backidle, onFrame);
        }
    }
    public static class nav
    {
        private static Form next = null;
        public static FormWindowState lastState = FormWindowState.Normal;
        public static Size lastSize = new Size(1920, 1080);

        public static void go(Form nextForm)
        {
            next = nextForm;
        }

        public static void remember(Form current)
        {
            lastState = current.WindowState;
            if (current.WindowState == FormWindowState.Normal)
                lastSize = current.Size;
        }

        public static void apply(Form f)
        {
            f.Size = lastSize;
            f.WindowState = lastState;
        }

        public static void run()
        {
            while (next != null)
            {
                Form current = next;
                next = null;
                current.ShowDialog();
                current.Dispose();
            }
        }
    }
}