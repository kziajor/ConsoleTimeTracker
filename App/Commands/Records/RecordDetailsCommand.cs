using App.Commands.Records.Common;
using App.Entities;
using App.Extensions;
using App.Repositories;

using Spectre.Console;

using System.CommandLine;

namespace App.Commands.Records;

public class RecordDetailsCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
   private readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();

   public RecordDetailsCommand() : base("show", "Display record details")
   {
      var idArgument = RecordArguments.GetIdArgument();
      var manualMode = CommonOptions.GetManualModeOption();

      Add(idArgument);
      Add(manualMode);

      this.SetHandler((recordId, manualMode) => RecordDetailsHandler(recordId, manualMode), idArgument, manualMode);
   }

   private void RecordDetailsHandler(int recordId, bool manualMode)
   {
      Record? record = manualMode
         ? _dbRepository.Records.Get(recordId)
         : RecordCommon.GetOrChoose(recordId);

      if (record is null)
      {
         _console.WriteError("Record not found");
         return;
      }

      RecordCommon.DisplayDetails(record);
   }
}