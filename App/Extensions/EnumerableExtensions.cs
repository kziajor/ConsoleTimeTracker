namespace App.Extensions;

public static class EnumerableExtensions
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

   public static IEnumerable<TElement> UniqueBy<TElement, TKey>(this IEnumerable<TElement> source, Func<TElement, TKey> keySelector)
      where TKey : IEquatable<TKey>
   {
      var result = new Dictionary<TKey, TElement>();

      foreach (var element in source)
      {
         try
         {
            var keyValue = keySelector.Invoke(element);
            if (keyValue is null || result.TryGetValue(keyValue, out _)) { continue; }

            result[keyValue] = element;
         }
         catch { continue; }
      }

      return result.Select(d => d.Value);
   }
}