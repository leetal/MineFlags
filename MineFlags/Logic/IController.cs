using MineFlags.PlayerType;

namespace MineFlags.Logic
{
    public interface IController
    {
        void NewGame(bool reset, int rows, int columns, int mines, bool addAiPlayer);
        void Dispose();
        void GameFromState();
        void OpenNeighbouringMines(int index, IPlayer p);
        void ChangeTurn();
    }
}
