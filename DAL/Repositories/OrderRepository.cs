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
using System.Data.SqlClient;
using DAL.Exceptions;

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
                throw new ProhibitionDeleteException("Can not delete order with status \"Completed\"");
            }
        }

        public override void Update(Order order)
        {
            Order sourceOrder = Find(order);


            if (sourceOrder.OrderStatus == OrderStatuses.Underway || sourceOrder.OrderStatus == OrderStatuses.Completed)
            {
                throw new ProhibitionUpdateException("Order status prohibits changes");
            }



            Type type = order.GetType();
            IEnumerable<PropertyInfo> unchangeableProperties = type.GetProperties().Where(p => p.IsDefined(typeof(UnchangeableAttribute)));

            if (sourceOrder != null)
            {
                foreach (var property in unchangeableProperties)
                {
                    if (!Equals(property.GetValue(sourceOrder), property.GetValue(order)))
                    {
                        throw new ProhibitionUpdateException($"Can not change the value of property {property.Name}");
                    }
                }

                base.Update(order);

            }
            else
            {
                throw new EntityNotExistsException("No entry found");
            }




        }

        public void SetOrderAsUnderway(Order order, DateTime orderDate)
        {
            Order sourceOrder = Find(order);

            if (sourceOrder != null)
            {
                sourceOrder.OrderDate = orderDate;
                base.Update(sourceOrder);
            }
            else
            {
                throw new EntityNotExistsException("No entry found");
            }

        }

        public void SetOrderAsCompleted(Order order, DateTime shippedDate)
        {
            Order sourceOrder = Find(order);

            if (sourceOrder != null)
            {
                sourceOrder.ShippedDate = shippedDate;
                base.Update(sourceOrder);
            }
            else
            {
                throw new EntityNotExistsException("No entry found");
            }
        }

        public IEnumerable<OrderHistory> GetCustOrderHistory(string customerId)
        {
            IDbCommand command = connection.CreateCommand();

            command.CommandText = "CustOrderHist";
            command.CommandType = CommandType.StoredProcedure;

            IDbDataParameter custIdPrarm = command.CreateParameter();
            custIdPrarm.ParameterName = "@CustomerID";
            custIdPrarm.DbType = DbType.StringFixedLength;
            custIdPrarm.Value = customerId;

            command.Parameters.Add(custIdPrarm);

            return Mapper<OrderHistory>(command);


        }

        public IEnumerable<CustOrderDetail> GetCustOrdersDetail(int orderId)
        {
            IDbCommand command = connection.CreateCommand();

            command.CommandText = "CustOrdersDetail";
            command.CommandType = CommandType.StoredProcedure;

            IDbDataParameter custIdPrarm = command.CreateParameter();
            custIdPrarm.ParameterName = "@OrderID";
            custIdPrarm.DbType = DbType.Int32;
            custIdPrarm.Value = orderId;

            command.Parameters.Add(custIdPrarm);

            return Mapper<CustOrderDetail>(command);
        }

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
