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
      var idArgument = TaskArguments.GetIdArgument();
      var manualModeOption = CommonOptions.GetManualModeOption();

      Add(idArgument);
      Add(manualModeOption);

      this.SetHandler((taskId, manualMode) => ShowTaskHandler(_dbRepository, _console, taskId, manualMode), idArgument, manualModeOption);
   }

   internal static void ShowTaskHandler(IDbRepository dbRepository, IAnsiConsole console, string? taskId, bool manualMode)
   {
      var universalTaskId = UniversalTaskId.Create(taskId);

      Task? task = manualMode
         ? dbRepository.Tasks.Get(universalTaskId)
         : TaskCommon.GetOrChoose(universalTaskId);

      if (task is null)
      {
         console.WriteError($"Task with id {universalTaskId} not found");
         return;
      }

      TaskCommon.ShowTaskDetails(task);
   }
}
