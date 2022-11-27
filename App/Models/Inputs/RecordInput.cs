using App.Models.Dtos;

namespace App.Models.Inputs;

public sealed class RecordInput : BaseInput
{
   public UniversalTaskId? UniversalTaskId { get; private set; }
   public int Id { get; set; }
   public DateTime? StartedAt { get; set; }
   public DateTime? FinishedAt { get; set; }
   public string? Comment { get; set; }
   public string? TaskId
   {
      get => UniversalTaskId?.ToString() ?? string.Empty;
      set => UniversalTaskId = UniversalTaskId.Create(value);
   }

   public bool ClearComment { get; set; }
   public bool ClearFinishedAt { get; set; }
}
