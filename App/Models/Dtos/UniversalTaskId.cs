using App.Extensions;
using App.Integrations;

namespace App.Models.Dtos;

public class UniversalTaskId
{
   public const string INTERNAL_TASK_ID_PREFIX = "internal";

   public ExternalSystemEnum? ExternalSystemType { get; private set; }
   public string? ExternalSystemTaskId { get; private set; }
   public int? TaskId { get; private set; }
   public bool IsInternal => TaskId is not null && ExternalSystemType is null;
   public bool IsEmpty => ExternalSystemType is null && TaskId is null;

   public static UniversalTaskId? Create(ExternalSystemEnum? systemType, string? taskId)
   {
      if (systemType is null) { return null; }

      return new UniversalTaskId
      {
         ExternalSystemType = systemType,
         ExternalSystemTaskId = taskId,
         TaskId = null,
      };
   }
   public static UniversalTaskId? Create(string? taskId)
   {
      if (string.IsNullOrEmpty(taskId)) { return null; }

      var settingsProvider = ServicesProvider.GetInstance<ISettingsProvider>();
      var splitResult = taskId.Split('-', 2);

      if (splitResult.Length == 2 && Enum.TryParse(typeof(ExternalSystemEnum), splitResult[0], true, out object? externalSystemType) && externalSystemType is not null)
      {
         return Create((ExternalSystemEnum)externalSystemType, splitResult[1]);
      }

      if (splitResult.Length == 2 && splitResult[0].ToLower() == INTERNAL_TASK_ID_PREFIX && splitResult[1].IsInt())
      {
         return new UniversalTaskId
         {
            ExternalSystemTaskId = null,
            ExternalSystemType = null,
            TaskId = splitResult[1].ToInt(),
         };
      }

      return new UniversalTaskId
      {
         ExternalSystemType = settingsProvider.ExternalSystemDefaultType,
         ExternalSystemTaskId = taskId,
         TaskId = taskId.ToInt(),
      };
   }

   public override string ToString()
   {
      if (IsInternal) { return TaskId?.ToString() ?? string.Empty; }
      return $"{ExternalSystemType}-{ExternalSystemTaskId}";
   }
}
