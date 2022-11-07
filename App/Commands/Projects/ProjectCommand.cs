using App.Commands.Projects.Common;
using App.Repositories;

using System.CommandLine;

namespace App.Commands.Projects
{
   public class ProjectCommand : Command
   {
      private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();

      public ProjectCommand() : base("project", "Manage projects")
      {
         AddAlias("p");

         Add(new ProjectAddCommand());
         Add(new ProjectEditCommand());
         Add(new ProjectDetailsCommand());

         var closedOption = ProjectOptions.GetClosedOption();
         Add(closedOption);

         this.SetHandler((closed) => ProjectsListHandle(closed), closedOption);
      }

      private void ProjectsListHandle(bool? closed)
      {
         var projects = closed ?? false
            ? _dbRepository.Projects.GetClosed()
            : _dbRepository.Projects.GetActive();

         ProjectCommon.DisplayProjectsList(projects, closed ?? false ? "Closed projects" : "Active projects");
      }
   }
}
