using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CanteenManagmentSystem
{
    public partial class FrmChangePassword : Form
    {
        ConnectionString conn = new ConnectionString();
        public FrmChangePassword()
        {
            InitializeComponent();
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

        private void BtnChange_Click(object sender, EventArgs e)
        {
            if (isEmpty())
            {
                VMessageBox VMsg = new VMessageBox("Please fill all available fields", "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;
            }
            SQLiteConnection connstring = new SQLiteConnection(conn.Connection);
            try
            {
                connstring.Open();
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = connstring;
                if (!(NameTextBox.Text == PasswordTextBox.Text))
                {
                    VMessageBox VMsg = new VMessageBox("Your passwords dont match", "Error", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                    PasswordTextBox.Focus();
                    return;
                }
                SQLiteDataAdapter adapter = new SQLiteDataAdapter("SELECT * FROM tbluser", connstring);
                adapter.UpdateCommand = new SQLiteCommand();
                command.CommandText = "SELECT [Password] FROM tblUser WHERE [Password] = '" + OldTextbox.Text + "' AND [UserID] = '"+UserIDLabel.Text+"'";
                SQLiteDataReader newreader = command.ExecuteReader();
                adapter.UpdateCommand.Connection = connstring;
                string oldpassword = "";
                while (newreader.Read())
                {
                    oldpassword = newreader.GetString(0);
                }
                if (OldTextbox.Text == oldpassword)
                {
                    adapter.UpdateCommand.CommandText = "UPDATE tblUser SET Password = @password WHERE ((UserID = @id))";
                    adapter.UpdateCommand.Parameters.Add(new SQLiteParameter("@password") { Value = PasswordTextBox.Text });
                    adapter.UpdateCommand.Parameters.Add(new SQLiteParameter("@id") { Value = UserIDLabel.Text });
                    newreader.Close();
                    adapter.UpdateCommand.ExecuteNonQuery();
                    connstring.Close();
                    MessageBox.Show("Password change successful", "Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Dispose();
                }
                else
                {
                    MessageBox.Show("The Old password you entered is not correct", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    OldTextbox.Text = "";
                    newreader.Close();
                    connstring.Close();
                    OldTextbox.Focus();
                }
            }
            catch (Exception v)
            {
                MessageBox.Show(v.Message, "Error");
            }
            finally
            {
                connstring.Close();
            }

        }

        private void FrmChangePassword_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            if (!(Properties.Settings.Default.CurrentUser.ToString() == ""))
            {
                UserIDLabel.Text = Properties.Settings.Default.CurrentUser.ToString();
            }
            OldTextbox.Select();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                BtnChange_Click(sender, e);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Are you sure you want to delete your account", "Delete Account",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                SQLiteConnection connstring = new SQLiteConnection(conn.Connection);
                try
                {
                    SQLiteCommand Command = new SQLiteCommand();
                    connstring.Open();
                    Command.Connection = connstring;
                    Command.CommandText = "SELECT * FROM tblUser WHERE UserID = '" + UserIDLabel.Text + "' AND AccessLevel = 'Administrator'";
                    SQLiteDataReader reader = Command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Close();
                        Command.CommandText = "DELETE FROM tblUser WHERE UserID = '" + UserIDLabel.Text + "'";
                        Command.ExecuteNonQuery();
                        Properties.Settings.Default.Administrator = false;
                        Properties.Settings.Default.Save();
                        reader.Close();
                        for (int i = 0; i < Application.OpenForms.Count; i++)
                        {
                            if (!(Application.OpenForms[i].Name == "FrmLogIn"))
                            {
                                Application.OpenForms[i].Close();
                            }
                        }
                        for(int i=0;i<Application.OpenForms.Count;i++)
                        {
                            if (Application.OpenForms[i].Name == "FrmLogIn")
                            {
                                FrmLogIn form = (FrmLogIn)Application.OpenForms[i];
                                form.FrmLogIn_Load(sender, e);
                            }
                        }
                        reader.Close();
                        return;
                    }

                    reader.Close();
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter("SELECT * FROM tbluser", connstring);
                    adapter.UpdateCommand = new SQLiteCommand();
                    adapter.UpdateCommand.Connection = connstring;
                    adapter.UpdateCommand.CommandText = "UPDATE tblUser SET [Status] = @status WHERE (([UserID] = @id))";
                    adapter.UpdateCommand.Parameters.Add(new SQLiteParameter("@status") { Value = "Deactivated" });
                    adapter.UpdateCommand.Parameters.Add(new SQLiteParameter("@id") { Value = UserIDLabel.Text });
                    adapter.UpdateCommand.ExecuteNonQuery();
                    LogsFunction logs = new LogsFunction();
                    logs.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), "User account deleted. User ID is " + UserIDLabel.Text);
                    for (int i = 0; i < Application.OpenForms.Count; i++)
                    {
                        if (!(Application.OpenForms[i].Name == "FrmLogIn"))
                        {
                            Application.OpenForms[i].Close();
                        }
                    }
                }
                catch (Exception v)
                {
                    VMessageBox VMsg = new VMessageBox(v.Message, "Error",VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                }
                finally
                {
                connstring.Close();
                }
            }
        }
    }
}
