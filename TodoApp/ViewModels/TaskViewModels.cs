using System.ComponentModel.DataAnnotations;
using TodoApp.Models;

namespace TodoApp.ViewModels;

public class TaskFormViewModel
{
    public Guid? Id { get; set; }
    public TaskType Type { get; set; }

    [Required(ErrorMessage = "Please enter a title")]
    [StringLength(200, ErrorMessage = "Title can be at most 200 characters")]
    [Display(Name = "Title")]
    public string Title { get; set; } = "";

    [StringLength(2000, ErrorMessage = "Description can be at most 2000 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Range(1, 5, ErrorMessage = "Difficulty must be between 1 and 5")]
    [Display(Name = "Difficulty")]
    public int Difficulty { get; set; } = 3;

    [Display(Name = "Priority")]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    // Todo
    [Display(Name = "Deadline")]
    public DateOnly? Deadline { get; set; }

    [Display(Name = "Category")]
    public ActivityCategory? Category { get; set; }

    [Display(Name = "Effort")]
    public decimal? Effort { get; set; }

    // Daily
    [Display(Name = "Repeat")]
    public RepeatType? Repeat { get; set; }
}
