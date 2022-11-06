using App.Commands.Projects.Common;
using App.Entities;
using App.Extensions;
using App.Models.Inputs;
using App.Repositories;

using Spectre.Console;

using System.CommandLine;

namespace App.Commands.Projects;

public class ProjectEditCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
   private readonly IAppConsole _console = ServicesProvider.GetInstance<IAppConsole>();

   public ProjectEditCommand() : base("edit", "Edit project")
   {
      AddAlias("e");

      Add(ProjectCommonArguments.Id);
      Add(ProjectCommonOptions.Name);
      Add(ProjectCommonOptions.Closed);
      Add(CommandCommonOptions.InteractiveMode);

      this.SetHandler(
         (projectInput) => EditProjectHandler(projectInput),
         new ProjectInputBinder(
            ProjectCommonArguments.Id,
            ProjectCommonOptions.Name,
            ProjectCommonOptions.Closed,
            CommandCommonOptions.InteractiveMode));
   }

   private void EditProjectHandler(ProjectInput input)
   {
      if (input.Id <= 0 && !input.InteractiveMode)
      {
         _console.WriteError($"Project id {input.Id} is not valid");
         return;
      }

      Project? project = input.InteractiveMode
         ? ProjectCommon.GetOrChoose(input.Id)
         : _dbRepository.Projects.Get(input.Id);

      if (project is null)
      {
         _console.MarkupLine("[red]Project not found[/]");
         return;
      }

      try
      {
         if (input.InteractiveMode) { Interactive(project, input.Name, input.Closed); }
         else { ManualMode(project, input.Name, input.Closed); }

         ProjectCommon.ValidateModel(project);
      }
      catch (Exception ex)
      {
         _console.WriteError(ex.Message);
         return;
      }

      var success = _dbRepository.Projects.Update(project);

      if (!success)
      {
         _console.MarkupLine("[red]Error while updating project[/]");
      }
      else
      {
         _console.MarkupLine("[green]Project updated successfully[/]");
         _console.WriteLine();
         ProjectCommon.DisplayProjectsList(_dbRepository.Projects.GetActive().OrderByDescending(p => p.PR_Id), "Active projects");
      }
   }

   private static void ManualMode(Project project, string? name, bool? closed)
   {
      project.PR_Name = name ?? project.PR_Name;
      project.PR_Closed = closed ?? project.PR_Closed;
   }

   private void Interactive(Project project, string? name, bool? closed)
   {
      project.PR_Name = name ?? CommandCommon.AskForWithEmptyAllowed("Project name", project.PR_Name) ?? string.Empty;
      project.PR_Closed = closed ?? _console.Confirm("Is closed", project.PR_Closed);
   }
}
