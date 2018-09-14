using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Attributes;

namespace DAL.Exceptions
{
    /// <summary>
    /// Возникает при попытки изменения полей помеченныех атрибутом <see cref="UnchangeableAttribute"/>
    /// </summary>
    public class ProhibitionUpdateException : Exception
    {
        public ProhibitionUpdateException(string message) : base(message)
        {
        }
    }
}
