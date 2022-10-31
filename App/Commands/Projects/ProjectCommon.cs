using App.Entities;
using App.Extensions;
using App.Repositories;
using Spectre.Console;

namespace App.Commands.Projects
{
   public static class ProjectCommon
   {
      public static void DisplayProjectsList(IEnumerable<Project> projects, string header = "Projects")
      {
         var table = new Table();

         table.Border(TableBorder.Rounded);

         table.AddColumn(new TableColumn("[green]Id[/]").LeftAligned());
         table.AddColumn(new TableColumn("[green]Name[/]").LeftAligned());
         table.AddColumn(new TableColumn("[green]Active[/]").Centered());

         foreach (var project in projects)
         {
            table.AddRow(project.id.ToString(), project.name, project.closed ? "" : "[green]X[/]");
         }

         AnsiConsole.MarkupLineInterpolated($"[green]{header}[/]");
         AnsiConsole.Write(table);
      }

      public static void ShowProjectDetails(Project project)
      {
         AnsiConsole.Console.WriteKeyValuePair("Id", project.id.ToString());
         AnsiConsole.Console.WriteKeyValuePair("Name", project.name);
         AnsiConsole.Console.WriteKeyValuePair("Active", project.closed ? "No" : "Yes");
      }

      public static Project? GetOrChoose(IDbRepository dbRepository, int? projectId = null, IEnumerable<Project>? projects = null)
      {
         if (projectId is null || projectId <= 0)
         {
            return (projects ?? dbRepository.Projects.GetActive())
               .ChooseOne("Choose project", 20, (project) => project.GetOptionLabel());
         }

         return dbRepository.Projects.Get(projectId.Value);
      }
   }
}
