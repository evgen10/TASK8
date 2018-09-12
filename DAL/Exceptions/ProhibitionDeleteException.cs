using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Exceptions
{
    public class ProhibitionDeleteException : Exception
    {
        public ProhibitionDeleteException(string message) : base(message)
        {
        }
    }
}
