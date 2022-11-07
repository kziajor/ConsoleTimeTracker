using System.CommandLine;

namespace App.Commands.Tasks.Common;

public static class TaskOptions
{
   public static Option<string?> GetTitleOption()
   {
      var value = new Option<string?>(
         name: "--title",
         getDefaultValue: () => string.Empty,
         description: "Task title");
      value.AddAlias("-t");
      return value;
   }

   public static Option<bool?> GetClosedOption()
   {
         var value = new Option<bool?>(
            name: "--closed",
            getDefaultValue: () => null,
            description: "Task closed");
         value.AddAlias("-c");
         return value;
   }

   public static Option<int?> GetProjectIdOption()
   {
         var value = new Option<int?>(
            name: "--project-id",
            getDefaultValue: () => null,
            description: "Project id");
         value.AddAlias("-p");
         return value;
   }

   public static Option<int?> GetPlannedTimeOption()
   {
      return new(
         name: "--planned-time",
         getDefaultValue: () => null,
         description: "Planned time"
      );
   }
}