using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace MineFlags
{
    internal class StateHandler
    {
        public static void exportToStorage(MineFlagController data, String filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            // Create a serializer
            XmlSerializer serializer = new XmlSerializer(typeof(MineFlagController));

            // Open a FileStream to write the data to
            FileStream stream = new FileStream(filename, FileMode.Create);
            serializer.Serialize(stream, data);

            // Close the file
            stream.Close();
        }

        public static void importFromStorage<T>(String filename)
        {
            // Deserialize
        }
    }
}
