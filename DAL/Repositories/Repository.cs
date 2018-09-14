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
      
        protected readonly IDbConnection connection;

        public Repository(IDbConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Добавить сущность в базу данных
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Create(TEntity entity)//(Задание 3)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = GetInsertQuery(typeof(TEntity).Name, entity);
            command.CommandType = CommandType.Text;

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Удалить сущность из базы данных
        /// </summary>
        /// <param name="entity"></param>
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

        /// <summary>
        /// Получить все сущности из базы данных
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetAll()//(Задание 1)
        {

            IDbCommand command = connection.CreateCommand();
            command.CommandText = GetAllOrderQuery(typeof(TEntity).Name);
            command.CommandType = CommandType.Text;

            return Mapper<TEntity>(command);


        }
        
        /// <summary>
        /// Изменение сущности в базе данных
        /// </summary>
        /// <param name="entity"></param>
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

        /// <summary>
        /// Поиск сущности в базе данных
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Проецирует записи в таблицы в коллекцию объектов
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
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
                        //не рассматриваем поля помеченные атрибутом NotColumn
                        if (!property.IsDefined(typeof(NotColumnAttribute)))
                        {
                            //если поле в бд имеет значение DBNull
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

        /// <summary>
        /// Возвращает запрос для выбора всех элементов с таблицы
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string GetAllOrderQuery(string tableName)
        {
            return $"select * from dbo.{GetPluralize(tableName)}";
        }

        /// <summary>
        /// Формирует запрос на вставку в базу данных
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Формирует запрос на удаление из базы данных
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GetDeleteQuery(string tableName, TEntity entity)
        {

            tableName = GetPluralize(tableName);

            StringBuilder deleteCondition = GetCondition(entity);

            return $"delete from {tableName} where {deleteCondition}";


        }

        /// <summary>
        /// Возвращает запрос для поиска записи
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GetFindQuery(string tableName, TEntity entity)
        {
            StringBuilder condition = GetCondition(entity);

            tableName = GetPluralize(tableName);

            return $"select * from {tableName} where {condition}";
        }

        /// <summary>
        /// Возвращает запрос для обновления данных в таблице
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="sourceEntity"></param>
        /// <param name="changeableEntity"></param>
        /// <returns></returns>
        private string GetUpdateQuery(string tableName, TEntity sourceEntity , TEntity changeableEntity)
        {
            StringBuilder settings  = GetUpdateSetting(sourceEntity, changeableEntity);
            StringBuilder condition = GetCondition(changeableEntity);

            tableName = GetPluralize(tableName);      
       

            return $"update {tableName} set {settings} where {condition}";
        }

        /// <summary>
        /// Формирует область SET для обновления данных
        /// </summary>
        /// <param name="sourceEntity"></param>
        /// <param name="changeableEntity"></param>
        /// <returns></returns>
        private StringBuilder GetUpdateSetting(TEntity sourceEntity, TEntity changeableEntity)
        {
            Type type = sourceEntity.GetType();

            //НЕ рассматриваем ключевые поля и поля которых нет в таблице
            List<PropertyInfo> properties = type.GetProperties().Where(p => !p.IsDefined(typeof(KeyAttribute)) && !p.IsDefined(typeof(NotColumnAttribute))).ToList();

            StringBuilder settings = new StringBuilder();          

            foreach (var property in properties)
            {
               //меняем данные если исходные данные не равны изменяемым
                if (!Equals(property.GetValue(sourceEntity), property.GetValue(changeableEntity)))
                {
                    //если изменяемое свойство null
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

        /// <summary>
        /// Возвращает условие опираясь на ключевые поля
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private StringBuilder GetCondition(TEntity entity)
        {
            //получаем только ключевые поля
            Type type = entity.GetType();
            IEnumerable<PropertyInfo> properties = type.GetProperties().Where(p => p.IsDefined(typeof(KeyAttribute)));

            StringBuilder condition = new StringBuilder();

            if (properties.Count() != 0)
            {
                //если ключей больше чем 1 то формируется сложное условие
                if (properties.Count() > 1)
                {
                    string andStr = " AND ";

                    foreach (var property in properties)
                    {
                        condition.Append($"{property.Name}='{property.GetValue(entity)}'");
                        condition.Append(andStr);
                    }

                    //убираем лишний AND
                    condition.Length -= andStr.Length;
                }
                else
                {
                    condition.Append($"{properties.First().Name}='{properties.First().GetValue(entity)}'");
                }

            }
            else
            {
                throw new LackKeyFieldException($"There is no  a key property in entity {type.Name}");
            }

            return condition;
        }

        /// <summary>
        /// Переводит название сущности в множественное число
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private string GetPluralize(string word)
        {
            PluralizationService server = PluralizationService.CreateService(CultureInfo.CurrentCulture);
            word = server.Pluralize(word);

            return word;
        }

        /// <summary>
        /// Получаем свойства поля для вставки в базу данных
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private IEnumerable<PropertyInfo> GetDateForInsertUpdate(TEntity entity)
        {

            Type type = entity.GetType();

            //поля не должны быть помечены как Key, NotColumn и не быть null
            IEnumerable<PropertyInfo> properties = type.GetProperties().Where(p => !p.IsDefined(typeof(NotColumnAttribute)) &&
                                                             !p.IsDefined(typeof(KeyAttribute)) &&
                                                              p.GetValue(entity) != null);

            return properties;

        }

    }
}
