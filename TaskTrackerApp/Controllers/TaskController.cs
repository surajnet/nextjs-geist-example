using Microsoft.AspNetCore.Mvc;
using TaskTrackerApp.Models;
using TaskTrackerApp.Services;

namespace TaskTrackerApp.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(ITaskService taskService, ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        // GET: Task
        public async Task<IActionResult> Index(string filter = "all")
        {
            try
            {
                IEnumerable<TaskModel> tasks;

                switch (filter.ToLower())
                {
                    case "completed":
                        tasks = await _taskService.GetTasksByStatusAsync(true);
                        ViewData["Filter"] = "completed";
                        ViewData["FilterTitle"] = "Completed Tasks";
                        break;
                    case "pending":
                        tasks = await _taskService.GetTasksByStatusAsync(false);
                        ViewData["Filter"] = "pending";
                        ViewData["FilterTitle"] = "Pending Tasks";
                        break;
                    case "overdue":
                        tasks = await _taskService.GetOverdueTasksAsync();
                        ViewData["Filter"] = "overdue";
                        ViewData["FilterTitle"] = "Overdue Tasks";
                        break;
                    default:
                        tasks = await _taskService.GetAllTasksAsync();
                        ViewData["Filter"] = "all";
                        ViewData["FilterTitle"] = "All Tasks";
                        break;
                }

                return View(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks with filter: {Filter}", filter);
                TempData["ErrorMessage"] = "An error occurred while retrieving tasks. Please try again.";
                return View(new List<TaskModel>());
            }
        }

        // GET: Task/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    TempData["ErrorMessage"] = "Task not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task details for ID: {TaskId}", id);
                TempData["ErrorMessage"] = "An error occurred while retrieving task details. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Task/Create
        public IActionResult Create()
        {
            var task = new TaskModel
            {
                DueDate = DateTime.Today.AddDays(1)
            };
            return View(task);
        }

        // POST: Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskModel task)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _taskService.CreateTaskAsync(task);
                    TempData["SuccessMessage"] = "Task created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                return View(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task: {TaskTitle}", task.Title);
                ModelState.AddModelError("", "An error occurred while creating the task. Please try again.");
                return View(task);
            }
        }

        // GET: Task/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    TempData["ErrorMessage"] = "Task not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task for editing with ID: {TaskId}", id);
                TempData["ErrorMessage"] = "An error occurred while retrieving the task. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TaskModel task)
        {
            if (id != task.Id)
            {
                TempData["ErrorMessage"] = "Invalid task ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var updatedTask = await _taskService.UpdateTaskAsync(task);
                    if (updatedTask == null)
                    {
                        TempData["ErrorMessage"] = "Task not found.";
                        return RedirectToAction(nameof(Index));
                    }

                    TempData["SuccessMessage"] = "Task updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                return View(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID: {TaskId}", id);
                ModelState.AddModelError("", "An error occurred while updating the task. Please try again.");
                return View(task);
            }
        }

        // GET: Task/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    TempData["ErrorMessage"] = "Task not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task for deletion with ID: {TaskId}", id);
                TempData["ErrorMessage"] = "An error occurred while retrieving the task. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var result = await _taskService.DeleteTaskAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Task deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Task not found.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID: {TaskId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the task. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Task/ToggleComplete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleComplete(Guid id)
        {
            try
            {
                var result = await _taskService.ToggleTaskCompletionAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Task status updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Task not found.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling task completion with ID: {TaskId}", id);
                TempData["ErrorMessage"] = "An error occurred while updating the task status. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // AJAX endpoint for quick status toggle
        [HttpPost]
        public async Task<IActionResult> QuickToggle(Guid id)
        {
            try
            {
                var result = await _taskService.ToggleTaskCompletionAsync(id);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in quick toggle for task ID: {TaskId}", id);
                return Json(new { success = false, error = "An error occurred while updating the task." });
            }
        }
    }
}
