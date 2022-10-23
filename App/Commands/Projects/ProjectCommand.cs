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

         this.SetHandler(() => ProjectsListHandle());
      }

      private void ProjectsListHandle()
      {
         var projects = _dbRepository.Projects.GetAll();

         ProjectCommon.DisplayProjectsList(projects);
      }
   }
}
