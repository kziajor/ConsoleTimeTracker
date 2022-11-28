using App.Commands.Projects.Common;
using App.Commands.Records.Common;
using App.Commands.Tasks.Common;
using App.Repositories;
using Spectre.Console;
using System.CommandLine;

namespace App.Commands.Records;

public class RecordCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
   private readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();

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
      var totalTimeSpent = records.Sum(r => r.RE_MinutesSpent);
      var headerDatePostfix = $"{fromDay.Value:yyyy-MM-dd} ({fromDay.Value.DayOfWeek})";

      ProjectCommon.DisplaySummary(records.GetProjectsSummary(), totalTimeSpent, $"Projects - {headerDatePostfix}");
      _console.WriteLine();
      _console.Write(new Rule().RuleStyle("green"));
      _console.WriteLine();

      TaskCommon.DisplaySummary(records.GetTasksSummary(), totalTimeSpent, $"Tasks - {headerDatePostfix}");
      _console.WriteLine();
      _console.Write(new Rule().RuleStyle("green"));
      _console.WriteLine();

      RecordCommon.DisplayList(records, $"Records - {headerDatePostfix}");
   }
}
