using App.Migrations;
using Spectre.Console;
using System.CommandLine;

namespace App;

static class Program
{
   static async Task<int> Main(string[] args)
   {
      ShowTitle();
      const string defaultDbPath = "./Data.db";
      // TODO: Save and read settings from User directory
      // TODO: Get path for database from settings or default - user can set this path
      DbMigrator.Migrate($"Data Source={defaultDbPath}");

      var rootCommand = new RootCommand("Console Time Tracker")
      {
         Name = "TT",
      };

      return await rootCommand.InvokeAsync(args);
   }

   private static void ShowTitle()
   {
      AnsiConsole.Write(new FigletText("TT").Color(Color.Green));
   }
}