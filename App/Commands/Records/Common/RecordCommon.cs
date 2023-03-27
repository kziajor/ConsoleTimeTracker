using App.Assets;
using App.Commands.Records.Common;
using App.Commands.Tasks.Common;
using App.Entities;
using App.Extensions;
using App.Models.Inputs;
using App.Repositories;

using Spectre.Console;

namespace App.Commands.Records
{
   public static class RecordCommon
   {
      private static readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();
      private static readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();

      public static void DisplayList(IEnumerable<Record> records, string header = "Records", string footer = "")
      {
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
            table.AddRow(
               new Text(record.RE_Id?.ToString() ?? string.Empty),
               new Text(record.RE_Task?.UniversalTaskId.ToString().EscapeMarkup() ?? string.Empty),
               new Text(record.RE_Task?.TA_Project?.PR_Name.EscapeMarkup() ?? string.Empty),
               new Text(record.RE_Task?.TA_Title.EscapeMarkup() ?? string.Empty),
               new Text(record.RE_StartedAt.ToIsoDateTime()),
               new Text(record.RE_FinishedAt?.ToIsoDateTime() ?? string.Empty),
               new Text(record.TimeSpentHours > 0 ? record.TimeSpentHours.ToString("0.00") : "-").RightAligned(),
               new Text(record.RE_Comment.EscapeMarkup() ?? string.Empty)
            );
         }

         _console.MarkupLineInterpolated($"[green]{header}[/]");
         _console.Write(table);
         _console.WriteLine();
         _console.MarkupLine(footer);
         _console.WriteLine();
      }

      public static void DisplayRecordsInProgress(IEnumerable<Record> records)
      {
         var recordsToDisplay = records.ToList();

         if (recordsToDisplay.Count == 0) { return; }
         if (recordsToDisplay.Count == 1)
         {
            _console.MarkupLine($"[{Colors.Error}]Record in progress[/]");
            _console.WriteLine();
            DisplayDetails(recordsToDisplay[0]);
         }

         DisplayList(recordsToDisplay, "Records in progress");
      }

      public static Record CreateNewRecordInteractive(RecordInput input)
      {
         if (input is null) throw new Exception("Wrong input data");

         var taskId = input.UniversalTaskId?.IsInternal == true && input.UniversalTaskId.InternalTaskId > 0
            ? input.UniversalTaskId.InternalTaskId
            : _dbRepository.Tasks.Get(input.UniversalTaskId)?.TA_Id;

         var result = new Record
         {
            RE_RelTaskId = taskId ?? _dbRepository.Tasks
               .GetActive()
               .OrderByDescending(t => t.TA_Id)
               .ChooseOne(
                  "Choose task",
                  optionNameConverter: t => t.GetOptionLabel())
               ?.TA_Id ?? 0,
            RE_StartedAt = input.StartedAt ?? CommandCommon.AskForWithEmptyAllowed("Started at", DateTime.Now),
            RE_FinishedAt = input.FinishedAt ?? CommandCommon.AskForWithEmptyAllowed<DateTime?>("Finished at (leave empty if not finished)", null),
            RE_Comment = input.Comment ?? CommandCommon.AskForWithEmptyAllowed<string?>("Comment", null),
         };

         result.RE_MinutesSpent = result.CalculateMinutesSpent();

         return result;
      }

      public static Record CreateNewRecord(RecordInput input)
      {
         if (input is null) throw new Exception("Wrong input data");

         var taskId = input.UniversalTaskId?.IsInternal == true && input.UniversalTaskId.InternalTaskId > 0
            ? input.UniversalTaskId.InternalTaskId
            : _dbRepository.Tasks.Get(input.UniversalTaskId)?.TA_Id;

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
         if (record.RE_FinishedAt is not null && record.RE_FinishedAt < record.RE_StartedAt) throw new ArgumentOutOfRangeException($"'Finished at' date is earlier than 'Started at' date ({record.RE_FinishedAt.Value.ToIsoDateTime()} < {record.RE_StartedAt.ToIsoDateTime()}");
         if (record.RE_FinishedAt is not null && record.RE_StartedAt.Date != record.RE_FinishedAt?.Date) throw new ArgumentOutOfRangeException("'Finished at' date is in different day than 'Started at' date. Record must start and finished in the same day.");
         // TODO: Validate that record is not in conflict with another record (dates and times)
      }

      public static Record? GetOrChoose(int? recordId = null, IEnumerable<Record>? records = null)
      {
         if (recordId is null || recordId <= 0)
         {
            return (records ?? _dbRepository.Records.GetAll())
               .ChooseOne("Choose record", 5, optionNameConverter: (record) => record.GetOptionLabel());
         }

         return _dbRepository.Records.Get(recordId.Value);
      }

      public static void UpdateRecordDataInteractive(Record record, RecordInput input)
      {
         if (input is null) throw new Exception("Wrong input data");

         var taskId = input.UniversalTaskId?.IsInternal == true && input.UniversalTaskId.InternalTaskId > 0
            ? input.UniversalTaskId.InternalTaskId
            : _dbRepository
               .Tasks
               .Get(input.UniversalTaskId)
               ?.TA_Id;

         record.RE_RelTaskId = taskId ?? _dbRepository.Tasks
               .GetActive()
               .OrderBy(t => t.TA_Title)
               .ToList()
               .MoveToTop(t => t.TA_Id == record.RE_RelTaskId)
               .ChooseOne(
                  "Choose task",
                  predicateDefaultOption: t => t.TA_Id == record.RE_RelTaskId,
                  optionNameConverter: t => t.GetOptionLabel())
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

         var taskId = input.UniversalTaskId?.IsInternal == true && input.UniversalTaskId.InternalTaskId > 0
            ? input.UniversalTaskId.InternalTaskId
            : _dbRepository
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
            .AddKeyValueRow("Project", record.RE_Task?.TA_Project?.PR_Name)
            .AddKeyValueRow("Task", record.RE_Task?.TA_Title)
            .AddKeyValueRow("Started at", record.RE_StartedAt)
            .AddKeyValueRow("Finished at", record.RE_FinishedAt)
            .AddKeyValueRow("Time spent (m)", record.RE_MinutesSpent)
            .AddKeyValueRow("Time spent (h)", Math.Round((double)record.RE_MinutesSpent / 60, 2))
            .AddKeyValueRow("Comment", record.RE_Comment);

         _console.Write(grid);
      }
   }
}
