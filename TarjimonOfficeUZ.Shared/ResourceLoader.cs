using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace TarjimonOfficeUZ.Shared
{
    public static class ResourceLoader
    {
        private static readonly Assembly assembly = typeof(ResourceLoader).Assembly;
        public static Bitmap Load(string resourceName)
        {
            Assembly assembly = typeof(ResourceLoader).Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new Exception("Resurs topilmadi: " + resourceName);

                return new Bitmap(stream);
            }
        }

        public static Bitmap A_A =>
            Load("TarjimonOfficeUZ.Shared.Resources.Icons.A_A.png");

        public static Bitmap Kalit =>
            Load("TarjimonOfficeUZ.Shared.Resources.Icons.Kalit.png");
    }
}