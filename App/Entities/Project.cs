namespace Cli.Entities;

public class Project
{
   public int? Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public bool Closed { get; set; } = false;
}
