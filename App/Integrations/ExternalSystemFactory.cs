namespace App.Integrations;

public static class ExternalSystemFactory
{
   public static IExternalSystem GetInstance(ExternalSystemEnum externalSystemType)
   {
      var settingsProvider = ServicesProvider.GetInstance<ISettingsProvider>();

      return externalSystemType switch
      {
         ExternalSystemEnum.Azure => new AzureSystem(),
         _ => GetInstance(settingsProvider.ExternalSystemDefaultType)
      };
   }
}