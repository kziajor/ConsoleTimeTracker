using App.Commands.Records.Common;
using App.Commands.Tasks.Common;
using App.Extensions;
using App.Models.Inputs;
using App.Repositories;

using Spectre.Console;

using System.CommandLine;

namespace App.Commands.Records;

public class RecordAddCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
   private readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();

   public RecordAddCommand() : base("add", "Add new record")
   {
      var taskIdArgument = TaskArguments.GetIdArgument();
      var startedAtOption = RecordOptions.GetStartedAtOption();
      var finishedAtOption = RecordOptions.GetFinishedAtOption();
      var commentOption = RecordOptions.GetCommentOption();
      var manualModeOption = CommonOptions.GetManualModeOption();

      Add(taskIdArgument);
      Add(startedAtOption);
      Add(finishedAtOption);
      Add(commentOption);
      Add(manualModeOption);

      this.SetHandler(
         (recordInput) => AddRecordHandler(recordInput),
         new RecordInputBinder(
            taskIdArgument: taskIdArgument,
            startedAt: startedAtOption,
            finishedAt: finishedAtOption,
            comment: commentOption,
            manualMode: manualModeOption
         )
      );
   }

   private void AddRecordHandler(RecordInput input)
   {
      try
      {
         var recordsInProgress = _dbRepository.Records.GetInProgress().ToList();
         RecordCommon.DisplayList(recordsInProgress, "Records in progres");
         _console.WriteLine();

         var record = input.ManualMode
            ? RecordCommon.CreateNewRecord(input)
            : RecordCommon.CreateNewRecordInteractive(input);

         RecordCommon.ValidateModel(record);

         if (recordsInProgress.Count > 0 && record.RE_FinishedAt is null)
         {
            _console.WriteError("There could be only one record in progres. Set 'Finished at' value for new record or finish record that is already in progress");
            return;
         }

         var result = _dbRepository.Records.Insert(record);

         if (result is null)
         {
            _console.WriteErrorAndExit("Error while adding new record to database");
            return;
         }

         _console.MarkupLine("[green]New record added[/]");
         _console.WriteLine();
         RecordCommon.DisplayList(_dbRepository.Records.GetByDay(result.RE_StartedAt), "Records");
      }
      catch (Exception ex)
      {
         _console.WriteError(ex.Message);
      }
   }
}