using DbUp.Engine;
using System.Data;

namespace DbMigrations.MigrationScripts;

internal class Script_007_ChangeExternalSystemTypeAndExternalSystemTaskIdColumnsNamesInTasksTable : IScript
{
    private static string CheckExternalSystemTypeColumnExistsQuery =>
       "SELECT count(*) FROM pragma_table_info('Tasks') WHERE name = 'TA_ExternalSystemType'";
    private static string CheckExternalSystemTaskIdExistsQuery =>
       "SELECT count(*) FROM pragma_table_info('Tasks') WHERE name = 'TA_ExternalSystemTaskId'";
    private static string ChangeExternalSystemTypeColumnNameQuery =>
       "ALTER TABLE Tasks RENAME COLUMN TA_ExternalSystemType TO TA_SourceType;";
    private static string ChangeExternalSystemTaskIdColumnNameQuery =>
       "ALTER TABLE Tasks RENAME COLUMN TA_ExternalSystemTaskId TO TA_SourceTaskId;";

    public string ProvideScript(Func<IDbCommand> dbCommandFactory)
    {
        var command = dbCommandFactory();

        command.CommandText = CheckExternalSystemTypeColumnExistsQuery;
        var externalSystemTypeColumnExists = Convert.ToInt64(command.ExecuteScalar()) > 0;

        command.CommandText = CheckExternalSystemTaskIdExistsQuery;
        var externalSystemTaskIdColumnExists = Convert.ToInt64(command.ExecuteScalar()) > 0;

        var result = externalSystemTypeColumnExists ? ChangeExternalSystemTypeColumnNameQuery : string.Empty;
        result += externalSystemTaskIdColumnExists ? ChangeExternalSystemTaskIdColumnNameQuery : string.Empty;

        return result;
    }
}
