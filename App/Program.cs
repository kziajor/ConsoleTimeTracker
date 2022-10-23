using App.Commands.Projects;
using App.Migrations;
using App.Repositories;
using Spectre.Console;
using System.CommandLine;

namespace App;

static class Program
{
   static async Task<int> Main(string[] args)
   {
      var settingsProvider = new SettingsProvider();

      if (settingsProvider.DisplayTitle)
      {
         AnsiConsole.Write(new FigletText("ConsoleTT").Color(Color.Green));
      }

      EnsureDbDataDirectoryExists(settingsProvider.DbFile);

      DbMigrator.Migrate(settingsProvider.ConnectionString);

      ShowBasicInfo(settingsProvider);

      var rootCommand = new RootCommand("Console Time Tracker")
      {
         Name = "TT",
      };

      var dbRepository = (IDbRepository)new DbRepository(settingsProvider.ConnectionString);

      rootCommand.Add(new ProjectCommand(dbRepository));

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