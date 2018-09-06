using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Abstracts;
using DAL.Entities;
using System.Data.Common;
using System.Data;
using System.Reflection;
using DAL.Attributes;

namespace DAL.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(string connectionString, string providerName):base(connectionString,providerName)
        {
            
        } 
        

    }
}
