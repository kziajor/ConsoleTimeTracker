using App.Commands.Tasks.Common;
using App.Repositories;

using System.CommandLine;

namespace App.Commands.Tasks;

public class TaskCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
   private readonly ISettingsProvider settingsProvider = ServicesProvider.GetInstance<ISettingsProvider>();

   public TaskCommand() : base("task", "Manage tasks")
   {
      AddAlias("t");

      Add(new TaskAddCommand());
      Add(new TaskEditCommand());
      Add(new TaskDetailsCommand());

      var closedOption = TaskOptions.GetClosedOption();

      Add(closedOption);

      this.SetHandler((closed) => TaskListHandle(closed), closedOption);
   }

   private void TaskListHandle(bool? closed)
   {
      var orderBy = settingsProvider.ExternalSystemPriority
         ? "TA_ExternalSystemType ASC, TA_ExternalSystemTaskId DESC"
         : "TA_Id DESC";
      var tasks = closed ?? false
         ? _dbRepository.Tasks.GetClosed(orderBy)
         : _dbRepository.Tasks.GetActive(orderBy);

      TaskCommon.DisplayTasksList(tasks, closed ?? false ? "Closed tasks" : "Active tasks");
   }
}
