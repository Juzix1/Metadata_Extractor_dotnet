using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingLibrary {
    public interface ILogger {
        public Task LogStartAsync();
        public Task LogFoundAsync(string file);
        public Task LogErrorAsync();
        public Task LogStartReadAsync();

        public Task LogReadType(string message);
        public Task LogReadMethod(string message);
        public Task LogFinishAsync();

        public Task LogStartXmlSave();
        public Task LogErrorXmlSave(string message);
        public Task LogXmlClassSaved(string message);
        public Task LogXmlMethodSaved(string message);

    }
}
