using App.Assets;
using App.Commands.Projects.Common;
using App.Entities;
using App.Extensions;
using App.Integrations;
using App.Models.Dtos;
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
         var settingsProvider = ServicesProvider.GetInstance<ISettingsProvider>();
         var table = new Table();

         table.Border(TableBorder.Rounded);

         var secondaryIdHeader = settingsProvider.ExternalSystemPriority
            ? "[green]Internal id[/]"
            : "[green]External (system/id)[/]";
         table.AddColumn(new TableColumn("[green]Id[/]").LeftAligned());
         table.AddColumn(new TableColumn(secondaryIdHeader).LeftAligned());
         table.AddColumn(new TableColumn("[green]Project[/]").LeftAligned());
         table.AddColumn(new TableColumn("[green]Title[/]").LeftAligned());
         table.AddColumn(new TableColumn("[green]Time planned (H)[/]").RightAligned());
         table.AddColumn(new TableColumn("[green]Time spent (H)[/]").RightAligned());
         table.AddColumn(new TableColumn("[green]Active[/]").Centered());

         foreach (var task in tasks)
         {
            var primaryId = settingsProvider.ExternalSystemPriority
               ? task.ExternalFullId
               : task.TA_Id.ToString();
            var secondaryId = settingsProvider.ExternalSystemPriority
               ? task.TA_Id.ToString()
               : task.ExternalFullId;

            table.AddRow(
               primaryId,
               secondaryId,
               task.Project?.PR_Name ?? "",
               task.TA_Title,
               task.PlannedTimeInHours > 0 ? task.PlannedTimeInHours.ToString("0.00") : "-",
               task.SpentTimeInHours > 0 ? task.SpentTimeInHours.ToString("0.00") : "-",
               task.TA_Closed ? Icons.CHECK : Icons.CLOCK
            );
         }

         AnsiConsole.MarkupLineInterpolated($"[green]{header}[/]");
         AnsiConsole.Write(table);
      }

      public static void ShowTaskDetails(Task? task)
      {
         if (task is null) { return; }

         var console = ServicesProvider.GetInstance<IAnsiConsole>();
         var grid = new Grid()
            .AddColumn()
            .AddColumn();

         grid
            .AddKeyValueRow("Id", task.TA_Id.ToString())
            .AddKeyValueRow("External system", $"{task.TA_ExternalSystemType?.ToString() ?? "-"}/{task.TA_ExternalSystemTaskId ?? "-"}")
            .AddKeyValueRow("Name", task.TA_Title)
            .AddKeyValueRow("Active", task.TA_Closed ? "No" : "Yes")
            .AddKeyValueRow("Project", task.Project?.PR_Name ?? string.Empty)
            .AddKeyValueRow("Planned time", $"{task.PlannedTimeInHours} h")
            .AddKeyValueRow("Spent time", $"{task.SpentTimeInHours} h");

         console.Write(grid);
      }

      public static Task? Choose(IEnumerable<Task>? tasks = null)
      {
         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();
         return (tasks ?? dbRepository.Tasks.GetActive())
               .ChooseOne("Choose task", 20, (task) => task.GetOptionLabel());
      }

      public static Task? GetOrChoose(UniversalTaskId? universalTaskId, IEnumerable<Task>? tasks = null)
      {
         if (universalTaskId is null || universalTaskId.TaskId is null || universalTaskId.TaskId <= 0)
         {
            return Choose(tasks);
         }

         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();
         return dbRepository.Tasks.Get(universalTaskId);
      }

      public static Task? GetOrChoose(int? taskId = null, IEnumerable<Task>? tasks = null)
      {
         if (taskId is null || taskId <= 0)
         {
            return Choose(tasks);
         }

         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();
         return dbRepository.Tasks.Get(taskId.Value);
      }

      public static Task CreateTaskInteractive(TaskInput input)
      {
         if (input is null) throw new ArgumentNullException("Invalid task input");

         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();
         var console = ServicesProvider.GetInstance<IAnsiConsole>();
         var settingsProvider = ServicesProvider.GetInstance<ISettingsProvider>();

         var result = new Task
         {
            TA_Title = string.IsNullOrEmpty(input.Title)
            ? CommandCommon.AskFor<string>("Task title")
            : input.Title,
            TA_RelProjectId = input.ProjectId ?? dbRepository.Projects
               .GetActive()
               .ChooseOne("Choose project", 20, (p) => p.GetOptionLabel())
               ?.PR_Id ?? 0,
            PlannedTimeInHours = input.PlannedTime ?? CommandCommon.AskFor<decimal>("Planned time (H)"),
            TA_Closed = input.Closed ?? console.Confirm("Task closed", false),
            TA_ExternalSystemType = input.ExternalSystemType ?? CommandCommon.AskForWithEmptyAllowed<ExternalSystemEnum?>("External system", settingsProvider.ExternalSystemPriority ? settingsProvider.ExternalSystemDefaultType : null), // TODO: Change to list of enums with option to set empty value
         };

         if (result.TA_ExternalSystemType is not null)
         {
            result.TA_ExternalSystemTaskId = string.IsNullOrEmpty(input.ExternalSystemTaskId)
               ? CommandCommon.AskFor<string>("External id")
               : input.ExternalSystemTaskId;
         }

         return result;
      }

      public static Task CreateTask(TaskInput input)
      {
         return new Task
         {
            TA_Title = input.Title ?? string.Empty,
            TA_RelProjectId = input.ProjectId ?? 0,
            PlannedTimeInHours = input.PlannedTime ?? 0.00M,
            TA_Closed = input.Closed ?? false,
            TA_ExternalSystemType = input.ExternalSystemType,
            TA_ExternalSystemTaskId = input.ExternalSystemType is not null ? input.ExternalSystemTaskId : null,
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
         task.PlannedTimeInHours = input.PlannedTime ?? CommandCommon.AskForWithEmptyAllowed<decimal?>("Planned time", task.PlannedTimeInHours) ?? task.PlannedTimeInHours;
         task.TA_ExternalSystemType = input.ExternalSystemType ?? CommandCommon.AskForWithEmptyAllowed<ExternalSystemEnum?>("External system", task.TA_ExternalSystemType); // TODO: Change to list of enums with option to set empty value

         if (task.TA_ExternalSystemType is not null)
         {
            task.TA_ExternalSystemTaskId = input.ExternalSystemTaskId ?? CommandCommon.AskForWithEmptyAllowed("External id", task.TA_ExternalSystemTaskId);
         }
      }

      public static void UpdateTaskData(Task task, TaskInput input)
      {
         task.TA_Title = string.IsNullOrEmpty(input.Title) ? task.TA_Title : input.Title;
         task.TA_RelProjectId = input.ProjectId ?? task.TA_RelProjectId;
         task.TA_Closed = input.Closed ?? task.TA_Closed;
         task.PlannedTimeInHours = input.PlannedTime ?? task.PlannedTimeInHours;
         task.TA_ExternalSystemType = input.ExternalSystemType ?? task.TA_ExternalSystemType;
         task.TA_ExternalSystemTaskId = task.TA_ExternalSystemType is not null ? input.ExternalSystemTaskId ?? task.TA_ExternalSystemTaskId : null;
      }

      public static void ValidateModel(Task task)
      {
         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();

         if (task.TA_Title.IsNullOrEmpty()) throw new Exception("Title is empty");
         if (task.TA_RelProjectId <= 0) throw new ArgumentOutOfRangeException("Project id out of range");
         if (task.TA_PlannedTime < 0) throw new ArgumentOutOfRangeException("Planned time less then 0");
         if (task.TA_ExternalSystemType is not null && task.TA_ExternalSystemTaskId is null) throw new ArgumentException("External system was provided withoud external task id");
         if (task.TA_ExternalSystemType is not null
            && dbRepository.Tasks.ExternalTaskIdExists(task.TA_ExternalSystemType.Value, task.TA_ExternalSystemTaskId!, task.TA_Id))
            throw new ArgumentException($"Task with external id {task.TA_ExternalSystemTaskId} in system {task.TA_ExternalSystemType} already exists");
      }
   }
}
