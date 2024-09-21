using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using Excel = Microsoft.Office.Interop.Excel;

namespace CanteenManagmentSystem
{
    public partial class FrmViewSales : Form
    {
        private BindingSource source = new BindingSource();
        public FrmViewSales()
        {
            InitializeComponent();
        }

        private void ExportToolStrip_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog
            {
                Title = "Export",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Excel Files(*.xlsx)|*.xlsx"
            };

            string filename = "Sales" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
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

                        int sum = 0;
                        for (int v = 0; v < dataGridView1.Rows.Count; v++)
                        {
                            sum += Convert.ToInt32(dataGridView1.Rows[v].Cells[3].Value);
                        }


                        current.Cells[rows + 2, 3].Font.FontStyle = "Bold";
                        current.Cells[rows + 2, 3].value = "Total";
                        current.Cells[rows + 2, 4].value = sum;


                        current.Rows["1:1"].Font.FontStyle = "Bold";
                        current.Rows["1:1"].Font.Size = 12;
                        current.Columns.AutoFit();
                        current.Columns.EntireColumn.AutoFit();
                        current.Cells[1, 1].Select();


                        app.ActiveWorkbook.SaveAs(save.FileName);
                        app.ActiveWorkbook.Saved = true;
                        app.Quit();

                        this.Cursor = Cursors.Arrow;

                        exp.Close();

                        LogsFunction log = new LogsFunction();
                        log.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Sales list exported.File name is " + System.IO.Path.GetFileNameWithoutExtension(save.FileName));

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

        private void RefreshToolStrip_Click(object sender, EventArgs e)
        {
            FrmViewSales_Load(sender, e);
        }

        private async void FrmViewSales_Load(object sender, EventArgs e)
        {
            await GetData("SELECT * FROM tblSales");
            dataGridView1.DataSource = source;
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                DataGridViewColumn column = dataGridView1.Columns[i];
                column.Width = 200;

            }
            StartPosition = FormStartPosition.CenterScreen;
            bindingNavigator1.BindingSource = source;
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            try {
                ConnectionString connString = new ConnectionString();
                string selectedDate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                string CommandText = "SELECT * FROM tblSales WHERE STRFTIME('%Y-%m-%d',TimeOfSale) = STRFTIME('%Y-%m-%d','" + selectedDate + "')";
                SQLiteDataAdapter SearchAdapter = new SQLiteDataAdapter(CommandText, connString.Connection);
                DataTable dt = new DataTable();
                SearchAdapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch(Exception ex)
            {
                VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = source;
        }

        private void txtMode_TextChanged(object sender, EventArgs e)
        {
            try
            { 
            ConnectionString connString = new ConnectionString();
            string CommandText = "SELECT * FROM tblSales WHERE [ModeOfPayment] LIKE'%"+txtMode.Text+"%'";
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

        private void txtUserSearch_TextChanged(object sender, EventArgs e)
        {
            try {
                ConnectionString connString = new ConnectionString();
                string CommandText = "SELECT * FROM tblSales WHERE [User] LIKE'%" + txtUserSearch.Text + "%'";
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
