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
    public partial class FrmLogIn : Form
    {
        ConnectionString connString = new ConnectionString();
        public FrmLogIn()
        {
            InitializeComponent();
        }

        private void BtnQuit_Click(object sender, EventArgs e)
        {
            LogsFunction logs = new LogsFunction();
            logs.Logs("N/A", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "System terminated");
            Application.Exit();
        }

        private void CreateDatabaseFile()
        {
           //SQLiteConnection.CreateFile("CanteenDB.sqlite");
        }

        private void BtnLogIn_Click(object sender, EventArgs e)
        {
            try {
                if (String.IsNullOrEmpty(UserIDTextBox.Text) || String.IsNullOrEmpty(PasswordTextBox.Text))
                {
                    VMessageBox VMsg = new VMessageBox("Make Sure Both UserID and Password Are entered", "Log In", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                    return;
                }

                Cursor = Cursors.WaitCursor;

                SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
                sqliteConn.Open();
                string sql  = "SELECT [AccessLevel] FROM tblUser WHERE UserID = '" + UserIDTextBox.Text + "' AND Password ='" + PasswordTextBox.Text + "' AND Status = 'Active' ";
                SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
                SQLiteDataReader reader = sqliteCmd.ExecuteReader();
                string AccessLevel = "";
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        AccessLevel = reader.GetString(0);
                    }
                    reader.Close();
                    sqliteConn.Close();
                    if (AccessLevel == "User")
                    {
                        PasswordTextBox.Text = "";
                        Cursor = Cursors.Arrow;
                        Properties.Settings.Default.CurrentUser = UserIDTextBox.Text;
                        FrmMainWinodw Main = new FrmMainWinodw();
                        if (!(Properties.Settings.Default.OrgName == ""))
                        {
                            Main.Text = Properties.Settings.Default.OrgName;
                        }
                        Main.Text = Main.Text + " - Logged In As: " + UserIDTextBox.Text;
                        UserIDTextBox.Text = "";
                        Main.foodsToolStripMenuItem.Enabled = false;
                        Main.organizationNameToolStripMenuItem.Enabled = false;
                        Main.restoreDatabaseToolStripMenuItem.Enabled = false;
                        Main.addEmployeeToolStripMenuItem.Enabled = false;
                        Main.employeesToolStripMenuItem.Enabled = false;
                        Main.logsToolStripMenuItem.Enabled = false;
                        Main.BtnAddUser.Enabled = false;
                        Main.BtnSalesReport.Enabled = false;
                        Main.BtnUsers.Enabled = false;
                        LogsFunction logs = new LogsFunction();
                        logs.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Properties.Settings.Default.CurrentUser.ToString() + " logged in");
                        Main.Show();
                        this.Hide();
                    }
                    else
                    {
                        PasswordTextBox.Text = "";
                        Cursor = Cursors.Arrow;
                        Properties.Settings.Default.CurrentUser = UserIDTextBox.Text;
                        FrmMainWinodw Main = new FrmMainWinodw();
                        if (!(Properties.Settings.Default.OrgName == ""))
                        {
                            Main.Text = Properties.Settings.Default.OrgName;
                        }
                        Main.Text = Main.Text + " - Logged In As: " + UserIDTextBox.Text + " - Administrator";
                        UserIDTextBox.Text = "";
                        LogsFunction logs = new LogsFunction();
                        logs.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Properties.Settings.Default.CurrentUser.ToString() + " logged in");
                        Main.Show();
                        this.Hide();
                    }
                }
                else
                {
                    Cursor = Cursors.Arrow;
                    VMessageBox VMsg = new VMessageBox("Error wrong password or Username", "Log In", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                    PasswordTextBox.Text = "";
                    PasswordTextBox.Select();
                    ForgotPasswordLink.Visible = true;
                    reader.Close();
                }
                sqliteConn.Close();
            }catch(Exception ex)
            {
                //DO NOTHING
                MessageBox.Show(ex.Message);
                Cursor = Cursors.Arrow;
            }
        }

        public void FrmLogIn_Load(object sender, EventArgs e)
        {
            //CreateDatabaseFile();
            this.Cursor = Cursors.AppStarting;
            if (Properties.Settings.Default.Administrator == true)
            {
                CreateAdministratorLabel.Visible = false;
                CreateUserLabel.Visible  = true;
            }
            else
            {
                CreateAdministratorLabel.Visible = true;
                CreateUserLabel.Visible = false;
            }
            UserIDTextBox.Select();
            this.Cursor = Cursors.Arrow;
            LogsFunction logs = new LogsFunction();
            logs.Logs("N/A", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "System started");
        }

        private void PasswordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnLogIn_Click(sender, e);
            }
        }

        private void BtnLogIn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnLogIn_Click(sender, e);
            }
        }

        private void UserIDTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnLogIn_Click(sender, e);
            }
        }

        private void CreateAdministratorLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmAddUser AddUser = new FrmAddUser();
            AddUser.AccessComboBox.SelectedIndex = 1;
            AddUser.AccessComboBox.Enabled = false;
            AddUser.ShowDialog();
            FrmLogIn_Load(sender, e);
        }

        private void CreateUserLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UserIDTextBox.Select();
            FrmAddUser AddUser = new FrmAddUser();
            AddUser.ShowDialog();
        }

        private void chkShowHide_CheckedChanged(object sender, EventArgs e)
        {
            if(chkShowHide.CheckState == CheckState.Checked)
            {
                PasswordTextBox.PasswordChar = '\0';
                PasswordTextBox.Select();
            }
            else
            {
                PasswordTextBox.PasswordChar = '*';
                PasswordTextBox.Select();
            }
        }

        private void ForgotPasswordLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmPassword password = new FrmPassword();
            password.ShowDialog();
        }
    }
}
