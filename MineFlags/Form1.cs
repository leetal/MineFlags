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
        public const int HEADERHEIGHT = 80;
        public const int BUTTONSIZE = 32;
        public static int ROWS = 16;
        public static int COLUMNS = 16;
        public static int MINES = 50;
        private MineFlagController _controller;
        private MineButton[] _mineButtons;

        public MineField()
        {

            MineFlagController.onMineOpened += _handleMineAction;
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
            this.Size = new Size((COLUMNS * BUTTONSIZE + PADDING * 2) + 18, (ROWS * BUTTONSIZE + PADDING * 2) + 45 + HEADERHEIGHT);
            _setupMinebuttons(getScreen());
            _setupHeader();
        }

        private void _setupHeader()
        {
            Label player1Points = new Label();
            player1Points.ImageAlign = ContentAlignment.TopLeft;
            player1Points.Location = new System.Drawing.Point(PADDING,PADDING);
            player1Points.BackColor = System.Drawing.Color.FromArgb(255, 210, 210, 210); // Taken (Gray)
            player1Points.UseMnemonic = true;
            player1Points.Text = "Player 1 points: ";
            player1Points.Size = new Size(player1Points.PreferredWidth, player1Points.PreferredHeight);
            this.Controls.Add(player1Points);
        }

        private void _setupMinebuttons(Rectangle size)
        {
            int currentButtonIndex = 0;

            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLUMNS; col++)
                {
                    MineFlags.MineButton tempButton = new MineButton();
                    tempButton.Location = new System.Drawing.Point(PADDING + (col * BUTTONSIZE), HEADERHEIGHT + PADDING + (row * BUTTONSIZE));
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
            int clickedIndex = (int)caller.Tag;
            Console.WriteLine("Button click at:" + clickedIndex.ToString());
            // Handle mine open event
            _controller.openMine(clickedIndex);
        }

        private void _handleMineAction(Mine mine)
        {
            Console.WriteLine("Mineaction cathced: "+mine.index.ToString());
            // Catch onMineOpened event
            // Update view accordingly
            MineButton modifiedMine = _mineButtons[mine.index];
            if (mine.isOpened())
            {
                modifiedMine.adjacentNeighbours = mine.getNeighbours();
                if (mine.isMine())
                {
                    modifiedMine.player = Mine.Player.TWO;
                }
            }
        }

    }
}