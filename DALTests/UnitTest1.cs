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
            OrderRepository or = new OrderRepository(connectionString, providerName);
            IEnumerable<Order> orders = or.GetAll();
        }

        [TestMethod]
        public void GetAllProductTest()
        {
        
            ProductRepository or = new ProductRepository(connectionString, providerName);

            IEnumerable<Product> orders = or.GetAll();

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

            OrderRepository or = new OrderRepository(connectionString, providerName);

            or.Create(order);

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


            ProductRepository pr = new ProductRepository(connectionString, providerName);
            pr.Create(product);




        }
        

        [TestMethod]
        public void DeleteEmployeeTerritories()
        {

            EmployeeTerritories empTer = new EmployeeTerritories()
            {
                EmployeeID = 1,
                TerritoryID = "06897"
            };


            EmployeeTerritoriesRepository r = new EmployeeTerritoriesRepository(connectionString, providerName);
            r.Delete(empTer);

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

            OrderRepository or = new OrderRepository(connectionString, providerName);

            or.Delete(order);



        }

        [TestMethod]
        public void GetOrderNomenclature()
        {
            OrderNomenclature orderNom;

            OrderRepository or = new OrderRepository(connectionString,providerName);

            orderNom = or.GetOrderNomenclature(5522200);


        }
    }
}
