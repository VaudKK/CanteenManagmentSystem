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
    public partial class VMessageBox : Form
    {
        NotificationSoundEngine notificationSound = new NotificationSoundEngine();
        MessageBoxType getMsgType;
        public VMessageBox(string Message,string Title,MessageBoxType MsgType)
        {
            InitializeComponent();
            components = new Container();
            components.Add(notificationSound);
            txtMessage.Text = Message;
            Text = Title;
            getMsgType = MsgType;
        }

        private void VMessageBox_Load(object sender, EventArgs e)
        {
            if (getMsgType == MessageBoxType.Error)
            {
                DisplayImage.Image = Properties.Resources.delete_icon;
                notificationSound.OpenMedia(Application.StartupPath + "/ErrorSound.wav");
            }
            else if(getMsgType == MessageBoxType.Information)
            {
                notificationSound.OpenMedia(Application.StartupPath + "/TaskCompletedSound.wav");
                DisplayImage.Image = Properties.Resources.Info2;
            }
        }

        public enum MessageBoxType
        {
            Error = 0,
            Information = 1
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            notificationSound.Dispose();
            Close();
        }
    }
}
