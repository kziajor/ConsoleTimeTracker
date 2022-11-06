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

      this.SetHandler(() => RecordListHandle());
   }

   private void RecordListHandle()
   {
      var records = _dbRepository.Records.GetAll();

      RecordCommon.DisplayRecordsList(records);
   }
}
