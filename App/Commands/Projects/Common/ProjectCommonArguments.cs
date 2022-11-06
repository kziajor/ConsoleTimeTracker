using System.CommandLine;

namespace App.Commands.Projects.Common;

public static class ProjectCommonArguments
{
   public static Argument<int> Id => new(
      name: "id",
      getDefaultValue: () => 0,
      description: "Project id"
   );
}