using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Windows.Forms;

namespace CanteenManagmentSystem
{
    public class LogsFunction
    {
        ConnectionString connString = new ConnectionString();
        SQLiteConnection  Connection;
        SQLiteDataAdapter InsertAdapter;

        public void Logs(string UserID,string time,string operation)
        {
            CreateTable();
            Connection = new SQLiteConnection(connString.Connection);
            try
            {
                InsertAdapter = new SQLiteDataAdapter("SELECT * FROM tblLogs", Connection);
                InsertAdapter.InsertCommand = new SQLiteCommand();
                InsertAdapter.InsertCommand.Connection = Connection;
                Connection.Open();
                InsertAdapter.InsertCommand.CommandText = "INSERT INTO tblLogs([User],[Time],[Operation]) VALUES(@user,@time,@ops)";
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@user") { Value = UserID });
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@time") { Value = time });
                InsertAdapter.InsertCommand.Parameters.Add(new SQLiteParameter("@ops") { Value = operation });
                InsertAdapter.InsertCommand.ExecuteNonQuery();
                Connection.Close();
            }
           catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                Connection.Close();
            }
        }

        private void DropTable()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "DROP TABLE tblLogs";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void CreateTable()
        {
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "CREATE TABLE IF NOT EXISTS tblLogs(LogNo INTEGER PRIMARY KEY ASC AUTOINCREMENT,User VARCHAR(150) NOT NULL,"+
                "Time TEXT NOT NULL,Operation TEXT NOT NULL)";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }
    }
}
