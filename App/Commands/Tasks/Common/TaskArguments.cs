using System.CommandLine;

namespace App.Commands.Tasks.Common;

public static class TaskArguments
{
   public static Argument<int> GetIdArgument()
   {
      return new(
         name: "id",
         getDefaultValue: () => 0,
         description: "Task id"
      );
   }
}
