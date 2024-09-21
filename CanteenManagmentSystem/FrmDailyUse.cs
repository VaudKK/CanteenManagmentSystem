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

namespace CanteenManagmentSystem
{
    public partial class FrmDailyUse : Form
    {
        public string ObjSender;

        ConnectionString connString = new ConnectionString();
        public FrmDailyUse()
        {
            InitializeComponent();
        }

        private void FrmStockItems_Load(object sender, EventArgs e)
        {
            CreateTable();
            this.FormBorderStyle =FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            DateLabel.Text = DateTime.Now.ToLongDateString();
            ItemIDTextBox.Select();
            using(SQLiteConnection ConnString = new SQLiteConnection(connString.Connection))
            {
                try
                {
                    SQLiteCommand Command = new SQLiteCommand("SELECT ItemName FROM tblStock",ConnString);
                    ConnString.Open();
                    AutoCompleteStringCollection Collection = new AutoCompleteStringCollection();
                    SQLiteDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        Collection.Add(reader.GetString(0));
                    }
                    ItemIDTextBox.AutoCompleteCustomSource = Collection;
                    ItemIDTextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
                    reader.Close();
                    ConnString.Close();
                }
                catch (Exception)
                {
                    ConnString.Close();
                }
            }
        }

        private bool isEmpty()
        {
            foreach(Control c in Controls)
            {
                if(c is TextBox)
                {
                    if (c.Text == "")
                        return true;
                    return false;
                }
            }

            return true;
        }

        private void ClearEntries()
        {
            foreach(Control c in Controls)
            {
                if(c is TextBox)
                {
                    c.Text = "";
                }
            }

            MeasurementComboBox.SelectedIndex = 0;
        }

        private void CreateTable()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "CREATE TABLE IF NOT EXISTS tblDailyUse(UsageID INTEGER PRIMARY KEY ASC AUTOINCREMENT,ItemID TEXT NOT NULL,ItemName VARCHAR(60) NOT NULL," +
                          "Quantity INTEGER NOT NULL,Measurement VARCHAR(100), DATE TEXT NOT NULL)";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (isEmpty())
            {
               VMessageBox VMsg = new VMessageBox("Please fill all available fields", "Save",VMessageBox.MessageBoxType.Information);
               VMsg.ShowDialog();
               return;
            }
            if(MeasurementComboBox.Text == "")
            {
                VMessageBox VMsg = new VMessageBox("Please select a measurement", "Save", VMessageBox.MessageBoxType.Information);
                VMsg.ShowDialog();
                return;
            }
            SQLiteConnection connstring = new SQLiteConnection();
            connstring.ConnectionString = connString.Connection;
            int ItemID = int.Parse(ItemIDTextBox.Text);
            connstring.Open();
            SQLiteCommand command = new SQLiteCommand();
            command.Connection = connstring;
            command.CommandText = "SELECT Quantity FROM tblStock WHERE ItemID = '" + ItemID + "'";
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                int Current=0;
                while(reader.Read())
                { Current = reader.GetInt32(0); }
                SQLiteConnection DatabaseConnection = new SQLiteConnection();
                DatabaseConnection.ConnectionString = connString.Connection;
                DatabaseConnection.Open();
                try
                {
                    double Entry = double.Parse(QuanitityTextBox.Text);
                    double balance;
                    balance = double.Parse(Current.ToString()) - Entry;
                    if (balance < 0)
                    {
                        VMessageBox Msg = new VMessageBox("The amount of "+NameTextBox.Text+" left is "+Current+Environment.NewLine+"Please replenish your stock", "Low on Stock", VMessageBox.MessageBoxType.Information);
                        Msg.ShowDialog();
                        return;
                    }

                    SQLiteDataAdapter UpdateAdapter = new SQLiteDataAdapter("SELECT * FROM tblStock", connString.Connection);
                    UpdateAdapter.UpdateCommand = new SQLiteCommand();
                    UpdateAdapter.UpdateCommand.Connection = DatabaseConnection;
                    UpdateAdapter.UpdateCommand.CommandText = "UPDATE tblStock SET Quantity = @qty  WHERE((ItemID = @id ))";
                    UpdateAdapter.UpdateCommand.Parameters.Add(new SQLiteParameter("@qty") { Value = Convert.ToInt32(balance)});
                    UpdateAdapter.UpdateCommand.Parameters.Add(new SQLiteParameter("@id") { Value = ItemIDTextBox.Text });
                    UpdateAdapter.UpdateCommand.ExecuteNonQuery();
                    LogsFunction updatelog = new LogsFunction();
                    updatelog.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Stock Item added" + ItemIDTextBox.Text);
                    try
                    {
                        SQLiteDataAdapter InsertAdapter = new SQLiteDataAdapter("SELECT * FROM tblDailyUse", connString.Connection);
                        InsertAdapter.InsertCommand = new SQLiteCommand();
                        InsertAdapter.InsertCommand.Connection = DatabaseConnection;
                        InsertAdapter.InsertCommand.CommandText = "INSERT INTO tblDailyUse (ItemID,ItemName,Quantity,Measurement,Date) VALUES(@id,@name,@qty,@meas,@date)";
                        InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@id") { Value = ItemIDTextBox.Text});
                        InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@name") { Value = NameTextBox.Text });
                        InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@qty") { Value = Convert.ToInt32(QuanitityTextBox.Text) });
                        InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@meas") { Value = MeasurementComboBox.Text });
                        InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@date") { Value = DateTime.Parse(DateLabel.Text) });
                        InsertAdapter.InsertCommand.ExecuteNonQuery();
                        DatabaseConnection.Close();
                        VMessageBox VMsg = new VMessageBox("Details Saved", "Saved", VMessageBox.MessageBoxType.Information);
                        VMsg.ShowDialog();

                        LogsFunction log = new LogsFunction();
                        log.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "New stock utem inserted.ItemID "+ItemIDTextBox.Text);

                    }
                    catch (Exception v)
                    {
                        VMessageBox VMsg = new VMessageBox(v.Message, "Error", VMessageBox.MessageBoxType.Error);
                        VMsg.ShowDialog();
                        DatabaseConnection.Close();
                    }
                }
                catch (Exception)
                {
                    VMessageBox VMsg = new VMessageBox("The value of quantity you entered is not valid", "Save", VMessageBox.MessageBoxType.Information);
                    VMsg.ShowDialog();
                    return;
                }
            }

            ClearEntries();
        }

        private void ItemIDTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SQLiteCommand command = new SQLiteCommand();
                SQLiteConnection DatabaseConnection = new SQLiteConnection();
                DatabaseConnection.ConnectionString = connString.Connection;
                try
                {
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
                    }
                    else
                    {
                    VMessageBox VMsg = new VMessageBox("Not Item with the ID: "+ItemIDTextBox.Text+" exits", "Item Not Found", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                    NameTextBox.Text = "";
                    }

                    Reader.Close();
                    DatabaseConnection.Close();
                }
                catch (Exception v)
                {
                    MessageBox.Show(v.Message);
                    DatabaseConnection.Close();
                }
                
            }
        }

        private void btnGetID_Click(object sender, EventArgs e)
        {
            FrmViewStock view = new FrmViewStock();
            view.Mode = "SelectionDaily";
            view.ShowDialog();
        }

        private bool hasNumber(string value)
        {
            int parsedvalue;
            if (!int.TryParse(ItemIDTextBox.Text, out parsedvalue))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void NumericFetch()
        {
            SQLiteCommand command = new SQLiteCommand();
            SQLiteConnection DatabaseConnection = new SQLiteConnection();
            DatabaseConnection.ConnectionString = connString.Connection;
            try
            {
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
                }
                else
                {
                    NameTextBox.Text = "";
                }

                Reader.Close();
                DatabaseConnection.Close();
            }
            catch (Exception)
            {
                DatabaseConnection.Close();
                NameTextBox.Text = "";
            }
        }

        private void StringFetch()
        {
            SQLiteCommand command = new SQLiteCommand();
            SQLiteConnection DatabaseConnection = new SQLiteConnection();
            DatabaseConnection.ConnectionString = connString.Connection;
            try
            {
                DatabaseConnection.Open();
                command.Connection = DatabaseConnection;
                command.CommandText = "SELECT * FROM  tblStock WHERE ItemName = '" + ItemIDTextBox.Text + "'";
                SQLiteDataReader Reader = command.ExecuteReader();
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        var price = Reader["ItemName"];
                        var itemID = Reader["ItemID"];
                        NameTextBox.Text = price.ToString();
                        ItemIDTextBox.Text = itemID.ToString();
                    }
                }
                else
                {
                    NameTextBox.Text = "";
                }

                Reader.Close();
                DatabaseConnection.Close();
            }
            catch (Exception)
            {
                DatabaseConnection.Close();
                NameTextBox.Text = "";
            }
        }

        private void ItemIDTextBox_TextChanged(object sender, EventArgs e)
        {
            if(hasNumber(ItemIDTextBox.Text) == true)
            {
                NumericFetch();
            }
            else
            {
                StringFetch();
            }
           
        }

        private void FrmDailyUse_FormClosing(object sender, FormClosingEventArgs e)
        {
            for(int i = 0; i < Application.OpenForms.Count; i++)
            {
                if(Application.OpenForms[i].Name == "FrmExpectation")
                {
                    FrmExpectation expFrm = (FrmExpectation)Application.OpenForms[i];
                    expFrm.Reload();
                    ObjSender = "";
                }
            }
        }
    }
}
