namespace App.Models;

public class SettingsDto
{
   public string? DbFilePath { get; set; }
   public bool? DisplayLargeAppName { get; set; }
   public bool? ClearConsoleAfterEveryCommand { get; set; }
}
