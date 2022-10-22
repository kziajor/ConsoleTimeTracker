namespace Cli.Entities;

public class Task
{
   public int Id { get; set; }
   public string Title { get; set; } = string.Empty;
   public float PlannedTime { get; set; }
   public bool Closed { get; set; }
   public int RelProjectId { get; set; }
}
