using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using Excel = Microsoft.Office.Interop.Excel;

namespace CanteenManagmentSystem
{
    public partial class FrmEndOfDay : Form
    {
        public FrmEndOfDay()
        {
            InitializeComponent();
        }
        private SQLiteDataAdapter adapter = new SQLiteDataAdapter();
        private BindingSource source = new BindingSource();
        private SQLiteDataAdapter ad = new SQLiteDataAdapter();
        private BindingSource binding = new BindingSource();
        ConnectionString connString = new ConnectionString();
        private async void FrmEndOfDay_Load(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            string selectedDate = DateTime.Now.ToString("yyyy-MM-dd ");
            string SalesCommandText = "SELECT * FROM tblSales WHERE STRFTIME('%Y-%m-%d',TimeOfSale) = STRFTIME('%Y-%m-%d','" + selectedDate + "')";
            string SupplyCommandText = "SELECT * FROM tblSupply WHERE STRFTIME('%Y-%m-%d',Delivery) = STRFTIME('%Y-%m-%d','" + selectedDate + "')";
            await GetData(SalesCommandText);
            dataGridView1.DataSource = this.source;
            await SelectCommand(SupplyCommandText);
            dataGridView2.DataSource = this.binding;
            for (int i = 0; i < dataGridView1.ColumnCount - 1; i++)
            {
                DataGridViewColumn column = dataGridView1.Columns[i];
                column.Width = 155;

            }
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;
            dataGridView1.ReadOnly = true;
            for (int i = 0; i < dataGridView2.ColumnCount - 1; i++)
            {
                DataGridViewColumn column = dataGridView2.Columns[i];
                column.Width = 200;

            }

            dataGridView2.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;
            dataGridView2.ReadOnly = true;
            int sum = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                sum += Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value);
            }
            TotalLabel.Text = "" + sum.ToString();

            int total = 0;
            for(int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                total += Convert.ToInt32(dataGridView2.Rows[i].Cells[7].Value);
            }
            totalpurchases.Text = "Total: " + total.ToString();

            Cursor = Cursors.Arrow;
        }
        private async Task<object> GetData(string command)
        {
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command, sqlConn);
            SQLiteCommandBuilder builder = new SQLiteCommandBuilder(this.adapter);
            DataTable table = new DataTable();
            await Task.Run(() => adapter.Fill(table));
            source.DataSource = table;

            return source;
        }

       private async Task<object> SelectCommand(string command)
       {
           ConnectionString connString = new ConnectionString();
           SQLiteDataAdapter ad = new SQLiteDataAdapter(command, connString.Connection);
           SQLiteCommandBuilder builder = new SQLiteCommandBuilder(this.ad);
           DataTable table = new DataTable();
           await Task.Run(() => ad.Fill(table));
           binding.DataSource = table;


           return binding;
      }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog
            {
                Title = "Export",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Excel Files(*.xlsx)|*.xlsx"

            };

            string filename = "TodaysSales"+DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString()+DateTime.Now.Year.ToString();
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

                        LogsFunction logs = new LogsFunction();
                        logs.Logs(Properties.Settings.Default.CurrentUser, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Exported new file. File location: " + save.FileName);

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

        private void btnExpt_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog
            {
                Title = "Export",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Excel Files(*.xlsx)|*.xlsx"
            };
            string filename = "TodaysPurchases" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();
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

                        rows = dataGridView2.RowCount - 1;
                        columns = dataGridView2.Columns.Count - 1;
                        Excel.Application app = new Excel.Application();
                        Excel.Workbook book = app.Workbooks.Add();
                        Excel.Worksheet sheet = (Excel.Worksheet)book.Worksheets[1];
                        var current = sheet;
                        current.Columns.Select();
                        current.Columns.Delete();

                        //Exporting the Header Texts
                        for (ic = 0; ic <= columns; ic++)
                        {
                            current.Cells[1, ic + 1].Value = dataGridView2.Columns[ic].HeaderText;
                        }

                        // Exporting the rows
                        for (i = 0; i <= rows - 1; i++)
                        {
                            for (j = 0; j <= columns; j++)
                            {
                                current.Cells[i + 2, j + 1].Value = dataGridView2.Rows[i].Cells[j].Value;
                            }
                        }


                        int sum = 0;
                        for (int v = 0; v < dataGridView2.Rows.Count; v++)
                        {
                            sum += Convert.ToInt32(dataGridView2.Rows[v].Cells[7].Value);
                        }


                        current.Cells[rows + 2, 7].Font.FontStyle = "Bold";
                        current.Cells[rows + 2, 7].value = "Total";
                        current.Cells[rows + 2, 8].value = sum;


                        current.Rows["1:1"].Font.FontStyle = "Bold";
                        current.Rows["1:1"].Font.Size = 12;
                        current.Columns.AutoFit();
                        current.Columns.EntireColumn.AutoFit();
                        current.Cells[1, 1].Select();


                        app.ActiveWorkbook.SaveAs(save.FileName);
                        app.ActiveWorkbook.Saved = true;
                        app.Quit();

                        LogsFunction logs = new LogsFunction();
                        logs.Logs(Properties.Settings.Default.CurrentUser, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Exported new file. File location: " + save.FileName);

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
                        VMessageBox VMsg = new VMessageBox(ex.Message, "Error",VMessageBox.MessageBoxType.Error);
                        VMsg.ShowDialog();
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
