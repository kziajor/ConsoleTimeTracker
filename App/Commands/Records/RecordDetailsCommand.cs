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

   public RecordDetailsCommand() : base("details", "Display record details")
   {
      AddAlias("d");

      var idArgument = RecordArguments.GetIdArgument();
      var interactiveMode = CommonOptions.GetInteractiveModeOption();

      Add(idArgument);
      Add(interactiveMode);

      this.SetHandler((recordId, interactiveMode) => RecordDetailsHandler(recordId, interactiveMode), idArgument, interactiveMode);
   }

   private void RecordDetailsHandler(int recordId, bool interactiveMode)
   {
      Record? record = interactiveMode
         ? RecordCommon.GetOrChoose(recordId)
         : _dbRepository.Records.Get(recordId);

      if (record is null)
      {
         _console.WriteError("Record not found");
         return;
      }

      RecordCommon.DisplayDetails(record);
   }
}