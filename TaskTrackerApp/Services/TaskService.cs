using TaskTrackerApp.Models;

namespace TaskTrackerApp.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskModel>> GetAllTasksAsync();
        Task<TaskModel?> GetTaskByIdAsync(Guid id);
        Task<TaskModel> CreateTaskAsync(TaskModel task);
        Task<TaskModel?> UpdateTaskAsync(TaskModel task);
        Task<bool> DeleteTaskAsync(Guid id);
        Task<bool> ToggleTaskCompletionAsync(Guid id);
        Task<IEnumerable<TaskModel>> GetTasksByStatusAsync(bool isCompleted);
        Task<IEnumerable<TaskModel>> GetOverdueTasksAsync();
    }

    public class TaskService : ITaskService
    {
        // In-memory storage for demo purposes
        // In a real application, this would be replaced with a database
        private static readonly List<TaskModel> _tasks = new List<TaskModel>();
        private readonly ILogger<TaskService> _logger;

        public TaskService(ILogger<TaskService> logger)
        {
            _logger = logger;
            
            // Initialize with some sample data if empty
            if (!_tasks.Any())
            {
                InitializeSampleData();
            }
        }

        public async Task<IEnumerable<TaskModel>> GetAllTasksAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all tasks");
                return await Task.FromResult(_tasks.OrderBy(t => t.DueDate).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all tasks");
                throw new Exception("Error retrieving tasks", ex);
            }
        }

        public async Task<TaskModel?> GetTaskByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving task with ID: {TaskId}", id);
                return await Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task with ID: {TaskId}", id);
                throw new Exception($"Error retrieving task with ID: {id}", ex);
            }
        }

        public async Task<TaskModel> CreateTaskAsync(TaskModel task)
        {
            try
            {
                if (task == null)
                    throw new ArgumentNullException(nameof(task));

                if (string.IsNullOrWhiteSpace(task.Title))
                    throw new ArgumentException("Task title cannot be empty");

                task.Id = Guid.NewGuid();
                task.CreatedAt = DateTime.Now;
                
                _tasks.Add(task);
                
                _logger.LogInformation("Created new task: {TaskTitle} with ID: {TaskId}", task.Title, task.Id);
                return await Task.FromResult(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task: {TaskTitle}", task?.Title);
                throw new Exception("Error creating task", ex);
            }
        }

        public async Task<TaskModel?> UpdateTaskAsync(TaskModel task)
        {
            try
            {
                if (task == null)
                    throw new ArgumentNullException(nameof(task));

                var existingTask = _tasks.FirstOrDefault(t => t.Id == task.Id);
                if (existingTask == null)
                    return null;

                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.DueDate = task.DueDate;
                existingTask.IsCompleted = task.IsCompleted;

                _logger.LogInformation("Updated task: {TaskTitle} with ID: {TaskId}", task.Title, task.Id);
                return await Task.FromResult(existingTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID: {TaskId}", task?.Id);
                throw new Exception("Error updating task", ex);
            }
        }

        public async Task<bool> DeleteTaskAsync(Guid id)
        {
            try
            {
                var task = _tasks.FirstOrDefault(t => t.Id == id);
                if (task == null)
                    return false;

                _tasks.Remove(task);
                _logger.LogInformation("Deleted task: {TaskTitle} with ID: {TaskId}", task.Title, id);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID: {TaskId}", id);
                throw new Exception($"Error deleting task with ID: {id}", ex);
            }
        }

        public async Task<bool> ToggleTaskCompletionAsync(Guid id)
        {
            try
            {
                var task = _tasks.FirstOrDefault(t => t.Id == id);
                if (task == null)
                    return false;

                task.IsCompleted = !task.IsCompleted;
                _logger.LogInformation("Toggled completion status for task: {TaskTitle} with ID: {TaskId}. New status: {IsCompleted}", 
                    task.Title, id, task.IsCompleted);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling task completion with ID: {TaskId}", id);
                throw new Exception($"Error toggling task completion with ID: {id}", ex);
            }
        }

        public async Task<IEnumerable<TaskModel>> GetTasksByStatusAsync(bool isCompleted)
        {
            try
            {
                _logger.LogInformation("Retrieving tasks with completion status: {IsCompleted}", isCompleted);
                return await Task.FromResult(_tasks.Where(t => t.IsCompleted == isCompleted).OrderBy(t => t.DueDate).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks by status: {IsCompleted}", isCompleted);
                throw new Exception("Error retrieving tasks by status", ex);
            }
        }

        public async Task<IEnumerable<TaskModel>> GetOverdueTasksAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving overdue tasks");
                return await Task.FromResult(_tasks.Where(t => !t.IsCompleted && t.DueDate.Date < DateTime.Today).OrderBy(t => t.DueDate).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving overdue tasks");
                throw new Exception("Error retrieving overdue tasks", ex);
            }
        }

        private void InitializeSampleData()
        {
            _tasks.AddRange(new List<TaskModel>
            {
                new TaskModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Complete project documentation",
                    Description = "Write comprehensive documentation for the task tracker application",
                    DueDate = DateTime.Today.AddDays(3),
                    IsCompleted = false,
                    CreatedAt = DateTime.Now.AddDays(-2)
                },
                new TaskModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Review code changes",
                    Description = "Review and approve pending pull requests",
                    DueDate = DateTime.Today.AddDays(1),
                    IsCompleted = false,
                    CreatedAt = DateTime.Now.AddDays(-1)
                },
                new TaskModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Setup development environment",
                    Description = "Configure local development environment for new team members",
                    DueDate = DateTime.Today.AddDays(-1),
                    IsCompleted = true,
                    CreatedAt = DateTime.Now.AddDays(-5)
                },
                new TaskModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Prepare presentation",
                    Description = "Create slides for the quarterly review meeting",
                    DueDate = DateTime.Today,
                    IsCompleted = false,
                    CreatedAt = DateTime.Now.AddHours(-6)
                }
            });
        }
    }
}
