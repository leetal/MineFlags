﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFlags
{
    class Mine
    {
        public enum Player
        {
            ONE,
            TWO
        };

        private bool _opened = false;
        private int _neighbours = 0;
        private bool _mine = false;
        private int _column;
        private int _row;

        public Mine(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public int row
        {
            get { return _row;  }
        }

        public int column
        {
            get { return _column; }
        }

        public int index() {
            return (_row * 1) + _column;
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
            return _neighbours;
        }

        public void open() {
            _opened = true;
        }

        public bool isOpened() {
            return _opened;
        }

        /* A mine is considered 'empty' if it has no neighbours,
         * isn't a mine itself and hasn't been opened. */
        public bool isEmpty()
        {
            return (_neighbours == 0) && !_mine && !_opened;
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
