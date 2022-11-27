using System.CommandLine;

namespace App.Commands.Projects.Common;

public static class ProjectArguments
{
   public static Argument<int> GetIdArgument()
   {
      return new Argument<int>(name: "id",
         getDefaultValue: () => 0,
         description: "Project id"
      );
   }
}