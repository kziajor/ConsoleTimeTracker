using App.Repositories;

using System.CommandLine;

namespace App.Commands.Projects
{
   public class ProjectCommand : Command
   {
      private readonly IDbRepository _dbRepository;
      public ProjectCommand(IDbRepository dbRepository) : base("project", "Manage projects")
      {
         _dbRepository = dbRepository;

         AddAlias("p");

         Add(new AddProjectCommand(_dbRepository));
         Add(new EditProjectCommand(_dbRepository));

         var closedOption = new Option<bool>(
               name: "--closed",
               getDefaultValue: () => false,
               description: "Get closed projects"
            );
         closedOption.AddAlias("-c");

         Add(closedOption);

         this.SetHandler((closed) => ProjectsListHandle(closed), closedOption);
      }

      private void ProjectsListHandle(bool closed)
      {
         var projects = closed
            ? _dbRepository.Projects.GetClosed()
            : _dbRepository.Projects.GetActive();

         ProjectCommon.DisplayProjectsList(projects, closed ? "Closed projects" : "Active projects");
      }
   }
}
