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
        private Panel _gameContainer;

        public MineField()
        {

            MineFlagController.onMineOpened += _handleMineAction;
            MineFlagController.announceTurn += _handleTurn;
            InitializeComponent();

            // Instantiate our MineFlagController
            _controller = new MineFlagController(ROWS, COLUMNS, MINES);
            _mineButtons = new MineButton[ROWS * COLUMNS];
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Static size on the game
            this.ClientSize = new Size((COLUMNS * BUTTONSIZE + PADDING * 2), (ROWS * BUTTONSIZE + PADDING * 2) + HEADERHEIGHT);
            _startGame(1);
        }

        private void _startGame(int type)
        {
            _setupContainer(this.ClientSize);
            _setupHeader(this.ClientSize);
            _setupMinebuttons(this.ClientSize);
            
        }

        private void _setupContainer(Size size)
        {
            Console.WriteLine("Width:" + size.Width.ToString() + ", Height: " + size.Height.ToString());
            // Add a container for all controls
            _gameContainer = new Panel();
            _gameContainer.Size = new Size(size.Width - 2 * PADDING, size.Height - 2 * PADDING);
            _gameContainer.Location = new System.Drawing.Point(PADDING, PADDING);
            _gameContainer.Dock = DockStyle.None;
            this.Controls.Add(_gameContainer);
        }

        private void _setupHeader(Size size)
        {
            Label player1Points = new Label();
            player1Points.ImageAlign = ContentAlignment.TopLeft;
            player1Points.Location = new System.Drawing.Point(PADDING, PADDING);
            //player1Points.BackColor = System.Drawing.Color.FromArgb(255, 210, 210, 210); // Taken (Gray)
            player1Points.UseMnemonic = true;
            player1Points.Text = "Player 1 points: ";
            player1Points.Size = new Size(player1Points.PreferredWidth, player1Points.PreferredHeight);

            Label player2Points = new Label();
            player2Points.ImageAlign = ContentAlignment.TopLeft;
            player2Points.Location = new System.Drawing.Point(PADDING, 3 * PADDING);
            //player2Points.BackColor = System.Drawing.Color.FromArgb(255, 210, 210, 210); // Taken (Gray)
            player2Points.UseMnemonic = true;
            player2Points.Text = "Player 2 points: ";
            player2Points.Size = new Size(player2Points.PreferredWidth, player2Points.PreferredHeight);

            Label playerTurn = new Label();
            playerTurn.ImageAlign = ContentAlignment.TopLeft;
            //player2Points.BackColor = System.Drawing.Color.FromArgb(255, 210, 210, 210); // Taken (Gray)
            playerTurn.UseMnemonic = true;
            playerTurn.Text = "Turn";
            playerTurn.Font = new Font(playerTurn.Font.FontFamily, 30);
            playerTurn.Size = new Size(playerTurn.PreferredWidth, playerTurn.PreferredHeight);
            playerTurn.Location = new System.Drawing.Point(size.Width / 2 - playerTurn.Width / 2 - PADDING, HEADERHEIGHT - playerTurn.Height);

            _gameContainer.Controls.AddRange(new Control[] { player1Points, player2Points, playerTurn });
        }

        private void _setupMinebuttons(Size size)
        {
            int currentButtonIndex = 0;
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLUMNS; col++)
                {
                    MineFlags.MineButton tempButton = new MineButton();
                    tempButton.Location = new System.Drawing.Point((col * BUTTONSIZE), (row * BUTTONSIZE) + HEADERHEIGHT);
                    tempButton.Name = "MineButton" + currentButtonIndex.ToString();
                    tempButton.Size = new System.Drawing.Size(BUTTONSIZE, BUTTONSIZE);
                    tempButton.TabIndex = 0;
                    tempButton.Tag = currentButtonIndex;
                    tempButton.Click += new System.EventHandler(this.mineButtonClickEvent);
                    _mineButtons[currentButtonIndex] = tempButton;
                    currentButtonIndex++;
                }
            }
            // Add the buttons as a range
            _gameContainer.Controls.AddRange(_mineButtons);
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
            Console.WriteLine("Mineaction cathced: " + mine.index.ToString());
            // Catch onMineOpened event
            // Update view accordingly
            MineButton modifiedMine = _mineButtons[mine.index];
            if (mine.isOpened()) {
                modifiedMine.adjacentNeighbours = mine.getNeighbours();

                if (mine.isMine()) {
                    modifiedMine.player = mine.opened_by;
                }
            }
        }

        private void _handleTurn(Player player) {
            Console.WriteLine("Player " + ((player == Player.ONE) ? "ONE" : "TWO"));
        }

    }
}