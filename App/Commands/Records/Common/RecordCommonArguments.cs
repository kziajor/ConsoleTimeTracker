using System.CommandLine;

namespace App.Commands.Records.Common;

public static class RecordCommonArguments
{
   public static Argument<int> Id => new(
      name: "id",
      getDefaultValue: () => 0,
      description: "Record Id"
   );
}