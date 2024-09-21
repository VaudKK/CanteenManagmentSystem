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
    public partial class FrmViewPetty : Form
    {
        private SQLiteDataAdapter adapter = new SQLiteDataAdapter();
        private BindingSource source = new BindingSource();
        public FrmViewPetty()
        {
            InitializeComponent();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                ConnectionString connString = new ConnectionString();
                string selectedDate = dateTimePicker1.Value.ToString("yyyy-MM-dd ");
                string CommandText = "SELECT * FROM tblPetty WHERE STRFTIME('%Y-%m-%d',Date) = STRFTIME('%Y-%m-%d','" + selectedDate + "')";
                SQLiteDataAdapter da = new SQLiteDataAdapter(CommandText, connString.Connection);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception v)
            {
                VMessageBox VMsg = new VMessageBox(v.Message, "Error", VMessageBox.MessageBoxType.Error);
                VMsg.ShowDialog();
            }
        }

        private async Task<object> GetData(string commmand)
        {
            ConnectionString connString = new ConnectionString();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(commmand, connString.Connection);
            SQLiteCommandBuilder commandbuilder = new SQLiteCommandBuilder(this.adapter);
            DataTable table = new DataTable();
            await Task.Run(() => adapter.Fill(table));
            source.DataSource = table;

            return source;
        }

        private async void FrmViewPetty_Load(object sender, EventArgs e)
        {
            await GetData("SELECT * FROM tblPetty");
            dataGridView1.DataSource = source;
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                DataGridViewColumn column = dataGridView1.Columns[i];
                column.Width = 220;
            }
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;
            dataGridView1.ReadOnly = true;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void BtnRemoveFilter_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = source;
        }
    }
}
