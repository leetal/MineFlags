using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MineFlags
{
    public enum MineButtonState
    {
        CLOSED,
        MINE,
        NUMBER,
        EMPTY
    };
    
    class MineButton : Button
    {
        private MineButtonState _state;
        private int _adjacentNeighbours;
        private Mine.Player _player;

        public MineButtonState state
        {
            get { return _state; }
            set { _state = value; }
        }

        public int adjacentNeighbours
        {
            get { return _adjacentNeighbours; }
            set { _adjacentNeighbours = value; }
        }

        public Mine.Player player
        {
            get { return _player; }
            set { _player = value; }
        }

        public override void NotifyDefault(bool value) {
            base.NotifyDefault(false);
        }

        public MineButton(MineButtonState state, int adjacentNeighbours, Mine.Player player) : base()
        {
            // Button constructor
            _state = state;

            switch (_state)
            {
                case MineButtonState.CLOSED:
                     this.BackColor = System.Drawing.Color.FromArgb(46, 204, 113); // Emerald light (hoover)
                    break;
                case MineButtonState.EMPTY:
                    this.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(255, 155, 89, 182); // Empty (Amethyst)
                    break;
                case MineButtonState.MINE:
                    if (player == Mine.Player.ONE)
                    {
                        this.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(255, 231, 76, 60); // One (ALZARIN)
                    }
                    else
                    {
                         this.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(255, 52, 73, 94); // Two (wet asphalt)
                    }
                    break;
                case MineButtonState.NUMBER:
                     this.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(255, 155, 89, 182); // Empty (Amethyst)
                     this.Text = adjacentNeighbours.ToString();
                    break;
                default:
                    break;
            };

            this.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(255, 255, 255, 255); 
            this.TabStop = false;
            this.Margin = new Padding(0, 0, 0, 0);
            this.FlatAppearance.BorderSize = 1;
            this.FlatStyle = FlatStyle.Flat;
        }
    }
}
