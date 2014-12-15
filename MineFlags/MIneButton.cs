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

        public MineButtonState state
        {
            get { return _state; }
            set { this._state = value; }
        }

        public override void NotifyDefault(bool value) {
            base.NotifyDefault(false);
        }

        public MineButton(MineButtonState state) : base()
        {
            // Button constructor
            _state = state;

            this.BackColor = System.Drawing.Color.FromArgb(46, 204, 113); // Emerald light (hoover)

            this.TabStop = false;
            this.Margin = new Padding(0, 0, 0, 0);
            this.FlatAppearance.BorderSize = 1;
            this.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(255, 255, 255, 255); //Transparent borders
            this.FlatStyle = FlatStyle.Flat;

            
            //this.BackColor = System.Drawing.Color.FromArgb(1, 46, 204, 113); // Emerald light (hoover)
            //this.BackColor = System.Drawing.Color.FromArgb(1, 46, 204, 113); // Emerald light (hoover)
            //this.BackColor = System.Drawing.Color.FromArgb(1, 46, 204, 113); // Emerald light (hoover)
        }
    }
}
