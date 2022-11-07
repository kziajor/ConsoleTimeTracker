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

         var titleOption = TaskOptions.GetTitleOption();
         var projectIdOption = TaskOptions.GetProjectIdOption();
         var plannedTimeOption = TaskOptions.GetPlannedTimeOption();
         var closedOption = TaskOptions.GetClosedOption();
         var externalSystemTypeOption = TaskOptions.GetExternalSystemTypeOption();
         var externalSystemTaskIdOption = TaskOptions.GetExternalSystemTaskIdOption();
         var interactiveMode = CommonOptions.GetInteractiveModeOption();

         Add(titleOption);
         Add(projectIdOption);
         Add(plannedTimeOption);
         Add(closedOption);
         Add(externalSystemTypeOption);
         Add(externalSystemTaskIdOption);
         Add(interactiveMode);

         this.SetHandler(
            (taskInput) => AddTaskHandler(taskInput),
            new TaskInputBinder(
               title: titleOption,
               closed: closedOption,
               projectId: projectIdOption,
               plannedTime: plannedTimeOption,
               interactiveMode: interactiveMode,
               externalSystemType: externalSystemTypeOption,
               externalSystemTaskId: externalSystemTaskIdOption
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
