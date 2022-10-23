using DbUp.Engine;
using System.Data;

namespace App.Migrations.MigrationScripts;

public class Script_003_CreateRecordsTable : IScript
{
   public string ProvideScript(Func<IDbCommand> dbCommandFactory) =>
@"
CREATE TABLE IF NOT EXISTS records (
   id INTEGER PRIMARY KEY AUTOINCREMENT,
   started_at DATETIME NOT NULL,
   stoped_at DATETIME NULL,
   minutes_spent INTEGER NOT NULL DEFAULT 0,
   comment TEXT,
   rel_task_id INTEGER NOT NULL REFERENCES tasks(id)
)
";
}
