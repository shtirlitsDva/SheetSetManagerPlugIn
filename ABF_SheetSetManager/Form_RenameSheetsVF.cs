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
    public partial class Form_RenameSheetsVF : Form
    {
        public bool RenameAndRenumber = false;
        public string Program = string.Empty;
        public string VFkommunekode = string.Empty;
        public string Energidistrikt = string.Empty;
        public Form_RenameSheetsVF()
        {
            InitializeComponent();
        }

        private void button_RenameAndRenumber_Click(object sender, EventArgs e)
        {
            RenameAndRenumber = true;
            Program = textBox_Program.Text;
            VFkommunekode = textBox_VFkommunekode.Text;
            Energidistrikt= textBox_Energidistrikt.Text;
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
