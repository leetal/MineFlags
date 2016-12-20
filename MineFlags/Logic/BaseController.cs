﻿using System;
using MineFlags.GenericTypes;
using MineFlags.PlayerType;

namespace MineFlags.Logic
{
    public abstract class BaseController : IController
    {
        public abstract void NewGame(bool reset, int rows, int columns, int mines, bool addAiPlayer);
        public abstract void Dispose();
        public abstract void GameFromState();
        public abstract void OpenNeighbouringMines(int index, IPlayer p);
        public abstract void ChangeTurn();

        // Delegates + Events manage the data binding
        // Delegates
        public delegate void NewGameType(int rows, int columns, int numberOfPlayers);
        public delegate void MineHandlerType(PlayerNum playerNumber, Mine m, bool success);
        public delegate void MinefieldHandlerType();
        public delegate void ResetMinefieldHandlerType(bool ai);
        public delegate void TurnHandlerType(PlayerNum playerNumber);
        public delegate void PlayerScoreChangedType(IPlayer player, int score);
        public delegate void GameCompletedType(IPlayer player);
        public delegate void MineOpenType(int index, PlayerNum playerNumber, bool manualInput);

        // Events
        public static event NewGameType NewGameEvent;
        public static event MineHandlerType MineOpenedEvent;
        public static event MinefieldHandlerType MinefieldBuiltEvent;
        public static event ResetMinefieldHandlerType ResetMinefieldEvent;
        public static event TurnHandlerType AnnounceTurnEvent;
        public static event PlayerScoreChangedType ScoreChangedEvent;
        public static event GameCompletedType GameCompletedEvent;
        public static event MineOpenType OpenMineEvent;

        public static void OnNewGame(int rows, int columns, int numberOfPlayers)
        {
            NewGameEvent?.Invoke(rows, columns, numberOfPlayers);
        }
        public static void OnMineOpened(PlayerNum playerNumber, Mine m, bool success)
        {
            MineOpenedEvent?.Invoke(playerNumber, m, success);
        }
        public static void OnMinefieldBuilt()
        {
            MinefieldBuiltEvent?.Invoke();
        }
        public static void OnResetMinefield(bool ai)
        {
            ResetMinefieldEvent?.Invoke(ai);
        }
        public static void OnAnnounceTurn(PlayerNum playerNumber)
        {
            AnnounceTurnEvent?.Invoke(playerNumber);
        }
        public static void OnScoreChanged(ref IPlayer player, int score)
        {
            ScoreChangedEvent?.Invoke(player, score);
        }
        public static void OnGameCompleted(IPlayer player)
        {
            GameCompletedEvent?.Invoke(player);
        }
        public static void OnOpenMine(int index, PlayerNum playerNumber, bool manualInput)
        {
            OpenMineEvent?.Invoke(index, playerNumber, manualInput);
        }
    }
}
