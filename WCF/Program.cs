using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using WCF.services;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddServiceModelServices().AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior>(new ServiceDebugBehavior {
    IncludeExceptionDetailInFaults = true
});
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();


var app = builder.Build();

var myWSHttpBinding = new WSHttpBinding(SecurityMode.Transport);
myWSHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

app.UseServiceModel(builder => {
    builder.AddService<MetadataService>((serviceOptions) => { })
    .AddServiceEndpoint<MetadataService, IMetadataService>(new BasicHttpBinding(), "/MetadataService/basichttp")
    .AddServiceEndpoint<MetadataService, IMetadataService>(myWSHttpBinding, "/MetadataService/WSHttps");
});


var serviceMetadataBehavior = app.Services.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();
serviceMetadataBehavior.HttpGetEnabled = true;

app.Run();