namespace App.Entities;

public sealed class Task
{
   public int TA_Id { get; set; }
   public string TA_Title { get; set; } = string.Empty;
   public int TA_PlannedTime { get; set; }
   public bool TA_Closed { get; set; } = false;
   public int TA_SpentTime { get; set; }
   public int TA_RelProjectId { get; set; }
   public int? TA_ExternalSystemType { get; set; }
   public string? TA_ExternalSystemTaskId { get; set; }

   public Project? Project { get; set; }
}
