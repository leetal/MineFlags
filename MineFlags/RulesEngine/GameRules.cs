using System;
using MineFlags.GenericTypes;
using MineFlags.Logic;
using System.Collections.Generic;
using MineFlags.PlayerType;

namespace MineFlags.RulesEngine
{
    class GameRules : IRules
    {
        private IController Controller;
        public GameRules(IController controller)
        {
            Controller = controller;
        }

        /*
         * This will evaluate the current mine open
         *  
         * Returns true upon successful open (that gave the player a point). False otherwise
         * 
         */
        public bool Evaluate(ref Mine mine, ref IPlayer player)
        {
            // Open the mine
            mine.Open(player);

            if (!mine.IsMine() && mine.GetNeighbours() == 0)
            {
                // Reveal all neighbouring mines
                Controller.OpenNeighbouringMines(mine.index, player);

                // Change turn last
                Controller.ChangeTurn();
                return false;
            }
            else if (mine.IsMine())
            {
                /* Up the score of the one who took it */
                player.IncrementPlayerScore();

                // Notify about any score change
                BaseController.OnScoreChanged(player, player.GetPlayerScore());
                return true;
            }
            else
            {
                Controller.ChangeTurn();
                return false;
            }
        }
    }
}
