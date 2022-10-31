using Task = App.Entities.Task;

namespace App.Commands.Tasks;

internal static class TaskExtensions
{
   public static string GetOptionLabel(this Task task)
   {
      var activityLabel = task.closed ? "not active" : "active";
      return $"{task.id}\t{task.title} ({activityLabel})";
   }
}