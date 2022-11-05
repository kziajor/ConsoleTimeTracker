using App.Entities;
using Dapper;
using Task = App.Entities.Task;

namespace App.Repositories;

public interface IRecordRepository
{
   Record? Insert(Record record);
   bool Update(Record record);
   Record? Get(int id);
   IEnumerable<Record> GetAll(uint limit = 100, uint skip = 0);
}

public class RecordRepository : BaseRepository, IRecordRepository
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
            (RE_StartedAt, RE_StopedAt, RE_MinutesSpent, RE_Comment, RE_RelTaskId)
         VALUES
            (@RE_StartedAt, @RE_StopedAt, @RE_MinutesSpent, @RE_Comment, @RE_RelTaskId);
         SELECT last_insert_rowid();
      ";
   private const string UpdateQuery =
      @"
         UPDATE tasks
         SET
            RE_StartedAt = @RE_StartedAt,
            RE_StopedAt = @RE_StopedAt,
            RE_MinutesSpent = @RE_MinutesSpent,
            RE_Comment = @RE_Comment,
            RE_RelTaskId = @RE_RelTaskId
         WHERE
            RE_Id = @RE_Id
      ";
   private static string GetByIdQuery => $"{GetAllQuery} WHERE RE_Id = @RE_Id";

   #endregion
   public RecordRepository(string connectionString) : base(connectionString) { }

   public Record? Insert(Record record)
   {
      var result = Query((connection) => connection.ExecuteScalar<int>(InsertQuery, record));

      if (result == 0) { return null; }

      record.RE_Id = result;

      return record;
   }

   public bool Update(Record record)
   {
      return Query(connection => connection.Execute(UpdateQuery, record)) == 1;
   }

   public Record? Get(int id)
   {
      return Query((connection) => connection.Query<Record, Task, Project, Record>(GetByIdQuery, (record, task, project) =>
      {
         task.Project = project;
         record.Task = task;

         return record;
      },
      splitOn: "TA_ID, PR_ID").FirstOrDefault());
   }

   public IEnumerable<Record> GetAll(uint limit = 100, uint skip = 0)
   {

      if (limit >= _maxReturnedRecords)
      {
         throw new ArgumentOutOfRangeException($"{nameof(limit)} value {limit} is out of range. Maximum number is {_maxReturnedRecords}");
      }

      var query = $"{GetAllQuery} ORDER BY RE_StartedAt DESC LIMIT {limit} OFFSET {skip}";

      return Query((connection) => connection.Query<Record, Task, Project, Record>(query, (record, task, project) =>
      {
         task.Project = project;
         record.Task = task;

         return record;
      },
      splitOn: "TA_ID, PR_ID"));
   }
}
