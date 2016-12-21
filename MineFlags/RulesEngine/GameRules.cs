using MineFlags.Exceptions;
using MineFlags.GenericTypes;
using MineFlags.Logic;
using MineFlags.Notification;
using MineFlags.PlayerType;

namespace MineFlags.RulesEngine
{
    class GameRules : IRules
    {
        /// <summary>
        /// Instantiates a new GameRules engine that evaluates each move
        /// </summary>
        public GameRules() { }

        /// <summary>
        /// This will evaluate the current mine open
        /// </summary>
        /// <param name="mine"></param>
        /// <param name="player"></param>
        /// <returns>true upon successful open (that gave the player a point). False otherwise</returns>
        public bool Evaluate(ref Mine mine)
        {
            if (!mine.IsMine() && mine.GetNeighbours() == 0)
            {
                return false;
            }
            else if (mine.IsMine())
            {
                return true;
            }
            throw new UnknownNeighboursException();
        }
    }
}
