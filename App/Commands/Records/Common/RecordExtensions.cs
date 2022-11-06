using App.Entities;
using Spectre.Console;

namespace App.Commands.Records.Common;

internal static class RecordExtensions
{
   public static string GetOptionLabel(this Record record)
   {
      var postfix = $"\t[{record.Task?.Project?.PR_Name ?? string.Empty} - {record.Task?.TA_Title ?? string.Empty}]";
      return $"{record.RE_StartedAt} - {record.RE_FinishedAt}{postfix}".EscapeMarkup();
   }
}
