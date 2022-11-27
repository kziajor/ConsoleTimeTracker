namespace App.Models.Inputs;

public sealed class ProjectInput : BaseInput
{
   public int Id { get; set; }
   public string? Name { get; set; }
   public bool? Closed { get; set; }
}