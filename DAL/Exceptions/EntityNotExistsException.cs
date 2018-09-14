using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Exceptions
{
    /// <summary>
    /// Возникает если сущность не найдена в базе данных
    /// </summary>
    public class EntityNotExistsException : Exception
    {
        public EntityNotExistsException(string message) : base(message)
        {
        }
    }
}
