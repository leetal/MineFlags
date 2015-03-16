using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MineFlags
{
    class MineButton : Button
    {
        private int _adjacentNeighbours;
        private Player _player;

        public int adjacentNeighbours
        {
            get { return _adjacentNeighbours; }
            set { 
                _adjacentNeighbours = value;
                this.BackColor = System.Drawing.Color.FromArgb(255, 210, 210, 210); // Taken (Gray)
                if(_adjacentNeighbours > 0)
                   this.Text = adjacentNeighbours.ToString();

            }
        }

        public Player player
        {
            get { return _player; }
            set { 
                _player = value;
                if (_player == Player.ONE) {
                    this.BackColor = System.Drawing.Color.FromArgb(255, 231, 76, 60); // One (ALZARIN)
                } else {
                    this.BackColor = System.Drawing.Color.FromArgb(255, 52, 73, 94); // Two (wet asphalt)
                }
            }
        }

        public override void NotifyDefault(bool value) {
            base.NotifyDefault(false);
        }

        public MineButton() : base()
        { 
            // Button constructor
            this.BackColor = System.Drawing.Color.FromArgb(46, 204, 113); // Emerald light (hoover)
            this.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(255, 255, 255, 255); 
            this.TabStop = false;
            this.Margin = new Padding(0, 0, 0, 0);
            this.FlatAppearance.BorderSize = 1;
            this.FlatStyle = FlatStyle.Flat;
        }
    }
}
