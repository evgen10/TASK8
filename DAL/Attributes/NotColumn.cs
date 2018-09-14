using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DAL.Attributes
{
    /// <summary>
    /// Атрибут указывает, что свойство не имеет аналогов в таблице
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true) ]
    class NotColumnAttribute: Attribute
    {


    }
}
