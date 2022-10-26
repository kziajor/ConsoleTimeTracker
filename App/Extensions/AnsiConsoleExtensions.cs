using Spectre.Console;

namespace App.Extensions;

public static class AnsiConsoleExtensions
{
   public static void WriteKeyValuePair(this IAnsiConsole console, string key, string value)
   {
      console.MarkupLineInterpolated($"[green]{key}[/]: {value}");
   }
}
