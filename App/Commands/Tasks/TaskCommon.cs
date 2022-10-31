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

         foreach (var task in tasks.OrderBy(t => t.id))
         {
            table.AddRow(task.id.ToString(), task.Project?.name ?? "", task.title, task.planned_time.ToString(), task.closed ? "" : "[green]X[/]");
         }

         AnsiConsole.MarkupLineInterpolated($"[green]{header}[/]");
         AnsiConsole.Write(table);
      }

      public static void ShowTaskDetails(Task task)
      {
         AnsiConsole.Console.WriteKeyValuePair("Id", task.id.ToString());
         AnsiConsole.Console.WriteKeyValuePair("Name", task.title);
         AnsiConsole.Console.WriteKeyValuePair("Active", task.closed ? "No" : "Yes");
         AnsiConsole.Console.WriteKeyValuePair("Project", task.Project?.name ?? string.Empty);
         AnsiConsole.Console.WriteKeyValuePair("Planned time", $"{task.planned_time} minutes");
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
