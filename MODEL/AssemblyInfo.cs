using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary {
    public class AssemblyInfo {
        public string Name { get; set; }
        public List<TypeInfo> Types { get; set; } = new List<TypeInfo>();

        public override string ToString() {
            StringBuilder sb = new();
            sb.AppendLine($"\nAssembly: {Name}");

            foreach (var type in Types) {
                sb.AppendLine($"Type: {type.TypeName}");

                foreach (var method in type.Methods) {
                    sb.AppendLine($"  Method: {method.MethodName}");
                }
            }

            return sb.ToString();
        }

    }
}
