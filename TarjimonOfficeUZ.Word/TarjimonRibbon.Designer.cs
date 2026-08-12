namespace TarjimonOfficeUZ.Word
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
            this.btnMenu = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.grpTarjimon.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.grpTarjimon);
            this.tab1.Label = "KL Officce uz";
            this.tab1.Name = "tab1";
            // 
            // grpTarjimon
            // 
            this.grpTarjimon.Items.Add(this.btnLatinToCyrillic);
            this.grpTarjimon.Items.Add(this.btnCyrillicToLatin);
            this.grpTarjimon.Items.Add(this.btnMenu);
            this.grpTarjimon.Label = "KL Office";
            this.grpTarjimon.Name = "grpTarjimon";
            // 
            // btnLatinToCyrillic
            // 
            this.btnLatinToCyrillic.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnLatinToCyrillic.Label = "Latin → Кирилл";
            this.btnLatinToCyrillic.Name = "btnLatinToCyrillic";
            this.btnLatinToCyrillic.ShowImage = true;
            this.btnLatinToCyrillic.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnLatinToCyrillic_Click);
            // 
            // btnCyrillicToLatin
            // 
            this.btnCyrillicToLatin.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnCyrillicToLatin.Label = "Кирилл → Latin";
            this.btnCyrillicToLatin.Name = "btnCyrillicToLatin";
            this.btnCyrillicToLatin.ShowImage = true;
            this.btnCyrillicToLatin.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCyrillicToLatin_Click);
            // 
            // btnMenu
            // 
            this.btnMenu.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnMenu.Label = "▼";
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.ShowImage = true;
            this.btnMenu.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnMenu_Click);
            // 
            // TarjimonRibbon
            // 
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
