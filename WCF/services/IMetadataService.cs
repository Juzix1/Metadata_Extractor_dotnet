using CoreWCF;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace WCF.services {
    [ServiceContract]
    public interface IMetadataService {
        [OperationContract]
        Task<string> ProcessMetadataResult(byte[] fileBytes);
    }

}
