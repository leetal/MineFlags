using System;
using System.IO;
using System.Xml.Serialization;

namespace MineFlags.Logic
{
    public class StateHandler
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

        public static State importFromStorage(String filename)
        {
            // Deserialize
            XmlSerializer serializer = new XmlSerializer(typeof(State));

            // Open the file stream
            State state;
            Stream reader = new FileStream(filename, FileMode.Open);
            state = (State)serializer.Deserialize(reader);

            // Close the file
            reader.Close();

            return state;
        }
    }
}
