using CoreLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODEL.Plugins {
    public interface IMetadataPlugin { 
        string Name { get; }
        Task<object> Read(string sourcePath);
        Task<object> ReadStream(Stream stream);
        AssemblyInfo GetAssemblyInfo();
    }
}
