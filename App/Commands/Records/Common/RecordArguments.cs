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

    public static Argument<string> GetDayArgument()
    {
        return new(
           name: "day",
           getDefaultValue: () => DateTime.Now.ToString("yyyy-MM-dd"),
           description: "Display records from given day. You can input date in format 'YYYY-MM-DD' or a number which means number of days from now (example: -1 means one day before now)"
        );
    }
}