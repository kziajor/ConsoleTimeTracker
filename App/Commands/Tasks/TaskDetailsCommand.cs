using App.Commands.Tasks.Common;
using App.Extensions;
using App.Repositories;

using Spectre.Console;

using System.CommandLine;

using Task = App.Entities.Task;

namespace App.Commands.Tasks;

public class TaskDetailsCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
   private readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();

   public TaskDetailsCommand() : base("details", "Show task details")
   {
      AddAlias("d");

      var idArgument = TaskArguments.GetIdArgument();
      var interactiveModeOption = CommonOptions.GetInteractiveModeOption();

      Add(idArgument);
      Add(interactiveModeOption);

      this.SetHandler((taskId, interactiveMode) => ShowTaskHandler(taskId, interactiveMode), idArgument, interactiveModeOption);
   }

   private void ShowTaskHandler(int taskId, bool interactiveMode)
   {
      Task? task = interactiveMode
         ? TaskCommon.GetOrChoose(taskId)
         : _dbRepository.Tasks.Get(taskId);

      if (task is null)
      {
         _console.WriteError($"Task with id {taskId} not found");
         return;
      }

      TaskCommon.ShowTaskDetails(task);
   }
}
