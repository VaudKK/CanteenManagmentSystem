using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SQLite;
using Excel = Microsoft.Office.Interop.Excel;

namespace CanteenManagmentSystem
{
    public partial class FrmLogs : Form
    {
        private SQLiteDataAdapter adapter = new SQLiteDataAdapter();
        private BindingSource source = new BindingSource();
        public FrmLogs()
        {
            InitializeComponent();
        }

        private void CreateTable()
        {
            ConnectionString connString = new ConnectionString();
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "CREATE TABLE IF NOT EXISTS tblLogs(LogNo INTEGER PRIMARY KEY ASC AUTOINCREMENT,User VARCHAR(150) NOT NULL," +
                "Time TEXT NOT NULL,Operation TEXT NOT NULL)";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private async void FrmLogs_Load(object sender, EventArgs e)
        {
            CreateTable();
            string selectedDate = DateTime.Now.ToString("yyyy-MM-dd ");
            string CommandText = "SELECT * FROM tblLogs WHERE STRFTIME('%Y-%m-%d',Time) = STRFTIME('%Y-%m-%d','" + selectedDate + "')";
            await GetData(CommandText);
            datagridview1.DataSource = this.source;
            foreach (DataGridViewColumn Column in datagridview1.Columns)
            {
                Column.Width = 200;
            }
            datagridview1.Columns[3].Width = 450;
        }
        private async Task<object> GetData(string command)
        {
            ConnectionString connString = new ConnectionString();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command, connString.Connection);
            SQLiteCommandBuilder builder = new SQLiteCommandBuilder(this.adapter);
            DataTable table = new DataTable();
            await Task.Run(() => adapter.Fill(table));
            source.DataSource = table;
            return source;
        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {
           if(rbtnUser.Checked==true)
            {
                try
                {
                    source.Filter = "User LIKE'%" + txtUser.Text + "%'";
                }
                catch(Exception v)
                {
                    txtUser.Clear();
                    MessageBox.Show(v.Message, "Error");
                }
            }
        }

        private void rbtnUser_CheckedChanged(object sender, EventArgs e)
        {
            if(rbtnUser.Checked == true)
            {
                txtUser.Enabled = true;
                this.datagridview1.DataSource = source;
            }
            else
            {
                txtUser.Enabled = false;
            }
        }

        private void rbtnSpecific_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnSpecific.Checked == true)
            {
                dateTimePicker1.Enabled = true;
            }
            else
            {
                dateTimePicker1.Enabled = false;
            }
        }

        private void rbtnRange_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnRange.Checked == true)
            {
                dateTimePicker2.Enabled = true;
                dateTimePicker3.Enabled = true;
                btnSearch.Enabled = true;
            }
            else
            {
                dateTimePicker2.Enabled = false;
                dateTimePicker3.Enabled = false;
                btnSearch.Enabled = false;
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if(rbtnSpecific.Checked == true)
            {
                try
                {
                    ConnectionString connString = new ConnectionString();
                    string selectedDate = dateTimePicker1.Value.ToString("yyyy-MM-dd ");
                    string CommandText = "SELECT * FROM tblLogs WHERE STRFTIME('%Y-%m-%d',Time) = STRFTIME('%Y-%m-%d','" + selectedDate + "')";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(CommandText, connString.Connection);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    datagridview1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(rbtnRange.Checked==true)
            {
                ConnectionString connString = new ConnectionString();
                SQLiteConnection Conn = new SQLiteConnection(connString.Connection);
                string fromDate = dateTimePicker3.Value.ToString("yyyy-MM-dd");
                string toDate = dateTimePicker2.Value.ToString("yyyy-MM-dd");
                try
                {
                    string query = "SELECT * FROM tblLogs WHERE Time BETWEEN @time1 AND @time2";
                    SQLiteCommand cmd = new SQLiteCommand(query, Conn);
                    cmd.Parameters.Add(new SQLiteParameter("@time1") { Value = fromDate });
                    cmd.Parameters.Add(new SQLiteParameter("@time2") { Value = toDate});
                    DataSet ds = new DataSet();
                    SQLiteDataAdapter FilterAdapter = new SQLiteDataAdapter();
                    Conn.Open();
                    FilterAdapter.SelectCommand = cmd;
                    FilterAdapter.Fill(ds, "tblLogs");
                    datagridview1.DataSource = ds.Tables[0];
                    Conn.Close();
                }
                catch(Exception v)
                {
                    MessageBox.Show(v.Message, "Error");
                    Conn.Close();
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.datagridview1.DataSource = source;
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                ConnectionString connString = new ConnectionString();
                string CommandText = "SELECT * FROM tblLogs";
                SQLiteDataAdapter da = new SQLiteDataAdapter(CommandText, connString.Connection);
                DataTable dt = new DataTable();
                da.Fill(dt);
                datagridview1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog
            {
                Title = "Export",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Excel Files(*.xlsx)|*.xlsx"

            };

            string filename = "Logs" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();
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

                        rows = datagridview1.RowCount - 1;
                        columns = datagridview1.Columns.Count - 1;
                        Excel.Application app = new Excel.Application();
                        Excel.Workbook book = app.Workbooks.Add();
                        Excel.Worksheet sheet = (Excel.Worksheet)book.Worksheets[1];
                        var current = sheet;
                        current.Columns.Select();
                        current.Columns.Delete();

                        //Exporting the Header Texts
                        for (ic = 0; ic <= columns; ic++)
                        {
                            current.Cells[1, ic + 1].Value = datagridview1.Columns[ic].HeaderText;
                        }

                        // Exporting the rows
                        for (i = 0; i <= rows - 1; i++)
                        {
                            for (j = 0; j <= columns; j++)
                            {
                                current.Cells[i + 2, j + 1].Value = datagridview1.Rows[i].Cells[j].Value;
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

                        LogsFunction logs = new LogsFunction();
                        logs.Logs(Properties.Settings.Default.CurrentUser, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Logs exported. File location: " + save.FileName);

                        this.Cursor = Cursors.Arrow;

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
    }
}
