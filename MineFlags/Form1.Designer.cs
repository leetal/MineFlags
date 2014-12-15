namespace MineFlags
{
    partial class Form1
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
            this.mineButton1 = new MineFlags.MineButton();
            this.SuspendLayout();
            // 
            // mineButton1
            // 
            this.mineButton1.Location = new System.Drawing.Point(204, 90);
            this.mineButton1.Name = "mineButton1";
            this.mineButton1.Size = new System.Drawing.Size(75, 23);
            this.mineButton1.TabIndex = 0;
            this.mineButton1.Text = "mineButton1";
            this.mineButton1.UseVisualStyleBackColor = true;
            this.mineButton1.Click += new System.EventHandler(this.mineButton1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 528);
            this.Controls.Add(this.mineButton1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private MineButton mineButton1;
    }
}

