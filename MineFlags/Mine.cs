﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFlags
{
    public class Mine
    {
        private bool _opened = false;
        private int _neighbours = 0;
        private bool _mine = false;
        private int _column;
        private int _row;
        private Player _opened_by;

        public Mine(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public int row
        {
            get { return _row;  }
        }

        public Player opened_by
        {
            get { return _opened_by;  }
        }

        public int column
        {
            get { return _column; }
        }

        public int index {
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

        public void increaseNeighbours() {
            _neighbours++;
        }

        public int getNeighbours() {
            if (isMine())
                return 0;

            return _neighbours;
        }

        public void open(Player p) {
            _opened = true;
            _opened_by = p;
        }

        public bool isOpened() {
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
