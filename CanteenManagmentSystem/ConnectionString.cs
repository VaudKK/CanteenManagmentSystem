using System;
using System.Windows.Forms;

namespace CanteenManagmentSystem
{
    public class ConnectionString
    {
        public string Connection = "DataSource ="+ Application.StartupPath + "\\CanteenDB.sqlite;Version=3";
        //Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
    }
}
