using App.Extensions;
using App.Integrations;
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
   private readonly Option<decimal?>? _plannedTime;
   private readonly Option<ExternalSystemEnum?>? _externalSystemType;
   private readonly Option<string?>? _externalSystemTaskId;
   private readonly Option<bool>? _interactiveMode;

   public TaskInputBinder(Argument<int>? id = null, Option<string?>? title = null, Option<bool?>? closed = null, Option<int?>? projectId = null, Option<decimal?>? plannedTime = null, Option<bool>? interactiveMode = null, Option<ExternalSystemEnum?>? externalSystemType = null, Option<string?>? externalSystemTaskId = null)
   {
      _id = id;
      _title = title;
      _closed = closed;
      _projectId = projectId;
      _plannedTime = plannedTime;
      _interactiveMode = interactiveMode;
      _externalSystemType = externalSystemType;
      _externalSystemTaskId = externalSystemTaskId;
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
         ExternalSystemType = bindingContext.ParseResult.GetValueForOptionOrDefault(_externalSystemType),
         ExternalSystemTaskId = bindingContext.ParseResult.GetValueForOptionOrDefault(_externalSystemTaskId),
         InteractiveMode = bindingContext.ParseResult.GetValueForOptionOrDefault(_interactiveMode),
      };
   }
}