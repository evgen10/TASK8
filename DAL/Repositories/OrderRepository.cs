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
        public OrderRepository(IDbConnection connection) : base(connection)
        {

        }

        public override void Delete(Order order)
        {
            if (order.OrderStatus != OrderStatuses.Completed)
            {
                base.Delete(order);
            }
            else
            {
                throw new Exception("Can not delete order with status \"Completed\"");
            }
        }

        //public override void Update(Order order)
        //{

        //    //if (order.OrderStatus == OrderStatuses.Underway || order.OrderStatus == OrderStatuses.Completed)
        //    //{
        //    //    throw new Exception("Order status prohibits changes");
        //    //}

        //    Type type = order.GetType();
        //    List<PropertyInfo> unchangeableProperty = type.GetProperties().Where(p => p.IsDefined(typeof(UnchangeableAttribute))).ToList();


        //    Order ord = Find(order);

        //    if (ord != null)
        //    {

                
                
                   
                




        //    }
        //    else
        //    {
        //        throw new Exception("No entry found");
        //    }






        //}

        public OrderNomenclature GetOrderNomenclature(int orderId)
        {

            try
            {

                IDbCommand command = connection.CreateCommand();
                command.CommandText = GetOrderNomenclaturQuery(orderId);
                command.CommandType = CommandType.Text;


                return Mapper<OrderNomenclature>(command).First();
            }
            catch (InvalidOperationException)
            {
                return null;
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
