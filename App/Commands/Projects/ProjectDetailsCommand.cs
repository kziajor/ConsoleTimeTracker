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
      var idArgument = ProjectArguments.GetIdArgument();
      var manualModeOption = CommonOptions.GetManualModeOption();

      Add(idArgument);
      Add(manualModeOption);

      this.SetHandler((projectId, manualMode) => ShowProjectHandler(projectId, manualMode), idArgument, manualModeOption);
   }

   private void ShowProjectHandler(int projectId, bool manualMode)
   {
      Project? project = manualMode
         ? _dbRepository.Projects.Get(projectId)
         : ProjectCommon.GetOrChoose(projectId);

      if (project is null)
      {
         _console.MarkupLineInterpolated($"Project with id {projectId} not found");
         return;
      }

      ProjectCommon.ShowProjectDetails(project);
   }
}
