using App.Integrations;
using Task = App.Entities.Task;

namespace App.Models.Dtos;

public sealed class UniversalTaskId
{
   private const char FULL_TASK_ID_SEPARATOR = '.';

   private static readonly ISettingsProvider _settingsProvider = ServicesProvider.GetInstance<ISettingsProvider>();

   private string _externalTaskId = string.Empty;
   private int _internalTaskId = 0;

   public SourceSystemType SourceSystemType { get; private set; }
   public string SourceSystemTaskId
   {
      get { return IsInternal ? InternalTaskId.ToString() : _externalTaskId; }
      set
      {
         if (IsInternal)
         {
            _externalTaskId = string.Empty;
            _ = int.TryParse(value, out _internalTaskId);
         }
         else
         {
            _externalTaskId = value;
            _internalTaskId = 0;
         }
      }
   }
   public int InternalTaskId => _internalTaskId;
   public bool IsInternal => SourceSystemType == Integrations.SourceSystemType.Internal;
   public bool IsEmpty => string.IsNullOrEmpty(_externalTaskId) && _internalTaskId <= 0;
   public string FullId => $"{SourceSystemType}{FULL_TASK_ID_SEPARATOR}{SourceSystemTaskId}";

   public static UniversalTaskId Create(string sourceTaskId, SourceSystemType? sourceType)
   {
      if (string.IsNullOrEmpty(sourceTaskId)) { return new UniversalTaskId(); }

      return new UniversalTaskId()
      {
         SourceSystemType = sourceType ?? _settingsProvider.SourceSystemDefaultType,
         SourceSystemTaskId = sourceTaskId,
      };
   }

   public static UniversalTaskId Create(string? sourceTaskId)
   {
      if (string.IsNullOrEmpty(sourceTaskId)) { return new UniversalTaskId(); }

      var splitResult = sourceTaskId.Split(FULL_TASK_ID_SEPARATOR, 2);

      if (splitResult.Length < 2)
      {
         return Create(sourceTaskId, null);
      }

      if (!Enum.TryParse(splitResult[0], true, out SourceSystemType sourceSystemType)) { return new UniversalTaskId(); }

      return Create(
         splitResult[1]
,
         sourceSystemType);
   }

   public static UniversalTaskId Create(Task? task)
   {
      return task is not null ?
         Create(task.TA_SourceTaskId, task.TA_SourceType)
         : new UniversalTaskId();
   }

   public override string ToString()
   {
      if (IsEmpty) { return string.Empty; }
      if (SourceSystemType == _settingsProvider.SourceSystemDefaultType) { return SourceSystemTaskId; }
      return FullId;
   }
}
