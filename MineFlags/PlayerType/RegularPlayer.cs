using MineFlags.GenericTypes;

namespace MineFlags.PlayerType
{
    public class RegularPlayer : AbstractPlayer
    {

        public RegularPlayer() : base() { }

        public RegularPlayer(int score, PlayerNum num) : base(score, num) { }

        protected override void OnNewGame(int rows, int columns, int numberOfPlayers) { }

        protected override void HandleTurn(PlayerNum playerNumber) { }

        protected override void OnMineOpened(PlayerNum playerNumber, Mine m, bool success) { }

        public override string GetPlayerType()
        {
            return "regular";
        }
    }
}
