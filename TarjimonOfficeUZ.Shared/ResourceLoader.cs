using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;

namespace TarjimonOfficeUZ.Shared
{
    public static class ResourceLoader
    {
        private static readonly Assembly assembly = typeof(ResourceLoader).Assembly;

        public static Bitmap Load(string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new Exception("Resurs topilmadi: " + resourceName);

                return new Bitmap(stream);
            }
        }

        // Backward-compatible alias kept for existing code.
        public static Bitmap A_A => LatinToCyrillic;

        // Backward-compatible alias kept for existing code.
        public static Bitmap Kalit => Settings;

        public static Bitmap LatinToCyrillic => CreateTranslationIcon(false);

        public static Bitmap CyrillicToLatin => CreateTranslationIcon(true);

        public static Bitmap Settings => CreateSettingsIcon();

        private static Bitmap CreateTranslationIcon(bool cyrillicToLatin)
        {
            const int size = 96;
            Bitmap bitmap = new Bitmap(size, size);
            bitmap.SetResolution(96, 96);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Font letterFont = new Font("Segoe UI", 21, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Font smallFont = new Font("Segoe UI", 9, FontStyle.Bold, GraphicsUnit.Pixel))
            using (SolidBrush latinBrush = new SolidBrush(Color.FromArgb(31, 112, 209)))
            using (SolidBrush cyrillicBrush = new SolidBrush(Color.FromArgb(38, 170, 79)))
            using (SolidBrush textBrush = new SolidBrush(Color.White))
            using (Pen arrowPen = new Pen(Color.FromArgb(34, 153, 80), 5))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.Clear(Color.Transparent);

                Rectangle latinRect = new Rectangle(4, 18, 34, 42);
                Rectangle cyrillicRect = new Rectangle(58, 18, 34, 42);

                DrawRoundedPanel(graphics, latinRect, Color.FromArgb(31, 112, 209));
                DrawRoundedPanel(graphics, cyrillicRect, Color.FromArgb(38, 170, 79));

                string leftText = cyrillicToLatin ? "Аа" : "Aa";
                string rightText = cyrillicToLatin ? "Aa" : "Аа";

                DrawCenteredText(graphics, leftText, letterFont, textBrush, latinRect);
                DrawCenteredText(graphics, rightText, letterFont, textBrush, cyrillicRect);

                Point start = cyrillicToLatin ? new Point(58, 39) : new Point(38, 39);
                Point end = cyrillicToLatin ? new Point(38, 39) : new Point(58, 39);
                DrawArrow(graphics, arrowPen, start, end);

                Rectangle captionRect = new Rectangle(0, 66, 96, 16);
                DrawCenteredText(
                    graphics,
                    cyrillicToLatin ? "Кирилл  →  Latin" : "Latin  →  Кирилл",
                    smallFont,
                    new SolidBrush(Color.FromArgb(55, 55, 55)),
                    captionRect);
            }

            return bitmap;
        }

        private static Bitmap CreateSettingsIcon()
        {
            const int size = 96;
            Bitmap bitmap = new Bitmap(size, size);
            bitmap.SetResolution(96, 96);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (SolidBrush gearBrush = new SolidBrush(Color.FromArgb(83, 91, 102)))
            using (SolidBrush centerBrush = new SolidBrush(Color.White))
            using (SolidBrush triangleBrush = new SolidBrush(Color.FromArgb(45, 45, 45)))
            using (Pen outline = new Pen(Color.FromArgb(120, 128, 140), 2))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.Clear(Color.Transparent);

                PointF center = new PointF(42, 42);
                const float outer = 24;
                const float inner = 15;
                const int teeth = 8;

                for (int i = 0; i < teeth; i++)
                {
                    double angle = i * Math.PI / 4.0;
                    float cx = center.X + (float)Math.Cos(angle) * 19;
                    float cy = center.Y + (float)Math.Sin(angle) * 19;
                    RectangleF tooth = new RectangleF(cx - 6, cy - 6, 12, 12);
                    graphics.FillRectangle(gearBrush, tooth);
                }

                graphics.FillEllipse(gearBrush, center.X - outer, center.Y - outer, outer * 2, outer * 2);
                graphics.FillEllipse(centerBrush, center.X - inner / 2, center.Y - inner / 2, inner, inner);
                graphics.DrawEllipse(outline, center.X - outer, center.Y - outer, outer * 2, outer * 2);

                PointF[] triangle =
                {
                    new PointF(66, 64),
                    new PointF(84, 64),
                    new PointF(75, 77)
                };
                graphics.FillPolygon(triangleBrush, triangle);
            }

            return bitmap;
        }

        private static void DrawRoundedPanel(Graphics graphics, Rectangle rectangle, Color color)
        {
            using (SolidBrush brush = new SolidBrush(color))
            using (GraphicsPath path = RoundedRectangle(rectangle, 8))
            {
                graphics.FillPath(brush, path);
            }
        }

        private static GraphicsPath RoundedRectangle(Rectangle rectangle, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rectangle.X, rectangle.Y, diameter, diameter);

            path.AddArc(arc, 180, 90);
            arc.X = rectangle.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rectangle.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rectangle.X;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void DrawCenteredText(Graphics graphics, string text, Font font, Brush brush, RectangleF area)
        {
            StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };

            graphics.DrawString(text, font, brush, area, format);
            format.Dispose();
        }

        private static void DrawArrow(Graphics graphics, Pen pen, Point start, Point end)
        {
            graphics.DrawLine(pen, start, end);

            double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
            const double head = Math.PI / 6;
            const float length = 9;

            PointF p1 = new PointF(
                end.X - length * (float)Math.Cos(angle - head),
                end.Y - length * (float)Math.Sin(angle - head));
            PointF p2 = new PointF(
                end.X - length * (float)Math.Cos(angle + head),
                end.Y - length * (float)Math.Sin(angle + head));

            graphics.DrawLine(pen, end, p1);
            graphics.DrawLine(pen, end, p2);
        }
    }
}
