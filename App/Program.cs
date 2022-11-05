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
      var settingsProvider = new SettingsProvider();

      if (settingsProvider.ClearConsoleAfterEveryCommand) { AnsiConsole.Clear(); }

      DisplayTitle(settingsProvider);

      EnsureDbDataDirectoryExists(settingsProvider.DbFile);

      DbMigrator.Migrate(settingsProvider.ConnectionString);

      ShowBasicInfo(settingsProvider);
      AnsiConsole.WriteLine();

      var rootCommand = new RootCommand("Console Time Tracker")
      {
         Name = "TT",
      };

      var dbRepository = (IDbRepository)new DbRepository(settingsProvider.ConnectionString);

      rootCommand.Add(new ProjectCommand(dbRepository));
      rootCommand.Add(new TaskCommand(dbRepository));

      var result = await rootCommand.InvokeAsync(args);

      AnsiConsole.WriteLine();
      AnsiConsole.Write(new Rule().RuleStyle("green"));

      return result;
   }

   private static void DisplayTitle(SettingsProvider settingsProvider)
   {
      AnsiConsole.WriteLine();
      if (settingsProvider.DisplayLargeAppName)
      {
         AnsiConsole.Write(new Rule().RuleStyle("green"));
         AnsiConsole.Write(new FigletText("ConsoleTT").Color(Color.Green));
      }
      else
      {
         var rule = new Rule("[green]ConsoleTT[/]")
            .LeftAligned()
            .RuleStyle("green");

         AnsiConsole.Write(rule);
         AnsiConsole.WriteLine();
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

   private static void ShowBasicInfo(SettingsProvider settingsProvider)
   {
      AnsiConsole.WriteLine();
      AnsiConsole.MarkupLine($"Database file: [green]{settingsProvider.DbFile.FullName}[/]");
      AnsiConsole.WriteLine();
   }
}