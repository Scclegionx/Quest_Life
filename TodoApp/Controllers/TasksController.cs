using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.Services;
using TodoApp.ViewModels;

namespace TodoApp.Controllers;

[Authorize]
public class TasksController(TaskService tasks) : GameControllerBase
{
    [HttpGet]
    public IActionResult Create(TaskType type = TaskType.Todo, ActivityCategory? category = null)
    {
        if (!Enum.IsDefined(type)) type = TaskType.Todo;
        return View("Form", new TaskFormViewModel
        {
            Type = type,
            Repeat = type == TaskType.Daily ? RepeatType.Daily : null,
            Category = type == TaskType.Todo && category is { } c && Enum.IsDefined(c) ? c : null,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskFormViewModel form)
    {
        Validate(form);
        if (!ModelState.IsValid) return View("Form", form);
        await tasks.CreateAsync(UserId, form, Clock);
        return Done(GameResult.Success($"Added {TaskLabels.Type(form.Type).ToLowerInvariant()} \"{form.Title.Trim()}\""), "Index", "Dashboard");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var t = await tasks.GetAsync(UserId, id);
        if (t is null) return NotFound();
        return View("Form", new TaskFormViewModel
        {
            Id = t.Id, Type = t.Type, Title = t.Title, Description = t.Description, Difficulty = t.Difficulty,
            Priority = t.Priority, Deadline = t.Deadline, Category = t.Category, Effort = t.Effort, Repeat = t.Repeat,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TaskFormViewModel form)
    {
        var existing = form.Id is { } id ? await tasks.GetAsync(UserId, id) : null;
        if (existing is null) return NotFound();
        form.Type = existing.Type; // the kind of a task can't change
        Validate(form);
        if (!ModelState.IsValid) return View("Form", form);
        await tasks.UpdateAsync(UserId, form, Clock);
        return Done(GameResult.Success("Task updated"), "Index", "Dashboard");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await tasks.DeleteAsync(UserId, id);
        return Done(GameResult.Success("Task deleted"), "Index", "Dashboard");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(Guid id) =>
        Done(await tasks.CompleteAsync(UserId, id), "Index", "Dashboard");

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Habit(Guid id, bool positive) =>
        Done(await tasks.HabitAsync(UserId, id, positive), "Index", "Dashboard");

    private void Validate(TaskFormViewModel f)
    {
        foreach (var (field, message) in TaskFormValidator.Validate(f))
            ModelState.AddModelError(field, message);
    }
}
