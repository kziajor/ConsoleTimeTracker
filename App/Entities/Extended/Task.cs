using App.Models.Dtos;

namespace App.Entities;

public partial class Task
{
   private UniversalTaskId? _universalTaskId;
   public UniversalTaskId UniversalTaskId
   {
      get
      {
         var taskId = TA_SourceType == Integrations.SourceSystemType.Internal ? TA_Id.ToString() : TA_SourceTaskId;
         return _universalTaskId ??= UniversalTaskId.Create(taskId, TA_SourceType);
      }
   }
   public decimal TimePlannedHours { get => Math.Round((decimal)TA_PlannedTime / 60, 2); set => TA_PlannedTime = (int)(value * 60); }
   public decimal TimeSpentHours => Math.Round((decimal)TA_SpentTime / 60, 2);
}
