using App.Entities;
using App.Extensions;
using App.Repositories;

using Spectre.Console;

namespace App.Commands.Projects.Common
{
   public static class ProjectCommon
   {
      public static void DisplayProjectsList(IEnumerable<Project> projects, string header = "Projects")
      {
         var console = ServicesProvider.GetInstance<IAnsiConsole>();
         var table = new Table
         {
            Border = TableBorder.Rounded
         }
            .AddColumn(new TableColumn("[green]Id[/]").LeftAligned())
            .AddColumn(new TableColumn("[green]Name[/]").LeftAligned())
            .AddColumn(new TableColumn("[green]Active[/]").Centered());

         foreach (var project in projects)
         {
            table.AddRow(project.PR_Id.ToString(), project.PR_Name, project.PR_Closed ? "" : "[green]X[/]");
         }

         console.MarkupLineInterpolated($"[green]{header}[/]");
         console.Write(table);
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
               .ChooseOne("Choose project", 20, (project) => project.GetOptionLabel());
         }

         return dbRepository.Projects.Get(projectId.Value);
      }

      public static void ValidateModel(Project project)
      {
         if (project.PR_Name.IsNullOrEmpty()) throw new InvalidDataException("Project name empty");
      }
   }
}
