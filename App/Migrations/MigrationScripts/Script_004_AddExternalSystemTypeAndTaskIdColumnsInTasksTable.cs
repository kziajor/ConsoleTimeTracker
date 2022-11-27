using DbUp.Engine;
using System.Data;

namespace App.Migrations.MigrationScripts;

public sealed class Script_004_AddExternalSystemTypeAndTaskIdColumnsInTasksTable : IScript
{
   private static string ScriptAddingExternalSystemTypeColumn =>
      @"
         ALTER TABLE Tasks
         ADD COLUMN TA_ExternalSystemType INTEGER NULL;
      ";

   private static string ScriptAddingExternalSystemTaskIdColumn =>
      @"
         ALTER TABLE Tasks
         ADD COLUMN TA_ExternalSystemTaskId TEXT NULL;
      ";

   private static string PreConditionCheckForExternalSystemTypeColumn =>
      "SELECT count(*) FROM pragma_table_info('Tasks') WHERE name = 'TA_ExternalSystemType';";

   private static string PreConditionCheckForExternalSystemTaskIdColumn =>
      "SELECT count(*) FROM pragma_table_info('Tasks') WHERE name = 'TA_ExternalSystemTaskId';";

   public string ProvideScript(Func<IDbCommand> dbCommandFactory)
   {
      var command = dbCommandFactory();
      command.CommandText = PreConditionCheckForExternalSystemTypeColumn;

      var externalSystemTypeColumnExists = Convert.ToInt64(command.ExecuteScalar() ?? 0);

      command.CommandText = PreConditionCheckForExternalSystemTaskIdColumn;

      var externalSystemTaskIdColumnExists = Convert.ToInt64(command.ExecuteScalar() ?? 0);

      var result = externalSystemTypeColumnExists == 0 ? ScriptAddingExternalSystemTypeColumn : "";
      result += externalSystemTaskIdColumnExists == 0 ? ScriptAddingExternalSystemTaskIdColumn : "";

      return result;
   }
}
