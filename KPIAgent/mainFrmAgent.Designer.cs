namespace KPIAgent
{
    partial class mainFrmAgent
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainFrmAgent));
            this.KPIAgent = new System.Windows.Forms.NotifyIcon(this.components);
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.dateSelect = new System.Windows.Forms.DateTimePicker();
            this.pBar = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblTransactionType = new System.Windows.Forms.Label();
            this.lblBcFrom = new System.Windows.Forms.Label();
            this.dash = new System.Windows.Forms.Label();
            this.lblBcTo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // KPIAgent
            // 
            this.KPIAgent.Icon = ((System.Drawing.Icon)(resources.GetObject("KPIAgent.Icon")));
            this.KPIAgent.Text = "KPI Agent";
            this.KPIAgent.Visible = true;
            this.KPIAgent.DoubleClick += new System.EventHandler(this.KPIAgent_DoubleClick);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(149, 94);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(96, 51);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "START";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(35, 94);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(96, 51);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // dateSelect
            // 
            this.dateSelect.CustomFormat = "yyyy-MM-dd";
            this.dateSelect.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateSelect.Location = new System.Drawing.Point(93, 56);
            this.dateSelect.Name = "dateSelect";
            this.dateSelect.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dateSelect.Size = new System.Drawing.Size(96, 20);
            this.dateSelect.TabIndex = 2;
            // 
            // pBar
            // 
            this.pBar.Location = new System.Drawing.Point(-1, 172);
            this.pBar.Name = "pBar";
            this.pBar.Size = new System.Drawing.Size(283, 29);
            this.pBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pBar.TabIndex = 3;
            this.pBar.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblTransactionType
            // 
            this.lblTransactionType.AutoSize = true;
            this.lblTransactionType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransactionType.Location = new System.Drawing.Point(42, 18);
            this.lblTransactionType.Name = "lblTransactionType";
            this.lblTransactionType.Size = new System.Drawing.Size(111, 20);
            this.lblTransactionType.TabIndex = 4;
            this.lblTransactionType.Text = "DOMESTIC :";
            // 
            // lblBcFrom
            // 
            this.lblBcFrom.AutoSize = true;
            this.lblBcFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBcFrom.Location = new System.Drawing.Point(148, 18);
            this.lblBcFrom.Name = "lblBcFrom";
            this.lblBcFrom.Size = new System.Drawing.Size(39, 20);
            this.lblBcFrom.TabIndex = 5;
            this.lblBcFrom.Text = "000";
            // 
            // dash
            // 
            this.dash.AutoSize = true;
            this.dash.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dash.Location = new System.Drawing.Point(186, 18);
            this.dash.Name = "dash";
            this.dash.Size = new System.Drawing.Size(15, 20);
            this.dash.TabIndex = 6;
            this.dash.Text = "-";
            // 
            // lblBcTo
            // 
            this.lblBcTo.AutoSize = true;
            this.lblBcTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBcTo.Location = new System.Drawing.Point(201, 18);
            this.lblBcTo.Name = "lblBcTo";
            this.lblBcTo.Size = new System.Drawing.Size(39, 20);
            this.lblBcTo.TabIndex = 7;
            this.lblBcTo.Text = "000";
            // 
            // mainFrmAgent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 201);
            this.Controls.Add(this.lblBcTo);
            this.Controls.Add(this.dash);
            this.Controls.Add(this.lblBcFrom);
            this.Controls.Add(this.lblTransactionType);
            this.Controls.Add(this.pBar);
            this.Controls.Add(this.dateSelect);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "mainFrmAgent";
            this.Text = "KPI Agent";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon KPIAgent;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.DateTimePicker dateSelect;
        public System.Windows.Forms.ProgressBar pBar;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblTransactionType;
        private System.Windows.Forms.Label lblBcFrom;
        private System.Windows.Forms.Label dash;
        private System.Windows.Forms.Label lblBcTo;
    }
}

