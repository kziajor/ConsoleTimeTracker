using System.CommandLine;

namespace App.Commands.Reports.Common;

internal static class ReportArguments
{
   internal static Argument<string> FromArgument
   {
      get => new
      (
         name: "from",
         description: "Give report period start date (number - how many days from now, date - exact date. Special keywords available: lastmonth, currentmonth"
      );
   }

   internal static Argument<string> ToArgument
   {
      get => new
      (
         name: "to",
         getDefaultValue: () => string.Empty,
         description: "Give report period end date (number - how many days from now, date - exact date. Special keywords available: lastmonth, currentmonth. If no value given then value of argument 'From' is used as value of argument 'To' also"
      );
   }

   internal static Argument<int> MonthOffset
   {
      get => new
      (
          name: "month-offset",
          getDefaultValue: () => 0,
          description: "Month offset from now. For current month give 0 or no argument. For last month give -1. Etc."
      );
   }
}