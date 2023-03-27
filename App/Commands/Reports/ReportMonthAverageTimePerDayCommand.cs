using App.Assets;
using App.Commands.Reports.Common;
using App.Extensions;
using App.Repositories;

using Spectre.Console;

using System.CommandLine;

namespace App.Commands.Reports;

internal class ReportMonthAverageTimePerDayCommand : Command
{
   private readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();

   public ReportMonthAverageTimePerDayCommand() : base("month-average", "Average time spent on tasks per day in given month")
   {
      var monthOffset = ReportArguments.MonthOffset;

      Add(monthOffset);

      this.SetHandler((monthOffset) => ProjectSummaryHandler(monthOffset), monthOffset);
   }

   private void ProjectSummaryHandler(int monthOffset)
   {
      if (monthOffset > 0)
      {
         _console.WriteError("Couldn't calculate average time for future months");
         return;
      }

      var monthStart = DateTime.Now.AddMonths(monthOffset).MonthBegin();
      var monthEnd = monthStart.MonthEnd();
      var records = _dbRepository.Records.GetByMonth(monthStart);

      if (records is not null)
      {
         var workingDays = records.Select(r => r.RE_StartedAt.Date).Distinct().Count();
         var totalHours = records.Sum(r => r.TimeSpentHours);
         var average = workingDays > 0 ? totalHours / workingDays : totalHours;

         _console.MarkupLineInterpolated($"Period of time: [{Colors.Primary}]{monthStart.ToIsoDate()} - {monthEnd.ToIsoDate()}[/]");
         _console.MarkupLineInterpolated($"Average time per day: [{Colors.Primary}]{average:N2} hours[/]");

         return;
      }

      _console.MarkupLineInterpolated($"[{Colors.Warning}]Records not found in given period of time: {monthStart.ToIsoDate()} - {monthEnd.ToIsoDate()}[/]");
   }
}
