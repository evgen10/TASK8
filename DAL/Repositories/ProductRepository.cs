using DAL.Abstracts;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ProductRepository: Repository<Product>, IProductRepository
    {
        public ProductRepository(IDbConnection connection):base(connection)
        {

        }


    }
}
