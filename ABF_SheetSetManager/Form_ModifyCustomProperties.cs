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
    public partial class Form_ModifyCustomProperties : Form
    {
        public Dictionary<string, string> PropsAndValues = new Dictionary<string, string>();
        private List<string> PropNames = new List<string>();

        public Form_ModifyCustomProperties(List<string> propNames)
        {
            InitializeComponent();

            PropNames = propNames;

            //comboBox1.DisplayMember = "Name";
            //comboBox1.ValueMember = "ParameterList";
            comboBox1.DataSource = new BindingSource { DataSource = PropNames };

            //comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

            //Initialization of CBs moved to form_load event
        }

        private void addRowMethod(object sender, EventArgs e)
        {
            int rowIndex = tableLayoutPanel1.GetRow((Button)sender);

            //Take the last row and create same one at end of collection
            RowStyle temp = tableLayoutPanel1.RowStyles[tableLayoutPanel1.RowCount - 1];
            tableLayoutPanel1.RowCount++;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(temp.SizeType, temp.Height));

            //Take the previous row and change style
            temp = tableLayoutPanel1.RowStyles[0];
            tableLayoutPanel1.RowStyles[tableLayoutPanel1.RowCount - 2] = new RowStyle(temp.SizeType, temp.Height);

            if (tableLayoutPanel1.RowCount - rowIndex != 3) //= 3 means the control is the last and need not moved
            {
                //Move controls down
                for (int i = tableLayoutPanel1.RowCount - 3; i > rowIndex; i--)
                {
                    for (int j = 0; j < tableLayoutPanel1.ColumnCount; j++)
                    {
                        var control = tableLayoutPanel1.GetControlFromPosition(j, i);
                        if (control != null)
                        {
                            tableLayoutPanel1.SetRow(control, i + 1);
                        }
                    }
                }
            }

            #region Add controls
            //Add controls
            ComboBox cb1 = new ComboBox();
            cb1.Dock = DockStyle.Fill;
            cb1.Anchor = (AnchorStyles)15;
            cb1.DropDownStyle = ComboBoxStyle.DropDownList;
            //cb1.DisplayMember = "Name";
            //cb1.ValueMember = "ParameterList";
            cb1.DataSource = new BindingSource { DataSource = PropNames };
            //cb1.SelectedValue = ListToBindParametersType[0];
            //cb1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            tableLayoutPanel1.Controls.Add(cb1, 0, rowIndex + 1);

            TextBox cb2 = new TextBox();
            cb2.Dock = DockStyle.Fill;
            cb2.Anchor = (AnchorStyles)15;
            tableLayoutPanel1.Controls.Add(cb2, 1, rowIndex + 1);

            //Initialize data for second combo box by firing the associated method of the event
            //comboBox1_SelectedIndexChanged(cb1, new EventArgs());

            //Add buttons
            Button button = new Button() { Dock = DockStyle.Fill, Text = "+" };
            button.Anchor = (AnchorStyles)15;
            button.Click += addRowMethod;

            tableLayoutPanel1.Controls.Add(button, 2, rowIndex + 1);

            button = new Button() { Dock = DockStyle.Fill, Text = "-" };
            button.Anchor = (AnchorStyles)15;
            button.Click += removeRowMethod;

            tableLayoutPanel1.Controls.Add(button, 3, rowIndex + 1);
            #endregion
        }

        private void removeRowMethod(object sender, EventArgs e)
        {
            int rowIndex = tableLayoutPanel1.GetRow((Button)sender);

            if (rowIndex >= tableLayoutPanel1.RowCount)
            {
                return;
            }

            // delete all controls of row that we want to delete
            for (int i = 0; i < tableLayoutPanel1.ColumnCount; i++)
            {
                var control = tableLayoutPanel1.GetControlFromPosition(i, rowIndex);
                tableLayoutPanel1.Controls.Remove(control);
                control.Dispose();
            }

            // move up row controls that comes after row we want to remove
            for (int i = rowIndex + 1; i < tableLayoutPanel1.RowCount; i++)
            {
                for (int j = 0; j < tableLayoutPanel1.ColumnCount; j++)
                {
                    var control = tableLayoutPanel1.GetControlFromPosition(j, i);
                    if (control != null)
                    {
                        tableLayoutPanel1.SetRow(control, i - 1);
                    }
                }
            }

            var removeStyle = tableLayoutPanel1.RowCount - 1;

            if (tableLayoutPanel1.RowStyles.Count > removeStyle)
                tableLayoutPanel1.RowStyles.RemoveAt(removeStyle);

            tableLayoutPanel1.RowCount--;
        }

        private void EditGroupingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int rowIndex = 0; rowIndex < tableLayoutPanel1.RowCount; rowIndex++)
            {
                var comboBox = tableLayoutPanel1.GetControlFromPosition(0, rowIndex) as ComboBox;
                var textBox = tableLayoutPanel1.GetControlFromPosition(1, rowIndex) as TextBox;

                if (comboBox != null && textBox != null)
                {
                    string key = comboBox.SelectedItem.ToString();
                    string value = textBox.Text;

                    if (!PropsAndValues.ContainsKey(key)) // Avoiding duplicate keys
                    {
                        PropsAndValues.Add(key, value);
                    }
                }
            }


            Properties.Settings.Default.Save();
        }
    }
}
