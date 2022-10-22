using Cli.Entities;
using Dapper;

namespace App.Repositories;

public class ProjectsRepository : DbRepository
{
   public ProjectsRepository(string connectionString) : base(connectionString)
   {
   }

   public IEnumerable<Project> GetProjects()
   {
      const string query = "SELECT * FROM projects";
      return Query((connection) => connection.Query<Project>(query));
   }
}