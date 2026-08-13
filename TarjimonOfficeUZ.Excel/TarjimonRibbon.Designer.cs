namespace TarjimonOfficeUZ.Excel
{
    partial class TarjimonRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        private System.ComponentModel.IContainer components = null;

        public TarjimonRibbon() : base(Globals.Factory.GetRibbonFactory()) { InitializeComponent(); }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tab1 = this.Factory.CreateRibbonTab();
            this.grpTarjimon = this.Factory.CreateRibbonGroup();
            this.btnLatinToCyrillic = this.Factory.CreateRibbonButton();
            this.btnCyrillicToLatin = this.Factory.CreateRibbonButton();
            this.btnADX = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.grpTarjimon.SuspendLayout();
            this.SuspendLayout();

            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.grpTarjimon);
            this.tab1.Label = "KL Officce uz";
            this.tab1.Name = "tab1";

            this.grpTarjimon.Items.Add(this.btnLatinToCyrillic);
            this.grpTarjimon.Items.Add(this.btnCyrillicToLatin);
            this.grpTarjimon.Items.Add(this.btnADX);
            this.grpTarjimon.Label = "KL Office";
            this.grpTarjimon.Name = "grpTarjimon";

            this.btnLatinToCyrillic.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnLatinToCyrillic.Label = "Lotin → Kirill";
            this.btnLatinToCyrillic.Name = "btnLatinToCyrillic";
            this.btnLatinToCyrillic.ScreenTip = "Lotin matnini kirill yozuviga o‘giradi";
            this.btnLatinToCyrillic.ShowImage = false;
            this.btnLatinToCyrillic.ShowLabel = true;
            this.btnLatinToCyrillic.SuperTip = "Lotin → Kirill";
            this.btnLatinToCyrillic.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnLatinToCyrillic_Click);

            this.btnCyrillicToLatin.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnCyrillicToLatin.Label = "Kirill → Lotin";
            this.btnCyrillicToLatin.Name = "btnCyrillicToLatin";
            this.btnCyrillicToLatin.ScreenTip = "Kirill matnini lotin yozuviga o‘giradi";
            this.btnCyrillicToLatin.ShowImage = false;
            this.btnCyrillicToLatin.ShowLabel = true;
            this.btnCyrillicToLatin.SuperTip = "Kirill → Lotin";
            this.btnCyrillicToLatin.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCyrillicToLatin_Click);

            this.btnADX.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnADX.Label = "";
            this.btnADX.Name = "btnADX";
            this.btnADX.ScreenTip = "Sozlamalar";
            this.btnADX.ShowImage = true;
            this.btnADX.ShowLabel = false;
            this.btnADX.SuperTip = "Sozlamalar menyusi";
            this.btnADX.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnADX_Click);

            this.Name = "TarjimonRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.TarjimonRibbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.grpTarjimon.ResumeLayout(false);
            this.grpTarjimon.PerformLayout();
            this.ResumeLayout(false);
        }

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpTarjimon;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnLatinToCyrillic;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCyrillicToLatin;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnADX;
    }

    partial class ThisRibbonCollection
    {
        internal TarjimonRibbon Ribbon1 { get { return this.GetRibbon<TarjimonRibbon>(); } }
    }
}
