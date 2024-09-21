using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using PrinterUtility;

namespace CanteenManagmentSystem
{
    public partial class FrmMakeASale : Form
    {
        string ItemName;
        string StockItemID;
        ConnectionString connString = new ConnectionString();
        byte[] ByteData = new byte[1];
        public FrmMakeASale()
        {
            InitializeComponent();
        }
       
        private void FrmMakeASale_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            setDefaultValues();
            ItemTextBox.Select();
            CreateTable();
            CreateDailyUseTable();
            try
            {
                using(SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection))
                {
                    AutoCompleteStringCollection Collection = new AutoCompleteStringCollection();
                    SQLiteCommand sqlCmd = new SQLiteCommand("SELECT ProductName FROM tblItems", sqlConn);
                    sqlConn.Open();
                    SQLiteDataReader sqlReader = sqlCmd.ExecuteReader();
                    while (sqlReader.Read())
                    {
                        Collection.Add(sqlReader.GetString(0));
                    }
                    ItemTextBox.AutoCompleteCustomSource = Collection;
                    ItemTextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
                    sqlReader.Close();
                    sqlConn.Close();
                }
            }catch(Exception)
            {
                 //DO NOTHING
            }
        }

        private void CreateTable()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "CREATE TABLE IF NOT EXISTS tblSales(SalesID INTEGER PRIMARY KEY ASC AUTOINCREMENT,ItemList TEXT NOT NULL,TimeOfSale TEXT NOT NULL," +
                          "TotalCost FLOAT NOT NULL,ModeOfPayment VARCHAR(20) NOT NULL,OrderType VARCHAR(20) NOT NULL,TransactionNo VARCHAR(100) NOT NULL,User VARCHAR(200) NOT NULL)";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void DropTable()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "DROP TABLE tblSales";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void SaveTransaction()
        {
            SQLiteConnection DatabaseConnection = new SQLiteConnection(connString.Connection);
            try
            {
                SQLiteDataAdapter InsertAdapter = new SQLiteDataAdapter("SELECT * FROM tblSales", DatabaseConnection);
                InsertAdapter.InsertCommand = new SQLiteCommand();
                InsertAdapter.InsertCommand.Connection = DatabaseConnection;
                InsertAdapter.InsertCommand.CommandText = "INSERT INTO tblSales ([ItemList],[TimeOfSale],[TotalCost],[ModeOfPayment],[OrderType],[TransactionNo],[User]) VALUES(@list,@time,@cost,@mop,@type,@trans,@user)";
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@list") { Value = GetItemList() });
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@time") { Value = DateTime.Now .ToString("yyyy-MM-dd HH:mm:ss")});
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@cost") { Value = Convert.ToDouble(TotalCostLabel.Text) });
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@type") { Value = OrderTypeComboBox.Text });
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@trans") { Value = txtTransactionNo.Text });
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@mop") { Value = MODComboBox.Text });
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@user") { Value = Properties.Settings.Default.CurrentUser.ToString()});
                DatabaseConnection.Open();
                InsertAdapter.InsertCommand.ExecuteNonQuery();
                DatabaseConnection.Close();
                getListAndUpdate();     
                LogsFunction log = new LogsFunction();
                log.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "New sale made.Items: " + GetItemList());
                VMessageBox VMsg = new VMessageBox("Sold", "Item Sold", VMessageBox.MessageBoxType.Information);
                VMsg.ShowDialog();
            }
            catch (Exception ex)
            {
                VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
            }
            finally
            {
                DatabaseConnection.Close();
            }
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (AmountPaidTextBox.Text == "")
            {
                VMessageBox VMsg = new VMessageBox("Please enter amount paid", "Save", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;
            }
            if (OrderTypeComboBox.Text == "")
            {
                VMessageBox VMsg = new VMessageBox("Please Select OrderType", "Save", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;

            }

            if (txtTransactionNo.Text == "")
            {
                VMessageBox VMsg = new VMessageBox("Please enter Transaction No", "Save", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;

            }

            if (MODComboBox.Text == "")
            {
                VMessageBox VMsg = new VMessageBox("Please Select mode Of Payment", "Save", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;

            }
            SaveTransaction();
            clear();
            setDefaultValues();
            ClearList();
        }

        private string GetItemList()
        {
            bool firstEntry = true;
            string orderlist = null;
            if(ItemsList.Items.Count > 0)
            {
                foreach(ListViewItem item in ItemsList.Items)
                {
                    if (firstEntry)
                    {
                        orderlist += item.SubItems[0].Text + " x " + item.SubItems[2].Text + "  @" + item.SubItems[1].Text;
                        firstEntry = false;
                    }
                    else
                    {
                        orderlist += "," + item.SubItems[0].Text + " x " + item.SubItems[2].Text + "  @" + item.SubItems[1].Text; 
                    }
                }

                return orderlist;
            }

            return "";
        }

        private void getListAndUpdate()
        {
            if (ItemsList.Items.Count > 0)
            {
                foreach (ListViewItem item in ItemsList.Items)
                {
                    UpdateStock(getItemStockID(item.SubItems[0].Text), Convert.ToInt32(item.SubItems[2].Text));
                    InsertIntoDailyUse(getItemProductID(item.SubItems[0].Text), item.SubItems[0].Text, Convert.ToInt32(item.SubItems[2].Text));
                }
            }
       }

        public void clear()
        {
            ItemTextBox.Text = "";
            PriceTextBox.Text = "";
            AmountPaidTextBox.Text = "";
            ItemTextBox.Select();
        }

        private void QuatityTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!(char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void CreateDailyUseTable()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "CREATE TABLE IF NOT EXISTS tblDailyUse(UsageID INTEGER PRIMARY KEY ASC AUTOINCREMENT,ItemID TEXT NOT NULL,ItemName VARCHAR(60) NOT NULL," +
                          "Quantity INTEGER NOT NULL,Date TEXT NOT NULL)";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
       }

    
        private void DropTableDU()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "DROP TABLE tblDailyUse";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void InsertIntoDailyUse(string ItemID,string ItemName, int Quantity)
        {
            SQLiteConnection DatabaseConnection = new SQLiteConnection(connString.Connection);
            DatabaseConnection.Open();
            SQLiteCommand sqlCmd = new SQLiteCommand("SELECT * FROM tblDailyUse WHERE ItemID = '" + ItemID + "' AND ItemName = '" + ItemName + "' AND Date = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' COLLATE NOCASE",DatabaseConnection);
            SQLiteDataReader sqlReader = sqlCmd.ExecuteReader();
            int currentAmount = 0;

            //update if record exists

            if (sqlReader.HasRows)
            {
                //get current quantity
                while (sqlReader.Read())
                {
                    currentAmount = sqlReader.GetInt32(3);
                }

                currentAmount += Quantity;

                try
                {
                    SQLiteDataAdapter UpdateAdapter = new SQLiteDataAdapter("SELECT * FROM tblDailyUse", connString.Connection);
                    UpdateAdapter.UpdateCommand = new SQLiteCommand();
                    UpdateAdapter.UpdateCommand.Connection = DatabaseConnection;
                    UpdateAdapter.UpdateCommand.CommandText = "UPDATE tblDailyUse SET Quantity = @qty  WHERE((ItemID = @id AND Date = @date))";
                    UpdateAdapter.UpdateCommand.Parameters.Add(new SQLiteParameter("@qty") { Value = currentAmount});
                    UpdateAdapter.UpdateCommand.Parameters.Add(new SQLiteParameter("@id") { Value = ItemID});
                    UpdateAdapter.UpdateCommand.Parameters.Add(new SQLiteParameter("@date") { Value = DateTime.Now.ToString("yyyy-MM-dd") });
                    UpdateAdapter.UpdateCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    MessageBox.Show("error A");
                }
                finally
                {
                    DatabaseConnection.Close();
                }

            }
            else
            {
                //add new if record does not exist

                try
                {
                    SQLiteDataAdapter InsertAdapter = new SQLiteDataAdapter("SELECT * FROM tblDailyUse", connString.Connection);
                    InsertAdapter.InsertCommand = new SQLiteCommand();
                    InsertAdapter.InsertCommand.Connection = DatabaseConnection;
                    InsertAdapter.InsertCommand.CommandText = "INSERT INTO tblDailyUse (ItemID,ItemName,Quantity,Date) VALUES(@id,@name,@qty,@date)";
                    InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@id") { Value = ItemID });
                    InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@name") { Value = ItemName });
                    InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@qty") { Value = Convert.ToInt32(Quantity) });
                    InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@date") { Value = DateTime.Now.ToString("yyyy-MM-dd") });
                    InsertAdapter.InsertCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    MessageBox.Show("error B");
                }
                finally
                {
                    DatabaseConnection.Close();
                }
            }
        }


        private void DishTextBox_TextChanged(object sender, EventArgs e)
        {
            //No Code
        }

        private void DishTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ConnectionString connString = new ConnectionString();
                SQLiteCommand command = new SQLiteCommand();
                SQLiteConnection DatabaseConnection = new SQLiteConnection(connString.Connection);
                DatabaseConnection.Open();
                command.Connection = DatabaseConnection;
                command.CommandText = "SELECT * FROM  tblItems WHERE ProductID = '" + ItemTextBox.Text + "'";
                SQLiteDataReader Reader = command.ExecuteReader();
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        var price = Reader["Price"];
                        var productName = Reader["ProductName"];
                        PriceTextBox.Text = price.ToString();
                        ItemName = productName.ToString();
                    }
                    Reader.Close();
                }
                Reader.Close();
                DatabaseConnection.Close();
            }
        }

        private void NumericFetch()
        {
            ConnectionString connString = new ConnectionString();
            SQLiteCommand command = new SQLiteCommand();
            SQLiteConnection DatabaseConnection = new SQLiteConnection(connString.Connection);
            try
            {
                PriceTextBox.Text = "";
                DatabaseConnection.Open();
                command.Connection = DatabaseConnection;
                command.CommandText = "SELECT * FROM  tblItems WHERE ProductID = '" + ItemTextBox.Text + "'";
                SQLiteDataReader Reader = command.ExecuteReader();
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        var price = Reader["Price"];
                        var productName = Reader["ProductName"];
                        var productID = Reader["ProductID"];
                        PriceTextBox.Text = price.ToString();
                        ItemName = productName.ToString();
                        ItemTextBox.Text = productID.ToString();
                    }
                    Reader.Close();
                }
                Reader.Close();
                DatabaseConnection.Close();
            }
            catch (Exception ex)
            {
                VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                DatabaseConnection.Close();
                PriceTextBox.Text = "";
            }
        }
        
        private void StringFetch()
        {
            ConnectionString connString = new ConnectionString();
            SQLiteCommand command = new SQLiteCommand();
            SQLiteConnection DatabaseConnection = new SQLiteConnection(connString.Connection);
            try
            {
                PriceTextBox.Text = "";
                DatabaseConnection.Open();
                command.Connection = DatabaseConnection;
                command.CommandText = "SELECT * FROM  tblItems WHERE ProductName = '" + ItemTextBox.Text + "'";
                SQLiteDataReader Reader = command.ExecuteReader();
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        var price = Reader["Price"];
                        var productName = Reader["ProductName"];
                        var productID = Reader["ProductID"];
                        PriceTextBox.Text = price.ToString();
                        ItemName = productName.ToString();
                        ItemTextBox.Text = productID.ToString();
                    }
                    Reader.Close();
                }
                Reader.Close();
                DatabaseConnection.Close();
            }
            catch (Exception ex)
            {
                VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                DatabaseConnection.Close();
                PriceTextBox.Text = "";
            }
        }


        private void ClearList()
        {
            foreach(ListViewItem item in ItemsList.Items)
            {
                ItemsList.Items.RemoveAt(item.Index);
            }
        }
        private void BtnClose_Click(object sender, EventArgs e)
        {
            clear();
            this.Dispose();
        }

        private void BtnSaveAndPrint_Click(object sender, EventArgs e)
        {
            if (AmountPaidTextBox.Text == "")
            {
                VMessageBox VMsg = new VMessageBox("Please enter amount paid", "Save", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;
            }

            if (txtTransactionNo.Text == "")
            {
                VMessageBox VMsg = new VMessageBox("Please enter Transaction No", "Save", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;
            }

            if (OrderTypeComboBox.Text == "")
            {
                VMessageBox VMsg = new VMessageBox("Please Select OrderType", "Save", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;

            }
            if (MODComboBox.Text == "")
            {
                VMessageBox VMsg = new VMessageBox("Please Select mode Of Payment", "Save", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;

            }
            SaveTransaction();
            
            PrinterUtility.EscPosEpsonCommands.EscPosEpson obj = new PrinterUtility.EscPosEpsonCommands.EscPosEpson();
            ByteData = PrintExtensions.AddBytes(ByteData, obj.CharSize.DoubleHeight5());
            ByteData = PrintExtensions.AddBytes(ByteData, obj.Alignment.Center());
            ByteData = PrintExtensions.AddBytes(ByteData, Encoding.ASCII.GetBytes(Properties.Settings.Default.OrgName.ToString() + "\n\n\n"));
            ByteData = PrintExtensions.AddBytes(ByteData, obj.CharSize.Nomarl());
            ByteData = PrintExtensions.AddBytes(ByteData, obj.Alignment.Left());
            ByteData = PrintExtensions.AddBytes(ByteData, string.Format("{0,-40}{1,6}{2,9}{3,9:N2}\n", "Item","PPI", "Qty", "SubTotal"));
            ByteData = PrintExtensions.AddBytes(ByteData, obj.Separator());
            foreach(ListViewItem item in ItemsList.Items)
            {
                string Item = item.SubItems[0].Text;
                string quantity = item.SubItems[2].Text;
                string PPU = item.SubItems[1].Text;
               
                double qty = double.Parse(quantity);
                double unitprice = double.Parse(PPU);
                double sub = qty * unitprice;

                string subtotal = sub.ToString();
                ByteData = PrintExtensions.AddBytes(ByteData, string.Format("{0,-40}{1,6}{2,9}{3,9:N2}\n",Item,PPU,quantity,subtotal));
            }

            ByteData = PrintExtensions.AddBytes(ByteData, obj.Alignment.Right());
            ByteData = PrintExtensions.AddBytes(ByteData, obj.Separator());
            ByteData = PrintExtensions.AddBytes(ByteData, Encoding.ASCII.GetBytes("Total: " + TotalCostLabel.Text+"\n"));
            ByteData = PrintExtensions.AddBytes(ByteData, Encoding.ASCII.GetBytes("Amount Paid: " + AmountPaidTextBox.Text + "\n"));
            ByteData = PrintExtensions.AddBytes(ByteData, Encoding.ASCII.GetBytes("Balance: " + BalanceLabel.Text + "\n"));
            ByteData = PrintExtensions.AddBytes(ByteData, obj.Alignment.Center());
            ByteData = PrintExtensions.AddBytes(ByteData, obj.Separator());
            ByteData = PrintExtensions.AddBytes(ByteData, Encoding.ASCII.GetBytes("Served By: " + Properties.Settings.Default.CurrentUser.ToString() +"\n"));
            ByteData = PrintExtensions.AddBytes(ByteData, Encoding.ASCII.GetBytes("Time: " + DateTime.Now.ToString()));
            ByteData = PrintExtensions.AddBytes(ByteData, obj.Alignment.Left());
            ByteData = PrintExtensions.AddBytes(ByteData, CutPage());
            PrintExtensions.Print(ByteData, "\\\\localhost\\EPSON L220 Series");
            clear();
            setDefaultValues();
            ClearList();
        }

        private byte[] CutPage()
        {
            List<byte> obj = new List<byte>();
            obj.Add(Convert.ToByte(Convert.ToChar(0x1D)));
            obj.Add(Convert.ToByte('V'));
            obj.Add((byte)66);
            obj.Add((byte)3);
            return obj.ToArray();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!(ItemName == "" || PriceTextBox.Text == "" || QuatityTextBox.Text == ""))
            {
                if (isStockAvailable(ItemName))
                {
                   if(getQuantityAvailable(ItemName) > 0  && Convert.ToInt32(QuatityTextBox.Text) <= getQuantityAvailable(ItemName))
                    {
                        ListViewItem Item = new ListViewItem(ItemName);
                        Item.SubItems.Add(PriceTextBox.Text);
                        Item.SubItems.Add(QuatityTextBox.Text);
                        ItemsList.Items.Add(Item);

                        //Add the prices to get the sum total
                        double totalPrice = 0;
                        foreach (ListViewItem item in ItemsList.Items)
                        {
                            double price, totalTemp;
                            int quantity;
                            price = Convert.ToDouble(item.SubItems[1].Text);
                            quantity = Convert.ToInt32(item.SubItems[2].Text);
                            totalTemp = price * quantity;
                            totalPrice += totalTemp;
                            TotalCostLabel.Text = "" + totalPrice;
                        }

                        ItemTextBox.Text = PriceTextBox.Text = "";
                        QuatityTextBox.Text = "1";
                        ItemTextBox.Select();
                    }
                    else
                    {
                        VMessageBox VMsg = new VMessageBox("The number of "+ItemName+" available in stock is "+getQuantityAvailable(ItemName)+"."+ Environment.NewLine + "Please replenish your" +
                       " stock", "Low On Stock", VMessageBox.MessageBoxType.Error);
                        VMsg.ShowDialog();
                    }
                }
                else
                {
                    VMessageBox VMsg = new VMessageBox("The Item " + ItemName + " does not exists in your stock." + Environment.NewLine + "Make sure the name is correct or"+
                        " add the item to your stock","Item Not Available",VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                }
                
            }
        }

        private void AmountPaidTextBox_TextChanged(object sender, EventArgs e)
        {
            Double paid, total, balance;
            try
            {
                paid = double.Parse(AmountPaidTextBox.Text);
                total = double.Parse(TotalCostLabel.Text);
                balance = paid - total;
                BalanceLabel.Text = "" + balance;
            }
            catch (Exception)
            {
                BalanceLabel.Text = "0.00";
            }
        }

        private void AmountPaidTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void ItemsList_KeyDown(object sender, KeyEventArgs e)
        {
            if(ItemsList.SelectedItems.Count > 0)
            {
                if(e.KeyCode == Keys.Delete)
                {
                    ItemsList.Items.RemoveAt(ItemsList.SelectedItems[0].Index);

                    //Recalculate total
                    double totalPrice = 0;
                    foreach (ListViewItem item in ItemsList.Items)
                    {
                        double price, totalTemp;
                        int quantity;
                        price = Convert.ToDouble(item.SubItems[1].Text);
                        quantity = Convert.ToInt32(item.SubItems[2].Text);
                        totalTemp = price * quantity;
                        totalPrice += totalTemp;
                        TotalCostLabel.Text = "" + totalPrice;
                    }

                    if(ItemsList.Items.Count == 0)
                    {
                        TotalCostLabel.Text = "0.00";
                    }
                }
            }
        }

        private void setDefaultValues()
        {
            QuatityTextBox.Text = "1";
            MODComboBox.SelectedIndex = 1;
            OrderTypeComboBox.SelectedIndex = 1;
            TotalCostLabel.Text = "0.00";
            BalanceLabel.Text = "0.00";
        }

        private void btnGetItem_Click(object sender, EventArgs e)
        {
            FrmViewProducts view = new FrmViewProducts();
            view.Mode = "GetID";
            view.ShowDialog();
        }

        private bool hasNumber(string value)
        {
            /*
            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsNumber(value[i]))
                {
                    return true;
                }
            }

            return false;
            */
            int parsedvalue;
            if(!int.TryParse(ItemTextBox.Text,out parsedvalue))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ItemTextBox_TextChanged(object sender, EventArgs e)
        {

            if (hasNumber(ItemTextBox.Text) == true)
            {
                NumericFetch();
            }
            else
            {
                StringFetch();
            }
        }

        private void MODComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(MODComboBox.SelectedIndex == 2)
            {
                txtTransactionNo.Enabled = true;
                txtTransactionNo.Text = "";
            }
            else
            {
                txtTransactionNo.Enabled = false;
                txtTransactionNo.Text = "NOT APPLICABLE";
            }
        }

        private void txtTransactionNo_TextChanged(object sender, EventArgs e)
        {
            txtTransactionNo.CharacterCasing = CharacterCasing.Upper;
        }

        private bool isStockAvailable(string itemName)
        {
            bool exists = false;
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            SQLiteCommand sqlCmd = new SQLiteCommand("SELECT ItemID,ItemName FROM tblStock WHERE ItemName = '" + itemName + "'",sqlConn);
            sqlConn.Open();
            try
            {
                SQLiteDataReader sqlReader = sqlCmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    StockItemID = sqlReader.GetString(0);
                    exists = true;
                }
                sqlReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
            }
            finally
            {
                sqlConn.Close();
            }

            return exists;
        }

        private int getQuantityAvailable(string itemName)
        {
            int quantity = 0;
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            SQLiteCommand sqlCmd = new SQLiteCommand("SELECT Quantity FROM tblStock WHERE ItemName = '" + itemName + "'", sqlConn);
            sqlConn.Open();
            try
            {
                SQLiteDataReader sqlReader = sqlCmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    quantity = sqlReader.GetInt32(0);
                }
                sqlReader.Close();
            }
            catch (Exception)
            {
                //DO NOTHING
            }
            finally
            {
                sqlConn.Close();
            }

            return quantity;
        }

        private string getItemProductID(string ItemName)
        {
            string id = "";
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            SQLiteCommand sqlCmd = new SQLiteCommand("SELECT ProductID,ProductName FROM tblItems WHERE ProductName = '" + ItemName + "'", sqlConn);
            sqlConn.Open();
            try
            {
                SQLiteDataReader sqlReader = sqlCmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    id = sqlReader.GetInt32(0).ToString();
                }
                sqlReader.Close();
            }
            catch (Exception)
            {
                //DO NOTHING
            }
            finally
            {
                sqlConn.Close();
            }

            return id;
        }

        private string  getItemStockID(string itemName)
        {
            string id = "";
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            SQLiteCommand sqlCmd = new SQLiteCommand("SELECT ItemID,ItemName FROM tblStock WHERE ItemName = '" + itemName + "'", sqlConn);
            sqlConn.Open();
            try
            {
                SQLiteDataReader sqlReader = sqlCmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    id = sqlReader.GetString(0);
                }
                sqlReader.Close();
            }
            catch (Exception)
            {
                //DO NOTHING
            }
            finally
            {
                sqlConn.Close();
            }

            return id;
        }


        private void UpdateStock(string itemID,int Quantity)
        {
            SQLiteConnection DatabaseConnection = new SQLiteConnection();
            DatabaseConnection.ConnectionString = connString.Connection;
            DatabaseConnection.Open();
            try
            {
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = DatabaseConnection;
                command.CommandText = "SELECT Quantity FROM tblStock WHERE ItemID = '" + itemID + "'";
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    int Current = 0;
                    while (reader.Read())
                    { Current = reader.GetInt32(0); }


                    double Entry = double.Parse(Quantity.ToString());
                    double balance;
                    balance = double.Parse(Current.ToString()) - Entry;

                    SQLiteDataAdapter UpdateAdapter = new SQLiteDataAdapter("SELECT * FROM tblStock", connString.Connection);
                    UpdateAdapter.UpdateCommand = new SQLiteCommand();
                    UpdateAdapter.UpdateCommand.Connection = DatabaseConnection;
                    UpdateAdapter.UpdateCommand.CommandText = "UPDATE tblStock SET Quantity = @qty  WHERE((ItemID = @id ))";
                    UpdateAdapter.UpdateCommand.Parameters.Add(new SQLiteParameter("@qty") { Value = Convert.ToInt32(balance) });
                    UpdateAdapter.UpdateCommand.Parameters.Add(new SQLiteParameter("@id") { Value = itemID });
                    UpdateAdapter.UpdateCommand.ExecuteNonQuery();
                }

            }
            catch(Exception ex)
            {
                VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
            }
            finally
            {
                DatabaseConnection.Close();
            }
            
        }
    }
}
