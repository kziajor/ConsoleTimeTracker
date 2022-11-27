using App.Assets;
using Spectre.Console;
using Task = App.Entities.Task;

namespace App.Commands.Tasks.Common;

public static class TaskExtensions
{
   public static string GetOptionLabel(this Task task)
   {
      var activityIcon = task.TA_Closed ? Icons.CHECK : Icons.CLOCK;
      var projectLabel = $"[{task.TA_Project?.PR_Name ?? string.Empty}]".EscapeMarkup();
      var taskId = task.UniversalTaskId?.ToString().PadRight(20, ' ');

      return Emoji.Replace($"{activityIcon} {taskId}\t{task.TA_Title} {projectLabel}");
   }
}