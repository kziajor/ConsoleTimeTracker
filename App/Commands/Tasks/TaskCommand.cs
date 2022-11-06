﻿using App.Commands.Tasks.Common;
using App.Repositories;
using System.CommandLine;

namespace App.Commands.Tasks;

public class TaskCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();

   public TaskCommand() : base("task", "Manage tasks")
   {
      AddAlias("t");

      Add(new TaskAddCommand());
      Add(new TaskEditCommand());
      Add(new TaskDetailsCommand());

      Add(TaskCommonOptions.Closed);

      this.SetHandler((closed) => TaskListHandle(closed), TaskCommonOptions.Closed);
   }

   private void TaskListHandle(bool? closed)
   {
      var tasks = closed ?? false
         ? _dbRepository.Tasks.GetClosed()
         : _dbRepository.Tasks.GetActive();

      TaskCommon.DisplayTasksList(tasks, closed ?? false ? "Closed tasks" : "Active tasks");
   }
}
