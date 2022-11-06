using App.Commands.Records.Common;
using App.Extensions;
using App.Models.Inputs;
using App.Repositories;
using Spectre.Console;
using System.CommandLine;

namespace App.Commands.Records
{
   public class RecordAddCommand : Command
   {
      private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
      private readonly IAppConsole _console = ServicesProvider.GetInstance<IAppConsole>();

      public RecordAddCommand() : base("add", "Add new record")
      {
         AddAlias("a");

         Add(RecordCommonOptions.TaskId);
         Add(RecordCommonOptions.StartedAt);
         Add(RecordCommonOptions.FinishedAt);
         Add(RecordCommonOptions.Comment);
         Add(CommandCommonOptions.InteractiveMode);

         this.SetHandler(
            (recordInput) => AddRecordHandler(recordInput),
            new RecordInputBinder(
               taskId: RecordCommonOptions.TaskId,
               startedAt: RecordCommonOptions.StartedAt,
               finishedAt: RecordCommonOptions.FinishedAt,
               comment: RecordCommonOptions.Comment,
               interactiveMode: CommandCommonOptions.InteractiveMode
            )
         );
      }

      private void AddRecordHandler(RecordInput input)
      {
         try
         {
            var recordsInProgress = _dbRepository.Records.GetInProgress().ToList();
            RecordCommon.DisplayRecordsList(recordsInProgress, "Records in progres");
            _console.WriteLine();

            var record = input.InteractiveMode
               ? RecordCommon.CreateNewRecordInteractive(input)
               : RecordCommon.CreateNewRecord(input);

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
            }
         }
         catch (Exception ex)
         {
            _console.WriteError(ex.Message);
            return;
         }

         _console.MarkupLine("[green]New record added[/]");
         _console.WriteLine();
         RecordCommon.DisplayRecordsList(_dbRepository.Records.GetAll(), "Records");
      }
   }
}
