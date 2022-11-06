﻿namespace App.Models.Inputs;

public sealed class TaskInput : BaseInput
{
   public int Id { get; set; }
   public string? Title { get; set; }
   public bool? Closed { get; set; }
   public int? ProjectId { get; set; }
   public int? PlannedTime { get; set; }
}