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

         Add(ProjectCommonOptions.Closed);

         this.SetHandler((closed) => ProjectsListHandle(closed), ProjectCommonOptions.Closed);
      }

      private void ProjectsListHandle(bool? closed)
      {
         var projects = closed ?? false
            ? _dbRepository.Projects.GetClosed()
            : _dbRepository.Projects.GetActive();

         ProjectCommon.DisplayProjectsList(projects.OrderByDescending(p => p.PR_Id), closed ?? false ? "Closed projects" : "Active projects");
      }
   }
}
