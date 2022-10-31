namespace App.Repositories.Queries;

public static class TaskQueries
{
   public const string AllTasksQuery = "SELECT * FROM tasks AS t INNER JOIN projects AS p ON p.Id = t.rel_project_id";
   public const string ActiveTasksQuery = $"{AllTasksQuery} WHERE t.closed = 0";
   public const string ClosedTasksQuery = $"{AllTasksQuery} WHERE t.closed > 0";
   public const string TaskQuery = $"{AllTasksQuery} WHERE t.Id = @id";
}
