using System.CommandLine;

namespace App.Commands.Records.Common;

public static class RecordArguments
{
   public static Argument<int> GetIdArgument()
   {
      return new(
         name: "id",
         getDefaultValue: () => 0,
         description: "Record Id"
      );
   }
}