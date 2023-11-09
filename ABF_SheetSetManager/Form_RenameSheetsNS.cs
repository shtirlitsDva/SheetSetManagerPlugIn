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
    public partial class Form_RenameSheetsNS : Form
    {
        public bool RenameAndRenumber = false;
        public string Projekt = string.Empty;
        public string Etape = string.Empty;
        public Form_RenameSheetsNS()
        {
            InitializeComponent();
        }

        private void button_RenameAndRenumber_Click(object sender, EventArgs e)
        {
            RenameAndRenumber = true;
            Projekt = textBox_Projekt.Text;
            Etape = textBox_Etape.Text;
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
