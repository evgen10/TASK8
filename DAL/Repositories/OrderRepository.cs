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


        public override void Delete(Order order)//(Задание 5)
        {
            //запрет на удаление заказов со статусом Completed
            if (order.OrderStatus != OrderStatuses.Completed)
            {
                base.Delete(order);
            }
            else
            {
                throw new ProhibitionDeleteException("Can not delete order with status \"Completed\"");
            }
        }

        public override void Update(Order order)//(Задание 4)
        {
            //проверяем есть ли данный заказ в базе
            Order sourceOrder = Find(order);

            // запрет на изменения данных в заказах со статусами Underway и Completed
            if (sourceOrder.OrderStatus == OrderStatuses.Underway || sourceOrder.OrderStatus == OrderStatuses.Completed)//(Задание 4 с)
            {
                throw new ProhibitionUpdateException("Order status prohibits changes");
            }



            //получаем свойства помеченные атрибутом Unchangeable Attribute
            Type type = order.GetType();
            IEnumerable<PropertyInfo> unchangeableProperties = type.GetProperties().Where(p => p.IsDefined(typeof(UnchangeableAttribute)));


            
            if (sourceOrder != null)//(Задание 4 а, b)
            {
                //проверяем меняются ли эти свойства 
                foreach (var property in unchangeableProperties)
                {
                    if (!Equals(property.GetValue(sourceOrder), property.GetValue(order)))
                    {
                        throw new ProhibitionUpdateException($"Can not change the value of property {property.Name}");
                    }
                }

                //если нет то изменяем сущность
                base.Update(order);

            }
            else
            {
                throw new EntityNotExistsException("No entry found");
            }




        }


        /// <summary>
        /// Присваивает заказу статус "Underway"
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDate"></param>
        public void SetOrderAsUnderway(Order order, DateTime orderDate)//(Задание 6 а)
        {
            Order sourceOrder = Find(order);

            if (sourceOrder != null)
            {
                sourceOrder.OrderDate = orderDate;
                //вызывается базовый метод т.к в предопределенном запрещается менять данное свойство
                base.Update(sourceOrder);
            }
            else
            {
                throw new EntityNotExistsException("No entry found");
            }

        }

        /// <summary>
        ///  Присваивает заказу статус "Completed"
        /// </summary>
        /// <param name="order"></param>
        /// <param name="shippedDate"></param>
        public void SetOrderAsCompleted(Order order, DateTime shippedDate)//(Задание 6 b)
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

        /// <summary>
        /// Вызвает хранимую процедуру "CustOrderHist" 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public IEnumerable<OrderHistory> GetCustOrderHistory(string customerId)//(Задание 7 а)
        {
            IDbCommand command = connection.CreateCommand();

            command.CommandText = "CustOrderHist";
            command.CommandType = CommandType.StoredProcedure;

            //создаем параметр
            IDbDataParameter custIdPrarm = command.CreateParameter();
            custIdPrarm.ParameterName = "@CustomerID";
            custIdPrarm.DbType = DbType.StringFixedLength;
            custIdPrarm.Value = customerId;

            command.Parameters.Add(custIdPrarm);

            return Mapper<OrderHistory>(command);


        }

        /// <summary>
        /// Вызвает хранимую процедуру "CustOrdersDetail" 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public IEnumerable<CustOrderDetail> GetCustOrdersDetail(int orderId)//(Задание 7 b)
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

        /// <summary>
        /// Показывать подробные сведения о конкретном заказе 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public OrderNomenclature GetOrderNomenclature(int orderId)// (Задание 2)
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


        /// <summary>
        /// Возвращает запрос  для подробных сведений о конкретном заказе 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
