namespace App.Models.Inputs;

public sealed class RecordInput : BaseInput
{
   public int Id { get; set; }
   public DateTime? StartedAt { get; set; }
   public DateTime? FinishedAt { get; set; }
   public string? Comment { get; set; }
   public int? RelTaskId { get; set; }

   public bool ClearComment { get; set; }
   public bool ClearFinishedAt { get; set; }
}
