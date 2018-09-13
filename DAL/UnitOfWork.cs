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

        public IDbConnection Connection { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IProductRepository Products { get; private set; }
        public IEmployeeTerritoriesRepository EmployeeTerritories { get; private set; }

        public UnitOfWork(string connectionString, string providerName)
        {
            providerFactory = DbProviderFactories.GetFactory(providerName);

            Connection = providerFactory.CreateConnection();
            Connection.ConnectionString = connectionString;
            Connection.Open();


            Orders = new OrderRepository(Connection);
            Products = new ProductRepository(Connection);
            EmployeeTerritories = new EmployeeTerritoriesRepository(Connection);
        }
        
        public void Dispose()
        {
            Connection.Close();
        }
    }
}
