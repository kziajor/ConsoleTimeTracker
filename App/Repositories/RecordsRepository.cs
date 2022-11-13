using App.Commands.Records.Common;
using App.Entities;
using App.Extensions;
using Dapper;
using Task = App.Entities.Task;

namespace App.Repositories;

public interface IRecordsRepository
{
   Record? Insert(Record record);
   bool Update(Record record);
   Record? Get(int? id);
   IEnumerable<Record> GetAll(uint limit = 100, uint skip = 0, string orderBy = "RE_StartedAt DESC");
   IEnumerable<Record> GetInProgress(string orderBy = "RE_StartedAt DESC");
}

public sealed class RecordsRepository : BaseRepository, IRecordsRepository
{
   private const uint _maxReturnedRecords = 1000;

   #region Queries

   private const string GetAllQuery =
      @"
         SELECT *
         FROM Records
         INNER JOIN Tasks ON TA_Id = RE_RelTaskId
         INNER JOIN Projects ON PR_Id = TA_RelProjectId
      ";
   private const string InsertQuery =
      @"
         INSERT INTO Records
            (RE_StartedAt, RE_FinishedAt, RE_MinutesSpent, RE_Comment, RE_RelTaskId)
         VALUES
            (@RE_StartedAt, @RE_FinishedAt, @RE_MinutesSpent, @RE_Comment, @RE_RelTaskId);
         SELECT last_insert_rowid();
      ";
   private const string UpdateQuery =
      @"
         UPDATE Records
         SET
            RE_StartedAt = @RE_StartedAt,
            RE_FinishedAt = @RE_FinishedAt,
            RE_MinutesSpent = @RE_MinutesSpent,
            RE_Comment = @RE_Comment,
            RE_RelTaskId = @RE_RelTaskId
         WHERE
            RE_Id = @RE_Id
      ";
   private static string GetByIdQuery => $"{GetAllQuery} WHERE RE_Id = @RE_Id";

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
         task.Project = project;
         record.Task = task;

         return record;
      },
      param: new { RE_Id = id },
      splitOn: "TA_ID, PR_ID").FirstOrDefault());
   }

   public IEnumerable<Record> GetAll(uint limit = 100, uint skip = 0, string orderBy = "RE_StartedAt DESC")
   {
      if (orderBy.IsNullOrEmpty()) { throw new ArgumentException($"{nameof(orderBy)} parameter must have value"); }
      if (limit >= _maxReturnedRecords)
      {
         throw new ArgumentOutOfRangeException($"{nameof(limit)} value {limit} is out of range. Maximum number is {_maxReturnedRecords}");
      }

      var query = $"{GetAllQuery} ORDER BY {orderBy} LIMIT {limit} OFFSET {skip}";

      return Query((connection) => connection.Query<Record, Task, Project, Record>(query, (record, task, project) =>
      {
         task.Project = project;
         record.Task = task;

         return record;
      },
      splitOn: "TA_Id, PR_Id"));
   }

   public IEnumerable<Record> GetInProgress(string orderBy = "RE_StartedAt DESC")
   {
      if (orderBy.IsNullOrEmpty()) { throw new ArgumentException($"{nameof(orderBy)} parameter must have value"); }

      var query = $"{GetAllQuery} WHERE RE_FinishedAt IS NULL ORDER BY {orderBy}";

      return Query((connection) => connection.Query<Record, Task, Project, Record>(query, (record, task, project) =>
      {
         task.Project = project;
         record.Task = task;

         return record;
      }, splitOn: "TA_Id, PR_Id"));
   }
}
