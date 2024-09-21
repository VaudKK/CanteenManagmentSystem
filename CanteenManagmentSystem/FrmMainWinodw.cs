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
    public partial class FrmMainWinodw : Form
    {
        ConnectionString connString = new ConnectionString();
        public FrmMainWinodw()
        {
            InitializeComponent();
        }

        private void FrmMainWinodw_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }
        private void BtnMakeASale_Click(object sender, EventArgs e)
        {
            selectionPanel.Visible = true;
            Button button = (Button)sender;
            selectionPanel.Height = button.Height;
            selectionPanel.Top = button.Top;
            FrmMakeASale MakeAsSale = new FrmMakeASale();
            MakeAsSale.ShowDialog();
        }

        private void BtnPurchase_Click(object sender, EventArgs e)
        {
            selectionPanel.Visible = true;
            Button button = (Button)sender;
            selectionPanel.Height = button.Height;
            selectionPanel.Top = button.Top;
            FrmSupply Stock = new FrmSupply();
            Stock.ShowDialog();
        }

        private void dishesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void foodCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void foodsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmProduct product = new FrmProduct();
            product.ShowDialog();
        }

        private void FrmMainWinodw_FormClosing(object sender, FormClosingEventArgs e)
        {
            DateTime notificationTime = DateTime.Parse("19:00:00");
            DateTime now =DateTime.Parse(DateTime.Now.ToString("HH:mm:ss"));
            if(now > notificationTime)
            {
                MessageBox.Show("Do you want to back up your data?", "BackUp", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(DialogResult == DialogResult.Yes)
                {
                    backUpDatabaseToolStripMenuItem_Click(sender, e);
                }
            }

            LogsFunction logs = new LogsFunction();
            logs.Logs(Properties.Settings.Default.CurrentUser.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Properties.Settings.Default.CurrentUser.ToString() + " logged out");
            Properties.Settings.Default.CurrentUser = "";
            for (int i = 0; i < Application.OpenForms.Count; i++)
            {
                if (Application.OpenForms[i].Name == "FrmLogIn")
                {
                    FrmLogIn form = (FrmLogIn)Application.OpenForms[i];
                    form.Show();
                }
            }
        }

        private void BtnViewAllSales_Click(object sender, EventArgs e)
        {
            selectionPanel.Visible = true;
            Button button = (Button)sender;
            selectionPanel.Height = button.Height;
            selectionPanel.Top = button.Top;
            FrmViewSales Sales = new FrmViewSales();
            Sales.WindowState = FormWindowState.Maximized;
            Sales.ShowDialog();
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Are you sure you want to LogOut", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (!(result == DialogResult.No))
            {
                Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            selectionPanel.Visible = true;
            Button button = (Button)sender;
            selectionPanel.Height = button.Height;
            selectionPanel.Top = button.Top;
            FrmEndOfDay EOD = new FrmEndOfDay();
            EOD.WindowState = FormWindowState.Maximized;
            EOD.ShowDialog();
        }

        private void BtnUsers_Click(object sender, EventArgs e)
        {
            selectionPanel.Visible = true;
            Button button = (Button)sender;
            selectionPanel.Height = button.Height;
            selectionPanel.Top = button.Top;
            FrmUsers Users = new FrmUsers();
            Users.ShowDialog();
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmChangePassword ChangePassword = new FrmChangePassword();
            ChangePassword.ShowDialog();
        }

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            selectionPanel.Visible = true;
            Button button = (Button)sender;
            selectionPanel.Height = button.Height;
            selectionPanel.Top = button.Top;
            FrmAddUser AddUser = new FrmAddUser();
            AddUser.ShowDialog();
        }

        private void dishesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FrmViewProducts Dishes = new FrmViewProducts();
            Dishes.ShowDialog();
        }

        private void stockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmViewStock ViewStock = new FrmViewStock();
            ViewStock.ShowDialog();
        }

        private void todaysUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDailyUse StockItems = new FrmDailyUse();
            StockItems.ShowDialog();
        }

        private void viewAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAllUses AllUses = new FrmAllUses();
            AllUses.ShowDialog();
        }

        private void BtnSalesReport_Click(object sender, EventArgs e)
        {
            selectionPanel.Visible = true;
            Button button = (Button)sender;
            selectionPanel.Height = button.Height;
            selectionPanel.Top = button.Top;
            FrmSalesReport rpt = new FrmSalesReport();
            rpt.ShowDialog();
        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmLogs Log = new FrmLogs();
            Log.ShowDialog();
            Log.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private bool isPreviousVersionAvailable()
        {
            return System.IO.File.Exists(System.IO.Path.GetTempPath() + "/DysBackUp/CanteenDB.bak");
        }

        private void ReleaseDatabase()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            sqliteConn.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public async Task<int> BackUpDatabase(string srcFilePath,string destFilePath,string srcFileName,string DestFileName)
        {
            ReleaseDatabase();
            var srcfile = System.IO.Path.Combine(srcFilePath, srcFileName);
            var destfile = System.IO.Path.Combine(destFilePath, DestFileName);

            await Task.Run(() => System.IO.File.Copy(srcfile, destfile));

            return 0;
        }

        public void  RestoreDatabase(string srcFilePath,string destFilePath,string srcFileName,string DestFileName)
        {
            ReleaseDatabase();
            try
            {
                var srcfile = System.IO.Path.Combine(srcFilePath, srcFileName);
                var destfile = System.IO.Path.Combine(destFilePath, DestFileName);

                System.IO.File.Copy(srcfile, destfile, true);

                Cursor = Cursors.Arrow;
                VMessageBox VMsg = new VMessageBox("Database Restored", "Database Restore", VMessageBox.MessageBoxType.Information);
                VMsg.ShowDialog();
            }
            catch (System.IO.FileNotFoundException ex)
            {
                VMessageBox VMsg = new VMessageBox("Back up file not found" + " HRESULT: " + ex.HResult, "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                Cursor = Cursors.Arrow;
            }
            catch(Exception ex)
            {
                VMessageBox VMsg = new VMessageBox(ex.Message + " HRESULT: " + ex.HResult, "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                Cursor = Cursors.Arrow;
            }
           
        }

        private async void backUpDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
            LogsFunction log = new LogsFunction();
            log.Logs(Properties.Settings.Default.CurrentUser, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Database backed up");

            Cursor = Cursors.WaitCursor;

                if (!System.IO.Directory.Exists(System.IO.Path.GetTempPath() + "/DysBackUp"))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetTempPath() + "/DysBackUp");
                }

                if (isPreviousVersionAvailable())
                {
                    System.IO.File.Delete(System.IO.Path.GetTempPath() + "/DysBackUp/CanteenDB.bak");
                }

                //await BackUpDatabase(Application.StartupPath, System.IO.Path.GetTempPath() + "/DysBackUp/", "CanteenDB.sqlite", "CanteenDB.bak");
                await BackUpDatabase(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.IO.Path.GetTempPath() + "/DysBackUp/", "CanteenDB.sqlite", "CanteenDB.bak");

                Cursor = Cursors.Arrow;
            VMessageBox VMsg = new VMessageBox("BackUp succeeded", "Database BackUp", VMessageBox.MessageBoxType.Information);
            VMsg.ShowDialog();
            }
            catch (Exception)
            {
                VMessageBox VMsg = new VMessageBox("Sorry an error occurred.Backup failed", "Database BackUp", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                Cursor = Cursors.Arrow;
            }
        }

        private void addEmployeeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmEmployees employee = new FrmEmployees();
            employee.ShowDialog();
        }

        private void employeesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmViewEmployees view = new FrmViewEmployees();
            view.ShowDialog();
        }

        private void itemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmViewProducts Dishes = new FrmViewProducts();
            Dishes.ShowDialog();
        }

        private void newSupplyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSupply supply = new FrmSupply();
            supply.ShowDialog();
        }

        private void viewSuppliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSupply supply = new FrmSupply();
            supply.ShowDialog();
        }

        private void restoreDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("You are about to restore database records that may not\nbe up to date with the current records you have.\nThis includes former user details such as usernames,emails and passwords\nDo you wish to continue?", "Restore Database", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;

                    if (!System.IO.Directory.Exists(System.IO.Path.GetTempPath() + "/DysRestored"))
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetTempPath() + "/DysRestored");
                    }

                    LogsFunction log = new LogsFunction();
                    log.Logs(Properties.Settings.Default.CurrentUser, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Database Restored");
                    RestoreDatabase(System.IO.Path.GetTempPath() + "/DysBackUp/", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CanteenDB.bak", "CanteenDB.sqlite");

                }
                catch (Exception ex)
                {
                    VMessageBox VMsg = new VMessageBox("Sorry an error occurred.Backup failed. "+ ex.HResult, "Database BackUp", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                    Cursor = Cursors.Arrow;
                }
            }
        }

        private void organizationNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmOrgName orgname = new FrmOrgName();
            orgname.ShowDialog();
        }

        private void viewExportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = "";
            OpenFileDialog open = new OpenFileDialog
            {
                Title = "Open Exports",
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Excel Files(*.xlsx)|*.xlsx"
            };

            if(open.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    filename = open.FileName;
                    System.Diagnostics.Process.Start(open.FileName);
                    LogsFunction log = new LogsFunction();
                    log.Logs(Properties.Settings.Default.CurrentUser, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Export opened. FileName: "+System.IO.Path.GetFileName(filename));
                }
                catch(Exception ex)
                {
                    VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                    VMsg.ShowDialog();
                }
            }
        }

        private void editUserInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmUserInfo info = new FrmUserInfo();
            info.ShowDialog();
        }

        private void printerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPrinter printer = new FrmPrinter();
            printer.ShowDialog();
        }

        private void pettyCashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPettyCash petty = new frmPettyCash();
            petty.ShowDialog();
        }

        private void viewAllPettyCashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmViewPetty view = new FrmViewPetty();
            view.ShowDialog();
        }

        private void viewTodaysUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAllUses daily = new FrmAllUses();
            daily.ShowDialog();
        }
    }
}
