using System.Configuration;

namespace SampleWebApi.Model
{
    public class CosmosDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ContainerName { get; set; }
    }
   
}
