using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace TarjimonOfficeUZ.Shared
{
    public static class ResourceLoader
    {
        private static readonly Assembly assembly = typeof(ResourceLoader).Assembly;

        public static Bitmap LatinToCyrillic => CreateText("Lotin → Kirill");
        public static Bitmap CyrillicToLatin => CreateText("Kirill → Lotin");
        public static Bitmap Settings => CreateHexagon();

        public static Bitmap Load(string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new Exception("Resurs topilmadi: " + resourceName);
                return new Bitmap(stream);
            }
        }

        private static Bitmap CreateText(string text)
        {
            var bitmap = new Bitmap(128, 64);
            using (var graphics = Graphics.FromImage(bitmap))
            using (var font = new Font("Segoe UI", 20f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Pixel))
            using (var brush = new SolidBrush(Color.Black))
            {
                graphics.Clear(Color.Transparent);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                var size = graphics.MeasureString(text, font);
                var x = Math.Max(2f, (128f - size.Width) / 2f);
                var y = Math.Max(0f, (64f - size.Height) / 2f);
                graphics.DrawString(text, font, brush, x, y);
            }
            return bitmap;
        }

        private static Bitmap CreateHexagon()
        {
            var bitmap = new Bitmap(72, 64);
            using (var graphics = Graphics.FromImage(bitmap))
            using (var brush = new SolidBrush(Color.Black))
            {
                graphics.Clear(Color.Transparent);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                const float centerX = 36f;
                const float centerY = 32f;
                const float radius = 29f;
                var points = new PointF[6];
                for (var i = 0; i < 6; i++)
                {
                    var angle = (-90 + i * 60) * Math.PI / 180.0;
                    points[i] = new PointF(
                        centerX + (float)(radius * Math.Cos(angle)),
                        centerY + (float)(radius * Math.Sin(angle)));
                }
                graphics.FillPolygon(brush, points);
            }
            return bitmap;
        }
    }
}
