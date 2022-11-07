using System.CommandLine;

namespace App.Commands.Records.Common;

public static class RecordOptions
{
   public static Option<int?> GetTaskIdOption()
   {
      var value = new Option<int?>(
         name: "--task-id",
         getDefaultValue: () => null,
         description: "Task Id");
      value.AddAlias("-t");
      return value;
   }

   public static Option<DateTime?> GetStartedAtOption()
   {
      var value = new Option<DateTime?>(
         name: "--start-at",
         getDefaultValue: () => null,
         description: "Start at");
      value.AddAlias("-s");
      return value;
   }

   public static Option<DateTime?> GetFinishedAtOption()
   {
      var value = new Option<DateTime?>(
         name: "--finished-at",
         getDefaultValue: () => null,
         description: "Finished at");
      value.AddAlias("-f");
      return value;
   }

   public static Option<string?> GetCommentOption()
   {
      var value = new Option<string?>(
         name: "--comment",
         getDefaultValue: () => null);
      value.AddAlias("-c");
      return value;
   }

   public static Option<bool> GetClearFinishedAtOption()
   {
      return new(
         name: "--clear-finished-at",
         getDefaultValue: () => false,
         description: "Clear finished at value"
      );
   }

   public static Option<bool> GetClearCommentOption()
   {
      return new(
         name: "--clear-comment",
         getDefaultValue: () => false,
         description: "Clear comment value"
      );
   }
}