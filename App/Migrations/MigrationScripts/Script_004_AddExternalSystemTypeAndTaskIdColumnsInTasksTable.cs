using DbUp.Engine;
using System.Data;

namespace App.Migrations.MigrationScripts;

public class Script_004_AddExternalSystemTypeAndTaskIdColumnsInTasksTable : IScript
{
   private string _scriptAddingExternalSystemTypeColumn =>
      @"
      ALTER TABLE tasks
      ADD COLUMN external_system_type INTEGER NULL;
      ";

   private string _scriptAddingExternalSystemTaskIdColumn =>
      @"
      ALTER TABLE tasks
      ADD COLUMN external_system_task_id INTEGER NULL;
      ";

   private string _preConditionCheckForExternalSystemTypeColumn =>
      "SELECT count(*) FROM pragma_table_info('tasks') WHERE name = 'external_system_type';";

   private string _preConditionCheckForExternalSystemTaskIdColumn =>
      "SELECT count(*) FROM pragma_table_info('tasks') WHERE name = 'external_system_task_id';";

   public string? PostConditionCheck => throw new NotImplementedException();

   public string ProvideScript(Func<IDbCommand> dbCommandFactory)
   {
      var command = dbCommandFactory();
      command.CommandText = _preConditionCheckForExternalSystemTypeColumn;

      var externalSystemTypeColumnExists = Convert.ToInt64(command.ExecuteScalar() ?? 0);

      command.CommandText = _preConditionCheckForExternalSystemTaskIdColumn;

      var externalSystemTaskIdColumnExists = Convert.ToInt64(command.ExecuteScalar() ?? 0);

      var result = externalSystemTypeColumnExists == 0 ? _scriptAddingExternalSystemTypeColumn : "";
      result += externalSystemTaskIdColumnExists == 0 ? _scriptAddingExternalSystemTaskIdColumn : "";

      return result;
   }
}
