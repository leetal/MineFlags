using MineFlags.GenericTypes;
using MineFlags.PlayerType;
using System;

namespace MineFlags.Notification
{
    public sealed class GameCenter
    {
        // Provide a multithreaded singleton for convenience when working with events
        private static volatile GameCenter instance;
        private static object syncRoot = new Object(); // Used as a mutex in this context

        private GameCenter() { }

        public static GameCenter Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new GameCenter();
                    }
                }

                return instance;
            }
        }

        // Delegates + Events manage the data bindings
        // Delegates
        public delegate void NewGameType(int rows, int columns, int numberOfPlayers);
        public delegate void MineHandlerType(PlayerNum playerNumber, Mine m, bool success);
        public delegate void MinefieldHandlerType();
        public delegate void NotificationHandlerType(string notification);
        public delegate void ResetMinefieldHandlerType(bool ai);
        public delegate void TurnHandlerType(PlayerNum playerNumber);
        public delegate void PlayerScoreChangedType(IPlayer player, int score);
        public delegate void GameCompletedType(IPlayer player);
        public delegate void MineOpenType(int index, PlayerNum playerNumber, bool manualInput);

        // Events, static for convenience
        public event NewGameType NewGameEvent;
        public event NotificationHandlerType NotificationEvent;
        public event MineHandlerType MineOpenedEvent;
        public event MinefieldHandlerType MinefieldBuiltEvent;
        public event ResetMinefieldHandlerType ResetMinefieldEvent;
        public event TurnHandlerType AnnounceTurnEvent;
        public event PlayerScoreChangedType ScoreChangedEvent;
        public event GameCompletedType GameCompletedEvent;
        public event MineOpenType OpenMineEvent;

        // These are here for convenience when invoking an event
        public void OnNewGame(int rows, int columns, int numberOfPlayers)
        {
            NewGameEvent?.Invoke(rows, columns, numberOfPlayers);
        }
        public void OnNewNotification(string message)
        {
            NotificationEvent?.Invoke(message);
        }
        public void OnMineOpened(PlayerNum playerNumber, Mine m, bool success)
        {
            MineOpenedEvent?.Invoke(playerNumber, m, success);
        }
        public void OnMinefieldBuilt()
        {
            MinefieldBuiltEvent?.Invoke();
        }
        public void OnResetMinefield(bool ai)
        {
            ResetMinefieldEvent?.Invoke(ai);
        }
        public void OnAnnounceTurn(PlayerNum playerNumber)
        {
            AnnounceTurnEvent?.Invoke(playerNumber);
        }
        public void OnScoreChanged(ref IPlayer player, int score)
        {
            ScoreChangedEvent?.Invoke(player, score);
        }
        public void OnGameCompleted(IPlayer player)
        {
            GameCompletedEvent?.Invoke(player);
        }
        public void OnOpenMine(int index, PlayerNum playerNumber, bool manualInput)
        {
            OpenMineEvent?.Invoke(index, playerNumber, manualInput);
        }
    }
}
