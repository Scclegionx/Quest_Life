using TodoApp.Models;
using TodoApp.ViewModels;

namespace TodoApp.Services;

/// <summary>Validation shared by the website form and the Discord commands. Also rounds the effort to its unit.</summary>
public static class TaskFormValidator
{
    public static List<(string Field, string Message)> Validate(TaskFormViewModel f)
    {
        var errors = new List<(string, string)>();

        if (f.Type == TaskType.Daily && f.Repeat is null)
            errors.Add((nameof(f.Repeat), "Please choose how often it repeats"));

        if (f.Type == TaskType.Todo && f.Category is { } cat)
        {
            var info = Categories.Get(cat);
            if (f.Effort is null or <= 0)
                errors.Add((nameof(f.Effort), $"Please enter the number of {info.Unit}"));
            else if (f.Effort > info.MaxPerEntry)
                errors.Add((nameof(f.Effort), $"At most {info.MaxPerEntry.Num()} {info.Unit} per task"));
            else
                f.Effort = info.IsHours ? Math.Round(f.Effort.Value, 2) : Math.Round(f.Effort.Value, 0);
        }
        return errors;
    }
}
