using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.NetworkInformation;

namespace CanteenManagmentSystem
{
    public partial class FrmPassword : Form
    {
        private string newpassword;
        public FrmPassword()
        {
            InitializeComponent();
        }

        private void FrmPassword_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Text = "Password Recovery";
            txtUserName.Select();
        }

        private void AutoCompleteEmail(string username)
        {
            ConnectionString conn = new ConnectionString();

            using (SQLiteConnection connstring = new SQLiteConnection(conn.Connection))
            {
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM tblUser WHERE UserID = '" + username + "'", connstring);
                    connstring.Open();

                    AutoCompleteStringCollection Collection = new AutoCompleteStringCollection();
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        Collection.Add(reader.GetString(1));
                    }
                    txtEmail.AutoCompleteCustomSource = Collection;
                    reader.Close();
                }
                catch(Exception)
                {
                    //DO NOTHING
                }
                finally
                {
                    connstring.Close();
                } 
                
            }
        }

        private bool isEmailValid(string email)
        {

            ConnectionString connString = new ConnectionString();
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            SQLiteCommand sqlCmd = new SQLiteCommand ();
            sqlCmd.Connection = sqlConn;
            sqlConn.Open();
            sqlCmd.CommandText = "SELECT * FROM tblUser WHERE [Email] = '" + txtEmail.Text + "' AND [UserID] = '" + txtUserName.Text + "'";
            SQLiteDataReader sqlReader = sqlCmd.ExecuteReader();
            if (sqlReader.HasRows)
            {
                sqlReader.Close();
                sqlConn.Close();
                return true;
            }
            else
            {
                sqlReader.Close();
                sqlConn.Close();
                return false;
            }
        }

        private void EnableControls(bool state)
        {
            txtEmail.Enabled = state;
            txtUserName.Enabled = state;
            btnSend.Enabled = state;
            btnCancel.Enabled = state;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            RemoteCertificateValidationCallback rmtCallback = ServicePointManager.ServerCertificateValidationCallback;

            if (txtUserName.Text == "")
            {
                VMessageBox VMsg = new VMessageBox("Please enter your user name", "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                txtUserName.Select();
                return;
            }


            if (!(txtEmail.Text == ""))
            {
                if (!(isEmailValid(txtEmail.Text)))
                {
                    VMessageBox VMsg = new VMessageBox("The email address you entered does not exist"+Environment.NewLine+"Or no user with the email you entered exists"+Environment.NewLine+"Please check your entries", "Error", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                    txtUserName.Select();
                    return;
                }

                Cursor = Cursors.WaitCursor;

                try
                {
                    EnableControls(false);
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(onValidateCertificate);
                    SmtpClient client = new SmtpClient("smtp.gmail.com");
                    MailAddress from = new MailAddress("systemdys@gmail.com", "Dys " + (char)0xD8 + " System", System.Text.Encoding.UTF8);
                    MailAddress to = new MailAddress(txtEmail.Text);

                    newpassword = GeneratePassword(8);
                    MailMessage message = new MailMessage(from, to);
                    message.Body = "Your new password is "+newpassword;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.Subject = "New Password";
                    message.SubjectEncoding = System.Text.Encoding.UTF8;

                    client.Port = 587;
                    client.Credentials = new System.Net.NetworkCredential("systemdys@gmail.com", Properties.Settings.Default.dyspassword);
                    client.EnableSsl = true;

                    client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallBack);

                    string userState = "Sending State";
                    client.SendAsync(message, userState);
                }
                catch (Exception ex)
                {
                    VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                    Cursor = Cursors.Arrow;
                    EnableControls(true);
                }
                finally
                {
                    ServicePointManager.ServerCertificateValidationCallback = rmtCallback;
                }
            }
        }

        private bool onValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private bool isConnected()
        {
            try
            {
                Ping ping = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions options = new PingOptions();
                PingReply reply = null;
                reply = ping.Send(host, timeout, buffer, options);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception) { return false; };
        }

        private static string GeneratePassword(int Length)
        {
            //TO REVISE
            char[] mychar = new char[200];
            mychar = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?.".ToCharArray();
            byte[] data = new byte[1];
            System.Security.Cryptography.RNGCryptoServiceProvider crypt = new System.Security.Cryptography.RNGCryptoServiceProvider();
            crypt.GetNonZeroBytes(data);
            data = new byte[Length];
            crypt.GetNonZeroBytes(data);
            StringBuilder builder = new StringBuilder(Length);
            foreach (byte b in data)
            {
                builder.Append(mychar[b % (mychar.Length)]);
            }

            return builder.ToString();
        }

        private void SendCompletedCallBack(object sender, AsyncCompletedEventArgs e)
        {
            string token = (string)e.UserState;
            if(e.Error != null)
            {
                Cursor = Cursors.Arrow;
                VMessageBox VMsg = new VMessageBox(token+"."+e.Error.ToString(), "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                EnableControls(true);
            }
            else
            {
                Cursor = Cursors.Arrow;
                VMessageBox VMsg = new VMessageBox("Mail Sent", "Sent", VMessageBox.MessageBoxType.Information);
                VMsg.ShowDialog();
                UpdatePassword();
                this.Close();
                EnableControls(true);
            }
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btnSend_Click(sender, e);
            }
        }

        private void UpdatePassword()
        {
            ConnectionString connString = new ConnectionString();
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            SQLiteCommand sqlCmd = new SQLiteCommand();
            sqlCmd.Connection = sqlConn;
            sqlConn.Open();
            try
            {
                Cursor = Cursors.WaitCursor;
                sqlCmd.CommandText = "UPDATE tblUser SET [Password] = @password WHERE (([UserID] = @id))";
                sqlCmd.Parameters.Add(new SQLiteParameter("@password") { Value = newpassword });
                sqlCmd.Parameters.Add(new SQLiteParameter("@id") { Value = txtUserName.Text.Trim() });
                sqlCmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
            }
            finally
            {
                sqlConn.Close();
                Cursor = Cursors.Arrow;
            }
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
            if (txtUserName.TextLength > 100)
            {
                VMessageBox VMsg = new VMessageBox("Maximum number of characters allowed is 100", "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                txtUserName.Text = "";
                return;
            }
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            if (txtEmail.TextLength > 100)
            {
                VMessageBox VMsg = new VMessageBox("Maximum number of characters allowed is 100", "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                txtEmail.Text = "";
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
