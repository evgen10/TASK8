using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Exceptions
{
    /// <summary>
    /// Возникает если отсутствует ключевое поле
    /// </summary>
    public class LackKeyFieldException : Exception
    {
        public LackKeyFieldException(string message) : base(message)
        {
        }
    }
}
