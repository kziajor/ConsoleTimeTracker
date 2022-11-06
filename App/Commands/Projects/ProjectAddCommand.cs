using App.Commands.Projects.Common;
using App.Entities;
using App.Extensions;
using App.Models.Inputs;
using App.Repositories;

using Spectre.Console;

using System.CommandLine;

namespace App.Commands.Projects
{
   public sealed class ProjectAddCommand : Command
   {
      private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
      private readonly IAppConsole _console = ServicesProvider.GetInstance<IAppConsole>();

      public ProjectAddCommand() : base("add", "Add new project")
      {
         AddAlias("a");

         Add(ProjectCommonOptions.Name);
         Add(ProjectCommonOptions.Closed);
         Add(CommandCommonOptions.InteractiveMode);

         this.SetHandler((projectInput) => AddProjectHandler(projectInput),
            new ProjectInputBinder(
               name: ProjectCommonOptions.Name,
               closed: ProjectCommonOptions.Closed,
               interactiveMode: CommandCommonOptions.InteractiveMode));
      }

      private void AddProjectHandler(ProjectInput input)
      {
         var project = new Project();

         if (input.InteractiveMode) { Interactive(project, input.Name, input.Closed); }
         else { Manual(project, input.Name, input.Closed); }

         try
         {
            ProjectCommon.ValidateModel(project);
         }
         catch (Exception ex)
         {
            _console.WriteError(ex.Message);
            return;
         }

         var result = _dbRepository.Projects.Insert(project);

         if (result is null)
         {
            _console.WriteError("Error while adding new project to database");
            return;
         }

         _console.MarkupLine("[green]New project added[/]");
         _console.WriteLine();
         ProjectCommon.DisplayProjectsList(_dbRepository.Projects.GetActive().OrderByDescending(p => p.PR_Id), "Active projects");
      }

      private void Interactive(Project project, string? name, bool? closed)
      {
         project.PR_Name = !string.IsNullOrEmpty(name)
            ? name
            : CommandCommon.AskFor<string>("Project name");
         project.PR_Closed = closed ?? _console.Confirm("Project closed", false);
      }

      private static void Manual(Project project, string? name, bool? closed)
      {
         project.PR_Name = name ?? string.Empty;
         project.PR_Closed = closed ?? false;
      }

   }
}
