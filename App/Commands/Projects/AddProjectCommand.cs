using App.Entities;
using App.Extensions;
using App.Repositories;

using Spectre.Console;

using System.CommandLine;

namespace App.Commands.Projects
{
   public class AddProjectCommand : Command
   {
      private readonly IDbRepository _dbRepository;

      public AddProjectCommand(IDbRepository dbRepository) : base("add", "Add new project")
      {
         _dbRepository = dbRepository;

         AddAlias("a");

         var projectNameArgument = new Argument<string>(
               name: "name",
               getDefaultValue: () => string.Empty,
               description: "Project name");

         Add(projectNameArgument);
         this.SetHandler((nameArgument) => AddProjectHandler(nameArgument), projectNameArgument);
      }

      private void AddProjectHandler(string projectName)
      {
         if (projectName.IsNullOrEmpty()) { projectName = ProjectCommon.AskForProjectName(); }

         var result = _dbRepository.Projects.Add(new Project { Name = projectName });

         if (result is null)
         {
            AnsiConsole.WriteException(new Exception("Error while adding new project to database"));
            return;
         }

         AnsiConsole.MarkupLine("[green]New project added[/]");

         var projects = _dbRepository.Projects.GetActive();

         ProjectCommon.DisplayProjectsList(projects, "Active projects");
      }
   }
}
