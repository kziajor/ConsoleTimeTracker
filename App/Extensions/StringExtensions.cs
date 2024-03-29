﻿namespace App.Extensions;

public static class StringExtensions
{
   public static bool IsNullOrEmpty(this string @string)
   {
      return string.IsNullOrEmpty(@string);
   }

   public static bool IsNotNullOrEmpty(this string @string)
   {
      return !string.IsNullOrEmpty(@string);
   }

   public static bool IsInt(this string @string)
   {
      return int.TryParse(@string, out _);
   }

   public static int? ToInt(this string @string)
   {
      return int.TryParse(@string, out int result) ? result : null;
   }
}
