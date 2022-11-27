namespace App.Models;

public sealed class SettingsDto
{
   public string? DbFilePath { get; set; }
   public bool? DisplayLargeAppName { get; set; }
   public bool? ClearConsoleAfterEveryCommand { get; set; }
   public string? SourceSystemDefaultType { get; set; }
}
