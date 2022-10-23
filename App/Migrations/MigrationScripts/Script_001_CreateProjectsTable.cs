using DbUp.Engine;
using System.Data;

namespace App.Migrations.MigrationScripts;

public class Script_001_CreateProjectsTable : IScript
{
   public string ProvideScript(Func<IDbCommand> dbCommandFactory) =>
@"
CREATE TABLE IF NOT EXISTS projects (
   id INTEGER PRIMARY KEY AUTOINCREMENT,
   name TEXT,
   closed NUMERIC
)
";
}
