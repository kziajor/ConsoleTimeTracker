using System.CommandLine;

namespace App.Commands.Tasks.Common;

public static class TaskCommonArguments
{
   public static Argument<int> Id => new(
      name: "id",
      getDefaultValue: () => 0,
      description: "Task id");
}
