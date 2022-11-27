using App.Entities;
using Spectre.Console;

namespace App.Commands.Projects.Common;

internal static class ProjectExtensions
{
   public static string GetOptionLabel(this Project project)
   {
      var activityLabel = project.PR_Closed ? "not active" : "active";
      return $"{project.PR_Id}\t{project.PR_Name} ({activityLabel})".EscapeMarkup();
   }
}