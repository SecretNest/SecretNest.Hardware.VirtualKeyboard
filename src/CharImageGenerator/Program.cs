using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace CharImageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string keys =
                @"`1234567890-=qwertyuiop[]\asdfghjkl;'zxcvbnm,./~!@#$%^&*()_+QWERTYUIOP{}|ASDFGHJKL:""ZXCVBNM<>? ";
            Dictionary<char, Bitmap> whites = new Dictionary<char, Bitmap>();
            Dictionary<char, Bitmap> blacks = new Dictionary<char, Bitmap>();

            var bytes = Encoding.ASCII.GetBytes(keys);

            using (Font font = new Font("Consolas", 10, FontStyle.Bold))
            {
                PointF point = new PointF(-2, -3);

                for (int i = 0; i < keys.Length; i++)
                {
                    whites[keys[i]] = Generate(keys[i].ToString(), Color.White, Brushes.Black, font, point);
                    blacks[keys[i]] = Generate(keys[i].ToString(), Color.Black, Brushes.White, font, point);
                }
            }

            using (Font font = new Font("Consolas", 7, FontStyle.Bold))
            {
                PointF point = new PointF(-3, 0);
                char key = '⏎';

                whites[key] = Generate(key.ToString(), Color.White, Brushes.Black, font, point);
                blacks[key] = Generate(key.ToString(), Color.Black, Brushes.White, font, point);
            }

            using (Bitmap final = new Bitmap(128, 144))
            using (Graphics g = Graphics.FromImage(final))
            {
                int x = 8, y = 0;

                PointF p = new PointF(0, y);
                g.DrawImage(blacks['1'], p);

                foreach (var c in "234567890,.(); ")
                {
                    p = new PointF(x, y);
                    x += 8;
                    g.DrawImage(whites[c], p);
                }

                x = 0;
                y = 12;
                foreach (var c in "abcdefghijklmnop")
                {
                    p = new PointF(x, y);
                    x += 8;
                    g.DrawImage(whites[c], p);
                }

                x = 0;
                y = 24;
                foreach (var c in "qrstuvwxyz+-%#&⏎")
                {
                    p = new PointF(x, y);
                    x += 8;
                    g.DrawImage(whites[c], p);
                }

                x = 0;
                y = 36;
                foreach (var c in @"?!:_*/\=[]{}<>""'")
                {
                    p = new PointF(x, y);
                    x += 8;
                    g.DrawImage(whites[c], p);
                }

                x = 0;
                y = 48;
                foreach (var c in "ABCDEFGHIJKLMNOP")
                {
                    p = new PointF(x, y);
                    x += 8;
                    g.DrawImage(whites[c], p);
                }

                x = 0;
                y = 60;
                foreach (var c in "QRSTUVWXYZ^~`|$@")
                {
                    p = new PointF(x, y);
                    x += 8;
                    g.DrawImage(whites[c], p);
                }

                x = 8;
                y = 72;
                p = new PointF(0, y);
                g.DrawImage(whites['1'], p);

                foreach (var c in "234567890,.(); ")
                {
                    p = new PointF(x, y);
                    x += 8;
                    g.DrawImage(blacks[c], p);
                }

                x = 0;
                y = 84;
                foreach (var c in "abcdefghijklmnop")
                {
                    p = new PointF(x, y);
                    x += 8;
                    g.DrawImage(blacks[c], p);
                }

                x = 0;
                y = 96;
                foreach (var c in "qrstuvwxyz+-%#&⏎")
                {
                    p = new PointF(x, y);
                    x += 8;
                    g.DrawImage(blacks[c], p);
                }

                x = 0;
                y = 108;
                foreach (var c in @"?!:_*/\=[]{}<>""'")
                {
                    p = new PointF(x, y);
                    x += 8;
                    g.DrawImage(blacks[c], p);
                }

                x = 0;
                y = 120;
                foreach (var c in "ABCDEFGHIJKLMNOP")
                {
                    p = new PointF(x, y);
                    x += 8;
                    g.DrawImage(blacks[c], p);
                }

                x = 0;
                y = 132;
                foreach (var c in "QRSTUVWXYZ^~`|$@")
                {
                    p = new PointF(x, y);
                    x += 8;
                    g.DrawImage(blacks[c], p);
                }

                final.Save("Output.bmp");
            }


        }

        static readonly Rectangle rectangle = new Rectangle(0, 0, 8, 12);
        
        static Bitmap Generate(string text, Color backColor, Brush brush, Font font, PointF point)
        {
            using Bitmap bitmap = new Bitmap(8, 12);
            using Graphics g = Graphics.FromImage(bitmap);
            g.Clear(backColor);
            g.DrawString(text, font, brush, point);
            Bitmap mono = bitmap.Clone(rectangle, PixelFormat.Format1bppIndexed);
            return mono;
        }
    }
}
