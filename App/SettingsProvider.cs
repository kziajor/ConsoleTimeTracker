using App.Models;
using Spectre.Console;
using System.Text.Json;

namespace App;

public interface ISettingsProvider
{
   string CurrentUserApplicationDataPath { get; }
   string CurrentUserDocumentsDirectory { get; }
   FileInfo SettingsFile { get; }
   FileInfo DbFile { get; }
   string ConnectionString { get; }
}

public class SettingsProvider : ISettingsProvider
{
   private readonly FileInfo _defaultDbFile;

   public string CurrentUserApplicationDataPath { get; }
   public string CurrentUserDocumentsDirectory { get; }
   public FileInfo SettingsFile { get; }


   #region AppSettings

   public FileInfo DbFile { get; }
   public string ConnectionString { get; private set; } = string.Empty;
   public bool DisplayLargeAppName { get; private set; }
   public bool ClearConsoleAfterEveryCommand { get; private set; }

   #endregion

   public SettingsProvider()
   {
      CurrentUserApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      CurrentUserDocumentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      SettingsFile = new FileInfo(Path.Join(CurrentUserApplicationDataPath, "./ConsoleTT/settings.json"));
      _defaultDbFile = new FileInfo(Path.Join(CurrentUserDocumentsDirectory, "./ConsoleTT/Data.db"));

      SettingsDto? settingsDto = SettingsFileReadOrCreate();
      DbFile = settingsDto?.DbFilePath is not null ? new FileInfo(settingsDto.DbFilePath) : _defaultDbFile;

      SetConfigValues(settingsDto);
   }

   private void SetConfigValues(SettingsDto? settingsDto)
   {
      ConnectionString = $"Data Source={DbFile}";
      DisplayLargeAppName = settingsDto?.DisplayLargeAppName ?? true;
      ClearConsoleAfterEveryCommand = settingsDto?.ClearConsoleAfterEveryCommand ?? false;
   }

   private SettingsDto SettingsFileReadOrCreate()
   {
      SettingsDto? settingsDto;
      if (!SettingsFile.Exists)
      {
         if (SettingsFile.Directory is null)
         {
            throw new ArgumentException($"Settings directory error for path: {SettingsFile.FullName}");
         }

         settingsDto = new SettingsDto
         {
            DbFilePath = _defaultDbFile.FullName
         };

         if (!SettingsFile.Directory!.Exists)
         {
            Directory.CreateDirectory(SettingsFile.Directory.FullName);
         }

         File.WriteAllText(SettingsFile.FullName, JsonSerializer.Serialize(settingsDto));
      }
      else
      {
         settingsDto = JsonSerializer.Deserialize<SettingsDto>(File.ReadAllText(SettingsFile.FullName));

         if (settingsDto is null)
         {
            AnsiConsole.WriteException(new Exception($"Settings file is corrupted: {SettingsFile}. Fix it or remove it."));
            Environment.Exit(1);
         }
      }

      return settingsDto;
   }
}
