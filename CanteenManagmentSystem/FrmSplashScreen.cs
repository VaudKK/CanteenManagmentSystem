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
    public partial class FrmSplashScreen : Form
    {
        private Timer time = new Timer();

        public FrmSplashScreen()
        {
            InitializeComponent();
            time.Interval = 4000;
            time.Enabled = true;
            time.Tick += Time_Tick;
        }

        private void Time_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmSplashScreen_Load(object sender, EventArgs e)
        {
            time.Start();
        }
    }
}
