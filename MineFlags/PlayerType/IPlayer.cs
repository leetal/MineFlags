using MineFlags.GenericTypes;
using MineFlags.Storage;

namespace MineFlags.PlayerType
{
    // This is here for now, since this game by design only supports two players
    public enum PlayerNum
    {
        NONE = 0,
        ONE = 1,
        TWO = 2
    };

    public interface IPlayer : IStorageEntity
    {
        int GetPlayerScore();
        string GetPlayerType();
        void IncrementPlayerScore();
        PlayerNum GetPlayerNumber();
        void Dispose();
    }
}
