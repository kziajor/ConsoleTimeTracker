using App.Models.Dtos;

namespace App.Integrations;

internal sealed class InternalSystem : ISourceSystem
{
    public ExternalSystemTaskData GetTaskData(string externalSystemTaskId)
    {
        throw new NotImplementedException();
    }
}