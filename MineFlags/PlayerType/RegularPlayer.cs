using System;
using MineFlags.GenericTypes;
using MineFlags.Logic;

namespace MineFlags.PlayerType
{
    public class RegularPlayer : AbstractPlayer
    {

        public RegularPlayer(int score, PlayerNum num) : base(score, num) {

            // Add the event handlers
            
        }

        public override void HandleTurn(PlayerNum playerNumber)
        {

        }

        public override void OnMineOpened(Mine m)
        {
            Console.WriteLine("[Player] onMineOpened: ", PlayerNum);
        }
    }
}
