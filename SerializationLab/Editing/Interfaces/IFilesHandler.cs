﻿using System.Collections.Generic;

namespace SerializationLab
{
    public interface IFilesHandler
    {
        string Read(string identifier);

        void Append(string data, string identifier);

        void Rewrite(string data, string identifier);

        List<string> GetIdentifiers();
    }
}
