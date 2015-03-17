using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFlags
{
    [Serializable]
    public class Mine
    {
        public bool _opened { get; set; }
        public int _neighbours { get; set; }
        public bool _mine { get; set; }
        public int _column { get; set; }
        public int _row { get; set; }
        public Player _opened_by { get; set; }

        public Mine(int row, int column)
        {
            _row = row;
            _column = column;
            _opened = false;
            _neighbours = 0;
            _mine = false;
        }

        public int row
        {
            get { return _row; }
        }

        public Player opened_by
        {
            get { return _opened_by; }
        }

        public int column
        {
            get { return _column; }
        }

        public int index
        {
            get { return (_row * MineField.ROWS) + _column; }
        }

        public bool isMine()
        {
            return _mine;
        }

        public void setAsMine(bool value)
        {
            _mine = value;
        }

        public void increaseNeighbours()
        {
            _neighbours++;
        }

        public int getNeighbours()
        {
            if (isMine())
                return 0;

            return _neighbours;
        }

        public void open(Player p)
        {
            _opened = true;
            _opened_by = p;
        }

        public bool isOpened()
        {
            return _opened;
        }

        /* A mine is considered 'empty' if it has no neighbours,
         * isn't a mine itself and hasn't been opened. */
        public bool isEmpty()
        {
            return (getNeighbours() >= 0) && !_mine;
        }

        public string toString()
        {
            if (_mine)
                return "X";
            else if (_opened)
                return "O";
            else
                return _neighbours > 0 ? _neighbours.ToString() : " ";
        }
    }
}
