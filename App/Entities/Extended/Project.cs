namespace App.Entities;

public partial class Project
{
   public decimal TimeSpentHours => Math.Round((decimal)PR_TimeSpent / 60, 2);
}