using System;
using System.Drawing;
using System.Windows.Forms;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class ModuleInitializerAttribute : Attribute { }
}

namespace TarjimonOfficeUZ.Setup.Preflight
{
    internal static class DesignRuntime
    {
        private static bool applied;

        [System.Runtime.CompilerServices.ModuleInitializer]
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
                Apply(form);
                applied = true;
                Application.Idle -= ApplyWhenReady;
                return;
            }
        }

        private static void Apply(Control root)
        {
            foreach (Control control in root.Controls)
            {
                if (control is Button button)
                {
                    if (string.Equals(button.Text, "Tasdiqlash", StringComparison.OrdinalIgnoreCase))
                    {
                        button.BackColor = Color.FromArgb(46, 125, 50);
                        button.ForeColor = Color.White;
                        button.FlatStyle = FlatStyle.Flat;
                        button.FlatAppearance.BorderSize = 0;
                    }
                    else if (string.Equals(button.Text, "Bekor qilish", StringComparison.OrdinalIgnoreCase))
                    {
                        button.BackColor = Color.FromArgb(30, 105, 190);
                        button.ForeColor = Color.White;
                        button.FlatStyle = FlatStyle.Flat;
                        button.FlatAppearance.BorderSize = 0;
                    }
                }

                if (control is Label label && string.Equals(label.Text, "Office tarjimonlarini aniqlash", StringComparison.OrdinalIgnoreCase))
                    label.ForeColor = Color.FromArgb(46, 125, 50);

                if (control is Panel panel && HasActionButtons(panel))
                {
                    panel.Height = 48;
                    panel.Padding = new Padding(18, 7, 18, 7);
                    foreach (Control child in panel.Controls)
                        if (child is Button b) b.Top = 7;
                }

                if (control.HasChildren) Apply(control);
            }
        }

        private static bool HasActionButtons(Control control)
        {
            var hasOk = false;
            var hasCancel = false;
            foreach (Control child in control.Controls)
            {
                if (child is Button button)
                {
                    hasOk |= string.Equals(button.Text, "Tasdiqlash", StringComparison.OrdinalIgnoreCase);
                    hasCancel |= string.Equals(button.Text, "Bekor qilish", StringComparison.OrdinalIgnoreCase);
                }
            }
            return hasOk && hasCancel;
        }
    }
}
