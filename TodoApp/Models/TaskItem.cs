namespace TodoApp.Models;

public enum TaskType { Habit, Daily, Todo }
public enum RepeatType { Daily, Weekly, Monthly, Yearly }
public enum TaskPriority { Low = 1, Medium = 2, High = 3 }

/// <summary>One table for all three task kinds; fields that don't apply to a kind stay null.</summary>
public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public TaskType Type { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    /// <summary>1 (very easy) .. 5 (very hard).</summary>
    public int Difficulty { get; set; } = 3;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>Soft delete.</summary>
    public DateTime? ArchivedAt { get; set; }

    // Todo
    public DateOnly? Deadline { get; set; }
    public ActivityCategory? Category { get; set; }
    /// <summary>Hours or times, depending on the category's unit.</summary>
    public decimal? Effort { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? OverduePenalizedAt { get; set; }

    // Daily
    public RepeatType? Repeat { get; set; }
    /// <summary>Start date of the last finished period already checked for a miss.</summary>
    public DateOnly? LastCheckedPeriod { get; set; }
    public int Streak { get; set; }
    public int BestStreak { get; set; }

    public bool IsDone => CompletedAt is not null;
}

/// <summary>A completed period of a daily task (the period is identified by its start date).</summary>
public class DailyCompletion
{
    public Guid TaskId { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>One press of the + (Positive) or − button of a habit.</summary>
public class HabitLog
{
    public long Id { get; set; }
    public Guid TaskId { get; set; }
    public bool Positive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class TaskLabels
{
    public static string Difficulty(int d) => d switch { 1 => "Trivial", 2 => "Easy", 3 => "Medium", 4 => "Hard", _ => "Extreme" };
    public static string Stars(int d) => new string('★', d) + new string('☆', 5 - d);
    public static string Priority(TaskPriority p) => p switch { TaskPriority.Low => "Low", TaskPriority.Medium => "Medium", _ => "High" };
    public static string Repeat(RepeatType r) => r switch
    {
        RepeatType.Daily => "Daily", RepeatType.Weekly => "Weekly", RepeatType.Monthly => "Monthly", _ => "Yearly",
    };
    public static string Type(TaskType t) => t switch { TaskType.Habit => "Habit", TaskType.Daily => "Daily", _ => "To-Do" };
}
