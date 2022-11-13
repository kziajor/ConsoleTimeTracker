namespace App.Extensions;

public static class IntExtensions
{
   public static double MinutesToHours(this int minutes)
   {
      return Math.Round((double)minutes / 60, 2);
   }
}
