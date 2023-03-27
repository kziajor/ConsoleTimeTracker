using System.CommandLine;

namespace App.Commands.Reports;

internal class ReportCommand : Command
{
   public ReportCommand() : base("report", "Reports")
   {
      Add(new ReportProjectSummaryCommand());
      Add(new ReportMonthAverageTimePerDayCommand());
   }
}