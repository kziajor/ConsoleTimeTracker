using App.Entities;
using App.Extensions;
using App.Repositories;

using Spectre.Console;

using System.CommandLine;

namespace App.Commands.Projects;

public class EditProjectCommand : Command
{
   private readonly IDbRepository _dbRepository;

   public EditProjectCommand(IDbRepository dbRepository) : base("edit", "Edit project")
   {
      _dbRepository = dbRepository;

      AddAlias("e");

      var projectIdArgument = new Argument<int>(
            name: "id",
            getDefaultValue: () => 0,
            description: "Project id"
         );

      var projectNameArgument = new Argument<string>(
               name: "name",
               getDefaultValue: () => string.Empty,
               description: "Project name");
      var projectActiveArgument = new Argument<bool?>(
            name: "active",
            getDefaultValue: () => null,
            description: "Project active"
         );

      Add(projectIdArgument);
      Add(projectNameArgument);
      Add(projectActiveArgument);

      this.SetHandler((projectId, projectName, projectActive) => EditProjectHandler(projectId, projectName, projectActive), projectIdArgument, projectNameArgument, projectActiveArgument);
   }

   private void EditProjectHandler(int projectId, string name, bool? active)
   {
      Project? project = ProjectCommon.GetOrChoose(_dbRepository, projectId);

      if (project is null)
      {
         AnsiConsole.MarkupLine("[red]Project not found[/]");
         return;
      }

      if (name.IsNullOrEmpty()) { name = CommandCommon.AskForWithEmptyAllowed<string>("Project name (leave empty if not changed):") ?? string.Empty; }

      active ??= CommandCommon.AskForYesNo("Project active");
      project.PR_Name = name.IsNullOrEmpty() ? project.PR_Name : name;
      project.PR_Closed = !active.Value;

      var success = _dbRepository.Projects.Update(project);

      if (!success)
      {
         AnsiConsole.MarkupLine("[red]Error while updating project[/]");
      }
      else
      {
         AnsiConsole.MarkupLine("[green]Project updated successfully[/]");
         AnsiConsole.WriteLine();
         ProjectCommon.DisplayProjectsList(_dbRepository.Projects.GetActive().OrderBy(p => p.PR_Id), "Active projects");
      }
   }
}
