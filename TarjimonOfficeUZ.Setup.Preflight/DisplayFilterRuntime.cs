using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace TarjimonOfficeUZ.Setup.Preflight
{
    // 1.0 FINALIZATION: display-only filter.
    // Deliberately does not modify discovery, duplicate merge, uninstall, or installer logic.
    internal static class DisplayFilterRuntime
    {
        private static bool applied;

        [ModuleInitializer]
        internal static void Initialize()
        {
            Application.Idle += ApplyWhenReady;
        }

        private static void ApplyWhenReady(object sender, EventArgs e)
        {
            if (applied) return;

            foreach (Form form in Application.OpenForms)
            {
                if (!string.Equals(form.GetType().Name, "ReviewForm", StringComparison.Ordinal)) continue;
                var list = FindListView(form);
                if (list == null || list.Items.Count == 0) continue;

                Filter(list);
                applied = true;
                Application.Idle -= ApplyWhenReady;
                return;
            }
        }

        private static ListView FindListView(Control root)
        {
            foreach (Control child in root.Controls)
            {
                var list = child as ListView;
                if (list != null) return list;
                if (child.HasChildren)
                {
                    var nested = FindListView(child);
                    if (nested != null) return nested;
                }
            }
            return null;
        }

        private static void Filter(ListView list)
        {
            var remove = new List<ListViewItem>();
            foreach (ListViewItem row in list.Items)
            {
                var candidate = row.Tag as AddinCandidate;
                if (candidate == null) continue;
                if (!IsDisplayTranslatorCandidate(candidate)) remove.Add(row);
            }

            foreach (var row in remove) list.Items.Remove(row);
        }

        private static bool IsDisplayTranslatorCandidate(AddinCandidate item)
        {
            if (item == null) return false;
            if (item.IsOwnProduct) return true;

            var product = Normalize(item.Product);
            var publisher = Normalize(item.Publisher);
            var evidence = Normalize(item.Evidence);
            var host = Normalize(item.Host);
            var all = product + " " + publisher + " " + evidence;

            // Explicit translator/transliterator products.
            if (ContainsAny(all,
                "translit", "transliteration", "transliterator", "translator", "translation",
                "tarjimon", "переводчик", "перевод", "savodxon", "preslovljanje", "preslovljavanje"))
                return true;

            // Explicit Cyrillic/Latin conversion products. A single generic 'convert' is not enough.
            var cyrillic = ContainsAny(all, "kirill", "kiril", "cyrillic", "кирилл");
            var latin = ContainsAny(all, "lotin", "latin", "латин");
            if (cyrillic && latin) return true;

            // Office add-ins with a strong translation/conversion signal.
            if (ContainsAny(host, "word", "excel", "office") &&
                ContainsAny(all, "translate", "translation", "translator", "translit", "transliteration", "tarjimon", "перевод"))
                return true;

            // Known technical Office components and ordinary applications are never display candidates.
            if (ContainsAny(all,
                "microsoft office", "office mui", "proofing tools", "proofingtool", "shared features",
                "shared components", "office shared", "visual studio", "visual studio tools",
                "github", "git", "google chrome", "chrome", "mozilla firefox", "firefox",
                "telegram", "lightshot", "7 zip", "7zip", "winrar", "easeus", "silverlight",
                "runtime", "redistributable", "developer tools", "sdk", "browser"))
                return false;

            // Generic converter/convert terms alone must never make a program a translator.
            if (ContainsAny(all, "converter", "conversion", "convert")) return false;

            // Unknown low-confidence candidates are hidden; the discovery engine remains unchanged.
            return false;
        }

        private static bool ContainsAny(string text, params string[] values)
        {
            foreach (var value in values)
                if (text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return false;
        }

        private static string Normalize(string value)
        {
            return (value ?? string.Empty).Replace("-", " ").Replace("_", " ").ToLowerInvariant();
        }
    }
}
