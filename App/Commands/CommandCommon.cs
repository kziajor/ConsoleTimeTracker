using App.Assets;
using App.Extensions;
using Spectre.Console;

namespace App.Commands;

public static class CommandCommon
{
   public static T AskFor<T>(string promptText)
   {
      var prompt = new TextPrompt<T>(promptText).PromptStyle("green");

      return AnsiConsole.Prompt(prompt);
   }

   public static T AskForWithEmptyAllowed<T>(string promptText, T defaultValue)
   {
      var prompt = new TextPrompt<T>(promptText)
                  .PromptStyle("green")
                  .AllowEmpty()
                  .DefaultValue(defaultValue)
                  .ShowDefaultValue()
                  .DefaultValueStyle(new Style(decoration: Decoration.Bold))
                  .WithConverter(choice =>
                  {
                     return choice switch
                     {
                        DateTime c => c.ToIsoDateTime(),
                        _ => choice?.ToString() ?? string.Empty,
                     };
                  });

      return AnsiConsole.Prompt(prompt);
   }

   public static T ChooseOne<T>(this IEnumerable<T> choices, string promptText, int pageSize = 20, Func<T, bool>? predicateDefaultOption = null, Func<T, string>? optionNameConverter = null) where T : notnull
   {
      var selectionPrompt = new SelectionPrompt<T>()
         .Title(promptText)
         .AddChoices(choices)
         .PageSize(pageSize)
      .UseConverter(o =>
      {
         string optionLabel = optionNameConverter?.Invoke(o) ?? o.ToString() ?? string.Empty;
         string result = predicateDefaultOption?.Invoke(o) ?? false
            ? $"[{Colors.Primary.ToMarkup()} underline]{optionLabel}[/]"
            : optionLabel;

         return result;
      });

      return AnsiConsole.Prompt(selectionPrompt);
   }
}
