namespace OsrsColorBot
{
    partial class OsrsGui
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
            this.btnPowerMine = new System.Windows.Forms.Button();
            this.numIterations = new System.Windows.Forms.NumericUpDown();
            this.lblIterations = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numIterations)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPowerMine
            // 
            this.btnPowerMine.Location = new System.Drawing.Point(97, 141);
            this.btnPowerMine.Name = "btnPowerMine";
            this.btnPowerMine.Size = new System.Drawing.Size(75, 23);
            this.btnPowerMine.TabIndex = 0;
            this.btnPowerMine.Text = "Power Mine";
            this.btnPowerMine.UseVisualStyleBackColor = true;
            this.btnPowerMine.Click += new System.EventHandler(this.btnPowerMine_Click);
            // 
            // numIterations
            // 
            this.numIterations.Location = new System.Drawing.Point(69, 72);
            this.numIterations.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numIterations.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numIterations.Name = "numIterations";
            this.numIterations.Size = new System.Drawing.Size(120, 20);
            this.numIterations.TabIndex = 1;
            this.numIterations.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // lblIterations
            // 
            this.lblIterations.AutoSize = true;
            this.lblIterations.Location = new System.Drawing.Point(69, 53);
            this.lblIterations.Name = "lblIterations";
            this.lblIterations.Size = new System.Drawing.Size(102, 13);
            this.lblIterations.TabIndex = 2;
            this.lblIterations.Text = "Number of Iterations";
            // 
            // OsrsGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.lblIterations);
            this.Controls.Add(this.numIterations);
            this.Controls.Add(this.btnPowerMine);
            this.Name = "OsrsGui";
            this.Text = "OsrsGui";
            ((System.ComponentModel.ISupportInitialize)(this.numIterations)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPowerMine;
        private System.Windows.Forms.NumericUpDown numIterations;
        private System.Windows.Forms.Label lblIterations;
    }
}