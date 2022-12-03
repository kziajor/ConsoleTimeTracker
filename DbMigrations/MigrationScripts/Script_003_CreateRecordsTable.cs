using DbUp.Engine;
using System.Data;

namespace DbMigrations.MigrationScripts;

public sealed class Script_003_CreateRecordsTable : IScript
{
    public string ProvideScript(Func<IDbCommand> dbCommandFactory) =>
       @"
         CREATE TABLE IF NOT EXISTS Records (
            RE_Id INTEGER PRIMARY KEY AUTOINCREMENT,
            RE_StartedAt DATETIME NOT NULL,
            RE_StopedAt DATETIME NULL,
            RE_MinutesSpent INTEGER NOT NULL DEFAULT 0,
            RE_Comment TEXT,
            RE_RelTaskId INTEGER NOT NULL REFERENCES Tasks(TA_Id)
         )
      ";
}
