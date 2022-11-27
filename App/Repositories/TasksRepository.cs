using App.Entities;
using App.Extensions;
using App.Integrations;
using App.Models.Dtos;
using App.Models.Filters;
using App.Repositories.Helpers;

using Dapper;

using Task = App.Entities.Task;

namespace App.Repositories;

public interface ITasksRepository
{
   Task? Insert(Task task);
   bool Update(Task task);
   Task? Get(int? id);
   Task? Get(UniversalTaskId? id);
   IEnumerable<Task> GetAll(string? orderBy = null);
   IEnumerable<Task> GetClosed(string? orderBy = null);
   IEnumerable<Task> GetActive(string? orderBy = null);
   int GetSpentTimeInMinutes(int? id);
   IEnumerable<Task> GetFiltered(TaskFilters filters, string? orderBy = null);
   bool SourceTaskIdExists(SourceSystemType SourceSystemType, string SourceSystemTaskId, int excludedTaskId = -1);
}

public sealed class TasksRepository : BaseRepository, ITasksRepository
{
   private static readonly string _defaultOrderBy = $"{nameof(Project.PR_Name)} DESC, {nameof(Task.TA_Title)} DESC";

   #region Queries

   private static readonly string GetAllQuery =
      $@"
         SELECT {Task.TableName}.*, sum({nameof(Record.RE_MinutesSpent)}) as '{nameof(Task.TA_SpentTime)}', {Project.TableName}.*
         FROM {Task.TableName}
         INNER JOIN {Project.TableName} ON {nameof(Project.PR_Id)} = {nameof(Task.TA_RelProjectId)}
         LEFT JOIN {Record.TableName} ON {nameof(Record.RE_RelTaskId)} = {nameof(Task.TA_Id)}
         {{0}}
         GROUP BY
          {nameof(Task.TA_Id)}, {nameof(Task.TA_Title)}, {nameof(Task.TA_PlannedTime)}, {nameof(Task.TA_Closed)}, {nameof(Task.TA_RelProjectId)}, {nameof(Task.TA_SourceType)}, {nameof(Task.TA_SourceTaskId)}, {nameof(Project.PR_Id)}, {nameof(Project.PR_Name)}, {nameof(Project.PR_Closed)}
      ";

   private static readonly string InsertQuery =
      $@"
         INSERT INTO {Task.TableName}
            ({nameof(Task.TA_Title)}, {nameof(Task.TA_PlannedTime)}, {nameof(Task.TA_Closed)}, {nameof(Task.TA_RelProjectId)}, {nameof(Task.TA_SourceType)}, {nameof(Task.TA_SourceTaskId)})
         VALUES
            (@{nameof(Task.TA_Title)}, @{nameof(Task.TA_PlannedTime)}, @{nameof(Task.TA_Closed)}, @{nameof(Task.TA_RelProjectId)}, @{nameof(Task.TA_SourceType)}, @{nameof(Task.TA_SourceTaskId)});
         SELECT last_insert_rowid();
      ";

   private static readonly string UpdateQuery =
      $@"
         UPDATE {Task.TableName}
         SET
            {nameof(Task.TA_Title)} = @{nameof(Task.TA_Title)},
            {nameof(Task.TA_PlannedTime)} = @{nameof(Task.TA_PlannedTime)},
            {nameof(Task.TA_Closed)} = @{nameof(Task.TA_Closed)},
            {nameof(Task.TA_RelProjectId)} = @{nameof(Task.TA_RelProjectId)},
            {nameof(Task.TA_SourceType)} = @{nameof(Task.TA_SourceType)},
            {nameof(Task.TA_SourceTaskId)} = @{nameof(Task.TA_SourceTaskId)}
         WHERE
            {nameof(Task.TA_Id)} = @{nameof(Task.TA_Id)}
      ";

   private static readonly string CheckExternalIdIsUniqueQuery =
      $"SELECT count(*) FROM {Task.TableName} WHERE {nameof(Task.TA_SourceType)} = @SourceSystemType AND {nameof(Task.TA_SourceTaskId)} = @SourceSystemTaskId AND {nameof(Task.TA_Id)} <> @ExcludedTaskId";

   private static readonly string GetByIdQuery =
      string.Format(GetAllQuery, $"WHERE {nameof(Task.TA_Id)} = @TA_Id");

   private static readonly string GetBySourceSystemIdQuery =
      string.Format(GetAllQuery, $"WHERE {nameof(Task.TA_SourceType)} <> {(int)SourceSystemType.Internal} AND {nameof(Task.TA_SourceType)} = @SourceSystemType AND {nameof(Task.TA_SourceTaskId)} = @SourceSystemTaskId");

   private static readonly string GetActiveQuery =
      string.Format(GetAllQuery, $"WHERE {nameof(Task.TA_Closed)} <= 0");

   private static readonly string GetClosedQuery =
      string.Format(GetAllQuery, $"WHERE {nameof(Task.TA_Closed)} >= 1");

   private static readonly string GetSpentTimeInMinutesQuery =
      $"SELECT sum({nameof(Record.RE_MinutesSpent)}) as '{nameof(Task.TA_SpentTime)}' FROM {Record.TableName} WHERE {nameof(Record.RE_RelTaskId)} = @TaskId";

   #endregion

   public TasksRepository(string connectionString) : base(connectionString) { }

   public Task? Insert(Task task)
   {
      if (task.TA_SourceType == SourceSystemType.Internal) { task.TA_SourceTaskId = string.Empty; }

      var result = Query((connection) => connection.ExecuteScalar<int>(InsertQuery, task));

      if (result == 0) { return null; }

      task.TA_Id = result;

      return task;
   }

   public bool Update(Task task)
   {
      if (task.TA_SourceType == SourceSystemType.Internal) { task.TA_SourceTaskId = string.Empty; }
      return Query(connection => connection.Execute(UpdateQuery, task)) == 1;
   }

   public Task? Get(int? id)
   {
      if (id is null) { return null; }

      return Query((connection) => connection.Query<Task, Project, Task>(GetByIdQuery, (task, project) =>
      {
         task.TA_Project = project;
         return task;
      },
      param: new { TA_Id = id }, splitOn: "PR_Id").FirstOrDefault());

   }

   public Task? Get(UniversalTaskId? id)
   {
      if (id is null) { return null; }
      if (id.IsInternal && id.InternalTaskId > 0)
      {
         return Get(id.InternalTaskId);
      }

      var result = GetManyWithProject(GetBySourceSystemIdQuery, new
      {
         TaskId = id.InternalTaskId,
         SourceSystemType = ((int?)id.SourceSystemType) ?? -1,
         SourceSystemTaskId = id.SourceSystemTaskId ?? string.Empty,
      }).ToList();

      if (result.Count == 0) { return null; }

      return result.SingleOrDefault(t => t.TA_SourceType == id.SourceSystemType && t.TA_SourceTaskId == id.SourceSystemTaskId) ?? result[0];
   }

   public int GetSpentTimeInMinutes(int? id)
   {
      if (id is null) { return 0; }

      return Query(connection => connection.ExecuteScalar<int>(GetSpentTimeInMinutesQuery, new { TaskId = id.Value }));
   }

   public IEnumerable<Task> GetAll(string? orderBy = null)
   {
      var query = string.Format(GetAllQuery, string.Empty) + $" ORDER BY {orderBy ?? _defaultOrderBy}";
      return GetManyWithProject(query);
   }

   public IEnumerable<Task> GetClosed(string? orderBy = null)
   {
      var query = $"{GetClosedQuery} ORDER BY {orderBy ?? _defaultOrderBy}";
      return GetManyWithProject(query);
   }

   public IEnumerable<Task> GetActive(string? orderBy = null)
   {
      var query = $"{GetActiveQuery} ORDER BY {orderBy ?? _defaultOrderBy}";
      return GetManyWithProject(query);
   }

   public IEnumerable<Task> GetFiltered(TaskFilters filters, string? orderBy = null)
   {
      var query = GetAllQuery;
      var conditionBuilder = new QueryConditionBuilder();

      if (filters.TaskId is not null) { conditionBuilder.Add(ConditionOperators.AND, $"TA_ID = @{nameof(filters.TaskId)}"); }
      if (filters.ProjectId is not null) { conditionBuilder.Add(ConditionOperators.AND, $"TA_RelProjectId = @{nameof(filters.ProjectId)}"); }
      if (filters.Title is not null) { conditionBuilder.Add(ConditionOperators.AND, $"TA_Title LIKE '%@{nameof(filters.Title)}%'"); }
      if (filters.Closed is not null) { conditionBuilder.Add(ConditionOperators.AND, $"TA_Closed = @{nameof(filters.Closed)}"); }
      if (filters.SourceSystemType is not null) { conditionBuilder.Add(ConditionOperators.AND, $"TA_SourceType = @{nameof(filters.SourceSystemType)}"); }
      if (filters.SourceSystemTaskId is not null) { conditionBuilder.Add(ConditionOperators.AND, $"TA_SourceTaskId LIKE '%@{nameof(filters.SourceSystemTaskId)}%'"); }

      var conditions = conditionBuilder.ToString();

      if (conditions.IsNotNullOrEmpty()) { query += $" WHERE {conditions}"; }
      query += $" ORDER BY {orderBy ?? _defaultOrderBy} LIMIT {filters.Limit} OFFSET {filters.Skip}";

      return Query((connection) => connection.Query<Task, Project, Task>(query, (task, project) =>
      {
         task.TA_Project = project;
         return task;
      },
      param: filters,
      splitOn: nameof(Project.PR_Id)));
   }

   public bool SourceTaskIdExists(SourceSystemType sourceType, string sourceTaskId, int excludedTaskId = -1)
   {
      var result = Query(connection => connection.ExecuteScalar<int>(CheckExternalIdIsUniqueQuery, new
      {
         SourceSystemType = sourceType,
         SourceSystemTaskId = sourceTaskId,
         ExcludedTaskId = excludedTaskId,
      }));
      return result > 0;
   }

   private IEnumerable<Task> GetManyWithProject(string query, object? param = null)
   {
      return Query((connection) => connection.Query<Task, Project, Task>(query, (task, project) =>
      {
         task.TA_Project = project;
         return task;
      },
      param: param,
      splitOn: nameof(Project.PR_Id)));
   }
}