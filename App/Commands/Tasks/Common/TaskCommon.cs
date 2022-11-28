using App.Assets;
using App.Commands.Projects.Common;
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
      private static readonly ISettingsProvider _settingsProvider = ServicesProvider.GetInstance<ISettingsProvider>();
      private static readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();
      private static readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();

      public static void DisplayList(IEnumerable<Task> tasks, string header = "Tasks")
      {
         var table = new Table();

         table.Border(TableBorder.Rounded);
         table
            .AddColumn(new TableColumn("[green]Id[/]").LeftAligned())
            .AddColumn(new TableColumn("[green]Project[/]").LeftAligned())
            .AddColumn(new TableColumn("[green]Title[/]").LeftAligned())
            .AddColumn(new TableColumn("[green]Time planned (H)[/]").RightAligned())
            .AddColumn(new TableColumn("[green]Time spent (H)[/]").RightAligned())
            .AddColumn(new TableColumn("[green]Active[/]").Centered());

         foreach (var task in tasks)
         {
            table.AddRow(
               task.UniversalTaskId.ToString(),
               task.TA_Project?.PR_Name ?? "",
               task.TA_Title,
               task.TimePlannedHours > 0 ? task.TimePlannedHours.ToString("0.00") : "-",
               task.TimeSpentHours > 0 ? task.TimeSpentHours.ToString("0.00") : "-",
               task.TA_Closed ? Icons.CHECK : Icons.CLOCK
            );
         }

         _console.MarkupLineInterpolated($"[green]{header}[/]");
         _console.Write(table);
      }

      public static void DisplaySummary(IEnumerable<Task> tasks, int totalTime, string header = "Tasks summary")
      {
         var table = new Table { Border = TableBorder.Rounded }
         .AddColumn(new TableColumn($"[{Colors.Primary}]Id[/]").LeftAligned())
         .AddColumn(new TableColumn($"[{Colors.Primary}]Title[/]").LeftAligned())
         .AddColumn(new TableColumn($"[{Colors.Primary}]Time spent[/]").RightAligned())
         .AddColumn(new TableColumn($"[{Colors.Primary}]Percentage[/]").RightAligned());

         foreach (var task in tasks)
         {
            double percentage = (double)task.TA_SpentTime / totalTime;

            table.AddRow(
               new Text(task.TA_Id.ToString()),
               new Text(task.TA_Title),
               new Text(task.TimeSpentHours > 0 ? task.TimeSpentHours.ToString("0.00") : "-").RightAligned(),
               new Text(percentage > 0 ? percentage.ToString("0%") : "0%").RightAligned()
            );
         }

         _console.MarkupLineInterpolated($"[green]{header}[/]");
         _console.Write(table);
         _console.MarkupLineInterpolated($"Total time spent: [{Colors.Primary} bold]{totalTime.MinutesToHours():0.00} h[/]");
      }

      public static void ShowDetails(Task? task)
      {

         if (task is null)
         {
            _console.WriteError("Task not found");
            return;
         }

         var grid = new Grid()
            .AddColumn()
            .AddColumn();

         grid
            .AddKeyValueRow("Id", task.UniversalTaskId.ToString())
            .AddKeyValueRow("Source system", task.TA_SourceType.ToString())
            .AddKeyValueRow("Name", task.TA_Title)
            .AddKeyValueRow("Active", task.TA_Closed ? "No" : "Yes")
            .AddKeyValueRow("Project", task.TA_Project?.PR_Name ?? string.Empty)
            .AddKeyValueRow("Planned time", $"{task.TimePlannedHours} h")
            .AddKeyValueRow("Spent time", $"{task.TimeSpentHours} h");

         _console.Write(grid);
      }

      public static Task? Choose(IEnumerable<Task>? tasks = null)
      {
         return (tasks ?? _dbRepository.Tasks.GetActive())
               .ChooseOne("Choose task", 20, optionNameConverter: (task) => task.GetOptionLabel() ?? task.TA_Title);
      }

      public static Task? GetOrChoose(UniversalTaskId universalTaskId, IEnumerable<Task>? tasks = null)
      {
         if (universalTaskId.IsEmpty)
         {
            tasks ??= _dbRepository.Tasks.GetActive();
            return Choose(tasks);
         }

         return _dbRepository.Tasks.Get(universalTaskId);
      }

      public static Task? GetOrChoose(int? taskId = null, IEnumerable<Task>? tasks = null)
      {
         if (taskId is null || taskId <= 0)
         {
            tasks ??= _dbRepository.Tasks.GetActive();
            return Choose(tasks);
         }

         return _dbRepository.Tasks.Get(taskId.Value);
      }

      public static Task CreateTaskInteractive(TaskInput input)
      {
         if (input is null) throw new ArgumentNullException("Invalid task input");

         var result = new Task
         {
            TA_Title = string.IsNullOrEmpty(input.Title)
            ? CommandCommon.AskFor<string>("Task title")
            : input.Title,
            TA_RelProjectId = input.ProjectId ?? _dbRepository.Projects
               .GetActive()
               .ChooseOne("Choose project", optionNameConverter: p => p.GetOptionLabel())
               ?.PR_Id ?? 0,
            TimePlannedHours = input.PlannedTime ?? CommandCommon.AskFor<decimal>("Planned time (H)"),
            TA_Closed = input.Closed ?? _console.Confirm("Task closed", false),
            TA_SourceType = input.SourceType ?? Enum.GetValues<SourceSystemType>()
               .ToList()
               .MoveToTop(sst => sst == _settingsProvider.SourceSystemDefaultType)
               .ChooseOne("Source system", predicateDefaultOption: sst => sst == _settingsProvider.SourceSystemDefaultType)
         };

         if (result.TA_SourceType != SourceSystemType.Internal)
         {
            result.TA_SourceTaskId = string.IsNullOrEmpty(input.SourceTaskId)
               ? CommandCommon.AskFor<string>("Task ID in source system")
               : input.SourceTaskId;
         }

         return result;
      }

      public static Task CreateTask(TaskInput input)
      {
         return new Task
         {
            TA_Title = input.Title ?? string.Empty,
            TA_RelProjectId = input.ProjectId ?? 0,
            TimePlannedHours = input.PlannedTime ?? 0.00M,
            TA_Closed = input.Closed ?? false,
            TA_SourceType = input.SourceType ?? _settingsProvider.SourceSystemDefaultType,
            TA_SourceTaskId = string.IsNullOrEmpty(input.SourceTaskId) ? string.Empty : input.SourceTaskId,
         };
      }

      public static void UpdateTaskDataInteractive(Task task, TaskInput input)
      {
         task.TA_Title = string.IsNullOrEmpty(input.Title)
            ? CommandCommon.AskForWithEmptyAllowed("Task title", task.TA_Title) ?? string.Empty
            : input.Title;

         task.TA_RelProjectId = input.ProjectId ?? _dbRepository.Projects
            .GetActive()
            .OrderBy(p => p.PR_Name)
            .ToList()
            .MoveToTop(p => p.PR_Id == task.TA_RelProjectId)
            .ChooseOne(
               "Choose project",
               predicateDefaultOption: p => p.PR_Id == task.TA_RelProjectId,
               optionNameConverter: p => p.GetOptionLabel())
            ?.PR_Id ?? task.TA_RelProjectId;

         task.TA_Closed = input.Closed ?? _console.Confirm("Task closed", task.TA_Closed);
         task.TimePlannedHours = input.PlannedTime
            ?? CommandCommon.AskForWithEmptyAllowed<decimal?>("Planned time", task.TimePlannedHours)
            ?? task.TimePlannedHours;
         task.TA_SourceType = input.SourceType ?? Enum.GetValues<SourceSystemType>()
               .ToList()
               .MoveToTop(sst => sst == task.TA_SourceType)
               .ChooseOne("Source system", predicateDefaultOption: sst => sst == task.TA_SourceType);

         if (task.TA_SourceType != SourceSystemType.Internal)
         {
            task.TA_SourceTaskId = string.IsNullOrEmpty(input.SourceTaskId)
               ? CommandCommon.AskForWithEmptyAllowed("Task ID in source system", task.TA_SourceTaskId)
               : input.SourceTaskId;
         }
      }

      public static void UpdateTaskData(Task task, TaskInput input)
      {
         task.TA_Title = string.IsNullOrEmpty(input.Title) ? task.TA_Title : input.Title;
         task.TA_RelProjectId = input.ProjectId ?? task.TA_RelProjectId;
         task.TA_Closed = input.Closed ?? task.TA_Closed;
         task.TimePlannedHours = input.PlannedTime ?? task.TimePlannedHours;
         task.TA_SourceType = input.SourceType ?? task.TA_SourceType;
         task.TA_SourceTaskId = task.TA_SourceType != SourceSystemType.Internal
            ? string.IsNullOrEmpty(input.SourceTaskId) ? task.TA_SourceTaskId : input.SourceTaskId
            : string.Empty;
      }

      public static void ValidateModel(Task task)
      {
         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();

         if (task.TA_Title.IsNullOrEmpty()) throw new Exception("Title is empty");
         if (task.TA_RelProjectId <= 0) throw new ArgumentOutOfRangeException("Project id out of range");
         if (task.TA_PlannedTime < 0) throw new ArgumentOutOfRangeException("Planned time less then 0");
         if (task.TA_SourceType != SourceSystemType.Internal && dbRepository.Tasks.SourceTaskIdExists(task.TA_SourceType, task.TA_SourceTaskId, task.TA_Id))
            throw new ArgumentException($"Task with id {task.UniversalTaskId} already exists in '{task.TA_SourceType}' system");
      }
   }
}
