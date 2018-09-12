using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Exceptions
{
    public class ProhibitionUpdateException : Exception
    {
        public ProhibitionUpdateException(string message) : base(message)
        {
        }
    }
}
