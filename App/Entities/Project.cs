using Dapper.Contrib.Extensions;

namespace App.Entities;

[Table("projects")]
public class Project
{
   [Key]
   public int id { get; set; }
   public string name { get; set; } = string.Empty;
   public bool closed { get; set; }
}
