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
    public partial class Form1 : Form
    {
        public const int ROWS = 16;
        public const int COLUMNS = 16;
        public const int MINES = 25;
        private MineFlagController _controller;

        public Form1()
        {
            InitializeComponent();

        // Instantiate our MineFlagController
            _controller = new MineFlagController(ROWS, COLUMNS, MINES);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void mineButton1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Testclick");
        }

    }
}
