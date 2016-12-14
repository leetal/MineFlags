using MineFlags.GenericTypes;
using MineFlags.Logic;

namespace MineFlags.PlayerType
{
    public abstract class AbstractPlayer : IPlayer
    {
        public abstract void HandleTurn(PlayerNum playerNumber);
        public abstract void OnMineOpened(Mine m);

        public PlayerNum PlayerNum { get; set; }
        public int Score { get; set; }

        public AbstractPlayer(int score, PlayerNum playerNum)
        {
            PlayerNum = playerNum;
            Score = score;

            // Subscribe to the events
            BaseController.ScoreChanged += OnScoreChanged;
            BaseController.MineOpened += OnMineOpened;
            BaseController.AnnounceTurn += HandleTurn;
        }

        public void Dispose()
        {
            // Unsubscribe from the events
            BaseController.ScoreChanged -= OnScoreChanged;
            BaseController.MineOpened -= OnMineOpened;
            BaseController.AnnounceTurn -= HandleTurn;
        }

        public PlayerNum GetPlayerNumber()
        {
            return PlayerNum;
        }

        public int GetPlayerScore()
        {
            return Score;
        }

        public void IncrementPlayerScore()
        {
            // Always just add "1" in score. Potential add-on: Use delta based on number of successful turns
            Score += 1;
        }

        private void OnScoreChanged(IPlayer player, int score)
        {
            if (PlayerNum == player.GetPlayerNumber())
            {
                // Set the score of we get notified about it
                Score = score;
            }
        }
    }
}
