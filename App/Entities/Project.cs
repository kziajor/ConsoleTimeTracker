using Dapper.Contrib.Extensions;

namespace App.Entities;

[Table("projects")]
public class Project
{
   [Key]
   public int Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public bool Closed { get; set; }
}
