using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DAL.Entities;
using DAL.Repositories;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DALTests
{
    [TestClass]
    public class DALTests
    {
        [TestMethod]
        public void OrderStatusShouldBeNew()
        {
            Order order = new Order()
            {
                OrderDate = null
            };


            Assert.AreEqual(order.OrderStatus, OrderStatuses.New);

        }

        [TestMethod]
        public void OrderStatusShouldBeUnderway()
        {
            Order order = new Order()
            {
                OrderDate = DateTime.Now,
                ShippedDate = null
            };


            Assert.AreEqual(order.OrderStatus, OrderStatuses.Underway);


        }


        [TestMethod]
        public void OrderStatusShouldBeComplited()
        {
            Order order = new Order()
            {
                OrderDate = DateTime.Now,
                ShippedDate = DateTime.Now
            };


            Assert.AreEqual(order.OrderStatus, OrderStatuses.Completed);


        }



        [TestMethod]
        public void GetAllOrdersTest()
        {          

            string connectionString = @"Data Source = (localdb)\ProjectsV13; Initial Catalog = Northwind_1; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = True; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";

            OrderRepository or = new OrderRepository(connectionString, "System.Data.SqlClient");

            IEnumerable<Order> orders = or.GetAll();

        }
    }
}
