using App.Entities;
using Dapper.Contrib.Extensions;

namespace App.Repositories;

public interface IProjectsRepository
{
   Project Get(int id);
   IEnumerable<Project> GetAll();
   Project? Add(Project project);
   bool Update(Project project);
}

public class ProjectsRepository : BaseRepository, IProjectsRepository
{
   public ProjectsRepository(string connectionString) : base(connectionString)
   {
   }

   public Project Get(int id)
   {
      return Query((connection) => connection.Get<Project>(id));
   }

   public IEnumerable<Project> GetAll()
   {
      return Query((connection) => connection.GetAll<Project>());
   }

   public Project? Add(Project project)
   {
      var result = Query((connection) => connection.Insert(project));

      if (result == 0) { return null; }

      project.Id = Convert.ToInt32(result);

      return project;
   }

   public bool Update(Project project)
   {
      return Query(connection => connection.Update(project));
   }
}