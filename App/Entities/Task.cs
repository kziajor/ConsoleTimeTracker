using App.Integrations;

namespace App.Entities;

public sealed class Task
{
   public int TA_Id { get; set; }
   public string TA_Title { get; set; } = string.Empty;
   public int TA_PlannedTime { get; set; }
   public bool TA_Closed { get; set; }
   public int TA_SpentTime { get; set; }
   public int TA_RelProjectId { get; set; }
   public ExternalSystemEnum? TA_ExternalSystemType { get; set; } // TODO: change to not null in database and set default value to 0
   public string? TA_ExternalSystemTaskId { get; set; }

   public Project? Project { get; set; }
   public decimal PlannedTimeInHours { get => Math.Round((decimal)TA_PlannedTime / 60, 2); set => TA_PlannedTime = (int)(value * 60); }
   public decimal SpentTimeInHours => Math.Round((decimal)TA_SpentTime / 60, 2);
   public string ExternalFullId => TA_ExternalSystemType is not null ? $"{TA_ExternalSystemType}-{TA_ExternalSystemTaskId}" : string.Empty;
}
