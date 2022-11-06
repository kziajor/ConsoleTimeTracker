namespace App.Extensions;

public static class DateTimeExtensions
{
   public static string ToIsoString(this DateTime dateTime)
   {
      return dateTime.ToString("yyyy-MM-dd HH:mm");
   }
}
