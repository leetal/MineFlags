using System.Xml.Linq;

namespace MineFlags.Storage
{
    public interface IStorageEntity
    {
        XElement ObjectToX();
        void XToObject(XElement elem);
    }
}
