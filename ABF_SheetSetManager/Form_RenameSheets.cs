using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ABF_SheetSetManager
{
    public partial class Form_RenameSheets : Form
    {
        public bool RenameAndRenumber = false;
        public string projectNumber = string.Empty;
        public string etapeNumber = string.Empty;
        public string sheetTypeNumber = string.Empty;
        public Form_RenameSheets()
        {
            InitializeComponent();
        }

        private void button_RenameAndRenumber_Click(object sender, EventArgs e)
        {
            RenameAndRenumber = true;
            projectNumber = textBox_ProjectNumber.Text;
            etapeNumber = textBox_PhaseNumber.Text;
            sheetTypeNumber= textBox_SheetTypeNumber.Text;
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
