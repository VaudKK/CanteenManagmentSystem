using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;

namespace CanteenManagmentSystem
{
    public partial class FrmUsers : Form
    {
        ConnectionString connString = new ConnectionString();
        public FrmUsers()
        {
            InitializeComponent();
        }
        private void FrmUsers_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            listView1.Columns.Add("UserID");
            listView1.Columns.Add("AccessLevel");
            listView1.Columns.Add("Status");
            listView1.Font = new Font("Times New Roman",13);
            listView1.FullRowSelect = true;
            ConnectionString connString = new ConnectionString();
            SQLiteConnection connection = new SQLiteConnection(connString.Connection);
            SQLiteDataAdapter Adapter = new SQLiteDataAdapter("SELECT UserID,AccessLevel,Status FROM tblUser",connection);
            DataTable Table = new DataTable();
            Adapter.Fill(Table);
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow Row = Table.Rows[i];
                ListViewItem ListItem = new ListViewItem(Row["UserID"].ToString());
                ListItem.SubItems.Add(Row["AccessLevel"].ToString());
                ListItem.SubItems.Add(Row["Status"].ToString());
                listView1.Items.Add(ListItem);
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
    }
}
