namespace App.Integrations;

public static class SourceSystemFactory
{
   private static readonly ISettingsProvider _settingsProvider = ServicesProvider.GetInstance<ISettingsProvider>();

   public static ISourceSystem GetInstance(SourceSystemType externalSystemType)
   {
      return externalSystemType switch
      {
         SourceSystemType.Internal => new InternalSystem(),
         SourceSystemType.Azure => new AzureSystem(),
         _ => GetInstance(_settingsProvider.SourceSystemDefaultType)
      };
   }
}