namespace WoTGame
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tmrTimer = new System.Windows.Forms.Timer(this.components);
            this.btnRoll = new System.Windows.Forms.Button();
            this.lblGameState = new System.Windows.Forms.Label();
            this.lblRoll = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tmrTimer
            // 
            this.tmrTimer.Enabled = true;
            // 
            // btnRoll
            // 
            this.btnRoll.Location = new System.Drawing.Point(470, 248);
            this.btnRoll.Name = "btnRoll";
            this.btnRoll.Size = new System.Drawing.Size(100, 23);
            this.btnRoll.TabIndex = 0;
            this.btnRoll.Text = "Roll";
            this.btnRoll.UseVisualStyleBackColor = true;
            this.btnRoll.Click += new System.EventHandler(this.btnRoll_Click);
            // 
            // lblGameState
            // 
            this.lblGameState.AutoSize = true;
            this.lblGameState.Location = new System.Drawing.Point(457, 109);
            this.lblGameState.Name = "lblGameState";
            this.lblGameState.Size = new System.Drawing.Size(71, 13);
            this.lblGameState.TabIndex = 1;
            this.lblGameState.Text = "Player 1 Rolls";
            // 
            // lblRoll
            // 
            this.lblRoll.AutoSize = true;
            this.lblRoll.Location = new System.Drawing.Point(460, 144);
            this.lblRoll.Name = "lblRoll";
            this.lblRoll.Size = new System.Drawing.Size(38, 13);
            this.lblRoll.TabIndex = 2;
            this.lblRoll.Text = "Ready";
            // 
            // lblScore
            // 
            this.lblScore.AutoSize = true;
            this.lblScore.Location = new System.Drawing.Point(470, 419);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(94, 13);
            this.lblScore.TabIndex = 3;
            this.lblScore.Text = "Score: In Progress";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.lblScore);
            this.Controls.Add(this.lblRoll);
            this.Controls.Add(this.lblGameState);
            this.Controls.Add(this.btnRoll);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Snakes and Foxes";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseClick);
            this.Resize += new System.EventHandler(this.FrmMain_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();
            this.tmrTimer.Tick += this.TmrTimer_Tick;
            this.tmrTimer.Interval = 100;
            this.tmrTimer.Enabled = true;
        }

        #endregion

        private System.Windows.Forms.Timer tmrTimer;
        private System.Windows.Forms.Button btnRoll;
        private System.Windows.Forms.Label lblGameState;
        private System.Windows.Forms.Label lblRoll;
        private System.Windows.Forms.Label lblScore;
    }
}

