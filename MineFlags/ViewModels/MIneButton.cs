using System.Windows.Forms;
using MineFlags.PlayerType;

namespace MineFlags
{
    class MineButton : Button
    {
        private int AdjacentNeighbours;
        private PlayerNum IntPlayerNumber;
        public int adjacentNeighbours
        {
            get { return AdjacentNeighbours; }
            set { 
                AdjacentNeighbours = value;
                BackColor = System.Drawing.Color.FromArgb(255, 210, 210, 210); // Taken (Gray)
                if(AdjacentNeighbours > 0)
                    Text = adjacentNeighbours.ToString();

            }
        }
        
        // The player sets the taken color of the mine(button)
        public PlayerNum PlayerNumber
        {
            get { return IntPlayerNumber; }
            set {
                IntPlayerNumber = value;
                // Not optimal, but we only support two players ATM
                if (IntPlayerNumber == PlayerNum.ONE) {
                    BackColor = System.Drawing.Color.FromArgb(255, 231, 76, 60); // One (ALZARIN)
                } else {
                    BackColor = System.Drawing.Color.FromArgb(255, 52, 73, 94); // Two (wet asphalt)
                }
            }
        }

        // DO NOT INVOKE THE DEFAULT ACTION OF THE BUTTON!
        public override void NotifyDefault(bool value) {
            base.NotifyDefault(false);
        }

        public MineButton() : base()
        { 
            // Button constructor
            BackColor = System.Drawing.Color.FromArgb(46, 204, 113); // Emerald light (hoover)
            FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(255, 255, 255, 255); 
            TabStop = false;
            Margin = new Padding(0, 0, 0, 0);
            FlatAppearance.BorderSize = 1;
            FlatStyle = FlatStyle.Flat;
        }
    }
}
