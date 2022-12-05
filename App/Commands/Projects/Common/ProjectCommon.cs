using App.Assets;
using App.Entities;
using App.Extensions;
using App.Repositories;

using Spectre.Console;

namespace App.Commands.Projects.Common
{
   public static class ProjectCommon
   {
      private static readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();

      public static void DisplayList(IEnumerable<Project> projects, string header = "Projects")
      {
         var table = new Table { Border = TableBorder.Rounded }
            .AddColumn(new TableColumn($"[{Colors.Primary}]Id[/]").LeftAligned())
            .AddColumn(new TableColumn($"[{Colors.Primary}]Name[/]").LeftAligned())
            .AddColumn(new TableColumn($"[{Colors.Primary}]Active[/]").Centered());

         foreach (var project in projects)
         {
            table.AddRow(project.PR_Id.ToString(), project.PR_Name.EscapeMarkup(), project.PR_Closed ? "" : "[green]X[/]");
         }

         _console.MarkupLineInterpolated($"[green]{header}[/]");
         _console.Write(table);
      }

      public static void DisplaySummary(IEnumerable<Project> projects, int totalTime, string header = "Projects summary")
      {
         var table = new Table { Border = TableBorder.Rounded }
            .AddColumn(new TableColumn($"[{Colors.Primary}]Id[/]").LeftAligned())
            .AddColumn(new TableColumn($"[{Colors.Primary}]Name[/]").LeftAligned())
            .AddColumn(new TableColumn($"[{Colors.Primary}]Time spent[/]").RightAligned())
            .AddColumn(new TableColumn($"[{Colors.Primary}]Percentage[/]").RightAligned());

         foreach (var project in projects)
         {
            double percentage = (double)project.PR_TimeSpent / totalTime;

            table.AddRow(
               new Text(project.PR_Id.ToString()),
               new Text(project.PR_Name),
               new Text(project.TimeSpentHours > 0 ? project.TimeSpentHours.ToString("0.00") : "-").RightAligned(),
               new Text(percentage > 0 ? percentage.ToString("0.00%") : "0.00%").RightAligned()
            );
         }

         _console.MarkupLineInterpolated($"[green]{header}[/]");
         _console.Write(table);
         _console.MarkupLineInterpolated($"Total time spent: [{Colors.Primary} bold]{totalTime.MinutesToHours():0.00} h[/]");
      }

      public static void ShowProjectDetails(Project project)
      {
         var console = ServicesProvider.GetInstance<IAnsiConsole>();

         var grid = new Grid()
            .AddColumn()
            .AddColumn();

         grid.AddKeyValueRow("Id", project.PR_Id.ToString());
         grid.AddKeyValueRow("Name", project.PR_Name);
         grid.AddKeyValueRow("Active", project.PR_Closed ? "No" : "Yes");

         console.Write(grid);
      }

      public static Project? GetOrChoose(int? projectId = null, IEnumerable<Project>? projects = null)
      {
         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();

         if (projectId is null || projectId <= 0)
         {
            return (projects ?? dbRepository.Projects.GetActive())
               .ChooseOne("Choose project", 20, optionNameConverter: (project) => project.GetOptionLabel());
         }

         return dbRepository.Projects.Get(projectId.Value);
      }

      public static void ValidateModel(Project project)
      {
         if (project.PR_Name.IsNullOrEmpty()) throw new InvalidDataException("Project name empty");
      }
   }
}
