namespace App.Entities;

public sealed class Record
{
   public int? RE_Id { get; set; }
   public DateTime RE_StartedAt { get; set; }
   public DateTime? RE_FinishedAt { get; set; }
   public int RE_MinutesSpent { get; set; } = 0;
   public string? RE_Comment { get; set; } = string.Empty;
   public int RE_RelTaskId { get; set; }
   public Task? Task { get; set; }

   public decimal TimeSpentHours => Math.Round((decimal)RE_MinutesSpent / 60, 2);
}