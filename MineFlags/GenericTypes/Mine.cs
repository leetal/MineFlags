using System;
using System.Xml.Linq;
using MineFlags.PlayerType;
using MineFlags.Storage;

namespace MineFlags.GenericTypes
{
    public class Mine : IStorageEntity
    {
        public bool Opened { get; set; }
        public int Neighbours { get; set; }
        public bool IsRealMine { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public PlayerNum OpenedBy { get; set; }

        public Mine() { }

        public Mine(int row, int column)
        {
            Row = row;
            Column = column;
            Opened = false;
            Neighbours = 0;
            IsRealMine = false;
            OpenedBy = PlayerNum.NONE;
        }

        public int row
        {
            get { return Row; }
        }

        public PlayerNum opened_by
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

        public void Open(PlayerNum p)
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

        public XElement ObjectToX()
        {
            return new XElement("mine",
                new XElement("opened", Opened),
                new XElement("neighbours", Neighbours),
                new XElement("isrealmine", IsRealMine),
                new XElement("column", Column),
                new XElement("row", Row),
                new XElement("openedby", (int)OpenedBy)
            );
        }

        public void XToObject(XElement elem)
        {
            Opened = (bool)elem.Element("opened");
            Neighbours = (int)elem.Element("neighbours");
            IsRealMine = (bool)elem.Element("isrealmine");
            Column = (int)elem.Element("column");
            Row = (int)elem.Element("row");
            OpenedBy = (PlayerNum)((int)elem.Element("openedby"));
        }
    }
}
