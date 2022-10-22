namespace App.Migrations.MigrationScripts;

public class Script002_CreateTasksTable : ISqlScript
{
   public string Script =>
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