using System;
using System.IO;
using System.Xml.Serialization;
using TarjimonOfficeUZ.Shared.Managers;

namespace TarjimonOfficeUZ.Shared.Managers
{
    public static class SettingsManager
    {
        private static readonly string Folder =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                "Tarjimon Office UZ");

        private static readonly string FileName =
            Path.Combine(
                Folder,
                "settings.xml");

        public static SettingsModel Current { get; private set; }

        static SettingsManager()
        {
            Load();
        }

        public static void Load()
        {
            try
            {
                if (!Directory.Exists(Folder))
                    Directory.CreateDirectory(Folder);

                if (!File.Exists(FileName))
                {
                    Current = new SettingsModel();

                    Save();

                    return;
                }

                XmlSerializer serializer =
                    new XmlSerializer(typeof(SettingsModel));

                using (FileStream stream =
                    new FileStream(FileName, FileMode.Open))
                {
                    Current =
                        (SettingsModel)serializer.Deserialize(stream);
                }

                if (Current == null)
                    Current = new SettingsModel();
            }
            catch
            {
                Current = new SettingsModel();
            }
        }

        public static void Save()
        {
            try
            {
                if (!Directory.Exists(Folder))
                    Directory.CreateDirectory(Folder);

                XmlSerializer serializer =
                    new XmlSerializer(typeof(SettingsModel));

                using (FileStream stream =
                    new FileStream(FileName, FileMode.Create))
                {
                    serializer.Serialize(stream, Current);
                }
            }
            catch
            {

            }
        }
    }
}
