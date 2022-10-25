namespace App.Repositories.Queries;

public static class ProjectQueries
{
   public const string ActiveProjectsQuery = "SELECT * FROM projects WHERE closed = 0";
   public const string ClosedProjectsQuery = "SELECT * FROM projects WHERE closed > 0";
}
