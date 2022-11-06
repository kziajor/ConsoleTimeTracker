using App.Commands.Tasks.Common;
using App.Repositories;

using Spectre.Console;

using System.CommandLine;
using Task = App.Entities.Task;

namespace App.Commands.Tasks;

public class TaskDetailsCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
   private readonly IAppConsole _console = ServicesProvider.GetInstance<IAppConsole>();

   public TaskDetailsCommand() : base("details", "Show task details")
   {
      AddAlias("d");

      var taskIdArgument = new Argument<int>(
            name: "id",
            getDefaultValue: () => 0,
            description: "Task id"
         );

      Add(taskIdArgument);

      this.SetHandler(taskId => ShowTaskHandler(taskId), taskIdArgument);
   }

   private void ShowTaskHandler(int taskId)
   {
      Task? task;

      if (taskId <= 0)
      {
         task = _dbRepository.Tasks.GetActive().ChooseOne("Choose task", 20, (task) => task.GetOptionLabel());
      }
      else
      {
         task = _dbRepository.Tasks.Get(taskId);
      }

      if (task is null)
      {
         AnsiConsole.MarkupLineInterpolated($"Task with id {taskId} not found");
         return;
      }

      TaskCommon.ShowTaskDetails(task);
   }
}
