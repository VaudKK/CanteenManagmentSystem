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
    public partial class FrmViewProducts : Form
    {
        public FrmViewProducts()
        {
            InitializeComponent();
        }
        private BindingSource source = new BindingSource();
        public string Mode = "View";
        int Clicked = 0;
        private async void FrmViewDishes_Load(object sender, EventArgs e)
        {
            await GetData("SELECT * FROM tblItems");
            dataGridView1.DataSource = source;
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                DataGridViewColumn column = dataGridView1.Columns[i];
                column.Width = 235;

            }
            StartPosition = FormStartPosition.CenterScreen;
            bindingNavigator1.BindingSource = source;
            if(Mode == "Edit")
            {
                lblHint.Visible = true;
            }
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;
        }
        private async Task<object> GetData(string query)
        {
            ConnectionString connString = new ConnectionString();
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            sqlConn.Open();
            SQLiteDataAdapter sqliteAdapter = new SQLiteDataAdapter(query, sqlConn);
            SQLiteCommandBuilder sqliteBuilder = new SQLiteCommandBuilder(sqliteAdapter);
            DataTable table = new DataTable();
            await Task.Run(() => sqliteAdapter.Fill(table));
            source.DataSource = table;
            sqlConn.Close();

            return source;
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(Mode == "Edit")
            {
                for(int i = 0; i < Application.OpenForms.Count; i++)
                {
                    if(Application.OpenForms[i].Name == "FrmProduct")
                    {
                        FrmProduct product = (FrmProduct)Application.OpenForms[i];
                        try {
                            DataGridViewRow row = dataGridView1.SelectedRows[0];
                            product.txtProductID.Text = row.Cells[0].Value.ToString();
                            product.txtProductName.Text = row.Cells[1].Value.ToString();
                            product.txtPrice.Text = row.Cells[2].Value.ToString();
                            product.OriginalID = row.Cells[0].Value.ToString();
                            product.BtnSave.Text = "Update";
                        }
                        catch (Exception) { product.BtnSave.Text = "Save"; }
                    }
                }

                this.Close();
            }else if(Mode == "GetID")
            {
                for (int i = 0; i < Application.OpenForms.Count; i++)
                {
                    if (Application.OpenForms[i].Name == "FrmMakeASale")
                    {
                        FrmMakeASale makesale = (FrmMakeASale)Application.OpenForms[i];
                        try
                        {
                            DataGridViewRow row = dataGridView1.SelectedRows[0];
                            makesale.ItemTextBox.Text = row.Cells[0].Value.ToString();
                        }
                        catch (Exception) {
                            //DO NOTHING
                        }
                    }
                }

                this.Close();
            }
        }

        private void RefreshToolStrip_Click(object sender, EventArgs e)
        {
            FrmViewDishes_Load(sender, e);
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
                        log.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Product list exported.File name: " + System.IO.Path.GetFileNameWithoutExtension(save.FileName));

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
                            int productID = Convert.ToInt32(dataGridView1.Rows[row.Index].Cells[1].Value.ToString());
                            ConnectionString connString = new ConnectionString();
                            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
                            try
                            {
                                sqlConn.Open();
                                string DeleteCommand = "DELETE FROM tblItems WHERE ProductID = @id ";
                                SQLiteCommand sqlCommand = new SQLiteCommand(DeleteCommand, sqlConn);
                                sqlCommand.Parameters.Add(new SQLiteParameter("@id") {Value = productID });
                                sqlCommand.ExecuteNonQuery();
                                LogsFunction log = new LogsFunction();
                                log.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Product removed");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error");
                            }
                            finally
                            {
                                sqlConn.Close();
                            }
                        }
                    }

                    Clicked = 0;
                    dataGridView1.Columns.Remove("Delete");
                    RefreshToolStrip_Click(sender, e);
                }
                else
                {
                    Clicked = 0;
                    dataGridView1.Columns.Remove("Delete");
                }
            }
        }
    }
}
