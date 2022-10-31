using App.Commands.Projects;
using App.Extensions;
using App.Repositories;
using Spectre.Console;
using System.CommandLine;
using Task = App.Entities.Task;

namespace App.Commands.Tasks
{
   public class AddTaskCommand : Command
   {
      private readonly IDbRepository _dbRepository;

      public AddTaskCommand(IDbRepository dbRepository) : base("add", "Add new task")
      {
         _dbRepository = dbRepository;

         AddAlias("a");

         var taskTitleArgument = new Argument<string>(
            name: "title",
            getDefaultValue: () => string.Empty,
            description: "Task title");
         var taskProjectId = new Argument<int>(
            name: "project-id",
            getDefaultValue: () => 0,
            description: "Project id");
         var taskPlannedTimeArgument = new Argument<int>(
            name: "planned-time",
            getDefaultValue: () => 0,
            description: "Planned time in minutes");

         Add(taskTitleArgument);
         Add(taskProjectId);
         Add(taskPlannedTimeArgument);
         this.SetHandler((titleArgument, projectIdArgument, plannedTimeArgument) =>
            AddTaskHandler(titleArgument, projectIdArgument, plannedTimeArgument), taskTitleArgument, taskProjectId, taskPlannedTimeArgument);
      }

      private void AddTaskHandler(string taskTitle, int projectId, int plannedTime)
      {
         if (taskTitle.IsNullOrEmpty()) { taskTitle = CommandCommon.AskFor<string>("Task title"); }
         if (projectId <= 0)
         {
            projectId = _dbRepository.Projects
               .GetActive()
               .ChooseOne("Choose project", 20, (p) => p.GetOptionLabel())
               ?.id ?? 0;
         }
         if (plannedTime <= 0) { plannedTime = CommandCommon.AskFor<int>("Planned time in minutes"); }

         var result = _dbRepository.Tasks.Add(new Task
         {
            title = taskTitle,
            rel_project_id = projectId,
            planned_time = plannedTime,
            closed = false,
         });

         if (result is null)
         {
            AnsiConsole.WriteException(new Exception("Error while adding new project to database"));
            return;
         }

         AnsiConsole.MarkupLine("[green]New task added[/]");
         AnsiConsole.WriteLine();
         TaskCommon.DisplayTasksList(_dbRepository.Tasks.GetActive().OrderBy(t => t.id), "Active tasks");
      }
   }
}
