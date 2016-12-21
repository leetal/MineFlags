using MineFlags.GenericTypes;

namespace MineFlags.Storage
{
    public interface IStateHandler
    {
        void Dispose();
        bool StorageExists();
        void DeleteStorageIfExists();
        void ExportToStorage(IStorageEntity state);
        State ImportFromStorage();
    }
}
