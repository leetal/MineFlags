using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MineFlags.GenericTypes;
using MineFlags.PlayerType;

namespace MineFlags.Logic
{
    public abstract class BaseController : IController
    {
        public abstract void NewGame(int rows, int columns, int mines, bool addAiPlayer);
        public abstract void Dispose();
        public abstract void ResumeGameFromState();
        public abstract bool OpenMine(int index);
        public abstract void OpenNeighbouringMines(int index, IPlayer p);
        public abstract void ChangeTurn();

        // Delegates + Events manage the data binding
        // Delegates
        public delegate void MineHandlerType(Mine m);
        public delegate void MinefieldHandlerType();
        public delegate void TurnHandlerType(PlayerNum playerNumber);
        public delegate void PlayerScoreChangedType(IPlayer player, int score);
        public delegate void GameCompletedType(IPlayer player);

        // Events
        public static event MineHandlerType MineOpened;
        public static event MinefieldHandlerType ResetMinefield;
        public static event TurnHandlerType AnnounceTurn;
        public static event PlayerScoreChangedType ScoreChanged;
        public static event GameCompletedType GameCompleted;

        public static void OnMineOpened(Mine m)
        {
            MineOpened?.Invoke(m);
        }
        public static void OnResetMinefield()
        {
            ResetMinefield?.Invoke();
        }
        public static void OnAnnounceTurn(PlayerNum playerNumber)
        {
            AnnounceTurn?.Invoke(playerNumber);
        }
        public static void OnScoreChanged(IPlayer player, int score)
        {
            ScoreChanged?.Invoke(player, score);
        }
        public static void OnGameCompleted(IPlayer player)
        {
            GameCompleted?.Invoke(player);
        }
    }
}
