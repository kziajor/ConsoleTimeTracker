using DbUp.Engine;
using System.Data;

namespace DbMigrations.MigrationScripts;

public sealed class Script_001_CreateProjectsTable : IScript
{
    public string ProvideScript(Func<IDbCommand> dbCommandFactory) =>
       @"
         CREATE TABLE IF NOT EXISTS Projects (
            PR_Id INTEGER PRIMARY KEY AUTOINCREMENT,
            PR_Name TEXT,
            PR_Closed NUMERIC
         )
      ";
}
