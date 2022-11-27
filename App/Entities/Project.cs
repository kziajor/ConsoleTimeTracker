﻿namespace App.Entities;

public sealed class Project
{
   internal static string TableName => "Projects";

   public int PR_Id { get; set; }
   public string PR_Name { get; set; } = string.Empty;
   public bool PR_Closed { get; set; }
}