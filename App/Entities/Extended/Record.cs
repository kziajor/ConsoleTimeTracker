namespace App.Entities;

public partial class Record
{
   public decimal TimeSpentHours => Math.Round((decimal)RE_MinutesSpent / 60, 2);
}
