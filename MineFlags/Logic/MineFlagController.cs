using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MineFlags.PlayerType;
using MineFlags.GenericTypes;
using MineFlags.RulesEngine;

namespace MineFlags.Logic
{
    public class MineFlagController : BaseController
    {
        private const String FILENAME = "data.xml";
        private Mine[] Minefield;
        private int Rows;
        private int Columns;
        private int RemainingMines;
        private int Mines;
        private bool Saving = false;
        private Dictionary<PlayerNum, IPlayer> Players = new Dictionary<PlayerNum, IPlayer>();
        private PlayerNum CurrentPlayer;
        private IRules RulesEngine;

        // File watcher
        public Watcher FileWatcher { get; set; }

        // Constructor
        public MineFlagController()
        {
            RulesEngine = new GameRules(this);

            // If a game state file exists, use that!
            if (File.Exists(FILENAME))
            {
                // This will load the state into the game logic
                ResumeGameFromState();

                Console.WriteLine("Remaining mines: {RemainingMines}");

                // Announce the scores for each player
                foreach (KeyValuePair<PlayerNum, IPlayer> player in Players)
                {
                    OnScoreChanged(player.Value, player.Value.GetPlayerScore());
                }

                // Announce the current player's turn
                OnAnnounceTurn(CurrentPlayer);
            }
        }

        // Destructor
        ~MineFlagController()
        {
            Console.WriteLine("Dealloc of MineFlagController");
            Dispose();
        }

        public override void NewGame(int rows, int columns, int mines, bool addAiPlayer)
        {
            // The first we ALWAYS need to do is to clear any previous values
            Dispose();

            // Set the current player to the first player
            CurrentPlayer = PlayerNum.ONE;
            // Add at least one player
            Players.Add(CurrentPlayer, new RegularPlayer(0, CurrentPlayer));
            
            Rows = rows;
            Columns = columns;
            Mines = mines;
            RemainingMines = mines;

            // Create a watcher for keeping track on game updates from the state file
            if (FileWatcher == null)
            {
                FileWatcher = new Watcher(FILENAME, this);
                FileWatcher.Run();
            }
            
            // Add our PrintMinefield as an EventListener
            ResetMinefield += PrintMinefield;

            // Build the minefield
            BuildMinefield();

            // Should we instantiate an AI IPlayer? 
            if (addAiPlayer)
            {
                Players.Add(PlayerNum.TWO, new AIPlayer(this, rows, columns, 0, PlayerNum.TWO));
            }
            else
            {
                // Add a second regular player
                Players.Add(PlayerNum.TWO, new RegularPlayer(0, PlayerNum.TWO));
            }
        }

        public override void Dispose()
        {
            if (FileWatcher != null)
            {
                FileWatcher.Dispose();
                FileWatcher = null;
            }

            if (Players != null)
            {
                foreach (KeyValuePair<PlayerNum, IPlayer> pair in Players)
                {
                    // Call dispose on the objects to release event listeners and other variables not used anymore
                    pair.Value.Dispose();
                }
                Players.Clear();
            }
        }

        // This is not optimal, but we only support two users ATM
        public override void ChangeTurn()
        {
            if (CurrentPlayer == PlayerNum.ONE)
            {
                CurrentPlayer = PlayerNum.TWO;
            }
            else
            {
                CurrentPlayer = PlayerNum.ONE;
            }
        }

        /**
         * Opens a mine on the game field
         * 
         * Returns true upon successful open. Otherwise false
         */
        public override bool OpenMine(int index)
        {
            Mine mine = Minefield[index];

            // Do nothing if the mine is already opened
            if (mine.IsOpened())
                return false;

            IPlayer PlayerTemp = Players[CurrentPlayer];

            // Evaluate the mine
            bool successfulOpen = RulesEngine.Evaluate(ref mine, ref PlayerTemp);
            if (successfulOpen)
            {
                // Decrement the remaining mines
                RemainingMines--;
            }

            /* Notify everyone about the opened mine */
            OnMineOpened(mine);

            // The game is finished if there are no mines left, or if any player has a greter score than half of the available mines
            foreach (KeyValuePair<PlayerNum, IPlayer> player in Players)
            {
                if (RemainingMines == 0 || player.Value.GetPlayerScore() > Mines / 2)
                {
                    OnGameCompleted(PlayerTemp);
                    return true;
                }
            }
     
            // Save the state
            SaveState();

            // Notify everyone about the turn
            OnAnnounceTurn(CurrentPlayer);

            return true;
        }

        // This method will open the neighbouring mines to the selected mine
        public override void OpenNeighbouringMines(int index, IPlayer p)
        {
            Mine mine = Minefield[index];

            if (!mine.IsMine() && mine.GetNeighbours() == 0)
            {
                List<Mine> mines = GetNeighbouringMines(index, true);
                foreach (Mine m in mines)
                {
                    if (!m.IsOpened() && !m.IsMine())
                    {
                        // Open the mine
                        m.Open(p);

                        // Invoke the delegate call
                        OnMineOpened(m);

                        // Recursively open all neighbours
                        OpenNeighbouringMines(m.index, p);
                    }
                }
            }
        }

        public override void ResumeGameFromState()
        {
            try
            {
                Saving = true;

                // Get the state from state-handler class
                State state = StateHandler.importFromStorage(FILENAME);
                // Set all variables to their correct values
                Mines = state.Mines;
                Rows = state.Rows;
                Columns = state.Columns;
                Minefield = state.Minefield;
                CurrentPlayer = state.CurrentPlayer;
                RemainingMines = state.RemainingMines;
                Players = state.Players;

                // Notify all listeners about how the minefield looks like
                foreach (Mine mine in Minefield)
                {
                    if (mine.Opened)
                    {
                        OnMineOpened(mine);
                    }
                }

                Console.WriteLine("There are {state.RemainingMines} mines left to open");

                Saving = false;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
        }

        // State handling
        private void SaveState()
        {
            try
            {
                Saving = true;
                Console.WriteLine("Saving state...");

                // Use LINQ to dave the state og the mines
                Mine[] mines = (from mine in Minefield where mine.GetType() == typeof(Mine) select (Mine)mine).ToArray();

                // Create a "state" object that we can utilize for serialization and deserialization
                State saveGame = new State(mines, Rows, Columns, Mines, RemainingMines, CurrentPlayer, Players);

                // Save the state to external storage
                StateHandler.exportToStorage(saveGame, FILENAME);

                Saving = false;
            }
            catch (Exception e)
            {
                //MessageBox.Show("Error: " + e.Message);
            }
        }

        private void BuildMinefield()
        {
            // Create all the mines in the minefield
            Minefield = new Mine[Rows * Columns];
            for (int index = 0; index < Minefield.Length; ++index)
                Minefield[index] = new Mine((index / Columns), (index % Columns));

            // Set out some mines
            Random RandomGenerator = new Random();
            int AddedMines = 0;
            while (AddedMines < RemainingMines)
            {
                int index = RandomGenerator.Next(0, Minefield.Length - 1);
                Mine Mine = Minefield[index];

                // If the mine already is a mine, try again
                if (Mine.IsMine())
                    continue;

                Mine.SetAsMine(true);

                /* Tell the neighbours about the newly added mine */
                List<Mine> mines = GetNeighbouringMines(index, true);
                mines.ForEach(delegate (Mine m)
                {
                    m.IncreaseNeighbours();
                });

                ++AddedMines;
            }

            // Invoke the reset call if we have one defined
            OnResetMinefield();
        }

        private List<Mine> GetNeighbouringMines(int index, bool corners)
        {
            List<Mine> Mines = new List<Mine>();
            int y = GetRowForIndex(index);
            int x = GetColumnForIndex(index);

            // Get the mines above about the new mine
            {
                int row = y - 1;
                if (row >= 0)
                {
                    if (corners && (x - 1) >= 0) Mines.Add(Minefield[row * Rows + (x - 1)]);
                    if (corners && (x + 1) < Columns) Mines.Add(Minefield[row * Rows + (x + 1)]);
                    Mines.Add(Minefield[row * Rows + x]);
                }
            }

            // Get the one's on the sides
            {
                int row = y;
                if ((x - 1) >= 0) Mines.Add(Minefield[row * Rows + (x - 1)]);
                if ((x + 1) < Columns) Mines.Add(Minefield[row * Rows + (x + 1)]);
            }

            // Get the one's below
            {
                int row = (y + 1);
                if (row < Rows)
                {
                    if (corners && (x - 1) >= 0) Mines.Add(Minefield[row * Rows + (x - 1)]);
                    if (corners && (x + 1) < Columns) Mines.Add(Minefield[row * Rows + (x + 1)]);
                    Mines.Add(Minefield[row * Rows + x]);
                }
            }

            return Mines;
        }

        private int GetRowForIndex(int index)
        {
            return (index / Columns);
        }

        private int GetColumnForIndex(int index)
        {
            return index % Columns;
        }

        private void PrintMinefield()
        {
            for (int index = 0; index < Minefield.Length; ++index)
            {
                if ((index % Columns) == 0)
                    Console.Write("\n");

                Console.Write(Minefield[index].ToString());
            }
        }
    }
}
