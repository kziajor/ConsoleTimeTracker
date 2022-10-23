﻿using App.Entities;
using App.Extensions;
using Spectre.Console;

namespace App.Commands.Projects
{
   public static class ProjectCommon
   {
      public static void DisplayProjectsList(IEnumerable<Project> projects)
      {
         var table = new Table();

         table.Border(TableBorder.Rounded);

         table.AddColumn(new TableColumn("[green]Id[/]").LeftAligned());
         table.AddColumn(new TableColumn("[green]Name[/]").LeftAligned());
         table.AddColumn(new TableColumn("[green]Active[/]").Centered());

         foreach (var project in projects)
         {
            table.AddRow(project.Id.ToString(), project.Name, project.Closed ? "" : "[green]X[/]");
         }

         AnsiConsole.MarkupLine("[green]Projects[/]");
         AnsiConsole.Write(table);
      }

      public static string AskForProjectName(bool allowEmpty = false, string customPrompt = "")
      {
         var promptMessage = customPrompt.IsNullOrEmpty() ? "Project name:" : customPrompt;
         var prompt = new TextPrompt<string>(promptMessage)
                     .PromptStyle("green");

         if (allowEmpty) { prompt.AllowEmpty(); }

         return AnsiConsole.Prompt(prompt);
      }

      public static bool AskIsProjectActive()
      {
         var choice = AnsiConsole.Prompt(
               new SelectionPrompt<string>()
                  .Title("Is project active")
                  .AddChoices(new[] { "Yes", "No" })
            );

         return choice == "Yes";
      }

      public static Project AskForProjectId(IEnumerable<Project> projects)
      {
         return AnsiConsole.Prompt(
               new SelectionPrompt<Project>()
                  .Title("Choose project")
                  .AddChoices(projects)
                  .UseConverter((project) =>
                  {
                     var activityLabel = project.Closed ? "not active" : "active";
                     return $"{project.Id}\t{project.Name} ({activityLabel})";
                  })
            );
      }
   }
}
