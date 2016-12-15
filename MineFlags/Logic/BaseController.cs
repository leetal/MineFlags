using MineFlags.GenericTypes;
using MineFlags.PlayerType;

namespace MineFlags.Logic
{
    public abstract class BaseController : IController
    {
        public abstract void NewGame(int rows, int columns, int mines, bool addAiPlayer);
        public abstract void Dispose();
        public abstract void ResumeGameFromState();
        //public abstract bool OpenMine(int index);
        public abstract void OpenNeighbouringMines(int index, IPlayer p);
        public abstract void ChangeTurn();

        // Delegates + Events manage the data binding
        // Delegates
        public delegate void MineHandlerType(PlayerNum playerNumber, Mine m, bool success);
        public delegate void MinefieldHandlerType();
        public delegate void TurnHandlerType(PlayerNum playerNumber);
        public delegate void PlayerScoreChangedType(ref IPlayer player, int score);
        public delegate void GameCompletedType(ref IPlayer player);
        public delegate void MineOpenType(int index, PlayerNum playerNumber);

        // Events
        public static event MineHandlerType MineOpenedEvent;
        public static event MinefieldHandlerType ResetMinefieldEvent;
        public static event TurnHandlerType AnnounceTurnEvent;
        public static event PlayerScoreChangedType ScoreChangedEvent;
        public static event GameCompletedType GameCompletedEvent;
        public static event MineOpenType OpenMineEvent;

        public static void OnMineOpened(PlayerNum playerNumber, Mine m, bool success)
        {
            MineOpenedEvent?.Invoke(playerNumber, m, success);
        }
        public static void OnResetMinefield()
        {
            ResetMinefieldEvent?.Invoke();
        }
        public static void OnAnnounceTurn(PlayerNum playerNumber)
        {
            AnnounceTurnEvent?.Invoke(playerNumber);
        }
        public static void OnScoreChanged(ref IPlayer player, int score)
        {
            ScoreChangedEvent?.Invoke(ref player, score);
        }
        public static void OnGameCompleted(ref IPlayer player)
        {
            GameCompletedEvent?.Invoke(ref player);
        }
        public static void OnOpenMine(int index, PlayerNum playerNumber)
        {
            OpenMineEvent?.Invoke(index, playerNumber);
        }
    }
}
