using App.Integrations;

namespace App.Models.Dtos;

public sealed class ExternalSystemTaskData
{
   public string Id { get; set; } = string.Empty;
   public string Title { get; set; } = string.Empty;
   public string Description { get; set; } = string.Empty;
   public SourceSystemStatus Status { get; set; }
}
