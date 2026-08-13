using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace TarjimonOfficeUZ.Shared
{
    public static class ResourceLoader
    {
        private static readonly Assembly assembly = typeof(ResourceLoader).Assembly;

        public static Bitmap LatinToCyrillic => CreateTranslationText("Latin", "Кирилл", false);

        public static Bitmap CyrillicToLatin => CreateTranslationText("Кирилл", "Latin", true);

        public static Bitmap Settings => CreateSettingsTriangle();

        public static Bitmap Load(string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new Exception("Resurs topilmadi: " + resourceName);

                return new Bitmap(stream);
            }
        }

        private static Bitmap CreateTranslationText(string leftText, string rightText, bool reverse)
        {
            const int width = 120;
            const int height = 62;
            var bitmap = new Bitmap(width, height);
            bitmap.SetResolution(96, 96);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Font textFont = new Font("Segoe UI", 15.5f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Pixel))
            using (Font arrowFont = new Font("Segoe UI Symbol", 18f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                graphics.Clear(Color.Transparent);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                SizeF leftSize = graphics.MeasureString(leftText, textFont);
                SizeF rightSize = graphics.MeasureString(rightText, textFont);
                SizeF arrowSize = graphics.MeasureString("→", arrowFont);

                float totalWidth = leftSize.Width + arrowSize.Width + rightSize.Width + 8f;
                float x = (width - totalWidth) / 2f;
                float y = 16f;

                if (reverse)
                {
                    DrawGoldSheenText(graphics, rightText, textFont, x, y, Color.FromArgb(31, 112, 209), Color.FromArgb(48, 155, 232));
                    x += rightSize.Width + 4f;
                    DrawGoldSheenArrow(graphics, arrowFont, "←", x, y - 1f);
                    x += arrowSize.Width + 4f;
                    DrawGoldSheenText(graphics, leftText, textFont, x, y, Color.FromArgb(38, 170, 79), Color.FromArgb(78, 199, 100));
                }
                else
                {
                    DrawGoldSheenText(graphics, leftText, textFont, x, y, Color.FromArgb(31, 112, 209), Color.FromArgb(48, 155, 232));
                    x += leftSize.Width + 4f;
                    DrawGoldSheenArrow(graphics, arrowFont, "→", x, y - 1f);
                    x += arrowSize.Width + 4f;
                    DrawGoldSheenText(graphics, rightText, textFont, x, y, Color.FromArgb(38, 170, 79), Color.FromArgb(78, 199, 100));
                }
            }

            return bitmap;
        }

        private static void DrawGoldSheenText(Graphics graphics, string text, Font font, float x, float y, Color baseColor, Color lightColor)
        {
            SizeF size = graphics.MeasureString(text, font);
            using (LinearGradientBrush gradient = new LinearGradientBrush(
                new RectangleF(x, y, size.Width + 2, size.Height + 2),
                baseColor,
                Color.FromArgb(255, 218, 92),
                LinearGradientMode.Vertical))
            using (var shadow = new SolidBrush(Color.FromArgb(55, 0, 0, 0)))
            using (var highlightPen = new Pen(Color.FromArgb(160, 255, 255, 255), 1.2f))
            {
                graphics.DrawString(text, font, shadow, x + 1.2f, y + 1.8f);
                graphics.DrawString(text, font, gradient, x, y);

                // Small diagonal "shine" accent.
                float shineX = x + size.Width * 0.18f;
                graphics.DrawLine(highlightPen, shineX, y + size.Height * 0.72f, shineX + 9f, y + size.Height * 0.18f);
            }
        }

        private static void DrawGoldSheenArrow(Graphics graphics, Font font, string arrow, float x, float y)
        {
            using (var shadow = new SolidBrush(Color.FromArgb(55, 0, 0, 0)))
            using (var gradient = new LinearGradientBrush(
                new RectangleF(x, y, 22f, 24f),
                Color.FromArgb(232, 177, 32),
                Color.FromArgb(255, 231, 127),
                LinearGradientMode.Vertical))
            {
                graphics.DrawString(arrow, font, shadow, x + 1f, y + 1.4f);
                graphics.DrawString(arrow, font, gradient, x, y);
            }
        }

        private static Bitmap CreateSettingsTriangle()
        {
            const int width = 72;
            const int height = 62;
            var bitmap = new Bitmap(width, height);
            bitmap.SetResolution(96, 96);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (var shadow = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
            using (var gradient = new LinearGradientBrush(
                new RectangleF(8, 10, 56, 42),
                Color.FromArgb(214, 157, 30),
                Color.FromArgb(255, 224, 102),
                LinearGradientMode.Vertical))
            using (var shine = new Pen(Color.FromArgb(190, 255, 255, 255), 2f))
            {
                graphics.Clear(Color.Transparent);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                PointF[] shadowTriangle =
                {
                    new PointF(20, 25),
                    new PointF(54, 25),
                    new PointF(37, 46)
                };
                graphics.FillPolygon(shadow, shadowTriangle);

                PointF[] triangle =
                {
                    new PointF(18, 23),
                    new PointF(52, 23),
                    new PointF(35, 44)
                };
                graphics.FillPolygon(gradient, triangle);
                graphics.DrawLine(shine, new PointF(24, 27), new PointF(46, 27));
            }

            return bitmap;
        }
    }
}
