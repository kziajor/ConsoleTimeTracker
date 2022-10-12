namespace Cli.Entities;

public class Task
{
   public int Id { get; set; }
   public int AzureDevOpsId { get; set; }
   public string Title { get; set; } = string.Empty;
   public float TimePlanned { get; set; }
   public int RelProjectId { get; set; }

}
