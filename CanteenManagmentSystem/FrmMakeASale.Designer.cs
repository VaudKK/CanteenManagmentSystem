namespace CanteenManagmentSystem
{
    partial class FrmMakeASale
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMakeASale));
            this.panel2 = new System.Windows.Forms.Panel();
            this.BtnSaveAndPrint = new System.Windows.Forms.Button();
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnSave = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnGetItem = new System.Windows.Forms.Button();
            this.BtnAdd = new System.Windows.Forms.Button();
            this.QuatityTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.PriceTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ItemTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ItemsList = new System.Windows.Forms.ListView();
            this.ItemColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PriceColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.QuantityColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label4 = new System.Windows.Forms.Label();
            this.OrderTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TotalCostLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.AmountPaidTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.BalanceLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.MODComboBox = new System.Windows.Forms.ComboBox();
            this.BillingPanel = new System.Windows.Forms.Panel();
            this.txtTransactionNo = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.BillingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.BtnSaveAndPrint);
            this.panel2.Controls.Add(this.BtnClose);
            this.panel2.Controls.Add(this.BtnSave);
            this.panel2.Location = new System.Drawing.Point(602, 421);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(332, 57);
            this.panel2.TabIndex = 9;
            // 
            // BtnSaveAndPrint
            // 
            this.BtnSaveAndPrint.Image = ((System.Drawing.Image)(resources.GetObject("BtnSaveAndPrint.Image")));
            this.BtnSaveAndPrint.Location = new System.Drawing.Point(111, 13);
            this.BtnSaveAndPrint.Name = "BtnSaveAndPrint";
            this.BtnSaveAndPrint.Size = new System.Drawing.Size(109, 29);
            this.BtnSaveAndPrint.TabIndex = 5;
            this.BtnSaveAndPrint.Text = "Sell And Print";
            this.BtnSaveAndPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnSaveAndPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnSaveAndPrint.UseVisualStyleBackColor = true;
            this.BtnSaveAndPrint.Visible = false;
            this.BtnSaveAndPrint.Click += new System.EventHandler(this.BtnSaveAndPrint_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.Image = ((System.Drawing.Image)(resources.GetObject("BtnClose.Image")));
            this.BtnClose.Location = new System.Drawing.Point(226, 13);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(93, 29);
            this.BtnClose.TabIndex = 4;
            this.BtnClose.Text = "Close";
            this.BtnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnSave
            // 
            this.BtnSave.Image = ((System.Drawing.Image)(resources.GetObject("BtnSave.Image")));
            this.BtnSave.Location = new System.Drawing.Point(12, 13);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(208, 29);
            this.BtnSave.TabIndex = 2;
            this.BtnSave.Text = "Sell";
            this.BtnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnGetItem);
            this.panel1.Controls.Add(this.BtnAdd);
            this.panel1.Controls.Add(this.QuatityTextBox);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.PriceTextBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.ItemTextBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(94, 112);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(432, 147);
            this.panel1.TabIndex = 8;
            // 
            // btnGetItem
            // 
            this.btnGetItem.Location = new System.Drawing.Point(340, 15);
            this.btnGetItem.Name = "btnGetItem";
            this.btnGetItem.Size = new System.Drawing.Size(34, 24);
            this.btnGetItem.TabIndex = 17;
            this.btnGetItem.Text = "...";
            this.btnGetItem.UseVisualStyleBackColor = true;
            this.btnGetItem.Click += new System.EventHandler(this.btnGetItem_Click);
            // 
            // BtnAdd
            // 
            this.BtnAdd.Image = ((System.Drawing.Image)(resources.GetObject("BtnAdd.Image")));
            this.BtnAdd.Location = new System.Drawing.Point(349, 103);
            this.BtnAdd.Name = "BtnAdd";
            this.BtnAdd.Size = new System.Drawing.Size(68, 29);
            this.BtnAdd.TabIndex = 6;
            this.BtnAdd.Text = "Add";
            this.BtnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnAdd.UseVisualStyleBackColor = true;
            this.BtnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // QuatityTextBox
            // 
            this.QuatityTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QuatityTextBox.Location = new System.Drawing.Point(156, 91);
            this.QuatityTextBox.Name = "QuatityTextBox";
            this.QuatityTextBox.Size = new System.Drawing.Size(82, 24);
            this.QuatityTextBox.TabIndex = 16;
            this.QuatityTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.QuatityTextBox_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(21, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 18);
            this.label3.TabIndex = 15;
            this.label3.Text = "Quantity:";
            // 
            // PriceTextBox
            // 
            this.PriceTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PriceTextBox.Location = new System.Drawing.Point(156, 56);
            this.PriceTextBox.Name = "PriceTextBox";
            this.PriceTextBox.ReadOnly = true;
            this.PriceTextBox.Size = new System.Drawing.Size(218, 24);
            this.PriceTextBox.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 18);
            this.label2.TabIndex = 13;
            this.label2.Text = "Price:";
            // 
            // ItemTextBox
            // 
            this.ItemTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ItemTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.ItemTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemTextBox.Location = new System.Drawing.Point(156, 15);
            this.ItemTextBox.Name = "ItemTextBox";
            this.ItemTextBox.Size = new System.Drawing.Size(178, 24);
            this.ItemTextBox.TabIndex = 12;
            this.ItemTextBox.TextChanged += new System.EventHandler(this.ItemTextBox_TextChanged);
            this.ItemTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DishTextBox_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(21, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 18);
            this.label1.TabIndex = 11;
            this.label1.Text = "ItemID:";
            // 
            // ItemsList
            // 
            this.ItemsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ItemColumn,
            this.PriceColumn,
            this.QuantityColumn});
            this.ItemsList.FullRowSelect = true;
            this.ItemsList.HideSelection = false;
            this.ItemsList.Location = new System.Drawing.Point(564, 112);
            this.ItemsList.Name = "ItemsList";
            this.ItemsList.Size = new System.Drawing.Size(402, 285);
            this.ItemsList.TabIndex = 11;
            this.ItemsList.UseCompatibleStateImageBehavior = false;
            this.ItemsList.View = System.Windows.Forms.View.Details;
            this.ItemsList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ItemsList_KeyDown);
            // 
            // ItemColumn
            // 
            this.ItemColumn.Text = "Item";
            this.ItemColumn.Width = 150;
            // 
            // PriceColumn
            // 
            this.PriceColumn.Text = "Price";
            this.PriceColumn.Width = 100;
            // 
            // QuantityColumn
            // 
            this.QuantityColumn.Text = "Quantity";
            this.QuantityColumn.Width = 100;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(34, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 18);
            this.label4.TabIndex = 29;
            this.label4.Text = "OrderType:";
            // 
            // OrderTypeComboBox
            // 
            this.OrderTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OrderTypeComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrderTypeComboBox.FormattingEnabled = true;
            this.OrderTypeComboBox.Items.AddRange(new object[] {
            "",
            "Sitting",
            "Take Away"});
            this.OrderTypeComboBox.Location = new System.Drawing.Point(169, 10);
            this.OrderTypeComboBox.Name = "OrderTypeComboBox";
            this.OrderTypeComboBox.Size = new System.Drawing.Size(218, 26);
            this.OrderTypeComboBox.TabIndex = 30;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(35, 234);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 18);
            this.label5.TabIndex = 31;
            this.label5.Text = "Total:";
            // 
            // TotalCostLabel
            // 
            this.TotalCostLabel.AutoSize = true;
            this.TotalCostLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalCostLabel.Location = new System.Drawing.Point(136, 234);
            this.TotalCostLabel.Name = "TotalCostLabel";
            this.TotalCostLabel.Size = new System.Drawing.Size(36, 18);
            this.TotalCostLabel.TabIndex = 32;
            this.TotalCostLabel.Text = "0.00";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(34, 140);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 18);
            this.label6.TabIndex = 33;
            this.label6.Text = "Amount Paid:";
            // 
            // AmountPaidTextBox
            // 
            this.AmountPaidTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AmountPaidTextBox.Location = new System.Drawing.Point(169, 136);
            this.AmountPaidTextBox.Name = "AmountPaidTextBox";
            this.AmountPaidTextBox.Size = new System.Drawing.Size(107, 24);
            this.AmountPaidTextBox.TabIndex = 34;
            this.AmountPaidTextBox.TextChanged += new System.EventHandler(this.AmountPaidTextBox_TextChanged);
            this.AmountPaidTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AmountPaidTextBox_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(291, 234);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 18);
            this.label7.TabIndex = 35;
            this.label7.Text = "Balance:";
            // 
            // BalanceLabel
            // 
            this.BalanceLabel.AutoSize = true;
            this.BalanceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BalanceLabel.Location = new System.Drawing.Point(362, 234);
            this.BalanceLabel.Name = "BalanceLabel";
            this.BalanceLabel.Size = new System.Drawing.Size(36, 18);
            this.BalanceLabel.TabIndex = 36;
            this.BalanceLabel.Text = "0.00";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(33, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(124, 18);
            this.label8.TabIndex = 37;
            this.label8.Text = "ModeOfPayment:";
            // 
            // MODComboBox
            // 
            this.MODComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MODComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MODComboBox.FormattingEnabled = true;
            this.MODComboBox.Items.AddRange(new object[] {
            "",
            "Cash",
            "Mpesa"});
            this.MODComboBox.Location = new System.Drawing.Point(168, 52);
            this.MODComboBox.Name = "MODComboBox";
            this.MODComboBox.Size = new System.Drawing.Size(218, 26);
            this.MODComboBox.TabIndex = 38;
            this.MODComboBox.SelectedIndexChanged += new System.EventHandler(this.MODComboBox_SelectedIndexChanged);
            // 
            // BillingPanel
            // 
            this.BillingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BillingPanel.Controls.Add(this.txtTransactionNo);
            this.BillingPanel.Controls.Add(this.label9);
            this.BillingPanel.Controls.Add(this.MODComboBox);
            this.BillingPanel.Controls.Add(this.label8);
            this.BillingPanel.Controls.Add(this.BalanceLabel);
            this.BillingPanel.Controls.Add(this.label7);
            this.BillingPanel.Controls.Add(this.AmountPaidTextBox);
            this.BillingPanel.Controls.Add(this.label6);
            this.BillingPanel.Controls.Add(this.TotalCostLabel);
            this.BillingPanel.Controls.Add(this.label5);
            this.BillingPanel.Controls.Add(this.OrderTypeComboBox);
            this.BillingPanel.Controls.Add(this.label4);
            this.BillingPanel.Location = new System.Drawing.Point(94, 284);
            this.BillingPanel.Name = "BillingPanel";
            this.BillingPanel.Size = new System.Drawing.Size(432, 291);
            this.BillingPanel.TabIndex = 10;
            // 
            // txtTransactionNo
            // 
            this.txtTransactionNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTransactionNo.Location = new System.Drawing.Point(168, 97);
            this.txtTransactionNo.Name = "txtTransactionNo";
            this.txtTransactionNo.Size = new System.Drawing.Size(219, 24);
            this.txtTransactionNo.TabIndex = 40;
            this.txtTransactionNo.TextChanged += new System.EventHandler(this.txtTransactionNo_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(33, 97);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(110, 18);
            this.label9.TabIndex = 39;
            this.label9.Text = "TransactionNo:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(92, 89);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Georgia", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Blue;
            this.label10.Location = new System.Drawing.Point(110, 32);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(144, 29);
            this.label10.TabIndex = 30;
            this.label10.Text = "Make A Sale";
            // 
            // FrmMakeASale
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1007, 611);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ItemsList);
            this.Controls.Add(this.BillingPanel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMakeASale";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Make A Sale";
            this.Load += new System.EventHandler(this.FrmMakeASale_Load);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.BillingPanel.ResumeLayout(false);
            this.BillingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button BtnSaveAndPrint;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox QuatityTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox PriceTextBox;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox ItemTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button BtnAdd;
        private System.Windows.Forms.ListView ItemsList;
        private System.Windows.Forms.ColumnHeader ItemColumn;
        private System.Windows.Forms.ColumnHeader PriceColumn;
        private System.Windows.Forms.ColumnHeader QuantityColumn;
        private System.Windows.Forms.Button btnGetItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox OrderTypeComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label TotalCostLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox AmountPaidTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label BalanceLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox MODComboBox;
        private System.Windows.Forms.Panel BillingPanel;
        private System.Windows.Forms.TextBox txtTransactionNo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label10;
    }
}