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
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(string connectionString, string providerName):base(connectionString,providerName)
        {
            
        } 

        public new void  Delete(Order order)
        {
            if (order.OrderStatus!= OrderStatuses.Completed)
            {
               base.Delete(order);
            }
            else
            {
                throw new Exception("Can not delete order with status \"Completed\"");
            }            
        }

        public OrderNomenclature GetOrderNomenclature(int orderId)
        {
            using (IDbConnection connection = providerFactory.CreateConnection())
            {
                try
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();

                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = GetOrderNomenclaturQuery(orderId);
                    command.CommandType = CommandType.Text;


                    return Mapper<OrderNomenclature>(command).First();
                }
                catch (InvalidOperationException e)
                {
                    return null;
                }
             

            }
        }
        
        private string GetOrderNomenclaturQuery(int id)
        {
            return @"select o.*,od.ProductID,p.ProductName, od.UnitPrice,od.Quantity,od.Discount
                    from dbo.Orders o
                    join dbo.[Order Details] od on o.OrderID = od.OrderID
                    join dbo.Products p on p.ProductID = od.ProductID" +
                    $" where o.OrderID = {id}";
        }


    }
}
