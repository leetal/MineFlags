using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MineFlags
{
    public enum Player
    {
        ONE = 0,
        TWO = 1
    };

    public class MineFlagController
    {
        private const String FILENAME = "data.xml";
        private Mine[] _minefield;
        private int _rows;
        private int _columns;
        private int _remaining_mines;
        private int _mines;
        private bool _saving = false;
        private int[] _scores = new int[2] { 0, 0 };
        private Player _current_player_turn = Player.ONE;

        private AIPlayer _ai;

        public Watcher _watcher { get; set; }

        // Delegates
        public delegate void MinefieldHandler();
        public delegate void TurnHandler(Player player);
        public delegate void MineHandler(Mine m);
        public delegate void GameCompleted(Player player);
        public delegate void PlayerScoreChanged(Player player, int score);

        // Events
        public static event MineHandler onMineOpened;
        public static event MinefieldHandler onResetMinefield;
        public static event TurnHandler announceTurn;
        public static event PlayerScoreChanged onScoreChanged;
        public static event GameCompleted onGameCompleted;

        public MineFlagController() { }
        public MineFlagController(int rows, int columns, int mines, bool ai_player = true)
        {
            NewGame(rows, columns, mines, ai_player);
        }

        ~MineFlagController() 
        {
            Console.WriteLine("Dealloc of MineFlagController");
        }

        public void NewGame(int rows, int columns, int mines, bool ai_player = true)
        {
            _rows = rows;
            _columns = columns;
            _mines = mines;
            _remaining_mines = _mines;

            // Create a watcher for keeping track on game updates
            _watcher = new Watcher(FILENAME, this);
            _watcher.Run();

            /* Add our _printMinefield as an EventListener */
            onResetMinefield += _printMinefield;
            _buildMinefield();

            /* Should we instantiate an AI Player? */
            if (ai_player) {
                _ai = new AIPlayer(this, rows, columns);
            }

            // Announce the turn directly
            announceTurn(_current_player_turn);
        }

        public void Dispose()
        {
            _watcher.Dispose();
            _watcher = null;
            _ai.Dispose();
            _ai = null;
        }

        // State handling
        public void SaveState()
        {
            try
            {
                _saving = true;
                List<Mine> mines = (from mine in _minefield where mine.GetType() == typeof(Mine) select (Mine)mine).ToList();
                State saveGame = new State(mines, _rows, _columns, _mines, _remaining_mines, _scores, _current_player_turn, _ai);
                StateHandler.exportToStorage(saveGame, FILENAME);
                _saving = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public void ContinueGame()
        {
            try
            {
                // Get the state from statehandler
                State state = StateHandler.importFromStorage(FILENAME);
            }
            catch (Exception e)
            {
                MessageBox.Show("What" + e);
            }
        }

        public void openMine(int index) {
            Mine mine = _minefield[index];
            if (mine.isOpened())
                return;

            /* Open the mine and always change turns */
            mine.open(_current_player_turn);

            if (!mine.isMine() && mine.getNeighbours() == 0) {
                /* Reveal all neighbouring mines if the mine has an value of 0 */
                if (!mine.isOpened() && !mine.isMine()) {
                    mine.open(_current_player_turn);
                }

                _openNeighbouringMines(mine.index, _current_player_turn);
                _changeTurns();
            } else if (mine.isMine()) {
                /* Up the score of the one who took it */
                _scores[(int)_current_player_turn] += 1;
                _remaining_mines--;

                // Notify about any score change
                onScoreChanged(_current_player_turn, _scores[(int)_current_player_turn]);
            } else {
                _changeTurns();
            }

            /* Notify everyone about the opened mine */
            if (onMineOpened != null) {
                onMineOpened(mine);
            }

            if (_remaining_mines == 0 || _remaining_mines < (_mines/2))
            {
                onGameCompleted(_current_player_turn);
                return;
            }

            // Notify everyone about the turn
            announceTurn(_current_player_turn);
        }

        private void _buildMinefield()
        {
            // Create all the mines in the minefield
            _minefield = new Mine[_rows * _columns];
            for (int index = 0; index < _minefield.Length; ++index)
                _minefield[index] = new Mine((index / _columns), (index % _columns));

            // Set out some mines
            Random r = new Random();
            int added_mines = 0;
            while (added_mines < _remaining_mines) {
                int index = r.Next(0, _minefield.Length - 1);
                Mine mine = _minefield[index];

                /* If the mine already is a mine, try again */
                if (mine.isMine())
                    continue;

                mine.setAsMine(true);

                /* Tell the neighbours about the newly added mine */
                List<Mine> mines = _getNeighbouringMines(index, true);
                mines.ForEach(delegate(Mine m) {
                    m.increaseNeighbours();
                });

                ++added_mines;
            }

            if (onResetMinefield != null)
                onResetMinefield();
        } 

        //private void _updateAroundMine(int index)
        private List<Mine> _getNeighbouringMines(int index, bool corners)
        {
            List<Mine> mines = new List<Mine>();
            int y = _getRowForIndex(index);
            int x = _getColumnForIndex(index);

            // Get the mines above about the new mine
            {
                int row = y - 1;
                if (row >= 0) {
                    if (corners && (x - 1) >= 0) mines.Add(_minefield[row * _rows + (x - 1)]);
                    if (corners && (x + 1) < _columns) mines.Add(_minefield[row * _rows + (x + 1)]);
                    mines.Add(_minefield[row * _rows + x]);
                }
            }

            // Get the one's on the sides
            {
                int row = y;
                if ((x - 1) >= 0) mines.Add(_minefield[row * _rows + (x - 1)]);
                if ((x + 1) < _columns) mines.Add(_minefield[row * _rows + (x + 1)]);
            }

            // Get the one's below
            {
                int row = (y + 1);
                if (row < _rows)
                {
                    if (corners && (x - 1) >= 0) mines.Add(_minefield[row * _rows + (x - 1)]);
                    if (corners && (x + 1) < _columns) mines.Add(_minefield[row * _rows + (x + 1)]);
                    mines.Add(_minefield[row * _rows + x]);
                }
            }

            return mines;
        }

        private int _getRowForIndex(int index)
        {
            return (index / _columns);
        }

        private int _getColumnForIndex(int index)
        {
            return index % _columns;
        }

        private void _printMinefield()
        {
            for (int index = 0; index < _minefield.Length; ++index)
            {
                if ((index % _columns) == 0)
                    Console.Write("\n");

                Console.Write(_minefield[index].toString());
            }
        }

        private void _openNeighbouringMines(int index, Player p)
        {
            Mine mine = _minefield[index];

            if (!mine.isMine() && mine.getNeighbours() == 0) {
                List<Mine> mines = _getNeighbouringMines(index, true);
                foreach (Mine m in mines) {
                    if (!m.isOpened() && !m.isMine()) {
                        m.open(p);
                        onMineOpened(m);
                        _openNeighbouringMines(m.index, p);
                    }
                }
            }
        }

        private void _changeTurns()
        {
            if (_current_player_turn == Player.ONE)
                _current_player_turn = Player.TWO;
            else
                _current_player_turn = Player.ONE;
        }
    }
}
