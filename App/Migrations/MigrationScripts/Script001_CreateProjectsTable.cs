namespace App.Migrations.MigrationScripts;

public class Script001_CreateProjectsTable : ISqlScript
{
   public string Script =>
@"
CREATE TABLE IF NOT EXISTS projects (
   id INTEGER PRIMARY KEY AUTOINCREMENT,
   name TEXT,
   closed NUMERIC
)
";
}
