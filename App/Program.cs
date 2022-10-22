using App.Migrations;
using Spectre.Console;
using System.CommandLine;

namespace App;

static class Program
{
   static async Task<int> Main(string[] args)
   {
      AnsiConsole.Write(new FigletText("ConsoleTT").Color(Color.Green));

      var settingsProvider = new SettingsProvider();

      EnsureDbDataDirectoryExists(settingsProvider.DbFile);

      DbMigrator.Migrate(settingsProvider.ConnectionString);

      ShowBasicInfo(settingsProvider);

      var rootCommand = new RootCommand("Console Time Tracker")
      {
         Name = "TT",
      };

      return await rootCommand.InvokeAsync(args);
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

   private static void ShowBasicInfo(SettingsProvider settingsProvider)
   {
      AnsiConsole.MarkupLine($"Database file: [green]{settingsProvider.DbFile.FullName}[/]");
   }
}