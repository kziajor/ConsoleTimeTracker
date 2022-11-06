using App.Entities;
using App.Extensions;
using App.Models.Filters;
using App.Repositories.Helpers;
using Dapper;
using Task = App.Entities.Task;

namespace App.Repositories;

public interface ITasksRepository
{
   Task? Insert(Task task);
   bool Update(Task task);
   Task? Get(int id);
   IEnumerable<Task> GetAll();
   IEnumerable<Task> GetClosed();
   IEnumerable<Task> GetActive();
   IEnumerable<Task> GetFiltered(TaskFilters filters);

}

public sealed class TasksRepository : BaseRepository, ITasksRepository
{
   #region Queries

   private const string GetAllQuery =
      @"
         SELECT *
         FROM Tasks
         INNER JOIN Projects ON PR_Id = TA_RelProjectId
      ";
   private const string InsertQuery =
      @"
         INSERT INTO tasks
            (TA_Title, TA_PlannedTime, TA_Closed, TA_RelProjectId, TA_ExternalSystemType, TA_ExternalSystemTaskId)
         VALUES
            (@TA_Title, @TA_PlannedTime, @TA_Closed, @TA_RelProjectId, @TA_ExternalSystemType, @TA_ExternalSystemTaskId);
         SELECT last_insert_rowid();
      ";
   private const string UpdateQuery =
      @"
         UPDATE tasks
         SET
            TA_Title = @TA_Title,
            TA_PlannedTime = @TA_PlannedTime,
            TA_Closed = @TA_Closed,
            TA_RelProjectId = @TA_RelProjectId,
            TA_ExternalSystemType = @TA_ExternalSystemType,
            TA_ExternalSystemTaskId = @TA_ExternalSystemTaskId
         WHERE
            TA_Id = TA_Id
      ";
   private static string GetByIdQuery => $"{GetAllQuery} WHERE TA_Id = @TA_Id";
   private static string GetActiveQuery => $"{GetAllQuery} WHERE TA_Closed <= 0";
   private static string GetClosedQuery => $"{GetAllQuery} WHERE TA_Closed >= 1";

   #endregion

   public TasksRepository(string connectionString) : base(connectionString) { }

   public Task? Insert(Task task)
   {
      var result = Query((connection) => connection.ExecuteScalar<int>(InsertQuery, task));

      if (result == 0) { return null; }

      task.TA_Id = result;

      return task;
   }

   public bool Update(Task task)
   {
      return Query(connection => connection.Execute(UpdateQuery, task)) == 1;
   }

   public Task? Get(int id)
   {
      return Query((connection) => connection.Query<Task, Project, Task>(GetByIdQuery, (task, project) =>
      {
         task.Project = project;
         return task;
      },
      param: new { TA_Id = id }, splitOn: "PR_Id").FirstOrDefault());

   }

   public IEnumerable<Task> GetAll()
   {
      return Query((connection) => connection.Query<Task, Project, Task>(GetAllQuery, (task, project) =>
      {
         task.Project = project;
         return task;
      },
      splitOn: "PR_Id"));
   }

   public IEnumerable<Task> GetClosed()
   {
      return Query((connection) => connection.Query<Task, Project, Task>(GetClosedQuery, (task, project) =>
      {
         task.Project = project;
         return task;
      },
      splitOn: "PR_Id"));
   }

   public IEnumerable<Task> GetActive()
   {
      return Query((connection) => connection.Query<Task, Project, Task>(GetActiveQuery, (task, project) =>
      {
         task.Project = project;
         return task;
      },
      splitOn: "PR_Id"));
   }

   public IEnumerable<Task> GetFiltered(TaskFilters filters)
   {
      var query = GetAllQuery;
      var conditionBuilder = new QueryConditionBuilder();

      if (filters.TaskId is not null) { conditionBuilder.Add(ConditionOperators.AND, $"TA_ID = @{nameof(filters.TaskId)}"); }
      if (filters.ProjectId is not null) { conditionBuilder.Add(ConditionOperators.AND, $"TA_RelProjectId = @{nameof(filters.ProjectId)}"); }
      if (filters.Title is not null) { conditionBuilder.Add(ConditionOperators.AND, $"TA_Title LIKE '%@{nameof(filters.Title)}%'"); }
      if (filters.Closed is not null) { conditionBuilder.Add(ConditionOperators.AND, $"TA_Closed = @{nameof(filters.Closed)}"); }
      if (filters.ExternalSystemType is not null) { conditionBuilder.Add(ConditionOperators.AND, $"TA_ExternalSystemType = @{nameof(filters.ExternalSystemType)}"); }
      if (filters.ExternalSystemTaskId is not null) { conditionBuilder.Add(ConditionOperators.AND, $"TA_ExternalSystemTaskId LIKE '%@{nameof(filters.ExternalSystemTaskId)}%'"); }

      var conditions = conditionBuilder.ToString();

      if (conditions.IsNotNullOrEmpty()) { query += $" WHERE {conditions}"; }
      query += $" ORDER BY TA_ID DESC LIMIT {filters.Limit} OFFSET {filters.Skip}";

      return Query((connection) => connection.Query<Task, Project, Task>(query, (task, project) =>
      {
         task.Project = project;
         return task;
      },
      param: filters,
      splitOn: "PR_Id"));
   }
}