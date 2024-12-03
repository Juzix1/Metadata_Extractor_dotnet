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


        public Task LogFinishAsync();

        public Task LogStartXmlSave();
        public Task LogErrorXmlSave(string message);


    }
}
