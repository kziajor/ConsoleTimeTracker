using App.Commands.Records.Common;
using App.Entities;
using App.Extensions;
using App.Repositories;
using System.CommandLine;

namespace App.Commands.Records;

public class RecordDetailsCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
   private readonly IAppConsole _console = ServicesProvider.GetInstance<IAppConsole>();

   public RecordDetailsCommand() : base("details", "Display record details")
   {
      AddAlias("d");

      Add(RecordCommonArguments.Id);
      Add(CommandCommonOptions.InteractiveMode);

      this.SetHandler((recordId, interactiveMode) => RecordDetailsHandler(recordId, interactiveMode), RecordCommonArguments.Id, CommandCommonOptions.InteractiveMode);
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

      RecordCommon.ShowDetails(record);
   }
}