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
    public partial class frmAddProduct : Form
    {
        bool _canceled = true;

        public frmAddProduct()
        {
            InitializeComponent();
        }

        public bool Canceled { get { return _canceled; } }

        public string Product { get; set; }

        private void Button_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender == btnOK)
                {
                    if (string.IsNullOrWhiteSpace(txtProductName.Text))
                        throw new ApplicationException("Product name cannot be empty");

                    _canceled = false;
                    Product = txtProductName.Text.Trim();
                    Close();
                }
                else if (sender == btnCancel)
                {
                    _canceled = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
