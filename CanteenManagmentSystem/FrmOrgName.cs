using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CanteenManagmentSystem
{
    public partial class FrmOrgName : Form
    {
        public FrmOrgName()
        {
            InitializeComponent();
        }

        private void FrmOrgName_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            if(Properties.Settings.Default.OrgName != "")
            {
                txtName.Text = Properties.Settings.Default.OrgName;
            }
            txtName.Select();
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            if(!(txtName.Text == ""))
            {
                Properties.Settings.Default.OrgName = txtName.Text;
                Properties.Settings.Default.Save();
                VMessageBox VMsg = new VMessageBox("Succeeded", "Saved", VMessageBox.MessageBoxType.Information);
                VMsg.ShowDialog();
                for(int i = 0; i < Application.OpenForms.Count; i++)
                {
                    if(Application.OpenForms[i].Name == "FrmMainWinodw")
                    {
                        FrmMainWinodw main = (FrmMainWinodw)Application.OpenForms[i];
                        if (main.Text.Contains("Administrator"))
                        {
                            main.Text = Properties.Settings.Default.OrgName + " - Logged In As: " + Properties.Settings.Default.CurrentUser+" - Administrator";
                        }
                        else
                        {
                            main.Text = Properties.Settings.Default.OrgName + " - Logged In As: " + Properties.Settings.Default.CurrentUser;
                        }
                        
                    }

                }
                Close();
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if(txtName.TextLength > 50)
            {
                VMessageBox VMsg = new VMessageBox("Maximum number of characters allowed is 50", "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                txtName.Clear();
                txtName.Select();
            }
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btnSet_Click(sender, e);
            }
        }
    }
}
