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
using DAL.Exceptions;

namespace DAL.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity, new()
    {
        protected readonly DbProviderFactory providerFactory;
        protected readonly string connectionString;
        protected readonly IDbConnection connection;

        public Repository(IDbConnection connection)
        {
            this.connection = connection;
        }

        public virtual void Create(TEntity entity)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = GetInsertQuery(typeof(TEntity).Name, entity);
            command.CommandType = CommandType.Text;

            command.ExecuteNonQuery();
        }

        public virtual void Delete(TEntity entity)
        {
            IDbCommand command = connection.CreateCommand();

            if (Find(entity) != null)
            {
                command.CommandText = GetDeleteQuery(typeof(TEntity).Name, entity);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
            else
            {
                throw new EntityNotExistsException("No entry found");
            }


        }

        public virtual IEnumerable<TEntity> GetAll()
        {

            IDbCommand command = connection.CreateCommand();
            command.CommandText = GetAllOrderQuery(typeof(TEntity).Name);
            command.CommandType = CommandType.Text;

            return Mapper<TEntity>(command);


        }
        
        public virtual void Update(TEntity entity)
        {
            IDbCommand command = connection.CreateCommand();

            TEntity sourceEntity = Find(entity);

            if (sourceEntity != null)
            {
                command.CommandText = GetUpdateQuery(typeof(TEntity).Name, sourceEntity, entity);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
            else
            {
                throw new EntityNotExistsException("No entry found");
            }
        }

        public virtual TEntity Find(TEntity entity)
        {
            try
            {
                IDbCommand command = connection.CreateCommand();

                command.CommandText = GetFindQuery(typeof(TEntity).Name, entity);
                command.CommandType = CommandType.Text;
                return Mapper<TEntity>(command).First();
            }
            catch (InvalidOperationException)
            {
                return null;
            }

        }

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

            foreach (PropertyInfo item in GetDateForInsertUpdate(entity))
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

            tableName = GetPluralize(tableName);

            StringBuilder idColumns = GetCondition(entity);

            return $"delete from {tableName} where {idColumns}";


        }

        private string GetFindQuery(string tableName, TEntity entity)
        {
            StringBuilder condition = GetCondition(entity);

            tableName = GetPluralize(tableName);

            return $"select * from {tableName} where {condition}";
        }

        private string GetUpdateQuery(string tableName, TEntity sourceEntity , TEntity changeableEntity)
        {
            StringBuilder settings  = GetUpdateSetting(sourceEntity, changeableEntity);
            StringBuilder condition = GetCondition(changeableEntity);

            tableName = GetPluralize(tableName);      
       

            return $"update {tableName} set {settings} where {condition}";
        }

        private StringBuilder GetUpdateSetting(TEntity sourceEntity, TEntity changeableEntity)
        {
            Type type = sourceEntity.GetType();

            List<PropertyInfo> properties = type.GetProperties().Where(p => !p.IsDefined(typeof(KeyAttribute)) && !p.IsDefined(typeof(NotColumnAttribute))).ToList();

            StringBuilder settings = new StringBuilder();          

            foreach (var property in properties)
            {
               
                if (!Equals(property.GetValue(sourceEntity), property.GetValue(changeableEntity)))
                {
                    if (property.GetValue(changeableEntity) == null)
                    {
                        settings.AppendLine($"{property.Name} = null ");
                    }
                    else
                    {
                        settings.AppendLine($"{property.Name} = '{property.GetValue(changeableEntity)}'");
                    }
                    
                    settings.Append(",");
                }

            }

            settings.Length--; 
            
            return settings;

        }

        private StringBuilder GetCondition(TEntity entity)
        {
            Type type = entity.GetType();
            List<PropertyInfo> properties = type.GetProperties().Where(p => p.IsDefined(typeof(KeyAttribute))).ToList();

            StringBuilder idColumns = new StringBuilder();

            if (properties.Count != 0)
            {
                if (properties.Count > 1)
                {
                    string andStr = " AND ";

                    foreach (var property in properties)
                    {
                        idColumns.Append($"{property.Name}='{property.GetValue(entity)}'");
                        idColumns.Append(andStr);
                    }

                    idColumns.Length -= andStr.Length;
                }
                else
                {
                    idColumns.Append($"{properties.First().Name}='{properties.First().GetValue(entity)}'");
                }

            }
            else
            {
                throw new LackKeyFieldException($"There is no  a key property in entity {type.Name}");
            }

            return idColumns;
        }

        private string GetPluralize(string word)
        {
            PluralizationService server = PluralizationService.CreateService(CultureInfo.CurrentCulture);
            word = server.Pluralize(word);

            return word;
        }

        private List<PropertyInfo> GetDateForInsertUpdate(TEntity entity)
        {

            Type type = entity.GetType();

            List<PropertyInfo> properties = type.GetProperties().Where(p => !p.IsDefined(typeof(NotColumnAttribute)) &&
                                                             !p.IsDefined(typeof(KeyAttribute)) &&
                                                              p.GetValue(entity) != null).ToList();

            return properties;

        }

    }
}
