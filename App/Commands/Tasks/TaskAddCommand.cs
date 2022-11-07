using App.Commands.Tasks.Common;
using App.Extensions;
using App.Models.Inputs;
using App.Repositories;
using Spectre.Console;
using System.CommandLine;

namespace App.Commands.Tasks
{
   public class TaskAddCommand : Command
   {
      private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
      private readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();

      public TaskAddCommand() : base("add", "Add new task")
      {
         AddAlias("a");

         Add(TaskCommonOptions.Title);
         Add(TaskCommonOptions.ProjectId);
         Add(TaskCommonOptions.PlannedTime);
         Add(CommandCommonOptions.InteractiveMode);

         this.SetHandler(
            (taskInput) => AddTaskHandler(taskInput),
            new TaskInputBinder(
               title: TaskCommonOptions.Title,
               projectId: TaskCommonOptions.ProjectId,
               plannedTime: TaskCommonOptions.PlannedTime,
               interactiveMode: CommandCommonOptions.InteractiveMode
            )
         );
      }

      private void AddTaskHandler(TaskInput input)
      {
         var task = input.InteractiveMode
            ? TaskCommon.CreateTaskInteractive(input)
            : TaskCommon.CreateTask(input);

         try
         {
            TaskCommon.ValidateModel(task);
         }
         catch (Exception ex)
         {
            _console.WriteError(ex.Message);
            return;
         }

         var result = _dbRepository.Tasks.Insert(task);

         if (result is null)
         {
            _console.WriteErrorAndExit("Error while adding new project to database");
         }

         _console.MarkupLine("[green]New task added[/]");
         _console.WriteLine();
         TaskCommon.DisplayTasksList(_dbRepository.Tasks.GetActive(), "Active tasks");
      }
   }
}
