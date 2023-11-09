namespace ABF_SheetSetManager
{
    partial class Form_RenameSheetsNS
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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Projekt = new System.Windows.Forms.TextBox();
            this.textBox_Etape = new System.Windows.Forms.TextBox();
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
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox_Projekt, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox_Etape, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.button_RenameAndRenumber, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.button_Cancel, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(632, 119);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label2.Location = new System.Drawing.Point(2, 26);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "Etape:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label1.Location = new System.Drawing.Point(2, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "Projekt nr.:";
            // 
            // textBox_Program
            // 
            this.textBox_Projekt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Projekt.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.textBox_Projekt.Location = new System.Drawing.Point(170, 2);
            this.textBox_Projekt.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_Projekt.Multiline = true;
            this.textBox_Projekt.Name = "textBox_Program";
            this.textBox_Projekt.Size = new System.Drawing.Size(460, 22);
            this.textBox_Projekt.TabIndex = 0;
            // 
            // textBox_VFkommunekode
            // 
            this.textBox_Etape.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Etape.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.textBox_Etape.Location = new System.Drawing.Point(170, 28);
            this.textBox_Etape.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_Etape.Multiline = true;
            this.textBox_Etape.Name = "textBox_VFkommunekode";
            this.textBox_Etape.Size = new System.Drawing.Size(460, 22);
            this.textBox_Etape.TabIndex = 0;
            // 
            // button_RenameAndRenumber
            // 
            this.button_RenameAndRenumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_RenameAndRenumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.button_RenameAndRenumber.Location = new System.Drawing.Point(2, 54);
            this.button_RenameAndRenumber.Margin = new System.Windows.Forms.Padding(2);
            this.button_RenameAndRenumber.Name = "button_RenameAndRenumber";
            this.button_RenameAndRenumber.Size = new System.Drawing.Size(164, 63);
            this.button_RenameAndRenumber.TabIndex = 4;
            this.button_RenameAndRenumber.Text = "Rename and renumber";
            this.button_RenameAndRenumber.UseVisualStyleBackColor = true;
            this.button_RenameAndRenumber.Click += new System.EventHandler(this.button_RenameAndRenumber_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.button_Cancel.Location = new System.Drawing.Point(170, 54);
            this.button_Cancel.Margin = new System.Windows.Forms.Padding(2);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(460, 63);
            this.button_Cancel.TabIndex = 5;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // Form_RenameSheetsNS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 119);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form_RenameSheetsNS";
            this.Text = "Form_RenameSheets";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBox_Projekt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Etape;
        private System.Windows.Forms.Button button_RenameAndRenumber;
        private System.Windows.Forms.Button button_Cancel;
    }
}