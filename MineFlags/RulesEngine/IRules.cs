using MineFlags.GenericTypes;
using MineFlags.PlayerType;

namespace MineFlags.RulesEngine
{
    interface IRules
    {
        bool Evaluate(ref Mine mine, ref IPlayer player);
    }
}
