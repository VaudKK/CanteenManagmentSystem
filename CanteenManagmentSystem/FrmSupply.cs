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
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace CanteenManagmentSystem
{
    public partial class FrmSupply : Form
    {
        private SQLiteDataAdapter adapter = new SQLiteDataAdapter();
        private BindingSource source = new BindingSource();
        ConnectionString connString = new ConnectionString();
        public FrmSupply()
        {
            InitializeComponent();
            PriceTextBox.GotFocus += PriceTextBox_GotFocus;
        }

        private void CreateTables()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "CREATE TABLE IF NOT EXISTS tblSupply(InvoiceID INTEGER PRIMARY KEY ASC NOT NULL,ItemID VARCHAR(100) NOT NULL,ItemName VARCHAR(150) NOT NULL," +
                          "Vendor TEXT NOT NULL,Quantity INTEGER NOT NULL, PricePerUnit FLOAT NOT NULL,Delivery TEXT NOT NULL,TotalPrice FLOAT NOT NULL)";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sql = "CREATE TABLE IF NOT EXISTS tblStock(ItemID VARCHAR(100) PRIMARY KEY NOT NULL,ItemName TEXT NOT NULL,Quantity INTEGER NOT NULL)";
            sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void DropTable()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "DROP TABLE tblSupply";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void PriceTextBox_GotFocus(object sender, EventArgs e)
        {
            if (QuantityTextBox.Text == "")
            {
               /* MessageBox.Show("Quantity cannot be less than one", "Quantity", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                QuantityTextBox.Focus();
                return;*/
            }
        }

        private void AssignInvoiceNo()
        {
            lblvalue.Text = GenerateInvoiceNo(6);
        }

        private static string GenerateInvoiceNo(int max)
        {
            //TO REVISE
            char[] mychar = new char[62];
            mychar = "123456789".ToCharArray();
            byte[] data = new byte[1];
            System.Security.Cryptography.RNGCryptoServiceProvider crypt = new System.Security.Cryptography.RNGCryptoServiceProvider();
            crypt.GetNonZeroBytes(data);
            data = new byte[max];
            crypt.GetNonZeroBytes(data);
            StringBuilder builder = new StringBuilder(max);
            foreach (byte b in data)
            {
                builder.Append(mychar[b % (mychar.Length)]);
            }

            return builder.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
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

                        int sum = 0;
                        for(int v = 0; v < dataGridView1.Rows.Count; v++)
                        {
                            sum += Convert.ToInt32(dataGridView1.Rows[v].Cells[7].Value);
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

                        this.Cursor = Cursors.Arrow;

                        exp.Close();

                        LogsFunction log = new LogsFunction();
                        log.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Sales list exported.File name is " + Path.GetFileNameWithoutExtension(save.FileName));

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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!isItemAvailableInProductList(NameTextBox.Text))
            {
                DialogResult result = MessageBox.Show("The Item '" + NameTextBox.Text + "' does not exist in your product list.\nDo you want to add it?", "Item Not Found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    FrmProduct addproduct = new FrmProduct();
                    addproduct.txtProductName.Text = NameTextBox.Text;
                    addproduct.ShowDialog();
                }
                else
                {
                    return;
                }
            }
            else
            {
                SQLiteConnection DatabaseConnection = new SQLiteConnection(connString.Connection);
                SQLiteCommand Command = new SQLiteCommand();
                try
                {
                    Command.Connection = DatabaseConnection;
                    DatabaseConnection.Open();
                    Command.CommandText = "SELECT [Quantity] FROM tblStock WHERE [ItemID] = '" + ItemIDTextBox.Text + "'";
                    SQLiteDataReader Reader = Command.ExecuteReader();
                    if (Reader.HasRows)
                    {
                        int value = 0;
                        while (Reader.Read())
                        {
                            value = Reader.GetInt32(0);
                        }

                        Reader.Close();

                        int currentvalue = value;
                        int Total = currentvalue + int.Parse(QuantityTextBox.Text);
                        SQLiteDataAdapter UpdateAdapter = new SQLiteDataAdapter("SELECT * FROM tblStock", connString.Connection);
                        UpdateAdapter.UpdateCommand = new SQLiteCommand();
                        UpdateAdapter.UpdateCommand.Connection = DatabaseConnection;
                        UpdateAdapter.UpdateCommand.CommandText = "UPDATE tblStock SET[Quantity] = @qty  WHERE(([ItemID] = @id ))";
                        UpdateAdapter.UpdateCommand.Parameters.Add(new SQLiteParameter("@qty") { Value = Total });
                        UpdateAdapter.UpdateCommand.Parameters.Add(new SQLiteParameter("@id") { Value = ItemIDTextBox.Text });
                        UpdateAdapter.UpdateCommand.ExecuteNonQuery();
                        InsertIntoTable();

                        LogsFunction log = new LogsFunction();
                        log.Logs(Properties.Settings.Default.CurrentUser, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Stock Item Updated. ItemID " + ItemIDTextBox.Text);

                        clear();
                    }
                    else
                    {
                        if (!isItemNameAvailable(NameTextBox.Text))
                        {
                            Reader.Close();
                            SQLiteDataAdapter InsertAdapter2 = new SQLiteDataAdapter("SELECT * FROM tblStock", DatabaseConnection);
                            InsertAdapter2.InsertCommand = new SQLiteCommand();
                            InsertAdapter2.InsertCommand.Connection = DatabaseConnection;
                            InsertAdapter2.InsertCommand.CommandText = "INSERT INTO tblStock ([ItemID],[ItemName],[Quantity]) VALUES(@id,@name,@qty)";
                            InsertAdapter2.InsertCommand.Parameters.Add(new SQLiteParameter("@id") { Value = ItemIDTextBox.Text });
                            InsertAdapter2.InsertCommand.Parameters.Add(new SQLiteParameter("@name") { Value = NameTextBox.Text });
                            InsertAdapter2.InsertCommand.Parameters.Add(new SQLiteParameter("@qty") { Value = Convert.ToInt32(QuantityTextBox.Text) });
                            InsertAdapter2.InsertCommand.ExecuteNonQuery();
                            InsertIntoTable();

                            LogsFunction log = new LogsFunction();
                            log.Logs(Properties.Settings.Default.CurrentUser, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "New Stock Item Added. ItemID " + ItemIDTextBox.Text);

                            clear();
                        }
                        else
                        {
                            MessageBox.Show(this, "An Item with the name " + NameTextBox.Text + " already exists.Please use another name", "Duplication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            NameTextBox.Select();
                            NameTextBox.SelectAll();
                        }
                    }
                }
                catch (Exception v)
                {
                    VMessageBox VMsg = new VMessageBox(v.Message, "Error", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                }

            }
           
        }

        private  bool isItemAvailableInProductList(string value)
        {
            bool exists = false;
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            string CommandText = "SELECT * FROM tblItems WHERE ProductName = '" + value + "' COLLATE NOCASE";
            sqlConn.Open();
            try
            {
                SQLiteCommand sqlCmd = new SQLiteCommand(CommandText, sqlConn);
                SQLiteDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader.HasRows)
                {
                    exists = true;
                }
                sqlReader.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to validate product name.Click okay to continue", "Error");
            }
            finally
            {
                sqlConn.Close();
            }
            return exists;
        }

        private async Task<object> GetData(string command)
        {
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command, connString.Connection);
                SQLiteCommandBuilder builder = new SQLiteCommandBuilder(this.adapter);
                DataTable table = new DataTable();
                await Task.Run(() => adapter.Fill(table));
                source.DataSource = table;

                return source;
        }

        private bool isItemNameAvailable(string ItemName)
        {
            bool exists = false;
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            SQLiteCommand sqlCmd = new SQLiteCommand();
            sqlCmd.Connection = sqlConn;
            sqlConn.Open();
            try
            {
                sqlCmd.CommandText = "SELECT * FROM tblStock WHERE ItemName = '" + ItemName + "' COLLATE NOCASE";
                SQLiteDataReader sqlReader = sqlCmd.ExecuteReader();
                if (sqlReader.HasRows)
                {
                    exists = true;
                }
                sqlReader.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to validate ItemName.Click OK to continue", "Validation Failed");
            }
            finally
            {
                sqlConn.Close();
            }
            return exists;
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            clear();
        }
        public void clear()
        {
            ItemIDTextBox.Text = "";
            NameTextBox.Text = "";
            VendorTextBox.Text = "";
            PriceTextBox.Text = "";
            QuantityTextBox.Text = "";
           TotalCostLabel.Text = " 0.00";
        }
        public async void InsertIntoTable()
        {
            SQLiteConnection DatabaseConnection = new SQLiteConnection(connString.Connection);
            DatabaseConnection.Open();
           try
            {
                AssignInvoiceNo();
                SQLiteDataAdapter InsertAdapter = new SQLiteDataAdapter("SELECT * FROM tblSupply", DatabaseConnection);
                InsertAdapter.InsertCommand = new SQLiteCommand();
                InsertAdapter.InsertCommand.Connection = DatabaseConnection;
                InsertAdapter.InsertCommand.CommandText = "INSERT INTO tblSupply ([InvoiceID],[ItemID],[ItemName],[Vendor],[Quantity],[PricePerUnit],[Delivery],[TotalPrice]) VALUES(@inv,@id,@name,@vendor,@qty,@price,@del,@total)";
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@inv") { Value = Convert.ToInt32(lblvalue.Text) });
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@id") { Value = ItemIDTextBox.Text });
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@name") { Value = NameTextBox.Text });
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@vendor") { Value = VendorTextBox.Text });
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@qty") { Value = Convert.ToInt32(QuantityTextBox.Text) });
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@price") { Value = Convert.ToDouble(PriceTextBox.Text) });
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@del") { Value = dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss") });
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@total") { Value = Convert.ToDouble(TotalCostLabel.Text) });
                InsertAdapter.InsertCommand.ExecuteNonQuery();
                DatabaseConnection.Close();
                VMessageBox VMsg = new VMessageBox("Details Saved", "Saved", VMessageBox.MessageBoxType.Information);
                VMsg.ShowDialog();
                await GetData("SELECT * FROM tblSupply");
            }
            catch( Exception v)
            {
                VMessageBox VMsg = new VMessageBox(v.Message, "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                DatabaseConnection.Close();
            }
        }
        private async void FrmStock_Load(object sender, EventArgs e)
        {
            CreateTables();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            await GetData("SELECT * FROM tblSupply");
            dataGridView1.DataSource = this.source;
            for (int i = 0; i < dataGridView1.ColumnCount - 1; i++)
            {
                DataGridViewColumn column = dataGridView1.Columns[i];
                column.Width = 150;

            }
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;
            dataGridView1.ReadOnly = true;
            using (SQLiteConnection Connection = new SQLiteConnection(connString.Connection))
            {
                try
                {
                    SQLiteCommand Command = new SQLiteCommand("SELECT ItemID FROM tblStock", Connection);
                    Connection.Open();
                    SQLiteDataReader Reader = Command.ExecuteReader();
                    AutoCompleteStringCollection Collection = new AutoCompleteStringCollection();
                    while (Reader.Read())
                    {
                        Collection.Add(Reader.GetString(0));
                    }
                    ItemIDTextBox.AutoCompleteCustomSource = Collection;
                    Connection.Close();
                }
                catch (Exception)
                {
                    //DO NOTHING
                }

                ItemIDTextBox.Select();
            }

            getAutoCompleteValues();
        }

        private void getAutoCompleteValues()
        {
            using (SQLiteConnection Connection = new SQLiteConnection(connString.Connection))
            {
                try
                {
                    SQLiteCommand Command = new SQLiteCommand("SELECT ProductName FROM tblItems", Connection);
                    Connection.Open();
                    SQLiteDataReader Reader = Command.ExecuteReader();
                    AutoCompleteStringCollection Collection = new AutoCompleteStringCollection();
                    while (Reader.Read())
                    {
                        Collection.Add(Reader.GetString(0));
                    }
                    NameTextBox.AutoCompleteCustomSource = Collection;
                    NameTextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
                    Connection.Close();
                }
                catch (Exception)
                {
                    //DO NOTHING
                }
            }
        }

        private void PriceTextBox_TextChanged(object sender, EventArgs e)
        {
            Double value = 0;
            try
            {
               value = Double.Parse(QuantityTextBox.Text);
            }
            catch (Exception)
            {
                // Nothing happens
            }
            if (value < 1 )
            {
                VMessageBox VMsg = new VMessageBox("Quantity cannot be less than one", "Price",VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;
            }
            else
            {
                Double total;
                Double quantity;
                try
                {
                    quantity = Double.Parse(PriceTextBox.Text);
                    total = value * quantity;
                    TotalCostLabel.Text = total.ToString("##.##");
                }
                catch (Exception)
                {
                    TotalCostLabel.Text = "0.00";
                }
                
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            clear();
            this.Close();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
           try{
                string selectedDate = dateTimePicker1.Value.ToString("yyyy-MM-dd ");
                string CommandText = "SELECT * FROM tblSupply WHERE STRFTIME('%Y-%m-%d',Delivery) = STRFTIME('%Y-%m-%d','" + selectedDate + "')";
                SQLiteDataAdapter da = new SQLiteDataAdapter(CommandText, connString.Connection);
                DataTable dt =  new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
           }
        catch(Exception v)
           {
            MessageBox.Show(v.Message, "Error");
           }
        }

        private void QuantityTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!( char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void PriceTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            /*
             if (!( char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
            */
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = source;
        }

        private void ItemIDTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                SQLiteCommand command = new SQLiteCommand();
                SQLiteConnection DatabaseConnection = new SQLiteConnection(connString.Connection);
                DatabaseConnection.Open();
                command.Connection = DatabaseConnection;
                command.CommandText = "SELECT * FROM  tblStock WHERE ItemID = '" + ItemIDTextBox.Text + "'";
                SQLiteDataReader Reader = command.ExecuteReader();
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        var price = Reader["ItemName"];
                        NameTextBox.Text = price.ToString();
                    }
                    Reader.Close();
                }
                else
                {
                    VMessageBox VMsg = new VMessageBox("The Item ID currently does not exist in the database." + Environment.NewLine + "Continue entry so as to save it", "Items",VMessageBox.MessageBoxType.Information);
                    VMsg.ShowDialog();
                    NameTextBox.Text = "";
                }
            }
        }

        private void btnGetID_Click(object sender, EventArgs e)
        {
            FrmViewStock view= new FrmViewStock();
            view.Mode = "Selection";
            view.ShowDialog();
        }

        private void ItemIDTextBox_TextChanged(object sender, EventArgs e)
        {
            /*
            if(ItemIDTextBox.TextLength > 7)
            {
                VMessageBox VMsg = new VMessageBox("Maximum amount of characters allowed is 7", "Error",VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;
            }
            */
            try {
                NameTextBox.Text = "";
                SQLiteCommand command = new SQLiteCommand();
                SQLiteConnection DatabaseConnection = new SQLiteConnection(connString.Connection);
                DatabaseConnection.Open();
                command.Connection = DatabaseConnection;
                command.CommandText = "SELECT * FROM  tblStock WHERE ItemID = '" + ItemIDTextBox.Text + "'";
                SQLiteDataReader Reader = command.ExecuteReader();
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        var price = Reader["ItemName"];
                        NameTextBox.Text = price.ToString();
                    }
                    Reader.Close();
                }

            }catch(Exception)
            {
                //Do nothing
                NameTextBox.Text = "";
            }
        }
    }
}
