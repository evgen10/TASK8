using DAL.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALTests.Comparators
{
    class OrderHistoryComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            OrderHistory ordHist1 = (OrderHistory)x;
            OrderHistory ordHist2 = (OrderHistory)y;

            return ordHist1.ProductName == ordHist2.ProductName &&
                   ordHist1.Total == ordHist2.Total ? 0 : 1;

        }
    }
}
