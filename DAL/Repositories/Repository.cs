using DAL.Abstracts;
using DAL.Attributes;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace DAL.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity, new()
    {
        protected readonly DbProviderFactory providerFactory;
        protected readonly string connectionString;

        public Repository(string connectionString, string providerName)
        {
            providerFactory = DbProviderFactories.GetFactory(providerName);
            this.connectionString = connectionString;
        }

        public virtual void Create(TEntity entity)
        {
            using (IDbConnection connection = providerFactory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                IDbCommand command = connection.CreateCommand();
                command.CommandText = GetInsertQuery(typeof(TEntity).Name, entity);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();


            }
        }

        public virtual void Delete(TEntity entity)
        {
            using (IDbConnection connection = providerFactory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                IDbCommand command = connection.CreateCommand();

                command.CommandText = GetDeleteQuery(typeof(TEntity).Name, entity);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }

        public virtual IEnumerable<TEntity> GetAll()
        {

            using (IDbConnection connection = providerFactory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                IDbCommand command = connection.CreateCommand();
                command.CommandText = GetAllOrderQuery(typeof(TEntity).Name);
                command.CommandType = CommandType.Text;

                return Mapper<TEntity>(command);
            }

        }

        public virtual void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        //public virtual TEntity Find(object id)
        //{

        //}

        protected IEnumerable<T> Mapper<T>(IDbCommand command) where T : new()
        {
            List<T> list = new List<T>();
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var entity = new T();

                    foreach (var property in properties)
                    {
                        if (!property.IsDefined(typeof(NotColumnAttribute)))
                        {
                            if (reader[property.Name] is DBNull)
                            {
                                property.SetValue(entity, null);
                            }
                            else
                            {
                                property.SetValue(entity, reader[property.Name]);
                            }
                        }
                    }

                    list.Add(entity);
                }

                return list;
            }
        }

        private string GetAllOrderQuery(string tableName)
        {
            return $"select * from dbo.{GetPluralize(tableName)}";
        }

        private string GetInsertQuery(string tableName, TEntity entity)
        {
            tableName = GetPluralize(tableName);

            StringBuilder columns = new StringBuilder();
            StringBuilder values = new StringBuilder();

            foreach (PropertyInfo item in GetDateForInsert(entity))
            {
                columns.Append(item.Name);
                columns.Append(",");

                values.Append(@"'");
                values.Append(item.GetValue(entity));
                values.Append(@"'");
                values.Append(",");

            }

            //убираем лишние запятые
            columns.Length--;
            values.Length--;

            return $"Insert into {tableName} ({columns}) values({values})";

        }

        private string GetDeleteQuery(string tableName, TEntity entity)
        {
            StringBuilder idColumns = new StringBuilder();

            tableName = GetPluralize(tableName);

            Type type = entity.GetType();
            List<PropertyInfo> properties = type.GetProperties().Where(p => p.IsDefined(typeof(KeyAttribute))).ToList();

            if (properties.Count != 0)
            {
                if (properties.Count>1)
                {
                    string andStr = " AND ";

                    foreach (var property in properties)
                    {
                        idColumns.Append($"{property.Name}='{property.GetValue(entity)}'");
                        idColumns.Append(andStr);                   
                        
                    }

                    idColumns.Length -=andStr.Length;
                }
                else
                {
                    idColumns.Append($"{properties.First().Name}='{properties.First().GetValue(entity)}'");
                }             
                
            }
            else
            {
                throw new Exception($"There is no key to delete in entity {type.Name}");
            }

            return $"delete from {tableName} where {idColumns}";


        }

        private string GetPluralize(string word)
        {
            PluralizationService server = PluralizationService.CreateService(CultureInfo.CurrentCulture);
            word = server.Pluralize(word);

            return word;
        }
        
        private List<PropertyInfo> GetDateForInsert(TEntity entity)
        {

            Type type = entity.GetType();
            List<PropertyInfo> properties = type.GetProperties().Where(p => !p.IsDefined(typeof(NotColumnAttribute)) &&
                                                             !p.IsDefined(typeof(KeyAttribute)) &&
                                                              p.GetValue(entity) != null).ToList();

            return properties;

        }
        
     

    }
}
