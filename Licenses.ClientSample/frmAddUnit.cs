using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Licenses.ClientSample
{
    public partial class frmAddUnit : Form
    {
        public frmAddUnit()
        {
            InitializeComponent();
        }

        public bool Canceled { get; set; } = true;

        public string UnitName { get; set; }

        public int UnitCount { get; set; }

        private void Button_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender == btnCancel)
                {
                    Cancel();
                }
                else if (sender == btnOK)
                {
                    Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void Cancel()
        {
            this.Canceled = true;
            this.Close();
        }

        void Save()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a Unit Name");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCount.Text))
            {
                MessageBox.Show("Please enter a Unit Count");
                return;
            }

            int cnt;
            if (!int.TryParse(txtCount.Text, out cnt) || cnt < 0)
            {
                MessageBox.Show("Please enter a positive integer value for the Unit Count");
                return;
            }

            this.UnitName = txtName.Text.Trim();
            this.UnitCount = cnt;
            this.Canceled = false;
            this.Close();
        }
    }
}
