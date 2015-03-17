using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MineFlags
{
    public partial class MineField : Form
    {
        public const int PADDING = 10;
        public const int HEADERHEIGHT = 80;
        public const int BUTTONSIZE = 32;
        public static int ROWS = 16;
        public static int COLUMNS = 16;
        public static int MINES = 1;
        private const String FILENAME = "data.xml";
        private MineFlagController _controller;
        private MineButton[] _mineButtons;
        private Panel _gameContainer;
        private Label _player1Points;
        private Label _player2Points;
        private Label _playerTurn;
        private bool _saving = false;

        public Watcher watcher { get; set; }
        public MineField()
        {
            InitializeComponent();

            // Create a watcher for keeping track on game updates
            watcher = new Watcher(FILENAME);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Add event handlers
            MineFlagController.onMineOpened += _handleMineAction;
            MineFlagController.announceTurn += _handleTurn;
            MineFlagController.onScoreChanged += _handleScoreChanged;
            MineFlagController.onGameCompleted += _handleGameCompleted;

            // Static size on the game
            this.ClientSize = new Size((COLUMNS * BUTTONSIZE + PADDING * 2), (ROWS * BUTTONSIZE + PADDING * 2) + HEADERHEIGHT);
            _createMenu();
            _mineButtons = new MineButton[ROWS * COLUMNS];
            _startGame();
        }

        private void _startGame()
        {
            _setupContainer(this.ClientSize);
            _setupHeader(this.ClientSize);
            _setupMinebuttons(this.ClientSize);
            _controller = new MineFlagController(ROWS, COLUMNS, MINES); // Instantiate our MineFlagController
        }

        private void _reStartGame()
        {

            this.Controls.Remove(_gameContainer);
            _gameContainer.Dispose();
            _controller.Dispose();
            _controller = null;
            _startGame();
        }

        private void _createMenu()
        {
            MenuStrip menu = new MenuStrip();
            menu.BackColor = System.Drawing.Color.FromKnownColor(KnownColor.ControlLight);

            ToolStripMenuItem fileItem = new ToolStripMenuItem("File");
            ToolStripMenuItem itemWithEventAndKey = new ToolStripMenuItem("New Game", null, newGame_Click, (Keys)Shortcut.Alt1);
            fileItem.DropDownItems.Add(itemWithEventAndKey);
            itemWithEventAndKey = new ToolStripMenuItem("Save Game", null, saveGame_Click, (Keys)Shortcut.Alt2);
            fileItem.DropDownItems.Add(itemWithEventAndKey);
            itemWithEventAndKey = new ToolStripMenuItem("Load Game", null, loadGame_Click, (Keys)Shortcut.Alt3);
            fileItem.DropDownItems.Add(itemWithEventAndKey);
            itemWithEventAndKey = new ToolStripMenuItem("Exit", null, exitGame_Click, (Keys)Shortcut.Alt4);
            fileItem.DropDownItems.Add(itemWithEventAndKey);

            menu.Items.Add(fileItem);
            this.Controls.Add(menu);
        }

        // Event that is called from menu item.
        private void newGame_Click(object sender, EventArgs e)
        {
            _reStartGame();
        }

        private void saveGame_Click(object sender, EventArgs e)
        {

        }

        private void loadGame_Click(object sender, EventArgs e)
        {

        }

        private void exitGame_Click(object sender, EventArgs e)
        {
            if (System.Windows.Forms.Application.MessageLoop)
            {
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                System.Environment.Exit(1);
            }
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
            _player1Points = new Label();
            _player1Points.ImageAlign = ContentAlignment.TopLeft;
            _player1Points.Location = new System.Drawing.Point(PADDING, PADDING);
            _player1Points.UseMnemonic = true;
            _player1Points.Text = "Player 1 points: 0";
            _player1Points.Size = new Size(_player1Points.PreferredWidth, _player1Points.PreferredHeight);

            _player2Points = new Label();
            _player2Points.ImageAlign = ContentAlignment.TopLeft;
            _player2Points.Location = new System.Drawing.Point(PADDING, 3 * PADDING);
            _player2Points.UseMnemonic = true;
            _player2Points.Text = "Player 2 points: 0";
            _player2Points.Size = new Size(_player2Points.PreferredWidth, _player2Points.PreferredHeight);

            _playerTurn = new Label();
            _playerTurn.ImageAlign = ContentAlignment.TopLeft;
            _playerTurn.UseMnemonic = true;
            _playerTurn.Text = "Player one's turn";     // "Player 1" is always first
            _playerTurn.Font = new Font(_playerTurn.Font.FontFamily, 24);
            _playerTurn.Size = new Size(_playerTurn.PreferredWidth, _playerTurn.PreferredHeight);
            _playerTurn.Location = new System.Drawing.Point(size.Width / 2 - _playerTurn.Width / 2 - PADDING, HEADERHEIGHT - _playerTurn.Height);

            _gameContainer.Controls.AddRange(new Control[] { _player1Points, _player2Points, _playerTurn });
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
            if (mine.isOpened())
            {
                modifiedMine.adjacentNeighbours = mine.getNeighbours();

                if (mine.isMine())
                {
                    modifiedMine.player = mine.opened_by;
                }
            }

            _saveState();
        }

        private void _handleTurn(Player player)
        {
            if (_playerTurn != null)
                _playerTurn.Text = "Player " + ((player == Player.ONE) ? "one's" : "two's") + " turn";
        }

        private void _handleScoreChanged(Player player, int score)
        {
            Console.WriteLine("Player {0} has a score of {1}", player.ToString(), score.ToString());
            switch (player)
            {
                case Player.ONE:
                    if (_player1Points != null)
                    {
                        _player1Points.Text = "Player 1 points: " + score.ToString();
                        _player1Points.Size = new Size(_player1Points.PreferredWidth, _player1Points.PreferredHeight);
                    }
                    break;
                case Player.TWO:
                    if (_player2Points != null)
                    {
                        _player2Points.Text = "Player 2 points: " + score.ToString();
                        _player2Points.Size = new Size(_player2Points.PreferredWidth, _player2Points.PreferredHeight);
                    }
                    break;
                default:
                    break;
            }
        }

        private void _saveState()
        {
            Console.WriteLine("--> About to save the state");
            StateHandler.exportToStorage(_controller, FILENAME);
        }
        private void _handleGameCompleted(Player player)
        {
            MessageBox.Show(this, "Player " + ((player == Player.ONE) ? "1" : "2") + " won!");
            Console.WriteLine("Game completed");
        }
    }
}