using DbUp.Engine;
using System.Data;

namespace App.Migrations.MigrationScripts;

public sealed class Script_005_RenameStopedAtColumnToFinishedAtInRecordsTable : IScript
{
   private static string PreConditionCheck =>
   "SELECT count(*) FROM pragma_table_info('Records') WHERE name = 'RE_FinishedAt';";

   private static string ScriptRenamingColumnName =>
      "ALTER TABLE Records RENAME COLUMN RE_StopedAt TO RE_FinishedAt;";

   public string ProvideScript(Func<IDbCommand> dbCommandFactory)
   {
      var command = dbCommandFactory();

      command.CommandText = PreConditionCheck;

      if (Convert.ToInt64(command.ExecuteScalar() ?? 0) > 0) { return string.Empty; }

      return ScriptRenamingColumnName;
   }
}