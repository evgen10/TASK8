using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DAL.Entities;
using DAL.Repositories;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using DAL;

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
            UnitOfWork uw = new UnitOfWork(connectionString, providerName);
            IEnumerable<Order> orders = uw.Orders.GetAll();
        }

        [TestMethod]
        public void GetAllProductTest()
        {

            UnitOfWork uw = new UnitOfWork(connectionString, providerName);
            IEnumerable<Product> products = uw.Products.GetAll();

        }
        
        [TestMethod]
        public void CreateOrder()
        {
            Order order = new Order
            {               
                CustomerID = "CACTU",
                EmployeeID = 4,
                OrderDate = DateTime.Now,
                RequiredDate = DateTime.Now,
                ShipCountry = "RK"

            };

            UnitOfWork uw = new UnitOfWork(connectionString, providerName);

            uw.Orders.Create(order);

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



        }

        [TestMethod]
        public void GetOrderNomenclature()
        {
            OrderNomenclature orderNom;

            UnitOfWork uw = new UnitOfWork(connectionString, providerName);

            orderNom = uw.Orders.GetOrderNomenclature(5522200);


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


        }


        [TestMethod]
        public void UpdateProduct()
        {
            Product product = new Product()
            {
                ProductID = 1,
                ProductName = "Phone",
                SupplierID = 1,
                CategoryID =1,
                QuantityPerUnit = null,
                UnitPrice = 14,
                UnitsInStock = 5,
                UnitsOnOrder = 8,
                ReorderLevel = null,
                Discontinued = true

            };

            UnitOfWork uw = new UnitOfWork(connectionString, providerName);

            uw.Products.Update(product);


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

        }

        [TestMethod]
        public void SetOrderAsUnderway()
        {
            Order order = new Order
            {
                OrderID = 11078           

            };


            UnitOfWork uw = new UnitOfWork(connectionString,providerName);

            uw.Orders.SetOrderAsUnderway(order, DateTime.Now);


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


        }

    }
}
