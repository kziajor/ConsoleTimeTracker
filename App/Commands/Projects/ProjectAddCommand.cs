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
      private readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();

      public ProjectAddCommand() : base("add", "Add new project")
      {
         AddAlias("a");

         var nameOption = ProjectOptions.GetNameOption();
         var closedOption = ProjectOptions.GetClosedOption();
         var interactiveMode = CommonOptions.GetInteractiveModeOption();

         Add(nameOption);
         Add(closedOption);
         Add(interactiveMode);

         this.SetHandler((projectInput) => AddProjectHandler(projectInput),
            new ProjectInputBinder(
               name: nameOption,
               closed: closedOption,
               interactiveMode: interactiveMode));
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
         ProjectCommon.DisplayProjectsList(_dbRepository.Projects.GetActive(), "Active projects");
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
