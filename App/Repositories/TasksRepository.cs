using App.Entities;
using App.Repositories.Queries;

using Dapper;
using Dapper.Contrib.Extensions;
using Task = App.Entities.Task;

namespace App.Repositories;

public interface ITasksRepository
{
   Task? Get(int id);
   IEnumerable<Task> GetAll();
   IEnumerable<Task> GetClosed();
   IEnumerable<Task> GetActive();
   Task? Add(Task task);
   bool Update(Task task);
}

public class TasksRepository : BaseRepository, ITasksRepository
{
   public TasksRepository(string connectionString) : base(connectionString)
   {
   }

   public Task? Get(int id)
   {
      return Query((connection) => connection.Query<Task, Project, Task>(TaskQueries.TaskQuery, (task, project) =>
      {
         task.Project = project;
         return task;
      },
      splitOn: "Id", param: new { id }).FirstOrDefault());
   }

   public IEnumerable<Task> GetAll()
   {
      return Query((connection) => connection.Query<Task, Project, Task>(TaskQueries.AllTasksQuery, (task, project) =>
      {
         task.Project = project;
         return task;
      },
      splitOn: "Id"));
   }

   public IEnumerable<Task> GetClosed()
   {
      return Query((connection) => connection.Query<Task, Project, Task>(TaskQueries.ClosedTasksQuery, (task, project) =>
      {
         task.Project = project;
         return task;
      },
      splitOn: "Id"));
   }

   public IEnumerable<Task> GetActive()
   {
      return Query((connection) => connection.Query<Task, Project, Task>(TaskQueries.ActiveTasksQuery, (task, project) =>
      {
         task.Project = project;
         return task;
      },
      splitOn: "Id"));
   }

   public Task? Add(Task task)
   {
      var result = Query((connection) => connection.Insert(task));

      if (result == 0) { return null; }

      task.id = Convert.ToInt32(result);

      return task;
   }

   public bool Update(Task task)
   {
      return Query(connection => connection.Update(task));
   }
}