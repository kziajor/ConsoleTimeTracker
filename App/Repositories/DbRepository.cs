namespace App.Repositories;

public interface IDbRepository
{
   IProjectsRepository Projects { get; }
}

public class DbRepository : IDbRepository
{
   private readonly string _connectionsString;

   private ProjectsRepository? projects;

   public IProjectsRepository Projects => projects ??= new ProjectsRepository(_connectionsString);

   public DbRepository(string connectionsString)
   {
      _connectionsString = connectionsString;
   }
}
