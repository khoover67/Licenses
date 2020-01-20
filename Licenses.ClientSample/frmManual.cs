using Licenses.Library.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Windows.Forms;

namespace Licenses.ClientSample
{
    public partial class frmManual : Form
    {
        string _connectionString = "";

        public frmManual()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                cboDatabase.Items.Clear();
                for (int i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
                    if (ConfigurationManager.ConnectionStrings[i].ProviderName == "INFORMIX")
                        cboDatabase.Items.Add(ConfigurationManager.ConnectionStrings[i].Name);

                txtDate.Text = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cboDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string name = cboDatabase.Text.Trim();
                _connectionString = ConfigurationManager.ConnectionStrings[name].ConnectionString;
                using (OdbcConnection con = new OdbcConnection(_connectionString))
                {
                    con.Open();
                    txtDatabasePath.Text = con.Database;
                    using (OdbcCommand cmd = new OdbcCommand("select con_name from co_company_name", con))
                    {
                        using (OdbcDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtClientName.Text = rdr["con_name"] is System.DBNull ? "<No Name Found>" : rdr["con_name"].ToString().Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender == btnExit)
                {
                    Close();
                }
                else if (sender == btnAddUnit)
                {
                    AddUnit();
                }
                else if (sender == btnDeleteUnit)
                {
                    DeleteUnit();
                }
                else if (sender == btnClear)
                {

                }
                else if (sender == btnPost)
                {
                    Post();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void AddUnit()
        {
            using (var frm = new frmAddUnit())
            {
                frm.ShowDialog(this);
                if (frm.Canceled)
                    return;

                string[] arr = new string[] { frm.UnitName, frm.UnitCount.ToString() };
                lvwUnits.Items.Add(new ListViewItem(arr));
            }
        }

        void DeleteUnit()
        {
            if (lvwUnits.SelectedIndices.Count < 1)
            {
                MessageBox.Show("No count was selected");
                return;
            }

            int idx = lvwUnits.SelectedIndices[0];
            lvwUnits.Items.RemoveAt(idx);
        }

        void Post()
        {
            string client = txtClientName.Text.Trim();
            string database = txtDatabasePath.Text.Trim();
            string product = txtProductName.Text.Trim();
            string url = txtServerAddress.Text.Trim();
            string date = txtDate.Text;
            if (string.IsNullOrWhiteSpace(client))
                throw new ApplicationException("Empty Client Name");
            if (string.IsNullOrWhiteSpace(database))
                throw new ApplicationException("Empty Database Path");
            if (string.IsNullOrWhiteSpace(product))
                throw new ApplicationException("Empty Product Name");
            if (string.IsNullOrWhiteSpace(date))
                throw new ApplicationException("Empty Date");

            DateTime dt;
            if (!DateTime.TryParse(date, out dt))
                throw new ApplicationException("Invalid Date");
            
            List<UpdateUnitDTO> units = new List<UpdateUnitDTO>();
            for (int i = 0; i < lvwUnits.Items.Count; i++)
            {
                string name = lvwUnits.Items[i].Text;
                int count = int.Parse(lvwUnits.Items[i].SubItems[1].Text);
                units.Add(new UpdateUnitDTO { UnitName = name, UnitCount = count });
            }

            if (units.Count < 1)
                throw new ApplicationException("No units were specified");

            UpdateCountDTO dto = Library.Client.Tools.CreateUpdateCountDTO(_connectionString, client, product, units, dt);
            ResultDTO result = Library.Server.Tools.PostToServer(dto, url, false);
            if (result.Message == ResultDTO.INTERVAL)
                MessageBox.Show("Interval not reached");
            else
                MessageBox.Show("Complete");
        }
    }
}
