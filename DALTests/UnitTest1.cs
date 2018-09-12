using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DAL.Entities;
using DAL.Repositories;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using DAL;
using DALTests.Comparators;
namespace DALTests
{
    [TestClass]
    public class DALTests
    {
        private string connectionString = @"Data Source = (localdb)\ProjectsV13; Initial Catalog = Northwind_1; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = True; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
        private string providerName = "System.Data.SqlClient";






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
            int orderCount = 830;

            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {
                IEnumerable<Order> orders = uw.Orders.GetAll();
                Assert.AreEqual(orders.Count(), orderCount);
            }

        }

        [TestMethod]
        public void GetAllProductTest()
        {
            int productCount = 77;
            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {
                IEnumerable<Product> products = uw.Products.GetAll();
                Assert.AreEqual(products.Count(), productCount);
            }



        }

        [TestMethod]
        public void CreateOrder()
        {

            //записываемый в бд заказ
            Order order = new Order
            {               
                CustomerID = "CACTU",
                EmployeeID = 4,
                OrderDate = DateTime.Now,
                RequiredDate = DateTime.Now,
                ShipCountry = "RK"

            };


            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {
                uw.Orders.Create(order);               
                             
                                               
            }

            using (UnitOfWork uow = new UnitOfWork(connectionString, providerName))
            {
                uow.Orders.GetAll().First(x => x == order);
            }            

        }

        [TestMethod]
        public void CreateProduct()
        {
            Product product = new Product()
            {
                ProductName = "Phone",
                SupplierID = 4,
                CategoryID = 4,
                QuantityPerUnit = "10 boxes x 20 bags",
                UnitPrice = (decimal)4584.4,
                UnitsInStock = 10,
                Discontinued = true
            };


            UnitOfWork uw = new UnitOfWork(connectionString, providerName);
            uw.Products.Create(product);
            uw.Dispose();



        }

        [TestMethod]
        public void DeleteEmployeeTerritories()
        {

            EmployeeTerritories empTer = new EmployeeTerritories()
            {
                EmployeeID = 1,
                TerritoryID = "06897"
            };


            UnitOfWork uw = new UnitOfWork(connectionString, providerName);
            uw.EmployeeTerritories.Delete(empTer);
            uw.Dispose();
        }

        [TestMethod]
        public void DeleteOrder()
        {
            Order order = new Order
            {
                OrderID = 12079,
                CustomerID = "HUNGO",
                EmployeeID = 3,
                OrderDate = DateTime.Now,
                RequiredDate = DateTime.Now,
                ShippedDate = DateTime.Now,
                ShipCountry = "RK"

            };

            UnitOfWork uw = new UnitOfWork(connectionString, providerName);

            uw.Orders.Delete(order);

            uw.Dispose();

        }

        [TestMethod]
        public void GetOrderNomenclature()
        {
            OrderNomenclature orderNom;

            UnitOfWork uw = new UnitOfWork(connectionString, providerName);

            orderNom = uw.Orders.GetOrderNomenclature(5522200);

            uw.Dispose();
        }

        [TestMethod]
        public void FindTest()
        {
            Order o = new Order()
            {
                OrderID = 102411
            };

            UnitOfWork uw = new UnitOfWork(connectionString, providerName);

            var order = uw.Orders.Find(o);

            uw.Dispose();

        }


        [TestMethod]
        public void UpdateProduct()
        {
            Product product = new Product()
            {
                ProductID = 1,
                ProductName = "Phone",
                SupplierID = 1,
                CategoryID = 1,
                QuantityPerUnit = null,
                UnitPrice = 14,
                UnitsInStock = 5,
                UnitsOnOrder = 8,
                ReorderLevel = null,
                Discontinued = true

            };

            UnitOfWork uw = new UnitOfWork(connectionString, providerName);

            uw.Products.Update(product);
            uw.Dispose();

        }

        [TestMethod]
        public void UpdateOrder()
        {
            Order order = new Order
            {
                OrderID = 11078,
                CustomerID = "HUNGO",
                EmployeeID = 6,
                // OrderDate = DateTime.Now,
                RequiredDate = DateTime.Now,
                // ShippedDate = DateTime.Now,
                Freight = null,
                ShipName = null,
                ShipPostalCode = null,
                ShipCountry = "RKY",
                ShipRegion = "RJ",
                ShipVia = 3


            };


            UnitOfWork uw = new UnitOfWork(connectionString, providerName);

            uw.Orders.Update(order);
            uw.Dispose();
        }

        [TestMethod]
        public void SetOrderAsUnderway()
        {
            Order order = new Order
            {
                OrderID = 11078

            };


            UnitOfWork uw = new UnitOfWork(connectionString, providerName);

            uw.Orders.SetOrderAsUnderway(order, DateTime.Now);

            uw.Dispose();
        }


        [TestMethod]
        public void SetOrderAsCompleted()
        {
            Order order = new Order
            {
                OrderID = 11078

            };


            UnitOfWork uw = new UnitOfWork(connectionString, providerName);

            uw.Orders.SetOrderAsCompleted(order, DateTime.Now);

            uw.Dispose();
        }


        [TestMethod]
        public void CustOrderHistory()
        {

            string custId = "ANATR";

            UnitOfWork uw = new UnitOfWork(connectionString, providerName);


            IEnumerable<OrderHistory> orderHistory = uw.Orders.GetCustOrderHistory(custId);


            uw.Dispose();

        }

        [TestMethod]
        public void CustOrderDetails()
        {

            int orderId = 10248;

            UnitOfWork uw = new UnitOfWork(connectionString, providerName);


            IEnumerable<CustOrderDetail> orderHistory = uw.Orders.GetCustOrdersDetail(orderId);


            uw.Dispose();

        }
    }
}
