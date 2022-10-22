using DbUp;
using DbUp.Engine;

namespace App.Migrations;

public static class DbMigrator
{
   public static void Migrate(string connectionString)
   {
      var migratorBuilder = DeployChanges.To
         .SQLiteDatabase(connectionString)
         .WithTransaction()
         .WithScripts(GetScripts());

#if DEBUG
      migratorBuilder.LogToConsole();
#else
      migratorBuilder.LogToNowhere();
#endif

      var engine = migratorBuilder.Build();

      if (!engine.IsUpgradeRequired()) { return; }

      engine.PerformUpgrade();
   }

   private static IEnumerable<SqlScript> GetScripts()
   {
      var scriptsNamespace = $"{typeof(DbMigrator).Namespace}.{nameof(MigrationScripts)}";
      var result = AppDomain.CurrentDomain
         .GetAssemblies()
         .SelectMany(a => a.GetTypes())
         .Where(t => typeof(ISqlScript).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && t.Namespace == scriptsNamespace)
         .Select(t =>
         {
            var scriptContent = ((ISqlScript?)Activator.CreateInstance(t)!).Script;
            return new SqlScript(t.FullName, scriptContent);
         })
         .OrderBy(script => script.Name);

      return result;
   }
}
