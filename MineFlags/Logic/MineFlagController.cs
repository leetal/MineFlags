using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MineFlags.PlayerType;
using MineFlags.GenericTypes;
using MineFlags.RulesEngine;
using MineFlags.Storage;
using System.ComponentModel;
using MineFlags.Notification;
using MineFlags.Exceptions;

namespace MineFlags.Logic
{
    public class MineFlagController : IController
    {
        private int Rows;
        private int Columns;
        private int RemainingMines;
        private int Mines;
        private bool GameLocked = false;

        private Mine[] Minefield;
        private Dictionary<PlayerNum, IPlayer> Players = new Dictionary<PlayerNum, IPlayer>();
        private PlayerNum CurrentPlayer;

        private IRules RulesEngine;
        private IStateHandler StateHandler;

        // Constructor
        public MineFlagController(IRules rulesEngine, IStateHandler stateHandler)
        {
            RulesEngine = rulesEngine;
            StateHandler = stateHandler;

            // Add a guard so that we never get into a state where we cannot do everything
            if (RulesEngine == null || StateHandler == null)
                throw new InvalidDIException("The required engines have not been initialized");
        }

        public void NewGame(bool reset, int rows, int columns, int mines, bool addAiPlayer)
        {
            // The first we ALWAYS need to do is to clear any previous local states
            ResetGameState();

            if (reset)
            {
                StateHandler.DeleteStorageIfExists();
            }

            // Add controller EventListeners
            GameCenter.Instance.MinefieldBuiltEvent += PrintMinefield;
            GameCenter.Instance.OpenMineEvent += OpenMine;
            StorageCenter.Instance.FileChangeEvent += GameFromState;

            // If a game state file exists, use that!
            if (StateHandler.StorageExists())
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
            GameCenter.Instance.OnNewGame(Rows, Columns, 2);

            if (StateHandler.StorageExists())
            {
                // This will restore the state again
                ResumeGameFromState();
            }

            // Announce the current player's turn
            GameCenter.Instance.OnAnnounceTurn(CurrentPlayer);
        }

        public void Dispose()
        {
            // Make sure to remove all states that have been set and call Dispose() downwards
            ResetGameState();
            StateHandler.Dispose();
        }

        // This is not optimal, but we only support two users ATM
        public void ChangeTurn()
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

        private void ResetGameState()
        {
            if (Players != null)
            {
                foreach (KeyValuePair<PlayerNum, IPlayer> pair in Players)
                {
                    // Call dispose on the objects to release event listeners and other variables not used anymore
                    pair.Value.Dispose();
                }
                Players.Clear();
            }

            // Remove all event handlers
            GameCenter.Instance.MinefieldBuiltEvent -= PrintMinefield;
            GameCenter.Instance.OpenMineEvent -= OpenMine;
            StorageCenter.Instance.FileChangeEvent -= GameFromState;

            GameLocked = false;
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
                GameCenter.Instance.OnMineOpened(playerNumber, mine, false);
                return;
            }

            IPlayer PlayerTemp = Players[CurrentPlayer];

            // Open the mine
            mine.Open(PlayerTemp.GetPlayerNumber());

            try
            {
                // Evaluate the open of the mine...
                bool successfulOpen = RulesEngine.Evaluate(ref mine);
                if (successfulOpen)
                {
                    /* Up the score of the one who took it */
                    PlayerTemp.IncrementPlayerScore();

                    // Notify about any score change
                    GameCenter.Instance.OnScoreChanged(ref PlayerTemp, PlayerTemp.GetPlayerScore());

                    // Decrement the remaining mines
                    RemainingMines--;
                }
                else
                {
                    // Reveal all neighbouring mines
                    OpenNeighbouringMines(mine.index, PlayerTemp);

                    // Change turn last
                    ChangeTurn();
                }
            }
            catch (UnknownNeighboursException)
            {
                // If we have an unknown neighbours state, change turn since that means we can do so without disrupting the game
                ChangeTurn();
            }

            /* Notify everyone about the opened mine */
            GameCenter.Instance.OnMineOpened(playerNumber, mine, true);

            // The game is finished if there are no mines left, or if any player has a greter score than half of the available mines
            foreach (KeyValuePair<PlayerNum, IPlayer> player in Players)
            {
                if (RemainingMines == 0 || player.Value.GetPlayerScore() > Mines / 2)
                {
                    // Lock the controller
                    GameLocked = true;
                    // Signal that the game is over
                    GameCenter.Instance.OnGameCompleted(PlayerTemp);
                    // Delete the stored state file as well!
                    StateHandler.DeleteStorageIfExists();
                    return;
                }
            }

            // (A file can only be written by a process at a given time)
            // Save the state
            SaveState();

            // Notify everyone about the turn
            GameCenter.Instance.OnAnnounceTurn(CurrentPlayer);
        }

        // This method will open the neighbouring mines to the selected mine
        public void OpenNeighbouringMines(int index, IPlayer p)
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
                        GameCenter.Instance.OnMineOpened(p.GetPlayerNumber(), tempMine, true);

                        // Recursively open all neighbours
                        OpenNeighbouringMines(tempMine.index, p);
                    }
                }
            }
        }

        public void GameFromState()
        {
            GameLocked = true;

            // If a game state file exists, use that!
            if (StateHandler.StorageExists())
            {
                // Signal a reset of the game
                GameCenter.Instance.OnResetMinefield(GameHasAIPlayer());

                // This will load the state into the game logic
                FetchStoredState();

                // Notify of new game
                GameCenter.Instance.OnNewGame(Rows, Columns, 2);

                // Sleep for 10ms to allow AI to initialize
                System.Threading.Thread.Sleep(10);

                // This will restore the state again
                ResumeGameFromState();
            }
        }

        public void FetchStoredState()
        {
            try
            {
                // Get the state from state-handler class
                State state = StateHandler.ImportFromStorage();

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
            catch (StateException e)
            {
                // Will in 99 percent of the cases never come here. Only if someone modifies the data in a faulty manner.
                // Let's assume that to be an edge case...
                GameCenter.Instance.OnNewNotification(string.Format("{0}. Reason: {1}", e.Message, e.InnerException.Message));
            }
        }

        // State handling
        private void SaveState()
        {
            try
            {
                // Use LINQ to save the state of the mines
                Mine[] mines = (from mine in Minefield where mine.GetType() == typeof(Mine) select (Mine)mine).ToArray();
                // Create a "state" object that we can utilize for serialization and deserialization
                State saveGame = new State(mines, Rows, Columns, Mines, RemainingMines, CurrentPlayer, Players);
                // Save the state to external storage
                StateHandler.ExportToStorage(saveGame);
            }
            catch (StateException e)
            {
                // Will in 99 percent of the cases never come here. Only if someone modifies the data in a faulty manner.
                // Let's assume that to be an edge case...
                GameCenter.Instance.OnNewNotification(string.Format("{0}. Reason: {1}", e.Message, e.InnerException.Message));
            }
        }

        public void ResumeGameFromState()
        {
            // Notify all listeners about how the minefield looks like
            for (int i = 0; i < Minefield.Length; i++)
            {
                Mine tempMine = Minefield[i];
                if (tempMine.Opened)
                {
                    // It is not a player that opens this mine, but the game controller itself
                    GameCenter.Instance.OnMineOpened(PlayerNum.NONE, tempMine, true);
                }
            }

            // Announce the scores for each player
            foreach (KeyValuePair<PlayerNum, IPlayer> player in Players)
            {
                IPlayer playerTemp = player.Value;
                GameCenter.Instance.OnScoreChanged(ref playerTemp, player.Value.GetPlayerScore());
            }
        }

        private bool CurrentPlayerIsAI()
        {
            return Players[CurrentPlayer].GetPlayerType().Equals("ai");
        }

        private bool GameHasAIPlayer()
        {
            foreach (KeyValuePair<PlayerNum, IPlayer> player in Players)
            {
                if (player.Value.GetPlayerType().Equals("ai"))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This will build the minefield asynchronously in a background thread
        /// </summary>
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

            GameCenter.Instance.OnMinefieldBuilt();
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

        /// <summary>
        /// Will print the current minefield to stdout
        /// </summary>
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
