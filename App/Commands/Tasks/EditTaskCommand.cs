using App.Commands.Projects;
using App.Entities;
using App.Extensions;
using App.Repositories;
using Spectre.Console;
using System.CommandLine;
using Task = App.Entities.Task;

namespace App.Commands.Tasks;

public class EditTaskCommand : Command
{
   private readonly IDbRepository _dbRepository;

   public EditTaskCommand(IDbRepository dbRepository) : base("edit", "Edit task")
   {
      _dbRepository = dbRepository;

      AddAlias("e");

      var taskIdArgument = new Argument<int>(
         name: "id",
         getDefaultValue: () => 0,
         description: "Task id");
      var taskTitleArgument = new Argument<string>(
         name: "title",
         getDefaultValue: () => string.Empty,
         description: "Task title");
      var taskActiveArgument = new Argument<bool?>(
         name: "active",
         getDefaultValue: () => null,
         description: "Task active");
      var projectIdArgument = new Argument<int>(
         name: "project-id",
         getDefaultValue: () => 0,
         description: "Project id");
      var plannedTimeArgument = new Argument<int>(
         name: "planned-time",
         getDefaultValue: () => 0,
         description: "Planned time");

      Add(taskIdArgument);
      Add(taskTitleArgument);
      Add(taskActiveArgument);
      Add(projectIdArgument);
      Add(plannedTimeArgument);

      this.SetHandler((taskId, taskTitle, taskActive, projectId, plannedTime) =>
         EditTaskHandler(taskId, taskTitle, taskActive, projectId, plannedTime),
         taskIdArgument, taskTitleArgument, taskActiveArgument, projectIdArgument, plannedTimeArgument);
   }

   private void EditTaskHandler(int taskId, string title, bool? active, int projectId, int? plannedTime)
   {
      Task? task = TaskCommon.GetOrChoose(_dbRepository, taskId);

      if (task is null)
      {
         AnsiConsole.MarkupLine("[red]Task not found[/]");
         return;
      }

      if (title.IsNullOrEmpty()) { title = CommandCommon.AskForWithEmptyAllowed<string>("Task name (leave empty if not changed):") ?? string.Empty; }

      var projects = _dbRepository.Projects.GetActive().Prepend(new Project() { id = 0, name = "<Leave current>" }).ToList();
      Project? project = ProjectCommon.GetOrChoose(_dbRepository, projectId, projects);

      if (project is null)
      {
         AnsiConsole.MarkupLine("[red]Project not found[/]");
         return;
      }

      active ??= CommandCommon.AskForYesNo("Task active");
      plannedTime = plannedTime <= 0 ? CommandCommon.AskForWithEmptyAllowed<int?>("Planned time (leave empty if not changed):") ?? 0 : plannedTime;

      task.title = title.IsNullOrEmpty() ? task.title : title;
      task.rel_project_id = project.id <= 0 ? task.rel_project_id : project.id;
      task.closed = !active.Value;
      task.planned_time = plannedTime is null || plannedTime < 0 ? task.planned_time : plannedTime.Value;

      var success = _dbRepository.Tasks.Update(task);

      if (!success)
      {
         AnsiConsole.MarkupLine("[red]Error while updating task[/]");
      }
      else
      {
         AnsiConsole.MarkupLine("[green]Task updated successfully[/]");
         AnsiConsole.WriteLine();
         TaskCommon.DisplayTasksList(_dbRepository.Tasks.GetActive().OrderBy(t => t.id), "Active tasks");
      }
   }
}
