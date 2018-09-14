using DAL.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALTests.Comparators
{
    class CustOrderDetailsComparator : IComparer
    {
        public int Compare(object x, object y)
        {

            CustOrderDetail custOrderDetail1 = (CustOrderDetail)x;
            CustOrderDetail custOrderDetail2 = (CustOrderDetail)y;

            return custOrderDetail1.Discount == custOrderDetail2.Discount &&
                   custOrderDetail1.ExtendedPrice == custOrderDetail2.ExtendedPrice &&
                   custOrderDetail1.ProductName == custOrderDetail2.ProductName &&
                   custOrderDetail1.Quantity == custOrderDetail2.Quantity ? 0 : 1;




        }
    }
}
