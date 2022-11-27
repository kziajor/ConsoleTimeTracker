using Microsoft.Data.Sqlite;

namespace App.Repositories;

public class BaseRepository
{
   protected readonly string _connectionString;

   public BaseRepository(string connectionString)
   {
      _connectionString = connectionString;
   }

   protected T Query<T>(Func<SqliteConnection, T> query)
   {
      using var connection = new SqliteConnection(_connectionString);
      connection.Open();
      return query.Invoke(connection);
   }
}
