using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALTests.Comparators
{
    public class OrderNomenclatureComparator
    {
        public bool Comparer(OrderNomenclature orderNum1, OrderNomenclature orderNum2)
        {
            return orderNum1.CustomerID == orderNum2.CustomerID &&
                   orderNum1.EmployeeID == orderNum2.EmployeeID &&
                   orderNum1.Freight == orderNum2.Freight &&
                   orderNum1.OrderDate == orderNum2.OrderDate &&
                   orderNum1.OrderID == orderNum2.OrderID &&
                   orderNum1.OrderStatus == orderNum2.OrderStatus &&
                   orderNum1.RequiredDate == orderNum2.RequiredDate &&
                   orderNum1.ShipAddress == orderNum2.ShipAddress &&
                   orderNum1.ShipCity == orderNum2.ShipCity &&
                   orderNum1.ShipCountry == orderNum2.ShipCountry &&
                   orderNum1.ShipName == orderNum2.ShipName &&
                   orderNum1.ShippedDate == orderNum2.ShippedDate &&
                   orderNum1.ShipPostalCode == orderNum2.ShipPostalCode &&
                   orderNum1.ShipRegion == orderNum2.ShipRegion &&
                   orderNum1.ShipVia == orderNum2.ShipVia &&
                   orderNum1.ProductID == orderNum2.ProductID &&
                   orderNum1.ProductName == orderNum2.ProductName &&
                   orderNum1.UnitPrice == orderNum2.UnitPrice &&
                   orderNum1.Quantity == orderNum2.Quantity &&
                   orderNum1.Discount == orderNum2.Discount;
                   


        }

    }
}
