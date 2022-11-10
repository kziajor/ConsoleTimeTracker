using App.Extensions;
using App.Models.Inputs;
using System.CommandLine;
using System.CommandLine.Binding;

namespace App.Commands.Records;

public sealed class RecordInputBinder : BinderBase<RecordInput>
{
   private readonly Argument<int>? _recordIdArgument;
   private readonly Argument<string?>? _taskIdArgument;
   private readonly Option<string?>? _taskIdOption;
   private readonly Option<DateTime?>? _startedAt;
   private readonly Option<DateTime?>? _finishedAt;
   private readonly Option<string?>? _comment;
   private readonly Option<bool>? _clearComment;
   private readonly Option<bool>? _clearFinishedAt;
   private readonly Option<bool>? _interactiveMode;

   public RecordInputBinder(Argument<int>? recordId = null, Argument<string?>? taskIdArgument = null, Option<string?>? taskIdOption = null, Option<DateTime?>? startedAt = null, Option<DateTime?>? finishedAt = null, Option<string?>? comment = null, Option<bool>? clearComment = null, Option<bool>? clearFinishedAt = null, Option<bool>? interactiveMode = null)
   {
      _recordIdArgument = recordId;
      _taskIdArgument = taskIdArgument;
      _taskIdOption = taskIdOption;
      _startedAt = startedAt;
      _finishedAt = finishedAt;
      _comment = comment;
      _clearComment = clearComment;
      _clearFinishedAt = clearFinishedAt;
      _interactiveMode = interactiveMode;
   }

   protected override RecordInput GetBoundValue(BindingContext bindingContext)
   {
      return new RecordInput
      {
         Id = bindingContext.ParseResult.GetValueForArgumentOrDefault(_recordIdArgument),
         TaskId = bindingContext.ParseResult.GetValueForArgumentOrDefault(_taskIdArgument) ?? bindingContext.ParseResult.GetValueForOptionOrDefault(_taskIdOption),
         StartedAt = bindingContext.ParseResult.GetValueForOptionOrDefault(_startedAt),
         FinishedAt = bindingContext.ParseResult.GetValueForOptionOrDefault(_finishedAt),
         Comment = bindingContext.ParseResult.GetValueForOptionOrDefault(_comment),
         ClearComment = bindingContext.ParseResult.GetValueForOptionOrDefault(_clearComment),
         ClearFinishedAt = bindingContext.ParseResult.GetValueForOptionOrDefault(_clearFinishedAt),
         InteractiveMode = bindingContext.ParseResult.GetValueForOptionOrDefault(_interactiveMode)
      };
   }
}