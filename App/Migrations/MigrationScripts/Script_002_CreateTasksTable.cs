using DbUp.Engine;
using System.Data;

namespace App.Migrations.MigrationScripts;

public class Script_002_CreateTasksTable : IScript
{
   public string ProvideScript(Func<IDbCommand> dbCommandFactory) =>
@"
CREATE TABLE IF NOT EXISTS tasks (
   id INTEGER PRIMARY KEY AUTOINCREMENT,
   title TEXT NOT NULL,
   planned_time INTEGER NOT NULL DEFAULT 0,
   closed NUMERIC NOT NULL DEFAULT 0,
   rel_project_id INTEGER NOT NULL REFERENCES projects(id)
)
";
}