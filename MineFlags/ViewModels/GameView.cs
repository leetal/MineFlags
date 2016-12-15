using System;
using System.Drawing;
using System.Windows.Forms;
using MineFlags.PlayerType;
using MineFlags.GenericTypes;
using MineFlags.Logic;

// LINQ: https://msdn.microsoft.com/en-us/library/bb308959.aspx

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
        private MineFlagController Controller;
        private MineButton[] MineButtons;
        private Panel GameContainer;
        private Label Player1Points;
        private Label Player2Points;
        private Label PlayerTurn;
        private PlayerNum CurrentPlayerNumber;

        public MineField()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Add event handlers
            BaseController.MineOpenedEvent += HandleMineAction;
            BaseController.AnnounceTurnEvent += HandleTurn;
            BaseController.ScoreChangedEvent += HandleScoreChanged;
            BaseController.GameCompletedEvent += HandleGameCompleted;

            // Static size on the game
            ClientSize = new Size((COLUMNS * BUTTONSIZE + PADDING * 2), (ROWS * BUTTONSIZE + PADDING * 2) + HEADERHEIGHT);
            CreateMenu();
            
            // Instantiate minebuttons array
            MineButtons = new MineButton[ROWS * COLUMNS];

            StartGame();
        }

        private void StartGame()
        {
            SetupContainer(ClientSize);
            SetupHeader(ClientSize);
            SetupMinebuttons(ClientSize);

            // Must exists POST-init of controls
            if (Controller == null)
            {
                Controller = new MineFlagController(); // Instantiate our MineFlagController (the main game logic)
                // Instantiate a new game WITHOUT AI from the start
                Controller.NewGame(ROWS, COLUMNS, MINES, false);
            }
        }

        private void NewGame(bool ai)
        {
            if (GameContainer != null)
            {
                Controls.Remove(GameContainer);
                GameContainer.Dispose();
            }

            // New game!
            if (Controller != null)
                Controller.NewGame(ROWS, COLUMNS, MINES, ai);

            StartGame();
        }

        private void CreateMenu()
        {
            MenuStrip menu = new MenuStrip();
            menu.BackColor = System.Drawing.Color.FromKnownColor(KnownColor.ControlLight);

            ToolStripMenuItem fileItem = new ToolStripMenuItem("File");
            ToolStripMenuItem itemWithEventAndKey = new ToolStripMenuItem("New Game");
            ToolStripMenuItem subitemWithEventAndKey = new ToolStripMenuItem("New Game with other player", null, NewGameClick, (Keys)Shortcut.CtrlN);
            itemWithEventAndKey.DropDownItems.Add(subitemWithEventAndKey);
            subitemWithEventAndKey = new ToolStripMenuItem("New Game with AI", null, NewGameAIClick, (Keys)Shortcut.CtrlA);
            itemWithEventAndKey.DropDownItems.Add(subitemWithEventAndKey);
            fileItem.DropDownItems.Add(itemWithEventAndKey);
            itemWithEventAndKey = new ToolStripMenuItem("Exit", null, ExitGameClick, (Keys)Shortcut.AltF4);
            fileItem.DropDownItems.Add(itemWithEventAndKey);

            menu.Items.Add(fileItem);
            Controls.Add(menu);
        }

        // Event that is called from menu item.
        private void NewGameClick(object sender, EventArgs e)
        {
            NewGame(false);
        }

        private void NewGameAIClick(object sender, EventArgs e)
        {
            NewGame(true);
        }

        private void ExitGameClick(object sender, EventArgs e)
        {
            // Gracefully shutdown
            if (Application.MessageLoop)
            {
                Application.Exit();
            }
            else
            {
                Environment.Exit(1);
            }
        }

        private void SetupContainer(Size size)
        {
            // Add a container for all controls
            GameContainer = new Panel();
            GameContainer.Size = new Size(size.Width - 2 * PADDING, size.Height - 2 * PADDING);
            GameContainer.Location = new Point(PADDING, PADDING);
            GameContainer.Dock = DockStyle.None;
            Controls.Add(GameContainer);
        }

        private void SetupHeader(Size size)
        {
            Player1Points = new Label();
            Player1Points.ImageAlign = ContentAlignment.TopLeft;
            Player1Points.Location = new Point(PADDING, 4 * PADDING);
            Player1Points.UseMnemonic = true;
            Player1Points.Text = "Player 1 points: 0";
            Player1Points.Size = new Size(Player1Points.PreferredWidth, Player1Points.PreferredHeight);

            Player2Points = new Label();
            Player2Points.ImageAlign = ContentAlignment.TopLeft;
            Player2Points.Location = new Point(PADDING, 6 * PADDING);
            Player2Points.UseMnemonic = true;
            Player2Points.Text = "Player 2 points: 0";
            Player2Points.Size = new Size(Player2Points.PreferredWidth, Player2Points.PreferredHeight);

            PlayerTurn = new Label();
            PlayerTurn.ImageAlign = ContentAlignment.TopLeft;
            PlayerTurn.UseMnemonic = true;
            PlayerTurn.Text = "Player one's turn";     // "Player 1" is always first
            PlayerTurn.Font = new Font(PlayerTurn.Font.FontFamily, 24);
            PlayerTurn.Size = new Size(PlayerTurn.PreferredWidth, PlayerTurn.PreferredHeight);
            PlayerTurn.Location = new Point(size.Width / 2 - PlayerTurn.Width / 2 - PADDING, HEADERHEIGHT - PlayerTurn.Height);

            GameContainer.Controls.AddRange(new Control[] { Player1Points, Player2Points, PlayerTurn });
        }

        private void SetupMinebuttons(Size size)
        {
            int currentButtonIndex = 0;
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLUMNS; col++)
                {
                    MineFlags.MineButton tempButton = new MineButton();
                    tempButton.Location = new Point((col * BUTTONSIZE), (row * BUTTONSIZE) + HEADERHEIGHT);
                    tempButton.Name = "MineButton" + currentButtonIndex.ToString();
                    tempButton.Size = new Size(BUTTONSIZE, BUTTONSIZE);
                    tempButton.TabIndex = 0;
                    tempButton.Tag = currentButtonIndex;
                    tempButton.Click += new EventHandler(mineButtonClickEvent);
                    MineButtons[currentButtonIndex] = tempButton;
                    currentButtonIndex++;
                }
            }

            // Add the buttons as a range
            GameContainer.Controls.AddRange(MineButtons);
        }

        private void mineButtonClickEvent(object sender, EventArgs e)
        {
            MineButton caller = (MineButton)sender;
            int clickedIndex = (int)caller.Tag;

            // Signal to the controller to open a mine
            BaseController.OnOpenMine(clickedIndex, CurrentPlayerNumber);
        }

        private void HandleMineAction(PlayerNum playerNumber, Mine mine, bool success)
        {
            // Catch onMineOpened event
            // Update view accordingly

            MineButton modifiedMine = MineButtons[mine.index];
            if (mine.IsOpened())
            {
                modifiedMine.adjacentNeighbours = mine.GetNeighbours();

                if (mine.IsMine())
                {
                    modifiedMine.PlayerNumber = mine.OpenedBy;
                }
            }
        }

        private void HandleTurn(PlayerNum playerNumber)
        {
            CurrentPlayerNumber = playerNumber;

            if (PlayerTurn != null)
                PlayerTurn.Text = "Player " + ((playerNumber == PlayerNum.ONE) ? "one's" : "two's") + " turn";
        }

        private void HandleScoreChanged(ref IPlayer player, int score)
        {
            Console.WriteLine("Player {0} has a score of {1}", player.ToString(), score.ToString());
            switch (player.GetPlayerNumber())
            {
                case PlayerNum.ONE:
                    if (Player1Points != null)
                    {
                        Player1Points.Text = "Player 1 points: " + score.ToString();
                        Player1Points.Size = new Size(Player1Points.PreferredWidth, Player1Points.PreferredHeight);
                    }
                    break;
                case PlayerNum.TWO:
                    if (Player2Points != null)
                    {
                        Player2Points.Text = "Player 2 points: " + score.ToString();
                        Player2Points.Size = new Size(Player2Points.PreferredWidth, Player2Points.PreferredHeight);
                    }
                    break;
                default:
                    break;
            }
        }

        private void HandleGameCompleted(ref IPlayer player)
        {
            MessageBox.Show(this, "Player " + player.GetPlayerNumber().ToString() + " won!");
        }
    }
}