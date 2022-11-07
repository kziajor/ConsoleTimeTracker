using App.Commands.Projects.Common;
using App.Entities;
using App.Extensions;
using App.Models.Inputs;
using App.Repositories;

using Spectre.Console;

using Task = App.Entities.Task;

namespace App.Commands.Tasks.Common
{
   public static class TaskCommon
   {
      public static void DisplayTasksList(IEnumerable<Task> tasks, string header = "Tasks")
      {
         var table = new Table();

         table.Border(TableBorder.Rounded);

         table.AddColumn(new TableColumn("[green]Id[/]").LeftAligned());
         table.AddColumn(new TableColumn("[green]Project[/]").LeftAligned());
         table.AddColumn(new TableColumn("[green]Title[/]").LeftAligned());
         table.AddColumn(new TableColumn("[green]Time planned H (M)[/]").LeftAligned());
         table.AddColumn(new TableColumn("[green]Time spent H (M)[/]").LeftAligned());
         table.AddColumn(new TableColumn("[green]Active[/]").Centered());

         foreach (var task in tasks)
         {
            var timePlannedHours = Math.Round((double)task.TA_PlannedTime / 60, 2);
            var timeSpentHours = Math.Round((double)task.TA_SpentTime / 60, 2);

            table.AddRow(
               task.TA_Id.ToString(),
               task.Project?.PR_Name ?? "",
               task.TA_Title, 
               $"{timePlannedHours} ({task.TA_PlannedTime})",
               $"{timeSpentHours} ({task.TA_SpentTime})",
               task.TA_Closed ? "" : "[green]X[/]"
            );
         }

         AnsiConsole.MarkupLineInterpolated($"[green]{header}[/]");
         AnsiConsole.Write(table);
      }

      public static void ShowTaskDetails(Task task)
      {
         var console = ServicesProvider.GetInstance<IAnsiConsole>();
         var timePlannedHours = Math.Round((double)task.TA_PlannedTime / 60, 2);
         var timeSpentHours = Math.Round((double)task.TA_SpentTime / 60, 2);

         var grid = new Grid()
            .AddColumn()
            .AddColumn();

         grid
            .AddKeyValueRow("Id", task.TA_Id.ToString())
            .AddKeyValueRow("Name", task.TA_Title)
            .AddKeyValueRow("Active", task.TA_Closed ? "No" : "Yes")
            .AddKeyValueRow("Project", task.Project?.PR_Name ?? string.Empty)
            .AddKeyValueRow("Planned time", $"{timePlannedHours} h ({task.TA_PlannedTime} m)")
            .AddKeyValueRow("Spent time", $"{timeSpentHours} h ({task.TA_SpentTime} m)");

         console.Write(grid);
      }

      public static Task? GetOrChoose(int? taskId = null, IEnumerable<Task>? tasks = null)
      {
         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();

         if (taskId is null || taskId <= 0)
         {
            return (tasks ?? dbRepository.Tasks.GetActive())
               .ChooseOne("Choose task", 20, (task) => task.GetOptionLabel());
         }

         return dbRepository.Tasks.Get(taskId.Value);
      }

      public static Task CreateTaskInteractive(TaskInput input)
      {
         if (input is null) throw new ArgumentNullException("Invalid task input");

         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();
         var console = ServicesProvider.GetInstance<IAnsiConsole>();

         return new Task
         {
            TA_Title = string.IsNullOrEmpty(input.Title)
            ? CommandCommon.AskFor<string>("Task title")
            : input.Title,
            TA_RelProjectId = input.ProjectId ?? dbRepository.Projects
               .GetActive()
               .ChooseOne("Choose project", 20, (p) => p.GetOptionLabel())
               ?.PR_Id ?? 0,
            TA_PlannedTime = input.PlannedTime ?? CommandCommon.AskFor<int>("Planned time in minutes"),
            TA_Closed = input.Closed ?? console.Confirm("Task closed", false),
         };
      }

      public static Task CreateTask(TaskInput input)
      {
         return new Task
         {
            TA_Title = input.Title ?? string.Empty,
            TA_RelProjectId = input.ProjectId ?? 0,
            TA_PlannedTime = input.PlannedTime ?? 0,
            TA_Closed = input.Closed ?? false,
         };
      }

      public static void UpdateTaskDataInteractive(Task task, TaskInput input)
      {
         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();
         var console = ServicesProvider.GetInstance<IAnsiConsole>();

         task.TA_Title = string.IsNullOrEmpty(input.Title)
            ? CommandCommon.AskForWithEmptyAllowed("Task title", task.TA_Title) ?? string.Empty
            : input.Title;

         task.TA_RelProjectId = input.ProjectId ?? dbRepository.Projects
            .GetActive()
            .Where(p => p.PR_Id != task.TA_RelProjectId)
            .Prepend(new Project() { PR_Id = task.TA_RelProjectId, PR_Name = $"[current] {task.Project?.PR_Name}", PR_Closed = task.Project?.PR_Closed ?? false })
            .ChooseOne("Choose project", 20, p => p.GetOptionLabel()).PR_Id;

         task.TA_Closed = input.Closed ?? console.Confirm("Task closed", task.TA_Closed);
         task.TA_PlannedTime = input.PlannedTime ?? CommandCommon.AskForWithEmptyAllowed<int?>("Planned time", task.TA_PlannedTime) ?? task.TA_PlannedTime;
      }

      public static void UpdateTaskData(Task task, TaskInput input)
      {
         task.TA_Title = string.IsNullOrEmpty(input.Title) ? task.TA_Title : input.Title;
         task.TA_RelProjectId = input.ProjectId ?? task.TA_RelProjectId;
         task.TA_Closed = input.Closed ?? task.TA_Closed;
         task.TA_PlannedTime = input.PlannedTime ?? task.TA_PlannedTime;
      }

      public static void ValidateModel(Task task)
      {
         if (task.TA_Title.IsNullOrEmpty()) throw new Exception("Title is empty");
         if (task.TA_RelProjectId <= 0) throw new ArgumentOutOfRangeException("Project id out of range");
         if (task.TA_PlannedTime < 0) throw new ArgumentOutOfRangeException("Planned time less then 0");
      }
   }
}
