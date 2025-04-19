namespace LOAN_MANAGEMENT_SOFTWARE
{
    partial class frm_borrower_main_form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_borrower_main_form));
            this.panel5 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtRole = new System.Windows.Forms.TextBox();
            this.txtID = new System.Windows.Forms.TextBox();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnExpense = new System.Windows.Forms.Button();
            this.btnDownPayment = new System.Windows.Forms.Button();
            this.btnOrders = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblUser = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxProfile = new System.Windows.Forms.PictureBox();
            this.main_panel = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel7 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblBusinessName = new System.Windows.Forms.Label();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.panel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProfile)).BeginInit();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(12)))), ((int)(((byte)(81)))));
            this.panel5.Controls.Add(this.label8);
            this.panel5.Controls.Add(this.label10);
            this.panel5.Controls.Add(this.label13);
            this.panel5.Controls.Add(this.txtRole);
            this.panel5.Controls.Add(this.txtID);
            this.panel5.Controls.Add(this.lblDateTime);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.ForeColor = System.Drawing.Color.White;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1504, 29);
            this.panel5.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(231)))), ((int)(((byte)(237)))));
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label8.Dock = System.Windows.Forms.DockStyle.Right;
            this.label8.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label8.Font = new System.Drawing.Font("Arial", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(799, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(235, 29);
            this.label8.TabIndex = 0;
            this.label8.Text = "   Profile Management   ";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label8.Visible = false;
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(231)))), ((int)(((byte)(237)))));
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label10.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label10.Dock = System.Windows.Forms.DockStyle.Right;
            this.label10.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label10.Font = new System.Drawing.Font("Arial", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(1034, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(235, 29);
            this.label10.TabIndex = 2;
            this.label10.Text = "   Calculator   ";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label10.Visible = false;
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(50)))), ((int)(((byte)(0)))));
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label13.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label13.Dock = System.Windows.Forms.DockStyle.Right;
            this.label13.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label13.Font = new System.Drawing.Font("Arial", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.White;
            this.label13.Location = new System.Drawing.Point(1269, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(235, 29);
            this.label13.TabIndex = 5;
            this.label13.Text = "   Logout   ";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label13.Visible = false;
            this.label13.Click += new System.EventHandler(this.label13_Click);
            // 
            // txtRole
            // 
            this.txtRole.Enabled = false;
            this.txtRole.Location = new System.Drawing.Point(436, -1);
            this.txtRole.Name = "txtRole";
            this.txtRole.Size = new System.Drawing.Size(100, 30);
            this.txtRole.TabIndex = 0;
            this.txtRole.Visible = false;
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(542, -4);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(100, 30);
            this.txtID.TabIndex = 9;
            this.txtID.Visible = false;
            // 
            // lblDateTime
            // 
            this.lblDateTime.AutoEllipsis = true;
            this.lblDateTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(12)))), ((int)(((byte)(81)))));
            this.lblDateTime.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblDateTime.Font = new System.Drawing.Font("Arial Black", 16.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateTime.ForeColor = System.Drawing.Color.White;
            this.lblDateTime.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDateTime.Location = new System.Drawing.Point(0, 0);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(734, 29);
            this.lblDateTime.TabIndex = 0;
            this.lblDateTime.Text = "DateTime";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 29);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(237, 717);
            this.panel2.TabIndex = 18;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnExpense);
            this.panel1.Controls.Add(this.btnDownPayment);
            this.panel1.Controls.Add(this.btnOrders);
            this.panel1.Controls.Add(this.btnDashboard);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 271);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(233, 657);
            this.panel1.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlLight;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Font = new System.Drawing.Font("Arial", 12F);
            this.btnClose.ForeColor = System.Drawing.Color.Black;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(0, 608);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(233, 49);
            this.btnClose.TabIndex = 31;
            this.btnClose.Text = "  Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Visible = false;
            // 
            // btnExpense
            // 
            this.btnExpense.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.btnExpense.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExpense.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnExpense.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlLight;
            this.btnExpense.FlatAppearance.BorderSize = 0;
            this.btnExpense.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExpense.Font = new System.Drawing.Font("Arial", 12F);
            this.btnExpense.ForeColor = System.Drawing.Color.Black;
            this.btnExpense.Image = ((System.Drawing.Image)(resources.GetObject("btnExpense.Image")));
            this.btnExpense.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExpense.Location = new System.Drawing.Point(0, 147);
            this.btnExpense.Name = "btnExpense";
            this.btnExpense.Size = new System.Drawing.Size(233, 49);
            this.btnExpense.TabIndex = 16;
            this.btnExpense.Text = "  Logout\r\n";
            this.btnExpense.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExpense.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExpense.UseVisualStyleBackColor = false;
            this.btnExpense.Click += new System.EventHandler(this.btnExpense_Click);
            // 
            // btnDownPayment
            // 
            this.btnDownPayment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.btnDownPayment.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDownPayment.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDownPayment.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlLight;
            this.btnDownPayment.FlatAppearance.BorderSize = 0;
            this.btnDownPayment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownPayment.Font = new System.Drawing.Font("Arial", 12F);
            this.btnDownPayment.ForeColor = System.Drawing.Color.Black;
            this.btnDownPayment.Image = ((System.Drawing.Image)(resources.GetObject("btnDownPayment.Image")));
            this.btnDownPayment.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDownPayment.Location = new System.Drawing.Point(0, 98);
            this.btnDownPayment.Name = "btnDownPayment";
            this.btnDownPayment.Size = new System.Drawing.Size(233, 49);
            this.btnDownPayment.TabIndex = 34;
            this.btnDownPayment.Text = "  Loan Payment";
            this.btnDownPayment.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDownPayment.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDownPayment.UseVisualStyleBackColor = false;
            this.btnDownPayment.Click += new System.EventHandler(this.btnDownPayment_Click);
            // 
            // btnOrders
            // 
            this.btnOrders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.btnOrders.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOrders.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOrders.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlLight;
            this.btnOrders.FlatAppearance.BorderSize = 0;
            this.btnOrders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOrders.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOrders.ForeColor = System.Drawing.Color.Black;
            this.btnOrders.Image = ((System.Drawing.Image)(resources.GetObject("btnOrders.Image")));
            this.btnOrders.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOrders.Location = new System.Drawing.Point(0, 49);
            this.btnOrders.Name = "btnOrders";
            this.btnOrders.Size = new System.Drawing.Size(233, 49);
            this.btnOrders.TabIndex = 33;
            this.btnOrders.Text = "  Loan Application";
            this.btnOrders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOrders.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOrders.UseVisualStyleBackColor = false;
            this.btnOrders.Click += new System.EventHandler(this.btnOrders_Click_1);
            // 
            // btnDashboard
            // 
            this.btnDashboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.btnDashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDashboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDashboard.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlLight;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboard.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDashboard.ForeColor = System.Drawing.Color.Black;
            this.btnDashboard.Image = ((System.Drawing.Image)(resources.GetObject("btnDashboard.Image")));
            this.btnDashboard.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashboard.Location = new System.Drawing.Point(0, 0);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(233, 49);
            this.btnDashboard.TabIndex = 11;
            this.btnDashboard.Text = "  Dashboard";
            this.btnDashboard.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDashboard.UseVisualStyleBackColor = false;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.panel4.Controls.Add(this.lblUser);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.pictureBoxProfile);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(233, 271);
            this.panel4.TabIndex = 1;
            this.panel4.Paint += new System.Windows.Forms.PaintEventHandler(this.panel4_Paint);
            // 
            // lblUser
            // 
            this.lblUser.AutoEllipsis = true;
            this.lblUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.lblUser.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblUser.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblUser.Font = new System.Drawing.Font("Arial Black", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUser.ForeColor = System.Drawing.Color.Black;
            this.lblUser.Location = new System.Drawing.Point(0, 204);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(233, 32);
            this.lblUser.TabIndex = 7;
            this.lblUser.Text = "Name";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoEllipsis = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.label1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(0, 236);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(233, 35);
            this.label1.TabIndex = 8;
            this.label1.Text = "Borrower";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxProfile
            // 
            this.pictureBoxProfile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxProfile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxProfile.Location = new System.Drawing.Point(11, 10);
            this.pictureBoxProfile.Name = "pictureBoxProfile";
            this.pictureBoxProfile.Size = new System.Drawing.Size(210, 184);
            this.pictureBoxProfile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxProfile.TabIndex = 6;
            this.pictureBoxProfile.TabStop = false;
            // 
            // main_panel
            // 
            this.main_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.main_panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.main_panel.Location = new System.Drawing.Point(250, 133);
            this.main_panel.Name = "main_panel";
            this.main_panel.Size = new System.Drawing.Size(1238, 598);
            this.main_panel.TabIndex = 19;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel7
            // 
            this.panel7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel7.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel7.BackgroundImage")));
            this.panel7.Controls.Add(this.pictureBox1);
            this.panel7.Controls.Add(this.lblBusinessName);
            this.panel7.Controls.Add(this.pictureBoxLogo);
            this.panel7.Location = new System.Drawing.Point(250, 41);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(1238, 76);
            this.panel7.TabIndex = 20;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(890, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(65, 67);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // lblBusinessName
            // 
            this.lblBusinessName.AutoEllipsis = true;
            this.lblBusinessName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(12)))), ((int)(((byte)(81)))));
            this.lblBusinessName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBusinessName.Font = new System.Drawing.Font("Arial Black", 28.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBusinessName.ForeColor = System.Drawing.Color.White;
            this.lblBusinessName.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBusinessName.Location = new System.Drawing.Point(233, 0);
            this.lblBusinessName.Name = "lblBusinessName";
            this.lblBusinessName.Size = new System.Drawing.Size(1005, 76);
            this.lblBusinessName.TabIndex = 3;
            this.lblBusinessName.Text = "Business Name";
            this.lblBusinessName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.BackColor = System.Drawing.Color.White;
            this.pictureBoxLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxLogo.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBoxLogo.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(233, 76);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLogo.TabIndex = 2;
            this.pictureBoxLogo.TabStop = false;
            // 
            // frm_borrower_main_form
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(140)))), ((int)(((byte)(223)))));
            this.ClientSize = new System.Drawing.Size(1504, 746);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.main_panel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel5);
            this.Font = new System.Drawing.Font("Arial", 12F);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frm_borrower_main_form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LOAN MANAGEMENT SYSTEM";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_borrower_main_form_FormClosing);
            this.Load += new System.EventHandler(this.frm_borrower_main_form_Load);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProfile)).EndInit();
            this.panel7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel5;
        public System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        public System.Windows.Forms.Button btnExpense;
        public System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Panel main_panel;
        private System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.TextBox txtRole;
        public System.Windows.Forms.Label label13;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.PictureBox pictureBoxProfile;
        public System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel4;
        public System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label lblBusinessName;
        public System.Windows.Forms.Button btnOrders;
        private System.Windows.Forms.Button btnDownPayment;
    }
}