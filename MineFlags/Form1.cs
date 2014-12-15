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
    public partial class MineField : Form
    {
        public const int PADDING = 10;
        public const int BUTTONSIZE = 32;
        public static int ROWS = 16;
        public static int COLUMNS = 16;
        public static int MINES = 25;
        private MineFlagController _controller;
        private MineButton[] _mineButtons;

        public MineField()
        {
            InitializeComponent();

        // Instantiate our MineFlagController
            _controller = new MineFlagController(ROWS, COLUMNS, MINES);
            _mineButtons = new MineButton[ROWS * COLUMNS];
        }

        public Rectangle getScreen()
        {
            return Screen.FromControl(this).Bounds;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new Size((COLUMNS * BUTTONSIZE + PADDING * 2) + 18, (ROWS * BUTTONSIZE + PADDING * 2) + 45);
            _setupMinebuttons(getScreen());
        }

        private void _setupMinebuttons(Rectangle size)
        {
            int currentButtonIndex = 0;

            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLUMNS; col++)
                {
                    MineFlags.MineButton tempButton = new MineButton(MineButtonState.CLOSED);
                    tempButton.Location = new System.Drawing.Point(PADDING + (col * BUTTONSIZE), PADDING + (row * BUTTONSIZE));
                    tempButton.Name = "MineButton"+currentButtonIndex.ToString();
                    tempButton.Size = new System.Drawing.Size(BUTTONSIZE, BUTTONSIZE);
                    tempButton.TabIndex = 0;
                    tempButton.Tag = currentButtonIndex;
                    tempButton.Click += new System.EventHandler(this.mineButtonClickEvent);

                    this.Controls.Add(tempButton);

                    _mineButtons[currentButtonIndex] = tempButton;
                    
                    currentButtonIndex++;
                }
            }
        }
               

        private void mineButtonClickEvent(object sender, EventArgs e)
        {
            MineButton caller = (MineButton)sender;
            Console.WriteLine("Button click at:" + caller.Tag.ToString());
        }

    }
}
