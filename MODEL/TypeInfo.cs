using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary {
    public class TypeInfo {
        public string TypeName { get; set; }
        public List<MethodInfo> Methods { get; set; } = new List<MethodInfo>();
    }
}
