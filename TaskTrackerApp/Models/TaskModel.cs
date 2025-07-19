using System.ComponentModel.DataAnnotations;

namespace TaskTrackerApp.Models
{
    public class TaskModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Due date is required")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(1);

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Helper property for display
        public string Status => IsCompleted ? "Completed" : "Pending";
        
        // Helper property for due date status
        public string DueDateStatus
        {
            get
            {
                if (IsCompleted) return "Completed";
                if (DueDate.Date < DateTime.Today) return "Overdue";
                if (DueDate.Date == DateTime.Today) return "Due Today";
                return "Upcoming";
            }
        }

        // Helper property for CSS class based on status
        public string StatusCssClass
        {
            get
            {
                if (IsCompleted) return "badge bg-success";
                if (DueDate.Date < DateTime.Today) return "badge bg-danger";
                if (DueDate.Date == DateTime.Today) return "badge bg-warning";
                return "badge bg-primary";
            }
        }
    }
}
