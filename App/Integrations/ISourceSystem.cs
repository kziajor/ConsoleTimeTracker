using App.Models.Dtos;

namespace App.Integrations;

// TODO: Should contain all methods to manage tasks. Task repository should be used by InternalSystem implementation.

public interface ISourceSystem
{
   public ExternalSystemTaskData GetTaskData(string externalSystemTaskId);
}
