using App.Repositories;

using Spectre.Console;

using System.CommandLine;

namespace App.Commands.Projects;

public class ShowProjectCommand : Command
{
   private readonly IDbRepository _dbRepository;

   public ShowProjectCommand(IDbRepository dbRepository) : base("show", "Show project details")
   {
      _dbRepository = dbRepository;

      AddAlias("s");

      var projectIdArgument = new Argument<int>(
            name: "id",
            getDefaultValue: () => 0,
            description: "Project id"
         );

      Add(projectIdArgument);

      this.SetHandler(projectId => ShowProjectHandler(projectId), projectIdArgument);
   }

   private void ShowProjectHandler(int projectId)
   {
      var project = _dbRepository.Projects.Get(projectId);

      if (project is null)
      {
         AnsiConsole.MarkupLineInterpolated($"Project with id {projectId} not found");
         return;
      }

      ProjectCommon.ShowProjectDetails(project);
   }
}
