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
    public partial class FrmPrinter : Form
    {
        public FrmPrinter()
        {
            InitializeComponent();
        }

        private void FrmPrinter_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            lblComputerName.Text = "Computer Name: " + Environment.MachineName;
        }

        private void btnGetPrinter_Click(object sender, EventArgs e)
        {
            try {
                string PrinterSettingsPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.System), "control.exe");
                System.Diagnostics.Process.Start(PrinterSettingsPath, "/name Microsoft.DevicesAndPrinters");
            }catch(Exception ex)
            {
                VMessageBox VMsg = new VMessageBox(ex.Message, "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!(txtPrinterName.Text == ""))
            {
                string printpath = "\\\\" + Environment.MachineName + "\\" + txtPrinterName.Text.Trim();
                Properties.Settings.Default.PrinterPath = printpath;
                Properties.Settings.Default.Save();
                VMessageBox VMsg = new VMessageBox("Printer Details Saved", txtPrinterName.Text, VMessageBox.MessageBoxType.Information);
                VMsg.ShowDialog();
                this.Close();
            }
            else
            {
                VMessageBox VMsg = new VMessageBox("Please enter printer name","Printer Name", VMessageBox.MessageBoxType.Information);
                VMsg.ShowDialog();
            }            
        }

        private void txtPrinterName_TextChanged(object sender, EventArgs e)
        {
            if(txtPrinterName.TextLength > 250)
            {
                VMessageBox VMsg = new VMessageBox("Maximum number of characters allowed is 250", "Out of Bounds", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
                txtPrinterName.Text = "";
            }
        }
    }
}
