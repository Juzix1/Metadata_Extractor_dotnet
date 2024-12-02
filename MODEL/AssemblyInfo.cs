using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    public class AssemblyInfo {
        public string Name { get; set; }
        public List<TypeInfo> Types { get; set; } = new List<TypeInfo>();
    }

}
