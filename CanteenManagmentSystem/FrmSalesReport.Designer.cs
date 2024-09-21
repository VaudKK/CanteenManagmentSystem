namespace CanteenManagmentSystem
{
    partial class FrmSalesReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSalesReport));
            this.SalesReportViewBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.DataSet1 = new CanteenManagmentSystem.DataSet1();
            this.rptSales = new Microsoft.Reporting.WinForms.ReportViewer();
            this.SalesReportViewTableAdapter = new CanteenManagmentSystem.DataSet1TableAdapters.SalesReportViewTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.SalesReportViewBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataSet1)).BeginInit();
            this.SuspendLayout();
            // 
            // SalesReportViewBindingSource
            // 
            this.SalesReportViewBindingSource.DataMember = "SalesReportView";
            this.SalesReportViewBindingSource.DataSource = this.DataSet1;
            // 
            // DataSet1
            // 
            this.DataSet1.DataSetName = "DataSet1";
            this.DataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // rptSales
            // 
            this.rptSales.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "DataSet1";
            reportDataSource1.Value = this.SalesReportViewBindingSource;
            this.rptSales.LocalReport.DataSources.Add(reportDataSource1);
            this.rptSales.LocalReport.ReportEmbeddedResource = "CanteenManagmentSystem.Report1.rdlc";
            this.rptSales.Location = new System.Drawing.Point(0, 0);
            this.rptSales.Name = "rptSales";
            this.rptSales.Size = new System.Drawing.Size(787, 417);
            this.rptSales.TabIndex = 0;
            // 
            // SalesReportViewTableAdapter
            // 
            this.SalesReportViewTableAdapter.ClearBeforeFill = true;
            // 
            // FrmSalesReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 417);
            this.Controls.Add(this.rptSales);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmSalesReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SalesReport";
            this.Load += new System.EventHandler(this.FrmSalesReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SalesReportViewBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataSet1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer rptSales;
        private System.Windows.Forms.BindingSource SalesReportViewBindingSource;
        private DataSet1 DataSet1;
        private DataSet1TableAdapters.SalesReportViewTableAdapter SalesReportViewTableAdapter;
    }
}