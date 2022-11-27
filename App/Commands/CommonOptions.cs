using System.CommandLine;

namespace App.Commands;

public static class CommonOptions
{
   public static Option<bool> GetManualModeOption()
   {
      var value = new Option<bool>(
            name: "--manual",
            getDefaultValue: () => false,
            description: "Use manual mode to fill data. Only using command params."
         );
      value.AddAlias("-m");
      return value;
   }
}