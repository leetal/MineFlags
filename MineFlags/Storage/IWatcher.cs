using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFlags.Storage
{
    public interface IWatcher
    {
        void Run();
        void Dispose();
        void Pause();
        void Resume();
    }
}
