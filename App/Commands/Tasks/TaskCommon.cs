using App.Extensions;
using App.Repositories;
using Spectre.Console;
using Task = App.Entities.Task;

namespace App.Commands.Tasks
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
         table.AddColumn(new TableColumn("[green]Planned time (minutes)[/]").LeftAligned());
         table.AddColumn(new TableColumn("[green]Active[/]").Centered());

         foreach (var task in tasks.OrderBy(t => t.TA_Id))
         {
            table.AddRow(task.TA_Id.ToString(), task.Project?.PR_Name ?? "", task.TA_Title, task.TA_PlannedTime.ToString(), task.TA_Closed ? "" : "[green]X[/]");
         }

         AnsiConsole.MarkupLineInterpolated($"[green]{header}[/]");
         AnsiConsole.Write(table);
      }

      public static void ShowTaskDetails(Task task)
      {
         AnsiConsole.Console.WriteKeyValuePair("Id", task.TA_Id.ToString());
         AnsiConsole.Console.WriteKeyValuePair("Name", task.TA_Title);
         AnsiConsole.Console.WriteKeyValuePair("Active", task.TA_Closed ? "No" : "Yes");
         AnsiConsole.Console.WriteKeyValuePair("Project", task.Project?.PR_Name ?? string.Empty);
         AnsiConsole.Console.WriteKeyValuePair("Planned time", $"{task.TA_PlannedTime} minutes");
      }

      public static Task? GetOrChoose(IDbRepository dbRepository, int? taskId = null, IEnumerable<Task>? tasks = null)
      {
         if (taskId is null || taskId <= 0)
         {
            return (tasks ?? dbRepository.Tasks.GetActive())
               .ChooseOne("Choose task", 20, (task) => task.GetOptionLabel());
         }

         return dbRepository.Tasks.Get(taskId.Value);
      }
   }
}
