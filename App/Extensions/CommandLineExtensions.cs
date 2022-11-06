using System.CommandLine;
using System.CommandLine.Parsing;

namespace App.Extensions;

public static class CommandLineExtensions
{
   public static T? GetValueForOptionOrDefault<T>(this ParseResult parseResult, Option<T>? option)
   {
      return option is not null ? parseResult.GetValueForOption(option) : default;
   }

   public static T? GetValueForArgumentOrDefault<T>(this ParseResult parseResult, Argument<T>? argument)
   {
      return argument is not null ? parseResult.GetValueForArgument(argument) : default;
   }
}