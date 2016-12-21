using System;
using System.Linq;
using MineFlags.GenericTypes;
using MineFlags.Logic;
using System.ComponentModel;
using MineFlags.Notification;

namespace MineFlags.PlayerType
{
    public class AIPlayer : AbstractPlayer
    {
        private bool[] OpenedMinefield { get; set; }
        private int Columns;
        private int Rows;

        public AIPlayer() : base() { }

        public AIPlayer(int score, PlayerNum num) : base(score, num) { }

        ~AIPlayer() { }

        protected override void OnNewGame(int rows, int columns, int numberOfPlayers)
        {
            Rows = rows;
            Columns = columns;

            // The AI must somehow remeber what tile has been opened or not...
            // All will default to false
            OpenedMinefield = new bool[Rows * Columns];
        }

        protected override void OnMineOpened(PlayerNum playerNumber, Mine m, bool success) {
            // IFF the AI tried to open a mine that was already taken, success == false
            // In such a case, the AI must try again!
            if (success)
            {
                OpenedMinefield[m.index] = true;
            }
            else if (playerNumber == PlayerNum)
            {
                // Try again!
                HandleTurn(PlayerNum);
            }
        }

        protected override void HandleTurn(PlayerNum playerNumber)
        {
            // If it is not the AI:s turn, just return...
            if (playerNumber != PlayerNum)
                return;

            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += new DoWorkEventHandler(
            delegate (object o, DoWorkEventArgs args)
            {
                // Simulate thinking...
                Random rand = new Random();

                System.Threading.Thread.Sleep(rand.Next(100, 1200));

                // This AI is really dumb and just randomly selects a mine and hopes for the best
                for (;;)
                {
                    int index = rand.Next(0, OpenedMinefield.Length - 1);

                    // We can't open that one, try again
                    if (OpenedMinefield[index])
                        continue;

                    // Signal the controller to open the mine (on the mainthread)
                    var mainThreadDelegate = new Action<object>(delegate (object param)
                    {
                        GameCenter.Instance.OnOpenMine(index, PlayerNum, false);
                    });
                    mainThreadDelegate.Invoke("test");

                    break;
                }
            });

            // Kick off the background worker to speed up the load of the view
            bw.RunWorkerAsync();
        }

        public override string GetPlayerType()
        {
            return "ai";
        }
    }
}
