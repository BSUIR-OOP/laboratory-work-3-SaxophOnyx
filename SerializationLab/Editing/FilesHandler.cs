using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SerializationLab
{
    public class FilesHandler: IFilesHandler
    {
        public string FilePath { get; set; }


        public FilesHandler(string filePath)
        {
            FilePath = filePath;
        }


        string IFilesHandler.Read(string identifier)
        {
            string path = string.Concat(FilePath, "\\", identifier);
            string res = null;

            using (StreamReader reader = new StreamReader(path))
            {
                res = reader.ReadToEnd();
                return res;
            }
        }

        void IFilesHandler.Append(string data, string identifier)
        {
            string path = string.Concat(FilePath, "\\", identifier);
            if (File.Exists(path))
                throw new Exception("This file already exists");

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.Write(data);
            }
        }

        void IFilesHandler.Rewrite(string data, string identifier)
        {
            string path = string.Concat(FilePath, "\\", identifier);
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.Write(data);
            }
        }

        List<string> IFilesHandler.GetIdentifiers()
        {
            var files = Directory.GetFiles(FilePath).ToList();
            List<string> res = new List<string>();

            foreach(var file in files)
            {
                string filename = file.Remove(0, FilePath.Length + 1);
                res.Add(filename);
            }

            return res;
        }
    }
}
