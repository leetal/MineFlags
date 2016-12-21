using System.Xml.Linq;
using MineFlags.GenericTypes;
using MineFlags.Notification;

namespace MineFlags.PlayerType
{
    public abstract class AbstractPlayer : IPlayer
    {
        protected abstract void OnNewGame(int rows, int columns, int numberOfPlayers);
        protected abstract void HandleTurn(PlayerNum playerNumber);
        protected abstract void OnMineOpened(PlayerNum playerNumber, Mine m, bool success);
        public abstract string GetPlayerType();

        public virtual XElement ObjectToX()
        {
            return new XElement("player",
               new XElement("score", Score),
               new XElement("playernumber", (int)PlayerNum),
               new XElement("type", GetPlayerType())
           );
        }

        public virtual void XToObject(XElement elem)
        {
            Score = (int)elem.Element("score");
            PlayerNum = (PlayerNum)((int)elem.Element("playernumber"));
        }

        public PlayerNum PlayerNum { get; set; }
        public int Score { get; set; }

        public AbstractPlayer()
        {
            // Subscribe to the events
            GameCenter.Instance.NewGameEvent += OnNewGame;
            GameCenter.Instance.ScoreChangedEvent += OnScoreChanged;
            GameCenter.Instance.MineOpenedEvent += OnMineOpened;
            GameCenter.Instance.AnnounceTurnEvent += HandleTurn;
        }

        public AbstractPlayer(int score, PlayerNum playerNum) : this()
        {
            PlayerNum = playerNum;
            Score = score;
        }

        public void Dispose()
        {
            // Unsubscribe from the events
            GameCenter.Instance.NewGameEvent -= OnNewGame;
            GameCenter.Instance.ScoreChangedEvent -= OnScoreChanged;
            GameCenter.Instance.MineOpenedEvent -= OnMineOpened;
            GameCenter.Instance.AnnounceTurnEvent -= HandleTurn;
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
