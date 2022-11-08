using Spectre.Console;

using Task = App.Entities.Task;

namespace App.Commands.Tasks.Common;

public static class TaskExtensions
{
   public static string GetOptionLabel(this Task task)
   {
      var activityLabel = task.TA_Closed ? "not active" : "active";
      var projectPostfix = $"[{task.Project?.PR_Name ?? string.Empty}]";
      return $"{task.TA_Id}\t{task.TA_Title}\t({activityLabel}){projectPostfix.EscapeMarkup()}";
   }

   public static string GetScopedExternalId(this Task task)
   {
      return $"{task.TA_ExternalSystemType} - {task.TA_ExternalSystemTaskId}";
   }
}