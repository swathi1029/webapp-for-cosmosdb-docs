using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SampleWebApi.Model;
using SampleWebApi.Repositories;

namespace SampleWebApi.Repositories
{
     

    public class TaskRepository : ITaskRepository
    {
        private string CosmosDbConnectionString;
        private string CosmosDbKey;



        //public readonly string CosmosDbConnectionString = "AccountEndpoint=https://cosmosdbfordocs.documents.azure.com:443/;AccountKey=xgNu22pE5VOs2l2dpOSHdp27EhexeERGhrrvvE8Zzx1hrjCG60w2FC3B6yHFGmmm2kurfxfP0PSPACDbqOEQZg==";
        public readonly string CosmosDbName = "TasksManagementDB";
        public readonly string CosmosDbContainerName = "Tasks";

        private Container GetContainerClient()
        {
            var credential = new DefaultAzureCredential();
            var keyVaultUrl = "https://swathi1325908.vault.azure.net/";
            var secretClient = new SecretClient(new Uri(keyVaultUrl), credential);

            var cosmosDbConnectionStringSecretName = "cosmosdbfordocs-endpoint";
            var cosmosDbKeySecretName = "cosmosdbfordocs-key";

            var cosmosDbConnectionStringSecret =  secretClient.GetSecret(cosmosDbConnectionStringSecretName);
            var cosmosDbKeySecret =  secretClient.GetSecret(cosmosDbKeySecretName);

            CosmosDbConnectionString = cosmosDbConnectionStringSecret.Value.Value;
            CosmosDbKey = cosmosDbKeySecret.Value.Value;
        


        var cosmosDbClient = new CosmosClient(CosmosDbConnectionString,CosmosDbKey);
            var container = cosmosDbClient.GetContainer(CosmosDbName, CosmosDbContainerName);
            return container;
        }

        //private readonly string CosmosDBAccountUri = "https://localhost:8081/";
        //private readonly string CosmosDBAccountPrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        //private readonly string databaseName = "TaskManagementDb";
        //private readonly string ContainerName = "Tasks";


        //private readonly Container _container;

        //public TaskRepository(CosmosClient cosmosDbClient, string databaseName, string containerName)
        //{
        //    cosmosDbClient = new CosmosClient(CosmosDBAccountUri, CosmosDBAccountPrimaryKey);
        //    _container = cosmosDbClient.GetContainer(databaseName, containerName);
        //}
        
        public async Task<TaskModel> CreateTaskAsync(TaskModel task)
        {
            if (string.IsNullOrEmpty(task.Id))
            {
                task.Id = Guid.NewGuid().ToString();
            }
            task.Id = task.Id.ToString();   

            var container = GetContainerClient();
            var response = await container.CreateItemAsync(task, new PartitionKey(task.Id));
            return response.Resource;
           

        }

        public async Task<List<TaskModel>> GetAllTasksAsync()
        {
            var container = GetContainerClient();
            var query = container.GetItemQueryIterator<TaskModel>(new QueryDefinition("SELECT * FROM c"));
            List<TaskModel> results = new List<TaskModel>();

            while (query.HasMoreResults)
            {
                FeedResponse<TaskModel> response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<TaskModel> GetTaskByIdAsync(string id)
        {
            var container = GetContainerClient();
            try
            {
                ItemResponse<TaskModel> response = await container.ReadItemAsync<TaskModel>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<TaskModel> UpdateTaskAsync(TaskModel task)
        {
            var container = GetContainerClient();
            ItemResponse<TaskModel> response = await container.ReplaceItemAsync(task, task.Id, new PartitionKey(task.Id));
            
            return response.Resource;
        }

        public async Task DeleteTaskAsync(string id)
        {
            var container = GetContainerClient();
            await container.DeleteItemAsync<TaskModel>(id, new PartitionKey(id));
        }
    }
}
