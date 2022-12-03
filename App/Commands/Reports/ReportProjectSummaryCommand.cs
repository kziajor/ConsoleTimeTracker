using App.Commands.Projects.Common;
using App.Commands.Reports.Common;
using App.Extensions;
using App.Repositories;
using Spectre.Console;
using System.CommandLine;
using System.Text.RegularExpressions;

namespace App.Commands.Reports;

internal class ReportProjectSummaryCommand : Command
{
   private readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();

   public ReportProjectSummaryCommand() : base("project", "Projects summary in given period of time")
   {
      var fromArgument = ReportArguments.FromArgument;
      var toArgument = ReportArguments.ToArgument;

      Add(fromArgument);
      Add(toArgument);

      this.SetHandler((from, to) => ProjectSummaryHandler(from, to), fromArgument, toArgument);
   }

   private void ProjectSummaryHandler(string from, string to)
   {
      DateTime? fromDate = ParseDate(from);

      if (fromDate is null)
      {
         _console.WriteError($"Couldn't parse input for argument 'From': {from}");
         return;
      }

      DateTime? toDate;

      if (string.IsNullOrEmpty(to))
      {
         toDate = fromDate;
      }
      else
      {
         toDate = ParseDate(to, true);
      }

      if (toDate is null)
      {
         _console.WriteError($"Couldn't parse input for argument 'To': {to}");
         return;
      }

      if (toDate < fromDate)
      {
         _console.WriteError($"'From' argument value is newer then 'To' argument value: {fromDate.Value.ToIsoDate()} > {toDate.Value.ToIsoDate()}");
         return;
      }

      var projects = _dbRepository.Projects.GetSummaryInPeriod(fromDate.Value, toDate.Value);
      var totalTime = projects.Sum(p => p.PR_TimeSpent);

      ProjectCommon.DisplaySummary(projects, totalTime, $"Project summary (from {fromDate.Value.ToIsoDate()} to {toDate.Value.ToIsoDate()})");
   }

   private static DateTime? ParseDate(string date, bool roundUp = false)
   {
      if (string.IsNullOrEmpty(date)) { return null; }

      return ParseDateFromSpecialKeywords(date, roundUp)
         ?? ParseDateFromDateString(date, roundUp)
         ?? ParseDateFromNumber(date, roundUp)
         ?? ParseDateFromPeriodModifier(date, roundUp);
   }

   private static DateTime? ParseDateFromSpecialKeywords(string date, bool roundUp)
   {
      return date.ToLower() switch
      {
         "lastmonth" => roundUp ? DateTime.Now.MonthBegin().AddTicks(-1) : DateTime.Now.MonthBegin().AddMonths(-1),
         "currentmonth" => roundUp ? DateTime.Now.MonthEnd() : DateTime.Now.MonthBegin(),
         _ => null,
      };
   }

   private static DateTime? ParseDateFromDateString(string date, bool roundUp)
   {
      return DateTime.TryParse(date, out DateTime parsedDate)
         ? roundUp ? parsedDate.DayEnd() : parsedDate.DayBegin()
         : null;
   }

   private static DateTime? ParseDateFromNumber(string date, bool roundUp)
   {
      return int.TryParse(date, out int parsedInt)
         ? roundUp ? DateTime.Now.AddDays(parsedInt).DayEnd() : DateTime.Now.AddDays(parsedInt).DayBegin()
         : null;
   }

   private static DateTime? ParseDateFromPeriodModifier(string date, bool roundUp)
   {
      try
      {
         Regex regex = new(@"^(?<number>[\-+]?\d*){1}(?<period>[mMdDyY]){1}$");
         var matchResult = regex.Match(date);

         if (matchResult.Success)
         {
            var numberPart = int.Parse(matchResult.Groups["number"].Value);
            var periodPart = matchResult.Groups["period"].Value;
            var currentDate = DateTime.Now;

            return periodPart.ToLower() switch
            {
               "d" => roundUp ? currentDate.AddDays(numberPart).DayEnd() : currentDate.AddDays(numberPart).DayBegin(),
               "m" => roundUp ? currentDate.AddMonths(numberPart).DayEnd() : currentDate.AddMonths(numberPart).DayBegin(),
               "y" => roundUp ? currentDate.AddYears(numberPart).DayEnd() : currentDate.AddYears(numberPart).DayBegin(),
               _ => roundUp ? currentDate.AddDays(numberPart).DayEnd() : currentDate.AddDays(numberPart).DayBegin()
            };
         }
      }
      catch { }

      return null;
   }
}
