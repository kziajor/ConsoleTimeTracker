using App.Entities;

namespace App.Commands.Projects;

internal static class ProjectExtensions
{
   public static string GetOptionLabel(this Project project)
   {
      var activityLabel = project.PR_Closed ? "not active" : "active";
      return $"{project.PR_Id}\t{project.PR_Name} ({activityLabel})";
   }
}