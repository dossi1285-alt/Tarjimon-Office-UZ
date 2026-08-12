namespace TarjimonOfficeUZ.Shared.Controls
{
    partial class GeneralControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.lblStartup = new System.Windows.Forms.Label();
            this.chkWordStartup = new System.Windows.Forms.CheckBox();
            this.chkExcelStartup = new System.Windows.Forms.CheckBox();
                       
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblStartup
            // 
            this.lblStartup.AutoSize = true;
            this.lblStartup.Location = new System.Drawing.Point(20, 20);
            this.lblStartup.Name = "lblStartup";
            this.lblStartup.Size = new System.Drawing.Size(41, 13);
            this.lblStartup.TabIndex = 0;
            this.lblStartup.Text = "Startup";
            // 
            // chkWordStartup
            // 
            this.chkWordStartup.AutoSize = true;
            this.chkWordStartup.Location = new System.Drawing.Point(20, 50);
            this.chkWordStartup.Name = "chkWordStartup";
            this.chkWordStartup.Size = new System.Drawing.Size(145, 17);
            this.chkWordStartup.TabIndex = 1;
            this.chkWordStartup.Text = "Start with Microsoft Word";
            this.chkWordStartup.UseVisualStyleBackColor = true;
            // 
            // chkExcelStartup
            // 
            this.chkExcelStartup.AutoSize = true;
            this.chkExcelStartup.Location = new System.Drawing.Point(20, 80);
            this.chkExcelStartup.Name = "chkExcelStartup";
            this.chkExcelStartup.Size = new System.Drawing.Size(145, 17);
            this.chkExcelStartup.TabIndex = 2;
            this.chkExcelStartup.Text = "Start with Microsoft Excel";
            this.chkExcelStartup.UseVisualStyleBackColor = true;
            //                   
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(20, 220);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(130, 220);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // GeneralControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            
            
            this.Controls.Add(this.chkExcelStartup);
            this.Controls.Add(this.chkWordStartup);
            this.Controls.Add(this.lblStartup);
            this.Name = "GeneralControl";
            this.Size = new System.Drawing.Size(273, 335);
            this.Load += new System.EventHandler(this.GeneralControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStartup;
        private System.Windows.Forms.CheckBox chkWordStartup;
        private System.Windows.Forms.CheckBox chkExcelStartup;
        
      
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
