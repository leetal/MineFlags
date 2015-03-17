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
        public static void exportToStorage(State state, String filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            // Create a serializer
            XmlSerializer serializer = new XmlSerializer(typeof(State));

            // Open a FileStream to write the data to
            TextWriter writer = new StreamWriter(filename);
            serializer.Serialize(writer, state);

            // Close the file
            writer.Close();
        }

        public static void importFromStorage<T>(String filename)
        {
            // Deserialize
        }
    }
}
