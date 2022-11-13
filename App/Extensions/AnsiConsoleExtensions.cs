using App.Assets;
using Spectre.Console;

namespace App.Extensions;

public static class AnsiConsoleExtensions
{
   public static void WriteKeyValuePair(this IAnsiConsole console, string key, string value)
   {
      console.MarkupLineInterpolated($"[green]{key}[/]: {value}");
   }

   public static void WriteError(this IAnsiConsole console, string errorMessage)
   {
      console.MarkupLineInterpolated($"[red]{errorMessage.EscapeMarkup()}[/]");
   }

   public static void WriteErrorAndExit(this IAnsiConsole console, string errorMessage, int errorCode = 0)
   {
      if (errorCode > 0)
      {
         errorMessage = $"[Error code: {errorCode}] " + errorMessage;
      }
      console.WriteError($"[red]{errorMessage.EscapeMarkup()}[/]");

      Environment.Exit(errorCode);
   }

   public static Grid AddKeyValueRow<T>(this Grid grid, string key, T? value, Color? valueColor = null)
   {
      var valueString = value is not null
         ? value switch
         {
            DateTime v => v.ToIsoString(),
            _ => value?.ToString() ?? string.Empty,
         }
         : string.Empty;

      grid.AddRow(new Text[]
      {
         new Text($"{key}:", new Style(Color.White)).RightAligned(),
         new Text(valueString, new Style(valueColor ?? Colors.Primary)).LeftAligned(),
      });

      return grid;
   }
}
