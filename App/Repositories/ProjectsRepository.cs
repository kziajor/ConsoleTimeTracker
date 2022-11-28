using App.Entities;
using Dapper;
using Task = App.Entities.Task;

namespace App.Repositories;

public interface IProjectsRepository
{
   Project? Insert(Project project);
   bool Update(Project project);
   Project? Get(int id);
   IEnumerable<Project> GetAll(string? orderBy = null);
   IEnumerable<Project> GetClosed(string? orderBy = null);
   IEnumerable<Project> GetActive(string? orderBy = null);
}

public sealed class ProjectsRepository : BaseRepository, IProjectsRepository
{
   private static readonly string _defaultOrderBy = $"{nameof(Project.PR_Name)} DESC";

   #region Queries

   private static readonly string GetAllQuery =
      $@"
         SELECT
            {Project.TableName}.*
            ,SUM({nameof(Record.RE_MinutesSpent)}) AS '{nameof(Project.PR_TimeSpent)}'
         FROM {Project.TableName}
         LEFT JOIN {Task.TableName} ON {nameof(Task.TA_RelProjectId)} = {nameof(Project.PR_Id)}
         LEFT JOIN {Record.TableName} ON {nameof(Record.RE_RelTaskId)} = {nameof(Task.TA_Id)}
         {{0}}
         GROUP BY {nameof(Project.PR_Id)}, {nameof(Project.PR_Name)}
      ";

   private static readonly string InsertQuery =
      $@"
         INSERT INTO {Project.TableName}
         (
            {nameof(Project.PR_Name)}
            ,{nameof(Project.PR_Closed)}
         )
         VALUES
         (
            @{nameof(Project.PR_Name)}
            ,@{nameof(Project.PR_Closed)}
         );
         SELECT last_insert_rowid();
      ";

   private static readonly string UpdateQuery =
      $@"
         UPDATE {Project.TableName}
         SET
            {nameof(Project.PR_Name)} = @{nameof(Project.PR_Name)}
            ,{nameof(Project.PR_Closed)} = @{nameof(Project.PR_Closed)}
         WHERE
            {nameof(Project.PR_Id)} = @{nameof(Project.PR_Id)}
      ";

   private static readonly string GetByIdQuery = string.Format(GetAllQuery, $"WHERE {nameof(Project.PR_Id)} = @{nameof(Project.PR_Id)}");

   private static readonly string GetClosedQuery = string.Format(GetAllQuery, $"WHERE {nameof(Project.PR_Closed)} >= 1");

   private static readonly string GetActiveQuery = string.Format(GetAllQuery, $"WHERE {nameof(Project.PR_Closed)} <= 0");

   #endregion

   public ProjectsRepository(string connectionString) : base(connectionString) { }

   public Project? Insert(Project project)
   {
      var result = Query((connection) => connection.ExecuteScalar<int>(InsertQuery, project));

      if (result == 0) { return null; }

      project.PR_Id = result;

      return project;
   }

   public bool Update(Project project)
   {
      return Query(connection => connection.Execute(UpdateQuery, project)) == 1;
   }

   public Project? Get(int id)
   {
      return Query((connection) => connection.QueryFirstOrDefault<Project?>(GetByIdQuery, new { PR_Id = id }));
   }

   public IEnumerable<Project> GetAll(string? orderBy = null)
   {
      return Query((connection) => connection.Query<Project>($"{GetAllQuery} ORDER BY {orderBy ?? _defaultOrderBy}"));
   }

   public IEnumerable<Project> GetClosed(string? orderBy = null)
   {
      return Query((connection) => connection.Query<Project>($"{GetClosedQuery} ORDER BY {orderBy ?? _defaultOrderBy}"));
   }

   public IEnumerable<Project> GetActive(string? orderBy = null)
   {
      return Query((connection) => connection.Query<Project>($"{GetActiveQuery} ORDER BY {orderBy ?? _defaultOrderBy}"));
   }
}