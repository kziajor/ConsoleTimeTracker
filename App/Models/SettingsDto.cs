namespace App.Models;

public sealed class SettingsDto
{
   public string? DbFilePath { get; set; }
   public bool? DisplayLargeAppName { get; set; }
   public bool? ClearConsoleAfterEveryCommand { get; set; }
   public bool? ExternalSystemPriority { get; set; }
   public string? ExternalSystemDefaultType { get; set; }
}
