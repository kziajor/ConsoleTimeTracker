using App.Commands.Projects;
using App.Commands.Records;
using App.Commands.Tasks;
using App.Repositories;
using DbMigrations;
using Spectre.Console;

using System.CommandLine;

namespace App;

static class Program
{
   static async Task<int> Main(string[] args)
   {
      System.Console.OutputEncoding = System.Text.Encoding.UTF8; // FIX for displaing emoji icons
      RegisterServices();

      var console = ServicesProvider.GetInstance<IAnsiConsole>();
      var settingsProvider = ServicesProvider.GetInstance<ISettingsProvider>();

      if (settingsProvider.ClearConsoleAfterEveryCommand) { console.Clear(); }

      DisplayHeader(console, settingsProvider);
      EnsureDbDataDirectoryExists(settingsProvider.DbFile);
      DbMigrator.Migrate(settingsProvider.ConnectionString);

      var rootCommand = new RootCommand("Console Time Tracker")
      {
         Name = "TT",
      };

      RegisterCommands(rootCommand);

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

      ServicesProvider.Register<IAnsiConsole, object>(AnsiConsole.Console);
      ServicesProvider.Register<ISettingsProvider, SettingsProvider>(settingsProvider);
      ServicesProvider.Register<IDbRepository, DbRepository>(dbRepository);
   }

   private static void RegisterCommands(RootCommand rootCommand)
   {
      rootCommand.Add(new ProjectCommand());
      rootCommand.Add(new TaskCommand());
      rootCommand.Add(new RecordCommand());
   }

   internal static void DisplayHeader(IAnsiConsole console, ISettingsProvider settingsProvider)
   {
      console.WriteLine();
      if (settingsProvider.DisplayLargeAppName)
      {
         console.Write(new Rule().RuleStyle("green"));
         console.Write(new FigletText("ConsoleTT").Color(Color.Green));
         ShowBasicInfo(console, settingsProvider);
      }
      else
      {
         var rule = new Rule($"[green]ConsoleTT    ({settingsProvider.DbFile.FullName})[/]")
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

   private static void ShowBasicInfo(IAnsiConsole console, ISettingsProvider settingsProvider)
   {
      console.WriteLine();
      console.MarkupLine($"Database file: [green]{settingsProvider.DbFile.FullName}[/]");
      console.WriteLine();
   }
}