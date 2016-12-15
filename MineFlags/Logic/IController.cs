using MineFlags.PlayerType;

namespace MineFlags.Logic
{
    public interface IController
    {
        void NewGame(int rows, int columns, int mines, bool addAiPlayer);
        void Dispose();
        void ResumeGameFromState();
        //bool OpenMine(int index);
        void OpenNeighbouringMines(int index, IPlayer p);
        void ChangeTurn();
    }
}
