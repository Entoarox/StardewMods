using System;
using System.IO;
using System.Collections.Generic;

namespace Entoarox.AdvancedLocationLoader
{
    public static class StructuredBinaryFile
    {
        public static Dictionary<string,int> Read(string file)
        {
            if (!File.Exists(file))
                return new Dictionary<string, int>();
            using (BinaryReader reader = new BinaryReader(new FileStream(file, FileMode.Open)))
            {
                string signature = new string(reader.ReadChars(3));
                if (!signature.Equals("SBF"))
                    throw new InvalidOperationException("File is not in SBF format: "+ file);
                byte version = reader.ReadByte();
                if (version != 0)
                    throw new InvalidOperationException("File is not in SBF0 format: " + file);
                if(!reader.ReadString().Equals("ALLCS"))
                    throw new InvalidOperationException("File is not in SBF0/ALLCS format: " + file);
                int count = reader.ReadInt32();
                Dictionary<string, int> data = new Dictionary<string, int>();
                for(int c=0;c<count;c++)
                    data.Add(reader.ReadString(), reader.ReadInt32());
                return data;
            }
        }
        public static void Write(string file, Dictionary<string,int> data)
        {
            if (data.Count == 0)
                return;
            using (BinaryWriter writer = new BinaryWriter(new FileStream(file, FileMode.Create)))
            {
                writer.Write(new char[] { 'S', 'B', 'F' });
                writer.Write((byte)0);
                writer.Write("ALLCS");
                foreach(KeyValuePair<string,int> index in data)
                {
                    writer.Write(index.Key);
                    writer.Write(index.Value);
                }
            }
        }
    }
}
