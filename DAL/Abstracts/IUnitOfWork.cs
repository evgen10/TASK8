using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Abstracts
{
    public interface IUnitOfWork
    {
        IOrderRepository Orders { get; }
        IProductRepository Products { get; }
        IEmployeeTerritoriesRepository EmployeeTerritories { get;}

    }
}
