using DAL.Abstracts;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UnitOfWork: IDisposable, IUnitOfWork
    {
       
        private readonly DbProviderFactory providerFactory;
        private readonly IDbConnection connection;

        public IOrderRepository Orders { get; private set; }
        public IProductRepository Products { get; private set; }
        public IEmployeeTerritoriesRepository EmployeeTerritories { get; private set; }

        public UnitOfWork(string connectionString, string providerName)
        {
            providerFactory = DbProviderFactories.GetFactory(providerName);

            connection = providerFactory.CreateConnection();
            connection.ConnectionString = connectionString;
            connection.Open();


            Orders = new OrderRepository(connection);
            Products = new ProductRepository(connection);
            EmployeeTerritories = new EmployeeTerritoriesRepository(connection);
        }
        
        public void Dispose()
        {
            connection.Close();
        }
    }
}
