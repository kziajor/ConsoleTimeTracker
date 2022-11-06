namespace App.Repositories;

public interface IDbRepository
{
   IProjectsRepository Projects { get; }
   ITasksRepository Tasks { get; }
}

public sealed class DbRepository : IDbRepository
{
   private readonly string _connectionsString;

   public IProjectsRepository Projects => new ProjectsRepository(_connectionsString);
   public ITasksRepository Tasks => new TasksRepository(_connectionsString);

   public DbRepository(string connectionsString)
   {
      _connectionsString = connectionsString;
   }
}
