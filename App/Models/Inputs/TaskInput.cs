using App.Extensions;
using App.Integrations;
using App.Models.Dtos;

namespace App.Models.Inputs;

public sealed class TaskInput : BaseInput
{
   public UniversalTaskId? UniversalTaskId { get; private set; }
   public string? Id
   {
      get => UniversalTaskId?.ToString() ?? string.Empty;
      set => UniversalTaskId = UniversalTaskId.Create(value);
   }
   public string? Title { get; set; }
   public bool? Closed { get; set; }
   public int? ProjectId { get; set; }
   public decimal? PlannedTime { get; set; }
   public ExternalSystemEnum? ExternalSystemType { get; set; }
   public string? ExternalSystemTaskId { get; set; }

   public int? InternalId => Id?.ToInt();
   public string? ExternalFullId => ExternalSystemType is not null ? $"{ExternalSystemType.Value}-{ExternalSystemTaskId}" : null;
}