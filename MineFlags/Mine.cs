using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFlags
{
    class Mine
    {
        private bool _opened = false;
        private int _neighbours = 0;
        private bool _mine = false;

        public Mine()
        { /* Does nothing */ }

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

        public string toString()
        {
            if (_mine)
                return "X";
            else
                return _neighbours.ToString();
        }
    }
}
