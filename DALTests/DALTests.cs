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
using DAL.Exceptions;

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
                OrderDate = null,


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

            TransactionScope scope = new TransactionScope();

            Order order = AddOrderToDB();

            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {

                uw.Orders.Create(order);


                Order resultOrder = uw.Orders.Find(order);


                scope.Dispose();


                Assert.IsTrue(new OrderComparator().Compare(order, resultOrder));

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
        public void UpdateOrderTest()
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
        [ExpectedException(typeof(ProhibitionUpdateException))]
        public void UpdateOrderDateTest()
        {

            TransactionScope scope = new TransactionScope();

            try
            {
                Order order = AddOrderToDB();

                Order updateOrder = new Order()
                {
                    OrderID = 4444444,
                    OrderDate = DateTime.Now
                };

                using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
                {
                    uw.Orders.Update(updateOrder);

                }
            }
            catch (ProhibitionUpdateException e )
            {
                throw new ProhibitionUpdateException(e.Message);

            }
            finally
            {
                scope.Dispose();
            }


        }

        [TestMethod]
        [ExpectedException(typeof(ProhibitionUpdateException))]
        public void UpdateShippedDateTest()
        {
            TransactionScope scope = new TransactionScope();
            try
            {
                Order updateOrder = new Order()
                {
                    OrderID = 4444444,
                    ShippedDate = DateTime.Now
                };               

                Order order = AddOrderToDB();


                using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
                {
                    uw.Orders.Update(updateOrder);
                    
                }
            }
            catch (ProhibitionUpdateException e )
            {
                throw new ProhibitionUpdateException(e.Message);
            }
            finally
            {
                scope.Dispose();
            }
          
        }

        [TestMethod]
        public void SetOrderAsUnderwayTest()
        {

            TransactionScope scope = new TransactionScope();

            Order order = AddOrderToDB();

            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {
                DateTime date = new DateTime(2018, 12, 1);

                uw.Orders.SetOrderAsUnderway(order, date);

                Order expectedOrder = uw.Orders.Find(order);

                scope.Dispose();

                Assert.AreEqual(expectedOrder.OrderStatus, OrderStatuses.Underway);


            }




        }

        [TestMethod]
        public void SetOrderAsCompletedTest()
        {

            TransactionScope scope = new TransactionScope();

            Order createdOrder = AddOrderToDB();

            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {
                DateTime date = new DateTime(2018, 12, 1);

                uw.Orders.SetOrderAsCompleted(createdOrder, date);

                Order expectedOrder = uw.Orders.Find(createdOrder);

                scope.Dispose();

                Assert.AreEqual(expectedOrder.OrderStatus, OrderStatuses.Completed);


            }
        }

        [TestMethod]
        public void CustOrderHistoryTest()
        {
            string custId = "ALFKI";

            List<OrderHistory> expectedOrderHistory = new List<OrderHistory>()
            {
                new OrderHistory() { ProductName = "Aniseed Syrup", Total = 6 },
                new OrderHistory() { ProductName = "Chartreuse verte", Total = 21 },
                new OrderHistory() { ProductName = "Escargots de Bourgogne", Total = 40 },
                new OrderHistory() { ProductName = "Flotemysost", Total = 20 },
                new OrderHistory() { ProductName = "Grandma's Boysenberry Spread", Total = 16 },
                new OrderHistory() { ProductName = "Lakkalikööri", Total = 15 },
                new OrderHistory() { ProductName = "Original Frankfurter grüne Soße", Total = 2 },
                new OrderHistory() { ProductName = "Raclette Courdavault", Total = 15 },
                new OrderHistory() { ProductName = "Rössle Sauerkraut", Total = 17 },
                new OrderHistory() { ProductName = "Spegesild", Total = 2 },
                new OrderHistory() { ProductName = "Vegie-spread", Total = 20 },

            };

            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {
                IEnumerable<OrderHistory> resultOrderHistory = uw.Orders.GetCustOrderHistory(custId);
                CollectionAssert.AreEqual(expectedOrderHistory, resultOrderHistory.ToList(), new OrderHistoryComparer());

            }


        }

        [TestMethod]
        public void CustOrderDetailsTest()
        {

            int orderId = 10248;


            List<CustOrderDetail> expectedCustOrderDetails = new List<CustOrderDetail>()
            {
                new CustOrderDetail() {ProductName = "Queso Cabrales", UnitPrice = 14.00M, Quantity =12, ExtendedPrice = 168.00M   },
                new CustOrderDetail() {ProductName = "Singaporean Hokkien Fried Mee", UnitPrice = 9.80M, Quantity =10, ExtendedPrice = 98.00M   },
                new CustOrderDetail() {ProductName = "Mozzarella di Giovanni", UnitPrice = 34.80M, Quantity =5, ExtendedPrice = 174.00M   }
            };



            using (UnitOfWork uw = new UnitOfWork(connectionString, providerName))
            {
                IEnumerable<CustOrderDetail> resultOrderHistory = uw.Orders.GetCustOrdersDetail(orderId);

                CollectionAssert.AreEqual(expectedCustOrderDetails, resultOrderHistory.ToList(), new CustOrderDetailsComparator());

            }







        }
    }
}
