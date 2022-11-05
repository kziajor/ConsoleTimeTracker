using Task = App.Entities.Task;

namespace App.Commands.Tasks;

internal static class TaskExtensions
{
   public static string GetOptionLabel(this Task task)
   {
      var activityLabel = task.TA_Closed ? "not active" : "active";
      return $"{task.TA_Id}\t{task.TA_Title} ({activityLabel})";
   }
}