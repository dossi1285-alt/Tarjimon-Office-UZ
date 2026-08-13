namespace TarjimonOfficeUZ.Word
{
    partial class TarjimonRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        private System.ComponentModel.IContainer components = null;

        public TarjimonRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        private void InitializeComponent()
        {
            this.tab1 = this.Factory.CreateRibbonTab();
            this.grpTarjimon = this.Factory.CreateRibbonGroup();
            this.btnLatinToCyrillic = this.Factory.CreateRibbonButton();
            this.btnCyrillicToLatin = this.Factory.CreateRibbonButton();
            this.btnMenu = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.grpTarjimon.SuspendLayout();
            this.SuspendLayout();

            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.grpTarjimon);
            this.tab1.Label = "KL Officce uz";
            this.tab1.Name = "tab1";

            this.grpTarjimon.Items.Add(this.btnLatinToCyrillic);
            this.grpTarjimon.Items.Add(this.btnCyrillicToLatin);
            this.grpTarjimon.Items.Add(this.btnMenu);
            this.grpTarjimon.Label = "KL Office";
            this.grpTarjimon.Name = "grpTarjimon";

            this.btnLatinToCyrillic.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnLatinToCyrillic.Label = "";
            this.btnLatinToCyrillic.Name = "btnLatinToCyrillic";
            this.btnLatinToCyrillic.ScreenTip = "Lotin matnini kirill yozuviga o‘giradi";
            this.btnLatinToCyrillic.ShowImage = true;
            this.btnLatinToCyrillic.ShowLabel = false;
            this.btnLatinToCyrillic.SuperTip = "Lotin → Kirill";
            this.btnLatinToCyrillic.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnLatinToCyrillic_Click);

            this.btnCyrillicToLatin.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnCyrillicToLatin.Label = "";
            this.btnCyrillicToLatin.Name = "btnCyrillicToLatin";
            this.btnCyrillicToLatin.ScreenTip = "Kirill matnini lotin yozuviga o‘giradi";
            this.btnCyrillicToLatin.ShowImage = true;
            this.btnCyrillicToLatin.ShowLabel = false;
            this.btnCyrillicToLatin.SuperTip = "Kirill → Lotin";
            this.btnCyrillicToLatin.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCyrillicToLatin_Click);

            this.btnMenu.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnMenu.Label = "";
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.ScreenTip = "Sozlamalar";
            this.btnMenu.ShowImage = true;
            this.btnMenu.ShowLabel = false;
            this.btnMenu.SuperTip = "Sozlamalar menyusi";
            this.btnMenu.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnMenu_Click);

            this.Name = "TarjimonRibbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon1_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.grpTarjimon.ResumeLayout(false);
            this.grpTarjimon.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpTarjimon;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnLatinToCyrillic;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCyrillicToLatin;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnMenu;
    }

    partial class ThisRibbonCollection
    {
        internal TarjimonRibbon Ribbon1
        {
            get { return this.GetRibbon<TarjimonRibbon>(); }
        }
    }
}
