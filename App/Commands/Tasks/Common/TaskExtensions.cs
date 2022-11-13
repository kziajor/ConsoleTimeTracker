using App.Assets;
using Spectre.Console;

using Task = App.Entities.Task;

namespace App.Commands.Tasks.Common;

public static class TaskExtensions
{
   public static string GetOptionLabel(this Task task, bool asDefault = false, bool externalSystemPriority = false)
   {
      var activityIcon = task.TA_Closed ? Icons.CHECK : Icons.CLOCK;
      var projectLabel = $"[{task.Project?.PR_Name ?? string.Empty}]".EscapeMarkup();
      var taskId = (externalSystemPriority ? $"{task.ExternalFullId} ({task.TA_Id})" : $"{task.TA_Id} ({task.ExternalFullId})").PadRight(20, ' ');
      var isDefaultIcon = asDefault ? "*" : " ";

      var result = Emoji.Replace($"{isDefaultIcon}{activityIcon} {taskId}\t{task.TA_Title} {projectLabel}");

      return asDefault ? $"[{Colors.Primary.ToMarkup()}]{result}[/]" : result;
   }
}