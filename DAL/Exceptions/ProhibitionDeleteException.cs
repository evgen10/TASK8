using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Exceptions
{
    /// <summary>
    /// Возникает при попытке напрямую удалить сущность со статусом "Completed"
    /// </summary>
    public class ProhibitionDeleteException : Exception
    {
        public ProhibitionDeleteException(string message) : base(message)
        {
        }
    }
}
