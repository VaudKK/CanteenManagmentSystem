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
    public partial class frmPettyCash : Form
    {
        ConnectionString connString = new ConnectionString();
        public frmPettyCash()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveToDatabase();
        }

        private bool isEmpty()
        {
            foreach(Control c in this.Controls)
            {
                if(c is TextBox)
                {
                    if (c.Text == "")
                        return true;
                }
            }

            return false;
        }

        private void SaveToDatabase()
        {
            if (isEmpty())
            {
                VMessageBox VMsg = new VMessageBox("Please fill all fields", "Empty Field(s)", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;
            }

            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            SQLiteCommand sqlCmd = new SQLiteCommand();
            sqlCmd.Connection = sqlConn;
            sqlConn.Open();
            try
            {
                sqlCmd.CommandText = "INSERT INTO tblPetty(Date,Purpose,Amount) VALUES(@date,@purpose,@amt)";
                sqlCmd.Parameters.Add(new SQLiteParameter("@date") { Value = dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss") });
                sqlCmd.Parameters.Add(new SQLiteParameter("@purpose") { Value = txtPurpose.Text.Trim() });
                sqlCmd.Parameters.Add(new SQLiteParameter("@amt") { Value = Convert.ToDouble(txtAmount.Text) });
                sqlCmd.ExecuteNonQuery();
                VMessageBox VMsg = new VMessageBox("Saved", "Details saved", VMessageBox.MessageBoxType.Information);
                VMsg.ShowDialog();
                ClearTextBox();
                txtAmount.Select();
            }
            catch(Exception ex)
            {
                VMessageBox VMsg = new VMessageBox("Error",ex.Message, VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                sqlConn.Close();
            }
            finally
            {
                sqlConn.Close();
            }
        }

        private void CreateTable()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "CREATE TABLE IF NOT EXISTS tblPetty(PCID INTEGER NOT NULL  PRIMARY KEY AUTOINCREMENT,Date TEXT,Purpose TEXT NOT NULL," +
                         "Amount FLOAT NOT NULL)";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void frmPettyCash_Load(object sender, EventArgs e)
        {
            CreateTable();
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            txtAmount.Select();
        }

        private void ClearTextBox()
        {
            txtAmount.Text = txtPurpose.Text = "";
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void txtPurpose_TextChanged(object sender, EventArgs e)
        {
            string inputText = txtPurpose.Text;
            if(inputText.Length > 200)
            {
                VMessageBox VMsg = new VMessageBox("Out Of Bounds", "Maximum number of characters allowed is 200", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                txtPurpose.Text = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
