using App.Repositories;

using System.CommandLine;

namespace App.Commands.Records;

public class RecordCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();

   public RecordCommand() : base("record", "Manage time tracker records")
   {
      AddAlias("r");

      Add(new RecordAddCommand());
      Add(new RecordEditCommand());
      Add(new RecordDetailsCommand());
      Add(new RecordStartCommand());

      // TODO: Add param displaing records grouped by day and maybe setting
      // TODO: Setting to set from how many days should be displayed records by default
      // TODO: Param to set how many days sholud be displayed.

      this.SetHandler(() => RecordListHandle());
   }

   private void RecordListHandle()
   {
      var records = _dbRepository.Records.GetAll();

      RecordCommon.DisplayList(records);
   }
}
