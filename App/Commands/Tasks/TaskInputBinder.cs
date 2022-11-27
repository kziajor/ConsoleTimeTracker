using App.Extensions;
using App.Integrations;
using App.Models.Inputs;

using System.CommandLine;
using System.CommandLine.Binding;

namespace App.Commands.Tasks;

public class TaskInputBinder : BinderBase<TaskInput>
{
   private readonly Argument<string?>? _id;
   private readonly Option<string?>? _title;
   private readonly Option<bool?>? _closed;
   private readonly Option<int?>? _projectId;
   private readonly Option<decimal?>? _plannedTime;
   private readonly Option<SourceSystemType?>? _sourceType;
   private readonly Option<string?>? _sourceTaskId;
   private readonly Option<bool>? _manualMode;

   public TaskInputBinder(Argument<string?>? id = null, Option<string?>? title = null, Option<bool?>? closed = null, Option<int?>? projectId = null, Option<decimal?>? plannedTime = null, Option<bool>? manualMode = null, Option<SourceSystemType?>? sourceType = null, Option<string?>? sourceTaskId = null)
   {
      _id = id;
      _title = title;
      _closed = closed;
      _projectId = projectId;
      _plannedTime = plannedTime;
      _manualMode = manualMode;
      _sourceType = sourceType;
      _sourceTaskId = sourceTaskId;
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
         SourceType = bindingContext.ParseResult.GetValueForOptionOrDefault(_sourceType),
         SourceTaskId = bindingContext.ParseResult.GetValueForOptionOrDefault(_sourceTaskId),
         ManualMode = bindingContext.ParseResult.GetValueForOptionOrDefault(_manualMode),
      };
   }
}