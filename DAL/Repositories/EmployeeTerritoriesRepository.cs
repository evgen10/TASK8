using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Abstracts;
using System.Data;

namespace DAL.Repositories
{
    public class EmployeeTerritoriesRepository : Repository<EmployeeTerritories>, IEmployeeTerritoriesRepository
    {
        public EmployeeTerritoriesRepository(IDbConnection connection) : base(connection)
        {
        }
    }
}
