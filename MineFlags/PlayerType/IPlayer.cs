using MineFlags.GenericTypes;

namespace MineFlags.PlayerType
{
    // This is here for now, since this game by design only supports two players
    public enum PlayerNum
    {
        NONE = 0,
        ONE = 1,
        TWO = 2
    };

    public interface IPlayer
    {
        int GetPlayerScore();
        void IncrementPlayerScore();
        PlayerNum GetPlayerNumber();
        void Dispose();
    }
}
