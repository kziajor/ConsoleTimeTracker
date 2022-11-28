using App.Entities;
using Spectre.Console;

namespace App.Commands.Records.Common;

internal static class RecordExtensions
{
   public static string GetOptionLabel(this Record record)
   {
      var postfix = $"\t[{record.RE_Task?.TA_Project?.PR_Name ?? string.Empty} - {record.RE_Task?.TA_Title ?? string.Empty}]";
      return $"{record.RE_StartedAt} - {record.RE_FinishedAt}{postfix}".EscapeMarkup();
   }

   public static int CalculateMinutesSpent(this Record record, DateTime? currentTime = null)
   {
      var finishTime = currentTime ?? record.RE_FinishedAt;
      return finishTime is not null
         ? (int)finishTime.Value.Subtract(record.RE_StartedAt).TotalMinutes
         : 0;
   }

   public static IEnumerable<Entities.Task> GetTasksSummary(this IEnumerable<Record> source)
   {
      var result = new Dictionary<int, Entities.Task>();

      foreach (var sourceItem in source)
      {
         if (sourceItem.RE_Task is null) { continue; }

         Entities.Task task = sourceItem.RE_Task!;
         if (result.TryGetValue(task.TA_Id, out _))
         {
            result[task.TA_Id].TA_SpentTime += sourceItem.RE_MinutesSpent;
         }
         else
         {
            result.Add(task.TA_Id, new Entities.Task
            {
               TA_Id = task.TA_Id,
               TA_Title = task.TA_Title,
               TA_SourceType = task.TA_SourceType,
               TA_SourceTaskId = task.TA_SourceTaskId,
               TA_RelProjectId = task.TA_RelProjectId,
               TA_PlannedTime = task.TA_PlannedTime,
               TA_SpentTime = sourceItem.RE_MinutesSpent,
               TA_Closed = task.TA_Closed,
               TA_Project = task.TA_Project,
            });
         }
      }

      return result.Select(t => t.Value);
   }

   public static IEnumerable<Project> GetProjectsSummary(this IEnumerable<Record> source)
   {
      var result = new Dictionary<int, Project>();

      foreach (var sourceItem in source)
      {
         if (sourceItem.RE_Task?.TA_Project is null) { continue; }

         Project project = sourceItem.RE_Task!.TA_Project;
         if (result.TryGetValue(project.PR_Id, out _))
         {
            result[project.PR_Id].PR_TimeSpent += sourceItem.RE_MinutesSpent;
         }
         else
         {
            result.Add(project.PR_Id, new Project
            {
               PR_Id = project.PR_Id,
               PR_Name = project.PR_Name,
               PR_TimeSpent = sourceItem.RE_MinutesSpent,
            });
         }
      }

      return result.Select(t => t.Value);
   }
}
