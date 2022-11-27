using App.Entities;
using Spectre.Console;

namespace App.Commands.Records.Common;

internal static class RecordExtensions
{
   public static string GetOptionLabel(this Record record)
   {
      var postfix = $"\t[{record.Task?.TA_Project?.PR_Name ?? string.Empty} - {record.Task?.TA_Title ?? string.Empty}]";
      return $"{record.RE_StartedAt} - {record.RE_FinishedAt}{postfix}".EscapeMarkup();
   }

   public static int CalculateMinutesSpent(this Record record, DateTime? currentTime = null)
   {
      var finishTime = currentTime ?? record.RE_FinishedAt;
      return finishTime is not null
         ? (int)finishTime.Value.Subtract(record.RE_StartedAt).TotalMinutes
         : 0;
   }
}
