using App.Commands.Projects;
using App.Commands.Tasks;
using App.Migrations;
using App.Repositories;

using Spectre.Console;

using System.CommandLine;

namespace App;

static class Program
{
   static async Task<int> Main(string[] args)
   {
      RegisterServices();

      var console = ServicesProvider.GetInstance<IAppConsole>();
      var settingsProvider = ServicesProvider.GetInstance<ISettingsProvider>();

      if (settingsProvider.ClearConsoleAfterEveryCommand) { console.Clear(); }

      DisplayTitle(console, settingsProvider);
      EnsureDbDataDirectoryExists(settingsProvider.DbFile);
      DbMigrator.Migrate(settingsProvider.ConnectionString);

      var rootCommand = new RootCommand("Console Time Tracker")
      {
         Name = "TT",
      };

      RegisterCommands(rootCommand);

      ShowBasicInfo(console, settingsProvider);
      console.WriteLine();

      var result = await rootCommand.InvokeAsync(args);

      console.WriteLine();
      console.Write(new Rule().RuleStyle("green"));

      return result;
   }

   private static void RegisterServices()
   {
      var settingsProvider = new SettingsProvider();
      var dbRepository = new DbRepository(settingsProvider.ConnectionString);

      ServicesProvider.Register<IAppConsole, object>(AnsiConsole.Console);
      ServicesProvider.Register<ISettingsProvider, SettingsProvider>(settingsProvider);
      ServicesProvider.Register<IDbRepository, DbRepository>(dbRepository);
   }
   private static void RegisterCommands(RootCommand rootCommand)
   {
      rootCommand.Add(new ProjectCommand());
   }
   private static void DisplayTitle(IAppConsole console, ISettingsProvider settingsProvider)
   {
      console.WriteLine();
      if (settingsProvider.DisplayLargeAppName)
      {
         console.Write(new Rule().RuleStyle("green"));
         console.Write(new FigletText("ConsoleTT").Color(Color.Green));
      }
      else
      {
         var rule = new Rule("[green]ConsoleTT[/]")
            .LeftAligned()
            .RuleStyle("green");

         console.Write(rule);
         console.WriteLine();
      }
   }

   private static void EnsureDbDataDirectoryExists(FileInfo dbDataFile)
   {
      if (dbDataFile.Directory is null)
      {
         throw new ArgumentException($"Data directory error for path: {dbDataFile.FullName}");
      }

      if (!dbDataFile.Directory.Exists)
      {
         Directory.CreateDirectory(dbDataFile.Directory.FullName);
      }
   }

   private static void ShowBasicInfo(IAppConsole console, ISettingsProvider settingsProvider)
   {
      console.WriteLine();
      console.MarkupLine($"Database file: [green]{settingsProvider.DbFile.FullName}[/]");
      console.WriteLine();
   }
}