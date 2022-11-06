using System.CommandLine;

namespace App.Commands.Projects.Common;

public static class ProjectCommonOptions
{
   public static Option<string?> Name
   {
      get
      {
         var value = new Option<string?>(
               name: "--name",
               getDefaultValue: () => null,
               description: "Project name");
         value.AddAlias("-n");
         return value;
      }
   }

   public static Option<bool?> Closed
   {
      get
      {
         var value = new Option<bool?>(
            name: "--closed",
            getDefaultValue: () => null,
            description: "Project active"
         );
         value.AddAlias("-c");
         return value;
      }
   }
}