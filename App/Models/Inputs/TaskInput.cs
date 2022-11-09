using App.Integrations;

namespace App.Models.Inputs;

public sealed class TaskInput : BaseInput
{
   public int Id { get; set; }
   public string? Title { get; set; }
   public bool? Closed { get; set; }
   public int? ProjectId { get; set; }
   public decimal? PlannedTime { get; set; }
   public ExternalSystemEnum? ExternalSystemType { get; set; }
   public string? ExternalSystemTaskId { get; set; }
}