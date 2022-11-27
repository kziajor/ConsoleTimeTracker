namespace App;

public static class ServicesProvider
{
   private static readonly Dictionary<Type, object> _services = new();

   public static T GetInstance<T>() where T : class
   {
      var type = typeof(T);

      if (_services.ContainsKey(type))
      {
         return (T)_services[type];
      }

      throw new Exception($"Service for type '{typeof(T)}' not registered");
   }

   public static void Register<T1, T2>(T2? existingInstance = null)
      where T1 : class
      where T2 : class, new()
   {
      var interfaceType = typeof(T1);

      if (!interfaceType.IsInterface)
      {
         throw new Exception($"{interfaceType} is not interface");
      }

      if (_services.ContainsKey(interfaceType))
      {
         _services[interfaceType] = existingInstance ?? new T2();
         return;
      }

      _services.Add(interfaceType, existingInstance ?? new T2());
   }
}
