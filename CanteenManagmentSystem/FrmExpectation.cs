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
    public partial class FrmExpectation : Form
    {
        private BindingSource source = new BindingSource();
        public string FileLocation;
        public FrmExpectation()
        {
            InitializeComponent();
        }

        public void Reload()
        {
            cboItem.Items.Clear();
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            ConnectionString connString = new ConnectionString();
            using (SQLiteConnection ConnString = new SQLiteConnection(connString.Connection))
            {
                try
                {
                    SQLiteCommand Command = new SQLiteCommand("SELECT ItemName FROM tblDailyUse WHERE STRFTIME('%Y-%m-%d',DATE) = '" + currentDate + "'", ConnString);
                    ConnString.Open();
                    SQLiteDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        cboItem.Items.Add(reader.GetString(0));
                    }
                    reader.Close();
                    ConnString.Close();
                    cboItem.Refresh();
                }
                catch (Exception)
                {
                    ConnString.Close();
                }

            }
        }

        private void FrmExpectation_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            ConnectionString connString = new ConnectionString();
            using (SQLiteConnection ConnString = new SQLiteConnection(connString.Connection))
            {
                try
                {
                    SQLiteCommand Command = new SQLiteCommand("SELECT ItemName FROM tblDailyUse WHERE STRFTIME('%Y-%m-%d',DATE) = '" + currentDate + "'", ConnString);
                    ConnString.Open();
                    SQLiteDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        cboItem.Items.Add(reader.GetString(0));
                    }
                    reader.Close();
                    ConnString.Close();
                }
                catch (Exception)
                {
                    ConnString.Close();
                }
            }

            if (!System.IO.Directory.Exists(System.IO.Path.GetTempPath() + "/SES"))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetTempPath() + "/SES");
            }

        }

        private async Task<object> GetData(string sqlCommand)
        {
            ConnectionString connString = new ConnectionString();
            SQLiteDataAdapter sqlAdapter = new SQLiteDataAdapter(sqlCommand, connString.Connection);
            SQLiteCommandBuilder sqlBuilder = new SQLiteCommandBuilder(sqlAdapter);

            DataTable table = new DataTable();
            await Task.Run(()=>sqlAdapter.Fill(table));
            source.DataSource = table;

            return source;
        }

        private void textBoxKeyPressedEvent_KeyPress(object sender,KeyPressEventArgs e)
        {
            var txtBox = (TextBox)sender;
            string name = txtBox.Name;

            switch (name)
            {
                case "txtPPU":
                    if (!(char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar)))
                    {
                        e.Handled = true;
                    }
                    break;

                case "txtNOU":
                    if (!(char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar)))
                    {
                        e.Handled = true;
                    }
                    break;


            }

        }

        private void fetchQuantity()
        {
            ConnectionString connString = new ConnectionString();
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            SQLiteCommand cmd = new SQLiteCommand("SELECT Quantity FROM tblDailyUse WHERE ItemName = '" + cboItem.Text.Trim() + "'", sqlConn);
            sqlConn.Open();
            SQLiteDataReader reader = cmd.ExecuteReader();
            string fetchedValue = "";
            while (reader.Read())
            {
                fetchedValue = reader.GetInt32(0).ToString();
            }
            reader.Close();
            sqlConn.Close();

            txtqty.Text = fetchedValue;
        }

        private void textChangedEvent_TextChanged(object sender,EventArgs e)
        {
            var textbox = (TextBox)sender;
            string name = textbox.Name;

            switch (name)
            {
                case "txtPPU":
                    try
                    {
                        double units = Convert.ToDouble(txtNOU.Text);
                        double PPU = Convert.ToDouble(txtPPU.Text);
                        double amount = units * PPU;
                        txtAmt.Text = amount.ToString();
                    }
                    catch (Exception)
                    {
                        txtAmt.Text="";
                    }
                    break;

                case "txtNOU":
                    try
                    {
                        double units = Convert.ToDouble(txtNOU.Text);
                        double PPU = Convert.ToDouble(txtPPU.Text);
                        double amount = units * PPU;
                        txtAmt.Text = amount.ToString();
                    }
                    catch (Exception)
                    {
                        txtAmt.Text = "";
                    }
                    break;

                default:
                    break;

            }
        }

        private void AddToDataGrid()
        {
            string[] textBoxValues = new string[6];
            /*
            int count = 0;
            foreach (Control c in this.Controls)
            {
                if (c is TextBox)
                {
                    count++;
                    textBoxValues[count-1] = c.Text;
                }
            }
            */
            textBoxValues[0] = cboItem.Text;
            textBoxValues[1] = txtqty.Text;
            textBoxValues[2] = txtNOU.Text;
            textBoxValues[3] = txtDescription.Text;
            textBoxValues[4] = txtPPU.Text;
            textBoxValues[5] = txtAmt.Text;

            dataGridView1.DataSource = null;
            DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
            for(int i = 0; i < textBoxValues.Count(); i++)
            {
                row.Cells[i].Value = textBoxValues[i];
            }
            dataGridView1.Rows.Add(row);
        }

        private bool AllTextsFilled()
        {
            foreach(Control c in this.Controls)
            {
                if(c is TextBox)
                {
                    if (c.Text == "")
                        return false;
                    return true;
                }
            }

            return false;
        }

        private bool RowExists()
        {
            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                if(row.Cells[0].Value != null)
                {
                    if (row.Cells[0].Value.ToString() == cboItem.Text)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }   
            }

            return false;
        }

        private void ClearTextBox()
        {
            foreach(Control c in this.Controls)
            {
                if(c is TextBox)
                {
                    c.Text = "";
                }
            }

            txtNOU.Select();
        }

        private void textBoxKeyDownEvent_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (AllTextsFilled())
                {
                    if (RowExists())
                    {
                        VMessageBox VMsg = new VMessageBox("The record you are about to enter already exits", "Duplication", VMessageBox.MessageBoxType.Error);
                        VMsg.ShowDialog();
                    }
                    else
                    {
                        AddToDataGrid();
                        ClearTextBox();
                    }
                }
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if(dataGridView1.SelectedRows.Count == 1)
            {
                if(e.KeyCode == Keys.Delete)
                {
                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                }
            }
        }

        private void cboItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            fetchQuantity();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            FrmDailyUse use = new FrmDailyUse();
            use.ObjSender = "Expectations";
            use.ShowDialog();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            FrmAllUses uses = new FrmAllUses();
            uses.ShowDialog();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

            string filename = "SE" + DateTime.Now.Year.ToString() + "" + DateTime.Now.Month.ToString() + "" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + "" + DateTime.Now.Minute.ToString() + "" + DateTime.Now.Second.ToString();

                if (filename != "")
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


                        app.ActiveWorkbook.SaveAs(filename);
                        app.ActiveWorkbook.Saved = true;
                        app.Quit();

                        this.Cursor = Cursors.Arrow;

                        exp.Close();

                        System.IO.File.Move(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + filename, System.IO.Path.GetTempPath() + "/SES/" + filename);

                        LogsFunction log = new LogsFunction();
                        //log.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Product list exported.File name: " + System.IO.Path.GetFileNameWithoutExtension(filename));

                        DialogResult result;
                        result = MessageBox.Show("Export succeeded.Do you want to open the file?", "Export", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(filename);
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

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            FrmOpenFile open = new FrmOpenFile();
            open.ShowDialog();
        }

        public void ImportToDGV()
        {
            //ADD TRY CATCH
            toolStripButton3.Enabled = false;
            ClearDGV();
            System.Data.OleDb.OleDbConnection Conn;
            DataSet dt;
            System.Data.OleDb.OleDbDataAdapter adapter;
            Conn = new System.Data.OleDb.OleDbConnection(@"provider = Microsoft.ACE.OLEDB.12.0;Data source='" + FileLocation + "';Extended Properties=Excel 12.0;");
            adapter = new System.Data.OleDb.OleDbDataAdapter("SELECT * FROM [Sheet1$]", Conn);
            adapter.TableMappings.Add("SESTable", "SESDataSet");
            dt = new DataSet();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt.Tables[0];
            Conn.Close();
        }

        private void ClearDGV()
        {
            dataGridView1.Columns.Clear();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            ClearDGV();
            toolStripButton3.Enabled = true;
            dataGridView1.DataSource = null;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cItem,
            this.cQty,
            this.cNOP,
            this.cDescription,
            this.cPrice,
            this.cAmt});

            // 
            // cItem
            // 
            this.cItem.HeaderText = "Item";
            this.cItem.Name = "cItem";
            this.cItem.ReadOnly = true;
            this.cItem.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cItem.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cItem.Width = 150;
            // 
            // cQty
            // 
            this.cQty.HeaderText = "Quantity";
            this.cQty.Name = "cQty";
            this.cQty.ReadOnly = true;
            this.cQty.Width = 150;
            // 
            // cNOP
            // 
            this.cNOP.HeaderText = "NoOfUnits";
            this.cNOP.Name = "cNOP";
            this.cNOP.ReadOnly = true;
            this.cNOP.Width = 150;
            // 
            // cDescription
            // 
            this.cDescription.HeaderText = "UnitDescription";
            this.cDescription.Name = "cDescription";
            this.cDescription.ReadOnly = true;
            // 
            // cPrice
            // 
            this.cPrice.HeaderText = "PricePerUnit";
            this.cPrice.Name = "cPrice";
            this.cPrice.ReadOnly = true;
            // 
            // cAmt
            // 
            this.cAmt.HeaderText = "Total Amount";
            this.cAmt.Name = "cAmt";
            this.cAmt.ReadOnly = true;
        }
    }
}
