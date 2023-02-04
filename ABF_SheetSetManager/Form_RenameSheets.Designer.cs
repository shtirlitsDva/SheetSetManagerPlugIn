namespace ABF_SheetSetManager
{
    partial class Form_RenameSheets
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_ProjectNumber = new System.Windows.Forms.TextBox();
            this.textBox_PhaseNumber = new System.Windows.Forms.TextBox();
            this.textBox_SheetTypeNumber = new System.Windows.Forms.TextBox();
            this.button_RenameAndRenumber = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.71937F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.28063F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox_ProjectNumber, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox_PhaseNumber, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox_SheetTypeNumber, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.button_RenameAndRenumber, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.button_Cancel, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1265, 229);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label3.Location = new System.Drawing.Point(3, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(332, 50);
            this.label3.TabIndex = 3;
            this.label3.Text = "Sheet type number:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label2.Location = new System.Drawing.Point(3, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(332, 50);
            this.label2.TabIndex = 2;
            this.label2.Text = "Phase (etape):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(332, 50);
            this.label1.TabIndex = 1;
            this.label1.Text = "Project number:";
            // 
            // textBox_ProjectNumber
            // 
            this.textBox_ProjectNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_ProjectNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.textBox_ProjectNumber.Location = new System.Drawing.Point(341, 3);
            this.textBox_ProjectNumber.Multiline = true;
            this.textBox_ProjectNumber.Name = "textBox_ProjectNumber";
            this.textBox_ProjectNumber.Size = new System.Drawing.Size(921, 44);
            this.textBox_ProjectNumber.TabIndex = 0;
            // 
            // textBox_PhaseNumber
            // 
            this.textBox_PhaseNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_PhaseNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.textBox_PhaseNumber.Location = new System.Drawing.Point(341, 53);
            this.textBox_PhaseNumber.Multiline = true;
            this.textBox_PhaseNumber.Name = "textBox_PhaseNumber";
            this.textBox_PhaseNumber.Size = new System.Drawing.Size(921, 44);
            this.textBox_PhaseNumber.TabIndex = 0;
            // 
            // textBox_SheetTypeNumber
            // 
            this.textBox_SheetTypeNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_SheetTypeNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.textBox_SheetTypeNumber.Location = new System.Drawing.Point(341, 103);
            this.textBox_SheetTypeNumber.Multiline = true;
            this.textBox_SheetTypeNumber.Name = "textBox_SheetTypeNumber";
            this.textBox_SheetTypeNumber.Size = new System.Drawing.Size(921, 44);
            this.textBox_SheetTypeNumber.TabIndex = 0;
            // 
            // button_RenameAndRenumber
            // 
            this.button_RenameAndRenumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_RenameAndRenumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.button_RenameAndRenumber.Location = new System.Drawing.Point(3, 153);
            this.button_RenameAndRenumber.Name = "button_RenameAndRenumber";
            this.button_RenameAndRenumber.Size = new System.Drawing.Size(332, 73);
            this.button_RenameAndRenumber.TabIndex = 4;
            this.button_RenameAndRenumber.Text = "Rename and renumber";
            this.button_RenameAndRenumber.UseVisualStyleBackColor = true;
            this.button_RenameAndRenumber.Click += new System.EventHandler(this.button_RenameAndRenumber_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.button_Cancel.Location = new System.Drawing.Point(341, 153);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(921, 73);
            this.button_Cancel.TabIndex = 5;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // Form_RenameSheets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1265, 229);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form_RenameSheets";
            this.Text = "Form_RenameSheets";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBox_ProjectNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_PhaseNumber;
        private System.Windows.Forms.TextBox textBox_SheetTypeNumber;
        private System.Windows.Forms.Button button_RenameAndRenumber;
        private System.Windows.Forms.Button button_Cancel;
    }
}