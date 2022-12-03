using DbUp;
using DbUp.Engine;

namespace DbMigrations;

public static class DbMigrator
{
   public static void Migrate(string connectionString)
   {
      var migratorBuilder = DeployChanges.To
         .SQLiteDatabase(connectionString)
         .WithTransaction()
         .WithScripts(GetScripts().ToArray());

#if DEBUG
      migratorBuilder.LogToConsole();
#else
      migratorBuilder.LogToNowhere();
#endif

      var engine = migratorBuilder.Build();

      if (!engine.IsUpgradeRequired()) { return; }

      engine.PerformUpgrade();
   }

   private static IEnumerable<IScript> GetScripts()
   {
      return AppDomain.CurrentDomain
         .GetAssemblies()
         .SelectMany(a => a.GetTypes())
         .Where(t => typeof(IScript).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && t.Name.StartsWith("Script_"))
         .Select(t => Activator.CreateInstance(t))
         .Where(o => o is not null)
         .Select(o => (IScript)o!)
         .OrderBy(s => nameof(s));
   }
}
