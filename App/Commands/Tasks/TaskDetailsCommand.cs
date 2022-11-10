using App.Commands.Tasks.Common;
using App.Extensions;
using App.Models.Dtos;
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

      this.SetHandler((taskId, interactiveMode) => ShowTaskHandler(_dbRepository, _console, taskId, interactiveMode), idArgument, interactiveModeOption);
   }

   internal static void ShowTaskHandler(IDbRepository dbRepository, IAnsiConsole console, string? taskId, bool interactiveMode)
   {
      var universalTaskId = UniversalTaskId.Create(taskId);

      Task? task = interactiveMode
         ? TaskCommon.GetOrChoose(universalTaskId)
         : dbRepository.Tasks.Get(universalTaskId);

      if (task is null)
      {
         console.WriteError($"Task with id {universalTaskId} not found");
         return;
      }

      TaskCommon.ShowTaskDetails(task);
   }
}
