using App.Commands.Records.Common;
using App.Repositories;

using System.CommandLine;

namespace App.Commands.Records;

public class RecordCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();

   public RecordCommand() : base("record", "Manage time tracker records")
   {
      var dayArgument = RecordArguments.GetDayArgument();

      Add(dayArgument);

      Add(new RecordAddCommand());
      Add(new RecordEditCommand());
      Add(new RecordDetailsCommand());
      Add(new RecordCountCommand());

      this.SetHandler(day => RecordListHandle(day), dayArgument);
   }

   private void RecordListHandle(string? day)
   {
      DateTime? fromDay = null;

      if (string.IsNullOrEmpty(day)) { fromDay = DateTime.Now; }
      else if (DateTime.TryParse(day, out DateTime parsedDay)) { fromDay = parsedDay; }
      else if (int.TryParse(day, out int parsedDayCount)) { fromDay = DateTime.Now.AddDays(parsedDayCount); }

      var records = _dbRepository.Records.GetByDay(fromDay ??= DateTime.Now);

      RecordCommon.DisplayList(records, $"Records - {fromDay.Value:yyyy-MM-dd} ({fromDay.Value.DayOfWeek})");
   }
}
