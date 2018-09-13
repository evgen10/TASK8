using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALTests.Comparators
{
    class OrderComparator
    {
        public bool Compare(Order order1, Order order2)
        {
            return order1.CustomerID     == order2.CustomerID &&
                   order1.EmployeeID     == order2.EmployeeID &&
                   order1.Freight        == order2.Freight &&
                   order1.OrderDate      == order2.OrderDate &&
                   order1.OrderID        == order2.OrderID &&
                   order1.OrderStatus    == order2.OrderStatus &&
                   order1.RequiredDate   == order2.RequiredDate &&
                   order1.ShipAddress    == order2.ShipAddress &&
                   order1.ShipCity       == order2.ShipCity &&
                   order1.ShipCountry    == order2.ShipCountry &&
                   order1.ShipName       == order2.ShipName &&
                   order1.ShippedDate    == order2.ShippedDate &&
                   order1.ShipPostalCode == order2.ShipPostalCode &&
                   order1.ShipRegion     == order2.ShipRegion &&
                   order1.ShipVia        == order2.ShipVia;
        }

    }
}
