using Azure.Identity;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Identity.Client;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SampleWebApi.Model;
using SampleWebApi.Repositories;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using ClientCredential = Microsoft.Identity.Client.ClientCredential;
using Microsoft.Azure.Documents;
using Azure.Core;

using Microsoft.Azure.Documents.Client;

using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace SampleWebApi.Repositories
{
    //public class AzureADAuthenticationHelper
    //{
    //    public async Task<string> GetAccessTokenAsync(string clientId, string clientSecret, string tenantId)
    //    {
    //        var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
    //        var tokenRequestContext = new TokenRequestContext(new[] { "https://vault.azure.net/.default" });

    //        var token = await clientSecretCredential.GetTokenAsync(tokenRequestContext);
    //        return token.Token;
    //    }
    //}
    public class KeyVaultHelper
    {
        private readonly string keyVaultUrl;
        private readonly TokenCredential tokenCredential;

        public KeyVaultHelper(string keyVaultUrl, TokenCredential tokenCredential)
        {
            this.keyVaultUrl = keyVaultUrl;
            this.tokenCredential = tokenCredential;
        }

        public string GetSecret(string secretName)
        {
            var secretClient = new SecretClient(new Uri(keyVaultUrl), tokenCredential);
            var secret = secretClient.GetSecret(secretName);

            return secret.Value.Value;
        }
    }
    public class TaskRepository : ITaskRepository
    {
        private string CosmosDbConnectionString;
        private string CosmosDbKey;



        //public readonly string CosmosDbConnectionString = "AccountEndpoint=https://cosmosdbfordocs.documents.azure.com:443/;AccountKey=xgNu22pE5VOs2l2dpOSHdp27EhexeERGhrrvvE8Zzx1hrjCG60w2FC3B6yHFGmmm2kurfxfP0PSPACDbqOEQZg==";
        public readonly string CosmosDbName = "TasksManagementDB";
        public readonly string CosmosDbContainerName = "Tasks";
        //private async Task<string> GetAccessTokenAsync()
        //{
        //    var clientId = "408a258e-cf9b-405c-9217-818982c9831a";
        //    var clientSecret = "vkH8Q~8bew0i3zl0QAYtqdq6zTqI2LugcTBFraQU";

        //    var authority = "https://login.microsoftonline.com/vkH8Q~8bew0i3zl0QAYtqdq6zTqI2LugcTBFraQU/";

        //    var clientCredential = AuthenticationContext.AcquireTokenAsync(string resource, Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential clientCredential);
        //    var authenticationContext = new AuthenticationContext(authority);

        //    var result = await authenticationContext.AcquireTokenAsync("https://vault.azure.net", clientCredential);

        //    return result.AccessToken;
        //}
        [Obsolete]
        private async Task<string> GetAccessToken(string authority, string resource, string scope)
        {

            string clientId = "408a258e-cf9b-405c-9217-818982c9831a";// Configuration["ClientID"];
            string clientSecret = "vkH8Q~8bew0i3zl0QAYtqdq6zTqI2LugcTBFraQU";
            var tenantId = "a6b744ad-6d50-4ee1-b5d5-39e674f09e29";
            var clientCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            var authContext = new AuthenticationContext(authority);
            var result = authContext.AcquireTokenAsync(resource, (IClientAssertionCertificate)clientCredential).GetAwaiter().GetResult();
            return result.AccessToken;
        }
        public string GetVaultValue()
        {
            var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
            string vaultBaseUrl = "https://Mykeyvault1325908.vault.azure.net/";
            string secretName = "cosmosdbfordocs-key";
            var secret = client.GetSecretAsync(vaultBaseUrl, secretName).GetAwaiter().GetResult();
            return secret.Value;

        }
        public string GetKeySecret()
        {
            DefaultAzureCredential credential = new DefaultAzureCredential();
            Uri keyVaultUri = new Uri("https://Mykeyvault1325908.vault.azure.net/");

            var client = new SecretClient(keyVaultUri, credential);

            KeyVaultSecret secret = client.GetSecret("cosmosdbfordocs-endpoint");

            var secretValue = secret.Value;
            return secretValue;
        }

        private Container GetContainerClient()
        {
            //var clientId = "408a258e-cf9b-405c-9217-818982c9831a";
            //var clientSecret = "vkH8Q~8bew0i3zl0QAYtqdq6zTqI2LugcTBFraQU";
            ////var credential = new DefaultAzureCredential();
            //var tenantId = "a6b744ad-6d50-4ee1-b5d5-39e674f09e29";

            ////var azureADHelper = new AzureADAuthenticationHelper();
            ////var accessToken = await azureADHelper.GetAccessTokenAsync("408a258e-cf9b-405c-9217-818982c9831a", "vkH8Q~8bew0i3zl0QAYtqdq6zTqI2LugcTBFraQU", "a6b744ad-6d50-4ee1-b5d5-39e674f09e29");
            //var keyVaultUrl = "https://swathi1325908.vault.azure.net/";


            //var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
            //string vaultBaseUrl = appSettings.Value.vaultBaseUrl;
            //string secretName = appSettings.Value.secretName;
            //var secret = client.GetSecretAsync(vaultBaseUrl, secretName).GetAwaiter().GetResult();
            //return secret.Value;

            //var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            //var keyVaultHelper = new KeyVaultHelper(keyVaultUrl, clientSecretCredential);
            //var client = new SecretClient(new Uri(keyVaultUrl), clientSecretCredential);


            //var secreturi = "cosmosdbfordocs-endpoint";
            var secretValue = GetKeySecret();
            // var secretkey = "cosmosdbfordocs-key";
            var secretkeyval = GetVaultValue();


            //var cosmosDbConnectionStringSecretName = "cosmosdbfordocs-endpoint";
            //var cosmosDbKeySecretName = "cosmosdbfordocs-key";

            //var cosmosDbConnectionStringSecret =  secretClient.GetSecret(cosmosDbConnectionStringSecretName);
           // var cosmosDbKeySecret =  secretClient.GetSecret(cosmosDbKeySecretName);

            CosmosDbConnectionString = secretValue;
            CosmosDbKey = secretkeyval;
        


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
            var response = await container.CreateItemAsync(task, new Microsoft.Azure.Cosmos.PartitionKey(task.Id));
            return response.Resource;
           

        }

        public async Task<List<TaskModel>> GetAllTasksAsync()
        {
            var container = GetContainerClient();
            var query = container.GetItemQueryIterator<TaskModel>(new QueryDefinition("SELECT * FROM c"));
            List<TaskModel> results = new List<TaskModel>();

            while (query.HasMoreResults)
            {
                Microsoft.Azure.Cosmos.FeedResponse<TaskModel> response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<TaskModel> GetTaskByIdAsync(string id)
        {
            var container = GetContainerClient();
            try
            {
                ItemResponse<TaskModel> response = await container.ReadItemAsync<TaskModel>(id, new Microsoft.Azure.Cosmos.PartitionKey(id));
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
            ItemResponse<TaskModel> response = await container.ReplaceItemAsync(task, task.Id, new Microsoft.Azure.Cosmos.PartitionKey(task.Id));
            
            return response.Resource;
        }

        public async Task DeleteTaskAsync(string id)
        {
            var container = GetContainerClient();
            await container.DeleteItemAsync<TaskModel>(id, new Microsoft.Azure.Cosmos.PartitionKey(id));
        }
    }
}
