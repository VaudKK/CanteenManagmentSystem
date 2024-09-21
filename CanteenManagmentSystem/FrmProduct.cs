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

namespace CanteenManagmentSystem
{
    public partial class FrmProduct : Form
    {
        ConnectionString connString = new ConnectionString();
        public string OriginalID;
        public FrmProduct()
        {
            InitializeComponent();
        }

        private void DropTable()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "DROP TABLE tblItems";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void CreateTable()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "CREATE TABLE IF NOT EXISTS tblItems(ProductID INTEGER PRIMARY KEY ASC NOT NULL,ProductName TEXT NOT NULL,Price FLOAT NOT NULL)";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (isEmpty())
            {
                VMessageBox VMsg = new VMessageBox("The following fields must be filled:"+Environment.NewLine+"ProductID"+Environment.NewLine+ "ProductName" + Environment.NewLine + "Price and Ingredients", "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;
            }
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            if (BtnSave.Text == "Save")
            {
                try
                {
                    sqlConn.Open();
                    string CommandText = "INSERT INTO tblItems(ProductID,ProductName,Price) VALUES(@id,@name,@price)";
                    SQLiteCommand sqlCmd = new SQLiteCommand(CommandText, sqlConn);
                    sqlCmd.Parameters.Add(new SQLiteParameter("@id") { Value = Convert.ToInt32(txtProductID.Text)});
                    sqlCmd.Parameters.Add(new SQLiteParameter("@name") { Value = txtProductName.Text.Trim() });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@price") { Value = Convert.ToDouble(txtPrice.Text)});
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    VMessageBox VMsg = new VMessageBox("Details Saved", "Saved", VMessageBox.MessageBoxType.Information);
                    VMsg.ShowDialog();
                    LogsFunction logs = new LogsFunction();
                    logs.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Saved new product. Product ID is " + txtProductID.Text);
                    ClearTextBox();
                }
                catch (Exception ex)
                {
                    VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                }
                finally
                {
                    sqlConn.Close();
                    txtProductID.Select();
                }
            } else if(BtnSave.Text == "Update")
            {
                try
                {
                    
                    sqlConn.Open();
                    string UpdateCommand = "UPDATE tblItems SET ProductID = @id ,ProductName = @name ,Price = @price WHERE ProductID = @original";
                    SQLiteCommand sqlCmd = new SQLiteCommand(UpdateCommand, sqlConn);
                    sqlCmd.Parameters.Add(new SQLiteParameter("@id") { Value = Convert.ToInt32(txtProductID.Text) });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@name") { Value = txtProductName.Text.Trim() });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@price") { Value = Convert.ToDouble(txtPrice.Text) });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@original") { Value = Convert.ToInt32(OriginalID) });
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    VMessageBox VMsg = new VMessageBox("Details updated", "Updated", VMessageBox.MessageBoxType.Information);
                    VMsg.ShowDialog();
                    LogsFunction logs = new LogsFunction();
                    logs.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Updated product with the ID " + txtProductID.Text);
                    ClearTextBox();
                    
                }
                catch (Exception ex)
                {
                   VMessageBox VMsg = new VMessageBox(ex.Message, "Error",VMessageBox.MessageBoxType.Error);
                   VMsg.ShowDialog();
                }
                finally
                {
                    sqlConn.Close();
                    BtnSave.Text = "Save";
                    txtProductID.Select();
                }
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            BtnSave.Text = "Save";
            ClearTextBox();
            FrmViewProducts viewPdcts = new FrmViewProducts();
            viewPdcts.ShowDialog();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            FrmViewProducts view = new FrmViewProducts();
            view.Mode = "Edit";
            view.ShowDialog();
        }

        private void FrmProduct_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            txtProductID.Select();
            CreateTable();
        }

        private Boolean isEmpty()
        {
            if (txtProductID.Text == "" || txtProductName.Text == "" || txtPrice.Text == "")
                return true;
            return false;
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar)))
                e.Handled = true;
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
        }

        private void txtProductID_TextChanged(object sender, EventArgs e)
        {
            if(txtProductID.TextLength > 7)
            {
                VMessageBox VMsg = new VMessageBox("Maximum amount of character allowed is 7", "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                txtProductID.Text = "";
            }
        }

        private void txtProductID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!(char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            txtProductID.Select();
            BtnSave.Text = "Save";
        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            /*
            string value = txtProductName.Text;
            for(int i = 0; i < value.Length; i++)
            {
                if (char.IsNumber(value[i]))
                {
                    VMessageBox VMsg = new VMessageBox("No numeric characters allowed", "Invalid Input", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                    txtProductName.Text = "";
                    return;
                }
            }
            */
        }
    }
}
