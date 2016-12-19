using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MineFlags.PlayerType;
using MineFlags.GenericTypes;
using MineFlags.RulesEngine;
using MineFlags.Storage;

namespace MineFlags.Logic
{
    public class MineFlagController : BaseController
    {
        private const string FILENAME = "data.xml";
        private Mine[] Minefield;
        private int Rows;
        private int Columns;
        private int RemainingMines;
        private int Mines;
        private Dictionary<PlayerNum, IPlayer> Players = new Dictionary<PlayerNum, IPlayer>();
        private PlayerNum CurrentPlayer;
        private IRules RulesEngine;
        private bool GameLocked = false;

        // File watcher
        private IWatcher FileWatcher { get; set; }

        // Constructor
        public MineFlagController()
        {
            RulesEngine = new GameRules(this);
        }

        // Destructor
        ~MineFlagController()
        {
            Console.WriteLine("Dealloc of MineFlagController");
            Dispose();
        }

        public override void NewGame(bool reset, int rows, int columns, int mines, bool addAiPlayer)
        {
            // The first we ALWAYS need to do is to clear any previous local states
            Dispose();

            if (reset)
            {
                StateHandler.DeleteStorageIfExists(FILENAME);
            }

            // Create a watcher for keeping track on game updates from the state file
            if (FileWatcher == null)
            {
                FileWatcher = new Watcher(FILENAME, this);
                FileWatcher.Run();
            }

            // Add controller EventListeners
            ResetMinefieldEvent += PrintMinefield;
            OpenMineEvent += OpenMine;

            // If a game state file exists, use that!
            if (File.Exists(FILENAME))
            {
                // This will load the state into the game logic
                FetchStoredState();
            }
            else
            {
                // Set the current player to the first player
                CurrentPlayer = PlayerNum.ONE;
                // Add at least one player
                Players.Add(CurrentPlayer, new RegularPlayer(0, CurrentPlayer));

                Rows = rows;
                Columns = columns;
                Mines = mines;
                RemainingMines = mines;

                // Build the minefield
                BuildMinefield();

                // Should we instantiate an AI IPlayer? 
                if (addAiPlayer)
                {
                    Players.Add(PlayerNum.TWO, new AIPlayer(0, PlayerNum.TWO));
                }
                else
                {
                    // Add a second regular player
                    Players.Add(PlayerNum.TWO, new RegularPlayer(0, PlayerNum.TWO));
                }
            }

            // Notify that we have started a new game
            // NOTE: The "2" players is hardcoded for now due to future addon
            OnNewGame(Rows, Columns, 2);

            if (File.Exists(FILENAME))
            {
                // This will restore the state again
                ResumeGameFromState();
            }
            
            // Announce the current player's turn
            OnAnnounceTurn(CurrentPlayer);
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

            // Delete the storage file after each run
            //StateHandler.DeleteStorageIfExists(FILENAME);

            // Remove all event handlers
            ResetMinefieldEvent -= PrintMinefield;
            OpenMineEvent -= OpenMine;

            GameLocked = false;
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
         */
        private void OpenMine(int index, PlayerNum playerNumber, bool manualInput)
        {
            // Prevent the user from opening an AI´s tiles
            if (CurrentPlayerIsAI() && manualInput)
            {
                Console.WriteLine(string.Format("[Player {0}] Cheating is baaaad!", playerNumber));
                return;
            }
            else if (GameLocked)
            {
                // Prevent any actions if the state is "locked"!
                return;
            }

            Mine mine = Minefield[index];

            // Do nothing if the mine is already opened
            if (mine.IsOpened())
            {
                OnMineOpened(playerNumber, mine, false);
                return;
            }

            IPlayer PlayerTemp = Players[CurrentPlayer];

            // Evaluate the mine
            bool successfulOpen = RulesEngine.Evaluate(ref mine, ref PlayerTemp);
            if (successfulOpen)
            {
                // Decrement the remaining mines
                RemainingMines--;
            }

            /* Notify everyone about the opened mine */
            OnMineOpened(playerNumber, mine, true);

            // The game is finished if there are no mines left, or if any player has a greter score than half of the available mines
            foreach (KeyValuePair<PlayerNum, IPlayer> player in Players)
            {
                if (RemainingMines == 0 || player.Value.GetPlayerScore() > Mines / 2)
                {
                    // Lock the controller
                    GameLocked = true;
                    // Signal that the game is over
                    OnGameCompleted(ref PlayerTemp);
                    // Delete the stored state file as well!
                    StateHandler.DeleteStorageIfExists(FILENAME);
                    return;
                }
            }
     
            // Save the state
            SaveState();

            // Notify everyone about the turn
            OnAnnounceTurn(CurrentPlayer);
        }

        // This method will open the neighbouring mines to the selected mine
        public override void OpenNeighbouringMines(int index, IPlayer p)
        {
            Mine mine = Minefield[index];

            if (!mine.IsMine() && mine.GetNeighbours() == 0)
            {
                List<Mine> mines = GetNeighbouringMines(index, true);
                for (int i = 0; i < mines.Count; i++)
                {
                    Mine tempMine = mines[i];
                    if (!tempMine.IsOpened() && !tempMine.IsMine())
                    {
                        // Open the mine
                        tempMine.Open(p.GetPlayerNumber());

                        // Invoke the delegate call
                        OnMineOpened(p.GetPlayerNumber(), tempMine, true);

                        // Recursively open all neighbours
                        OpenNeighbouringMines(tempMine.index, p);
                    }
                }
            }
        }

        public override void FetchStoredState()
        {
            try
            {
                // Get the state from state-handler class
                State state = StateHandler.ImportFromStorage(FILENAME);

                // Set all variables to their correct values
                Mines = state.Mines;
                Rows = state.Rows;
                Columns = state.Columns;
                Minefield = state.Minefield;
                CurrentPlayer = state.CurrentPlayer;
                RemainingMines = state.RemainingMines;
                Players = state.Players;

                Console.WriteLine(string.Format("Mines in total: {0}", Mines));
                Console.WriteLine(string.Format("Remaining mines: {0}", RemainingMines));
            }
            catch (Exception e)
            {
                MessageBox.Show("Fatal: " + e.Message);
            }
        }

        public override void ResumeGameFromState()
        {
            try
            {
                // Notify all listeners about how the minefield looks like
                for (int i = 0; i < Minefield.Length; i++)
                {
                    Mine tempMine = Minefield[i];
                    if (tempMine.Opened)
                    {
                        // It is not a player that opens this mine, but the game controller itself
                        OnMineOpened(PlayerNum.NONE, tempMine, true);
                    }
                }

                // Announce the scores for each player
                foreach (KeyValuePair<PlayerNum, IPlayer> player in Players)
                {
                    IPlayer playerTemp = player.Value;
                    OnScoreChanged(ref playerTemp, player.Value.GetPlayerScore());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Fatal: " + e.Message);
            }
        }

        private bool CurrentPlayerIsAI()
        {
            return Players[CurrentPlayer].GetPlayerType().Equals("ai");
        }

        // State handling
        private void SaveState()
        {
            // We need to pause the file watcher since this change otherwise will trigger an event
            FileWatcher.Pause();
            try
            {
                Console.WriteLine("Saving state...");

                // Use LINQ to dave the state og the mines
                Mine[] mines = (from mine in Minefield where mine.GetType() == typeof(Mine) select (Mine)mine).ToArray();

                // Create a "state" object that we can utilize for serialization and deserialization
                State saveGame = new State(mines, Rows, Columns, Mines, RemainingMines, CurrentPlayer, Players);

                // Save the state to external storage
                StateHandler.ExportToStorage(saveGame, FILENAME);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            FileWatcher.Resume();
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

                // If the mine already is a true mine, try again
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
                // Newline
                if ((index % Columns) == 0)
                {
                    Console.Write("\n");
                }
                    
                Console.Write(Minefield[index].ToString());
            }
        }
    }
}
