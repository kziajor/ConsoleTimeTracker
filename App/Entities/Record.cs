namespace App.Entities;

public sealed class Record
{ // TODO: Create class that will inherite from Record class, will contain calculated fields: TimeSpentHours and will protect MinutesSpent value. Entities should be exacte mirror of tables in db
   public int? RE_Id { get; set; }
   public DateTime RE_StartedAt { get; set; }
   public DateTime? RE_FinishedAt { get; set; }
   public int RE_MinutesSpent { get; set; }
   public string? RE_Comment { get; set; }
   public int RE_RelTaskId { get; set; }
   public Task? Task { get; set; }

   public decimal TimeSpentHours => Math.Round((decimal)RE_MinutesSpent / 60, 2);
}