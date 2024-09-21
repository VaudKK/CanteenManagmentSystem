using System;
using System.Windows.Forms;
using System.Data.SQLite;

namespace CanteenManagmentSystem
{
    public partial class FrmUserInfo : Form
    {
        string username = "",newusername= "";
        public FrmUserInfo()
        {
            InitializeComponent();
        }

        private void FrmUserInfo_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            txtUserName.Text = Properties.Settings.Default.CurrentUser.ToString();
            username = Properties.Settings.Default.CurrentUser.ToString();
            txtemail.Text = GetMyEmail(username);
        }

        private string GetMyEmail(string UserName)
        {
            string email = "";
            ConnectionString connString = new ConnectionString();
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            sqlConn.Open();
            try
            {
                SQLiteCommand sqlCmd = new SQLiteCommand();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "SELECT [Email] FROM tblUser WHERE [UserID] = '" + UserName + "'";
                SQLiteDataReader sqlReader = sqlCmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    email = sqlReader.GetString(0);
                }
                sqlReader.Close();
            }catch(Exception ex)
            {
                VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
            }
            finally
            {
                sqlConn.Close();
            }

            return email;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (isEmpty())
            {
                VMessageBox VMsg = new VMessageBox("Please fill all available fields", "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;
            }
            ConnectionString connString = new ConnectionString();
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            sqlConn.Open();
            try
            {
                SQLiteCommand sqlCmd = new SQLiteCommand();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "UPDATE tblUser SET [UserID] = @id , [Email] = @email WHERE [UserID] = @old";
                sqlCmd.Parameters.Add(new SQLiteParameter("@id") { Value = txtUserName.Text });
                sqlCmd.Parameters.Add(new SQLiteParameter("@email") { Value = txtemail.Text });
                sqlCmd.Parameters.Add(new SQLiteParameter("@old") { Value = username });
                sqlCmd.ExecuteNonQuery();
                newusername = txtUserName.Text;
                VMessageBox VMsg = new VMessageBox("Details Updated", "Updated", VMessageBox.MessageBoxType.Information);
                VMsg.ShowDialog();
                UpdateInformation();
                //NOT NECESSARY
                //LogsFunction log = new LogsFunction();
                //log.Logs(Properties.Settings.Default.CurrentUser, DateTime.Now, "Updated user information");
                this.Close();
            }
            catch(Exception ex)
            {
                VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
            }
            finally
            {
                sqlConn.Close();
            }
        }

        private void UpdateInformation()
        {
            if(!(newusername == Properties.Settings.Default.CurrentUser))
            {
                Properties.Settings.Default.CurrentUser = newusername;
                Properties.Settings.Default.Save();
                for (int i = 0; i < Application.OpenForms.Count; i++)
                {
                    if(Application.OpenForms[i].Name == "FrmMainWinodw")
                    {
                        FrmMainWinodw main = (FrmMainWinodw)Application.OpenForms[i];
                        if (main.Text.Contains("Administrator"))
                        {
                            main.Text = Properties.Settings.Default.OrgName + " - Logged In As: " + newusername + " - Administrator";
                        }
                        else
                        {
                            main.Text = Properties.Settings.Default.OrgName + " - Logged In As: " + newusername;
                        }
                    }
                }
            }
        }

        private bool isEmpty()
        {
            foreach(Control c in Controls){
                if(c is TextBox)
                {
                    if (c.Text == "")
                        return true;
                    return false;
                }
            }

            return false;
        }
    }
}
