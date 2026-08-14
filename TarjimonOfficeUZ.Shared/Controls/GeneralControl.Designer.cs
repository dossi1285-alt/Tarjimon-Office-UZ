namespace TarjimonOfficeUZ.Shared.Controls
{
    partial class GeneralControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblStartup = new System.Windows.Forms.Label();
            this.chkWordStartup = new System.Windows.Forms.CheckBox();
            this.chkExcelStartup = new System.Windows.Forms.CheckBox();
            this.chkAutoCheckUpdates = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // lblStartup
            this.lblStartup.AutoSize = true;
            this.lblStartup.Location = new System.Drawing.Point(20, 20);
            this.lblStartup.Name = "lblStartup";
            this.lblStartup.Size = new System.Drawing.Size(120, 13);
            this.lblStartup.TabIndex = 0;
            this.lblStartup.Text = "Office ishga tushishi";
            // chkWordStartup
            this.chkWordStartup.AutoSize = true;
            this.chkWordStartup.Location = new System.Drawing.Point(20, 50);
            this.chkWordStartup.Name = "chkWordStartup";
            this.chkWordStartup.Size = new System.Drawing.Size(220, 17);
            this.chkWordStartup.TabIndex = 1;
            this.chkWordStartup.Text = "Microsoft Word bilan ishga tushirish";
            this.chkWordStartup.UseVisualStyleBackColor = true;
            // chkExcelStartup
            this.chkExcelStartup.AutoSize = true;
            this.chkExcelStartup.Location = new System.Drawing.Point(20, 80);
            this.chkExcelStartup.Name = "chkExcelStartup";
            this.chkExcelStartup.Size = new System.Drawing.Size(225, 17);
            this.chkExcelStartup.TabIndex = 2;
            this.chkExcelStartup.Text = "Microsoft Excel bilan ishga tushirish";
            this.chkExcelStartup.UseVisualStyleBackColor = true;
            // chkAutoCheckUpdates
            this.chkAutoCheckUpdates.AutoSize = true;
            this.chkAutoCheckUpdates.Location = new System.Drawing.Point(20, 110);
            this.chkAutoCheckUpdates.Name = "chkAutoCheckUpdates";
            this.chkAutoCheckUpdates.Size = new System.Drawing.Size(250, 17);
            this.chkAutoCheckUpdates.TabIndex = 3;
            this.chkAutoCheckUpdates.Text = "Yangilanishlarni avtomatik tekshirish";
            this.chkAutoCheckUpdates.UseVisualStyleBackColor = true;
            // btnSave
            this.btnSave.Location = new System.Drawing.Point(20, 220);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Saqlash";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(130, 220);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Bekor qilish";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // GeneralControl
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkAutoCheckUpdates);
            this.Controls.Add(this.chkExcelStartup);
            this.Controls.Add(this.chkWordStartup);
            this.Controls.Add(this.lblStartup);
            this.Name = "GeneralControl";
            this.Size = new System.Drawing.Size(400, 335);
            this.Load += new System.EventHandler(this.GeneralControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblStartup;
        private System.Windows.Forms.CheckBox chkWordStartup;
        private System.Windows.Forms.CheckBox chkExcelStartup;
        private System.Windows.Forms.CheckBox chkAutoCheckUpdates;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}