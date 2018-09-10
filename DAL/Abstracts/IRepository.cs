using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Abstracts
{
    public interface IRepository<T> where T: BaseEntity
    {
        IEnumerable<T> GetAll();
        void Create(T entity);
        void Delete(T entity);
        void Update(T entity);
        T Find(T entity);
    }
}
