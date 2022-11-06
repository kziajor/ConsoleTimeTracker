namespace App.Repositories;

public interface IDbRepository
{
   IProjectsRepository Projects { get; }
   ITasksRepository Tasks { get; }
}

public sealed class DbRepository : IDbRepository
{
   private readonly string _connectionsString = string.Empty;

   private IProjectsRepository? _projectsRepository;
   private ITasksRepository? _tasksRepository;
   private IRecordsRepository? _recordsRepository;

   public IProjectsRepository Projects => _projectsRepository ??= new ProjectsRepository(_connectionsString);
   public ITasksRepository Tasks => _tasksRepository ??= new TasksRepository(_connectionsString);
   public IRecordsRepository Records => _recordsRepository ??= new RecordsRepository(_connectionsString);

   public DbRepository() { }
   public ITasksRepository Tasks => new TasksRepository(_connectionsString);

   public DbRepository(string connectionsString)
   {
      _connectionsString = connectionsString;
   }
}
