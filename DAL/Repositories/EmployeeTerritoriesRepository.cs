using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Abstracts;

namespace DAL.Repositories
{
    public class EmployeeTerritoriesRepository : Repository<EmployeeTerritories>, IEmployeeTerritoriesRepository
    {
        public EmployeeTerritoriesRepository(string connectionString, string providerName) : base(connectionString, providerName)
        {
        }
    }
}
