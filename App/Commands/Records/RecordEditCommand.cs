using App.Commands.Records.Common;
using App.Extensions;
using App.Models.Inputs;
using App.Repositories;

using Spectre.Console;

using System.CommandLine;

namespace App.Commands.Records;

public class RecordEditCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
   private readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();

   public RecordEditCommand() : base("edit", "Edit record")
   {
      AddAlias("e");

      var idArgument = RecordArguments.GetIdArgument();
      var taskIdOption = RecordOptions.GetTaskIdOption();
      var startedAtOption = RecordOptions.GetStartedAtOption();
      var finishedAtOption = RecordOptions.GetFinishedAtOption();
      var commentOption = RecordOptions.GetCommentOption();
      var clearCommentOption = RecordOptions.GetClearCommentOption();
      var clearFinishedAtOption = RecordOptions.GetClearFinishedAtOption();
      var interactiveModeOption = CommonOptions.GetInteractiveModeOption();

      Add(idArgument);
      Add(taskIdOption);
      Add(startedAtOption);
      Add(finishedAtOption);
      Add(commentOption);
      Add(clearCommentOption);
      Add(clearFinishedAtOption);
      Add(interactiveModeOption);

      this.SetHandler(
         (recordInput) => EditRecordHandler(recordInput),
         new RecordInputBinder(
            recordId: idArgument,
            taskIdOption: taskIdOption,
            startedAt: startedAtOption,
            finishedAt: finishedAtOption,
            comment: commentOption,
            interactiveMode: interactiveModeOption,
            clearComment: clearCommentOption,
            clearFinishedAt: clearFinishedAtOption));
   }

   private void EditRecordHandler(RecordInput input)
   {
      if (input.Id <= 0 && !input.InteractiveMode)
      {
         _console.WriteError($"Record id {input.Id} is not valid");
         return;
      }

      try
      {
         var recordsInProgress = _dbRepository.Records.GetInProgress().ToList();

         RecordCommon.DisplayRecordsList(recordsInProgress, "Records in progres");
         _console.WriteLine();

         var record = input.InteractiveMode
            ? RecordCommon.GetOrChoose(input.Id)
            : _dbRepository.Records.Get(input.Id);

         if (record == null)
         {
            _console.WriteError("Record not found");
            return;
         }

         if (input.InteractiveMode) { RecordCommon.UpdateRecordDataInteractive(record, input); }
         else { RecordCommon.UpdateRecordData(record, input); }

         if (record.RE_FinishedAt is null && recordsInProgress.Any(r => r.RE_Id != record.RE_Id))
         {
            _console.WriteError("There could be only one record in progres. Set 'Finished at' value for edited record or finish record that is already in progress");
            return;
         }

         var success = _dbRepository.Records.Update(record);

         if (!success)
         {
            _console.WriteError("Error while updating record in database");
            return;
         }
      }
      catch (Exception ex)
      {
         _console.WriteError(ex.Message);
         return;
      }

      _console.MarkupLine("[green]Record updated successfully[/]");
      _console.WriteLine();
      RecordCommon.DisplayRecordsList(_dbRepository.Records.GetAll(), "Records");
   }
}
