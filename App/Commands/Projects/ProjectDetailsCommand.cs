using App.Commands.Projects.Common;
using App.Entities;
using App.Repositories;

using Spectre.Console;

using System.CommandLine;

namespace App.Commands.Projects;

public class ProjectDetailsCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
   private readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();

   public ProjectDetailsCommand() : base("details", "Show project details")
   {
      AddAlias("d");

      var idArgument = ProjectArguments.GetIdArgument();
      var interactiveModeOption = CommonOptions.GetInteractiveModeOption();

      Add(idArgument);
      Add(interactiveModeOption);

      this.SetHandler((projectId, interactiveMode) => ShowProjectHandler(projectId, interactiveMode), idArgument, interactiveModeOption);
   }

   private void ShowProjectHandler(int projectId, bool interactiveMode)
   {
      Project? project = interactiveMode
         ? ProjectCommon.GetOrChoose(projectId)
         : _dbRepository.Projects.Get(projectId);

      if (project is null)
      {
         _console.MarkupLineInterpolated($"Project with id {projectId} not found");
         return;
      }

      ProjectCommon.ShowProjectDetails(project);
   }
}
