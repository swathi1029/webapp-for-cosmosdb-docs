using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using SampleWebApi.Model;

namespace SampleWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly string CosmosDBAccountUri = "https://localhost:8081/";
        private readonly string CosmosDBAccountPrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private readonly string CosmosDbName = "EmployeeManagementDB";
        private readonly string CosmosDbContainerName = "Employees";

        private Container ContainerClient()
        {
            CosmosClient cosmosDbClient = new CosmosClient(CosmosDBAccountUri, CosmosDBAccountPrimaryKey);
            Container containerClient = cosmosDbClient.GetContainer(CosmosDbName, CosmosDbContainerName);
            return containerClient;
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            try
            {
                var container = ContainerClient();
                var response = await container.CreateItemAsync(employee, new PartitionKey(employee.Department));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetails()
        {
            try
            {
                var container = ContainerClient();
                var sqlQuery = "SELECT * FROM c";
                QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
                FeedIterator<Employee> queryResultSetIterator = container.GetItemQueryIterator<Employee>(queryDefinition);
                List<Employee> employees = new List<Employee>();
                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<Employee> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (Employee employee in currentResultSet)
                    {
                        employees.Add(employee);
                    }
                }
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetailsById(string employeeId, string partitionKey)
        {
            try
            {
                var container = ContainerClient();
                ItemResponse<Employee> response = await container.ReadItemAsync<Employee>(employeeId, new PartitionKey(partitionKey));
                return Ok(response.Resource);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(Employee emp, string partitionKey)
        {
            try
            {
                var container = ContainerClient();
                ItemResponse<Employee> res = await container.ReadItemAsync<Employee>(emp.id, new PartitionKey(partitionKey));
                //Get Existing Item
                var existingItem = res.Resource;
                //Replace existing item values with new values
                existingItem.Name = emp.Name;
                existingItem.Country = emp.Country;
                existingItem.City = emp.City;
                existingItem.Department = emp.Department;
                existingItem.Designation = emp.Designation;
                var updateRes = await container.ReplaceItemAsync(existingItem, emp.id, new PartitionKey(partitionKey));
                return Ok(updateRes.Resource);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(string empId, string partitionKey)
        {
            try
            {
                var container = ContainerClient();
                var response = await container.DeleteItemAsync<Employee>(empId, new PartitionKey(partitionKey));
                return Ok(response.StatusCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





    }

   
}
