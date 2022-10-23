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
      var projects = _dbRepository.Projects.GetAll();

      if (!projects.Any())
      {
         AnsiConsole.MarkupLine("[red]Projects not found. You have to add one first.[/]");
         return;
      }

      Project? project;

      if (projectId <= 0)
      {
         project = ProjectCommon.AskForProjectId(projects);
      }
      else
      {
         project = projects.FirstOrDefault(p => p.Id == projectId);

         if (project == null)
         {
            AnsiConsole.MarkupLine($"[red]Project with id {projectId} not found.[/]");
            return;
         }
      }

      if (name.IsNullOrEmpty()) { name = ProjectCommon.AskForProjectName(true, "Project name (leave empty if not changed):"); }
      if (active is null) { active = ProjectCommon.AskIsProjectActive(); }

      project.Name = name.IsNullOrEmpty() ? project.Name : name;
      project.Closed = !active.Value;

      var success = _dbRepository.Projects.Update(project);

      if (!success)
      {
         AnsiConsole.MarkupLine("[red]Error while updating project[/]");
      }
      else
      {
         AnsiConsole.MarkupLine("[green]Project updated successfully[/]");
         ProjectCommon.DisplayProjectsList(projects);
      }
   }
}
