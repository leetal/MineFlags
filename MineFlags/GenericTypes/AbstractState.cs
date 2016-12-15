using MineFlags.Storage;
using System.Xml.Linq;

namespace MineFlags.GenericTypes
{
    public abstract class AbstractState : IStorageEntity
    {
        public static T CreateInstance<T>(XElement elem) where T : IStorageEntity, new()
        {
            // Instanciate the object
            T state = new T();
            state.XToObject(elem);
            return state;
        }

        public abstract XElement ObjectToX();
        public abstract void XToObject(XElement elem);
    }
}
