using System;
using System.Linq;
using MineFlags.GenericTypes;
using MineFlags.Logic;

namespace MineFlags.PlayerType
{
    [Serializable]
    public class AIPlayer : AbstractPlayer
    {
        private IController Controller;
        public bool[] OpenedMinefield { get; set; }

        public AIPlayer(IController controller, int rows, int columns, int score, PlayerNum num) : base(score, num)
        {
            /* Keep the reference to the controller */
            Controller = controller;

            // The AI must somehow remeber what tile has been opened or not..
            OpenedMinefield = Enumerable.Repeat(false, MineField.ROWS * MineField.COLUMNS).ToArray();
        }

        ~AIPlayer()
        {
            Controller = null;
        }

        public override void OnMineOpened(Mine m) {
            Console.WriteLine("[AIPlayer] onMineOpened");
            OpenedMinefield[m.index] = true;
        }

        public override void HandleTurn(PlayerNum playerNumber)
        {
            // Player 1 is always the "human" player if AI is playing the game
            if (playerNumber == PlayerNum.ONE)
                return;

            // This AI is really dumb and just randomly selects a mine and hopes for the best
            Random r = new Random();
            for (;;)
            {
                int index = r.Next(0, OpenedMinefield.Length - 1);

                // We can't open that one, try again
                if (OpenedMinefield[index])
                    continue;

                // Call the controller to open the mine
                if (!Controller.OpenMine(index))
                {
                    // If the AI did pick an already taken mine, try again
                    continue;
                }

                break;
            }
        }
    }
}
