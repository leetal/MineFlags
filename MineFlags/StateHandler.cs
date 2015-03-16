using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MineFlags
{
    internal class StateHandler
    {
        public static void exportToStorage(object data, String filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            // Serialize the data

            String serialized_data = "";
            File.WriteAllText(filename, serialized_data);
        }

        public static void importFromStorage<T>(String filename)
        {
            // Deserialize
        }
    }
}
