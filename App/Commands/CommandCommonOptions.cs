using System.CommandLine;

namespace App.Commands;

public static class CommandCommonOptions
{
   public static Option<bool> InteractiveMode
   {
      get
      {
         var value = new Option<bool>(
               name: "--interactive",
               getDefaultValue: () => false,
               description: "Use interactive mode to fill data"
            );
         value.AddAlias("-i");
         return value;
      }
   }
}