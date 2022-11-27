using App.Extensions;
using App.Integrations;
using App.Models.Dtos;

namespace App.Models.Inputs;

public sealed class TaskInput : BaseInput
{
   public UniversalTaskId UniversalTaskId { get; private set; } = new UniversalTaskId();
   public string? Id
   {
      get => UniversalTaskId.ToString();
      set => UniversalTaskId = UniversalTaskId.Create(value);
   }
   public string? Title { get; set; }
   public bool? Closed { get; set; }
   public int? ProjectId { get; set; }
   public decimal? PlannedTime { get; set; }
   public SourceSystemType? SourceType { get; set; }
   public string? SourceTaskId { get; set; }

   public int? InternalId => Id?.ToInt();
}