using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODEL.Plugins {
    public interface IMetadataPlugin { 
        string Name { get; }
        Task<object> Read(string sourcePath);
    }
}
