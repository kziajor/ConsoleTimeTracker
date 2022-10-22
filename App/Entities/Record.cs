namespace Cli.Entities;

public class Record
{
   public int? Id { get; set; }
   public DateTime StartedAt { get; set; }
   public DateTime? StopedAt { get; set; }
   public int MinutesSpent { get; set; } = 0;
   public string Comment { get; set; } = string.Empty;
   public int RelTaskId { get; set; }
}