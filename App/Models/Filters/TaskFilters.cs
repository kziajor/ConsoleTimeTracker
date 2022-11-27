namespace App.Models.Filters;

public sealed class TaskFilters
{
   public uint Limit { get; set; } = 100;
   public uint Skip { get; set; } = 0;
   public int? TaskId { get; set; } = null;
   public int? ProjectId { get; set; } = null;
   public string? Title { get; set; } = null;
   public bool? Closed { get; set; } = null;
   public string? SourceSystemType { get; set; } = null;
   public string? SourceSystemTaskId { get; set; } = null;
}
