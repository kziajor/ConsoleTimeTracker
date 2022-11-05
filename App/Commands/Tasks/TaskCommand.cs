using App.Repositories;
using System.CommandLine;

namespace App.Commands.Tasks;

public class TaskCommand : Command
{
   private readonly IDbRepository _dbRepository;
   public TaskCommand(IDbRepository dbRepository) : base("task", "Manage tasks")
   {
      _dbRepository = dbRepository;

      AddAlias("t");

      Add(new AddTaskCommand(dbRepository));
      Add(new EditTaskCommand(dbRepository));
      Add(new TaskDetailsCommand(dbRepository));

      var closedOption = new Option<bool>(
            name: "--closed",
            getDefaultValue: () => false,
            description: "Get closed tasks"
         );

      closedOption.AddAlias("-c");

      Add(closedOption);

      this.SetHandler((closed) => TaskListHandle(closed), closedOption);
   }

   private void TaskListHandle(bool closed)
   {
      var tasks = closed
         ? _dbRepository.Tasks.GetClosed()
         : _dbRepository.Tasks.GetActive();

      TaskCommon.DisplayTasksList(tasks.OrderBy(t => t.TA_Id), closed ? "Closed tasks" : "Active tasks");
   }
}
