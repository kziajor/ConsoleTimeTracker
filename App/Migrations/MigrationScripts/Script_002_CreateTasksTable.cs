using DbUp.Engine;
using System.Data;

namespace App.Migrations.MigrationScripts;

public sealed class Script_002_CreateTasksTable : IScript
{
   public string ProvideScript(Func<IDbCommand> dbCommandFactory) =>
      @"
         CREATE TABLE IF NOT EXISTS Tasks (
            TA_Id INTEGER PRIMARY KEY AUTOINCREMENT,
            TA_Title TEXT NOT NULL,
            TA_PlannedTime INTEGER NOT NULL DEFAULT 0,
            TA_Closed NUMERIC NOT NULL DEFAULT 0,
            TA_RelProjectId INTEGER NOT NULL REFERENCES Projects(PR_Id)
         )
      ";
}