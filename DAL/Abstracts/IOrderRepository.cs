using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Abstracts
{
    public interface IOrderRepository: IRepository<Order>
    {
        OrderNomenclature GetOrderNomenclature(int orderId);
           

    }
}
