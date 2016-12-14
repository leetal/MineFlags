using MineFlags.GenericTypes;
using MineFlags.PlayerType;
using System.Collections.Generic;

namespace MineFlags.RulesEngine
{
    interface IRules
    {
        bool Evaluate(ref Mine mine, ref IPlayer player);
    }
}
