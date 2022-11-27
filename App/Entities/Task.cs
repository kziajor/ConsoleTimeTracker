using App.Integrations;

namespace App.Entities;

public sealed partial class Task
{
   internal static string TableName => "Tasks";

   public int TA_Id { get; set; }
   public string TA_Title { get; set; } = string.Empty;
   public int TA_PlannedTime { get; set; }
   public bool TA_Closed { get; set; }
   public int TA_SpentTime { get; set; }
   public int TA_RelProjectId { get; set; }
   public SourceSystemType TA_SourceType { get; set; }
   public string TA_SourceTaskId { get; set; } = string.Empty;

   public Project? TA_Project { get; set; }
}
