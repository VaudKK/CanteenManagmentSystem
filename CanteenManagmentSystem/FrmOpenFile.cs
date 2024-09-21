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
    public partial class FrmOpenFile : Form
    {
       
        public FrmOpenFile()
        {
            InitializeComponent();
        }

        private void FrmOpenFile_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            LoadFiles();
        }

        private void LoadFiles()
        {
            foreach(string file in System.IO.Directory.EnumerateFiles(System.IO.Path.GetTempPath() + "/SES", "*.xlsx"))
            {
                ListViewItem item = new ListViewItem(System.IO.Path.GetFileName(file));
                item.SubItems.Add(System.IO.Path.GetFullPath(file));
                listView1.Items.Add(item);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int count = listView1.Items.Count;
            string path = "";
            if(count > 0)
            {
                path = listView1.SelectedItems[0].SubItems[1].Text;
            }

            for(int i = 0; i < Application.OpenForms.Count; i++)
            {
                if(Application.OpenForms[i].Name == "FrmExpectation")
                {
                    FrmExpectation expt = (FrmExpectation)Application.OpenForms[i];
                    expt.FileLocation = path;
                    expt.ImportToDGV();
                }
            }

            this.Dispose();
        }
    }
}
