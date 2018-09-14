using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DAL.Attributes
{
    /// <summary>
    ///  Атрибут указывает, что свойство не изменяется на прямую
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UnchangeableAttribute: Attribute
    {
    }
}
