using App.Entities;
using App.Extensions;

using Dapper;

namespace App.Repositories;

public interface IProjectsRepository
{
   Project? Insert(Project project);
   bool Update(Project project);
   Project? Get(int id);
   IEnumerable<Project> GetAll(string orderBy = "PR_Name ASC");
   IEnumerable<Project> GetClosed(string orderBy = "PR_Name ASC");
   IEnumerable<Project> GetActive(string orderBy = "PR_Name ASC");
}

public sealed class ProjectsRepository : BaseRepository, IProjectsRepository
{
   #region Queries

   private const string GetAllQuery = "SELECT * FROM Projects";
   private const string InsertQuery = "INSERT INTO Projects (PR_Name, PR_Closed) VALUES (@PR_Name, @PR_Closed); SELECT last_insert_rowid();";
   private const string UpdateQuery = "UPDATE projects SET PR_Name = @PR_Name, PR_Closed = @PR_Closed WHERE PR_Id = @PR_Id";
   private static string GetByIdQuery => $"{GetAllQuery} WHERE PR_Id = @PR_Id";
   private static string GetClosedQuery => $"{GetAllQuery} WHERE PR_Closed >= 1";
   private static string GetActiveQuery => $"{GetAllQuery} WHERE PR_Closed <= 0";

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

   public IEnumerable<Project> GetAll(string orderBy = "PR_Name ASC")
   {
      if (orderBy.IsNullOrEmpty()) { throw new ArgumentException($"Argument '{nameof(orderBy)}' is empty"); }
      return Query((connection) => connection.Query<Project>($"{GetAllQuery} ORDER BY {orderBy}"));
   }

   public IEnumerable<Project> GetClosed(string orderBy = "PR_Name ASC")
   {
      if (orderBy.IsNullOrEmpty()) { throw new ArgumentException($"Argument '{nameof(orderBy)}' is empty"); }
      return Query((connection) => connection.Query<Project>($"{GetClosedQuery} ORDER BY {orderBy}"));
   }

   public IEnumerable<Project> GetActive(string orderBy = "PR_Name ASC")
   {
      if (orderBy.IsNullOrEmpty()) { throw new ArgumentException($"Argument '{nameof(orderBy)}' is empty"); }
      return Query((connection) => connection.Query<Project>($"{GetActiveQuery} ORDER BY {orderBy}"));
   }
}