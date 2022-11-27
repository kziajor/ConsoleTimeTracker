namespace App.Extensions;

public static class ListExtensions
{
   public static List<T> MoveToTop<T>(this List<T> list, Func<T, bool> predicate)
   {
      var elementToMove = list.FirstOrDefault(predicate);

      if (elementToMove is null) { return list; }
      if (list.Remove(elementToMove)) { list.Insert(0, elementToMove); }

      return list;
   }

   public static List<T> MoveToBottom<T>(this List<T> list, Func<T, bool> predicate)
   {
      var elementToMove = list.FirstOrDefault(predicate);

      if (elementToMove is null) { return list; }
      if (list.Remove(elementToMove)) { list.Add(elementToMove); }

      return list;
   }
}