using Microsoft.Data.SqlClient;
using ShoeMart.Models.Entities;
using ShoeMart.Models.Interfaces;
using System.Reflection;

namespace ShoeMart.Models.Repositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly string connectionString;

        public GenericRepository()
        {
            connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ShoeMart;Integrated Security=True;";
        }

        public void Add(TEntity entity)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // extract table name and columns
                var tableName = typeof(TEntity).Name;
                var properties = typeof(TEntity).GetProperties();

                var columnNames = string.Join(",", properties.Select(p => p.Name));
                var parameterNames = string.Join(",", properties.Select(p => "@" + p.Name));

                var query = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames});";
                using (var command = new SqlCommand(query, conn))
                {
                    foreach (var property in properties)
                    {
                        // adding parameters to avoid sql injection
                        command.Parameters.AddWithValue("@" + property.Name, property.GetValue(entity));
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public TEntity GetById(string id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var tableName = typeof(TEntity).Name;
                var primaryKey = "Id";

                var query = $"SELECT * FROM {tableName} WHERE {primaryKey} = @Id;";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToObject(reader);
                        }
                        return null;
                    }
                }
            }
        }

        public IEnumerable<TEntity> GetAll()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var tableName = typeof(TEntity).Name;

                var query = $"SELECT * FROM {tableName};";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var entities = new List<TEntity>();
                        while (reader.Read())
                        {
                            entities.Add(MapReaderToObject(reader));
                        }
                        return entities;
                    }
                }
            }
        }

        public void Update(TEntity entity)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var tableName = typeof(TEntity).Name;
                var primaryKey = "Id";

                var properties = typeof(TEntity).GetProperties().Where(p => p.Name != primaryKey);

                var setClause = string.Join(",", properties.Select(p => $"{p.Name} = @{p.Name}"));
                var query = $"UPDATE {tableName} SET {setClause} WHERE {primaryKey} = @{primaryKey};";

                using (var command = new SqlCommand(query, connection))
                {
                    foreach (var property in properties)
                    {
                        command.Parameters.AddWithValue("@" + property.Name, property.GetValue(entity));
                    }
                    command.Parameters.AddWithValue("@" + primaryKey, typeof(TEntity).GetProperty(primaryKey).GetValue(entity));

                    command.ExecuteNonQuery();
                }
            }
        }

        public bool Delete(string id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var tableName = typeof(TEntity).Name;
                var primaryKey = "Id";

                var query = $"DELETE FROM {tableName} WHERE {primaryKey} = @Id;";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    int count = command.ExecuteNonQuery();
                    if(count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public IEnumerable<TEntity> GetFeaturedItems()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var tableName = typeof(TEntity).Name;

                var query = $"SELECT TOP 3 * FROM {tableName};";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var entities = new List<TEntity>();
                        while (reader.Read())
                        {
                            entities.Add(MapReaderToObject(reader));
                        }
                        return entities;
                    }
                }
            }
        }

        private TEntity MapReaderToObject(SqlDataReader reader)
        {
            var entity = Activator.CreateInstance<TEntity>();
            foreach (var property in typeof(TEntity).GetProperties())
            {
                if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                {
                    property.SetValue(entity, reader[property.Name]);
                }
            }
            return entity;
        }
    }
}
