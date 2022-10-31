using App.Entities;

namespace App.Commands.Projects;

internal static class ProjectExtensions
{
   public static string GetOptionLabel(this Project project)
   {
      var activityLabel = project.closed ? "not active" : "active";
      return $"{project.id}\t{project.name} ({activityLabel})";
   }
}