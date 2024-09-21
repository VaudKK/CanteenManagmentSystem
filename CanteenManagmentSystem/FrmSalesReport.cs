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
    public partial class FrmSalesReport : Form
    {
        private BindingSource source = new BindingSource();
        private SQLiteDataAdapter adapter;
        public FrmSalesReport()
        {
            InitializeComponent();
        }

        private async void FrmSalesReport_Load(object sender, EventArgs e)
        {
            CreateSalesView();
            CreateSupplyView();
            CreatePettyCashView();
            CreateSalesReportView();
            await GetData("SELECT * FROM SalesReportView");
            this.adapter.Fill(this.DataSet1.SalesReportView);
            this.rptSales.RefreshReport();
        }

        private async Task<object> GetData(string query)
        {
            ConnectionString connString = new ConnectionString();
            SQLiteConnection sqlConn = new SQLiteConnection(connString.Connection);
            adapter = new SQLiteDataAdapter(query, sqlConn);
            SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter);
            DataTable table = new DataTable();
            await Task.Run(() => adapter.Fill(table));
            source.DataSource = table;

            return source;
        }

        private void DropViews()
        {
            ConnectionString connString = new ConnectionString();
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "DROP VIEW SalesView";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sql = "DROP VIEW SupplyView";
            sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sql = "DROP VIEW SalesReportView";
            sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void CreateSupplyView()
        {
            ConnectionString connString = new ConnectionString();
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "CREATE VIEW IF NOT EXISTS SupplyView AS SELECT  STRFTIME('%Y-%m-%d',Delivery)  AS 'SupplyDate', SUM(TotalPrice) AS 'TotalExpenditure' FROM tblSupply" +
                         " GROUP BY STRFTIME('%Y-%m-%d',Delivery) ";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void CreateSalesView()
        {
            ConnectionString connString = new ConnectionString();
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "CREATE VIEW IF NOT EXISTS SalesView AS SELECT  STRFTIME('%Y-%m-%d',TimeOfSale)  AS 'SalesDate', SUM(TotalCost) AS 'TotalSales' FROM tblSales" +
                         " GROUP BY STRFTIME('%Y-%m-%d',TimeOfSale);";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void CreatePettyCashView()
        {
            ConnectionString connString = new ConnectionString();
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "CREATE VIEW IF NOT EXISTS PettyCashView AS SELECT  STRFTIME('%Y-%m-%d',Date)  AS 'Date', SUM(Amount) AS 'TotalPettyCash' FROM tblPetty" +
                         " GROUP BY STRFTIME('%Y-%m-%d',Date);";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void CreateSalesReportView()
        {
            ConnectionString connString = new ConnectionString();
            SQLiteConnection sqliteConn = new SQLiteConnection(connString.Connection);
            sqliteConn.Open();
            string sql = "CREATE VIEW IF NOT EXISTS SalesReportView AS SELECT  SalesView.SalesDate AS 'Date', SalesView.TotalSales,SupplyView.TotalExpenditure,PettyCashView.TotalPettyCash"+
                " FROM SalesView" +
                " LEFT OUTER JOIN SupplyView"+
                " ON  SalesView.SalesDate = SupplyView.SupplyDate"+
                " LEFT OUTER JOIN PettyCashView"+
                " ON  SalesView.SalesDate = PettyCashView.Date";
            SQLiteCommand sqliteCmd = new SQLiteCommand(sql, sqliteConn);
            sqliteCmd.ExecuteNonQuery();
            sqliteConn.Close();
        }

        private void CreateDatabaseFile()
        {
            SQLiteConnection.CreateFile("CanteenDB.sqlite");
        }
    }
}
