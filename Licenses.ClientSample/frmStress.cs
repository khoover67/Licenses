using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Licenses.ClientSample
{
    public partial class frmStress : Form
    {
        public frmStress()
        {
            InitializeComponent();
        }

        private void btnRandom_Click(object sender, EventArgs e)
        {
            try
            {
                RunRandom();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void RunRandom()
        {
            //DateTime dtStart = DateTime.Now;
            //int loop;
            //if (!int.TryParse(txtTestNum.Text, out loop) || loop < 1)
            //    throw new ApplicationException("Number of loops must be positive integer greater than 0");

            //lstResults.Items.Clear();

            //string[] databases = new string[] { "xxx/yyy/factor", "factor/blah/factor" };

            //Random random = new Random(Environment.TickCount);

            //for (int i = 0; i< loop; i++)
            //{
            //    string dbName = databases[random.Next(0, databases.Length)];
            //    string clientName = "Client " + random.Next(1, 301).ToString().PadLeft(3, '0');
            //    ClientModel client = new ClientModel { Name = clientName, DatabasePath = dbName };

            //    int prodCount = random.Next(3) + 1;

            //    for(int p = 1; p <= prodCount; p++)
            //    {
            //        string productName = "Product " + random.Next(1, 11).ToString().PadLeft(2, '0');
            //        ProductModel product = new ProductModel { Name = productName };
            //        client.Products.Add(product);

            //        int unitCount = random.Next(1, 30);
            //        for (int u = 1; u <= unitCount; u++)
            //        {
            //            string detailName = "Unit " + u.ToString().PadLeft(2, '0');
            //            ProductDetailModel detail = new ProductDetailModel { Name = detailName, Count = random.Next(1, 26) };
            //            product.Details.Add(detail);
            //        }
            //    }

            //    string url = txtAddress.Text.Trim();
            //    Licenses.Library.Server.PostToServer(client, url, "admin");
            //    if (lstResults.Items.Count == 0)
            //        lstResults.Items.Add(i + " - Complete");
            //    else
            //        lstResults.Items.Insert(0, i + " - Complete");

            //    Application.DoEvents();
            //}

            //TimeSpan ts = DateTime.Now.Subtract(dtStart);

            //MessageBox.Show($"Test Complete\r\n\r\n Duration: {ts.TotalSeconds} Seconds");
        }
    }
}
