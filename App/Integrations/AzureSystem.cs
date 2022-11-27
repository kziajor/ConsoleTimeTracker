using App.Models.Dtos;

namespace App.Integrations;

public sealed class AzureSystem : ISourceSystem
{
   public ExternalSystemTaskData GetTaskData(string externalSystemTaskId)
   {
      // TODO: Implement integration with Azure DevOps - Getting work items data (tasks, cases)
      throw new NotImplementedException();
   }
}