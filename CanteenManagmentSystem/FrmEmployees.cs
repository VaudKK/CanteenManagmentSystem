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
    public partial class FrmEmployees : Form
    {
        ConnectionString connString = new ConnectionString();
        public string OriginalID;
        public FrmEmployees()
        {
            InitializeComponent();
        }
        private void DropTable()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "DROP TABLE Employees";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void CreateTable()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "CREATE TABLE IF NOT EXISTS tblEmployees(EmployeeID INTEGER PRIMARY KEY ASC NOT NULL,EmployeeName TEXT NOT NULL,IDNO FLOAT NOT NULL," +
                          "Gender VARCHAR(10) NOT NULL,DateOfBirth TEXT NULL, Address TEXT,Tel FLOAT NOT NULL,Email TEXT NOT NULL,EmploymentDate TEXT NOT NULL,TerminationDate TEXT NULL,Status VARCHAR(20) DEFAULT('Active'))";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (isEmpty())
            {
                VMessageBox VMsg = new VMessageBox("The following fields must be filled: EmployeeID,EmployeeName,IDNO,Gender,TelephoneNo and Email", "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                return;
            }

            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            if (btnSave.Text == "Save")
            {
                try
                {
                    sqlConn.Open();
                    string InsertCommand = "INSERT INTO tblEmployees(EmployeeID,EmployeeName,[IDNO],Gender,DateOfBirth,Address,Tel,Email,EmploymentDate) VALUES(@id,@name,@idno,@gender,@dob,@addr,@tel,@mail,@doe)";
                    SQLiteCommand sqlCmd = new SQLiteCommand(InsertCommand, sqlConn);
                    sqlCmd.Parameters.Add(new SQLiteParameter("@id"){Value = Convert.ToInt32(txtID.Text.Trim())});
                    sqlCmd.Parameters.Add(new SQLiteParameter("@name") {Value = txtName.Text});
                    sqlCmd.Parameters.Add(new SQLiteParameter("@idno") { Value = Convert.ToDouble(txtidno.Text.Trim())});
                    sqlCmd.Parameters.Add(new SQLiteParameter("@gender") { Value = cboGender.Text });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@dob") { Value = DOB.Value.ToString("yyyy-MM-dd HH:mm:ss") });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@addr") { Value = txtAddr.Text });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@tel") { Value = Convert.ToDouble(txtTel.Text.Trim())});
                    sqlCmd.Parameters.Add(new SQLiteParameter("@mail") { Value = txtEmail.Text });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@doe") { Value = DOE.Value.ToString("yyyy-MM-dd HH:mm:ss") });
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    LogsFunction log = new LogsFunction();
                    log.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), "Saved new employee.Employee ID:" + txtID.Text);
                    VMessageBox VMsg = new VMessageBox("Details Saved", "Saved", VMessageBox.MessageBoxType.Information);
                    VMsg.ShowDialog();
                    ClearTextBox();
                    cboGender.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                }
                finally
                {
                    sqlConn.Close();
                    txtID.Select();
                }
            }else if(btnSave.Text == "Update")
            {
                try
                {
                    sqlConn.Open();
                    string UpdateCommand = "UPDATE tblEmployees SET EmployeeID = @id ,EmployeeName = @name, IDNO = @idno, Gender = @gender,DateOfBirth = @dob,Address = @addr,Tel = @tel ,Email = @mail,EmploymentDate = @doe WHERE EmployeeID = @original";
                    SQLiteCommand sqlCmd = new SQLiteCommand(UpdateCommand, sqlConn);
                    sqlCmd.Parameters.Add(new SQLiteParameter("@id") { Value = Convert.ToInt32(txtID.Text.Trim()) });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@name") { Value = txtName.Text });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@idno") { Value = Convert.ToDouble(txtidno.Text.Trim()) });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@gender") { Value = Convert.ToChar(cboGender.Text) });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@dob") { Value = DOB.Value.ToString("yyyy-MM-dd HH:mm:ss") });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@addr") { Value = txtAddr.Text });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@tel") { Value = Convert.ToDouble(txtTel.Text.Trim()) });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@mail") { Value = txtEmail.Text });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@doe") { Value = DOE.Value.ToString("yyyy-MM-dd HH:mm:ss") });
                    sqlCmd.Parameters.Add(new SQLiteParameter("@original") { Value = Convert.ToInt32(OriginalID)});
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    LogsFunction log = new LogsFunction();
                    log.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), "Updated employee.Employee ID:" + txtID.Text);
                    VMessageBox VMsg = new VMessageBox("Details Update", "Updated", VMessageBox.MessageBoxType.Information);
                    VMsg.ShowDialog();
                    ClearTextBox();
                    cboGender.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                }
                finally
                {
                    sqlConn.Close();
                    btnSave.Text = "Save";
                    txtID.Select();
                }
            }
        }

        private Boolean isEmpty()
        {
            if (txtID.Text == "" || txtName.Text == "" || txtidno.Text == "" || txtTel.Text == "" || cboGender.SelectedIndex == 0)
                return true;
            return false;
        }

        private void setDefaultValues()
        {
            cboGender.SelectedIndex = 0;
        }

        private void FrmEmployees_Load(object sender, EventArgs e)
        {
            setDefaultValues();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            txtID.Select();
            CreateTable();
        }

        private void ClearTextBox()
        {
            foreach (Control c in this.Controls)
            {
                if (c is TextBox)
                {
                    c.Text = "";
                }
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            FrmViewEmployees view = new FrmViewEmployees();
            btnSave.Text = "Save";
            ClearTextBox();
            view.ShowDialog();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            FrmViewEmployees view = new FrmViewEmployees();
            view.Mode = "Edit";
            view.ShowDialog();
        }

        private void TextBox_KeyPress(object sender,KeyPressEventArgs e)
        {
            if(!(char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar))){
                e.Handled = true;
            }
        }

        private void TextBox_TextChanged(object sender,EventArgs e)
        {
            var textBox = (TextBox)sender;
            if(textBox.TextLength > 7)
            {
                VMessageBox VMsg = new VMessageBox("Maximum number of characters allowed is 7", "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                textBox.Clear();
                textBox.Select();
            }
        }

        private void IDValidation_TextChanged(object sender,EventArgs e)
        {
            var textbox = (TextBox)sender;
            if(textbox.TextLength > 8)
            {
                VMessageBox VMsg = new VMessageBox("Maximum number of characters allowed is 8", "Out of Bounds", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                textbox.Clear();
                textbox.Select();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            btnSave.Text = "Save";
            txtID.Select();
        }

        private void txtTel_TextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.TextLength > 10)
            {
                VMessageBox VMsg = new VMessageBox("Maximum number of characters allowed is 10", "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                textBox.Clear();
                textBox.Select();
            }
        }
    }
}
