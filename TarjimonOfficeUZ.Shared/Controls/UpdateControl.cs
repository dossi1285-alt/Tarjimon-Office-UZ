using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using TarjimonOfficeUZ.Shared.Managers;

namespace TarjimonOfficeUZ.Shared.Controls
{
    public partial class UpdateControl : UserControl
    {
        private static readonly HttpClient Http = CreateHttpClient();

        public UpdateControl()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            lblVersion.Text = Constants.Version;
            ApplyLanguage();
        }

        private static HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true });
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("TarjimonOfficeUZ/1.0");
            return client;
        }

        public void ApplyLanguage()
        {
            switch (SettingsManager.Current.Language)
            {
                case "ru":
                    lblCurrentVersion.Text = "Текущая версия";
                    lblStatus.Text = "Статус обновления";
                    btnCheckUpdate.Text = "Проверить обновления";
                    btnClose.Text = "Закрыть";
                    break;
                case "en":
                    lblCurrentVersion.Text = "Current version";
                    lblStatus.Text = "Update status";
                    btnCheckUpdate.Text = "Check for updates";
                    btnClose.Text = "Close";
                    break;
                default:
                    lblCurrentVersion.Text = "Joriy versiya";
                    lblStatus.Text = "Yangilanish holati";
                    btnCheckUpdate.Text = "Yangilanishlarni tekshirish";
                    btnClose.Text = "Yopish";
                    break;
            }
        }

        private async void btnCheckUpdate_Click(object sender, EventArgs e)
        {
            btnCheckUpdate.Enabled = false;
            SetStatus(GetText("Tekshirilmoqda...", "Проверка...", "Checking..."));

            try
            {
                HttpResponseMessage response = await Http.GetAsync(Constants.UpdateUrl);

                if (!response.IsSuccessStatusCode)
                {
                    SetStatus(GetText(
                        "Hozircha GitHub Releases'da rasmiy reliz topilmadi.",
                        "Официальный релиз в GitHub Releases пока не опубликован.",
                        "No official release is published on GitHub Releases yet."));
                    return;
                }

                Uri finalUri = response.RequestMessage != null ? response.RequestMessage.RequestUri : null;
                string tag = ExtractVersionTag(finalUri);
                Version latest;
                Version current;

                if (!Version.TryParse(tag, out latest) || !Version.TryParse(Constants.Version, out current))
                {
                    SetStatus(GetText(
                        "Reliz mavjud. Batafsil ma'lumot uchun GitHub Releases sahifasini oching.",
                        "Релиз доступен. Откройте GitHub Releases для подробностей.",
                        "A release is available. Open GitHub Releases for details."));
                    return;
                }

                if (latest > current)
                {
                    SetStatus(GetText(
                        "Yangi versiya topildi: " + latest,
                        "Найдена новая версия: " + latest,
                        "New version found: " + latest));

                    DialogResult result = MessageBox.Show(
                        GetText(
                            "Yangi versiya mavjud. GitHub Releases sahifasini ochasizmi?",
                            "Доступна новая версия. Открыть страницу GitHub Releases?",
                            "A new version is available. Open GitHub Releases?"),
                        "Tarjimon Office UZ",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                        OpenUpdatePage();
                }
                else
                {
                    SetStatus(GetText(
                        "Sizda eng so'nggi versiya o'rnatilgan.",
                        "Установлена последняя версия.",
                        "Your version is up to date."));
                }
            }
            catch (TaskCanceledException)
            {
                SetStatus(GetText(
                    "Tekshiruv vaqti tugadi. Internet ulanishini tekshiring.",
                    "Время проверки истекло. Проверьте подключение к Интернету.",
                    "The update check timed out. Check your internet connection."));
            }
            catch (Exception)
            {
                SetStatus(GetText(
                    "Yangilanishni tekshirib bo'lmadi. Internet ulanishini tekshiring.",
                    "Не удалось проверить обновления. Проверьте подключение к Интернету.",
                    "Could not check for updates. Check your internet connection."));
            }
            finally
            {
                btnCheckUpdate.Enabled = true;
            }
        }

        private string ExtractVersionTag(Uri uri)
        {
            if (uri == null)
                return string.Empty;

            string[] parts = uri.AbsolutePath.Trim('/').Split('/');
            if (parts.Length == 0)
                return string.Empty;

            string value = parts[parts.Length - 1];
            if (value.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                value = value.Substring(1);
            return value;
        }

        private string GetText(string uz, string ru, string en)
        {
            switch (SettingsManager.Current.Language)
            {
                case "ru": return ru;
                case "en": return en;
                default: return uz;
            }
        }

        private void SetStatus(string text)
        {
            lblStatusValue.Text = text;
        }

        private void OpenUpdatePage()
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = Constants.UpdateUrl,
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show(
                    GetText("GitHub Releases sahifasini ochib bo'lmadi.", "Не удалось открыть GitHub Releases.", "Could not open GitHub Releases."),
                    "Tarjimon Office UZ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void lblVersion_Click(object sender, EventArgs e)
        {
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            FindForm()?.Close();
        }
    }
}