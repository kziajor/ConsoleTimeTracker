using App.Commands.Records.Common;
using App.Commands.Tasks.Common;
using App.Entities;
using App.Extensions;
using App.Models.Inputs;
using App.Repositories;

using Spectre.Console;

using Task = App.Entities.Task;

namespace App.Commands.Records
{
   public static class RecordCommon
   {
      public static void DisplayList(IEnumerable<Record> records, string header = "Records")
      {
         var settingsProvider = ServicesProvider.GetInstance<ISettingsProvider>();
         var table = new Table();

         table
            .Border(TableBorder.Rounded)
            .AddColumn(new TableColumn("[green]Id[/]").LeftAligned())
            .AddColumn(new TableColumn("[green]Task id[/]")).LeftAligned()
            .AddColumn(new TableColumn("[green]Project name[/]").LeftAligned())
            .AddColumn(new TableColumn("[green]Task title[/]").LeftAligned())
            .AddColumn(new TableColumn("[green]Started at[/]").LeftAligned())
            .AddColumn(new TableColumn("[green]Finished at[/]").LeftAligned())
            .AddColumn(new TableColumn("[green]Time spent (H)[/]").RightAligned())
            .AddColumn(new TableColumn("[green]Comment[/]").LeftAligned());

         foreach (var record in records)
         {
            var taskId = settingsProvider.ExternalSystemPriority
               ? $"{record.Task?.ExternalFullId} ({record.Task?.TA_Id})"
               : record.RE_RelTaskId.ToString();

            table.AddRow(
               new Text(record.RE_Id?.ToString() ?? string.Empty),
               new Text(taskId?.ToString() ?? string.Empty),
               new Text(record.Task?.Project?.PR_Name ?? string.Empty),
               new Text(record.Task?.TA_Title ?? string.Empty),
               new Text(record.RE_StartedAt.ToIsoString()),
               new Text(record.RE_FinishedAt?.ToIsoString() ?? string.Empty),
               new Text(record.TimeSpentHours > 0 ? record.TimeSpentHours.ToString("0.00") : "-").RightAligned(),
               new Text(record.RE_Comment ?? string.Empty)
            );
         }

         AnsiConsole.MarkupLineInterpolated($"[green]{header}[/]");
         AnsiConsole.Write(table);
      }

      public static Record CreateNewRecordInteractive(RecordInput input)
      {
         if (input is null) throw new Exception("Wrong input data");

         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();
         var taskId = input.UniversalTaskId?.IsInternal == true && input.UniversalTaskId.TaskId > 0
            ? input.UniversalTaskId.TaskId
            : dbRepository.Tasks.Get(input.UniversalTaskId)?.TA_Id;

         var result = new Record
         {
            RE_RelTaskId = taskId ?? dbRepository.Tasks
               .GetActive()
               .OrderByDescending(t => t.TA_Id)
               .ChooseOne("Choose task", 20, (t) => t.GetOptionLabel())
               ?.TA_Id ?? 0,
            RE_StartedAt = input.StartedAt ?? CommandCommon.AskForWithEmptyAllowed("Started at", DateTime.Now),
            RE_FinishedAt = input.FinishedAt ?? CommandCommon.AskForWithEmptyAllowed<DateTime?>("Finished at (leave empty if not finished)"),
            RE_Comment = input.Comment ?? CommandCommon.AskForWithEmptyAllowed<string?>("Comment"),
         };

         result.RE_MinutesSpent = result.CalculateMinutesSpent();

         return result;
      }

      public static Record CreateNewRecord(RecordInput input)
      {
         if (input is null) throw new Exception("Wrong input data");

         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();
         var taskId = input.UniversalTaskId?.IsInternal == true && input.UniversalTaskId.TaskId > 0
            ? input.UniversalTaskId.TaskId
            : dbRepository.Tasks.Get(input.UniversalTaskId)?.TA_Id;

         var result = new Record
         {
            RE_RelTaskId = taskId ?? 0,
            RE_StartedAt = input.StartedAt ?? DateTime.Now,
            RE_FinishedAt = input.FinishedAt,
            RE_Comment = input.Comment,
         };

         result.RE_MinutesSpent = result.CalculateMinutesSpent();

         return result;
      }

      public static void ValidateModel(Record record)
      {
         if (record.RE_RelTaskId == 0) throw new Exception("Task not found");
         if (record.RE_FinishedAt < record.RE_StartedAt) throw new ArgumentOutOfRangeException("'Finished at' date is earlier than 'Started at' date");
         // TODO: Validate that record is not in conflict with another record (dates and times)
      }

      public static Record? GetOrChoose(int? recordId = null, IEnumerable<Record>? records = null)
      {
         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();

         if (recordId is null || recordId <= 0)
         {
            return (records ?? dbRepository.Records.GetAll())
               .ChooseOne("Choose record", 20, (record) => record.GetOptionLabel());
         }

         return dbRepository.Records.Get(recordId.Value);
      }

      public static void UpdateRecordDataInteractive(Record record, RecordInput input)
      {
         if (input is null) throw new Exception("Wrong input data");

         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();
         var taskId = input.UniversalTaskId?.IsInternal == true && input.UniversalTaskId.TaskId > 0
            ? input.UniversalTaskId.TaskId
            : dbRepository
               .Tasks
               .Get(input.UniversalTaskId)
               ?.TA_Id;

         record.RE_RelTaskId = taskId ?? dbRepository.Tasks
               .GetActive()
               .Where(t => t.TA_Id != record.RE_RelTaskId)
               .OrderByDescending(t => t.TA_Id)
               .Prepend(record.Task ?? new Task())
               ?.ChooseOne("Choose task", 20, (t) => t.GetOptionLabel(t.TA_Id == record.RE_RelTaskId))
               ?.TA_Id ?? record.RE_RelTaskId;
         record.RE_StartedAt = input.StartedAt ?? CommandCommon.AskForWithEmptyAllowed("Started at", record.RE_StartedAt);
         record.RE_FinishedAt = input.ClearFinishedAt
            ? null
            : input.FinishedAt ?? CommandCommon.AskForWithEmptyAllowed("Finished at", record.RE_FinishedAt);
         record.RE_MinutesSpent = record.CalculateMinutesSpent();
         record.RE_Comment = input.ClearComment
            ? null
            : input.Comment ?? CommandCommon.AskForWithEmptyAllowed<string?>("Comment", record.RE_Comment);
      }

      public static void UpdateRecordData(Record record, RecordInput input)
      {
         if (input is null) throw new Exception("Wrong input data");

         var dbRepository = ServicesProvider.GetInstance<IDbRepository>();
         var taskId = input.UniversalTaskId?.IsInternal == true && input.UniversalTaskId.TaskId > 0
            ? input.UniversalTaskId.TaskId
            : dbRepository
               .Tasks
               .Get(input.UniversalTaskId)
               ?.TA_Id;

         record.RE_RelTaskId = taskId ?? record.RE_RelTaskId;
         record.RE_StartedAt = input.StartedAt ?? record.RE_StartedAt;
         record.RE_FinishedAt = input.FinishedAt ?? record.RE_FinishedAt;
         record.RE_Comment = input.Comment ?? record.RE_Comment;
         record.RE_MinutesSpent = record.CalculateMinutesSpent();
      }

      public static void DisplayDetails(Record record)
      {
         var grid = new Grid()
            .AddColumn()
            .AddColumn();

         grid
            .AddKeyValueRow("Id", record.RE_Id)
            .AddKeyValueRow("Project", record.Task?.Project?.PR_Name)
            .AddKeyValueRow("Task", record.Task?.TA_Title)
            .AddKeyValueRow("Started at", record.RE_StartedAt)
            .AddKeyValueRow("Finished at", record.RE_FinishedAt)
            .AddKeyValueRow("Time spent (m)", record.RE_MinutesSpent)
            .AddKeyValueRow("Time spent (h)", Math.Round((double)record.RE_MinutesSpent / 60, 2))
            .AddKeyValueRow("Comment", record.RE_Comment);

         AnsiConsole.Write(grid);
      }
   }
}
