using MineFlags.GenericTypes;
using MineFlags.PlayerType;

namespace MineFlags.RulesEngine
{
    /// <summary>
    /// IRules interface determines the available methods of the rules engine
    /// </summary>
    public interface IRules
    {
        bool Evaluate(ref Mine mine);
    }
}
