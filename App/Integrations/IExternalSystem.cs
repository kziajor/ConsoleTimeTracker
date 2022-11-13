using App.Models.Dtos;

namespace App.Integrations;

public interface IExternalSystem
{
   public ExternalSystemTaskData GetTaskData(string externalSystemTaskId);
}
