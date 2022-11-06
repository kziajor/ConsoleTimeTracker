using App.Extensions;
using App.Models.Inputs;
using System.CommandLine;
using System.CommandLine.Binding;

namespace App.Commands.Tasks;

public class TaskInputBinder : BinderBase<TaskInput>
{
   private readonly Argument<int>? _id;
   private readonly Option<string?>? _title;
   private readonly Option<bool?>? _closed;
   private readonly Option<int?>? _projectId;
   private readonly Option<int?>? _plannedTime;
   private readonly Option<bool>? _interactiveMode;

   public TaskInputBinder(Argument<int>? id = null, Option<string?>? title = null, Option<bool?>? closed = null, Option<int?>? projectId = null, Option<int?>? plannedTime = null, Option<bool>? interactiveMode = null)
   {
      _id = id;
      _title = title;
      _closed = closed;
      _projectId = projectId;
      _plannedTime = plannedTime;
      _interactiveMode = interactiveMode;
   }

   protected override TaskInput GetBoundValue(BindingContext bindingContext)
   {
      return new TaskInput
      {
         Id = bindingContext.ParseResult.GetValueForArgumentOrDefault(_id),
         Title = bindingContext.ParseResult.GetValueForOptionOrDefault(_title),
         Closed = bindingContext.ParseResult.GetValueForOptionOrDefault(_closed),
         ProjectId = bindingContext.ParseResult.GetValueForOptionOrDefault(_projectId),
         PlannedTime = bindingContext.ParseResult.GetValueForOptionOrDefault(_plannedTime),
         InteractiveMode = bindingContext.ParseResult.GetValueForOptionOrDefault(_interactiveMode),
      };
   }
}