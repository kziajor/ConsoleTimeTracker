using Spectre.Console;

namespace App.Commands;

public static class CommandCommon
{
   public static T AskFor<T>(string promptText)
   {
      var prompt = new TextPrompt<T>(promptText).PromptStyle("green");

      return AnsiConsole.Prompt(prompt);
   }

   public static T? AskForWithEmptyAllowed<T>(string promptText, T? defaultValue = default)
   {
      var prompt = new TextPrompt<T?>(promptText)
                  .PromptStyle("green")
                  .AllowEmpty()
                  .DefaultValue(defaultValue)
                  .ShowDefaultValue()
                  .DefaultValueStyle(new Style(decoration: Decoration.Bold))
                  .WithConverter(choice =>
                  {
                     return choice switch
                     {
                        DateTime c => c.ToIsoString(),
                        _ => choice?.ToString() ?? string.Empty,
                     };
                  });

      return AnsiConsole.Prompt(prompt);
   }

   public static bool AskForYesNo(string promptText)
   {
      var choice = AnsiConsole.Prompt(
               new SelectionPrompt<string>()
                  .Title(promptText)
                  .AddChoices(new[] { "Yes", "No" })
            );

      return choice == "Yes";
   }

   public static T ChooseOne<T>(this IEnumerable<T> choices, string promptText, int? pageSize = null, Func<T, string>? optionNameConverter = null) where T : class
   {
      var selectionPrompt = new SelectionPrompt<T>()
         .Title(promptText)
         .AddChoices(choices);

      if (optionNameConverter is not null)
      {
         selectionPrompt.UseConverter(optionNameConverter);
      }

      if (pageSize is not null)
      {
         selectionPrompt.PageSize(pageSize.Value);
      }

      return AnsiConsole.Prompt(selectionPrompt);
   }
}
