using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DAL.Entities;
using DAL.Repositories;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using DAL;
using DALTests.Comparators;
using System.Transactions;
using System.Data;
using System.Data.Common;

namespace DALTests
{
    [TestClass]
    public class DALTests
    {
        private string connectionString = @"Data Source = (localdb)\ProjectsV13; Initial Catalog = Northwind_1; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = True; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
        private string providerName = "System.Data.SqlClient";


        private Order AddOrderToDB()
        {

            Order order = new Order
            {
                OrderID = 4444444,
                CustomerID = "KOENE",
                EmployeeID = 9,
                OrderDate = null,
                RequiredDate = null,
                ShippedDate = null,
                ShipVia = 2,
                Freight = (decimal)21.19,
                ShipName = "Königlich Essen",
                ShipAddress = "Maubelstr. 90",
                ShipCity = "Brandenburg",
                ShipRegion = null,
                ShipPostalCode = "14776",
                ShipCountry = "Germany"

            };

            var factory = DbProviderFactories.GetFactory(providerName);

            using (IDbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                IDbCommand command = connection.CreateCommand();
                command.CommandText = @"set identity_insert Orders on 
                                       INSERT INTO Orders ( OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate,
                                                            ShippedDate, ShipVia, Freight, ShipName, ShipAddress,
                                                            ShipCity, ShipRegion, ShipPostalCode, ShipCountry)
                                        VALUES(4444444, N'KOENE', 9, NULL, NULL, NULL, 2, 21.19,
                                        N'Königlich Essen', N'Maubelstr. 90', N'Brandenburg', NULL, N'14776', N'Germany')
                                        set identity_insert Orders off ";


                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                return order;
            }





        }


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

            Order order = new Order
            {
                CustomerID = "CACTU",
                EmployeeID = 4,
                OrderDate = DateTime.Now,
                RequiredDate = DateTime.Now,
                ShipCountry = "RK"

            };

            int orderCount = 830;

            TransactionScope scope = new TransactionScope();

            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {

                uw.Orders.Create(order);
                orderCount++;

                var beforeCreateOrderCount = uw.Orders.GetAll().Count();
                scope.Dispose();

                Assert.AreEqual(orderCount, beforeCreateOrderCount);

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


            int productCount = 77;

            TransactionScope scope = new TransactionScope();

            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {

                uw.Products.Create(product);
                productCount++;
                var beforeCreateProductCount = uw.Products.GetAll().Count();

                scope.Dispose();
                Assert.AreEqual(productCount, beforeCreateProductCount);


            }




        }

        [TestMethod]
        public void DeleteEmployeeTerritories()
        {
            EmployeeTerritories empTer = new EmployeeTerritories()
            {
                EmployeeID = 1,
                TerritoryID = "06897"
            };

            TransactionScope scope = new TransactionScope();

            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {
                uw.EmployeeTerritories.Delete(empTer);

                EmployeeTerritories deletedEmpTer = uw.EmployeeTerritories.Find(empTer);

                scope.Dispose();

                Assert.IsNull(deletedEmpTer);


            }


        }

        [TestMethod]
        public void DeleteOrder()
        {

            TransactionScope scope = new TransactionScope();
            Order order = AddOrderToDB();

            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {

                uw.Orders.Delete(order);

                Order deletedOrder = uw.Orders.Find(order);

                scope.Dispose();

                Assert.IsNull(deletedOrder);

            }

        }

        [TestMethod]
        public void GetOrderNomenclature()
        {
            OrderNomenclature expectedOrderNom = new OrderNomenclature()
            {
                CustomerID = "VINET",
                Discount = 0,
                EmployeeID = 5,
                Freight = (decimal)32.3800,
                OrderDate = DateTime.Parse("7/4/1996 12:00:00 AM"),
                OrderID = 10248,
                ProductID = 11,
                ProductName = "Queso Cabrales",
                Quantity = 12,
                RequiredDate = DateTime.Parse("8/1/1996 12:00:00 AM"),
                ShipAddress = "59 rue de l'Abbaye",
                ShipCity = "Reims",
                ShipCountry = "France",
                ShipName = "Vins et alcools Chevalier",
                ShipPostalCode = "51100",
                ShipRegion = null,
                ShipVia = 3,
                ShippedDate = DateTime.Parse("7/16/1996 12:00:00 AM"),
                UnitPrice = (decimal)14.0000,

            };


            OrderNomenclature resultOrderNom;

            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {
                resultOrderNom = uw.Orders.GetOrderNomenclature(10248);

                Assert.IsTrue(new OrderNomenclatureComparator().Comparer(expectedOrderNom, resultOrderNom));

            }




        }

        [TestMethod]
        public void FidnOrderTest()
        {

            OrderNomenclature expectedOrder = new OrderNomenclature()
            {
                CustomerID = "VINET",               
                EmployeeID = 5,
                Freight = (decimal)32.3800,
                OrderDate = DateTime.Parse("7/4/1996 12:00:00 AM"),
                OrderID = 10248,              
                RequiredDate = DateTime.Parse("8/1/1996 12:00:00 AM"),
                ShipAddress = "59 rue de l'Abbaye",
                ShipCity = "Reims",
                ShipCountry = "France",
                ShipName = "Vins et alcools Chevalier",
                ShipPostalCode = "51100",
                ShipRegion = null,
                ShipVia = 3,
                ShippedDate = DateTime.Parse("7/16/1996 12:00:00 AM"),
         

            };


            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {
                Order resultOrder = uw.Orders.Find(expectedOrder);

                Assert.IsTrue(new OrderComparator().Compare(expectedOrder, resultOrder));

            }

                

            

        }


        [TestMethod]
        public void UpdateOrder()
        {       

            Order expecteOrder = new Order()
            {
                OrderID = 4444444,
                CustomerID = "KOENE",
                EmployeeID = 9,
                OrderDate = null,
                RequiredDate = null,
                ShippedDate = null,
                ShipVia = 2,
                Freight = (decimal)21.19,
                ShipName = "Change",
                ShipAddress = "Change",
                ShipCity = "Change",
                ShipRegion = null,
                ShipPostalCode = "Change",
                ShipCountry = "Change"
            };

            TransactionScope scope = new TransactionScope();

            Order sourceOrder = AddOrderToDB();

            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {

                uw.Orders.Update(expecteOrder);
                Order resultOrder = uw.Orders.Find(expecteOrder);
                scope.Dispose();

                Assert.IsTrue(new OrderComparator().Compare(expecteOrder, resultOrder));
            }              
            
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
