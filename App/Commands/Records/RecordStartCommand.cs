using App.Assets;
using App.Commands.Records.Common;
using App.Commands.Tasks.Common;
using App.Entities;
using App.Extensions;
using App.Models.Dtos;
using App.Repositories;

using Spectre.Console;

using System.CommandLine;

namespace App.Commands.Records;

public sealed class RecordStartCommand : Command
{
   private readonly IDbRepository _dbRepository = ServicesProvider.GetInstance<IDbRepository>();
   private readonly IAnsiConsole _console = ServicesProvider.GetInstance<IAnsiConsole>();
   private readonly ISettingsProvider _settingsProvider = ServicesProvider.GetInstance<ISettingsProvider>();

   public RecordStartCommand() : base("start", "Start recording time in live")
   {
      var taskIdArgument = TaskArguments.GetIdArgument();
      var manualMode = CommonOptions.GetManualModeOption();

      Add(taskIdArgument);
      Add(manualMode);

      this.SetHandler((taskIdArgument, manualMode) => RecordStartHandler(taskIdArgument, manualMode), taskIdArgument, manualMode);
   }

   internal void RecordStartHandler(string? taskIdArgument, bool manualMode)
   {
      var recordsInProgress = _dbRepository.Records.GetInProgress().ToList();
      var universalTaskId = UniversalTaskId.Create(taskIdArgument);
      Record? recordToStart;

      // FIX: App returns error when user provide id off task that is currently in progress
      if (recordsInProgress.Count > 0 && universalTaskId is not null)
      {
         RecordCommon.DisplayList(recordsInProgress, "Records in progres");
         _console.WriteError("There could be only one record in progres. Finish record that is already in progress.");
         return;
      }

      if (recordsInProgress.Count > 0 && universalTaskId is null)
      {
         recordToStart = recordsInProgress[0];
      }
      else
      {
         var task = _dbRepository.Tasks.Get(universalTaskId);

         if (!manualMode && task is null)
         {
            task ??= _dbRepository.Tasks
                  .GetActive()
                  .OrderBy(t => t.TA_Project?.PR_Name)
                  .OrderBy(t => t.TA_Title)
                  .ChooseOne("Choose task", 20, optionNameConverter: (t) => t.GetOptionLabel());
         }

         if (task is null)
         {
            _console.WriteError("Task not found");
            return;
         }

         recordToStart = new Record
         {
            RE_StartedAt = DateTime.Now,
            RE_RelTaskId = task.TA_Id,
         };

         recordToStart.RE_Id = _dbRepository.Records.Insert(recordToStart)?.RE_Id;
         recordToStart.Task = task;
      }

      if (recordToStart.RE_Id <= 0) { return; }

      recordToStart.Task = _dbRepository.Tasks.Get(recordToStart.RE_RelTaskId);

      DisplayTimer(recordToStart);
   }

   private void DisplayTimer(Record record)
   {
      var continueCounting = true;
      var finishRecordAfterStop = false;
      int lastMinutesSpent = -1;
      var plannedTime = record.Task?.TA_PlannedTime ?? 0;
      var spentTime = record.Task?.TA_SpentTime ?? 0;
      var topGrid = new Grid().AddColumn().AddColumn()
         .AddKeyValueRow("Task id", record.Task?.UniversalTaskId.ToString() ?? string.Empty)
         .AddKeyValueRow("Task title", record.Task?.TA_Title)
         .AddKeyValueRow("Project name", record.Task?.TA_Project?.PR_Name)
         .AddKeyValueRow("Started at", record.RE_StartedAt.ToIsoString());

      while (continueCounting)
      {
         if (Console.KeyAvailable)
         {
            var key = Console.ReadKey(true);

            continueCounting = !(key.KeyChar == 'x' || key.KeyChar == 'f');
            finishRecordAfterStop = key.KeyChar == 'f';
         }

         var currentTime = DateTime.Now;
         var currentMinutesSpent = record.CalculateMinutesSpent(currentTime);

         if (lastMinutesSpent != currentMinutesSpent)
         {
            var totalSpentTime = spentTime + currentMinutesSpent;
            var leftTime = plannedTime - totalSpentTime;
            var overtime = leftTime < 0 ? leftTime * (-1) : 0;
            var counterText = new FigletText($"{totalSpentTime.MinutesToHours():0.00} h")
               .Centered()
               .Color(overtime == 0 ? Color.Green : Color.Red);

            _console.Clear();

            Program.DisplayHeader(_console, _settingsProvider);
            _console.Write(topGrid);
            _console.Write(new Rule().RuleStyle("green"));
            _console.WriteLine();
            _console.Write(counterText);
            _console.WriteLine();
            _console.Write(new Rule().RuleStyle("green"));

            var bottomGrid = new Grid().AddColumn().AddColumn()
               .AddKeyValueRow("Planned time", plannedTime.MinutesToHours().ToString("0.00"))
               .AddKeyValueRow("Current record time spent", currentMinutesSpent.MinutesToHours().ToString("0.00"));

            if (plannedTime > 0)
            {
               if (overtime > 0) { bottomGrid.AddKeyValueRow("Overtime", overtime.MinutesToHours().ToString("0.00"), Colors.Error); }
               else { bottomGrid.AddKeyValueRow("Time left", leftTime.MinutesToHours().ToString("0.00"), Colors.Primary); }
            }

            _console.Write(bottomGrid);
            _console.WriteLine();
            _console.MarkupLine("Press 'x' to exit without finishing record counting");
            _console.MarkupLine("Press 'f' to finish counting with current time and exit");

            lastMinutesSpent = currentMinutesSpent;
         }

         Thread.Sleep(100);
      }

      _console.WriteLine();

      if (finishRecordAfterStop)
      {
         record.RE_FinishedAt = DateTime.Now;
         record.RE_MinutesSpent = record.CalculateMinutesSpent();
         _dbRepository.Records.Update(record);

         _console.Write(new Rule("Record details").RuleStyle("white").Centered());
         _console.WriteLine();
         RecordCommon.DisplayDetails(record);

         _console.WriteLine();
         _console.Write(new Rule("Task details").RuleStyle("white").Centered());
         _console.WriteLine();
         TaskCommon.ShowTaskDetails(_dbRepository.Tasks.Get(record.RE_RelTaskId));
      }
      else
      {
         RecordCommon.DisplayList(_dbRepository.Records.GetAll());
      }
   }
}