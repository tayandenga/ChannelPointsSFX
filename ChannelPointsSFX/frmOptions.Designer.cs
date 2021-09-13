namespace ChannelPointsSFX
{
    partial class frmOptions
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
            this.cbTrayMini = new System.Windows.Forms.CheckBox();
            this.btnResetID = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblVersioning = new System.Windows.Forms.Label();
            this.alertTimer = new System.Windows.Forms.NumericUpDown();
            this.labelTimer = new System.Windows.Forms.Label();
            this.serverBox = new System.Windows.Forms.CheckBox();
            this.httpPort = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.alertTimer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.httpPort)).BeginInit();
            this.SuspendLayout();
            // 
            // cbTrayMini
            // 
            this.cbTrayMini.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbTrayMini.Checked = true;
            this.cbTrayMini.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTrayMini.Location = new System.Drawing.Point(12, 35);
            this.cbTrayMini.Name = "cbTrayMini";
            this.cbTrayMini.Size = new System.Drawing.Size(301, 17);
            this.cbTrayMini.TabIndex = 0;
            this.cbTrayMini.Text = "Minimize to tray";
            this.cbTrayMini.UseVisualStyleBackColor = true;
            this.cbTrayMini.CheckedChanged += new System.EventHandler(this.cbTrayMini_CheckedChanged);
            // 
            // btnResetID
            // 
            this.btnResetID.Location = new System.Drawing.Point(131, 222);
            this.btnResetID.Name = "btnResetID";
            this.btnResetID.Size = new System.Drawing.Size(101, 23);
            this.btnResetID.TabIndex = 1;
            this.btnResetID.Text = "Reset ChannelID";
            this.btnResetID.UseVisualStyleBackColor = true;
            this.btnResetID.Click += new System.EventHandler(this.btnResetID_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(238, 222);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblVersioning
            // 
            this.lblVersioning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersioning.Location = new System.Drawing.Point(9, 9);
            this.lblVersioning.Name = "lblVersioning";
            this.lblVersioning.Size = new System.Drawing.Size(304, 23);
            this.lblVersioning.TabIndex = 3;
            this.lblVersioning.Text = "Channel Points SFX vX.Y.Z Options";
            // 
            // alertTimer
            // 
            this.alertTimer.AccessibleDescription = "";
            this.alertTimer.AccessibleName = "";
            this.alertTimer.Location = new System.Drawing.Point(262, 99);
            this.alertTimer.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.alertTimer.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.alertTimer.Name = "alertTimer";
            this.alertTimer.Size = new System.Drawing.Size(51, 20);
            this.alertTimer.TabIndex = 4;
            this.alertTimer.Tag = "";
            this.alertTimer.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.alertTimer.ValueChanged += new System.EventHandler(this.alertTimer_ValueChanged);
            // 
            // labelTimer
            // 
            this.labelTimer.AutoSize = true;
            this.labelTimer.Location = new System.Drawing.Point(13, 77);
            this.labelTimer.Name = "labelTimer";
            this.labelTimer.Size = new System.Drawing.Size(91, 13);
            this.labelTimer.TabIndex = 5;
            this.labelTimer.Text = "HTTP Server port";
            // 
            // serverBox
            // 
            this.serverBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.serverBox.Enabled = false;
            this.serverBox.Location = new System.Drawing.Point(12, 55);
            this.serverBox.Name = "serverBox";
            this.serverBox.Size = new System.Drawing.Size(301, 17);
            this.serverBox.TabIndex = 6;
            this.serverBox.Text = "HTTP Server status";
            this.serverBox.UseVisualStyleBackColor = true;
            // 
            // httpPort
            // 
            this.httpPort.AccessibleDescription = "";
            this.httpPort.AccessibleName = "";
            this.httpPort.Location = new System.Drawing.Point(262, 74);
            this.httpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.httpPort.Minimum = new decimal(new int[] {
            1001,
            0,
            0,
            0});
            this.httpPort.Name = "httpPort";
            this.httpPort.Size = new System.Drawing.Size(51, 20);
            this.httpPort.TabIndex = 7;
            this.httpPort.Tag = "";
            this.httpPort.Value = new decimal(new int[] {
            8080,
            0,
            0,
            0});
            this.httpPort.ValueChanged += new System.EventHandler(this.httpPort_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(162, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "HTTP Server reward display time";
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 280);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.httpPort);
            this.Controls.Add(this.serverBox);
            this.Controls.Add(this.labelTimer);
            this.Controls.Add(this.alertTimer);
            this.Controls.Add(this.lblVersioning);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnResetID);
            this.Controls.Add(this.cbTrayMini);
            this.MinimumSize = new System.Drawing.Size(341, 296);
            this.Name = "frmOptions";
            this.Text = "Dude22072\'s Channel Points SFX Program";
            this.Load += new System.EventHandler(this.frmOptions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.alertTimer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.httpPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbTrayMini;
        private System.Windows.Forms.Button btnResetID;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblVersioning;
        private System.Windows.Forms.NumericUpDown alertTimer;
        private System.Windows.Forms.Label labelTimer;
        private System.Windows.Forms.CheckBox serverBox;
        private System.Windows.Forms.NumericUpDown httpPort;
        private System.Windows.Forms.Label label1;
    }
}