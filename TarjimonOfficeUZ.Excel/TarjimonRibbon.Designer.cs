namespace TarjimonOfficeUZ.Excel
{
    partial class TarjimonRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public TarjimonRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
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
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.grpTarjimon);
            this.tab1.Label = "TabAddIns";
            this.tab1.Name = "tab1";
            // 
            // grpTarjimon
            // 
            this.grpTarjimon.Items.Add(this.btnLatinToCyrillic);
            this.grpTarjimon.Items.Add(this.btnCyrillicToLatin);
            this.grpTarjimon.Items.Add(this.btnADX);
            this.grpTarjimon.Label = "Tarjimon Office UZ";
            this.grpTarjimon.Name = "grpTarjimon";
            // 
            // btnLatinToCyrillic
            // 
            this.btnLatinToCyrillic.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnLatinToCyrillic.Label = "Lotin → Kirill";
            this.btnLatinToCyrillic.Name = "btnLatinToCyrillic";
            this.btnLatinToCyrillic.ScreenTip = "Lotin matnini kirill yozuviga o‘giradi";
            this.btnLatinToCyrillic.ShowImage = true;
            this.btnLatinToCyrillic.SuperTip = "Tanlangan katak yoki matnni lotin yozuvidan kirill yozuviga transliteratsiya qila" +
    "di.";
            this.btnLatinToCyrillic.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnLatinToCyrillic_Click);
            // 
            // btnCyrillicToLatin
            // 
            this.btnCyrillicToLatin.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnCyrillicToLatin.Label = "Kirill → Lotin";
            this.btnCyrillicToLatin.Name = "btnCyrillicToLatin";
            this.btnCyrillicToLatin.ScreenTip = "Kirill matnini lotin yozuviga o‘giradi";
            this.btnCyrillicToLatin.ShowImage = true;
            this.btnCyrillicToLatin.SuperTip = "Tanlangan katak yoki matnni kirill yozuvidan lotin yozuviga transliteratsiya qila" +
    "di.";
            this.btnCyrillicToLatin.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCyrillicToLatin_Click);
            // 
            // btnADX
            // 
            this.btnADX.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnADX.Label = "ADX";
            this.btnADX.Name = "btnADX";
            this.btnADX.ShowImage = true;
            this.btnADX.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnADX_Click);
            // 
            // TarjimonRibbon
            // 
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

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpTarjimon;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnLatinToCyrillic;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCyrillicToLatin;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnADX;
    }

    partial class ThisRibbonCollection
    {
        internal TarjimonRibbon TarjimonRibbon
        {
            get { return this.GetRibbon<TarjimonRibbon>(); }
        }
    }
}
