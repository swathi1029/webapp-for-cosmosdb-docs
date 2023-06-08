using SampleWebApi.Model;

namespace SampleWebApi.Repositories
{
    public interface ITaskRepository
    {
        Task<TaskModel> CreateTaskAsync(TaskModel task);
        Task<List<TaskModel>> GetAllTasksAsync();
        Task<TaskModel> GetTaskByIdAsync(string id);
        Task<TaskModel> UpdateTaskAsync(TaskModel task);
        Task DeleteTaskAsync(string id);
    }
}