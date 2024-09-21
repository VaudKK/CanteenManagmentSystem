using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using Excel = Microsoft.Office.Interop.Excel;

namespace CanteenManagmentSystem
{
    public partial class FrmViewEmployees : Form
    {
        private BindingSource source = new BindingSource();
        public string Mode = "View";
        bool search = false;
        int Clicked = 0;
        public FrmViewEmployees()
        {
            InitializeComponent();
        }

        private async void FrmViewEmployees_Load(object sender, EventArgs e)
        {
            await GetData("SELECT * FROM tblEmployees WHERE [Status] = 'Active'");
            txtSearch.ForeColor = Color.Gray;
            dataGridView1.DataSource = source;
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                DataGridViewColumn column = dataGridView1.Columns[i];
                column.Width = 200;

            }
            StartPosition = FormStartPosition.CenterScreen;
            bindingNavigator1.BindingSource = source;
            if (Mode == "Edit")
            {
                lblHint.Visible = true;
            }
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;
        }

        private async Task<object> GetData(string query)
        {
            ConnectionString connString = new ConnectionString();
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, sqlConn);
            SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter);
            DataTable table = new DataTable();
            await Task.Run(() => adapter.Fill(table));
            source.DataSource = table;

            return source;
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (Mode == "Edit")
            {
                for (int i = 0; i < Application.OpenForms.Count; i++)
                {
                    if (Application.OpenForms[i].Name == "FrmEmployees")
                    {
                        FrmEmployees employees = (FrmEmployees)Application.OpenForms[i];
                        try { 
                            DataGridViewRow row = dataGridView1.SelectedRows[0];
                            employees.txtID.Text = row.Cells[0].Value.ToString();
                            employees.txtName.Text = row.Cells[1].Value.ToString();
                            employees.txtidno.Text = row.Cells[2].Value.ToString();
                            employees.cboGender.SelectedItem = row.Cells[3].Value.ToString();
                            employees.DOB.Value = DateTime.Parse(row.Cells[4].Value.ToString());
                            employees.txtAddr.Text = row.Cells[5].Value.ToString();
                            employees.txtTel.Text = row.Cells[6].Value.ToString();
                            employees.txtEmail.Text = row.Cells[7].Value.ToString();
                            employees.DOE.Value = DateTime.Parse(row.Cells[8].Value.ToString());
                            employees.btnSave.Text = "Update";
                            employees.OriginalID = row.Cells[0].Value.ToString();
                        }
                        catch (Exception) { employees.btnSave.Text = "Save"; }
                    }
                }

                this.Close();
            }
        }

        private void ExportToolStrip_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog
            {
                Title = "Export",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Excel Files(*.xlsx)|*.xlsx"
            };

            string filename = "Employees" + DateTime.Now.Day.ToString()+ DateTime.Now.Month.ToString()+ DateTime.Now.Day.ToString();
            save.FileName = filename;

            if (save.ShowDialog() != DialogResult.Cancel)
            {
                if (save.FileName != "")
                {
                    this.Cursor = Cursors.WaitCursor;
                    FrmExporting exp = new FrmExporting();
                    exp.Show();
                    try
                    {
                        int rows = 0,
                            columns = 0,
                            i = 0,
                            j = 0,
                            ic = 0;

                        rows = dataGridView1.RowCount - 1;
                        columns = dataGridView1.Columns.Count - 1;
                        Excel.Application app = new Excel.Application();
                        Excel.Workbook book = app.Workbooks.Add();
                        Excel.Worksheet sheet = (Excel.Worksheet)book.Worksheets[1];
                        var current = sheet;
                        current.Columns.Select();
                        current.Columns.Delete();

                        //Exporting the Header Texts
                        for (ic = 0; ic <= columns; ic++)
                        {
                            current.Cells[1, ic + 1].Value = dataGridView1.Columns[ic].HeaderText;
                        }

                        // Exporting the rows
                        for (i = 0; i <= rows - 1; i++)
                        {
                            for (j = 0; j <= columns; j++)
                            {
                                current.Cells[i + 2, j + 1].Value = dataGridView1.Rows[i].Cells[j].Value;
                            }
                        }

                        current.Rows["1:1"].Font.FontStyle = "Bold";
                        current.Rows["1:1"].Font.Size = 12;
                        current.Columns.AutoFit();
                        current.Columns.EntireColumn.AutoFit();
                        current.Cells[1, 1].Select();


                        app.ActiveWorkbook.SaveAs(save.FileName);
                        app.ActiveWorkbook.Saved = true;
                        app.Quit();

                        this.Cursor = Cursors.Arrow;

                        LogsFunction log = new LogsFunction();
                        log.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Employees list exported.File name: " + System.IO.Path.GetFileNameWithoutExtension(save.FileName));

                        exp.Close();
                        DialogResult result;
                        result = MessageBox.Show("Export succeeded.Do you want to open the file?", "Export", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(save.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                        this.Cursor = Cursors.Arrow;
                        exp.Close();
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            Clicked += 1;
            if (Clicked == 1)
            {
                DataGridViewCheckBoxColumn DeleteCol = new DataGridViewCheckBoxColumn();
                DeleteCol.Name = "Delete";
                DeleteCol.HeaderText = "Delete";
                DeleteCol.Width = 40;
                DeleteCol.FillWeight = 10;
                dataGridView1.Columns.Insert(0, DeleteCol);
            }

            if (Clicked > 1)
            {
                int count = 0;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    bool s = Convert.ToBoolean(row.Cells[0].Value);
                    if (s == true)
                    {
                        count++;
                    }
                }

                DialogResult result;
                result = MessageBox.Show("You are about to delete " + count.ToString() + " record(s).\nAre you sure you want to delete this record(s)?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        bool s = Convert.ToBoolean(row.Cells[0].Value);
                        if (s == true)
                        {
                            int EmployeeID = (int)dataGridView1.Rows[row.Index].Cells[1].Value;
                            ConnectionString connString = new ConnectionString();
                            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
                            try
                            {
                                sqlConn.Open();
                                string UpdateCommand = "UPDATE tblEmployees SET [Status] = 'InActive' WHERE EmployeeID = @id ";
                                SQLiteCommand sqlCommand = new SQLiteCommand(UpdateCommand, sqlConn);
                                sqlCommand.Parameters.Add(new SQLiteParameter("@id") { Value = EmployeeID });
                                sqlCommand.ExecuteNonQuery();
                                sqlConn.Close();
                                LogsFunction log = new LogsFunction();
                                log.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),"Employee Removed");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error");
                                sqlConn.Close();
                            }
                        }
                    }

                    Clicked = 0;
                    dataGridView1.Columns.Remove("Delete");
                    RefreshToolStrip.PerformClick();
                }
                else
                {
                    Clicked = 0;
                    dataGridView1.Columns.Remove("Delete");
                }
            }
        }

        private void RefreshToolStrip_Click(object sender, EventArgs e)
        {
            FrmViewEmployees_Load(sender, e);
        }

        private void terminateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count == 1)
            {
                string status = dataGridView1.SelectedRows[0].Cells[10].Value.ToString();
                if(status == "InActive")
                {
                    return;
                }

                int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);
                ConnectionString connString = new ConnectionString();
                SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
                sqlConn.Open();
                try
                {
                    string UpdateCommand = "UPDATE tblEmployees SET [TerminationDate] = @date,[Status] = @state WHERE EmployeeID = @id";
                    SQLiteCommand sqlCmd = new SQLiteCommand(UpdateCommand, sqlConn);
                    sqlCmd.Parameters.Add(new SQLiteParameter("@date") { Value = DateTime.Now });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@state") { Value = "InActive" });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@ID") { Value = id });
                    sqlCmd.ExecuteNonQuery();
                    VMessageBox VMsg = new VMessageBox("Employee has been terminated", "Employee Terminated", VMessageBox.MessageBoxType.Information);
                    VMsg.ShowDialog();
                    LogsFunction log = new LogsFunction();
                    log.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Employee Terminated");
                    RefreshToolStrip_Click(sender, e);
                }
                catch (Exception ex)
                {
                    VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.ForeColor = Color.Black;
            if(txtSearch.Text == "Search By Name")
            {
                txtSearch.Text = "";
            }
            search = true;
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if(txtSearch.Text == "")
            {
                search = false;
                txtSearch.ForeColor = Color.Gray;
                txtSearch.Text = "Search By Name";
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if(search == true)
            {
                try
                {
                    ConnectionString connString = new ConnectionString();
                    string CommandText = "SELECT * FROM tblEmployees WHERE [EmployeeName] LIKE'%" + txtSearch.Text + "%'";
                    SQLiteDataAdapter SearchAdapter = new SQLiteDataAdapter(CommandText, connString.Connection);
                    DataTable dt = new DataTable();
                    SearchAdapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception)
                {
                    //DO NOTHING
                }
            }
        }

        private void rbtnshowAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnshowAll.Checked)
            {
                try
                {
                    ConnectionString connString = new ConnectionString();
                    string CommandText = "SELECT * FROM tblEmployees";
                    SQLiteDataAdapter SearchAdapter = new SQLiteDataAdapter(CommandText, connString.Connection);
                    DataTable dt = new DataTable();
                    SearchAdapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception)
                {
                    //DO NOTHING
                }
            }
        }

        private void rbtnShowActive_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ConnectionString connString = new ConnectionString();
                string CommandText = "SELECT * FROM tblEmployees WHERE [Status] = 'Active'";
                SQLiteDataAdapter SearchAdapter = new SQLiteDataAdapter(CommandText, connString.Connection);
                DataTable dt = new DataTable();
                SearchAdapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception)
            {
                //DO NOTHING
            }
        }
    }
}
