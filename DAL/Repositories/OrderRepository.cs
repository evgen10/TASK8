using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Abstracts;
using DAL.Entities;
using System.Data.Common;
using System.Data;
using System.Reflection;
using DAL.Attributes;

namespace DAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {

        private readonly DbProviderFactory providerFactory;
        private readonly string connectionString;


        public OrderRepository(string connectionString, string providerName)
        {
            providerFactory = DbProviderFactories.GetFactory(providerName);
            this.connectionString = connectionString;
        }

       
        //нужен рефакторинг
        public IEnumerable<Order> GetAll()
        {
            List<Order> orders = new List<Order>();
            Type type = typeof(Order);
            var properties = type.GetProperties();


            using (IDbConnection connection = providerFactory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "select * from dbo.Orders";/////изменить
                    command.CommandType = CommandType.Text;

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var order = new Order();

                            foreach (var property in properties)
                            {
                                if (!Attribute.IsDefined(property,typeof(NotColumnAttribute)))
                                {
                                    if (reader[property.Name] is DBNull)
                                    {
                                        property.SetValue(order, null);
                                    }
                                    else
                                    {
                                        property.SetValue(order, reader[property.Name]);
                                    }
                                }                         

                            }                         

                            orders.Add(order);
                        }
                    }

                }

            }

            return orders;
        }



      



        

       
    }
}
