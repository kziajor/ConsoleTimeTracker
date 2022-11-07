using App.Commands.Tasks.Common;
using App.Extensions;
using App.Models.Inputs;
using App.Repositories;
using Spectre.Console;
using System.CommandLine;
using Task = App.Entities.Task;

namespace App.Commands.Tasks;

public class TaskEditCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
   private readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();

   public TaskEditCommand() : base("edit", "Edit task")
   {
      AddAlias("e");

      Add(TaskCommonArguments.Id);
      Add(TaskCommonOptions.Title);
      Add(TaskCommonOptions.Closed);
      Add(TaskCommonOptions.ProjectId);
      Add(TaskCommonOptions.PlannedTime);
      Add(CommandCommonOptions.InteractiveMode);

      this.SetHandler(
         (taskInput) => EditTaskHandler(taskInput),
         new TaskInputBinder(
            id: TaskCommonArguments.Id,
            title: TaskCommonOptions.Title,
            closed: TaskCommonOptions.Closed,
            projectId: TaskCommonOptions.ProjectId,
            plannedTime: TaskCommonOptions.PlannedTime,
            interactiveMode: CommandCommonOptions.InteractiveMode
         )
      );
   }

   private void EditTaskHandler(TaskInput input)
   {
      Task? task = input.InteractiveMode
         ? TaskCommon.GetOrChoose(input.Id)
         : _dbRepository.Tasks.Get(input.Id);

      if (task is null)
      {
         _console.WriteError("Task not found");
         return;
      }

      if (input.InteractiveMode) { TaskCommon.UpdateTaskDataInteractive(task, input); }
      else { TaskCommon.UpdateTaskData(task, input); }

      try
      {
         TaskCommon.ValidateModel(task);
      }
      catch (Exception ex)
      {
         _console.WriteError(ex.Message);
         return;
      }

      var success = _dbRepository.Tasks.Update(task);

      if (!success)
      {
         _console.WriteError("Error while updating task");
         return;
      }

      _console.MarkupLine("[green]Task updated successfully[/]");
      _console.WriteLine();
      TaskCommon.DisplayTasksList(_dbRepository.Tasks.GetActive(), "Active tasks");
   }
}
