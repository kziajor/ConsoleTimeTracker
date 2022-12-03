namespace App.Extensions;

public static class DateTimeExtensions
{
   public static string ToIsoDateTime(this DateTime dateTime)
   {
      return dateTime.ToString("yyyy-MM-dd HH:mm");
   }

   public static string ToIsoDate(this DateTime dateTime)
   {
      return dateTime.ToString("yyyy-MM-dd");
   }

   public static DateTime MonthBegin(this DateTime dateTime)
   {
      return new DateTime(dateTime.Year, dateTime.Month, 1);
   }

   public static DateTime MonthEnd(this DateTime dateTime)
   {
      return dateTime.MonthBegin().AddMonths(1).AddTicks(-1);
   }

   public static DateTime DayBegin(this DateTime dateTime)
   {
      return dateTime.Date;
   }

   public static DateTime DayEnd(this DateTime dateTime)
   {
      return dateTime.Date.AddDays(1).AddTicks(-1);
   }
}
