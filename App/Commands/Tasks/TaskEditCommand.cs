﻿using App.Commands.Tasks.Common;
using App.Extensions;
using App.Models.Inputs;
using App.Repositories;
using Spectre.Console;
using System.CommandLine;
using Task = App.Entities.Task;


namespace App.Commands.Tasks;

public class TaskEditCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
   private readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();

   public TaskEditCommand() : base("edit", "Edit task")
   {
      var idArgument = TaskArguments.GetIdArgument();
      var titleOption = TaskOptions.GetTitleOption();
      var projectIdOption = TaskOptions.GetProjectIdOption();
      var plannedTimeOption = TaskOptions.GetPlannedTimeOption();
      var closedOption = TaskOptions.GetClosedOption();
      var sourceTypeOption = TaskOptions.GetSourceTypeOption();
      var sourceTaskIdOption = TaskOptions.GetSourceTaskIdOption();
      var manualMode = CommonOptions.GetManualModeOption();

      Add(idArgument);
      Add(titleOption);
      Add(projectIdOption);
      Add(plannedTimeOption);
      Add(closedOption);
      Add(sourceTypeOption);
      Add(sourceTaskIdOption);
      Add(manualMode);

      this.SetHandler(
         (taskInput) => EditTaskHandler(taskInput),
         new TaskInputBinder(
            id: idArgument,
            title: titleOption,
            closed: closedOption,
            projectId: projectIdOption,
            plannedTime: plannedTimeOption,
            manualMode: manualMode,
            sourceType: sourceTypeOption,
            sourceTaskId: sourceTaskIdOption
         )
      );
   }

   private void EditTaskHandler(TaskInput input)
   {
      Task? task = input.ManualMode
         ? _dbRepository.Tasks.Get(input.UniversalTaskId)
         : TaskCommon.GetOrChoose(input.UniversalTaskId);

      if (task is null)
      {
         _console.WriteError("Task not found");
         return;
      }

      if (input.ManualMode) { TaskCommon.UpdateTaskData(task, input); }
      else { TaskCommon.UpdateTaskDataInteractive(task, input); }

      try
      {
         TaskCommon.ValidateModel(task);
      }
      catch (Exception ex)
      {
         _console.WriteError(ex.Message);
         return;
      }

      var success = _dbRepository.Tasks.Update(task);

      if (!success)
      {
         _console.WriteError("Error while updating task");
         return;
      }

      _console.MarkupLine("[green]Task updated successfully[/]");
      _console.WriteLine();
      TaskCommon.DisplayList(_dbRepository.Tasks.GetActive(), "Active tasks");
   }
}
