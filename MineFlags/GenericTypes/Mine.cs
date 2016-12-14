using System;
using MineFlags.PlayerType;

namespace MineFlags.GenericTypes
{
    [Serializable]
    public class Mine
    {
        public bool Opened { get; set; }
        public int Neighbours { get; set; }
        public bool IsRealMine { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public IPlayer OpenedBy { get; set; }

        public Mine() { }

        public Mine(int row, int column)
        {
            Row = row;
            Column = column;
            Opened = false;
            Neighbours = 0;
            IsRealMine = false;
            OpenedBy = null;
        }

        public int row
        {
            get { return Row; }
        }

        public IPlayer opened_by
        {
            get { return OpenedBy; }
        }

        public int column
        {
            get { return Column; }
        }

        public int index
        {
            get { return (Row * MineField.ROWS) + Column; }
        }

        public bool IsMine()
        {
            return IsRealMine;
        }

        public void SetAsMine(bool value)
        {
            IsRealMine = value;
        }

        public void IncreaseNeighbours()
        {
            Neighbours++;
        }

        public int GetNeighbours()
        {
            if (IsMine())
                return 0;

            return Neighbours;
        }

        public void Open(IPlayer p)
        {
            Opened = true;
            OpenedBy = p;
        }

        public bool IsOpened()
        {
            return Opened;
        }

        /** 
         * A mine is considered 'empty' if it has no neighbours,
         * isn't a mine itself and hasn't been opened. 
         */
        public bool IsEmpty()
        {
            return (GetNeighbours() >= 0) && !IsRealMine;
        }

        public override string ToString()
        {
            if (IsRealMine)
                return "X";
            else if (Opened)
                return "O";
            else
                return Neighbours > 0 ? Neighbours.ToString() : " ";
        }
    }
}
