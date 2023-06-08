using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.VisualBasic;
using SampleWebApi.Model;
using SampleWebApi.Repositories;

namespace SampleWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        public TaskController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }


        // GET: api/tasks
        [HttpGet]
        public async Task<ActionResult<List<TaskModel>>> GetAllTasks()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return Ok(tasks);
        }

        // GET: api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskModel>> GetTaskById(string id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        // POST: api/tasks
        [HttpPost]
        public async Task<ActionResult<TaskModel>> CreateTaskAsync([FromBody] TaskModel task)
        {

            // Create an instance of CosmosClient
            //CosmosClient cosmosClient = new CosmosClient("connectionString");

            // Create an instance of TaskRepository
           // _taskRepository = new ITaskRepository(cosmosClient, "databaseName", "containerName");

            // Create a new task item
            TaskModel newTask = new TaskModel
            {
                Id = task.Id,
                Title = task.Title,

                Description = task.Description,
                DueDate = task.DueDate
            };

            var createdTask = await _taskRepository.CreateTaskAsync(newTask);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskAync(string id, [FromBody]TaskModel task)
        {
            var existingTask = await _taskRepository.GetTaskByIdAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }
            
            existingTask.Title = task.Title;
            existingTask.Description = task.Description;    
            existingTask.DueDate = task.DueDate;
           
            var updatedTask = await _taskRepository.UpdateTaskAsync(existingTask);
            return Ok(updatedTask);
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskAsync(string id)
        {
            var existingTask = await _taskRepository.GetTaskByIdAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            await _taskRepository.DeleteTaskAsync(id);
            return NoContent();
        }

    }
}
