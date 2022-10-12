namespace Cli.Entities
{
   public class TimeSheetRecord
   {
      public int Id { get; set; }
      public float TimeSpent { get; set; }
      public DateTime WorkedAt { get; set; }
      public string Description { get; set; } = string.Empty;
      public int RelTaskId { get; set; }
   }
}
