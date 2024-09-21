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
    public partial class FrmAddUser : Form
    {
        ConnectionString connString = new ConnectionString();
        public FrmAddUser()
        {
            InitializeComponent();
        }

        private void CreateTable()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "CREATE TABLE IF NOT EXISTS tblUser(UserID VARCHAR(100) PRIMARY KEY,Email VARCHAR(100),Password VARCHAR(50)," +
                         "AccessLevel VARCHAR(50),Status VARCHAR(20) DEFAULT('Active'))";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void PasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            ConfirmTextBox.Text = PasswordTextBox.Text;
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (isEmpty())
            {
                VMessageBox VMsg = new VMessageBox("Please fill all available fields", "Save", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;
            }

            if (AccessComboBox.Text == "")
            {
                VMessageBox VMsg = new VMessageBox("Please Select AccessLevel", "Save", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;

            }
            if (ConfirmTextBox.Text.Trim() != PasswordTextBox.Text.Trim() )
            {
                VMessageBox VMsg = new VMessageBox("Your passwords do not match", "Save", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;

            }
            Cursor = Cursors.WaitCursor;
            ConnectionString ConnString = new ConnectionString();
            SQLiteConnection sqliteConn = new SQLiteConnection(ConnString.Connection);
            try
            {
                sqliteConn.Open();
                string sql = "SELECT * FROM tblUser WHERE [AccessLevel] = '" + AccessComboBox.Text + "'";
                SQLiteCommand sqliteCmd = new SQLiteCommand(sql,sqliteConn);
                SQLiteDataReader reader = sqliteCmd.ExecuteReader();
                if (reader.HasRows && AccessComboBox.Text == "Administrator")
                {
                    VMessageBox VMsg = new VMessageBox("You cannot be the administrator.An Administrator already exists", "Save", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                    reader.Close();
                    return;
                }
                else
                {
                    reader.Close();
                    string command = "INSERT INTO tblUser (UserID,Email,Password,AccessLevel) VALUES(@id,@email,@password,@access)";
                    SQLiteCommand InsertCommand = new SQLiteCommand(command, sqliteConn);
                    InsertCommand.Parameters.Add(new SQLiteParameter("@id") { Value = UserIDTextBox.Text });
                    InsertCommand.Parameters.Add(new SQLiteParameter("@email") { Value = txtEmail.Text });
                    InsertCommand.Parameters.Add(new SQLiteParameter("@password") { Value = PasswordTextBox.Text });
                    InsertCommand.Parameters.Add(new SQLiteParameter("@access") { Value = AccessComboBox.Text });
                    InsertCommand.ExecuteNonQuery();
                    sqliteConn.Close();
                    VMessageBox VMsg = new VMessageBox("User Created ",UserIDTextBox.Text, VMessageBox.MessageBoxType.Information);
                    VMsg.ShowDialog();
                    Properties.Settings.Default.Administrator = true;
                    Properties.Settings.Default.Save();
                    LogsFunction log = new LogsFunction();
                    if(Properties.Settings.Default.CurrentUser.ToString()=="")
                    {
                        log.Logs("N/A", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Saved new user. UserID is " + UserIDTextBox.Text);
                    }
                    else
                    {
                        log.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Saved new user. UserID is " + UserIDTextBox.Text);
                    }
                    
                }
                reader.Close();
                sqliteConn.Close();
                clear();
                this.Close();
            }
            catch (Exception v)
            {
                VMessageBox VMsg = new VMessageBox(v.Message, "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                sqliteConn.Close();
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
           
        }
        public void clear()
        {
            UserIDTextBox.Text = "";
            PasswordTextBox.Text = "";
            ConfirmTextBox.Text = "";
            AccessComboBox.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clear();
            this.Dispose();
        }

        private void FrmAddUser_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            UserIDTextBox.Select();
            CreateTable();
        }

        private void textBoxTextChanged(object sender, EventArgs e)
        {
            var textbox = (TextBox)sender;
            if (textbox.TextLength > 100)
            {
                VMessageBox VMsg = new VMessageBox("Maximum number of characters allowed is 100", "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                textbox.Text = "";
                return;
            }
        }

        private void TextBox_KeyDown(object sender,KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                BtnSave_Click(sender, e);
            }
        }
    }
}
