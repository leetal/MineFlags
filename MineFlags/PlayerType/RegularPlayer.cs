using System;
using System.Xml.Linq;
using MineFlags.GenericTypes;

namespace MineFlags.PlayerType
{
    public class RegularPlayer : AbstractPlayer
    {

        public RegularPlayer() : base()
        {

        }

        public RegularPlayer(int score, PlayerNum num) : base(score, num) {}

        public override void HandleTurn(PlayerNum playerNumber) { }

        public override void OnMineOpened(PlayerNum playerNumber, Mine m, bool success)
        {
            Console.WriteLine("[Player] onMineOpened: ", PlayerNum);
        }

        public override string GetPlayerType()
        {
            return "regular";
        }
    }
}
