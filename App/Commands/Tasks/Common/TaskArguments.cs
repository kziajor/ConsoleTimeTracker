using System.CommandLine;

namespace App.Commands.Tasks.Common;

public static class TaskArguments
{
   public static Argument<string?> GetIdArgument()
   {
      return new(
         name: "id",
         getDefaultValue: () => null,
         description: "Task id"
      );
   }
}
