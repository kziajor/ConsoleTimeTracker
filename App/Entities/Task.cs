using Dapper.Contrib.Extensions;

namespace App.Entities;

[Table("tasks")]
public class Task
{
   [Key]
   public int id { get; set; }
   public string title { get; set; } = string.Empty;
   public int planned_time { get; set; }
   public bool closed { get; set; } = false;
   public int rel_project_id { get; set; }

   [Write(false)]
   [Computed]
   public Project? Project { get; set; }

   [Write(false)]
   [Computed]
   public int SpentTime { get; set; }
}
