namespace App.Migrations.MigrationScripts;

public class Script003_CreateRecordsTable : ISqlScript
{
   public string Script =>
@"
CREATE TABLE records (
   id INTEGER PRIMARY KEY AUTOINCREMENT,
   started_at DATETIME NOT NULL,
   stoped_at DATETIME NULL,
   minutes_spent INTEGER NOT NULL DEFAULT 0,
   comment TEXT,
   rel_task_id INTEGER NOT NULL REFERENCES tasks(id)
)
";
}
