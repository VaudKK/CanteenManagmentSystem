using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SQLite;

namespace CanteenManagmentSystem
{
    public partial class FrmViewStock : Form
    {
        public FrmViewStock()
        {
            InitializeComponent();
        }
        private SQLiteDataAdapter adapter = new SQLiteDataAdapter();
        private BindingSource source = new BindingSource();
        public string Mode = "View";
        private async void FrmViewStock_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            await GetData("SELECT * FROM tblStock");
            dataGridView1.DataSource = this.source;
            bindingNavigator1.BindingSource = this.source;
            for (int i = 0; i < dataGridView1.ColumnCount - 1; i++)
            {
                DataGridViewColumn column = dataGridView1.Columns[i];
                column.Width = 155;

            }
            this.dataGridView1.ReadOnly = true;
            this.Width = 470;
            this.Height = 600;
            this.MaximizeBox = false;
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

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (Mode == "Selection")
            {
                for (int i = 0; i < Application.OpenForms.Count; i++)
                {
                    DataGridViewRow row = dataGridView1.SelectedRows[0];
                    if (Application.OpenForms[i].Name == "FrmSupply")
                    {
                        FrmSupply supply = (FrmSupply)Application.OpenForms[i];
                        supply.ItemIDTextBox.Text = row.Cells[0].Value.ToString();
                    }
                }
                Close();
            }

            if (Mode == "SelectionDaily")
            {
                for (int i = 0; i < Application.OpenForms.Count; i++)
                {
                    DataGridViewRow row = dataGridView1.SelectedRows[0];
                    if (Application.OpenForms[i].Name == "FrmStockItems")
                    {
                        FrmDailyUse stock = (FrmDailyUse)Application.OpenForms[i];
                        stock.ItemIDTextBox.Text = row.Cells[0].Value.ToString();
                    }
                }

                Close();
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

                        exp.Close();

                        LogsFunction log = new LogsFunction();
                        log.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Stock list exported.File name: " + System.IO.Path.GetFileNameWithoutExtension(save.FileName));

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
            this.FrmViewStock_Load(sender, e);
        }
    }
}
