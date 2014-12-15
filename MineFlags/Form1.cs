using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MineFlags
{
    public partial class MineFlagField : Form
    {
        // Private members
        private MineButton[] mineButtons;

        public MineFlagField()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void mineButton1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Testclick");
        }

        // Private memeber functions

        // Helper to setup the mines on the field.
        private void _setupButtons(System.Drawing.Rectangle size)
        {
            /*
            this.mineButton1 = new MineFlags.MineButton();
            this.mineButton1.Location = new System.Drawing.Point(54, 28);
            this.mineButton1.Name = "mineButton1";
            this.mineButton1.Size = new System.Drawing.Size(75, 40);
            this.mineButton1.TabIndex = 0;
            this.mineButton1.Text = "mineButton1";
            this.mineButton1.UseVisualStyleBackColor = true;
            this.mineButton1.Click += new System.EventHandler(this.mineButton1_Click);*/
        }

        // Public accessors

        public Rectangle GetScreenSize()
        {
            return Screen.FromControl(this).Bounds;
        }

    }
}
