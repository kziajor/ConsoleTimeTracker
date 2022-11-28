using App.Commands.Tasks.Common;
using App.Repositories;
using Spectre.Console;
using System.CommandLine;

namespace App.Commands.Tasks;

public class TaskCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
   private readonly ISettingsProvider _settingsProvider = ServicesProvider.GetInstance<ISettingsProvider>();
   private readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();

   public TaskCommand() : base("task", "Manage tasks")
   {
      Add(new TaskAddCommand());
      Add(new TaskEditCommand());
      Add(new TaskDetailsCommand());

      var taskIdArgument = TaskArguments.GetIdArgument();
      var closedOption = TaskOptions.GetClosedOption();

      Add(taskIdArgument);
      Add(closedOption);

      this.SetHandler((taskId, closed) => TaskListHandle(taskId, closed), taskIdArgument, closedOption);
   }

   private void TaskListHandle(string? taskId, bool? closed)
   {
      if (!string.IsNullOrEmpty(taskId))
      {
         TaskDetailsCommand.ShowTaskHandler(_dbRepository, _console, taskId, false);
         return;
      }

      var tasks = closed ?? false
         ? _dbRepository.Tasks.GetClosed()
         : _dbRepository.Tasks.GetActive();

      TaskCommon.DisplayList(tasks, closed ?? false ? "Closed tasks" : "Active tasks");
   }
}
