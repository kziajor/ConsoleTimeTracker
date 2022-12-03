using DbUp.Engine;

using System.Data;

namespace DbMigrations.MigrationScripts;

public sealed class Script_006_ChangeExternalSystemTypeAndExternalSystemTaskIdColumnsInTaskTableToNotNull : IScript
{
    private static string CheckExternalSystemTypeColumnIsNullable => "SELECT COUNT([notnull]) FROM pragma_table_info('Tasks') WHERE name = 'TA_ExternalSystemType' OR name = 'TA_ExternalSystemTaksId'";

    private static string ChangeExternalSystemTypeColumnToNotNull =>
       @"
      PRAGMA foreign_keys=off;

      ALTER TABLE Tasks RENAME TO Tasks_AlterSchemaTempTable;
      ALTER TABLE Records RENAME TO Records_AlterSchemaTempTable;

      CREATE TABLE Tasks (
        TA_Id INTEGER PRIMARY KEY AUTOINCREMENT,
        TA_Title TEXT NOT NULL,
        TA_PlannedTime INTEGER NOT NULL DEFAULT 0,
        TA_Closed NUMERIC NOT NULL DEFAULT 0,
        TA_RelProjectId INTEGER NOT NULL REFERENCES Projects(PR_Id),
        TA_ExternalSystemType INTEGER NOT NULL,
        TA_ExternalSystemTaskId TEXT NOT NULL
      );

      CREATE TABLE Records (
	      RE_Id INTEGER PRIMARY KEY AUTOINCREMENT,
	      RE_StartedAt DATETIME NOT NULL,
	      RE_FinishedAt DATETIME NULL,
	      RE_MinutesSpent INTEGER NOT NULL DEFAULT 0,
	      RE_Comment TEXT,
	      RE_RelTaskId INTEGER NOT NULL REFERENCES ""Tasks""(TA_Id)
      );

      INSERT INTO Tasks
      SELECT
         TA_Id,
         TA_Title,
         TA_PlannedTime,
         TA_Closed,
         TA_RelProjectId,
         CASE TA_ExternalSystemType
            WHEN 0 THEN 1
            ELSE 0
         END TA_ExternalSystemType,
         CASE TA_ExternalSystemType
            WHEN 0 THEN TA_ExternalSystemTaskId
            ELSE ''
         END TA_ExternalSystemTaskId
      FROM Tasks_AlterSchemaTempTable;

      INSERT INTO Records
      SELECT * FROM Records_AlterSchemaTempTable;

      DROP TABLE Records_AlterSchemaTempTable;
      DROP TABLE Tasks_AlterSchemaTempTable;

      PRAGMA foreign_keys=off;
      ";

    public string ProvideScript(Func<IDbCommand> dbCommandFactory)
    {
        var command = dbCommandFactory();

        command.CommandText = CheckExternalSystemTypeColumnIsNullable;

        if (Convert.ToInt64(command.ExecuteScalar()) > 1) { return string.Empty; }

        return ChangeExternalSystemTypeColumnToNotNull;
    }
}