using App.Commands.Records.Common;
using App.Entities;
using Dapper;
using Task = App.Entities.Task;

namespace App.Repositories;

public interface IRecordsRepository
{
   Record? Insert(Record record);
   bool Update(Record record);
   Record? Get(int? id);
   IEnumerable<Record> GetAll(uint limit = 100, uint skip = 0, string? orderBy = null);
   IEnumerable<Record> GetInProgress(string? orderBy = null);
}

public sealed class RecordsRepository : BaseRepository, IRecordsRepository
{
   private const uint _maxReturnedRecords = 1000;
   private static readonly string _defaultOrderBy = $"{nameof(Record.RE_StartedAt)} ASC";

   #region Queries

   private static readonly string GetAllQuery =
      $@"
         SELECT *
         FROM {Record.TableName}
         INNER JOIN {Task.TableName} ON {nameof(Task.TA_Id)} = {nameof(Record.RE_RelTaskId)}
         INNER JOIN {Project.TableName} ON {nameof(Project.PR_Id)} = {nameof(Task.TA_RelProjectId)}
      ";

   private static readonly string InsertQuery =
      $@"
         INSERT INTO {Record.TableName}
         (
            {nameof(Record.RE_StartedAt)}
            ,{nameof(Record.RE_FinishedAt)}
            ,{nameof(Record.RE_MinutesSpent)}
            ,{nameof(Record.RE_Comment)}
            ,{nameof(Record.RE_RelTaskId)}
         )
         VALUES
         (
            @{nameof(Record.RE_StartedAt)}
            ,@{nameof(Record.RE_FinishedAt)}
            ,@{nameof(Record.RE_MinutesSpent)}
            ,@{nameof(Record.RE_Comment)}
            ,@{nameof(Record.RE_RelTaskId)}
         );
         SELECT last_insert_rowid();
      ";

   private static readonly string UpdateQuery =
      $@"
         UPDATE {Record.TableName}
         SET
            {nameof(Record.RE_StartedAt)} = @{nameof(Record.RE_StartedAt)}
            ,{nameof(Record.RE_FinishedAt)} = @{nameof(Record.RE_FinishedAt)}
            ,{nameof(Record.RE_MinutesSpent)} = @{nameof(Record.RE_MinutesSpent)}
            ,{nameof(Record.RE_Comment)} = @{nameof(Record.RE_Comment)}
            ,{nameof(Record.RE_RelTaskId)} = @{nameof(Record.RE_RelTaskId)}
         WHERE
            {nameof(Record.RE_Id)} = @{nameof(Record.RE_Id)}
      ";

   private static readonly string GetByIdQuery = $"{GetAllQuery} WHERE {nameof(Record.RE_Id)} = @{nameof(Record.RE_Id)}";

   #endregion
   public RecordsRepository(string connectionString) : base(connectionString) { }

   public Record? Insert(Record record)
   {
      record.RE_MinutesSpent = record.CalculateMinutesSpent();
      var result = Query((connection) => connection.ExecuteScalar<int>(InsertQuery, record));

      if (result == 0) { return null; }

      record.RE_Id = result;

      return record;
   }

   public bool Update(Record record)
   {
      record.RE_MinutesSpent = record.CalculateMinutesSpent();
      return Query(connection => connection.Execute(UpdateQuery, record)) == 1;
   }

   public Record? Get(int? id)
   {
      if (id is null) { return null; }

      return Query((connection) => connection.Query<Record, Task, Project, Record>(GetByIdQuery, (record, task, project) =>
      {
         task.TA_Project = project;
         record.Task = task;

         return record;
      },
      param: new { RE_Id = id },
      splitOn: "TA_ID, PR_ID").FirstOrDefault());
   }

   public IEnumerable<Record> GetAll(uint limit = 100, uint skip = 0, string? orderBy = null)
   {
      if (limit >= _maxReturnedRecords)
      {
         throw new ArgumentOutOfRangeException($"{nameof(limit)} value {limit} is out of range. Maximum number is {_maxReturnedRecords}");
      }

      var query = $"{GetAllQuery} ORDER BY {orderBy ?? _defaultOrderBy} LIMIT {limit} OFFSET {skip}";

      return Query((connection) => connection.Query<Record, Task, Project, Record>(query, (record, task, project) =>
      {
         task.TA_Project = project;
         record.Task = task;

         return record;
      },
      splitOn: "TA_Id, PR_Id"));
   }

   public IEnumerable<Record> GetInProgress(string? orderBy = null)
   {
      var query = $"{GetAllQuery} WHERE RE_FinishedAt IS NULL ORDER BY {orderBy ?? _defaultOrderBy}";

      return Query((connection) => connection.Query<Record, Task, Project, Record>(query, (record, task, project) =>
      {
         task.TA_Project = project;
         record.Task = task;

         return record;
      }, splitOn: "TA_Id, PR_Id"));
   }
}
